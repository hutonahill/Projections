// -----------------------------------------------------------------------------
// Project: Projections
// Copyright (c) 2025
// Author: Evan Riker
// GitHub Account: hutonahill
// Email: evan.k.riker@gmail.com
// 
// License: GNU General Public License
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3 as published by
// the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
// -----------------------------------------------------------------------------

namespace Projections;

using System.Collections;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// A dictionary wrapper that exposes the contents of an underlying 
/// <see cref="IDictionary{TKey,TSource}"/> as a projected 
/// <see cref="IDictionary{TKey,TValue}"/> using conversion functions.
/// <br/>
/// This allows callers to interact with the dictionary using values of type 
/// <typeparamref name="TValue"/>, while internally the data is stored in 
/// <typeparamref name="TSource"/> form.
/// </summary>
/// <typeparam name="TKey">
/// The type of the dictionary keys.
/// </typeparam>
/// <typeparam name="TValue">
/// The projected value type visible through this dictionary.
/// </typeparam>
/// <typeparam name="TSource">
/// The underlying value type stored in the wrapped dictionary.
/// </typeparam>
public class ProjectionDictionary<TKey, TValue, TSource> : IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue> {
    private readonly Func<TSource, TValue> _toValue;
    
    private readonly Func<TValue, TSource> _toSource;
    
    private readonly IDictionary<TKey, TSource> _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionDictionary{TKey, TValue, TSource}"/> class.
    /// </summary>
    /// <param name="source">The source dictionary this is projecting.</param>
    /// <param name="projector">a converter for converting between the two.</param>
    public ProjectionDictionary(IDictionary<TKey, TSource> source, Projector<TSource, TValue> projector) {
        _source = source;
        
        _toValue = s => (TValue)projector.ConvertToValue(s!)!;
        _toSource = v => (TSource)projector.ConvertToSource(v!)!;
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
        foreach (KeyValuePair<TKey, TSource> keyValuePair in _source) {
            yield return new KeyValuePair<TKey, TValue>(keyValuePair.Key, _toValue(keyValuePair.Value));
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<TKey, TValue> item) {
        _source.Add(item.Key, _toSource(item.Value));
    }

    /// <inheritdoc/>
    public void Clear() {
        _source.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(KeyValuePair<TKey, TValue> item) {
        return _source.Contains(new KeyValuePair<TKey, TSource>(item.Key, _toSource(item.Value)));
    }

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
        if (array == null) {
            throw new ArgumentNullException(nameof(array));
        }
        
        if (arrayIndex < 0) {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }
        
        if (array.Length - arrayIndex < _source.Count) {
            throw new ArgumentException("The target array is too small.", nameof(array));
        }
        
        int i = arrayIndex;
        foreach (KeyValuePair<TKey, TSource> pair in _source) {
            array[i++] = new KeyValuePair<TKey, TValue>(pair.Key, _toValue(pair.Value));
        }
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<TKey, TValue> item) {
        if (!_source.TryGetValue(item.Key, out TSource? sourceValue)) {
            return false;
        }
        
        TValue current = _toValue(sourceValue);
        
        if (!EqualityComparer<TValue>.Default.Equals(current, item.Value)) {
            return false;
        }
        
        return _source.Remove(item.Key);
    }

    /// <inheritdoc/>
    int ICollection<KeyValuePair<TKey, TValue>>.Count => _source.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => _source.IsReadOnly;

    /// <inheritdoc/>
    public void Add(TKey key, TValue value) {
        _source.Add(key, _toSource(value));
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.ContainsKey(TKey key) {
        return _source.ContainsKey(key);
    }

    /// <inheritdoc/>
    bool IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        bool output = _source.TryGetValue(key, out TSource? sourceValue);
        if (sourceValue == null) {
            value = default;
            return output;
        }
        
        value = _toValue(sourceValue);
        return output;
    }

    /// <inheritdoc/>
    public bool Remove(TKey key) {
        return _source.Remove(key);
    }

    /// <inheritdoc/>
    bool IReadOnlyDictionary<TKey, TValue>.ContainsKey(TKey key) {
        return _source.ContainsKey(key);
    }

    /// <inheritdoc/>
    bool IDictionary<TKey, TValue>.TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
        bool output = _source.TryGetValue(key, out TSource? sourceValue);
        if (sourceValue == null) {
            value = default;
            return output;
        }
        
        value = _toValue(sourceValue);
        return output;
    }
    
    /// <inheritdoc cref = "IDictionary{TKey,TValue}" />
    public TValue this[TKey key] {
        get {
            TSource sourceValue = _source[key];
            return _toValue(sourceValue);
        }
        
        set => _source[key] = _toSource(value);
    }

    /// <inheritdoc/>
    IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => _source.Keys;

    /// <inheritdoc/>
    IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => _source.Values.Select(ts => _toValue(ts));

    /// <inheritdoc/>
    ICollection<TKey> IDictionary<TKey, TValue>.Keys => _source.Keys;

    /// <inheritdoc/>
    ICollection<TValue> IDictionary<TKey, TValue>.Values => _source.Values.Select(ts => _toValue(ts)).ToList();

    /// <inheritdoc/>
    int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.Count => _source.Count;
}
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

/// <summary>
/// A view of a list that allows it to be edited.
/// </summary>
/// <typeparam name="TSource">The internal, source type.</typeparam>
/// <typeparam name="TValue">The external, exposed type.</typeparam>
public class ProjectionList<TSource, TValue> : IList<TValue>, IReadOnlyList<TValue> {
    private readonly Func<TSource, TValue> _toValue;
    
    private readonly Func<TValue, TSource> _toSource;
    
    private readonly IList<TSource> _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionList{TSource, TValue}"/> class.
    /// </summary>
    /// <param name="source">The core data type.</param>
    /// <param name="projector">For converting to and from <see cref="TSource"/> and <see cref="TValue"/>.</param>
    public ProjectionList(IList<TSource> source, Projector<TSource, TValue> projector) {
        _source = source;
        
        // convert the value convert to to strongly typed versions.
        _toValue = s => (TValue)projector.ConvertToValue(s!)!;
        _toSource = v => (TSource)projector.ConvertToSource(v!)!;
    }
    
    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() {
        foreach (TSource sourceItem in _source) {
            yield return _toValue(sourceItem);
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return _source.GetEnumerator();
    }

    /// <inheritdoc/>
    public void Add(TValue item) {
        _source.Add(_toSource(item));
    }

    /// <inheritdoc/>
    void ICollection<TValue>.Clear() {
        _source.Clear();
    }

    /// <inheritdoc/>
    public bool Contains(TValue item) {
        return _source.Contains(_toSource(item));
    }

    /// <inheritdoc/>
    public void CopyTo(TValue[] array, int arrayIndex) {
        _source.CopyTo(array.Select(v => _toSource(v)).ToArray(), arrayIndex);
    }

    /// <inheritdoc/>
    public bool Remove(TValue item) {
        return _source.Remove(_toSource(item));
    }

    /// <inheritdoc/>
    int ICollection<TValue>.Count => _source.Count;

    /// <inheritdoc/>
    bool ICollection<TValue>.IsReadOnly => _source.IsReadOnly;

    /// <inheritdoc/>
    public int IndexOf(TValue item) {
        return _source.IndexOf(_toSource(item));
    }

    /// <inheritdoc/>
    public void Insert(int index, TValue item) {
        _source.Insert(index, _toSource(item));
    }

    /// <inheritdoc/>
    void IList<TValue>.RemoveAt(int index) {
        _source.RemoveAt(index);
    }

    /// <inheritdoc cref="IList{T}"/>
    public TValue this[int index] {
        get => _toValue(_source[index]);
        set => _source[index] = _toSource(value);
    }

    /// <inheritdoc/>
    public int Count => _source.Count;
}
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
/// A set designed to expose a modified value, instead of the source value.
/// <br/> Useful for manning private values, while exposing part of the value of a set.
/// </summary>
/// <typeparam name="TSource">The type of the internal source.</typeparam>
/// <typeparam name="TValue">The exposed type.</typeparam>
public class ProjectionSet<TSource, TValue> : ISet<TValue>, IReadOnlySet<TValue> {
    private readonly Func<TSource, TValue> _toValue;
    
    private readonly Func<TValue, TSource> _toSource;
    
    private readonly ISet<TSource> _source;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionSet{TSource, TValue}"/> class.
    /// </summary>
    /// <param name="source">The source set that is being wrapped.</param>
    /// <param name="projector">A converter that converts to and from the value of the source.</param>
    public ProjectionSet(ISet<TSource> source, Projector<TSource, TValue> projector) {
        _source = source;
        
        _toValue = s => (TValue)projector.ConvertToValue(s!)!;
        _toSource = v => (TSource)projector.ConvertToSource(v!)!;
    }

    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() {
        foreach (TSource source in _source) {
            yield return _toValue(source);
        }
    }
    
    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<TValue> other) {
        _source.ExceptWith(other.Select(tv => _toSource(tv)));
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<TValue> other) {
        _source.IntersectWith(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool Contains(TValue item) {
        return _source.Contains(_toSource(item));
    }

    /// <inheritdoc cref="ISet{T}"/>
    public bool IsProperSubsetOf(IEnumerable<TValue> other) {
        return _source.IsProperSubsetOf(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool IsProperSupersetOf(IEnumerable<TValue> other) {
        return _source.IsProperSupersetOf(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool IsSubsetOf(IEnumerable<TValue> other) {
        return _source.IsSubsetOf(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool IsSupersetOf(IEnumerable<TValue> other) {
        return _source.IsSupersetOf(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool Overlaps(IEnumerable<TValue> other) {
        return _source.Overlaps(other.Select(tv => _toSource(tv)));
    }
    
    /// <inheritdoc cref="ISet{T}"/>
    public bool SetEquals(IEnumerable<TValue> other) {
        return _source.SetEquals(other.Select(tv => _toSource(tv)));
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<TValue> other) {
        _source.SymmetricExceptWith(other.Select(tv => _toSource(tv)));
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<TValue> other) {
        _source.UnionWith(other.Select(tv => _toSource(tv)));
    }

    /// <inheritdoc/>
    public bool Add(TValue item) {
        return _source.Add(_toSource(item));
    }

    /// <inheritdoc/>
    void ICollection<TValue>.Add(TValue item) {
        Add(item);
    }
    
    /// <inheritdoc/>
    public void Clear() {
        _source.Clear();
    }

    /// <inheritdoc/>
    public void CopyTo(TValue[] array, int arrayIndex) {
        if (array == null) {
            throw new ArgumentNullException(nameof(array));
        }
        
        if (arrayIndex < 0) {
            throw new ArgumentOutOfRangeException(nameof(arrayIndex));
        }
        
        if (array.Length - arrayIndex < _source.Count) {
            throw new ArgumentException("Target array is too small.", nameof(array));
        }
        
        int i = arrayIndex;
        foreach (TSource source in _source) {
            array[i++] = _toValue(source);
        }
    }

    /// <inheritdoc/>
    public bool Remove(TValue item) {
        return _source.Remove(_toSource(item));
    }

    /// <inheritdoc/>
    public bool IsReadOnly => _source.IsReadOnly;
    
    /// <inheritdoc cref="ISet{T}"/>
    public int Count => _source.Count;

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
}
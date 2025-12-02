// -----------------------------------------------------------------------------
// License: All Rights Reserved
// Copyright (c) 2025
// Author: Evan Riker
// GitHub Account: hutonahill
// Email: evan.k.riker@gmail.com
// 
// The Mistborn Deckbuilding Game is the property of Brotherwise Games.
// The Mistborn setting and themes are the property of Dragonsteel Entertainment.
// 
// This code may not be used, copied, modified, or distributed without the
// express written permission of the copyright holder.
// 
// Any commercial use of this code additionally requires authorization and
// licensing from Dragonsteel Entertainment and Brotherwise Games.
// 
// Should the proper rights be acquired, this is provided "as is" without warranty of any kind.
// -----------------------------------------------------------------------------

namespace Projections;

using System.Collections;

/// <summary>
/// Displays the values of a set after they have been changed by a projector.
/// </summary>
/// <typeparam name="TSource">The internal value of the set.</typeparam>
/// <typeparam name="TValue">The external value of the set.</typeparam>
public class ProjectionSet<TSource, TValue> : IReadOnlySet<TValue> {
    private readonly ISet<TSource> _source;
    private readonly Func<TSource, TValue> _projector;
    private readonly IEqualityComparer<TValue> _comparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionSet{TSource, TValue}"/> class.
    /// </summary>
    /// <param name="source">The source set with the data.</param>
    /// <param name="projector">A projector to convert the internal type to an external type.</param>
    /// <param name="comparer">optional tool for comparing <see cref="TValue"/>s.</param>
    public ProjectionSet(
        ISet<TSource> source, 
        Func<TSource, TValue> projector,
        IEqualityComparer<TValue>? comparer = null
    ) {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _projector = projector ?? throw new ArgumentNullException(nameof(projector));
        _comparer = comparer ?? EqualityComparer<TValue>.Default;
    }

    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() {
        foreach (TSource value in _source) {
            yield return _projector(value);
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return _source.GetEnumerator();
    }

    /// <inheritdoc/>
    public int Count => _source.Count;

    /// <inheritdoc/>
    public bool Contains(TValue item) {
        foreach (TSource s in _source) {
            if (_comparer.Equals(_projector(s), item)) {
                return true;
            }
        }
        
        return false;
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        if (Count >= others.Count) {
            return false;
        }
        
        return IsSubsetOf(others);
    }

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        if (Count <= others.Count) {
            return false;
        }
        
        return IsSupersetOf(others);
    }

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        foreach (TSource s in _source) {
            if (!others.Contains(_projector(s))) {
                return false;
            }
        }
        
        return true;
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TValue> other) {
        foreach (TValue v in other) {
            if (!Contains(v)) {
                return false;
            }
        }
        
        return true;
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TValue> other) {
        foreach (TValue v in other) {
            if (Contains(v)) {
                return true;
            }
        }
        
        return false;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        if (others.Count != Count) {
            return false;
        }
        
        foreach (TSource s in _source) {
            if (!others.Contains(_projector(s))) {
                return false;
            }
        }
        
        return true;
    }
}
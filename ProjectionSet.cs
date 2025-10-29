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

using System.Collections;

namespace Projections;

public class ProjectionSet<TSource, TValue> : IReadOnlySet<TValue> 
{
    private readonly ISet<TSource> _source;
    private readonly Func<TSource, TValue> _projector;
    private readonly IEqualityComparer<TValue> _comparer;
    
    public ProjectionSet(ISet<TSource> source, Func<TSource, TValue> projector,
        IEqualityComparer<TValue>? comparer = null) {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _projector = projector ?? throw new ArgumentNullException(nameof(projector));
        _comparer = comparer ?? EqualityComparer<TValue>.Default;
    }

    public IEnumerator<TValue> GetEnumerator() {
        foreach (TSource value in _source) {
            yield return _projector(value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return _source.GetEnumerator();
    }

    public int Count => _source.Count;
    
    public bool Contains(TValue item) {
        foreach (TSource s in _source) {
            if (_comparer.Equals(_projector(s), item)) {
                return true;
            }
        }
        return false;
    }
    
    public bool IsProperSubsetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        if (Count >= others.Count) {
            return false;
        }
        return IsSubsetOf(others);
    }
    
    public bool IsProperSupersetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        if (Count <= others.Count) {
            return false;
        }
        return IsSupersetOf(others);
    }
    
    public bool IsSubsetOf(IEnumerable<TValue> other) {
        HashSet<TValue> others = new HashSet<TValue>(other, _comparer);
        foreach (TSource s in _source) {
            if (!others.Contains(_projector(s))) {
                return false;
            }
        }
        return true;
    }
    
    public bool IsSupersetOf(IEnumerable<TValue> other) {
        foreach (TValue v in other) {
            if (!Contains(v)) {
                return false;
            }
        }
        return true;
    }
    
    public bool Overlaps(IEnumerable<TValue> other) {
        foreach (TValue v in other) {
            if (Contains(v)) {
                return true;
            }
        }
        return false;
    }
    
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
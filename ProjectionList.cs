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
/// Displays a view of a list transformed by a projector.
/// </summary>
/// <typeparam name="TSource">The type of the source list.</typeparam>
/// <typeparam name="TValue">The displayed type.</typeparam>
public class ProjectionList<TSource, TValue>(IList<TSource> source, Func<TSource, TValue> projector)
    : IReadOnlyList<TValue> {
    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() {
        foreach (TSource source1 in source) {
            yield return projector(source1);
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
        return source.GetEnumerator();
    }

    /// <inheritdoc/>
    public int Count => source.Count;

    /// <inheritdoc/>
    public TValue this[int index] => projector(source[index]);
}
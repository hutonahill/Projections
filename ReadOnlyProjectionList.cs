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
/// Displays a view of a list transformed by a projector.
/// </summary>
/// <typeparam name="TSource">The type of the source list.</typeparam>
/// <typeparam name="TValue">The displayed type.</typeparam>
public class ReadOnlyProjectionList<TSource, TValue>(IList<TSource> source, Func<TSource, TValue> projector)
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
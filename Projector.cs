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

public class Projector<TSource, TValue> {
    public Func<TSource, TValue> ConvertToValue { get; }
    
    public Func<TValue, TSource> ConvertToSource { get; }
    
    public Projector(
        Func<TSource, TValue> toValue,
        Func<TValue, TSource> toSource
    ){
        ConvertToValue = toValue;
        ConvertToSource = toSource;
    }
}
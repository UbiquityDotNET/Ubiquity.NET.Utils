// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Utility class to extend support for an <see cref="Interval"/></summary>
    public static class IntervalExtensions
    {
        /// <summary>Support C# Deconstruction for an <see cref="Interval"/></summary>
        /// <param name="interval"><see cref="Interval"/> to deconstruct</param>
        /// <param name="lowerBound">The lower bound value</param>
        /// <param name="upperBound">The upper bound value</param>
        public static void Deconstruct( this Interval interval, out int lowerBound, out int upperBound )
        {
            lowerBound = interval.a;
            upperBound = interval.b;
        }
    }
}

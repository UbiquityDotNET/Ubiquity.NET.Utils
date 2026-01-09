// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Utility class to provide extensions for an <see cref="ImmutableArray"/></summary>
    public static class ImmutableArrayExtensions
    {
        /// <summary>Formats an array as a string</summary>
        /// <typeparam name="T">Type of the elements of the array</typeparam>
        /// <param name="self">array to format</param>
        /// <returns>formatted form of the array</returns>
        /// <remarks>
        /// The <see cref="Object.ToString()"/> method on <see cref="ImmutableArray"/>
        /// will only report the type of the array, not the contents. This will
        /// display the contents of the array. If the elements don't have an overloaded
        /// <see cref="Object.ToString()"/> then it will only show the type name for each
        /// element.
        /// </remarks>
        public static string Format<T>(this ImmutableArray<T> self)
        {
            return $"[{string.Join(", ", self)}]";
        }
    }
}

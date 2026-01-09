// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Utility class to provide extensions for <see cref="TypedConstant"/></summary>
    public static class TypedConstantExtensions
    {
        /// <summary>Tests if a <see cref="TypedConstant"/> is a non null array</summary>
        /// <param name="self">The constant to test</param>
        /// <returns>true if <paramref name="self"/> is not null and an array</returns>
        public static bool IsNonNullArray( this TypedConstant self)
        {
            return !self.IsNull
                && self.Kind == TypedConstantKind.Array;
        }
    }
}

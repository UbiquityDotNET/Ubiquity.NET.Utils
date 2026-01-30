// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Utility class to support extensions relating to source locations</summary>
    public static class TerminalNodeExtensions
    {
        /// <summary>Gets the <see cref="SourceLocation"/> from an <see cref="ITerminalNode"/></summary>
        /// <param name="node">Node to get the location from</param>
        /// <returns><see cref="SourceLocation"/> of the node</returns>
        public static SourceLocation GetSourceLocation( this ITerminalNode node )
        {
            return node.Symbol.GetSourceLocation();
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Interface for a parse result</summary>
    /// <typeparam name="TParser">Type of the parser that produced the results</typeparam>
    /// <typeparam name="TTree">Type of the resulting parse tree</typeparam>
    public interface IParseResult<TParser, TTree>
        where TParser : Parser
        where TTree : IParseTree
    {
        /// <summary>Gets the result of parsing as a tree</summary>
        TTree Tree { get; }

        /// <summary>Gets the parser that produced <see cref="Tree"/></summary>
        TParser Parser { get; }
    }
}

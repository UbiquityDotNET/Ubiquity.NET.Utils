// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Generic base type for a parse result</summary>
    /// <typeparam name="TParser">Type of Parser the result comes from</typeparam>
    /// <typeparam name="TTree">Type of the resulting parse tree</typeparam>
    public class ParseResultBase<TParser, TTree>
        : IParseResult<TParser, TTree>
        where TParser : Parser
        where TTree : IParseTree
    {
        /// <summary>Initializes a new instance of the <see cref="ParseResultBase{TParser, TTree}"/> class</summary>
        /// <param name="parseTree">Result tree</param>
        /// <param name="parser">Parser that produced the tree</param>
        public ParseResultBase( TTree parseTree, TParser parser )
        {
            Tree = parseTree;
            Parser = parser;
        }

        /// <inheritdoc/>
        public TTree Tree { get; /*init;*/ }

        /// <inheritdoc/>
        public TParser Parser { get; /*init;*/ }
    }
}

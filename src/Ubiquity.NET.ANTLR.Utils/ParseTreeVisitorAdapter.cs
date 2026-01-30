// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.
#if DELETE_ME_LATER
namespace Ubiquity.NET.ANTLR.Utils
{
    internal class ParseTreeVisitorAdapter<TResult>
        : ISyntaxTreeVisitor<TResult>
    {
        public ParseTreeVisitorAdapter( IParseTreeVisitor<TResult> parseTreeVisitor )
        {
            ParseTreeVisitor = parseTreeVisitor;
        }

        public TResult? Visit( ISyntaxNode node )
        {
            return node is IParseTree tree
                ? ParseTreeVisitor.Visit(tree)
                : default;
        }

        private readonly IParseTreeVisitor<TResult> ParseTreeVisitor;
    }
}
#endif

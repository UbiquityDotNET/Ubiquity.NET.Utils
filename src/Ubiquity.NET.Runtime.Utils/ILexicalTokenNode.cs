// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary><see cref="IAstNode"/> for a lexical token with token kind</summary>
    public interface ILexicalTokenNode
        : ISyntaxNode
    {
        /// <summary>Gets the kind of element this node represents</summary>
        int TokenKind { get; }

        // Constraints not expressible in language:
        // 1) Children != null
        // 2) Children.Count == 0 && Children.Any() == false
    }
}

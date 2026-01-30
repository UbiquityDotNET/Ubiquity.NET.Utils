// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary><see cref="ISyntaxNode"/> wrapper for a Lexical token</summary>
    public class TokenNode
        : SyntaxNodeBase
        , ILexicalTokenNode
    {
        /// <summary>Initializes a new instance of the <see cref="TokenNode"/> class</summary>
        /// <param name="src">Token source this node is from</param>
        /// <param name="location">Location of the token</param>
        /// <remarks>
        /// This form is generally used for error reporting to indicate the location
        /// and source of a lexical error. In such a case no valid token exists.
        /// </remarks>
        public TokenNode( ITokenSource src, SourceLocation location )
            : base( location )
        {
            Token = null; // no token for this node
            TokenSource = src;
        }

        /// <summary>Initializes a new instance of the <see cref="TokenNode"/> class</summary>
        /// <param name="token">Token for this node</param>
        /// <remarks>
        /// This form uses the location from the token itself, assuming the location
        /// of the token in the source is the true location.
        /// </remarks>
        public TokenNode( IToken token )
            : this( token, token.GetSourceLocation() )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="TokenNode"/> class</summary>
        /// <param name="token">Token for this node</param>
        /// <param name="location">Location of the token</param>
        /// <remarks>
        /// This form of constructor is used when the actual location is different from the
        /// location from the token. This happens when the lexer the token is from comes
        /// from lexical pass on a sub string in a larger document. In such cases the location
        /// in the larger document is desired and is different from that of the token in the
        /// sub range of the full document.
        /// </remarks>
        public TokenNode( IToken token, SourceLocation location )
            : base( location )
        {
            Token = token;
            TokenSource = token.TokenSource;
        }

        /// <inheritdoc/>
        public int TokenKind => Token?.Type ?? 0;

        /// <summary>Gets the <see cref="ITokenSource"/> that this node is from</summary>
        public ITokenSource TokenSource { get; }

        /// <summary>Gets the token for this node or <see langword="null"/> if no token available</summary>
        public IToken? Token { get; }
    }
}

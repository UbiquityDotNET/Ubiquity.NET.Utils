// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Extensions for an <see cref="IParseTree"/></summary>
    public static class ParseTreeExtensions
    {
        /// <summary>Gets and enumerable collection of children for an <see cref="IParseTree"/></summary>
        /// <param name="tree">Tree to get children from</param>
        /// <returns>Enumerable of all of the children in the tree</returns>
        public static IEnumerable<IParseTree> GetChildren( this IParseTree tree )
        {
            for(int i = 0; i < tree.ChildCount; ++i)
            {
                yield return tree.GetChild( i );
            }
        }

        /// <summary>Gets a SyntaxNode from a parse tree</summary>
        /// <param name="tree">Parse tree</param>
        /// <returns><see cref="ISyntaxNode"/> for the input <see cref="IParseTree"/></returns>
        /// <exception cref="ApplicationException">Invalid <paramref name="tree"/></exception>
        /// <remarks>
        /// The formal documentation for an <see cref="IParseTree"/> is that the <see cref="ITree.Payload"/>
        /// property is either <see cref="IToken"/> or <see cref="RuleContext"/>. This depends on that
        /// by testing the type of Payload and using the payload itself to produce the resulting
        /// <see cref="ISyntaxNode"/>.
        /// </remarks>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Result is NOT simpler" )]
        public static ISyntaxNode AsSyntaxNode( this IParseTree tree )
        {
            if(tree.Payload is IToken token)
            {
                return new TokenNode( token );
            }
            else if(tree.Payload is RuleContext rule)
            {
                return new RuleContextNode( rule );
            }
            else
            {
                throw new NotSupportedException( $"Unexpected payload type: {tree.Payload.GetType().Name}" );
            }
        }

        /// <summary>Get the source location for an <see cref="IParseTree"/> instance</summary>
        /// <param name="tree">Tree to get the location from</param>
        /// <returns>Location from the tree</returns>
        /// <exception cref="ArgumentException"><see cref="ITree.Payload"/> for <paramref name="tree"/> is invalid</exception>
        /// <remarks>
        /// The formal documentation for an <see cref="IParseTree"/> is that the <see cref="ITree.Payload"/>
        /// property is either <see cref="IToken"/> or <see cref="RuleContext"/>. This depends on that
        /// by testing the type of Payload and using the payload itself to produce the resulting
        /// location.
        /// </remarks>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Result is NOT simpler" )]
        public static SourceLocation GetSourceLocation( this IParseTree tree )
        {
            if(tree.Payload is IToken token)
            {
                return token.GetSourceLocation();
            }
            else if(tree.Payload is RuleContext rule)
            {
                return rule.GetSourceLocation();
            }
            else
            {
                throw new NotSupportedException( $"Unexpected payload type: {tree.Payload.GetType().Name}" );
            }
        }
    }
}

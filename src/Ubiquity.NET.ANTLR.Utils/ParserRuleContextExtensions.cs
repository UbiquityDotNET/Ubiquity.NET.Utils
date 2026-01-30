// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Extension methods for a <see cref="ParserRuleContext"/></summary>
    public static class ParserRuleContextExtensions
    {
        /// <summary>Gets the source name from a <see cref="ParserRuleContext"/></summary>
        /// <param name="ctx">Rule to get the source from</param>
        /// <returns>Source name from the stream that the rules start token came from</returns>
        public static string GetSourceFileName( this ParserRuleContext ctx )
        {
            return ctx.Start.GetSourceFileName();
        }

        /// <summary>Gets the <see cref="SourceLocation"/> from a <see cref="ParserRuleContext"/></summary>
        /// <param name="ctx">Context to get the location from</param>
        /// <returns>Source Location of the context</returns>
        public static SourceLocation GetSourceLocation( this ParserRuleContext ctx )
        {
            SourceLocation startLoc = ctx.Start.GetSourceLocation();
            SourceLocation endLoc = ctx.Stop.GetSourceLocation();
            return startLoc.Source != endLoc.Source
                ? throw new ArgumentException( "Mismatched source for start and end tokens!", nameof( ctx ) )
                : new SourceLocation( ctx.Start.InputStream.SourceName, startLoc.Range );
        }

        /// <summary>Determines if a <see cref="ParserRuleContext"/> spans multiple lines</summary>
        /// <param name="ctx">Context to test</param>
        /// <returns><see langword="true"/> if <paramref name="ctx"/> spans multiple lines</returns>
        public static bool IsMultiLine( this ParserRuleContext ctx )
        {
            return ctx.Start.Line < ctx.Stop.Line;
        }

        /// <summary>Gets an enumerable from the children of a <see cref="ParserRuleContext"/></summary>
        /// <param name="ctx">Context to get children from</param>
        /// <returns>A non-null, but possibly empty enumeration of children</returns>
        public static IList<IParseTree> GetChildren( this ParserRuleContext ctx )
        {
            return ctx.children == null || ctx.ChildCount == 0
                 ? []
                 : ctx.children;
        }

        /// <summary>Gets an enumerable of the children of a <see cref="ParserRuleContext"/> of the specified type</summary>
        /// <typeparam name="T">Type of children to provide an enumerable for</typeparam>
        /// <param name="ctx">Context to get the children from</param>
        /// <returns>Non-null, but possibly empty enumeration of children matching the type of <typeparamref name="T"/></returns>
        public static IEnumerable<T> GetChildren<T>( this ParserRuleContext ctx )
        {
            return ctx.GetChildren().OfType<T>();
        }

        /// <summary>Retrieves the index, in the source <see cref="ICharStream"/>, of the first character of <paramref name="ctx"/></summary>
        /// <param name="ctx">Context to get the source index of</param>
        /// <returns>First character index</returns>
        /// <remarks>
        /// <note type="Important">
        /// It is important to note that ANTLR does NOT guarantee the <see cref="ParserRuleContext.Start"/> and
        /// <see cref="ParserRuleContext.Stop"/> are ordered! That is the stop token may indicate a position in the input
        /// that precedes that of the start token.
        /// </note>
        /// This method provides guarantees on the ordering internally and provides the first character index in input stream index order.
        /// </remarks>
        public static int FirstCharIndex( this ParserRuleContext ctx )
        {
            return Math.Min( ctx.Start.StartIndex, ctx.Stop.StopIndex );
        }

        /// <summary>Retrieves the index, in the source <see cref="ICharStream"/>, of the last character of <paramref name="ctx"/></summary>
        /// <param name="ctx">Context to get the source index of</param>
        /// <returns>Last character index</returns>
        /// <remarks>
        /// <note type="Important">
        /// It is important to note that ANTLR does NOT guarantee the <see cref="ParserRuleContext.Start"/> and
        /// <see cref="ParserRuleContext.Stop"/> are ordered! That is the stop token may indicate a position in the input
        /// that precedes that of the start token.
        /// </note>
        /// This method provides guarantees on the ordering internally and provides the last character index in input stream index order.
        /// </remarks>
        public static int LastCharIndex( this ParserRuleContext ctx )
        {
            return Math.Max( ctx.Start.StartIndex, ctx.Stop.StopIndex );
        }

        /// <summary>Retrieves the length, in the source <see cref="ICharStream"/>, of all characters of <paramref name="ctx"/></summary>
        /// <param name="ctx">Context to get the length of</param>
        /// <returns>Length of the input (character count) covered by the context</returns>
        /// <remarks>
        /// <note type="Important">
        /// It is important to note that ANTLR does NOT guarantee the <see cref="ParserRuleContext.Start"/> and
        /// <see cref="ParserRuleContext.Stop"/> are ordered! That is the stop token may indicate a position in the input
        /// that precedes that of the start token.
        /// </note>
        /// This method provides guarantees on the ordering internally and provides the length of the input text (including skipped or "off channel" tokens).
        /// </remarks>
        public static int CharLength( this ParserRuleContext ctx )
        {
            return ctx.LastCharIndex() - ctx.FirstCharIndex() + 1;
        }

        /// <summary>Gets the full source text for a <see cref="ParserRuleContext"/></summary>
        /// <param name="ctx">Context to get the source from</param>
        /// <returns>Full source text for the context (including any text hidden/skipped by the lexer)</returns>
        public static string GetSourceText( this ParserRuleContext ctx )
        {
            return ctx.Start.InputStream.GetText( new Interval( ctx.FirstCharIndex(), ctx.LastCharIndex() ) );
        }
    }
}

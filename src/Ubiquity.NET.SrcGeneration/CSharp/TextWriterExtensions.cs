// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.SrcGeneration.CSharp
{
    /// <summary>Utility extensions for a <see cref="TextWriter"/> specific to the C# language</summary>
    [SuppressMessage( "Performance", "CA1822:Mark members as static", Justification = "extension" )]
    [SuppressMessage( "Design", "CA1034:Nested types should not be visible", Justification = "extension" )]
    public static class TextWriterExtensions
    {
        /// <summary>Writes an attribute as a line</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="attribArgs">arguments for the attribute</param>
        public static void WriteAttributeLine( this TextWriter self, string attributeName, params string[] attribArgs )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentException.ThrowIfNullOrWhiteSpace(attributeName);
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace(attributeName);
#endif
            self.WriteAttribute( attributeName, attribArgs );
            self.WriteLine();
        }

        /// <summary>Writes an attribute to the specified writer</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="attributeName">Name of the attribute</param>
        /// <param name="attribArgs">arguments for the attribute</param>
        public static void WriteAttribute(this TextWriter self, string attributeName, params string[] attribArgs )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentException.ThrowIfNullOrWhiteSpace( attributeName );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace(attributeName);
#endif
            self.Write( $"[{attributeName}" );
            if(attribArgs.Length > 0)
            {
                self.Write( $"({string.Join( ", ", attribArgs )})" );
            }

            self.Write( "]" );
        }

        /// <summary>Writes an XML Doc comment summary</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="description">Text to include in the summary (Nothing is written if this is <see langword="null"/> or all whitespace </param>
        public static void WriteSummaryComment(this TextWriter self, string? description )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
#endif

            if(!string.IsNullOrWhiteSpace( description ))
            {
                self.WriteLine( $"/// <summary>{description!.Trim()}</summary>" );
            }
        }

        /// <summary>Writes remarks comment (XML Doc comment)</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="txt">Text for the remarks</param>
        /// <remarks>
        /// If <see cref="string.IsNullOrWhiteSpace(string?)"/> for <paramref name="txt"/> is true
        /// then nothing is generated.
        /// </remarks>
        public static void WriteRemarksComment( this TextWriter self, string? txt )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
#endif

            if(string.IsNullOrWhiteSpace( txt ))
            {
                return;
            }

            string[] lines = [ .. txt!.GetCommentLines() ];
            if(lines.Length > 0)
            {
                self.WriteLine( "/// <remarks>" );
                foreach(string line in lines)
                {
                    self.WriteLine( $"/// {line}" );
                }

                self.WriteLine( "/// </remarks>" );
            }
        }

        /// <summary>Writes summary and remarks comment (XML Doc comment)</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="txt">Text for the remarks</param>
        /// <param name="defaultSummary">Default summary text to use if <paramref name="txt"/> does not contain any</param>
        /// <remarks>
        /// If <see cref="string.IsNullOrWhiteSpace(string?)"/> for <paramref name="txt"/> is true
        /// then nothing is generated. If <paramref name="txt"/> has no content then <paramref name="defaultSummary"/>
        /// is used as the summary. If <paramref name="defaultSummary"/> is also empty or all Whitespace then nothing
        /// is output.
        /// </remarks>
        public static void WriteSummaryAndRemarksComments( this TextWriter self, string? txt, string? defaultSummary = null )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
#endif

            if(string.IsNullOrWhiteSpace( txt ))
            {
                if(!string.IsNullOrWhiteSpace( defaultSummary ))
                {
                    self.WriteLine( $"/// <summary>{defaultSummary!.Trim()}</summary>" );
                }

                return;
            }

#if NET8_0_OR_GREATER
            ArgumentException.ThrowIfNullOrWhiteSpace( defaultSummary );
#else
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace( defaultSummary );
#endif

            self.WriteLine( $"/// <summary>{defaultSummary.Trim()}</summary>" );
            string[] lines = [ .. txt!.GetCommentLines() ];
            if(lines.Length > 0)
            {
                // summary + remarks.
                self.WriteLine( "/// <remarks>" );
                for(int i = 0; i < lines.Length; ++i)
                {
                    self.Write( "/// " );
                    self.WriteLine( lines[ i ] );
                }

                self.WriteLine( "/// </remarks>" );
            }
        }

        /// <summary>Writes a C# using directive (namespace reference)</summary>
        /// <param name="self">The writer to write to</param>
        /// <param name="namespaceName">Namespace for the using directive</param>
        public static void WriteUsingDirective(this TextWriter self, string namespaceName )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentException.ThrowIfNullOrWhiteSpace(namespaceName);
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace(namespaceName);
#endif

            self.WriteLine( $"using {namespaceName};" );
        }

        /// <summary>Write an empty single line comment</summary>
        /// <param name="self">Writer to write the line to</param>
        public static void WriteEmptyCommentLine(this TextWriter self)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
#endif

            self.WriteLine("//");
        }

        /// <summary>Writes a single line comment to <paramref name="self"/></summary>
        /// <param name="self"><see cref="TextWriter"/> to write the comment to</param>
        /// <param name="comment">Contents of the comment</param>
        /// <exception cref="FormatException"><paramref name="comment"/> contains a new line</exception>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="comment"/> is null</exception>
        public static void WriteCommentLine( this TextWriter self, string comment)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentNullException.ThrowIfNull( comment );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNull( comment );
#endif
            if(comment.HasLineEndings())
            {
                throw new FormatException( "Single line comments cannot contain line endings" );
            }

            InternalWriteComment( self, comment );
        }

#if !NET9_0_OR_GREATER
        /// <summary>Writes a sequence of single line comments</summary>
        /// <param name="self">Writer to write the comments to</param>
        /// <param name="commentLines">Lines to write</param>
        /// <exception cref="FormatException">An element of the <paramref name="commentLines"/> sequence contains a new line</exception>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="commentLines"/> is null</exception>
        public static void WriteCommentLines( this TextWriter self, params string[] commentLines)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentNullException.ThrowIfNull( commentLines );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNull( commentLines );
#endif
            WriteCommentLines(self, (IEnumerable<string>)commentLines);
        }
#endif

        /// <summary>Writes a sequence of single line comments</summary>
        /// <param name="self">Writer to write the comments to</param>
        /// <param name="commentLines">Lines to write</param>
        /// <exception cref="FormatException">An element of the <paramref name="commentLines"/> sequence contains a new line</exception>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="commentLines"/> is null</exception>
#if NET9_0_OR_GREATER
        public static void WriteCommentLines( this TextWriter self, params IEnumerable<string> commentLines)
#else
        public static void WriteCommentLines( this TextWriter self, IEnumerable<string> commentLines)
#endif
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentNullException.ThrowIfNull( commentLines );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNull( commentLines );
#endif

            foreach(string comment in commentLines)
            {
                // comment MAY contain a new line, so go through checking method.
                WriteCommentLine(self, comment);
            }
        }

        /// <summary>Writes a block of text as single line comments</summary>
        /// <param name="self">Writer to write the comment(s) to</param>
        /// <param name="commentTextBlock">Block of text that may contain new lines</param>
        /// <remarks>
        /// The input <paramref name="commentTextBlock"/> is split on any existing line endings
        /// and a single line comment is emitted for each line found. If no new lines are present
        /// then the entire string is emitted as a single line comment.
        /// </remarks>
        public static void WriteCommentLines( this TextWriter self, string commentTextBlock)
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
            ArgumentNullException.ThrowIfNull( commentTextBlock );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
            PolyFillExceptionValidators.ThrowIfNull( commentTextBlock );
#endif

            // SplitLines already handles the new lines so optimize by doing a simple write
            foreach(string comment in commentTextBlock.SplitLines(StringSplitOptions2.TrimEntries))
            {
               InternalWriteComment(self, comment);
            }
        }

        private static void InternalWriteComment( TextWriter self, string comment )
        {
            self.Write( "// " );
            self.WriteLine( comment );
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections;

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Common Implementation of <see cref="IParseErrorList"/> specific to ANTLR parsers/lexers</summary>
    public class ParseErrorList
        : IReadOnlyList<DiagnosticMessage>
        , IParseErrorList
    {
        /// <summary>Initializes a new instance of the <see cref="ParseErrorList"/> class</summary>
        /// <param name="sourceName">Source name for all errors</param>
        /// <param name="severityMap">Severity map to use when transforming errors</param>
        /// <param name="formatter">Formatter to use to form a string version of the error code</param>
        /// <exception cref="ArgumentException"><paramref name="sourceName"/> is null or whitespace</exception>
        public ParseErrorList( string sourceName, IMessageLevelMap severityMap, IDiagnosticIdFormatter formatter )
            : this( new SourceLocation( sourceName ), // default location has position of (0,0) so all errors are effectively "absolute"
                    severityMap,
                    formatter
                  )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ParseErrorList"/> class</summary>
        /// <param name="baseLocation">Base location for all errors</param>
        /// <param name="severityMap">Severity map to use when transforming errors</param>
        /// <param name="formatter">Formatter to use to form a string version of the error code</param>
        /// <remarks>
        /// This constructor is generally used for post parse validation via a sub language parser
        /// [or perhaps a specific rule of the same parser]. The base location is used to compute
        /// a final location for any errors which are treated as relative to this location.
        /// </remarks>
        public ParseErrorList(
            SourceLocation baseLocation,
            IMessageLevelMap severityMap,
            IDiagnosticIdFormatter formatter
            )
        {
            BaseLocation = baseLocation;
            SeverityMap = severityMap;
            IdFormatter = formatter;
        }

        /// <summary>Gets the base location for all errors</summary>
        /// <remarks>
        /// This location is offset by any posted error positions. That is, the posted error positions
        /// are relative to this location.
        /// </remarks>
        public SourceLocation BaseLocation { get; init; }

        /// <summary>Gets the severity mapping used when forming a diagnostic</summary>
        public IMessageLevelMap SeverityMap { get; init; }

        /// <summary>Gets the functor used to format an integral id to a string form</summary>
        public IDiagnosticIdFormatter IdFormatter { get; init; }

        /// <inheritdoc/>
        public int Count => InnerList.Count;

        /// <inheritdoc/>
        public DiagnosticMessage this[ int index ] => InnerList.Messages[ index ];

        /// <inheritdoc/>
        public IEnumerator<DiagnosticMessage> GetEnumerator( )
        {
            return InnerList.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator( )
        {
            return InnerList.GetEnumerator();
        }

        /// <inheritdoc/>
        public void SyntaxError( TextWriter output // ignored, errors are collected not formatted for any output
                               , IRecognizer recognizer
                               , int offendingSymbol // ignored, always 0 for lexer errors
                               , int line
                               , int charPositionInLine
                               , string msg
                               , RecognitionException e
                               )
        {
            ArgumentNullException.ThrowIfNull(recognizer);

            // -1 for the index since, in ANTLR parsing, the current index is one past the source of the error!
            int srcIndex = (recognizer?.InputStream?.Index ?? 1) - 1;

            // +1 for column as SourceLocation uses a 1 based column, and ANTLR uses a 0 based
            int srcCol = charPositionInLine + 1;

            var loc = new SourceLocation(recognizer!.InputStream.SourceName, new SourcePosition(line, srcCol, srcIndex));
            var scopedCode = new ScopedDiagnosticId(ParseSource.Lexical, recognizer.State);
            var diagnostic = scopedCode.AsDiagnostic(SeverityMap, IdFormatter, msg, loc);
            InnerList.Report( diagnostic );
        }

        /// <inheritdoc/>
        public void SyntaxError( TextWriter output // ignored, errors are collected not formatted for any output
                               , IRecognizer recognizer
                               , IToken offendingSymbol
                               , int line
                               , int charPositionInLine
                               , string msg
                               , RecognitionException e
                               )
        {
            var loc = new SourceLocation(recognizer.InputStream.SourceName, new SourcePosition(line, charPositionInLine, recognizer.InputStream.Index));
            int id = e is ParseException pe ? pe.ErrorId : recognizer.State;
            var code = new ScopedDiagnosticId(ParseSource.Syntactic, id );
            var diagnostic = code.AsDiagnostic(SeverityMap, IdFormatter, msg, loc, subcategory: null);
            InnerList.Report( diagnostic );
        }

        private readonly DiagnosticMessageCollection InnerList = [];
    }
}

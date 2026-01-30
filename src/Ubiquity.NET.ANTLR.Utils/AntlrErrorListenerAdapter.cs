// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Adapter to translate ANTLR error listeners to an <see cref="IDiagnosticReporter"/></summary>
    /// <remarks>
    /// <para>This intentionally ignores the <see cref="TextWriter"/> provided by ANTLR and uses the <see cref="IDiagnosticReporter"/>
    /// provided in the constructor. This allows a much greater level of flexibility in reporting of diagnostics from
    /// a parser. Especially in abstracting the underlying parse technology from the diagnostic reporting</para>
    /// <para>
    /// The <see cref="SeverityMap"/> is used to allow for future adaptation of the parser to map errors from a
    /// recognizer state, which is not stable if the grammar changes. This ensures that the ID values remain unique
    /// even if the underlying grammar changes.
    /// </para>
    /// <para>The <see cref="DiagnosticMessage.Subcategory"/> for messages reported by this type are always null. If
    /// an application desires specific sub categories then it can use it's own implementation of this type</para>
    /// </remarks>
    public class AntlrErrorListenerAdapter
        : IAntlrErrorListener<int>
        , IAntlrErrorListener<IToken>
    {
        /// <summary>Initializes a new instance of the <see cref="AntlrErrorListenerAdapter"/> class</summary>
        /// <param name="sourceName">Source name for all errors</param>
        /// <param name="severityMap">Severity map to use when transforming errors</param>
        /// <param name="formatter">Formatter to use to form a string version of the error code</param>
        /// <param name="diagnosticReporter">Diagnostic reporter to adapt ANTL errors to</param>
        /// <exception cref="ArgumentException"><paramref name="sourceName"/> is null or whitespace</exception>
        public AntlrErrorListenerAdapter(
            string sourceName,
            IMessageLevelMap severityMap,
            IDiagnosticIdFormatter formatter,
            IDiagnosticReporter diagnosticReporter
        )
        {
            Requires.NotNullOrWhiteSpace( sourceName );

            SeverityMap = severityMap;
            IdFormatter = formatter;
            Reporter = diagnosticReporter;
        }

        /// <summary>Gets the severity mapping used when forming a diagnostic</summary>
        public IMessageLevelMap SeverityMap { get; init; }

        /// <summary>Gets the error ID mapping used when forming a diagnostic</summary>
        public IDiagnosticIdFormatter IdFormatter { get; init; }

        /// <summary>Gets the reporter this instance adapts to</summary>
        public IDiagnosticReporter Reporter { get; init; }

        /// <inheritdoc/>
        public void SyntaxError( TextWriter output // ignored
                               , IRecognizer recognizer
                               , int offendingSymbol
                               , int line
                               , int charPositionInLine
                               , string msg
                               , RecognitionException? e
                               )
        {
            var loc = new SourceLocation(recognizer.InputStream.SourceName, new SourcePosition(line, charPositionInLine, recognizer.InputStream.Index));
            var code = new ScopedDiagnosticId(ParseSource.Lexical, recognizer.State );
            var diagnostic = code.AsDiagnostic(SeverityMap, IdFormatter, msg, loc, subcategory: null);
            Reporter.Report( diagnostic );
        }

        /// <inheritdoc/>
        public void SyntaxError( TextWriter output // ignored
                               , IRecognizer recognizer
                               , IToken? offendingSymbol
                               , int line
                               , int charPositionInLine
                               , string msg
                               , RecognitionException? e
                               )
        {
            var loc = new SourceLocation(recognizer.InputStream.SourceName, new SourcePosition(line, charPositionInLine, recognizer.InputStream.Index));
            int id = e is ParseException pe ? pe.ErrorId : recognizer.State;
            var code = new ScopedDiagnosticId(ParseSource.Syntactic, id );
            var diagnostic = code.AsDiagnostic(SeverityMap, IdFormatter, msg, loc, subcategory: null);
            Reporter.Report( diagnostic );
        }
    }
}

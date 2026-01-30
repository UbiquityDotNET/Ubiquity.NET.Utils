// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Class to implement custom error strategies.</summary>
    /// <remarks>
    /// This is derived from the ANTLR provided <see cref="DefaultErrorStrategy"/> and extends
    /// it to support <see cref="ILexerErrorStrategy"/>. To correctly leverage the error strategy
    /// the lexer must implement <see cref="ISupportLexerErrorStrategy"/> so that lexer errors are
    /// redirected to the strategy before notifying the listeners. This allows consistent central
    /// adoption of custom error strings for error messages. It also allows for customized behavior
    /// with regards to actual error handling/recovery of errors during a parse.
    /// <note type="tip">
    /// There is no support in ANTLR for any custom recovery strategy support for a lexer. In fact,
    /// there is no error customization at all in the default lexer support. This class only supports
    /// customizing the lexer exception before notification if the lexer implements
    /// <see cref="ISupportLexerErrorStrategy"/>, usually in a customizing partial class.
    /// </note>
    /// </remarks>
    public class CustomErrorStrategy
        : DefaultErrorStrategy
        , ILexerErrorStrategy
    {
        /// <summary>Initializes a new instance of the <see cref="CustomErrorStrategy"/> class</summary>
        /// <param name="failOnUnknownException">Indicates if unknown <see cref="RecognitionException"/> types cause an immediate failure [Default: false]</param>
        /// <remarks>
        /// Unknown exceptions are always logged to the debugger, if attached. The <paramref name="failOnUnknownException"/> parameter
        /// determines if they are considered a failure point and stop additional processing.
        /// </remarks>
        public CustomErrorStrategy( bool failOnUnknownException = false )
        {
            FailOnUnknownException = failOnUnknownException;
        }

        /// <summary>Gets a value indicating whether this instance will fail on unknown <see cref="RecognitionException"/>s or just log them</summary>
        public bool FailOnUnknownException { get; }

        /// <summary>Overrides <see cref="DefaultErrorStrategy.ReportError(Parser, RecognitionException)"/> to prevent use of <see cref="Console.Error"/></summary>
        /// <param name="recognizer">Recognizer that found the error</param>
        /// <param name="e">Exception for the original error</param>
        public override void ReportError( Parser recognizer, RecognitionException e )
        {
            if(!InErrorRecoveryMode( recognizer ))
            {
                BeginErrorCondition( recognizer );
                if(e is NoViableAltException exception)
                {
                    ReportNoViableAlternative( recognizer, exception );
                    return;
                }

                if(e is InputMismatchException inputMisMatchEx)
                {
                    ReportInputMismatch( recognizer, inputMisMatchEx );
                    return;
                }

                if(e is FailedPredicateException failedPredEx)
                {
                    ReportFailedPredicate( recognizer, failedPredEx );
                    return;
                }

                // always log to an attached debugger.
                Debug.WriteLine( "ERROR: unknown recognition type: {0}", e.GetType().FullName );
                if(FailOnUnknownException)
                {
                    throw new NotSupportedException( $"Unknown recognition exception type: {e.GetType().FullName}" );
                }
                else
                {
                    // directly notify any listeners of an unknown exception type
                    NotifyErrorListeners( recognizer, e.Message, e );
                }
            }
        }

        /// <inheritdoc/>
        public virtual void ReportLexerError( Lexer recognizer, LexerNoViableAltException e )
        {
            var lexerWithStrategy = (ISupportLexerErrorStrategy)recognizer;

            string srcText = ((ITokenSource)recognizer).InputStream.GetText(Interval.Of(recognizer.TokenStartCharIndex, recognizer.InputStream.Index));
            string utfEscapedSrcText = recognizer.GetErrorDisplay(srcText);
            string msg = $"Syntax error: '{utfEscapedSrcText}' was unexpected here";
            recognizer.ErrorListenerDispatch.SyntaxError( lexerWithStrategy.NotificationErrorOutput, recognizer, 0, recognizer.TokenStartLine, recognizer.TokenStartColumn, msg, e );
        }
    }
}

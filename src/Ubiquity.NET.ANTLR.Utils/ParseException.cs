// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Wraps a <see cref="RecognitionException"/> with an error id value</summary>
    /// <remarks>
    /// This is used by a custom implementation of a lexer or parser error strategy.
    /// (See: <see cref="ILexerErrorStrategy"/>, <see cref="IAntlrErrorStrategy"/>, and <see cref="CustomErrorStrategy"/>)
    /// This type facilitates communication of a custom error ID to an error listener as there
    /// is no other good way to communicate between the two what the error ID is. The strategy is
    /// the best place to determine such a thing.
    /// </remarks>
    [Serializable]
    [SuppressMessage( "Design", "CA1032:Implement standard exception constructors", Justification = "Base doesn't implement those and they'd be nonsensical" )]
    public class ParseException
        : RecognitionException
    {
        /// <summary>Initializes a new instance of the <see cref="ParseException"/> class</summary>
        /// <param name="sourceException">The inner exception this wraps</param>
        /// <param name="errorId">Error Id for the error</param>
        public ParseException( RecognitionException? sourceException, int errorId )
            : base( sourceException?.Message ?? string.Empty, sourceException?.Recognizer, sourceException?.InputStream, sourceException?.Context as ParserRuleContext )
        {
            OffendingToken = sourceException?.OffendingToken;
            SourceException = sourceException ?? throw new ArgumentNullException(nameof( sourceException ));
            ErrorId = errorId;
        }

        /// <summary>Initializes a new instance of the <see cref="ParseException"/> class</summary>
        /// <param name="errorId">Error Id for the error</param>
        public ParseException( int errorId )
            : this( null, errorId )
        {
        }

        /// <summary>Gets the original exception</summary>
        public RecognitionException SourceException { get; }

        /// <summary>Gets the language specific unique ID for the error</summary>
        public int ErrorId { get; }
    }
}

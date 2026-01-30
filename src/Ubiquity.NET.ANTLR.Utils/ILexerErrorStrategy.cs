// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Interface for a lexer error strategy</summary>
    /// <remarks>
    /// To leverage this interface in a lexer partial class, it must
    /// implement <see cref="ISupportLexerErrorStrategy"/>
    /// </remarks>
    public interface ILexerErrorStrategy
    {
        /// <summary>Reports an error from a lexer</summary>
        /// <param name="recognizer">Lexer reporting the error</param>
        /// <param name="e">Error to report</param>
        /// <remarks>
        /// The provided <paramref name="recognizer"/> MUST implement the <see cref="ISupportLexerErrorStrategy"/>
        /// interface to allow access to the output text writer used for generating the output. (Sadly, ANTLR design
        /// requires the writer as a parameter that a notifier must track, instead of being a property of the listener)
        /// </remarks>
        void ReportLexerError( Lexer recognizer, LexerNoViableAltException e );
    }
}

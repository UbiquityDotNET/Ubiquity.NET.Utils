// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Interface for a lexer that supports error strategies</summary>
    /// <remarks>
    /// Sadly, a lexer doesn't have any sort of "Error strategy" as there is only the one
    /// known exception. Thus, there is no official strategy to provide consistency. This
    /// interface, in combination with <see cref="CustomErrorStrategy"/> allows for
    /// consistent interception and establishment of error messages for both a lexer and
    /// parser. This is normally applied to a partial class for the generated lexer as shown
    /// in the example.
    /// </remarks>
    /// <example>
    /// This example shows the basic implementation in a partial <see cref="Lexer"/> class
    /// for a fictitious `My.g4` language grammar.
    /// <code><![CDATA[
    /// public partial class MyLexer
    ///     : ISupportLexerErrorStrategy
    /// {
    ///     // ...
    ///
    ///     public ILexerErrorStrategy ErrorStrategy { get; set; }
    ///
    ///     TextWriter ISupportLexerErrorStrategy.NotificationErrorOutput => ErrorOutput;
    ///
    ///     /// <summary>Notifies listeners of lexer errors via <see cref="ErrorStrategy"/></summary>
    ///     /// <param name="e">Exception from the recognition process</param>
    ///     public override void NotifyListeners(LexerNoViableAltException e)
    ///     {
    ///         if (ErrorStrategy != null)
    ///         {
    ///             ErrorStrategy.ReportLexerError(this, e);
    ///         }
    ///         else
    ///         {
    ///             // no strategy, so 'fall-back' to the default behavior
    ///             base.NotifyListeners(e);
    ///         }
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public interface ISupportLexerErrorStrategy
    {
        /// <summary>Gets or sets the error strategy to use for this lexer (If any)</summary>
        /// <remarks>A null value will result in the default handling built into <see cref="Lexer"/></remarks>
        ILexerErrorStrategy? ErrorStrategy { get; set; }

        /// <summary>Gets the otherwise internal 'ErrorOutput' from a <see cref="Lexer"/></summary>
        /// <remarks>
        /// Sadly, the ANTLR design makes the output writer something the source must track instead of being a part of the
        /// listener. (A lexer really shouldn't need to care about the output writer in any way, as different listeners might
        /// use different output writers, or none at all.)
        /// </remarks>
        TextWriter NotificationErrorOutput { get; }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;

namespace Ubiquity.NET.SourceGenerator.Test.Utils
{
    /// <summary>Utility class to provide additional verification of a <see cref="Diagnostic"/></summary>
    /// <remarks>
    /// <see cref="AnalyzerTest{TVerifier}.DiagnosticVerifier"/> uses only a single delegate AND the default
    /// handling of a test doesn't cover some common scenarios. This class provides those in a form that is
    /// designed to make compossible verifiers as needed.
    /// </remarks>
    public static class DiagnosticVerifiers
    {
        /// <summary>Custom diagnostic verifier to validate the message arguments of a <see cref="Diagnostic"/></summary>
        /// <param name="diagnostic">diagnostic to verify (actual)</param>
        /// <param name="result">Diagnostic result containing the expected results</param>
        /// <param name="verifier">Implementation of <see cref="IVerifier"/> to use for verification</param>
        /// <remarks>
        /// Default verification provided by <see cref="AnalyzerTest{TVerifier}"/> use an undocumented set
        /// of heuristics to verify a given <see cref="Diagnostic"/> is "roughly equivalent" to an expected
        /// <see cref="DiagnosticResult"/>. Unfortunately, the heuristics used are not documented and ultimately
        /// don't include verification of the message args (which a diagnostic producer might get wrong). This
        /// method will verify the arguments by formatting a message using the expected arguments and comparing
        /// that to the message for the <paramref name="diagnostic"/>.
        /// </remarks>
        /// <seealso href="https://github.com/dotnet/roslyn-sdk/issues/1246"/>
        public static void VerifyMessageArguments( Diagnostic diagnostic, DiagnosticResult result, IVerifier verifier )
        {
            var parsedFormat = CompositeFormat.Parse(diagnostic.Descriptor.MessageFormat.ToString());
            int expectedMessageCount = result.MessageArguments?.Length ?? 0;
            int actualMessageCount = parsedFormat.MinimumArgumentCount;

            verifier.Equal(
                parsedFormat.MinimumArgumentCount,
                expectedMessageCount,
                $"Missing test arguments; The number of expected arguments ({expectedMessageCount}) does not match the actual message {actualMessageCount}. [This is a debug only verification]"
            );

            // only test diagnostic if there are expected message args to validate
            // There's no direct way to access the arguments of a diagnostic. (It's there but "internal" and
            // therefore can not be relied upon). So, get the message format from the descriptor and format a message
            // with the expected arguments. Then test the result of that against the message reported by the diagnostic.
            // This will ultimately test that the arguments match expectations.
            if(expectedMessageCount > 0 && parsedFormat.MinimumArgumentCount > 0 && result.MessageArguments is not null)
            {
                string expectedMessage = string.Format(CultureInfo.CurrentCulture, parsedFormat.Format, result.MessageArguments!);
                string actualMessage = diagnostic.GetMessage( CultureInfo.CurrentCulture );
                verifier.Equal( expectedMessage, actualMessage, "Message arguments should be the same" );
            }
        }
    }
}

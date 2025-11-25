// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    /// <summary>Utility class to provide extensions for <see cref="RootCommand"/> and derived types</summary>
    public static class RootCommandExtensions
    {
        /// <summary>Parses a root command and invokes the results</summary>
        /// <param name="rootCommand">Command to parse</param>
        /// <param name="reporter">Diagnostic reporter to use for any errors or information in parsing</param>
        /// <param name="settings">Settings to use for the parser</param>
        /// <param name="args">Command line arguments to parse</param>
        /// <returns>Exit code of the invocation</returns>
        /// <remarks>
        /// <para>This is generally used for single applications that don't need any special
        /// validation of options (Mutual exclusion etc...). Often this is a single (implicit)
        /// command app. This simplifies the requirements for parsing and invoking the results.
        /// </para>
        /// <para>If the <see cref="Command.Action"/> is an asynchronous command action then this will
        /// BLOCK the current thread until it completes. If that is NOT the desired behavior then
        /// callers should use <see cref="ParseAndInvokeResultAsync(RootCommand, IDiagnosticReporter, CmdLineSettings, CancellationToken, string[])"/>
        /// instead for explicitly async operation.</para>
        /// </remarks>
        public static int ParseAndInvokeResult(
            this RootCommand rootCommand,
            IDiagnosticReporter reporter,
            CmdLineSettings settings,
            params string[] args
            )
        {
            ParseResult parseResult = rootCommand.Parse(args, settings);

            // due to bugs and general design of how the command line handling of help and version commands are
            // handled this tries the default options BEFORE checking for errors.
            DefaultHandlerInvocationResult defaultHandlerInvocationResult = parseResult.InvokeDefaultOptions(settings, reporter);
            if(defaultHandlerInvocationResult.ShouldExit)
            {
                return defaultHandlerInvocationResult.ExitCode;
            }

            // If there are errors process them using settings to control the
            // output (Nothing else can set these properties. This is the only
            // known way to configure them)
            if(parseResult.Action is ParseErrorAction parseErrorAction)
            {
                parseErrorAction.ShowHelp = settings.ShowHelpOnErrors;
                parseErrorAction.ShowTypoCorrections = settings.ShowTypoCorrections;
            }

            return parseResult.Invoke( reporter );
        }

        /// <summary>Parses a root command and invokes the results</summary>
        /// <param name="rootCommand">Command to parse</param>
        /// <param name="reporter">Diagnostic reporter to use for any errors or information in parsing</param>
        /// <param name="settings">Settings to use for the parser</param>
        /// <param name="ct">Cancellation token for the operation</param>
        /// <param name="args">Command line arguments to parse</param>
        /// <returns>Exit code of the invocation</returns>
        /// <remarks>
        /// <para>This is generally used for single applications that don't need any special
        /// validation of options (Mutual exclusion etc...). Often this is a single (implicit)
        /// command app. This simplifies the requirements for parsing and invoking the results.
        /// </para>
        /// <para>If the <see cref="Command.Action"/> is an synchronous command action then this will
        /// run the action asynchronously. If that is NOT the desired behavior then callers should
        /// use <see cref="ParseAndInvokeResult(RootCommand, IDiagnosticReporter, CmdLineSettings, string[])"/>
        /// instead for explicitly sync operation.</para>
        /// </remarks>
        public static async Task<int> ParseAndInvokeResultAsync(
            this RootCommand rootCommand,
            IDiagnosticReporter reporter,
            CmdLineSettings settings,
            CancellationToken ct,
            params string[] args
            )
        {
            ParseResult parseResult = rootCommand.Parse(args, settings);
            ct.ThrowIfCancellationRequested();

            // due to bugs and general design of how the command line handling of help and version commands are
            // handled this tries the default options BEFORE checking for errors.
            DefaultHandlerInvocationResult defaultHandlerInvocationResult = parseResult.InvokeDefaultOptions(settings, reporter);
            if(defaultHandlerInvocationResult.ShouldExit)
            {
                return defaultHandlerInvocationResult.ExitCode;
            }

            ct.ThrowIfCancellationRequested();

            // If there are errors process them using settings to control the
            // output (Nothing else can set these properties. This is the only
            // known way to configure them)
            if(parseResult.Action is ParseErrorAction parseErrorAction)
            {
                parseErrorAction.ShowHelp = settings.ShowHelpOnErrors;
                parseErrorAction.ShowTypoCorrections = settings.ShowTypoCorrections;
            }

            return await parseResult.InvokeAsync( reporter, ct );
        }
    }
}

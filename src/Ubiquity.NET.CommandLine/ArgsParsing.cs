// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    /// <summary>Results of invoking the default handlers for <see cref="HelpOption"/> or <see cref="VersionOption"/></summary>
    /// <param name="ShouldExit">Indicates whether the caller should exit (either it was handled successfully or an error was reported)</param>
    /// <param name="ExitCode">Exit code from the invocation (only valid if <see cref="ShouldExit"/> is <see langword="true"/></param>
    [SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Simple record used by this type" )]
    public readonly record struct DefaultHandlerInvocationResult( bool ShouldExit, int ExitCode );

    /// <summary>Static utility class to provide parsing of command lines</summary>
    public static class ArgsParsing
    {
        /// <summary>Parses the command line</summary>
        /// <typeparam name="T">Type of value to bind the results to [Must implement <see cref="ICommandLineOptions{T}"/>]</typeparam>
        /// <param name="args">args array for the command line</param>
        /// <param name="settings">Settings for the parse</param>
        /// <returns>Results of the parse</returns>
        /// <remarks>
        /// Additional steps might include:<br/>
        ///
        /// 1) App specific Validation/semantic analysis<br/>
        /// 2) Binding of results to an app specific type<br/>
        /// 3) Act on the results as proper for the application<br/>
        ///     a. This might include actions parsed but generally isolating the various stages is an easier to understand/maintain model<br/>
        ///     b. Usually this is just app specific code that uses the bound results to adapt behavior<br/>
        ///
        /// <para>
        /// This isolation of stages fosters clean implementation AND allows for variances not considered or accounted for in the
        /// parsing library. (For instance mutual exclusion of options etc...) validation is an APP specific thing. There
        /// may well be common validations available for re-use. But ultimately the relationships of all options etc... are dependent
        /// on the app and sometimes the runtime environment. (i.e., Should an app maintain strict adherence to a command line options
        /// even when such options/patterns are NOT the norm on that environment/OS?)</para>
        /// <para>The <see cref="System.CommandLine"/> system already confuses the help and version "options" as they are conceptually
        /// "commands" in terms of that library. To be fair, the POSIX description it is based on confuses the point as well. This
        /// ambiguity continues by attaching actions and validation to many symbols. While that might seem like it's a good thing,
        /// almost every app needs to customize the behavior. Some apps simply can't do so using the current models. Thus, this
        /// implementation simply removes the actions and validation to leave both stages to the calling application as it keeps
        /// things clearer when stages are unique</para>
        /// </remarks>
        /// <seealso href="https://github.com/dotnet/command-line-api/issues/2659"/>
        public static ParseResult Parse<T>( string[] args, CmdLineSettings? settings = null )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull(args);

            settings ??= new CmdLineSettings();
            RootCommand rootCommand = T.BuildRootCommand(settings);
            return rootCommand.Parse( args, settings );
        }

        // FUTURE: Move these extension methods to ParseResultExtensions (ABI breaking change, but not source level)

        /// <summary>Invokes default options (<see cref="HelpOption"/> or <see cref="VersionOption"/>)</summary>
        /// <param name="parseResult">Result of parse</param>
        /// <param name="settings">settings to determine if enabled (optimizes implementation to a skip if none set)</param>
        /// <param name="diagnosticReporter">Reporter to use to report any diagnostics (Including the "output" of the option)</param>
        /// <returns>Results of the invocation (see remarks for details).</returns>
        /// <remarks>
        /// The results of invoking defaults is a tuple of a flag indicating if the app should exit - default command handled,
        /// and the exit code for the application. The exit code is undefined if the flag indicates the app should not exit (e.g., not
        /// handled). If it is defined, then that is what the app should return. It may be 0 if the command had no errors. But if there
        /// was an error with the execution of the default option.
        /// <note type="information">
        /// This is a back-compat entry point that provides behavior of original release that didn't have an overload to
        /// specify the use of the default handler or the timeout.
        /// </note>
        /// </remarks>
        public static DefaultHandlerInvocationResult InvokeDefaultOptions(
            this ParseResult parseResult,
            CmdLineSettings settings,
            IDiagnosticReporter diagnosticReporter
            )
        {
            return ParseResultExtensions.InvokeDefaultOptions( parseResult, settings, diagnosticReporter, enableDefaultHandler: false );
        }

        /// <summary>Reports any error found during parsing</summary>
        /// <param name="parseResult">Results of the parse</param>
        /// <param name="diagnosticReporter">Reporter to report parse errors to</param>
        /// <returns><see langword="true"/> if errors found and <see langword="false"/> if not</returns>
        public static bool ReportErrors( this ParseResult parseResult, IDiagnosticReporter diagnosticReporter )
        {
            if(parseResult.HasErrors())
            {
                foreach(var err in parseResult.Errors)
                {
                    // CONSIDER: extract location from error?
                    diagnosticReporter.Error( CultureInfo.CurrentCulture, err.Message );
                }

                return true;
            }

            return false;
        }

        /// <summary>Tries to parse a command line, and binds the results to a value</summary>
        /// <typeparam name="T">Type of the value to bind the results to</typeparam>
        /// <param name="args">Arguments to parse</param>
        /// <param name="settings">settings to use for the parse</param>
        /// <param name="diagnosticReporter">Reporter to use for diagnostic reporting</param>
        /// <param name="boundValue">Resulting value if parsed successfully</param>
        /// <param name="exitCode">Exit code for the process (only valid when return is <see langword="false"/> (see remarks)</param>
        /// <returns><see langword="true"/> if the app should continue and <see langword="false"/> if not</returns>
        /// <remarks>
        /// <para>Since this wraps several common operations, some of which may require exiting the app, the return value
        /// has the semantics of "App should continue". In the event of parse errors or failures in invocation of
        /// the default options the result is a <see langword="false"/> with an <paramref name="exitCode"/> set to
        /// a proper exit code value. (non-zero for errors and zero for success even though the app should still
        /// exit)</para>
        ///
        /// <para>This wraps the common pattern of parsing a command line, invoking default options, and binding the results of a parse
        /// using a standard .NET "try pattern". The invocation of default options may return <see langword="false"/> with
        /// <paramref name="exitCode"/> set to <c>0</c>. This indicates the parse was successful AND that the default option was
        /// run successfully and the app should exit.</para>
        ///
        /// In short, this wraps the following sequence of common operations and exiting on completion of
        /// any operation with errors or successful invocation of default options:<br/>
        /// 1) <see cref="Parse{T}(string[], CmdLineSettings?)"/><br/>
        /// 2) <see cref="ReportErrors(ParseResult, IDiagnosticReporter)"/><br/>
        /// 3) <see cref="InvokeDefaultOptions(ParseResult, CmdLineSettings, IDiagnosticReporter)"/><br/>
        /// 4) <see cref="ICommandLineOptions{T}.Bind(ParseResult)"/><br/>
        ///
        /// <para>The <paramref name="exitCode"/> is set to the exit code for the app on failures. This code indicates the
        /// parse errors and is the result of invoking <see cref="ParseErrorAction"/> which, as of the current release,
        /// is always a literal `1`, though this behavior is not documented and therefore subject to change. Thus, calling
        /// applications should ***NOT*** rely on this value and instead use their own value to indicate a parse error that
        /// is documented and stable.</para>
        /// <note type="important">
        /// It is recommended that applications NOT use this method (It will likely be obsoleted in the next release).
        /// Instead applications should use the <see cref="RootCommandExtensions.ParseAndInvokeResult(RootCommand, IDiagnosticReporter, CmdLineSettings, string[])"/>
        /// or <see cref="RootCommandExtensions.ParseAndInvokeResultAsync(RootCommand, IDiagnosticReporter, CmdLineSettings, CancellationToken, string[])"/> methods
        /// instead. These include invocation in the name and the result is more consistent with expectations.
        /// This method has a confusing return code (and was even incorrect in some cases). Nullability of the <paramref name="boundValue"/>
        /// is not guaranteed based on the return value semantics. (It is null if parsed correctly AND the default options were handled).
        /// This confusing state of affairs makes this less useful than was originally intended.
        /// </note>
        /// </remarks>
        /// <seealso href="https://github.com/UbiquityDotNET/Ubiquity.NET.Utils/issues/15"/>
        public static bool TryParse<T>(
            string[] args,
            CmdLineSettings? settings,
            IDiagnosticReporter diagnosticReporter,
            out T? boundValue,
            out int exitCode
            )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull( args );
            ArgumentNullException.ThrowIfNull( diagnosticReporter );

            settings ??= new CmdLineSettings();
            boundValue = default;
            RootCommand rootCommand = T.BuildRootCommand(settings);
            ParseResult parseResult = rootCommand.Parse( args, settings );

            // Special case the default options (Help/Version) before checking for reported errors
            // as errors about missing required params when the defaults are used is actually "normal"
            // (Ridiculously stupid, but considered normal by owners of System.CommandLine [sigh...])
            var invokeResults = parseResult.InvokeDefaultOptions(settings, diagnosticReporter);
            if(invokeResults.ShouldExit)
            {
                exitCode = invokeResults.ExitCode;
                return true;
            }

            if(parseResult.Action is ParseErrorAction pea)
            {
                pea.ShowHelp = settings.ShowHelpOnErrors;
                pea.ShowTypoCorrections = settings.ShowTypoCorrections;
                exitCode = pea.Invoke( parseResult );
                return true;
            }

            boundValue = T.Bind( parseResult );
            exitCode = 0;
            return false; // return semantics is "should exit".
        }

        /// <inheritdoc cref="TryParse{T}(string[], CmdLineSettings?, IDiagnosticReporter, out T, out int)"/>
        public static bool TryParse<T>( string[] args, CmdLineSettings settings, out T? boundValue, out int exitCode )
            where T : ICommandLineOptions<T>
        {
            ArgumentNullException.ThrowIfNull( settings );
            return TryParse<T>( args, settings, new ConsoleReporter( MsgLevel.Information ), out boundValue, out exitCode );
        }

        /// <inheritdoc cref="TryParse{T}(string[], CmdLineSettings?, IDiagnosticReporter, out T, out int)"/>
        public static bool TryParse<T>( string[] args, out T? boundValue, out int exitCode )
            where T : ICommandLineOptions<T>
        {
            return TryParse<T>( args, settings: null, new ConsoleReporter( MsgLevel.Information ), out boundValue, out exitCode );
        }

        /// <inheritdoc cref="TryParse{T}(string[], CmdLineSettings?, IDiagnosticReporter, out T, out int)"/>
        public static bool TryParse<T>( string[] args, IDiagnosticReporter diagnosticReporter, out T? boundValue, out int exitCode )
            where T : ICommandLineOptions<T>
        {
            return TryParse<T>( args, settings: null, diagnosticReporter, out boundValue, out exitCode );
        }
    }
}

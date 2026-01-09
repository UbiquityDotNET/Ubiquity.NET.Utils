// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    /// <summary>Extension of <see cref="RootCommand"/> that allows app control of defaults that are otherwise forced</summary>
    /// <remarks>
    /// This type is derived from <see cref="RootCommand"/> and offers little additional behavior beyond the construction.
    /// (Captures the settings for use with invocation, and the overloads to parse and invoke a result using the captured settings)
    /// The constructor will adapt the command based on the <see cref="CommandLineSettings"/> provided. This moves the
    /// hard coded defaults into an app controlled domain. The default constructed settings matches the behavior of
    /// <see cref="RootCommand"/> so there's no distinction. This allows an application to explicitly decide the behavior
    /// and support of various defaults that could otherwise surprise the author/user. This is especially important when
    /// replacing the internal command line handling of a published app or otherwise creating a "drop-in" replacement. In
    /// such cases, strict adherence to back-compat is of paramount importance and the addition of default behavior is
    /// potentially a breaking change.
    /// </remarks>
    [SuppressMessage( "Design", "CA1010:Generic interface should also be implemented", Justification = "Collection initialization" )]
    public class AppControlledDefaultsRootCommand
        : RootCommand
    {
        /// <summary>Initializes a new instance of the <see cref="AppControlledDefaultsRootCommand"/> class.</summary>
        /// <param name="description">Description of this root command</param>
        /// <param name="settings">Settings to apply for the command parsing</param>
        public AppControlledDefaultsRootCommand( CommandLineSettings settings, string description = "" )
            : base( description )
        {
            ArgumentNullException.ThrowIfNull(settings);
            Settings = settings;

            // RootCommand constructor already adds HelpOption and VersionOption so remove them
            // unless specified by caller.
            var removeableOptions = from o in Options
                                    where (o is HelpOption && !settings.DefaultOptions.HasFlag(DefaultOption.Help))
                                       || (o is VersionOption && !settings.DefaultOptions.HasFlag(DefaultOption.Version))
                                    select o;

            // .ToArray forces duplication of the enumeration to prevent exception from modifying
            // the underlying list while enumerating.
            foreach(var o in removeableOptions.ToArray())
            {
                Options.Remove( o );
            }

            // RootCommand constructor adds the "SuggestDirective" directive.
            if(!settings.DefaultDirectives.HasFlag( DefaultDirective.Suggest ))
            {
                // Remove default added and start clean.
                Directives.Clear();
            }

            // Add additional directives based on app controlled settings
            if(settings.DefaultDirectives.HasFlag( DefaultDirective.Diagram ))
            {
                Add( new DiagramDirective() );
            }

            if(settings.DefaultDirectives.HasFlag( DefaultDirective.EnvironmentVariables ))
            {
                Add( new EnvironmentVariablesDirective() );
            }
        }

        /// <summary>Gets the settings used for creation and subsequent invocation</summary>
        public CommandLineSettings Settings { get; }

        /// <summary>Parses a root command and invokes the results</summary>
        /// <param name="reporter">Diagnostic reporter to use for any errors or information in parsing</param>
        /// <param name="args">Command line arguments to parse</param>
        /// <returns>Exit code of the invocation</returns>
        /// <remarks>
        /// <para>If the <see cref="Command.Action"/> is an asynchronous command action then this will
        /// BLOCK the current thread until it completes. If that is NOT the desired behavior then
        /// callers should use <see cref="ParseAndInvokeResultAsync(IDiagnosticReporter, CancellationToken, string[])"/>
        /// instead for explicit async operation.</para>
        /// </remarks>
        public int ParseAndInvokeResult(
            IDiagnosticReporter reporter,
            params string[] args
            )
        {
            return RootCommandExtensions.ParseAndInvokeResult(this, reporter, Settings, args);
        }

        /// <summary>Parses a root command and invokes the results</summary>
        /// <param name="reporter">Diagnostic reporter to use for any errors or information in parsing</param>
        /// <param name="ct">Cancellation token for the operation</param>
        /// <param name="args">Command line arguments to parse</param>
        /// <returns>Exit code of the invocation</returns>
        /// <remarks>
        /// <para>If the <see cref="Command.Action"/> is an synchronous command action then this will
        /// run the action asynchronously. If that is NOT the desired behavior then callers should
        /// use <see cref="ParseAndInvokeResult(IDiagnosticReporter, string[])"/>
        /// instead for an explicit synchronous operation.</para>
        /// </remarks>
        public Task<int> ParseAndInvokeResultAsync(
            IDiagnosticReporter reporter,
            CancellationToken ct,
            params string[] args
            )
        {
            return RootCommandExtensions.ParseAndInvokeResultAsync( this, reporter, Settings, ct, args );
        }
    }
}

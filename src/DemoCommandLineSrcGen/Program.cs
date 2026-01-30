// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Threading;
using System.Threading.Tasks;

using Ubiquity.NET.CommandLine;
using Ubiquity.NET.Extensions;

namespace TestNamespace
{
    public static class Program
    {
        public static async Task<int> Main( string[] args )
        {
            // pressing CTRL-C cancels the entire operation
            using CancellationTokenSource cts = new();
            Console.CancelKeyPress += ( _, e ) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // start with information level for parsing; parsed options might specify different level
            var reporter = new ColoredConsoleReporter( MessageLevel.Information );

            return await TestOptions.BuildRootCommand( ( options, ct ) => AppMainAsync( options, reporter, ct ) )
                                    .ParseAndInvokeResultAsync( reporter, cts.Token, args );
        }

        /// <summary>Asynchronous main entry point</summary>
        /// <param name="options">Parsed command line options</param>
        /// <param name="reporter">Reporter to use for reporting diagnostics for this app</param>
        /// <param name="ct">Cancellation token to indicate cancellation of the entire app</param>
        /// <returns>Exit code for the app; 0=NOERROR; any other value MAY indicate an error (App defined)</returns>
        private static async Task<int> AppMainAsync(
                TestOptions options,
                ColoredConsoleReporter reporter,
                CancellationToken ct
            )
        {
            // Now that args are parsed, if a distinct verbosity level is specified use that
            if(options.Verbosity != reporter.Level)
            {
                reporter = new ColoredConsoleReporter( options.Verbosity );
            }

            // Use the cancellation token to indicate cancellation
            // This is set when CTRL-C is pressed in Main() above.
            ct.ThrowIfCancellationRequested();

            // For demo, Report the full parsed args to the "Verbose" channel
            // in a normal app this isn't done or would use some sort of logging
            // and not a UX reporter, but it is useful for validation scripts
            reporter.Verbose( $"TestOptions:\n{options}" );

            // Core application code here...
            // app should use/test ct for cancellation of long operations.
            return 0;
        }
    }
}

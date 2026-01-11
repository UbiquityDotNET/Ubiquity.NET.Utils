// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable SA1600 // Elements should be documented

using System;
using System.Threading;
using System.Threading.Tasks;

using Ubiquity.NET.CommandLine;

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
            var reporter = new ColoredConsoleReporter( MsgLevel.Information );

            return await TestOptions.BuildRootCommand( ( options, ct ) => AppMainAsync( options, reporter, ct ) )
                                    .ParseAndInvokeResultAsync( reporter, cts.Token, args );
        }

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

            reporter.Verbose( "AppMainAsync" );

            // Core application code here...

            // Use the cancellation token to indicate cancellation
            // This is set when CTRL-C is pressed in Main() above.
            ct.ThrowIfCancellationRequested();
            return 0;
        }
    }
}

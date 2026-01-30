// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    // internal extension for IDiagnosticReporter
    internal static class ReportingSettings
    {
        // Create an InvocationConiguration that wraps an IDiagnosticReporter
        public static InvocationConfiguration CreateConfig( this IDiagnosticReporter self, bool enableDefaultHandler, TimeSpan? timeout = null )
        {
            ArgumentNullException.ThrowIfNull( self );

            return new()
            {
                EnableDefaultExceptionHandler = enableDefaultHandler,
                Error = new DiagnosticReportingWriter( self, MessageLevel.Error ),
                Output = new DiagnosticReportingWriter( self, MessageLevel.Information ),
                ProcessTerminationTimeout = timeout,
            };
        }
    }
}

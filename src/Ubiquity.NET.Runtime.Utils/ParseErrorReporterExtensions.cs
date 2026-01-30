// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Utility class to provide extension methods for <see cref="IDiagnosticReporter"/></summary>
    public static class ParseErrorReporterExtensions
    {
        /// <summary>Collects and reports all diagnostics in an <see cref="IAstNode"/></summary>
        /// <param name="self">Reporter to use for any diagnostics found</param>
        /// <param name="node">Node to find diagnostics from</param>
        /// <returns><see langword="true"/> if any diagnostics were found; <see langword="false"/> if not</returns>
        public static bool CheckAndReportParseDiagnostics( this IDiagnosticReporter self, IAstNode? node )
        {
            ArgumentNullException.ThrowIfNull( self );

            if(node is null)
            {
                return true;
            }

            // Gather all diagnostics for the node, if none found
            // bail out early.
            var diagnostics = new DiagnosticCollector().Visit(node);
            if(diagnostics.Count == 0)
            {
                return false;
            }

            // report each diagnostic found
            foreach(var diagnostic in diagnostics)
            {
                self.Report( diagnostic );
            }

            return true;
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

#if DELETE_ME_LATER
namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Implementation of <see cref="IDiagnosticReporter"/> to collect any diagnostics that occur during a parse</summary>
    public class ParseErrorCollector
        : IDiagnosticReporter
    {
        /// <summary>Gets the level setting for this reporter (Always <see cref="MessageLevel.Error"/></summary>
        public MessageLevel Level => MessageLevel.Error;

        /// <inheritdoc/>
        public Encoding Encoding { get; init; } = Encoding.Unicode;

        public void Report( DiagnosticMessage diagnostic )
        {
            throw new NotImplementedException();
        }

        /// <summary>Gets the error nodes found by this listener</summary>
        public ImmutableArray<DiagnosticMessage> ErrorNodes { get; private set; } = [];

    }
}
#endif

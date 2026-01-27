// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine
{
    /// <summary><see cref="TextWriter"/> implementation that wraps an <see cref="IDiagnosticReporter"/></summary>
    /// <remarks>
    /// This is an instance of the adapter pattern to provide a text writer interface that wraps an <see cref="IDiagnosticReporter"/>.
    /// Newlines are detected and transformed into a diagnostic reported to the reporter. This will write a diagnostic
    /// when <see cref="TextWriter.NewLine"/> is detected. When a diagnostic is reported then the internal builder is cleared so that
    /// the next line is a new diagnostic. <see cref="MsgLevel"/> is used to specify the message level to report for all
    /// diagnostics in this writer.
    /// </remarks>
    internal class DiagnosticReportingWriter
        : StringWriter
    {
        /// <summary>Initializes a new instance of the <see cref="DiagnosticReportingWriter"/> class.</summary>
        /// <param name="reporter">Reporter to create reports with</param>
        /// <param name="msgLevel">Level of diagnostics to report for this instance</param>
        public DiagnosticReportingWriter( IDiagnosticReporter reporter, MessageLevel msgLevel )
        {
            Reporter = reporter;
            MsgLevel = msgLevel;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// IFF <paramref name="value"/> is <see cref="TextWriter.NewLine"/> then this will
        /// report the contents of the underlying <see cref="StringBuilder"/> and clear it
        /// so that any new content is treated as a new diagnostic.
        /// </remarks>
        public override void Write( string? value )
        {
            if(value is not null && value == NewLine)
            {
                ReportContents();
            }
            else
            {
                base.Write( value );
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// IFF <paramref name="buffer"/> is <see cref="TextWriter.NewLine"/> then this will
        /// report the contents of the underlying <see cref="StringBuilder"/> and clear it
        /// so that any new content is treated as a new diagnostic.
        /// </remarks>
        public override void Write( char[] buffer, int index, int count )
        {
            if(buffer.SequenceEqual( CoreNewLine ))
            {
                ReportContents();
            }
            else
            {
                base.Write( buffer, index, count );
            }
        }

        /// <summary>Reports the contents of the underlying builder (if any)</summary>
        public override void Flush( )
        {
            ReportContents();
        }

        /// <inheritdoc/>
        public override Task FlushAsync( )
        {
            return Task.Factory.StartNew( static state => ((TextWriter)state!).Flush(),
                                          this,
                                          CancellationToken.None,
                                          TaskCreationOptions.DenyChildAttach,
                                          TaskScheduler.Default
                                        );
        }

        /// <inheritdoc/>
        public override Task FlushAsync( CancellationToken cancellationToken )
        {
            return cancellationToken.IsCancellationRequested
                 ? Task.FromCanceled( cancellationToken )
                 : FlushAsync();
        }

        /// <summary>Gets the message level reported by this instance</summary>
        public MessageLevel MsgLevel { get; }

        /// <summary>Gets the reporter this instance reports diagnostics to</summary>
        public IDiagnosticReporter Reporter { get; }

        /// <inheritdoc/>
        protected override void Dispose( bool disposing )
        {
            if(disposing)
            {
                ReportContents();
            }

            base.Dispose( disposing );
        }

        private void ReportContents( )
        {
            var bldr = GetStringBuilder();
            string msg = bldr.ToString();
            if(!string.IsNullOrWhiteSpace( msg ))
            {
                Reporter.Report( MsgLevel, msg );
            }

            bldr.Clear();
        }
    }
}

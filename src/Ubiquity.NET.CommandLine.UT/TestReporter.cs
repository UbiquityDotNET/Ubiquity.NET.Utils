// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ubiquity.NET.Extensions;

// Disambiguate from test framework type
using MessageLevel = Ubiquity.NET.Extensions.MessageLevel;

namespace Ubiquity.NET.CommandLine.UT
{
    internal class TestReporter
        : IDiagnosticReporter
    {
        public TestReporter(MessageLevel level)
        {
            Level = level;
        }

        public TestReporter()
            : this( MessageLevel.Verbose ) // Captures ALL messages (Max level)
        {
        }

        public MessageLevel Level { get; }

        public Encoding Encoding => Encoding.Unicode;

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation will test if the <see cref="DiagnosticMessage.Level"/> of the
        /// message is enabled. If so, then a call is made to the virtual <see cref="ReportMessage(MessageLevel, string)"/>
        /// with the results of <see cref="DiagnosticMessage.ToString()"/> as the message text.
        /// </remarks>
        public void Report( DiagnosticMessage diagnostic )
        {
            if(!this.IsEnabled( diagnostic.Level ))
            {
                return;
            }

            AllMessages.Add(diagnostic);
        }

        /// <summary>Gets ALL of the messages provided to this reporter</summary>
        public List<DiagnosticMessage> AllMessages { get; } = [];

        /// <summary>Gets the messages with a level of <see cref="MessageLevel.None"/></summary>
        /// <remarks>This is always an error and tests should validate this is an empty array</remarks>
        public ImmutableArray<DiagnosticMessage> NoneMessages => [ .. GetMessages( MessageLevel.None ) ];

        /// <summary>Gets the messages with a level of <see cref="MessageLevel.Verbose"/></summary>
        public ImmutableArray<DiagnosticMessage> VerboseMessages => [ .. GetMessages(MessageLevel.Verbose) ];

        /// <summary>Gets the messages with a level of <see cref="MessageLevel.Information"/></summary>
        public ImmutableArray<DiagnosticMessage> InformationMessages => [ .. GetMessages( MessageLevel.Information ) ];

        /// <summary>Gets the messages with a level of <see cref="MessageLevel.Warning"/></summary>
        public ImmutableArray<DiagnosticMessage> WarningMessages => [ .. GetMessages( MessageLevel.Warning ) ];

        /// <summary>Gets the messages with a level of <see cref="MessageLevel.Error"/></summary>
        public ImmutableArray<DiagnosticMessage> ErrorMessages => [ .. GetMessages( MessageLevel.Error ) ];

        private IEnumerable<DiagnosticMessage> GetMessages(MessageLevel level)
        {
            return from dm in AllMessages
                   where dm.Level == level
                   select dm;
        }
    }
}

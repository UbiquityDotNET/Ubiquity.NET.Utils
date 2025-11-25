// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ubiquity.NET.CommandLine.UT
{
    internal class TestReporter
        : IDiagnosticReporter
    {
        public MsgLevel Level => MsgLevel.Error; // Captures ALL messages (Max level)

        public Encoding Encoding => Encoding.Unicode;

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation will test if the <see cref="DiagnosticMessage.Level"/> of the
        /// message is enabled. If so, then a call is made to the virtual <see cref="ReportMessage(MsgLevel, string)"/>
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

        /// <summary>Gets the messages with a level of <see cref="MsgLevel.None"/></summary>
        /// <remarks>This is always an error and tests should validate this is an empty array</remarks>
        public ImmutableArray<DiagnosticMessage> NoneMessages => [ .. GetMessages( MsgLevel.None ) ];

        /// <summary>Gets the messages with a level of <see cref="MsgLevel.Verbose"/></summary>
        public ImmutableArray<DiagnosticMessage> VerboseMessages => [ .. GetMessages(MsgLevel.Verbose) ];

        /// <summary>Gets the messages with a level of <see cref="MsgLevel.Information"/></summary>
        public ImmutableArray<DiagnosticMessage> InformationMessages => [ .. GetMessages( MsgLevel.Information ) ];

        /// <summary>Gets the messages with a level of <see cref="MsgLevel.Warning"/></summary>
        public ImmutableArray<DiagnosticMessage> WarningMessages => [ .. GetMessages( MsgLevel.Warning ) ];

        /// <summary>Gets the messages with a level of <see cref="MsgLevel.Error"/></summary>
        public ImmutableArray<DiagnosticMessage> ErrorMessages => [ .. GetMessages( MsgLevel.Error ) ];

        private IEnumerable<DiagnosticMessage> GetMessages(MsgLevel level)
        {
            return from dm in AllMessages
                   where dm.Level == level
                   select dm;
        }
    }
}

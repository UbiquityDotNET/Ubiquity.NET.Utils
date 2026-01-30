// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>AST visitor that collects all diagnostics for a given node and it's children</summary>
    public class DiagnosticCollector
        : AstVisitorBase<ImmutableList<DiagnosticMessage>>
    {
        /// <summary>Initializes a new instance of the <see cref="DiagnosticCollector"/> class</summary>
        public DiagnosticCollector( )
            : base( [] )
        {
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This implementation will aggregate the node to the results of diagnostics for all children
        /// which will, in turn, add any <see cref="DiagnosticMessage"/>s to the collected results. Thus
        /// resulting in a final array of errors.
        /// </remarks>
        public override ImmutableList<DiagnosticMessage> Visit( IAstNode node )
        {
            return AggregateResult( node.Diagnostics, VisitChildren( node ) );
        }

        /// <inheritdoc/>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Nested Conditionals is not simpler" )]
        protected override ImmutableList<DiagnosticMessage> AggregateResult(
            ImmutableList<DiagnosticMessage>? aggregate,
            ImmutableList<DiagnosticMessage>? newResult
            )
        {
            if(aggregate is null)
            {
                return newResult is null ? [] : newResult;
            }

            return newResult is null ? aggregate : aggregate.AddRange( newResult );
        }
    }
}

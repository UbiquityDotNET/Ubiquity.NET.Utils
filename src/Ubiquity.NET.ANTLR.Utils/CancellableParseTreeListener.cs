// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Threading;

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>IParseTreeListener that provides support for cancellation</summary>
    /// <remarks>
    /// This class will throw an OperationCanceled from any method when the provided
    /// <see cref="CancellationToken"/> has requested cancellation of the parse. Cancellable
    /// </remarks>
    public class CancellableParseTreeListener
        : IParseTreeListener
    {
        /// <summary>Initializes a new instance of the <see cref="CancellableParseTreeListener"/> class.</summary>
        /// <param name="ct">Token to use for detection of cancellation</param>
        public CancellableParseTreeListener( CancellationToken ct )
        {
            CancelToken = ct;
        }

        /// <inheritdoc/>
        public void EnterEveryRule( ParserRuleContext ctx )
        {
            CancelToken.ThrowIfCancellationRequested();
        }

        /// <inheritdoc/>
        public void ExitEveryRule( ParserRuleContext ctx )
        {
            CancelToken.ThrowIfCancellationRequested();
        }

        /// <inheritdoc/>
        public void VisitErrorNode( IErrorNode node )
        {
            CancelToken.ThrowIfCancellationRequested();
        }

        /// <inheritdoc/>
        public void VisitTerminal( ITerminalNode node )
        {
            CancelToken.ThrowIfCancellationRequested();
        }

        private readonly CancellationToken CancelToken;
    }
}

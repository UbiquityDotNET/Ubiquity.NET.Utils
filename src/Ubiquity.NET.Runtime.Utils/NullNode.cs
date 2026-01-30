// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Null Object pattern implementation for AST Nodes</summary>
    public class NullNode
        : IAstNode
    {
        /// <inheritdoc/>
        public SourceLocation Location { get; } = default;

        /// <inheritdoc/>
        public IEnumerable<IAstNode> Children { get; } = [];

        /// <inheritdoc/>
        public TResult? Accept<TResult>( IAstVisitor<TResult> visitor )
        {
            return default;
        }

        /// <inheritdoc/>
        public virtual TResult? Accept<TResult, TArg>( IAstVisitor<TResult, TArg> visitor, ref readonly TArg arg )
#if NET9_0_OR_GREATER
        where TArg : struct, allows ref struct
#else
        where TArg : struct
#endif
        {
            ArgumentNullException.ThrowIfNull( visitor );
            return default;
        }

        /// <inheritdoc/>
        public ImmutableList<DiagnosticMessage> Diagnostics => [];

        /// <inheritdoc/>
        public void AddDiagnostic( DiagnosticMessage error )
        {
            throw new NotSupportedException( "Cannot add diagnostics to a null node" );
        }

        /// <inheritdoc/>
        public void AddDiagnostics( IEnumerable<DiagnosticMessage> errors )
        {
            throw new NotSupportedException( "Cannot add diagnostics to a null node" );
        }

        /// <summary>Gets a singleton null node instance</summary>
        public static NullNode Instance => LazyInstance.Value;

        private static readonly Lazy<NullNode> LazyInstance = new(LazyThreadSafetyMode.PublicationOnly);
    }
}

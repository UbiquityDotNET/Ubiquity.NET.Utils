// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Base class for implementations of <see cref="ISyntaxNode"/></summary>
    public abstract class SyntaxNodeBase
        : ISyntaxNode
    {
        /// <summary>Initializes a new instance of the <see cref="SyntaxNodeBase"/> class</summary>
        /// <param name="location">Location of the node</param>
        protected SyntaxNodeBase( SourceLocation location )
        {
            Location = location;
        }

        /// <inheritdoc/>
        public SourceLocation Location { get; }

        /// <inheritdoc/>
        public virtual IEnumerable<ISyntaxNode> Children
            => [];

        /// <inheritdoc/>
        [SuppressMessage( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Only applies to 8.0 builds" )]
        [SuppressMessage( "Style", "IDE0305:Simplify collection initialization", Justification = "Result is obscure/terse syntax that is anything but more comprehensible" )]
        public ImmutableList<DiagnosticMessage> Diagnostics => DiagnosticList;

        /// <inheritdoc/>
        public void AddDiagnostic( DiagnosticMessage diagnostic )
        {
            ImmutableInterlocked.Update( ref DiagnosticList, (l, e) => l.Add( e ), diagnostic );
        }

        /// <inheritdoc/>
        public void AddDiagnostic( IEnumerable<DiagnosticMessage> diagnostics )
        {
            ImmutableInterlocked.Update( ref DiagnosticList, ( l, e ) => l.AddRange( e ), diagnostics );
        }

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <returns>Result of visiting this node</returns>
        public virtual TResult? Accept<TResult>( ISyntaxTreeVisitor<TResult> visitor )
        {
            return visitor.Visit( this );
        }

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <typeparam name="TArg">Type of the argument to pass on to the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <param name="arg">Argument to pass to the concrete type as a readonly ref</param>
        /// <returns>Result of visiting this node</returns>
        public virtual TResult? Accept<TResult, TArg>( ISyntaxTreeVisitor<TResult, TArg> visitor, ref readonly TArg arg )
#if NET9_0_OR_GREATER
        where TArg : struct
        , allows ref struct
#else
        where TArg : struct
#endif
        {
            return visitor.Visit( this, in arg );
        }

        private ImmutableList<DiagnosticMessage> DiagnosticList = [];
    }
}

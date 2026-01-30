// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Common abstract base implementation of <see cref="IAstNode"/></summary>
    public abstract class AstNode
        : IAstNode
    {
        /// <inheritdoc/>
        public SourceLocation Location { get; }

        /// <inheritdoc/>
        public abstract IEnumerable<IAstNode> Children { get; }

        /// <inheritdoc/>
        [SuppressMessage( "CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Only applies to 8.0 builds" )]
        [SuppressMessage( "Style", "IDE0305:Simplify collection initialization", Justification = "Result is obscure/terse syntax that is anything but more comprehensible" )]
        public ImmutableList<DiagnosticMessage> Diagnostics => DiagnosticList;

        /// <inheritdoc/>
        public void AddDiagnostic( DiagnosticMessage error )
        {
            ImmutableInterlocked.Update( ref DiagnosticList, ( l, e ) => l.Add( e ), error );
        }

        /// <inheritdoc/>
        public void AddDiagnostics( IEnumerable<DiagnosticMessage> errors )
        {
            ImmutableInterlocked.Update( ref DiagnosticList, ( l, e ) => l.AddRange( e ), errors );
        }

        // NOTE: Accept() dispatching is NOT implemented here to allow type specific handling
        //       dispatch to the correct Visit(...). Implementation of that method requires
        //       type specific knowledge of the thing being visited. So this is an abstract
        //       method that an implementation will need to provide, even though the implementation
        //       looks the same, it isn't, as it includes direct calls to the correct overload
        //       of the Visit() method. It is plausible that a source generator could create
        //       the implementation of such mundane and error prone code duplication though...

        /// <inheritdoc/>
        public abstract TResult? Accept<TResult>( IAstVisitor<TResult> visitor );

        /// <inheritdoc/>
        public abstract TResult? Accept<TResult, TArg>( IAstVisitor<TResult, TArg> visitor, ref readonly TArg arg )
#if NET9_0_OR_GREATER
        where TArg : struct, allows ref struct;
#else
        where TArg : struct;
#endif

        /// <summary>Initializes a new instance of the <see cref="AstNode"/> class</summary>
        /// <param name="location">Location in the source this node represents</param>
        protected AstNode( SourceLocation location )
        {
            Location = location;
        }

        private ImmutableList<DiagnosticMessage> DiagnosticList = [];
    }
}

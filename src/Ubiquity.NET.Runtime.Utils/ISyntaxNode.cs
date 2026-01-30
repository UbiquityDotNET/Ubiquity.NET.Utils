// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Interface for a node in a source tree</summary>
    public interface ISyntaxNode
    {
        /// <summary>Gets the location in source or a default constructed value if no source location available</summary>
        SourceLocation Location { get; }

        /// <summary>Gets immediate child nodes of this node</summary>
        /// <remarks>This enumeration is NOT <see langword="null"/>; implementations must provide a valid enumerable even if it is empty</remarks>
        IEnumerable<ISyntaxNode> Children { get; }

        /// <summary>Gets a list all diagnostics associated with this node</summary>
        ImmutableList<DiagnosticMessage> Diagnostics { get; }

        /// <summary>Add a diagnostic to this node</summary>
        /// <param name="diagnostic">Error to attach to this node</param>
        void AddDiagnostic( DiagnosticMessage diagnostic );

        /// <summary>Add a range of errors to this node</summary>
        /// <param name="diagnostics">Errors to attach to this node</param>
        void AddDiagnostic( IEnumerable<DiagnosticMessage> diagnostics );

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <returns>Result of visiting this node</returns>
        TResult? Accept<TResult>( ISyntaxTreeVisitor<TResult> visitor );

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <typeparam name="TArg">Type of the argument to pass on to the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <param name="arg">Argument to pass to the concrete type as a readonly ref</param>
        /// <returns>Result of visiting this node</returns>
        TResult? Accept<TResult, TArg>( ISyntaxTreeVisitor<TResult, TArg> visitor, ref readonly TArg arg )
#if NET9_0_OR_GREATER
        where TArg : struct
        , allows ref struct;
#else
        where TArg : struct;
#endif

    }
}

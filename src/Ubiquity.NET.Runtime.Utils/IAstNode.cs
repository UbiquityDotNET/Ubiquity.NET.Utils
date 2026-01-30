// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Root interface for nodes in the Abstract Syntax Tree</summary>
    public interface IAstNode
    {
        /// <summary>Gets the source location covering the original source for the node</summary>
        SourceLocation Location { get; }

        /// <summary>Gets a collection of children for the node</summary>
        IEnumerable<IAstNode> Children { get; }

        /// <summary>Gets all diagnostics associated with this node</summary>
        ImmutableList<DiagnosticMessage> Diagnostics { get; }

        /// <summary>Add an error to this node</summary>
        /// <param name="error">Error to attach to this node</param>
        void AddDiagnostic( DiagnosticMessage error );

        /// <summary>Add a range of errors to this node</summary>
        /// <param name="errors">Errors to attach to this node</param>
        void AddDiagnostics( IEnumerable<DiagnosticMessage> errors );

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <returns>Result of visiting this node</returns>
        TResult? Accept<TResult>( IAstVisitor<TResult> visitor );

        /// <summary>Visitor pattern support for implementations to dispatch the concrete node type to a visitor</summary>
        /// <typeparam name="TResult">Result type for the visitor</typeparam>
        /// <typeparam name="TArg">Type of the argument to pass on to the visitor</typeparam>
        /// <param name="visitor">Visitor to dispatch the concrete type to</param>
        /// <param name="arg">Argument to pass to the concrete type as a readonly ref</param>
        /// <returns>Result of visiting this node</returns>
        TResult? Accept<TResult, TArg>( IAstVisitor<TResult, TArg> visitor, ref readonly TArg arg )
#if NET9_0_OR_GREATER
        where TArg : struct
        , allows ref struct;
#else
        where TArg : struct;
#endif
    }
}

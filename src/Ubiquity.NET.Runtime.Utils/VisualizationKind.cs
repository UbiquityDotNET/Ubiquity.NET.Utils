// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    // TODO: Remove this, in favor of a more generalized "visualizer" that doesn't care about the form. Instead the REPL is provided
    // an ImmutableArray<IVisualizer> that can handle the various forms of input. (ISyntaxNode, IAstNode)...
    // see: https://github.com/UbiquityDotNET/Ubiquity.NET.Utils/issues/14

    /// <summary>Enumeration to define the kinds of diagnostic intermediate data to generate from a runtime/language AST</summary>
    [Flags]
    public enum VisualizationKind
    {
        /// <summary>No Visualizations</summary>
        None,

        /// <summary>Generate an XML representation of the parse tree</summary>
        Xml,

        /// <summary>Generate a DGML representation of the parse tree</summary>
        Dgml,

        /// <summary>Generates a BlockDiag representation of the parse tree</summary>
        BlockDiag,

        /// <summary>Emits debug tracing during the parse to an attached debugger</summary>
        /// <remarsk>This is a NOP if no debugger is attached</remarsk>
        DebugTraceParser,

        /// <summary>Emit all visualizations</summary>
        All = Xml | Dgml | BlockDiag | DebugTraceParser
    }
}

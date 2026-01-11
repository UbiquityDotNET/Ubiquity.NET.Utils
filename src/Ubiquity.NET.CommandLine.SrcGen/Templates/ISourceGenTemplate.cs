// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen.Templates
{
    // Implementation Note: This is NOT in Ubiquity.NET.SrcGeneration to prevent the need for
    //                      that to take a dependency on Mirosoft.CodeAnalyis just for the SourceText
    //                      result. Though a version that returns a StringBuilder might be useful...

    /// <summary>Interface for a source generating template</summary>
    /// <remarks>
    /// This simple interface defines the minimal requirements of a source generating
    /// template. It is based on the common use of generated T4 wrapper classes and
    /// intended for compatibility with those. (Though it uses <see cref="GenerateText"/>
    /// instead of `TransformText` and that method returns <see cref="SourceText"/>
    /// instead of <see cref="string"/>) This aids in transitioning such implementations
    /// to this new form of templating, especially for Roslyn Source generators.
    /// </remarks>
    public interface ISourceGenTemplate
    {
        /// <summary>Transforms the input and properties for the template into a <see cref="SourceText"/></summary>
        /// <returns>Generated textual representation</returns>
        SourceText GenerateText( );
    }
}

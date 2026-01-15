// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis.Text;

namespace Ubiquity.NET.SourceGenerator.Test.Utils
{
    /// <summary>Record to hold the hint path and content of an expected source generation</summary>
    /// <param name="HintPath">Hint path for the file</param>
    /// <param name="Content">Expected content of the file</param>
    [SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Simple record" )]
    public readonly record struct ExpectedInfo( string HintPath, SourceText Content );
}

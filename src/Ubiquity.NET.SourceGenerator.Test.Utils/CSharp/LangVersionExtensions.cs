// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Ubiquity.NET.SourceGenerator.Test.Utils.CSharp
{
    /// <summary>Utility class to provide extensions to <see cref="LanguageVersion"/></summary>
    [SuppressMessage( "Design", "CA1034: Nested types should not be visible", Justification = "Bogus; extension see: https://github.com/dotnet/sdk/issues/51681, and https://github.com/dotnet/roslyn-analyzers/issues/7765" )]
    public static class LangVersionExtensions
    {
        extension( LanguageVersion self )
        {
            /// <summary>Gets the default nullability for a specific language</summary>
            /// <remarks>Any unknown values for language version gets a result of a default <see cref="NullableContextOptions.Disable"/></remarks>
            public NullableContextOptions DefaultNullability
                => self switch
                {
                    LanguageVersion.CSharp1 or
                    LanguageVersion.CSharp2 or
                    LanguageVersion.CSharp3 or
                    LanguageVersion.CSharp4 or
                    LanguageVersion.CSharp5 or
                    LanguageVersion.CSharp6 or
                    LanguageVersion.CSharp7 or
                    LanguageVersion.CSharp7_1 or
                    LanguageVersion.CSharp7_2 or
                    LanguageVersion.CSharp7_3 => NullableContextOptions.Disable,

                    LanguageVersion.CSharp8 or
                    LanguageVersion.CSharp9 or
                    LanguageVersion.CSharp10 or
                    LanguageVersion.CSharp11 or
                    LanguageVersion.CSharp12 or
                    LanguageVersion.CSharp13 or
                    LanguageVersion.CSharp14 => NullableContextOptions.Enable,
                    _ => NullableContextOptions.Disable,
                };
        }
    }
}

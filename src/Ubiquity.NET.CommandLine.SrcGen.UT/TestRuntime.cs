// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.ComponentModel;

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    /// <summary>Test enumeration for use as a data row to test the component for different runtimes</summary>
    public enum TestRuntime
    {
        /// <summary>.NET 8.0 runtime</summary>
        Net8_0,

        /// <summary>.NET 10.0 runtime</summary>
        Net10_0
    }

    [SuppressMessage( "StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Extensions of Enum" )]
    internal static class TestRuntimeExtensions
    {
        extension(TestRuntime self)
        {
            // Analyzer tests use unresolved references
            internal Microsoft.CodeAnalysis.Testing.ReferenceAssemblies ReferenceAssemblies
                => self switch
                {
                    TestRuntime.Net8_0 => ReferenceAssemblies.Net.Net80,
                    TestRuntime.Net10_0 => TestHelpers.Net10,
                    _ => throw new InvalidEnumArgumentException( nameof( self ), (int)self, typeof( TestRuntime ) ),
                };

            internal LanguageVersion DefaultLangVersion
                => self switch
                {
                    TestRuntime.Net8_0 => LanguageVersion.CSharp12,
                    TestRuntime.Net10_0 => LanguageVersion.CSharp14,
                    _ => throw new InvalidEnumArgumentException( nameof( self ), (int)self, typeof( TestRuntime ) ),
                };
        }
    }
}

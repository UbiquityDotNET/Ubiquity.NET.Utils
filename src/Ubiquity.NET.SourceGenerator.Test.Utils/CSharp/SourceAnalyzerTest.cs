// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Ubiquity.NET.SourceGenerator.Test.Utils.CSharp
{
    /// <summary>Source analyzer tests for C# that allows specification of the language</summary>
    /// <typeparam name="TAnalyzer">Analyzer type</typeparam>
    /// <typeparam name="TVerifier">Verifier type</typeparam>
    public class SourceAnalyzerTest<TAnalyzer, TVerifier> : AnalyzerTest<TVerifier>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TVerifier : IVerifier, new()
    {
        /// <summary>Initializes a new instance of the <see cref="SourceAnalyzerTest{TAnalyzer, TVerifier}"/> class.</summary>
        /// <param name="languageVersion">Version of the language for the testing</param>
        /// <param name="nullableContextOptions">Nullability option to use for the test compilation</param>
        public SourceAnalyzerTest(LanguageVersion? languageVersion = null, NullableContextOptions? nullableContextOptions = null)
        {
            // .NET standard 2.0 is the lowest still supported, which uses C# 7.3
            LanguageVersion = languageVersion ?? LanguageVersion.CSharp7_3;
            NullableContextOptions = nullableContextOptions ?? LanguageVersion.DefaultNullability;
        }

        /// <inheritdoc/>
        protected override string DefaultFileExt => "cs";

        /// <inheritdoc/>
        public override string Language => LanguageNames.CSharp;

        /// <summary>Gets the version of the language to use for the tests</summary>
        public LanguageVersion LanguageVersion { get; }

        /// <summary>Gets the options that determines how nullability is handled</summary>
        public NullableContextOptions NullableContextOptions { get; }

        /// <inheritdoc/>
        protected override CompilationOptions CreateCompilationOptions( )
            => new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions );

        /// <inheritdoc/>
        protected override ParseOptions CreateParseOptions( )
            => new CSharpParseOptions( LanguageVersion, DocumentationMode.Diagnose );

        /// <inheritdoc/>
        protected override IEnumerable<DiagnosticAnalyzer> GetDiagnosticAnalyzers( )
            => [new TAnalyzer()];
    }
}

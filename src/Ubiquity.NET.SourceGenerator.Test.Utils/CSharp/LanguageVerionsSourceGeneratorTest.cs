// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace Ubiquity.NET.SourceGenerator.Test.Utils.CSharp
{
    /// <summary>Source generator tests for C# that allows specification of the language</summary>
    /// <typeparam name="TSourceGenerator">Source generator type</typeparam>
    /// <typeparam name="TVerifier">Verifier type</typeparam>
    /// <remarks>
    /// This type is generally used as a base class for language specific test types. It doesn't include logic/overloads
    /// to test for cached results, which is an important part of testing a source generator. The design of the
    /// <see cref="CSharpSourceGeneratorTest{TSourceGenerator, TVerifier}"/> assumes only a single pass which isn't
    /// sufficient for verification of cached state. For full cached testing use <see cref="CachedSourceGeneratorTest{TGenerator, TVerifier}"/>
    /// which uses this as a base.
    /// </remarks>
    public class LanguageVerionsSourceGeneratorTest<TSourceGenerator, TVerifier>
        : CSharpSourceGeneratorTest<TSourceGenerator, TVerifier>
        where TSourceGenerator : new()
        where TVerifier : IVerifier, new()
    {
        /// <summary>Initializes a new instance of the <see cref="LanguageVerionsSourceGeneratorTest{TSourceGenerator, TVerifier}"/> class.</summary>
        /// <param name="ver">Version of the language for this test</param>
        /// <param name="nullableContextOptions">Nullability context to use for compilation [default: is based on <paramref name="ver"/>]</param>
        public LanguageVerionsSourceGeneratorTest(LanguageVersion ver, NullableContextOptions? nullableContextOptions = null)
        {
            LanguageVersion = ver;
            NullableContextOptions = nullableContextOptions ?? LanguageVersion.DefaultNullability;
        }

        /// <summary>Gets the version of the language to use for the tests</summary>
        public LanguageVersion LanguageVersion { get; }

        /// <summary>Gets the options that determines how nullability is handled</summary>
        public NullableContextOptions NullableContextOptions { get; }

        /// <inheritdoc/>
        protected override ParseOptions CreateParseOptions( )
        {
            return ((CSharpParseOptions)base.CreateParseOptions()).WithLanguageVersion(LanguageVersion);
        }

        /// <inheritdoc/>
        protected override CompilationOptions CreateCompilationOptions( )
        {
            return new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true, nullableContextOptions: NullableContextOptions );
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    [TestClass]
    public sealed class RootCommandAttributeTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Basic_golden_path_succeeds( )
        {
            var sourceGenerator = new CommandGenerator().AsSourceGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: [sourceGenerator],
                driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
            );

            SourceText input = TestHelpers.GetTestText(nameof(RootCommandAttributeTests), "input.cs");
            SourceText expected = TestHelpers.GetTestText(nameof(RootCommandAttributeTests), "expected.cs");

            CSharpCompilation compilation = CreateCompilation(input, TestProgramCSPath);
            var diagnostics = compilation.GetDiagnostics( TestContext.CancellationToken );
            foreach(var diagnostic in diagnostics)
            {
                TestContext.WriteLine( diagnostic.ToString() );
            }

            Assert.HasCount( 0, diagnostics );

            var results = driver.RunGeneratorAndAssertResults(compilation, [TrackingNames.CommandClass]);
            Assert.IsEmpty( results.Diagnostics, "Should not have ANY diagnostics reported during generation" );

            // validate the generated trees have the correct count and names
            Assert.HasCount( 1, results.GeneratedTrees, "Should create 1 'files' during generation" );
            for(int i = 0; i < results.GeneratedTrees.Length; ++i)
            {
                string expectedName = GeneratedFilePaths[i];
                SyntaxTree tree = results.GeneratedTrees[i];

                Assert.AreEqual( expectedName, tree.FilePath, "Generated files should use correct name" );
                Assert.AreEqual( Encoding.UTF8, tree.Encoding, $"Generated files should use UTF8. [{expectedName}]" );
            }

            SourceText actual = results.GeneratedTrees[0].GetText( TestContext.CancellationToken );
            string uniDiff = expected.UniDiff(actual);
            if(!string.IsNullOrWhiteSpace( uniDiff ))
            {
                TestContext.WriteLine( uniDiff );
                Assert.Fail( "No Differences Expected" );
            }
        }

        // simple helper for these tests to create a C# Compilation
        internal static CSharpCompilation CreateCompilation(
            SourceText source,
            string path,
            CSharpParseOptions? parseOptions = default,
            CSharpCompilationOptions? compileOptions = default,
            List<MetadataReference>? references = default
            )
        {
            parseOptions ??= new CSharpParseOptions(LanguageVersion.CSharp14, DocumentationMode.None);
            compileOptions ??= new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable );

            // Default to .NET 10 if not specified.
            references ??= [ .. Net100.References.All ];
            references.Add( MetadataReference.CreateFromFile( Path.Combine( Environment.CurrentDirectory, "Ubiquity.NET.CommandLine.dll" ) ) );

            return CSharpCompilation.Create( "TestAssembly",
                                             [ CSharpSyntaxTree.ParseText( source, parseOptions, path ) ],
                                             references,
                                             compileOptions
                                           );
        }

        private const string TestProgramCSPath = @"input.cs";

        private readonly ImmutableArray<string> GeneratedFilePaths
        = [
            @"Ubiquity.NET.CommandLine.SrcGen\Ubiquity.NET.CommandLine.SrcGen.CommandGenerator\TestNamespace.TestOptions.g.cs",
        ];
    }
}

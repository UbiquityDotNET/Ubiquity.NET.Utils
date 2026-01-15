// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    [TestClass]
    public sealed class RootCommandAttributeTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task Basic_golden_path_succeeds( TestRuntime testRuntime )
        {
            const string inputFileName = "input.cs";
            const string expectedFileName = "expected.cs";
            string hintPath = Path.Combine("Ubiquity.NET.CommandLine.SrcGen", "Ubiquity.NET.CommandLine.SrcGen.CommandGenerator", "TestNamespace.TestOptions.g.cs");

            SourceText input = GetSourceText( nameof(Basic_golden_path_succeeds), inputFileName );
            SourceText expected = GetSourceText( nameof(Basic_golden_path_succeeds), expectedFileName );

            var runner = CreateTestRunner(input, testRuntime, [TrackingNames.CommandClass], hintPath, expected );
            await runner.RunAsync( TestContext.CancellationToken );
        }

        private SourceGeneratorTest<MsTestVerifier> CreateTestRunner(
            SourceText source,
            TestRuntime testRuntime,
            ImmutableArray<string> trackingNames,
            string expectedHintPath,
            SourceText expectedContent
            )
        {
            // Use a test runner with Caching, language and reference assemblies that match the runtime for the test run
            return new CachedSourceGeneratorTest<CommandGenerator, MsTestVerifier>( trackingNames, testRuntime.DefaultLangVersion )
            {
                TestState =
                {
                    Sources = { source },
                    ReferenceAssemblies = testRuntime.ReferenceAssemblies,
                    AdditionalReferences =
                    {
                        TestContext.GetUbiquityNetCommandLineLib( testRuntime )
                    },
                    OutputKind = OutputKind.DynamicallyLinkedLibrary, // Don't require a Main() method
                    GeneratedSources = { (expectedHintPath, expectedContent) }
                },

                // Allow ALL diagnostics for testing, input source should contain valid C# code
                // but might otherwise trigger the tested analyzer.
                CompilerDiagnostics = CompilerDiagnostics.All,
            };
        }

        private static SourceText GetSourceText(params string[] nameParts)
        {
            return TestHelpers.GetTestText( nameof( RootCommandAttributeTests ), nameParts );
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    [TestClass]
    public class CommandAnalyzerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task Empty_source_analyzes_clean( TestRuntime testRuntime )
        {
            var analyzerTest = CreateTestRunner( string.Empty, testRuntime );
            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task Option_attribute_without_command_triggers_diagnostic( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(Option_attribute_without_command_triggers_diagnostic), "input.cs" );
            var analyzerTest = CreateTestRunner( txt, testRuntime );

            // (9,6): warning UNC001: Property attribute OptionAttribute is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
            analyzerTest.ExpectedDiagnostics.AddRange(
               [
                   new DiagnosticResult("UNC001", DiagnosticSeverity.Error)
                       .WithArguments("OptionAttribute")
                       .WithSpan(9, 6, 9, 93),
               ]
           );

            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task FileValidation_attribute_without_command_triggers_diagnostic( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(FileValidation_attribute_without_command_triggers_diagnostic), "input.cs" );
            var analyzerTest = CreateTestRunner( txt, testRuntime );

            // (9,6): warning UNC001: Property attribute OptionAttribute is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
            // (10,6): warning UNC001: Property attribute FileValidationAttribute is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
            analyzerTest.ExpectedDiagnostics.AddRange(
               [
                   new DiagnosticResult("UNC001", DiagnosticSeverity.Error)
                      .WithArguments("OptionAttribute")
                      .WithSpan(9, 6, 9, 51),

                   new DiagnosticResult("UNC001", DiagnosticSeverity.Error)
                       .WithArguments("FileValidationAttribute")
                       .WithSpan(10, 6, 10, 51),
               ]
           );

            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task FolderValidation_attribute_without_command_triggers_diagnostic( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(FolderValidation_attribute_without_command_triggers_diagnostic), "input.cs" );
            var analyzerTest = CreateTestRunner( txt, testRuntime );

            // (11,6): error UNC001: Property attribute FolderValidationAttribute is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
            // (11,6): error UNC002: Property attribute FolderValidationAttribute is not allowed on a property independent of a qualifying attribute such as OptionAttribute.
            analyzerTest.ExpectedDiagnostics.AddRange(
               [
                   new DiagnosticResult("UNC001", DiagnosticSeverity.Error)
                       .WithArguments("FolderValidationAttribute")
                       .WithSpan(11, 6, 11, 55),

                   new DiagnosticResult("UNC002", DiagnosticSeverity.Error)
                       .WithArguments("FolderValidationAttribute")
                       .WithSpan(11, 6, 11, 55),
               ]
             );

            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task GoldenPath_produces_no_diagnostics( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(GoldenPath_produces_no_diagnostics), "input.cs" );

            var analyzerTest = CreateTestRunner( txt, testRuntime );
            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task FileValidation_with_wrong_type_produces_UNC002( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(FileValidation_with_wrong_type_produces_UNC002), "input.cs" );
            var analyzerTest = CreateTestRunner( txt, testRuntime );

            // (9,6): error UNC003: Property attribute '{0}' requires a property of type '{1}'.
            analyzerTest.ExpectedDiagnostics.AddRange(
               [
                   new DiagnosticResult("UNC003", DiagnosticSeverity.Error)
                      .WithArguments("FileValidationAttribute", "System.IO.FileInfo")
                      .WithSpan(9, 6, 9, 51),
               ]
             );

            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        [TestMethod]
        [DataRow( TestRuntime.Net8_0 )]
        [DataRow( TestRuntime.Net10_0 )]
        public async Task Required_nullable_types_produce_diagnostic( TestRuntime testRuntime )
        {
            SourceText txt = GetSourceText( nameof(Required_nullable_types_produce_diagnostic), "input.cs" );
            var analyzerTest = CreateTestRunner( txt, testRuntime );

            // (9,6): error UNC003: Property attribute '{0}' requires a property of type '{1}'.
            analyzerTest.ExpectedDiagnostics.AddRange(
               [
                   new DiagnosticResult("UNC004", DiagnosticSeverity.Warning)
                      .WithArguments("bool?", "Thing1")
                      .WithSpan(9, 6, 9, 127),
               ]
             );

            await analyzerTest.RunAsync( TestContext.CancellationToken );
        }

        // TODO: Test that a nullable value is not marked as required. (That's a conflicting claim, if it's required it can't be null)
        //       A nullable type MAY have a default value handler to provide a null default. Additional test - anything with a default
        //       value provider shouldn't be "required" it's also nonsensical.

        private AnalyzerTest<MsTestVerifier> CreateTestRunner( string source, TestRuntime testRuntime )
        {
            return CreateTestRunner( SourceText.From( source ), testRuntime );
        }

        private AnalyzerTest<MsTestVerifier> CreateTestRunner( SourceText source, TestRuntime testRuntime )
        {
            // Use a test analyzer with language and reference assemblies that match the runtime for the test run
            return new SourceAnalyzerTest<CommandLineAnalyzer, MsTestVerifier>( testRuntime.DefaultLangVersion )
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
                },

                // Allow ALL diagnostics for testing, input source should contain valid C# code
                // but might otherwise trigger the tested analyzer.
                CompilerDiagnostics = CompilerDiagnostics.All,

                // Add custom verification to handle cases not covered in default verifications
                DiagnosticVerifier = DiagnosticVerifier,
            };
        }

        /// <summary>Custom diagnostic verifier</summary>
        /// <param name="diagnostic">diagnostic to verify (actual)</param>
        /// <param name="result">Diagnostic result containing the expected results</param>
        /// <param name="verifier">Implementation of <see cref="IVerifier"/> to use for verification</param>
        /// <seealso href="https://github.com/dotnet/roslyn-sdk/issues/1246"/>
        private void DiagnosticVerifier( Diagnostic diagnostic, DiagnosticResult result, IVerifier verifier )
        {
            DiagnosticVerifiers.VerifyMessageArguments( diagnostic, result, verifier );

            // verify the help URI matches the expected form
            verifier.EqualOrDiff( diagnostic.Descriptor.HelpLinkUri, FormatHelpUri( diagnostic.Id ) );
        }

        private static string FormatHelpUri( string id )
        {
            return $"https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/{id}.html";
        }

        private static SourceText GetSourceText( params string[] nameParts )
        {
            return TestHelpers.GetTestText( nameof( CommandAnalyzerTests ), nameParts );
        }
    }
}

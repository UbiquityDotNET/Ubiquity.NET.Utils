// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Model;
using Microsoft.CodeAnalysis.Text;

namespace Ubiquity.NET.SourceGenerator.Test.Utils.CSharp
{
    /// <summary><see cref="SourceGeneratorTest{TVerifier}"/> implementation that verifies a generator only uses cached values</summary>
    /// <typeparam name="TGenerator">Type of the generator; Must implement <see cref="IIncrementalGenerator"/></typeparam>
    /// <typeparam name="TVerifier">Type of the verifier; Must implement <see cref="IVerifier"/></typeparam>
    /// <remarks>
    /// <para>The verifier is used to adapt to a variety of test frameworks in a consistent manner. It is usually, <see cref="DefaultVerifier"/>
    /// but may be any type implementing the <see cref="IVerifier"/> interface.</para>
    /// <para>The <see cref="TrackingNames"/> are used to determine what is verified for cached results. Only the results with matching
    /// names is validated for cached results. All other diagnostics apply, that is even if <see cref="TrackingNames"/> is empty the
    /// diagnostics produced are still validated as well as the generated output. The cached behavior is only tested AFTER all the
    /// other test states are verified.</para>
    /// </remarks>
    public class CachedSourceGeneratorTest<TGenerator, TVerifier>
        : LanguageVerionsSourceGeneratorTest<TGenerator, TVerifier>
        where TGenerator : IIncrementalGenerator, new()
        where TVerifier : IVerifier, new()
    {
        /// <summary>Initializes a new instance of the <see cref="CachedSourceGeneratorTest{TGenerator, TVerifier}"/> class.</summary>
        /// <param name="trackingNames">Tracking names to validate for cached results (Others are not validated for that)</param>
        /// <param name="languageVersion">Version of the language for the generator</param>
        /// <param name="nullableContextOptions">Nullability behavior for this generator; default is to enable it for language versions that support it.</param>
        public CachedSourceGeneratorTest(
            ImmutableArray<string> trackingNames,
            LanguageVersion languageVersion,
            NullableContextOptions? nullableContextOptions = null
            )
            : base(languageVersion, nullableContextOptions)
        {
            TrackingNames = trackingNames;
        }

        /// <summary>Gets the tracking names to use for this test</summary>
        public ImmutableArray<string> TrackingNames { get; }

        /// <inheritdoc/>
        /// <remarks>This will run the test AND validate cached output</remarks>
        protected override async Task RunImplAsync( CancellationToken cancellationToken )
        {
            // This replicates much of what is in the base type in order to expose the driver
            // or, more particularly the driver results of multiple passes of the same compilation
            // to ensure that both runs produce the same results AND that the second run uses
            // only cached results.

            if(!TestState.GeneratedSources.Any())
            {
                // Verify the test state has at least one source, which may or may not be generated
                Verify.NotEmpty( $"{nameof( TestState )}.{nameof( SolutionState.Sources )}", TestState.Sources );
            }

            SolutionState testState = GetSolutionState();
            Project project = await CreateProjectAsync( testState, cancellationToken );

            Compilation compilation = await GetAndVerifyCompilation( project, cancellationToken );

            // clone for use in validating cached behavior later
            var compilationClone = compilation.Clone();

            var sourceGenerator = new TGenerator().AsSourceGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(
                generators: [sourceGenerator],
                driverOptions: new GeneratorDriverOptions(default, trackIncrementalGeneratorSteps: true)
            );

            // save the resulting immutable driver for use in second run.
            driver = driver.RunGenerators( compilation, cancellationToken );
            GeneratorDriverRunResult runResult1 = driver.GetRunResult();
            Verify.Empty( "Result diagnostics", runResult1.Diagnostics );

            // validate the generated trees have the correct count and names
            var expected = testState.GeneratedSources;
            Verify.Equal( expected.Count, runResult1.GeneratedTrees.Length, $"Should generate {expected.Count} 'files' during generation Actual count is {runResult1.GeneratedTrees.Length}." );
            VerifyResultsEqual( runResult1, expected, cancellationToken );
            VerifyCached( driver, compilationClone, runResult1, cancellationToken );
        }

        private static async Task<Compilation> GetAndVerifyCompilation( Project project, CancellationToken cancellationToken )
        {
            Verify.True( project.SupportsCompilation, "Project must support compilation for testing" );

            // Previous Assertion validates compilation won't be null.
            var compilation = (await project.GetCompilationAsync( cancellationToken ))!;

            var diagnostics = compilation.GetDiagnostics( cancellationToken );
            var bldr = new StringBuilder();
            foreach(var diagnostic in diagnostics)
            {
                bldr.AppendLine( diagnostic.ToString() );
            }

            // Report compilation diagnostics as an error
            Verify.Equal( 0, diagnostics.Length, bldr.ToString() );
            return compilation;
        }

        private static void VerifyResultsEqual( GeneratorDriverRunResult runResult1, SourceFileCollection expected, CancellationToken cancellationToken )
        {
            for(int i = 0; i < expected.Count; ++i)
            {
                (string hintPath, SourceText expectedText) = expected[ i ];

                SyntaxTree tree = runResult1.GeneratedTrees[i];

                Verify.Equal( hintPath, tree.FilePath, "Generated files should use correct name" );
                Verify.Equal( Encoding.UTF8, tree.Encoding, $"Generated files should use UTF8. [{hintPath}]" );

                SourceText actualText = runResult1.GeneratedTrees[i].GetText( cancellationToken );
                Verify.AreEqual( expectedText, actualText, "Generated source should match expected content" );
            }
        }

        private void VerifyCached( GeneratorDriver driver, Compilation compilationClone, GeneratorDriverRunResult runResult1, CancellationToken cancellationToken )
        {
            if(!TrackingNames.IsDefaultOrEmpty)
            {
                GeneratorDriverRunResult runResult2 = driver.RunGenerators( compilationClone, cancellationToken )
                                                            .GetRunResult();

                Verify.AreEqual( runResult1, runResult2, TrackingNames );
                Verify.Cached( runResult2 );
            }
        }

        private Task<Project> CreateProjectAsync( SolutionState testState, CancellationToken cancellationToken )
        {
            var evaluatedState = new EvaluatedProjectState( testState, ReferenceAssemblies );
            var additionalProjects = ( from proj in TestState.AdditionalProjects.Values
                                       select new EvaluatedProjectState(proj, ReferenceAssemblies)
                                     ).ToImmutableArray();

            return CreateProjectAsync( evaluatedState, additionalProjects, cancellationToken );
        }

        private SolutionState GetSolutionState( )
        {
            var analyzers = GetDiagnosticAnalyzers().ToArray();
            var defaultDiagnostic = GetDefaultDiagnostic(analyzers);
            var supportedDiagnostics = analyzers.SelectMany(analyzer => analyzer.SupportedDiagnostics)
                                                .ToImmutableArray();

            var fixableDiagnostics = ImmutableArray<string>.Empty;
            return TestState.WithInheritedValuesApplied(null, fixableDiagnostics)
                            .WithProcessedMarkup(MarkupOptions, defaultDiagnostic, supportedDiagnostics, fixableDiagnostics, DefaultFilePath);
        }
    }
}

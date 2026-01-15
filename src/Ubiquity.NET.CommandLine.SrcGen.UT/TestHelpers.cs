// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.ComponentModel;

using ReferenceAssemblies = Microsoft.CodeAnalysis.Testing.ReferenceAssemblies;

namespace Ubiquity.NET.CommandLine.SrcGen.UT
{
    internal static class TestHelpers
    {
        public static Stream GetTestResourceStream( string testName, string name )
        {
            // Example:
            // Ubiquity.NET.CommandLine.SrcGen.UT.TestFiles.RootCommandAttributeTests.expected.cs
            // nameof(T) doesn't work for the namespace, so use reflection information to form the
            // name with a well defined pattern. (refactoring safe)
            var myType = typeof(TestHelpers);
            string resourceName = $"{myType.Namespace}.{TestFilesFolderName}.{testName}.{name}";
            return myType.Assembly.GetManifestResourceStream( resourceName )
                ?? throw new InvalidOperationException( $"Resource '{resourceName}' not found" );
        }

        public static SourceText GetTestText( string testName, params string[] nameParts )
        {
            if(nameParts.Length == 0)
            {
                throw new ArgumentException( "Must include at least one name part", nameof( nameParts ) );
            }

            using var strm = GetTestResourceStream( testName, string.Join(".", nameParts) );
            return SourceText.From( strm );
        }

        extension( TestContext self )
        {
            internal string BuildOutputBinPath
            {
                get
                {
                    string runDir = self.TestRunDirectory ?? throw new InvalidOperationException("No test run directory");
                    return Path.GetFullPath( Path.Combine( runDir, "..", "..", "bin" ) );
                }
            }

            public MetadataReference GetUbiquityNetCommandLineLib( TestRuntime testRuntime )
            {
                string runtimeName = testRuntime switch
                {
                    TestRuntime.Net8_0 => "net8.0",
                    TestRuntime.Net10_0 => "net10.0",
                    _ => throw new InvalidEnumArgumentException(nameof(testRuntime), (int)testRuntime, typeof(TestRuntime))
                };

                string pathName = Path.Combine( self.BuildOutputBinPath, "Ubiquity.NET.CommandLine", ConfigName, runtimeName, "Ubiquity.NET.CommandLine.dll" );
                return MetadataReference.CreateFromFile( pathName );
            }
        }

        #region .NET 10 Reference Assemblies

        /// <summary>Gets the .NET 10 reference assemblies</summary>
        /// <remarks>
        /// Sadly, Microsoft.CodeAnalysis.Testing.ReferenceAssemblies does not contain any reference for .NET 10
        /// (even the latest version of that lib)
        /// </remarks>
        /// <seealso href="https://github.com/dotnet/roslyn-sdk/issues/1233"/>
        public static ReferenceAssemblies Net10 => LazyNet10Refs.Value;

        private static readonly Lazy<ReferenceAssemblies> LazyNet10Refs = new(
            static ()=> new(
                targetFramework: "net10.0",
                referenceAssemblyPackage: new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0"),
                referenceAssemblyPath: Path.Combine("ref", "net10.0")
            )
        );
        #endregion

        // There is no way to detect the configuration at runtime to include the correct
        // reference to the DLL so use the compiler define to do the best available.
#if DEBUG
        private const string ConfigName = "Debug";
#else
        private const string ConfigName = "Release";
#endif
        private const string TestFilesFolderName = "TestFiles";
    }
}

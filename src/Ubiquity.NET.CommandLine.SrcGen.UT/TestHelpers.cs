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
                    string retVal = Path.GetFullPath( Path.Combine( runDir, "..", "..", "bin" ) );
                    return Directory.Exists(retVal)
                        ? retVal
                        : throw new DirectoryNotFoundException( $"Directory not found: '{retVal}'" );
                }
            }

            public MetadataReference GetUbiquityNetCommandLineLib( TestRuntime testRuntime )
            {
                return self.GetDependentLib( testRuntime, "Ubiquity.NET.CommandLine" );
            }

            public MetadataReference GetUbiquityNetExtensionsLib( TestRuntime testRuntime )
            {
                return self.GetDependentLib( testRuntime, "Ubiquity.NET.Extensions" );
            }

            public MetadataReference GetDependentLib( TestRuntime testRuntime, string targetName )
            {
                string runtimePathPartName = testRuntime switch
                {
                    TestRuntime.Net8_0 => "net8.0",
                    TestRuntime.Net10_0 => "net10.0",
                    _ => throw new InvalidEnumArgumentException(nameof(testRuntime), (int)testRuntime, typeof(TestRuntime))
                };

                string pathName = Path.Combine( self.BuildOutputBinPath, targetName, ConfigName, runtimePathPartName, $"{targetName}.dll" );
                return MetadataReference.CreateFromFile( pathName );
            }
        }

        // There is no way to detect the configuration at runtime to include the correct
        // reference to an assembly so use the compiler define to do the best available.
#if DEBUG
        private const string ConfigName = "Debug";
#else
        private const string ConfigName = "Release";
#endif
        private const string TestFilesFolderName = "TestFiles";
    }
}

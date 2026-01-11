// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

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

        public static SourceText GetTestText( string testName, string name )
        {
            using var strm = GetTestResourceStream( testName, name );
            return SourceText.From( strm );
        }

        private const string TestFilesFolderName = "TestFiles";
    }
}

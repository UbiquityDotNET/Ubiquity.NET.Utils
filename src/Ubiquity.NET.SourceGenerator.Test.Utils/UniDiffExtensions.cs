// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using DiffPlex.Renderer;

using Microsoft.CodeAnalysis.Text;

namespace Ubiquity.NET.SourceGenerator.Test.Utils
{
    internal static class UniDiffExtensions
    {
        public static string UniDiff( this SourceText self, SourceText other, string leftFileName = "expected", string rightFileName = "actual" )
        {
            return UniDiff( self.ToString(), other, leftFileName, rightFileName );
        }

        public static string UniDiff( this string self, SourceText other, string leftFileName = "expected", string rightFileName = "actual" )
        {
            var renderer = new UnidiffRenderer(contextLines: 1);
            return renderer.Generate(
                       self,
                       other.ToString(),
                       leftFileName,
                       rightFileName,
                       ignoreWhitespace: false
                   );
        }
    }
}

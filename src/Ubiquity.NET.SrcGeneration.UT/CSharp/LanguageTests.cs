// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.SrcGeneration.UT.CSharp
{
    [TestClass]
    public class LanguageTests
    {
        [TestMethod]
        public void Literal_values_are_correct( )
        {
            Assert.AreEqual( "{", CSharpLanguage.ScopeOpen );
            Assert.AreEqual( "}", CSharpLanguage.ScopeClose );

            Assert.HasCount( ExpectedKeywords.Length, CSharpLanguage.KeyWords );
            Assert.IsTrue( ExpectedKeywords.SequenceEqual( CSharpLanguage.KeyWords ) );

            Assert.AreEqual( "true", CSharpLanguage.AsLiteral( true ) );
            Assert.AreEqual( "false", CSharpLanguage.AsLiteral( false ) );

            // Escaping of strings is... challenging. The escaped literal form
            // includes the escaping characters so testing for that requires escaping the escape chars...
            Assert.AreEqual( "\"this is a \\\"test\\\"\"", CSharpLanguage.AsLiteral( "this is a \"test\"" ) );

            Assert.AreEqual( "'\u0124'", CSharpLanguage.AsLiteral( '\u0124' ) );
            Assert.AreEqual( "'\\u2028'", CSharpLanguage.AsLiteral( '\u2028' ) );

            Assert.AreEqual( $"@{ExpectedKeywords[ 0 ]}", CSharpLanguage.MakeIdentifier( ExpectedKeywords[ 0 ] ) );
            Assert.AreEqual( "foo_bar", CSharpLanguage.MakeIdentifier( "foo bar" ) );
        }

        private readonly ImmutableArray<string> ExpectedKeywords
            = [ // Source: Language spec. §6.4.4 Keywords
                "abstract",
                "as",
                "base",
                "bool",
                "break",
                "byte",
                "case",
                "catch",
                "char",
                "checked",
                "class",
                "const",
                "continue",
                "decimal",
                "default",
                "delegate",
                "do",
                "double",
                "else",
                "enum",
                "event",
                "explicit",
                "extern",
                "false",
                "finally",
                "fixed",
                "float",
                "for",
                "foreach",
                "goto",
                "if",
                "implicit",
                "in",
                "int",
                "interface",
                "internal",
                "is",
                "lock",
                "long",
                "namespace",
                "new",
                "null",
                "object",
                "operator",
                "out",
                "override",
                "params",
                "private",
                "protected",
                "public",
                "readonly",
                "ref",
                "return",
                "sbyte",
                "sealed",
                "short",
                "sizeof",
                "stackalloc",
                "static",
                "string",
                "struct",
                "switch",
                "this",
                "throw",
                "true",
                "try",
                "typeof",
                "uint",
                "ulong",
                "unchecked",
                "unsafe",
                "ushort",
                "using",
                "virtual",
                "void",
                "volatile",
                "while"
              ];
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using Microsoft.CodeAnalysis.CSharp;

namespace Ubiquity.NET.SrcGeneration.CSharp
{
    /// <summary>Support for generating source files in the C# language</summary>
    public static class CSharpLanguage
    {
        /// <summary>Opening of a scope for C#</summary>
        public const string ScopeOpen = "{";

        /// <summary>Closing of a scope for C#</summary>
        public const string ScopeClose = "}";

        /// <summary>Gets a literal value for a <see cref="bool"/> that is specific to the C# language</summary>
        /// <param name="value">value to get as a literal</param>
        /// <returns>literal string suitable for output to a writer for the C# language as-is</returns>
        public static string AsLiteral(bool value)
        {
            return SymbolDisplay.FormatPrimitive( value, quoteStrings: false, useHexadecimalNumbers: false )
                ?? throw new NotSupportedException("Internal error, bool formatting is not supported!?");
        }

        /// <summary>Gets a literal value for a <see cref="string"/> that is specific to the C# language</summary>
        /// <param name="value">value to get as a literal</param>
        /// <returns>literal string suitable for output to a writer for the C# language as-is</returns>
        /// <remarks>This, basically, surrounds <paramref name="value"/> with quotes, while handling any escape characters etc...</remarks>
        public static string AsLiteral( string value )
        {
            return SymbolDisplay.FormatLiteral( value, true );
        }

        /// <summary>Generates a literal for the character, which may require escaping to form a proper literal</summary>
        /// <param name="value">Character value to make as a literal</param>
        /// <returns>Literal as a string</returns>
        public static string AsLiteral( char value )
        {
            return SymbolDisplay.FormatLiteral( value, true );
        }

        /// <summary>Gets the language keywords</summary>
        /// <remarks>
        /// This is normally used from within <see cref="MakeIdentifier(string)"/>
        /// to escape keywords as identifiers. But is available for any use.
        /// </remarks>
        public static ImmutableArray<string> KeyWords { get; }
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

        /// <summary>Makes an identifier (Escaping a language keyword)</summary>
        /// <param name="self">identifier string to convert</param>
        /// <returns>Syntactically valid identifier</returns>
        /// <remarks>
        /// Current implementation simplistically performs keyword escaping AND
        /// conversion of space to `'_'`. Specifically, it does NOT (yet anyway)
        /// validate that the result satisfies the language definition of an
        /// identifier (which limits the characters allowed and further restricts
        /// the first such character)
        /// </remarks>
        public static string MakeIdentifier( this string self )
        {
#if NET8_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self );
#else
            PolyFillExceptionValidators.ThrowIfNull( self );
#endif

            // always replace invalid characters
            // TODO: more sophisticated Regex that matches anything NOT a valid identifier char
#if NETSTANDARD2_0
            string retVal = self.Replace( " ", "_" );
#else
            string retVal = self.Replace( " ", "_", StringComparison.Ordinal );
#endif
            return KeyWords.Contains( self )
                    ? $"@{retVal}"
                    : retVal;
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

// from .NET sources
// see: https://github.com/dotnet/runtime/blob/1d1bf92fcf43aa6981804dc53c5174445069c9e4/src/libraries/System.Private.CoreLib/src/System/String.Manipulation.cs

#pragma warning disable IDE0130 // Namespace does not match folder structure

#pragma warning disable IDE0005 // Using directive is unnecessary; (No it isn't!)
// Without this the compiler will try to resolve to an internal type and report in-accessibility.
using Ubiquity.NET.Extensions;
#pragma warning restore IDE0005

namespace System
{
    /// <summary>Polyfill extensions for support not present in older runtimes</summary>
    /// <inheritdoc cref="Ubiquity.NET.Extensions.Requires" path="/remarks"/>
    public static class PolyFillStringExtensions
    {
#if NETSTANDARD2_0
        /// <summary>Concatenates the members of a collection, using the specified separator between each member.</summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="separator">The character to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <param name="values">A collection that contains the objects to concatenate.</param>
        /// <returns>
        /// A string that consists of the members of values delimited by the separator character.
        /// -or- System.String.Empty if values has no elements.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="values"/> is <see langword="null"/></exception>
        /// <exception cref="System.OutOfMemoryException">The length of the resulting string overflows the maximum allowed length (<see cref="Int32.MaxValue"/>).</exception>
        public static string Join<T>( char separator, IEnumerable<T> values )
        {
            return string.Join( separator.ToString(), values );
        }

        /// <summary>Gets the hash-code for a string using the specified comparison type</summary>
        /// <param name="self">String to get the code for</param>
        /// <param name="comparisonType">Comparison type to use [Only supports the default <see cref="StringComparison.Ordinal"/>]</param>
        /// <returns>Hash code value</returns>
        /// <exception cref="InvalidEnumArgumentException"><paramref name="comparisonType"/> is not the default <see cref="StringComparison.Ordinal"/></exception>
        public static int GetHashCode( this string self, StringComparison comparisonType )
        {
            return comparisonType == StringComparison.Ordinal
                ? self.GetHashCode()
                : throw new InvalidEnumArgumentException( nameof( comparisonType ), (int)comparisonType, typeof( StringComparison ) );
        }
#endif

#if !NET6_0_OR_GREATER
        /// <summary>Replace line endings in the string with environment specific forms</summary>
        /// <param name="self">string to change line endings for</param>
        /// <returns>string with environment specific line endings</returns>
        /// <remarks>
        /// This API will explicitly replace line endings using the Runtime newline format. In most cases that is
        /// what is desired. However, when generating files or content consumed by something other than the
        /// current runtime it is usually not what is desired. In such a case the more explicit <see cref="ReplaceLineEndings(string, string)"/>
        /// is used to specify the precise line ending form to use. (Or Better yet, use <c>Ubiquity.NET.Extensions.StringNormalizer.NormalizeLineEndings(string?, LineEndingKind))</c> />
        /// </remarks>
        [SuppressMessage( "MicrosoftCodeAnalysis", "RS1035:Banned Symbol", Justification = "This form explicitly uses the runtime form" )]
        public static string ReplaceLineEndings( this string self )
        {
            return ReplaceLineEndings( self, Environment.NewLine );
        }

        // This is NOT the most performant implementation, it's going for simplistic polyfill that has
        // the correct behavior, even if not the most performant. If performance is critical, use a
        // later version of the runtime!

        /// <summary>Replace line endings in the string with a given string</summary>
        /// <param name="self">string to change line endings for</param>
        /// <param name="replacementText">Text to replace all of the line endings in <paramref name="self"/></param>
        /// <returns>string with line endings replaced by <paramref name="replacementText"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static string ReplaceLineEndings( this string self, string replacementText )
        {
            Requires.NotNull( self );
            Requires.NotNull( replacementText );

            string retVal = UnicodeNewLinesRegex.Replace(self, replacementText);

            // if the result of replacement is the same, just return the original
            // This is wasted overhead, but at least matches the behavior
            return self == retVal ? self : retVal;
        }

        // The Unicode Standard, Sec. 5.8, Recommendation R4 and Table 5-2 state that the CR, LF,
        // CRLF, NEL, LS, FF, and PS sequences are considered newline functions. That section
        // also specifically excludes VT from the list of newline functions, so we do not include
        // it in the regular expression match.

        // language=regex
        private const string UnicodeNewLinesRegExPattern = @"(\r\n|\r|\n|\f|\u0085|\u2028|\u2029)";

        // language=regex
        private const string SystemNewLinesRegExPattern = @"(\r\n|\r|\n)";

        // NOTE: can't use source generated Regex here as there's no way to declare the dependency on
        // the output of one generator as the input for another. They all see the same input, therefore
        // the partial implementation would never be filled in and produces a compilation error instead.
        // Thus these use a lazy pattern to take the cost only once.
        internal static Regex UnicodeNewLinesRegex
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => LazyUnicodeNewLinesRegex.Value;
        }

        internal static Regex SystemNewLinesRegex
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => LazySystemNewLinesRegex.Value;
        }

        private static readonly Lazy<Regex> LazyUnicodeNewLinesRegex
            = new(() => new Regex( UnicodeNewLinesRegExPattern ));

        internal static readonly Lazy<Regex> LazySystemNewLinesRegex
            = new(() => new Regex( SystemNewLinesRegExPattern ));
#endif
    }
}

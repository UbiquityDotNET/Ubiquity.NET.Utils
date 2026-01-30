// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Utility class to provide extensions for <see cref="string"/></summary>
    public static partial class StringExtensions
    {
        /// <summary>Tests if a string contains any line endings</summary>
        /// <param name="self">String to test</param>
        /// <param name="skipUnicodeLineEndigs">Indicates whether additional Unicode line endings are considered [default: false]</param>
        /// <returns><see langword="true"/> if the string contains any line endings</returns>
        public static bool HasLineEndings( this string self, bool skipUnicodeLineEndigs = false )
        {
            Requires.NotNull( self );
#if NETSTANDARD2_0
            return skipUnicodeLineEndigs
                 ? PolyFillStringExtensions.SystemNewLinesRegex.IsMatch(self)
                 : PolyFillStringExtensions.UnicodeNewLinesRegex.IsMatch(self);
#else
            return skipUnicodeLineEndigs
                 ? SystemNewLinesRegex.IsMatch(self)
                 : UnicodeNewLinesRegex.IsMatch(self);
#endif
        }

        // Since the poly fills are only applied for .NET Standard 2.0 this has to
        // replicate the new line Regular expressions for later runtimes.
#if !NETSTANDARD2_0
        // The Unicode Standard, Sec. 5.8, Recommendation R4 and Table 5-2 state that the CR, LF,
        // CRLF, NEL, LS, FF, and PS sequences are considered newline functions. That section
        // also specifically excludes VT from the list of newline functions, so we do not include
        // it in the regular expression match.

        // language=regex
        private const string UnicodeNewLinesRegExPattern = @"(\r\n|\r|\n|\f|\u0085|\u2028|\u2029)";

        // language=regex
        private const string SystemNewLinesRegExPattern = @"(\r\n|\r|\n)";

    #if NET9_0_OR_GREATER
        // Source generator for .NET 9+ understands properties directly
        [GeneratedRegex( UnicodeNewLinesRegExPattern )]
        internal static partial Regex UnicodeNewLinesRegex { get; }

        // Source generator for .NET 9+ understands properties directly
        [GeneratedRegex( SystemNewLinesRegExPattern )]
        internal static partial Regex SystemNewLinesRegex { get; }
    #else
        // The generated method uses a static instance already, so
        // this syntactic sugar should be aggressively inlined to
        // avoid even tail-call overhead.
        internal static Regex UnicodeNewLinesRegex
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => GetUnicodeNewLinesRegex();
        }

        [GeneratedRegex( UnicodeNewLinesRegExPattern )]
        private static partial Regex GetUnicodeNewLinesRegex();

        internal static Regex SystemNewLinesRegex
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => GetSystemNewLinesRegex();
        }

        [GeneratedRegex( SystemNewLinesRegExPattern )]
        private static partial Regex GetSystemNewLinesRegex();
    #endif
#endif
    }
}

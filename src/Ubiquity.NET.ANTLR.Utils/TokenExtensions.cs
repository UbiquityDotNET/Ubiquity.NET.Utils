// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Extension methods for an <see cref="IToken"/></summary>
    public static class TokenExtensions
    {
        private const int EoFTokenType = -1;

        /// <summary>Determines if a token is the EOF token</summary>
        /// <param name="t">Token to test</param>
        /// <returns><see langword="true"/> if the token is for an EOF</returns>
        public static bool IsEOF( this IToken t )
        {
            return t.Type == EoFTokenType;
        }

        /// <summary>Gets the length (in characters) of the token</summary>
        /// <param name="t">Token to get the length from</param>
        /// <returns>Length of the token</returns>
        public static int Length( this IToken t )
        {
            // StartIndex and StopIndex are INCLUSIVE
            // start(4) :--v v--: end(6) => 'ABC'
            //         0123ABC789
            return t.StopIndex - t.StartIndex + 1;
        }

        /// <summary>Gets the <see cref="SourceLocation"/> from an <see cref="IToken"/></summary>
        /// <param name="token">Token to get the location from</param>
        /// <returns><see cref="SourceLocation"/> of the token</returns>
        public static SourceLocation GetSourceLocation( this IToken token )
        {
            // Assumes tokens exclude newlines and are therefore on the same line.
            // This is a VERY reasonable assumption. In the unlikely event that
            // there is ever a language that includes newlines in a token, then
            // this would need to include counting the lines in the token to find
            // the total number of lines covered to get the end correct.
            // (+1 to column as SourceLocation uses a 1 based position, but ANTLR tokens are 0 based)
            return new SourceLocation(
                token.InputStream.SourceName,
                new SourcePosition( token.Line, token.Column + 1, token.StartIndex ),
                new SourcePosition( token.Line, token.Column + 1 + token.Length(), token.StopIndex )
                );
        }

        /// <summary>Gets the source name from an <see cref="IToken"/></summary>
        /// <param name="t">Token to get the source from</param>
        /// <returns>Source name from the stream the token came from</returns>
        public static string GetSourceFileName( this IToken t )
        {
            return t.InputStream.SourceName;
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.
#if NETSTANDARD2_0
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Text
{
    /// <summary>Utility class to host extensions for poly filling the existing <see cref="Encoding"/> class</summary>
    public static class PolyFillEncodingExtensions
    {
        /// <summary>Decodes all the bytes in the specified byte span into a string.</summary>
        /// <param name="self">Encoding to extend</param>
        /// <param name="bytes">A read-only byte span to decode to a Unicode string</param>
        /// <returns>A string that contains the decoded bytes from the provided read-only span.</returns>
        /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/system.text.encoding.getstring"/>
        public static unsafe string GetString(this Encoding self, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesPtr = bytes)
            {
                return self.GetString(bytesPtr, bytes.Length);
            }
        }

        /// <summary>Encodes into a span of bytes a set of characters from the specified read-only span.</summary>
        /// <param name="self">Encoding to extend</param>
        /// <param name="chars">The span containing the set of characters to encode.</param>
        /// <param name="bytes">The byte span to hold the encoded bytes.</param>
        /// <returns>The number of encoded bytes.</returns>
        [SuppressMessage( "StyleCop.CSharp.LayoutRules", "SA1519:Braces should not be omitted from multi-line child statement", Justification = "multiple fixed statements" )]
        public static unsafe int GetBytes( this Encoding self, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            unsafe
            {
                fixed(char* pSrc = chars)
                {
                    fixed(byte* pDst = bytes)
                    {
                        return self.GetBytes( pSrc, bytes.Length, pDst, bytes.Length );
                    }
                }
            }
        }
    }
}
#endif

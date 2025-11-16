// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace System.Text
{
    internal static class PolyFillEncodingExtensions
    {
#if NETSTANDARD2_0
        public static unsafe string GetString(this Encoding self, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* bytesPtr = bytes)
            {
                return self.GetString(bytesPtr, bytes.Length);
            }
        }

        public static unsafe int GetBytes( this Encoding self, ReadOnlySpan<char> bytes, Span<byte> dest)
        {
            fixed(char* pSrc = bytes)
            {
                fixed(byte* pDst = dest)
                {
                    return self.GetBytes( pSrc, bytes.Length, pDst, dest.Length );
                }
            }
        }
#endif
    }
}

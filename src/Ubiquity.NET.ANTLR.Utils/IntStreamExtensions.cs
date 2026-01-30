// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.
namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Extensions for an ANTLR int stream</summary>
    public static class IntStreamExtensions
    {
        /// <summary>Gets a string for a potentially escaped character from a stream at a given index</summary>
        /// <param name="strm">Stream to get the text from (Must also implement ICharStream)</param>
        /// <param name="index">Index in the stream to get the character</param>
        /// <returns>String representing the character or Empty if out of bounds</returns>
        public static string GetAt( this IIntStream strm, int index )
        {
            return ((ICharStream)strm).GetAt( index );
        }

        /// <summary>Gets a string for a potentially escaped character from a stream at a given index</summary>
        /// <param name="strm">Stream to get the text from</param>
        /// <param name="index">Index in the stream to get the character</param>
        /// <returns>String representing the character or Empty if out of bounds</returns>
        public static string GetAt( this ICharStream strm, int index )
        {
            string retVal = string.Empty;
            if(index >= 0 && index < strm.Size)
            {
                retVal = strm.GetText( Interval.Of( index, index ) );
                retVal = Antlr4.Runtime.Misc.Utils.EscapeWhitespace( retVal, escapeSpaces: false );
            }

            return retVal;
        }
    }
}

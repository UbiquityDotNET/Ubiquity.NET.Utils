// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Utility class to provide extension methods for a <see cref="Parser"/></summary>
    public static class ParserExtensions
    {
        /// <summary>Tries to get a Token stream from the parser</summary>
        /// <typeparam name="T">Type of stream to get (Must implement <see cref="ITokenStream"/>)</typeparam>
        /// <param name="self">Parser to get the stream from</param>
        /// <param name="strm">Stream from the parser</param>
        /// <returns><see langword="true"/> if the stream was retrieved</returns>
        public static bool TryGetTokenStream<T>( this Parser self, [MaybeNullWhen(false)] out T strm )
            where T : ITokenStream
        {
            if(self.TokenStream is T t)
            {
                strm = t;
                return true;
            }

            strm = default;
            return false;
        }

        /// <summary>Tries to get a Token stream from the parser</summary>
        /// <typeparam name="T">Type of stream to get (Must implement <see cref="ITokenStream"/>)</typeparam>
        /// <param name="self">Parser to get the stream from</param>
        /// <returns>Stream from the parser</returns>
        /// <exception cref="InvalidOperationException">Could not retrieve a stream of the specified type</exception>
        public static T GetTokenStream<T>( this Parser self )
            where T : ITokenStream
        {
            return TryGetTokenStream<T>( self, out T? retVal )
                ? retVal
                : throw new InvalidOperationException( $"Expected TokenStream of type {typeof( T )} but it is '{self.TokenStream.GetType()}'" );
        }

        /// <summary>Tries to get a Token Source from a <see cref="Parser"/></summary>
        /// <typeparam name="T">Type of the source (Must implement <see cref="ITokenSource"/></typeparam>
        /// <param name="self"><see cref="Parser"/> to get the source from</param>
        /// <param name="tokenSrc">out source if retrieved (<see langword="null"/> if not retrieved)</param>
        /// <returns><see langword="true"/> if the source was retrieved</returns>
        /// <remarks>
        /// This is normally used to get the Lexer from a parser. In such cases <typeparamref name="T"/>
        /// is the ANTLR generated lexer type for the grammar.
        /// </remarks>
        public static bool TryGetTokenSource<T>( this Parser self, [MaybeNullWhen(false)] out T tokenSrc )
            where T : ITokenSource
        {
            if(self.TokenStream.TokenSource is T t)
            {
                tokenSrc = t;
                return true;
            }

            tokenSrc = default;
            return false;
        }

        /// <summary>Tries to get a Token Source from a <see cref="Parser"/></summary>
        /// <typeparam name="T">Type of the source (Must implement <see cref="ITokenSource"/></typeparam>
        /// <param name="self"><see cref="Parser"/> to get the source from</param>
        /// <returns>Token source from the </returns>
        /// <remarks>
        /// This is normally used to get the Lexer from a parser. In such cases <typeparamref name="T"/>
        /// is the ANTLR generated lexer type for the grammar.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Could not retrieve the source of the specified type</exception>
        public static T GetTokenSource<T>( this Parser self )
            where T : ITokenSource
        {
            return TryGetTokenSource<T>( self, out T? retVal )
                   ? retVal
                   : throw new InvalidOperationException( $"Expected TokenSource of type {typeof( T )} but it is '{self.TokenStream.TokenSource.GetType()}'" );
        }

        /// <summary>Gets the combined error listener, if any, for a <see cref="Parser"/></summary>
        /// <param name="p">The parser to get the listener for</param>
        /// <returns><see cref="ICombinedParseErrorListener"/> for the parser or <see langword="null"/> if there is none or there is more than one</returns>
        public static ICombinedParseErrorListener? GetCombinedErrorListener( this Parser p )
        {
            return p.ErrorListeners
                    .OfType<ICombinedParseErrorListener>()
                    .SingleOrDefault();
        }
    }
}

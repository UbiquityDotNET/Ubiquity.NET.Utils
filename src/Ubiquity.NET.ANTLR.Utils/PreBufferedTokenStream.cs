// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>A <see cref="BufferedTokenStream"/> that pre-fetches **all** tokens in the constructor AND is an <see cref="ITokenSource"/></summary>
    /// <remarks>
    /// This class is used to cover scenarios where ALL tokens from lexical scanning are available. For example, in multi stage
    /// editor classification a first pass of classification is based on only the results of scanning the input. Subsequent
    /// passes will add/update classifications based on the results of a parse and yet again for semantic analysis of the
    /// parse. This multi-layered approach allows for getting partial results while rapid edits are in process. (Canceling
    /// a previous parse as it goes). The idea with these streams is to allow using the results of the lexical scan (all
    /// channels) and then apply a filter for the default channel to pass to the parser. Without performing another,
    /// redundant, lexical scan. [All the tokens were already produced - don't throw them away!]
    /// </remarks>
    [SuppressMessage( "Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "It has the correct suffix that matches base" )]
    public class PreBufferedTokenStream
        : BufferedTokenStream
    {
        /// <summary>Initializes a new instance of the <see cref="PreBufferedTokenStream"/> class</summary>
        /// <param name="src">The token source to fetch tokens from</param>
        public PreBufferedTokenStream( ITokenSource src )
            : base( src )
        {
            // fetch in chunks of 100, keep trying until EOF hit.
            while(Fetch( 100 ) > 0)
            {
            }
        }

        /// <summary>Gets a <see cref="ITokenSource"/> for this stream</summary>
        /// <returns>The tokens pre-fetched in this stream as a new <see cref="ITokenSource"/></returns>
        /// <remarks>
        /// This is normally used to pass the lexer results on to a parser after processing the results.
        /// Most common use for this pattern is in scenarios where an input is fully scanned and the application
        /// wants to deal with the tokens before parsing begins. (Typically used for multi-stage editor classification)
        /// </remarks>
        public ITokenSource AsTokenSource( ) => new ListTokenSource( tokens, SourceName );
    }
}

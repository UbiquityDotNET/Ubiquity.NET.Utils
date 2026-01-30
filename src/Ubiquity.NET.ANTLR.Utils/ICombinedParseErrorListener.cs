// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Interface that specifies requirements for a type supporting FULL error listening (Lexer, and parser)</summary>
    public interface ICombinedParseErrorListener
        : IAntlrErrorListener<int> // Lexers, provide discreet symbols as an integral value
        , IAntlrErrorListener<IToken> // Parsers, provide discreet symbols as a full token
    {
        // TODO: Consider additional Semantic Error methods to differentiate between
        //       actual syntactical errors, and post-parse semantic errors.
    }
}

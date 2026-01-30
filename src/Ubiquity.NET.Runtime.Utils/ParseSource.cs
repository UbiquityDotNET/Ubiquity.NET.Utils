// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Analysis level for error or other classifications of a complete parse</summary>
    public enum ParseSource
    {
        /// <summary>Lexical level - used to classify errors and other conditions from only a lexical analysis</summary>
        Lexical,

        /// <summary>Syntactic level - used to classify errors and other conditions from a parse that produces a parse tree</summary>
        Syntactic,

        /// <summary>Syntactic level - used to classify errors and other conditions that result from additional processing of a parse tree</summary>
        Semantic
    }
}

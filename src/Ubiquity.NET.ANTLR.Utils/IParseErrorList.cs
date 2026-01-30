// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.ANTLR.Utils
{
    /// <summary>Interface that specifies requirements for a type collecting errors (Lexer, and parser)</summary>
    public interface IParseErrorList
        : ICombinedParseErrorListener
        , IReadOnlyList<DiagnosticMessage> // Collects and provides the full set of errors as DiagnosticMessages
    {
    }
}

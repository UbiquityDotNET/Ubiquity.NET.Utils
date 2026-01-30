// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Interface for an error code formatter</summary>
    /// <remarks>
    /// Implementations of this interface provide conversion of an
    /// integral error code into a string suitable for use in a
    /// <see cref="DiagnosticMessage"/> or general UX. Normally this
    /// is a simple prefix added to a unified Id value.
    /// </remarks>
    public interface IDiagnosticIdFormatter
    {
        /// <summary>Converts an <see cref="ScopedDiagnosticId"/> into a string form</summary>
        /// <param name="id">Id to convert</param>
        /// <returns>Application specific string form of <paramref name="id"/></returns>
        string FormatCode( ScopedDiagnosticId id );

        /// <summary>Converts a unified ID into a string form</summary>
        /// <param name="id">Id to convert</param>
        /// <returns>Application specific string form of <paramref name="id"/></returns>
        string FormatCode( int id );
    }
}

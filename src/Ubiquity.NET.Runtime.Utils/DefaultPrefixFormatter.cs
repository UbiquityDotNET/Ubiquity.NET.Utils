// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary><see cref="IDiagnosticIdFormatter"/> implementation that formats a scoped ID based on a prefix and <see cref="IDiagnosticIdMap"/></summary>
    /// <remarks>
    /// This performs the simple task of formatting a string code by pre-pending the prefix to the unified code formatted as a string.
    /// Normally, this is how string codes are formed. Applications are free to use any other formatting appropriate to the application
    /// by implementing a custom variant of <see cref="IDiagnosticIdFormatter"/>
    /// </remarks>
    public class DefaultPrefixFormatter
        : IDiagnosticIdFormatter
    {
        /// <summary>Initializes a new instance of the <see cref="DefaultPrefixFormatter"/> class.</summary>
        /// <param name="prefix">Prefix to use for the unified ID</param>
        /// <param name="errorIdMap">Mapping to use for conversion from a scoped ID into a unified ID</param>
        public DefaultPrefixFormatter(string prefix, IDiagnosticIdMap? errorIdMap = null)
        {
            Prefix = prefix;
            IdMap = errorIdMap ?? DiagnosticIdMap.OneToOne;
        }

        /// <summary>Gets the <see cref="IDiagnosticIdMap"/> used to produce a unified integral value for a scoped ID</summary>
        public IDiagnosticIdMap IdMap { get; }

        /// <summary>Gets the prefix to use when formatting the code</summary>
        public string Prefix { get; }

        /// <inheritdoc/>
        /// <remarks>
        /// This will use <see cref="IdMap"/> to form a unified integral value
        /// for the scoped ID and then format that with <see cref="Prefix"/>
        /// into a string for the resulting string ID.
        /// </remarks>
        public string FormatCode( ScopedDiagnosticId id )
        {
            return FormatCode(IdMap.MapId(id));
        }

        /// <inheritdoc/>
        public string FormatCode( int id )
        {
            return $"{Prefix}{id:04}";
        }
    }
}

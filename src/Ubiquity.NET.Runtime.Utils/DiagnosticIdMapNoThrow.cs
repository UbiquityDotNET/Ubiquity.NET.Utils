// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Implementation of <see cref="IDiagnosticIdMap"/> that uses 1:1 mapping for a fallback</summary>
    /// <remarks>
    /// This is the most flexible implementation as it includes fall back to a simple 1:1 mapping. This
    /// allows the mapping provided to use a partial strategy. However, since 1:1 mapping may not be stable
    /// it has some risks. All mapped values must remain stable, once an ID is published it can never change.
    /// It may be deprecated but NEVER re-used. It is the responsibility of the application using this
    /// class to ensure this behavior.
    /// </remarks>
    public class DiagnosticIdMapNoThrow
        : IDiagnosticIdMap
    {
        /// <summary>Initializes a new instance of the <see cref="DiagnosticIdMapNoThrow"/> class.</summary>
        /// <param name="baseMap">Map to use for converting ID values, if none is specified 1:1 mapping is used for all values.</param>
        /// <remarks>
        /// Any ID not provided in <paramref name="baseMap"/> is simply returned as(1:1 mapping). Therefore
        /// the indexer <see cref="MapId(ScopedDiagnosticId)"/> will not throw an exception for keys not found.
        /// </remarks>
        public DiagnosticIdMapNoThrow( IImmutableDictionary<ScopedDiagnosticId, int>? baseMap = null )
        {
            InnerMap = baseMap;
        }

        /// <inheritdoc/>
        public int MapId( ScopedDiagnosticId id )
        {
            // Assume fallback of 1:1 mapping but try and find mapped value if possible.
            int mappedId = id.Code;
            if(InnerMap is not null && InnerMap.Count > 0)
            {
                if(InnerMap.TryGetValue( id, out int mapping ))
                {
                    mappedId = mapping;
                }
            }

            return mappedId;
        }

        private readonly IImmutableDictionary<ScopedDiagnosticId, int>? InnerMap;
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>An adapter from a dictionary to an <see cref="IDiagnosticIdMap"/></summary>
    public class DiagnosticIdMap
        : IDiagnosticIdMap
    {
        /// <summary>Initializes a new instance of the <see cref="DiagnosticIdMap"/> class.</summary>
        /// <param name="mapping">Mapping to use</param>
        public DiagnosticIdMap(IImmutableDictionary<ScopedDiagnosticId, int> mapping)
        {
            ArgumentNullException.ThrowIfNull(mapping);

            InnerMap = mapping;
        }

        /// <inheritdoc/>
        public int MapId( ScopedDiagnosticId id) => InnerMap[id];

        /// <summary>Gets an instance of <see cref="IDiagnosticIdMap"/> that provides 1:1 mapping (That is, no mapping and ignoring scope)</summary>
        /// <remarks>
        /// This is used when the source IDs are already stable and don't change based on scope.
        /// If the source isn't stable across releases then a custom implementation of
        /// <see cref="IDiagnosticIdMap"/> is used. (Typically via an instance of <see cref="DiagnosticIdMap"/>)
        /// </remarks>
        public static IDiagnosticIdMap OneToOne { get; } = new DiagnosticIdMapNoThrow();

        private readonly IImmutableDictionary<ScopedDiagnosticId, int> InnerMap;
    }
}

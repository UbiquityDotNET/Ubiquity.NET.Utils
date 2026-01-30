// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Runtime.Utils
{
    /// <summary>Interface for mapping error IDs</summary>
    /// <remarks>
    /// This allows use of a parse technology specific ID value that may not be stable
    /// across releases to a value that is stable. If no mapping is available then this
    /// uses a 1:1 mapping. (That is, the mapped ID is the same as the one provided).
    /// This is an extensibility point to allow for instability in the source to map
    /// to a stable value as once an ID is published it is immutable. It may be deprecated
    /// but NEVER re-used for any other purpose.
    /// </remarks>
    public interface IDiagnosticIdMap
    {
        /// <summary>Maps a technology specific, potentially unstable ID, to a stable form</summary>
        /// <param name="id">ID value to map</param>
        /// <returns>Mapped ID</returns>
        /// <remarks>
        /// The <see cref="ScopedDiagnosticId.Scope"/> parameter acts as a sort of namespace for the IDs. This
        /// allows mapping to take into account that a different source may produce the same value.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">ID isn't found or mappable</exception>
        int MapId(ScopedDiagnosticId id);
    }
}

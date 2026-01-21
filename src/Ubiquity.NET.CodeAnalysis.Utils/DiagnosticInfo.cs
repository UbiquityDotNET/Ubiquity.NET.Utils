// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

// Modified from idea in blog post: https://andrewlock.net/creating-a-source-generator-part-9-avoiding-performance-pitfalls-in-incremental-generators/

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Contains diagnostic information collected for reporting to the host</summary>
    /// <remarks>
    /// This is an equatable type and therefore is legit for use in generators/analyzers where
    /// that is needed for caching. A <see cref="Diagnostic"/> is not, so this record bundles
    /// the parameters needed for creation of one and defers the construction until needed.
    /// </remarks>
    public sealed class DiagnosticInfo
        : IEquatable<DiagnosticInfo>
    {
#if !NET9_0_OR_GREATER
        /// <summary>Initializes a new instance of the <see cref="DiagnosticInfo"/> class.</summary>
        /// <param name="descriptor">Descriptor for the diagnostic</param>
        /// <param name="location">Location in the source file that triggered this diagnostic</param>
        /// <param name="msgArgs">Args for the message</param>
        public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location, params object[] msgArgs)
            : this(descriptor, location, (IEnumerable<object>)msgArgs)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="DiagnosticInfo"/> class.</summary>
        /// <param name="descriptor">Descriptor for the diagnostic</param>
        /// <param name="location">Location in the source file that triggered this diagnostic</param>
        /// <param name="msgArgs">Args for the message</param>
        public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location, IEnumerable<object> msgArgs)
#else
        /// <summary>Initializes a new instance of the <see cref="DiagnosticInfo"/> class.</summary>
        /// <param name="descriptor">Descriptor for the diagnostic</param>
        /// <param name="location">Location in the source file that triggered this diagnostic</param>
        /// <param name="msgArgs">Args for the message</param>
        public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location, params IEnumerable<string> msgArgs)
#endif
        {
            Descriptor = descriptor;
            Location = location;
            Params = [ .. msgArgs ];
        }

        /// <summary>Gets the parameters for this diagnostic</summary>
        public ImmutableArray<object> Params { get; }

        /// <summary>Gets the descriptor for this diagnostic</summary>
        public DiagnosticDescriptor Descriptor { get; }

        // Microsoft.CodeAnalysis.Location is an abstract type but all derived types implement `IEquatable<T> where T is Location`
        // Thus, a location is equatable even though the base abstract type doesn't implement that interface.

        /// <summary>Gets the location of the source of this diagnostic</summary>
        public Location? Location { get; }

        /// <summary>Factory to create a <see cref="Diagnostic"/> from the information contained in this holder</summary>
        /// <returns><see cref="Diagnostic"/> that represents this information</returns>
        public Diagnostic CreateDiagnostic()
        {
            return Diagnostic.Create(Descriptor, Location, Params.ToArray());
        }

        /// <inheritdoc/>
        public bool Equals( DiagnosticInfo other )
        {
            return other is not null
                && StructuralComparisons.StructuralEqualityComparer.Equals(Params, other.Params)
                && Descriptor.Equals(other.Descriptor)
                && ( ReferenceEquals(Location, other.Location)
                  || (Location is not null && Location.Equals(other.Location))
                   );
        }

        /// <inheritdoc/>
        public override bool Equals( object obj )
        {
            return obj is DiagnosticInfo other
                && Equals( other );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            // sadly this will re-hash the hashcode computed for the structure, but there is no way
            // to combine the result of a hash with other things. (The overload of Add(int) is private)
            // The generic Add<T>() will call the type's GetHashCode() and ignores the implementation of
            // IStructuralEquatable.
            return HashCode.Combine(
                StructuralComparisons.StructuralEqualityComparer.GetHashCode( Params ),
                Descriptor,
                Location
            );
        }
    }
}

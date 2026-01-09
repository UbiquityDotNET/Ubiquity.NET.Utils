// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Keyed collection of <see cref="EquatableAttributeData"/> keyed by <see cref="EquatableAttributeData.Name"/></summary>
    public class EquatableAttributeDataCollection
        : IEnumerable<EquatableAttributeData>
        , IReadOnlyCollection<EquatableAttributeData>
        , IEquatable<EquatableAttributeDataCollection>
        , IStructuralEquatable
    {
        /// <summary>Initializes a new instance of the <see cref="EquatableAttributeDataCollection"/> class.</summary>
        /// <param name="attributes">Attribute information for this collection</param>
        /// <remarks>
        /// This will iterate over the collection <paramref name="attributes"/> to get the <see cref="EquatableAttributeData.Name"/>
        /// as a key for the item in the collection.
        /// </remarks>
        public EquatableAttributeDataCollection( IEnumerable<EquatableAttributeData> attributes )
            : this( ToDictionary( attributes ) )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="EquatableAttributeDataCollection"/> class.</summary>
        /// <param name="attributes">Dictionary of attributes for the collection</param>
        public EquatableAttributeDataCollection( EquatableDictionary<NamespaceQualifiedName, EquatableAttributeData> attributes )
        {
            InnerDictionary = attributes;
        }

        /// <summary>Gets the value for a key</summary>
        /// <param name="key">Name of the attribute data</param>
        /// <returns>Attribute data for the key</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null</exception>
        /// <exception cref="KeyNotFoundException"><paramref name="key"/> does not match any entry in this collection</exception>
        [SuppressMessage( "Design", "CA1043:Use Integral Or String Argument For Indexers", Justification = "Neither string nor integer is the key type" )]
        public EquatableAttributeData this[ NamespaceQualifiedName key ] => InnerDictionary[key];

        /// <inheritdoc/>
        public int Count => InnerDictionary.Count;

        /// <inheritdoc/>
        public IEnumerator<EquatableAttributeData> GetEnumerator( )
        {
            return Values.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator( )
        {
            return GetEnumerator();
        }

        #region Equatable implementation

        /// <inheritdoc/>
        public bool Equals( EquatableAttributeDataCollection other )
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals( object obj )
        {
            return obj is not null
                && obj is EquatableAttributeDataCollection collection
                && Equals( collection );
        }

        /// <inheritdoc />
        public override int GetHashCode( )
        {
            // CONSIDER: Optimize this to cache the hash code.
            HashCode retVal = default;
            foreach(var item in this)
            {
                retVal.Add( item );
            }

            return retVal.ToHashCode();
        }
        #endregion

        /// <summary>Gets a sequence of the names of all values.</summary>
        /// <remarks>
        /// These names are keys for the values used in <see cref="this[]"/> and
        /// <see cref="TryGetValue(NamespaceQualifiedName, out EquatableAttributeData)"/>.
        /// </remarks>
        public IEnumerable<NamespaceQualifiedName> Keys => InnerDictionary.Keys;

        /// <summary>Gets a sequence of all the values in this collection</summary>
        /// <remarks>
        /// This is no different that using the <see cref="IEnumerable{T}"/> implemented by this instance
        /// directly. It is here to support common dictionary functionality.
        /// </remarks>
        public IEnumerable<EquatableAttributeData> Values => InnerDictionary.Values;

        /// <summary>Tries to get the value for a given <see cref="NamespaceQualifiedName"/></summary>
        /// <param name="key">The type name for the attribute</param>
        /// <param name="item">Resulting attribute if found (default constructed if not)</param>
        /// <returns>true if the attribute is found in this collection</returns>
        public bool TryGetValue( NamespaceQualifiedName key, [MaybeNullWhen( false )] out EquatableAttributeData item )
        {
            return InnerDictionary.TryGetValue( key, out item );
        }

        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Simple wrapper over public constructor" )]
        public static implicit operator EquatableAttributeDataCollection( ImmutableArray<EquatableAttributeData> attributes )
        {
            return new EquatableAttributeDataCollection( attributes );
        }

        private readonly EquatableDictionary<NamespaceQualifiedName, EquatableAttributeData> InnerDictionary;

        private static EquatableDictionary<NamespaceQualifiedName, EquatableAttributeData> ToDictionary( IEnumerable<EquatableAttributeData> attributes )
        {
            var bldr = ImmutableDictionary.CreateBuilder<NamespaceQualifiedName, EquatableAttributeData>();
            if(attributes is not null)
            {
                foreach(var attr in attributes)
                {
                    bldr.Add( attr.Name, attr );
                }
            }

            return bldr.ToImmutable();
        }

        /// <inheritdoc/>
        bool IStructuralEquatable.Equals( object other, IEqualityComparer comparer )
        {
            bool retVal = other is EquatableAttributeDataCollection otherCollection
                       && ((IStructuralEquatable)InnerDictionary).Equals( otherCollection.InnerDictionary, comparer );

            return retVal;
        }

        /// <inheritdoc/>
        int IStructuralEquatable.GetHashCode( IEqualityComparer comparer )
        {
            return ((IStructuralEquatable)InnerDictionary).GetHashCode( comparer );
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>An equatable, immutable, dictionary</summary>
    /// <typeparam name="TKey">Type of keys in the dictionary (Must be <see cref="IEquatable{T}"/> for TKey</typeparam>
    /// <typeparam name="TValue">Type of keys in the dictionary (Must be <see cref="IEquatable{T}"/> for TValue</typeparam>
    /// <remarks>
    /// This is a struct that has no additional size beyond that of the wrapped reference to the underlying dictionary. It
    /// merely adds equatability checks to the dictionary for use in Roslyn source generator caching.
    /// </remarks>
    public readonly struct EquatableDictionary<TKey, TValue>
        : IImmutableDictionary<TKey, TValue>
        , IEquatable<EquatableDictionary<TKey, TValue>>
        , IStructuralEquatable
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        /// <summary>Initializes a new instance of the <see cref="EquatableDictionary{TKey, TValue}"/> struct.</summary>
        /// <param name="dictionaryToWrap">Dictionary to wrap</param>
        public EquatableDictionary( IImmutableDictionary<TKey, TValue> dictionaryToWrap )
        {
            InnerDictionary = dictionaryToWrap;
        }

        #region Equatability

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return InnerDictionary.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals( object obj )
        {
            return obj is EquatableDictionary<TKey, TValue> dictionary
                && Equals( dictionary );
        }

        /// <inheritdoc/>
        public bool Equals( EquatableDictionary<TKey, TValue> other )
        {
            return ((IStructuralEquatable)this).Equals( other, EqualityComparer<KeyValuePair<TKey, TValue>>.Default);
        }

        /// <summary>Test two instances for equality</summary>
        /// <param name="left">Left side of the comparison</param>
        /// <param name="right">Right side of the comparison</param>
        /// <returns>True if the values are equal and false if not</returns>
        public static bool operator ==( EquatableDictionary<TKey, TValue> left, EquatableDictionary<TKey, TValue> right )
        {
            return left.Equals( right );
        }

        /// <summary>Test two instances for inequality</summary>
        /// <param name="left">Left side of the comparison</param>
        /// <param name="right">Right side of the comparison</param>
        /// <returns>False if the values are not equal and true if not</returns>
        public static bool operator !=( EquatableDictionary<TKey, TValue> left, EquatableDictionary<TKey, TValue> right )
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        bool IStructuralEquatable.Equals( object other, IEqualityComparer comparer )
        {
            if(other is not EquatableDictionary<TKey, TValue> otherItem)
            {
                return false;
            }

            using var thisIterator = GetEnumerator();
            using var otherIterator = otherItem.GetEnumerator();

            bool mismatchFound = false;
            bool lhsValid = thisIterator.MoveNext();  // move to first entry; if any
            bool rhsValid = otherIterator.MoveNext();
            while(lhsValid && rhsValid)
            {
                // NOTE: KeyValuePair<T> is NOT equatable, it's just a struct
                // if one or both of the pair is a managed reference then it
                // is compared with what amount to reference equality. Which is
                // NOT what is desired here.
                if( !comparer.Equals( thisIterator.Current.Key, otherIterator.Current.Key )
                 || !comparer.Equals( thisIterator.Current.Value, otherIterator.Current.Value )
                )
                {
                    mismatchFound = true;
                    break; // stop loop as soon as mismatch is found
                }

                lhsValid = thisIterator.MoveNext();
                rhsValid = otherIterator.MoveNext();
            }

            // Only equal if no mismatch found AND both sequences are the same length
            // (that is, if one sequence is a super set of the other, it is not equal!
            return !mismatchFound && (lhsValid == rhsValid);
        }

        /// <inheritdoc/>
        int IStructuralEquatable.GetHashCode( IEqualityComparer comparer )
        {
            HashCode hashCode = default;
            foreach( var kvp in this)
            {
                hashCode.Add(kvp, comparer as IEqualityComparer<KeyValuePair<TKey, TValue>> );
            }

            return hashCode.ToHashCode();
        }
        #endregion

        #region IImmutableDictionary<TKey, TValue> interface implementations through wrapped dictionary

        /// <inheritdoc/>
        public TValue this[ TKey key ] => ((IReadOnlyDictionary<TKey, TValue>)InnerDictionary)[ key ];

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys => InnerDictionary.Keys;

        /// <inheritdoc/>
        public IEnumerable<TValue> Values => InnerDictionary.Values;

        /// <inheritdoc/>
        public int Count => InnerDictionary.Count;

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> Add( TKey key, TValue value )
        {
            return new EquatableDictionary<TKey, TValue>(InnerDictionary.Add( key, value ));
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> AddRange( IEnumerable<KeyValuePair<TKey, TValue>> pairs )
        {
            return new EquatableDictionary<TKey, TValue>( InnerDictionary.AddRange( pairs ) );
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> Clear( )
        {
            return new EquatableDictionary<TKey, TValue>( InnerDictionary.Clear() );
        }

        /// <inheritdoc/>
        public bool Contains( KeyValuePair<TKey, TValue> pair )
        {
            return InnerDictionary.Contains( pair );
        }

        /// <inheritdoc/>
        public bool ContainsKey( TKey key )
        {
            return InnerDictionary.ContainsKey( key );
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator( )
        {
            return InnerDictionary.GetEnumerator();
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> Remove( TKey key )
        {
            return InnerDictionary.Remove( key );
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> RemoveRange( IEnumerable<TKey> keys )
        {
            return InnerDictionary.RemoveRange( keys );
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> SetItem( TKey key, TValue value )
        {
            return InnerDictionary.SetItem( key, value );
        }

        /// <inheritdoc/>
        public IImmutableDictionary<TKey, TValue> SetItems( IEnumerable<KeyValuePair<TKey, TValue>> items )
        {
            return InnerDictionary.SetItems( items );
        }

        /// <inheritdoc/>
        public bool TryGetKey( TKey equalKey, out TKey actualKey )
        {
            return InnerDictionary.TryGetKey( equalKey, out actualKey );
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator( )
        {
            return ((IEnumerable)InnerDictionary).GetEnumerator();
        }

        /// <inheritdoc/>
        public bool TryGetValue( TKey key, out TValue value )
        {
            value = default!;
            if(!InnerDictionary.TryGetValue( key, out TValue? foundValue ))
            {
                return false;
            }

            value = foundValue;
            return true;
        }

        #endregion

        /// <summary>Implicit cast from <see cref="ImmutableDictionary{TKey, TValue}"/></summary>
        /// <param name="dictionaryToWrap">Dictionary to wrap</param>
        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Implicit cast for public constructor" )]
        public static implicit operator EquatableDictionary<TKey, TValue>( ImmutableDictionary<TKey, TValue> dictionaryToWrap )
        {
            return new(dictionaryToWrap);
        }

        /// <summary>Implicit cast from <see cref="ImmutableSortedDictionary{TKey, TValue}"/></summary>
        /// <param name="dictionaryToWrap">Dictionary to wrap</param>
        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Implicit cast for public constructor" )]
        public static implicit operator EquatableDictionary<TKey, TValue>( ImmutableSortedDictionary<TKey, TValue> dictionaryToWrap )
        {
            return new( dictionaryToWrap );
        }

        private readonly IImmutableDictionary<TKey, TValue> InnerDictionary;
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

#pragma warning disable SA1642 // Constructor summary documentation should begin with standard text
#pragma warning disable SA1615 // Element return value should be documented
#pragma warning disable SA1604 // Element documentation should have summary
#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable CA1000 // Do not declare static members on generic types
#pragma warning disable CA2225 // Operator overloads have named alternates

// ORIGINALLY FROM: https://github.com/CommunityToolkit/dotnet/blob/7b53ae23dfc6a7fb12d0fc058b89b6e948f48448/src/CommunityToolkit.Mvvm.SourceGenerators/Helpers/EquatableArray%7BT%7D.cs

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Modified heavily to support IStructuralEquatable

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Extensions for <see cref="EquatableArray{T}"/>.</summary>
    public static class EquatableArray
    {
        /// <summary>Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</summary>
        /// <typeparam name="T">The type of items in the input array.</typeparam>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static EquatableArray<T> AsEquatableArray<T>( this ImmutableArray<T> array )
            where T : IEquatable<T>
        {
            return array.IsDefault ? throw new ArgumentNullException( nameof( array ) )
                 : new( array );
        }

        /// <summary>Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}.Builder"/>.</summary>
        /// <typeparam name="T">The type of items in the input array.</typeparam>
        /// <param name="self">The input builder instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        [Obsolete( "Use ImmutableArry<T> instead" )]
        public static EquatableArray<T> AsEquatableArray<T>( this ImmutableArray<T>.Builder self )
            where T : IEquatable<T>
        {
            return AsEquatableArray<T>( self.ToImmutable() );
        }
    }

    /// <summary>
    /// An immutable, equatable array. This is equivalent to <see cref="ImmutableArray{T}"/> but with value equality of members support.
    /// </summary>
    /// <typeparam name="T">The type of values in the array.</typeparam>
    /// <remarks>
    /// Use of this type should be limited to cases where it is the result of analysis itself. That is, when the array is a
    /// member of some other equatable type, then it should use <see cref="ImmutableArray{T}"/> instead. The container should
    /// use the <see cref="IStructuralEquatable"/> support to implement it's <see cref="IEquatable{T}"/>. This type will enforce
    /// structural comparison as it's implementation of equality checks. All forms of retrieving a hash code resolve to the
    /// structural form. This ensures that the behavior is consistent and the array is cacheable.
    /// </remarks>
    public readonly struct EquatableArray<T>
        : IEquatable<EquatableArray<T>>
        , IEnumerable<T>
        , IStructuralEquatable
        where T : IEquatable<T>
    {
        /// <summary>
        /// The underlying <typeparamref name="T"/> array.
        /// </summary>
        private readonly T[]? InnerArray;

        /// <summary>
        /// Creates a new <see cref="EquatableArray{T}"/> instance.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> to wrap.</param>
        public EquatableArray( ImmutableArray<T> array )
        {
            InnerArray = Unsafe.As<ImmutableArray<T>, T[]?>( ref array );
        }

        /// <summary>
        /// Gets a reference to an item at a specified position within the array.
        /// </summary>
        /// <param name="index">The index of the item to retrieve a reference to.</param>
        /// <returns>A reference to an item at a specified position within the array.</returns>
        public ref readonly T this[ int index ]
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => ref AsImmutableArray().ItemRef( index );
        }

        /// <summary>
        /// Gets a value indicating whether the current array is empty.
        /// </summary>
        public bool IsEmpty
        {
            [MethodImpl( MethodImplOptions.AggressiveInlining )]
            get => AsImmutableArray().IsEmpty;
        }

        /// <summary>Gets the length of the array</summary>
        public int Length => InnerArray?.Length ?? 0;

        /// <inheritdoc/>
        public bool Equals( EquatableArray<T> array )
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals( InnerArray );
        }

        /// <inheritdoc/>
        public override bool Equals( [NotNullWhen( true )] object? obj )
        {
            return obj is EquatableArray<T> other
                && Equals( other );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode( InnerArray );
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}"/> instance from the current <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>The <see cref="ImmutableArray{T}"/> from the current <see cref="EquatableArray{T}"/>.</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ImmutableArray<T> AsImmutableArray( )
        {
            return Unsafe.As<T[]?, ImmutableArray<T>>( ref Unsafe.AsRef( in InnerArray ) );
        }

        /// <summary>
        /// Creates an <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <param name="array">The input <see cref="ImmutableArray{T}"/> instance.</param>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static EquatableArray<T> FromImmutableArray( ImmutableArray<T> array )
        {
            return new( array );
        }

        /// <summary>
        /// Returns a <see cref="ReadOnlySpan{T}"/> wrapping the current items.
        /// </summary>
        /// <returns>A <see cref="ReadOnlySpan{T}"/> wrapping the current items.</returns>
        public ReadOnlySpan<T> AsSpan( )
        {
            return AsImmutableArray().AsSpan();
        }

        /// <summary>
        /// Copies the contents of this <see cref="EquatableArray{T}"/> instance to a mutable array.
        /// </summary>
        /// <returns>The newly instantiated array.</returns>
        public T[] ToArray( )
        {
            return [ .. AsImmutableArray() ];
        }

        /// <summary>
        /// Gets an <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}.Enumerator"/> value to traverse items in the current array.</returns>
        public ImmutableArray<T>.Enumerator GetEnumerator( )
        {
            return AsImmutableArray().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator<T> IEnumerable<T>.GetEnumerator( )
        {
            return ((IEnumerable<T>)AsImmutableArray()).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator( )
        {
            return ((IEnumerable)AsImmutableArray()).GetEnumerator();
        }

        /// <inheritdoc/>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Nested conditionals are not simpler" )]
        bool IStructuralEquatable.Equals( object other, IEqualityComparer comparer )
        {
            if(other is not EquatableArray<T> otherArray)
            {
                return false;
            }

            return InnerArray is null
                ? ReferenceEquals(InnerArray, otherArray.InnerArray)
                : ((IStructuralEquatable)InnerArray).Equals(other, comparer);
        }

        /// <inheritdoc/>
        int IStructuralEquatable.GetHashCode( IEqualityComparer comparer )
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(InnerArray);
        }

        /// <summary>
        /// Implicitly converts an <see cref="ImmutableArray{T}"/> to <see cref="EquatableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="EquatableArray{T}"/> instance from a given <see cref="ImmutableArray{T}"/>.</returns>
        public static implicit operator EquatableArray<T>( ImmutableArray<T> array )
        {
            return FromImmutableArray( array );
        }

        /// <summary>
        /// Implicitly converts an <see cref="EquatableArray{T}"/> to <see cref="ImmutableArray{T}"/>.
        /// </summary>
        /// <returns>An <see cref="ImmutableArray{T}"/> instance from a given <see cref="EquatableArray{T}"/>.</returns>
        public static implicit operator ImmutableArray<T>( EquatableArray<T> array )
        {
            return array.AsImmutableArray();
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are equal.</returns>
        public static bool operator ==( EquatableArray<T> left, EquatableArray<T> right )
        {
            return left.Equals( right );
        }

        /// <summary>
        /// Checks whether two <see cref="EquatableArray{T}"/> values are not the same.
        /// </summary>
        /// <param name="left">The first <see cref="EquatableArray{T}"/> value.</param>
        /// <param name="right">The second <see cref="EquatableArray{T}"/> value.</param>
        /// <returns>Whether <paramref name="left"/> and <paramref name="right"/> are not equal.</returns>
        public static bool operator !=( EquatableArray<T> left, EquatableArray<T> right )
        {
            return !left.Equals( right );
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Ubiquity.NET.SourceGenerator.Test.Utils
{
    /// <summary>Comparer to test an array (element by element) for equality</summary>
    /// <typeparam name="T">Type of the elements of a sequence</typeparam>
    public class SequenceComparer<T>
        : IEqualityComparer<IEnumerable<T>>
    {
        /// <summary>Initializes a new instance of the <see cref="SequenceComparer{T}"/> class.</summary>
        /// <param name="comparer">Comparer to use in comparing individual items; Default is <see cref="EqualityComparer{T}.Default"/></param>
        public SequenceComparer( IEqualityComparer<T>? comparer = null )
        {
            ItemComparer = comparer ?? EqualityComparer<T>.Default;
        }

        /// <inheritdoc/>
        [SuppressMessage( "StyleCop.CSharp.NamingRules", "SA1305:Field names should not use Hungarian notation", Justification = "xValues and yValues are not Hungarian names" )]
        public bool Equals( IEnumerable<T>? x, IEnumerable<T>? y )
        {
            ArgumentNullException.ThrowIfNull( x );
            ArgumentNullException.ThrowIfNull( y );

            var xValues = x.ToImmutableArray();
            var yValues = x.ToImmutableArray();
            return xValues.Length == yValues.Length
                && xValues.Zip( yValues, ( a, b ) => ItemComparer.Equals( a, b ) ).All( x => x );
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Signature of interface requires non-null values for <paramref name="obj"/> so
        /// compilation should complain if nullability analysis is applied. However, this
        /// implementation is tolerant of null following common practice and turns that
        /// into a 0 hash code.
        /// </remarks>
        public int GetHashCode( [DisallowNull] IEnumerable<T> obj )
        {
            return obj is null ? 0 : obj.GetHashCode();
        }

        /// <summary>Gets the item comparer for comparing individual items</summary>
        /// <remarks>The default comparer is <see cref="EqualityComparer{T}.Default"/></remarks>
        public IEqualityComparer<T> ItemComparer { get; init; } = EqualityComparer<T>.Default;

        /// <summary>Default constructed comparer.</summary>
        public static readonly SequenceComparer<T> Default = new(EqualityComparer<T>.Default);
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Performs structural equality on a <see cref="TypedConstant"/></summary>
    /// <remarks>
    /// While <see cref="TypedConstant"/> implements <see cref="IEquatable{T}"/> it fails to
    /// account for structural equatability of the value, that is, it is ONLY a shallow equality
    /// check and that isn't valid for a source Roslyn component that is collecting data that
    /// requires caching.
    /// </remarks>
    public class StructuralTypedConstantComparer
        : IEqualityComparer<TypedConstant>
        , IEqualityComparer
    {
        /// <summary>Performs structural equality of two <see cref="TypedConstant"/> values</summary>
        /// <param name="x">First comparand</param>
        /// <param name="y">Second comparand</param>
        /// <returns>true if <paramref name="x"/> and <paramref name="y"/> are structurally equal</returns>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Nested conditional is not simpler" )]
        public bool Equals( TypedConstant x, TypedConstant y )
        {
            // handle fast checks first
            if( (x.IsNull != y.IsNull) || (x.Kind != y.Kind) )
            {
                return false;
            }

            if(!SymbolEqualityComparer.Default.Equals( x.Type, y.Type ))
            {
                return false;
            }

            // Due to how the properties are not simple accessors for the fields
            // this has to use distinct computations of equality for arrays vs. scalars
            bool retVal = x.Kind == TypedConstantKind.Array
                ? StructuralComparisons.StructuralEqualityComparer.Equals( x.Values, y.Values )
                : StructuralComparisons.StructuralEqualityComparer.Equals( x.Value, y.Value );

            return retVal;
        }

        /// <inheritdoc/>
        public int GetHashCode( TypedConstant obj )
        {
            return obj.Kind == TypedConstantKind.Array
                ? HashCode.Combine(
                    obj.Kind,
                    SymbolEqualityComparer.IncludeNullability.GetHashCode( obj.Type ),
                    StructuralComparisons.StructuralEqualityComparer.GetHashCode( obj.Values )
                    )
                : HashCode.Combine(
                    obj.Kind,
                    SymbolEqualityComparer.IncludeNullability.GetHashCode( obj.Type ),
                    StructuralComparisons.StructuralEqualityComparer.GetHashCode( obj.Value )
                    );
        }

        /// <summary>Determines if <paramref name="x"/> and <paramref name="y"/> are both <see cref="TypedConstant"/> and structurally equal</summary>
        /// <param name="x">First comparand</param>
        /// <param name="y">Second comparand</param>
        /// <returns>true if if <paramref name="x"/> and <paramref name="y"/> are both <see cref="TypedConstant"/> and structurally equal; false if not</returns>
        public new bool Equals( object x, object y )
        {
            return x is TypedConstant lhs
                && y is TypedConstant rhs
                && Equals( lhs, rhs );
        }

        /// <inheritdoc/>
        public int GetHashCode( object obj )
        {
            return obj is TypedConstant value
                 ? GetHashCode( value )
                 : obj?.GetHashCode() ?? 0;
        }

        /// <summary>Gets the default instance of this comparer</summary>
        public static StructuralTypedConstantComparer Default { get; } = new();
    }
}

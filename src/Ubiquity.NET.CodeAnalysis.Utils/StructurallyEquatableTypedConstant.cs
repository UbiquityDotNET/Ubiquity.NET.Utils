// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Wrapper for <see cref="TypedConstant"/> that implements structural (deep) equality</summary>
    /// <remarks>
    /// While <see cref="TypedConstant"/> implements <see cref="IEquatable{T}"/> it does so with a shallow
    /// depth if <see cref="TypedConstant.Kind"/> indicates an array. Use of this, type expresses intent
    /// to use structural equality (deep) semantics. It is intentionally designed where the size is the
    /// same as it is just functionality that is different.
    /// </remarks>
    public readonly struct StructurallyEquatableTypedConstant
        : IEquatable<StructurallyEquatableTypedConstant>
    {
        /// <summary>Initializes a new instance of the <see cref="StructurallyEquatableTypedConstant"/> struct.</summary>
        /// <param name="other"><see cref="TypedConstant"/> to wrap</param>
        public StructurallyEquatableTypedConstant(TypedConstant other)
        {
            InnerConst = other;
        }

        /// <inheritdoc cref="TypedConstant.Kind"/>
        public TypedConstantKind Kind => InnerConst.Kind;

        /// <inheritdoc cref="TypedConstant.Type"/>
        public ITypeSymbol? Type => InnerConst.Type;

        /// <inheritdoc cref="TypedConstant.IsNull"/>
        public bool IsNull => InnerConst.IsNull;

        /// <inheritdoc cref="TypedConstant.Value"/>
        public object? Value => InnerConst.Value;

        /// <summary>Gets the value for a <see cref="StructurallyEquatableTypedConstant"/> array.</summary>
        /// <value>
        /// <see langword="default" /> <c>ImmutableArray</c> if <see langword="null"/> was passed as the array value;
        /// <see cref="IsNull"/> can be used to check for this.
        /// </value>
        public ImmutableArray<StructurallyEquatableTypedConstant> Values => [ .. InnerConst.Values.Select(e=> new StructurallyEquatableTypedConstant(e)) ];

        /// <summary>Test if this instance is structurally equal to <paramref name="other"/></summary>
        /// <param name="other">Comparand</param>
        /// <returns>true if instance is structurally equal to <paramref name="other"/>; false if not</returns>
        public bool Equals( StructurallyEquatableTypedConstant other )
        {
            return StructuralTypedConstantComparer.Default.Equals(InnerConst, other.InnerConst);
        }

        /// <summary>Test if this instance is structurally equal to <paramref name="obj"/></summary>
        /// <param name="obj">Comparand</param>
        /// <returns>
        /// true if obj is <see cref="StructurallyEquatableTypedConstant"/> and this instance is structurally equal to <paramref name="obj"/>; false if not.
        /// </returns>
        public override bool Equals( object obj )
        {
            return obj is StructurallyEquatableTypedConstant other
                && Equals( other );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return StructuralTypedConstantComparer.Default.GetHashCode(InnerConst);
        }

        public static bool operator ==( StructurallyEquatableTypedConstant left, StructurallyEquatableTypedConstant right )
        {
            return left.Equals(right);
        }

        public static bool operator !=( StructurallyEquatableTypedConstant left, StructurallyEquatableTypedConstant right )
        {
            return !(left == right);
        }

        /// <summary>Gets the wrapped <see cref="TypedConstant"/></summary>
        /// <returns>The inner <see cref="TypedConstant"/></returns>
        /// <remarks>
        /// This is useful for getting access to extension methods for <see cref="TypedConstant"/>
        /// that are otherwise unrelated to equality.
        /// </remarks>
        public TypedConstant ToTypedConstant()
        {
            return InnerConst;
        }

        private readonly TypedConstant InnerConst;

        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Simple alternate for existing constructor" )]
        public static implicit operator StructurallyEquatableTypedConstant( TypedConstant other )
        {
            return new( other );
        }

        public static explicit operator TypedConstant( StructurallyEquatableTypedConstant other )
        {
            return other.ToTypedConstant();
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Captured type name, includes nullability</summary>
    /// <remarks>
    /// <para>Due to how nullability was introduced much later than the original language and runtime,
    /// the concept of nullability is not baked into the type. Reference types are, technically,
    /// always nullable, in general. However, sometimes they may be annotated to allow the compiler
    /// to perform static analysis to verify the rule. Additionally, any value that might be null is
    /// flagged as an error in the compiler (and IDE). Nullable value types have been around since day
    /// one and are all just instances of <see cref="Nullable{T}"/>. Thus the mechanics of nullable
    /// annotation are not consistent (Though a language compiler may make the syntax seem like it is).
    /// So, this will capture the information for both cases.</para>
    /// </remarks>
    public class NamespaceQualifiedTypeName
        : NamespaceQualifiedName
        , IEquatable<NamespaceQualifiedTypeName>
        , IFormattable
    {
        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedTypeName"/> class.</summary>
        public NamespaceQualifiedTypeName( )
            : base()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedTypeName"/> class.</summary>
        /// <param name="namespaceNames">Namespace names sequence for this type</param>
        /// <param name="simpleName">Simple name for this type</param>
        /// <param name="nullableAnnotation">Nullability annotation for this type</param>
        public NamespaceQualifiedTypeName(
            IEnumerable<string> namespaceNames,
            string simpleName,
            NullableAnnotation nullableAnnotation = NullableAnnotation.None
            )
            : base( namespaceNames, simpleName )
        {
            NullableAnnotation = nullableAnnotation;
        }

        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedTypeName"/> class.</summary>
        /// <param name="sym">Symbol to capture the type information for</param>
        /// <remarks>
        /// If <paramref name="sym"/> is a nullable value type (i.e., <see cref="Nullable{T}"/>> then this
        /// will capture the name of <c>T</c> and the <see cref="ITypeSymbol.NullableAnnotation"/>. Otherwise,
        /// it captures the name of <paramref name="sym"/> and the <see cref="ITypeSymbol.NullableAnnotation"/>.
        /// </remarks>
        public NamespaceQualifiedTypeName( ITypeSymbol sym )
            : this( GetNullableNamespaceNames( sym ), GetNullableSimpleName( sym ), sym.NullableAnnotation )
        {
        }

        /// <summary>Gets a value indicating whether this type has nullability annotation (and a generator should use a language specific nullability form)</summary>
        public bool IsNullable => NullableAnnotation == NullableAnnotation.Annotated;

        /// <summary>Gets the nullability annotation state for this type</summary>
        public NullableAnnotation NullableAnnotation { get; init; }

        /// <summary>Formats this instance according to the args</summary>
        /// <param name="format">Format string for this instance (see remarks)</param>
        /// <param name="formatProvider">[ignored]</param>
        /// <returns>Formatted string representation of this instance</returns>
        /// <remarks>
        /// The supported values for <paramref name="format"/> are:
        /// <list type="table">
        /// <listheader><term>Value</term><description>Description</description></listheader>
        /// <item><term>A</term><description>Format as a language specific alias if possible</description></item>
        /// <item><term>G</term><description>Format with a language specific global prefix.</description></item>
        /// <item><term>AG</term><description>Format with a language specific alias if possible, otherwise include a global prefix.</description></item>
        /// <item><term>R</term><description>The raw full name without any qualifications</description></item>
        /// </list>
        /// </remarks>
        /// <exception cref="NotSupportedException"><paramref name="format"/> is not supported</exception>
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Nested conditionals is never simpler" )]
        public override string ToString( string format, IFormatProvider? formatProvider )
        {
            // default to the C# formatter unless specified.
            formatProvider ??= NamespaceQualifiedNameFormatter.CSharp;

            // if no custom formatter is available, then just produce the raw name.
            var customFormatter = (ICustomFormatter<NamespaceQualifiedTypeName>)formatProvider.GetFormat(typeof(ICustomFormatter<NamespaceQualifiedTypeName>));
            if(customFormatter is null && NullableAnnotation == NullableAnnotation.Annotated)
            {
                return $"{string.Join( ".", FullNameParts )}?";
            }

            return customFormatter is null
                 ? string.Join( ".", FullNameParts )
                 : customFormatter.Format( format, this, formatProvider );
        }

        #region Equality

        /// <inheritdoc/>
        public bool Equals( NamespaceQualifiedTypeName other )
        {
            return IsNullable == other.IsNullable
                && base.Equals( other );
        }

        /// <inheritdoc/>
        public override bool Equals( object obj )
        {
            return obj is NamespaceQualifiedTypeName other
                && Equals( other );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return HashCode.Combine( base.GetHashCode(), NullableAnnotation );
        }

        /// <summary>Equality operator to test two names for equality</summary>
        /// <param name="left">left name to test</param>
        /// <param name="right">Right name to test</param>
        /// <returns>true if the names are equal</returns>
        public static bool operator ==( NamespaceQualifiedTypeName left, NamespaceQualifiedTypeName right )
        {
            return left.Equals( right );
        }

        /// <summary>Equality operator to test two names for inequality</summary>
        /// <param name="left">left name to test</param>
        /// <param name="right">Right name to test</param>
        /// <returns>true if the names are <em><b>NOT</b></em> equal; false otherwise</returns>
        public static bool operator !=( NamespaceQualifiedTypeName left, NamespaceQualifiedTypeName right )
        {
            return !(left == right);
        }
        #endregion

        // IFF sym is a Nullable<T> (Nullable value type) this will get the simple name of T
        private static string GetNullableSimpleName( ITypeSymbol sym )
        {
            return sym.IsNullableValueType() && sym is INamedTypeSymbol ns
                ? ns.TypeArguments[ 0 ].Name
                : sym.Name;
        }

        private static IEnumerable<string> GetNullableNamespaceNames( ITypeSymbol sym )
        {
            return sym.IsNullableValueType() && sym is INamedTypeSymbol ns
                 ? ns.TypeArguments[ 0 ].GetNamespaceNames()
                 : sym.GetNamespaceNames();
        }
    }
}

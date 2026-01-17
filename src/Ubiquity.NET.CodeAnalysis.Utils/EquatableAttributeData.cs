// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Diagnostics;

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Equatable form of <see cref="AttributeData"/></summary>
    /// <remarks>
    /// <note type="important">
    /// This CAPTURES only the portion of the attributes data that is relevant to source generators.
    /// Specifically, it captures the full <see cref="NamespaceQualifiedName"/> for the unnamed
    /// constructor arguments and a dictionary for the named arguments. All of which is captured
    /// in a manner that supports equality checks. These form the semantic set of properties
    /// necessary to capture for an attribute.
    /// </note>
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/attributes#2324-attribute-parameter-types">
    /// C# language specification §23.2.4 Attribute parameter types
    /// </seealso>
    public class EquatableAttributeData
        : IEquatable<EquatableAttributeData>
    {
        /// <summary>Initializes a new instance of the <see cref="EquatableAttributeData"/> class.</summary>
        /// <param name="data">The <see cref="AttributeData"/> to capture equatable information from</param>
        public EquatableAttributeData( AttributeData data )
        {
            PolyFillExceptionValidators.ThrowIfNull( data );

            Name = data.GetNamespaceQualifiedName();
            ConstructorArguments = [ .. data.ConstructorArguments.Select(e=>(StructurallyEquatableTypedConstant)e) ];
            var namedArgs = data.NamedArguments.Select(kvp => new KeyValuePair<string, StructurallyEquatableTypedConstant>(kvp.Key, (StructurallyEquatableTypedConstant)kvp.Value));
            NamedArguments = namedArgs.ToImmutableDictionary();
        }

        /// <summary>Gets the full namespace qualified name for the type of this attribute</summary>
        public NamespaceQualifiedTypeName Name { get; }

        /// <summary>Gets the unnamed constructor arguments for this attribute</summary>
        public ImmutableArray<StructurallyEquatableTypedConstant> ConstructorArguments { get; }

        /// <summary>Gets dictionary for the named arguments</summary>
        public EquatableDictionary<string, StructurallyEquatableTypedConstant> NamedArguments { get; } = [];

        /// <summary>Gets the constant for a named argument</summary>
        /// <param name="argName">Name of the argument to fetch</param>
        /// <returns>Optional value for the named argument (<see cref="Optional.HasValue"/> is false if <paramref name="argName"/> isn't provided)</returns>
        public Optional<StructurallyEquatableTypedConstant> GetNamedArgValue( string argName )
        {
            return NamedArguments.TryGetValue( argName, out StructurallyEquatableTypedConstant typedConst )
                 ? new(typedConst)
                 : default;
        }

        /// <summary>Gets the named argument constant as a specified type</summary>
        /// <typeparam name="T">Type of the value to retrieve if present</typeparam>
        /// <param name="name">Name of the attribute argument</param>
        /// <returns><see cref="Optional{T}"/> for the value</returns>
        /// <remarks>
        /// The <paramref name="name"/> name may not be specified in which case the result
        /// will have not value (<see cref="Optional{T}.HasValue"/> is false). It is also
        /// possible that it was specified AND that the value is null (if T is a nullable type
        /// or a default instance if it is not.) Thus it is important to examine the return
        /// to know if a value was specified that happens to be the default value for a type.
        /// </remarks>
        public Optional<T> GetNamedArgValue<T>( string name )
        {
            var argInfo = GetNamedArgValue(name);
            return !argInfo.HasValue
                 ? default
                 : new((T)argInfo.Value.Value!);
        }

        /// <summary>Gets the named argument constant as an <see cref="ImmutableArray{T}"/> of elements of specified type</summary>
        /// <typeparam name="TElement">Type of the value to retrieve if present</typeparam>
        /// <param name="argName">Name of the attribute argument</param>
        /// <returns><see cref="Optional{T}"/> for the array of values</returns>
        /// <remarks>
        /// The <paramref name="name"/> name may not be specified in which case the result
        /// will have not value (<see cref="Optional{T}.HasValue"/> is false). It is also
        /// possible that it was specified AND that the value is null (if T is a nullable type
        /// or a default instance if it is not.) Thus it is important to examine the return
        /// to know if a value was specified that happens to be the default value for a type.
        /// <note type="important">
        /// As nullability is NOT part of the type <typeparamref name="TElement"/> this method
        /// assumes it is allowed. Therefore, the resulting array may contain null values if
        /// that is what was provided to the attribute in the original source.
        /// </note>
        /// </remarks>
        public Optional<ImmutableArray<TElement>> GetNamedArgValueArray<TElement>( string argName )
        {
            var elementType = typeof( TElement );
            if(elementType.IsArray)
            {
                throw new InvalidOperationException("Arrays of arrays not supported. TElement must be a scalar.");
            }

            if(!NamedArguments.TryGetValue( argName, out StructurallyEquatableTypedConstant typedConst )
            || typedConst.IsNull
            || typedConst.Kind != TypedConstantKind.Array
            )
            {
                Debug.WriteLineIf( !typedConst.IsNull && typedConst.Kind != TypedConstantKind.Array, $"Non array named attribute; retrieved as array! '{argName}'" );
                return default;
            }

            // Nullability of TElement is not known, if the typed constant indicates it is a null value
            // then that is what it is. This does not skip such a thing nor attempt to validate nullability.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            return new( [ .. typedConst.Values.Select( static tc => (TElement)tc.Value ) ] );
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        /// <inheritdoc/>
        public bool Equals( EquatableAttributeData other )
        {
            return other is not null
                && Name == other.Name
                && StructuralComparisons.StructuralEqualityComparer.Equals(ConstructorArguments, other.ConstructorArguments)
                && StructuralComparisons.StructuralEqualityComparer.Equals(NamedArguments, other.NamedArguments);
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return HashCode.Combine(
                Name,
                StructuralComparisons.StructuralEqualityComparer.GetHashCode( ConstructorArguments ),
                StructuralComparisons.StructuralEqualityComparer.GetHashCode( NamedArguments )
            );
        }

        /// <inheritdoc/>
        public override bool Equals( object obj )
        {
            return obj is EquatableAttributeData other
                && Equals( other );
        }

        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Implicit cast for public constructor" )]
        public static implicit operator EquatableAttributeData( AttributeData data )
        {
            return new( data );
        }
    }
}

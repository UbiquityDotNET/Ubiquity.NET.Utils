// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    internal class PropertyInfo
        : IEquatable<PropertyInfo>
    {
        /// <summary>Initializes a new instance of the <see cref="PropertyInfo"/> class.</summary>
        /// <param name="property">Property symbol to capture</param>
        /// <param name="attributeNames">Names of attributes to capture</param>
        /// <remarks>
        /// Typically, a generator is only dealing with a subset of all attributes for a property.
        /// and may ignore the property outright if it doesn't have any of the requisite attributes.
        /// </remarks>
        public PropertyInfo( IPropertySymbol property, IEnumerable<NamespaceQualifiedName> attributeNames )
            : this(
                property.Name,
                property.Type.GetNamespaceQualifiedName(),
                property.CaptureMatchingAttributes( attributeNames )
              )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PropertyInfo"/> class.</summary>
        /// <param name="simpleName">Simple name of this property</param>
        /// <param name="typeName">Name of the type of this property</param>
        /// <param name="attributes">Map of Attributes for this property, keyed by the name of the attribute</param>
        public PropertyInfo(
            string simpleName,
            NamespaceQualifiedTypeName typeName,
            EquatableAttributeDataCollection attributes
            )
        {
            SimpleName = simpleName;
            TypeName = typeName;
            Attributes = attributes;
        }

        /// <summary>Gets a value indicating whether this instance is default constructed</summary>
        public bool IsDefault => SimpleName is null
                              && TypeName is null
                              && Attributes is null;

        /// <summary>Gets the simple name of <em><b>this</b></em> property</summary>
        public string SimpleName { get; }

        /// <summary>Gets the namespace qualified name of the type of <em><b>this</b></em> property</summary>
        public NamespaceQualifiedTypeName TypeName { get; }

        /// <summary>Gets a dictionary of generating attributes for this property</summary>
        /// <remarks>
        /// Ordinarily a generator pipeline doesn't capture this data and instead has a pipeline for each attribute.
        /// However, in the case of generating the binder, it needs to generate different code if the contributing
        /// property is "required".
        /// </remarks>
        public EquatableAttributeDataCollection Attributes { get; }

        /// <summary>Gets a value indicating whether this property is required</summary>
        /// <remarks>
        /// The concept "required" applies to the execution of the action for the command line. If a required
        /// value is not provided (and no default is set), then an error is produced at the time of invocation.
        /// <note type="important">
        /// Each attribute is different but the "Required" named argument is designed for consistency across all.
        /// Therefore, this will use a common name.
        /// </note>
        /// </remarks>
        public bool IsRequired
        {
            get
            {
                foreach(var attr in Attributes)
                {
                    var required = attr.GetNamedArgValue(Constants.CommonAttributeNamedArgs.Required);
                    if(required.HasValue && (bool)(required.Value.Value!))
                    {
                        return true;
                    }
                }

                // no attribute with the correct named arg set to "true"
                return false;
            }
        }

        public bool Equals( PropertyInfo other )
        {
            bool retVal = SimpleName == other.SimpleName
              && TypeName == other.TypeName
              && StructuralComparisons.StructuralEqualityComparer.Equals(Attributes, other.Attributes);

            return retVal;
        }

        public override bool Equals( object obj )
        {
            return obj is PropertyInfo other
                && Equals( other );
        }

        public override int GetHashCode( )
        {
            return HashCode.Combine(
                SimpleName,
                TypeName,
                StructuralComparisons.StructuralEqualityComparer.GetHashCode( Attributes )
                );
        }
    }
}

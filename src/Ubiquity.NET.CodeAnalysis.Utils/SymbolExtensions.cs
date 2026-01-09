// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Utility class to host extensions for <see cref="ISymbol"/></summary>
    public static class SymbolExtensions
    {
        /// <summary>Tries to find the first attribute with a matching name for an <see cref="ISymbol"/></summary>
        /// <param name="self">Symbol to test</param>
        /// <param name="names">Sequence of valid names of attributes to match</param>
        /// <returns><see cref="AttributeData"/> for the attribute</returns>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <remarks>
        /// As an optimization, this does not test the full namespace of the attribute until a match
        /// of the simple name is found.
        /// </remarks>
        public static AttributeData? FindFirstMatchingAttribute( this ISymbol self, IEnumerable<NamespaceQualifiedName> names )
        {
            foreach(var attribute in MatchingAttributes( self, names ))
            {
                return attribute;
            }

            return null;
        }

        /// <summary>Tries to find the attributes with a matching name for an <see cref="ISymbol"/></summary>
        /// <param name="self">Symbol to test</param>
        /// <param name="names">Sequence of valid names of attributes to match</param>
        /// <returns><see cref="AttributeData"/> for the attribute</returns>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <remarks>
        /// As an optimization, this does not test the full namespace of the attribute until a match
        /// of the simple name is found.
        /// </remarks>
        public static ImmutableArray<AttributeData> MatchingAttributes( this ISymbol self, IEnumerable<NamespaceQualifiedName> names )
        {
            if(self is null)
            {
                throw new ArgumentNullException( nameof( self ) );
            }

            var existingAttributes = self.GetAttributes();
            var retVal = ImmutableArray.CreateBuilder<AttributeData>(existingAttributes.Length);
            foreach(var attr in existingAttributes)
            {
                foreach(NamespaceQualifiedName name in names)
                {
                    if(attr.IsFullNameMatch( name ))
                    {
                        retVal.Add(attr);
                    }
                }
            }

            return retVal.ToImmutable();
        }

        /// <summary>Captures the attributes with a matching name for an <see cref="ISymbol"/></summary>
        /// <param name="self">Symbol to test</param>
        /// <param name="names">Sequence of valid names of attributes to match</param>
        /// <returns><see cref="AttributeData"/> for the attribute</returns>
        /// <exception cref="ArgumentNullException"><paramref name="self"/> is null</exception>
        /// <remarks>
        /// As an optimization, this does not test the full namespace of the attribute until a match
        /// of the simple name is found.
        /// </remarks>
        public static ImmutableArray<EquatableAttributeData> CaptureMatchingAttributes( this ISymbol self, IEnumerable<NamespaceQualifiedName> names )
        {
            if(self is null)
            {
                throw new ArgumentNullException( nameof( self ) );
            }

            var existingAttributes = self.GetAttributes();
            var retVal = ImmutableArray.CreateBuilder<EquatableAttributeData>(existingAttributes.Length);
            foreach(var attr in existingAttributes)
            {
                foreach(NamespaceQualifiedName name in names)
                {
                    if(attr.IsFullNameMatch( name ))
                    {
                        retVal.Add( attr );
                    }
                }
            }

            return retVal.ToImmutable();
        }
    }
}

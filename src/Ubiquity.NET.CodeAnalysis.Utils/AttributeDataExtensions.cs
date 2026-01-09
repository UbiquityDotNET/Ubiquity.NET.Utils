// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Utility class to provide extensions to <see cref="AttributeData"/></summary>
    public static class AttributeDataExtensions
    {
        /// <summary>Tests if the full namespace qualified name of the name matches this type</summary>
        /// <param name="self">AttributeData to test</param>
        /// <param name="fullName">Sequence of parts for the name to test with outer most namespace first (Including the simple name)</param>
        /// <returns>true if the name matches and false otherwise</returns>
        public static bool IsFullNameMatch( this AttributeData self, NamespaceQualifiedName fullName )
        {
            return self.AttributeClass is not null
                && self.AttributeClass.Name == fullName.SimpleName
                && self.AttributeClass.GetNamespaceNames()
                                      .SequenceEqual( fullName.NamespaceNames );
        }

        /// <summary>Gets the <see cref="NamespaceQualifiedName"/> for the attribute</summary>
        /// <param name="self">self</param>
        /// <returns><see cref="NamespaceQualifiedName"/> for the attribute</returns>
        public static NamespaceQualifiedName GetNamespaceQualifiedName( this AttributeData self )
        {
            return self.AttributeClass is null
                 ? new( [], string.Empty )
                 : self.AttributeClass.GetNamespaceQualifiedName();
        }
    }
}

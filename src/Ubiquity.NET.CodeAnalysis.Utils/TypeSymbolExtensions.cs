// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Utility class to provide extensions for <see cref="INamedTypeSymbol"/></summary>
    public static class TypeSymbolExtensions
    {
        /// <summary>Gets the full name of the symbol (Including namespace names)</summary>
        /// <param name="self">Symbol to get the name from</param>
        /// <returns>Enumerable collection of the name parts with outer most namespace first</returns>
        public static IEnumerable<string> GetFullNameParts( this ITypeSymbol self )
        {
            foreach(string namespacePart in GetNamespaceNames( self ))
            {
                yield return namespacePart;
            }

            yield return self.Name;
        }

        /// <summary>Gets the fill name (including namespaces) of a <see cref="ITypeSymbol"/></summary>
        /// <param name="self"><see cref="ITypeSymbol"/> to get the name from</param>
        /// <returns>full name for the type</returns>
        public static string GetFullName(this ITypeSymbol self)
        {
            return string.Join(".", GetFullNameParts(self));
        }

        /// <summary>Gets the sequence of namespaces names of the symbol (outermost to innermost)</summary>
        /// <param name="self">Symbol to get the name from</param>
        /// <returns>Enumerable collection of the name parts with outer most namespace first</returns>
        public static IEnumerable<string> GetNamespaceNames( this ITypeSymbol self )
        {
            return new NamespacePartReverseIterator(self);
        }

        /// <summary>Gets the <see cref="NamespaceQualifiedTypeName"/> for a symbol</summary>
        /// <param name="self">Symbol to get the name from</param>
        /// <returns><see cref="NamespaceQualifiedTypeName"/> for the symbol</returns>
        public static NamespaceQualifiedTypeName GetNamespaceQualifiedName( this ITypeSymbol self )
        {
            return new( self );
        }

        /// <summary>Gets a value that indicates whether a given type is a nullable value type</summary>
        /// <param name="self">Type to test</param>
        /// <returns>true if the type is a nullable value type and false if not</returns>
        public static bool IsNullableValueType( this ITypeSymbol self )
        {
            return self.NullableAnnotation == NullableAnnotation.Annotated
                && self.IsValueType;
        }

        /// <summary>Determines if this type symbol is a collection</summary>
        /// <param name="self">Type symbol to test</param>
        /// <returns>true if the type is a collection and false if not</returns>
        public static bool IsCollection( this ITypeSymbol self )
        {
            var collectionItf = new NamespaceQualifiedTypeName(["System", "Collections"], "ICollection");
            for(int i = 0; i < self.AllInterfaces.Length; ++i)
            {
                var itf = self.AllInterfaces[i];
                if(itf.GetNamespaceQualifiedName() == collectionItf)
                {
                    return true;
                }
            }

            return false;
        }

        // private iterator to defer the perf hit for reverse walk until the names
        // are iterated. The call to GetEnumerator() will take the hit to reverse walk
        // the names.
        private class NamespacePartReverseIterator
            : IEnumerable<string>
        {
            public NamespacePartReverseIterator( ITypeSymbol symbol )
            {
                Symbol = symbol;
            }

            public IEnumerator<string> GetEnumerator( )
            {
                // reverse the hierarchy using a stack as the first name isn't available
                // until all are traversed.
                var nameStack = new Stack<string>( 64 );
                for(ISymbol current = Symbol.ContainingNamespace; current is not null && !string.IsNullOrEmpty( current.Name ); current = current.ContainingNamespace)
                {
                    nameStack.Push( current.Name );
                }

                return nameStack.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator( )
            {
                return GetEnumerator();
            }

            private readonly ITypeSymbol Symbol;
        }
    }
}

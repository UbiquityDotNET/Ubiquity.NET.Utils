// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Simple record to represent a namespace qualified name as a sequence of strings</summary>
    /// <remarks>
    /// This is useful for matching names of attributes where comparison on the simple name is performed first. This filters
    /// out anything without a correct simple name before taking on the cost of establishing the full namespace hierarchical
    /// name.
    /// <note type="important">
    /// This does <em><b>NOT</b></em> support nested names. The syntax, and even support, of that is language dependent.
    /// Additionally, there is no provision to represent the transition to a nested type as the elements of the sequence
    /// are just strings.
    /// </note>
    /// </remarks>
    public class NamespaceQualifiedName
        : IEquatable<NamespaceQualifiedName>
        , IEquatable<Type>
        , IFormattable
    {
        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedName"/> class.</summary>
        public NamespaceQualifiedName()
        {
            NamespaceNames = [];
            SimpleName = string.Empty;
        }

        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedName"/> class.</summary>
        /// <param name="namespaceNames">sequence of namespace names (outermost to innermost)</param>
        /// <param name="simpleName">Unqualified name of the symbol</param>
        public NamespaceQualifiedName( IEnumerable<string> namespaceNames, string simpleName )
        {
            PolyFillExceptionValidators.ThrowIfNull(namespaceNames);
            PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace(simpleName);

            SimpleName = simpleName;
            NamespaceNames = [ .. namespaceNames.Select( s => ValidateNamespacePart(s) ) ];
            static string ValidateNamespacePart( string s, [CallerArgumentExpression(nameof(s))] string? exp = null )
            {
                PolyFillExceptionValidators.ThrowIfNullOrWhiteSpace( s, exp );
                return s;
            }
        }

        /// <summary>Gets the sequence of the namespace names starting with the outermost namespace moving inwards</summary>
        public ImmutableArray<string> NamespaceNames { get; }

        /// <summary>Gets the simple (unqualified) name of the symbol</summary>
        public string SimpleName { get; }

        /// <summary>Gets a string for the namespaces instance</summary>
        /// <remarks>
        /// This essentially returns an enumerable sequence that is the joining of
        /// <see cref="NamespaceNames"/> with "." as the delimiter.
        /// </remarks>
        public string Namespace => string.Join( ".", NamespaceNames );

        /// <summary>Gets the full sequence of the names for this instance</summary>
        /// <remarks>
        /// This essentially returns an enumerable sequence that is the concatenation of
        /// <see cref="NamespaceNames"/> with the <see cref="SimpleName"/> to form a sequence
        /// that contains the full name.
        /// </remarks>
        public IEnumerable<string> FullNameParts
        {
            get
            {
                foreach(string name in NamespaceNames)
                {
                    yield return name;
                }

                yield return SimpleName;
            }
        }

        #region Equality

        /// <inheritdoc/>
        public bool Equals( NamespaceQualifiedName other )
        {
            return FullNameParts.SequenceEqual( other.FullNameParts );
        }

        /// <inheritdoc/>
        public override bool Equals( object obj )
        {
            return obj is NamespaceQualifiedName other
                && Equals( other );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            // CONSIDER: Lazy create this so that overhead is paid only once.

            HashCode hashCode = default;

            foreach(string item in NamespaceNames)
            {
                hashCode.Add( item );
            }

            hashCode.Add( SimpleName );

            return hashCode.ToHashCode();
        }

        /// <summary>Equality operator to test two names for equality</summary>
        /// <param name="left">left name to test</param>
        /// <param name="right">Right name to test</param>
        /// <returns>true if the names are equal</returns>
        public static bool operator ==( NamespaceQualifiedName left, NamespaceQualifiedName right )
        {
            return left.Equals( right );
        }

        /// <summary>Equality operator to test two names for inequality</summary>
        /// <param name="left">left name to test</param>
        /// <param name="right">Right name to test</param>
        /// <returns>true if the names are <em><b>NOT</b></em> equal; false otherwise</returns>
        public static bool operator !=( NamespaceQualifiedName left, NamespaceQualifiedName right )
        {
            return !(left == right);
        }

        /// <summary>Compares this name with that of a <see cref="Type"/></summary>
        /// <param name="other">The type to compare the name to</param>
        /// <returns>true if the name of <paramref name="other"/> matches this name</returns>
        public bool Equals( Type other )
        {
            return ToString() == other.FullName;
        }
        #endregion

        /// <summary>Gets the string representation of the full namespace using '.' as the delimiter</summary>
        /// <returns>Full namespace qualified name as a string (with a global prefix or an alias if available)</returns>
        /// <remarks>
        /// This is just a tail call to <see cref="ToString(string, IFormatProvider?)"/> with "AG" as the format.
        /// That is, the default formatting is to provide an alias if possible and if not available use a global prefixed
        /// name. If any other behavior is desired then <see cref="ToString(string, IFormatProvider?)"/> is available
        /// to allow formatting as needed. (NOTE: That version is called when formatting a string with a specifier
        /// such as <c>var s = $"{MyNamespaceQualifiedName:R}";</c>
        /// </remarks>
        public override string ToString( )
        {
            return ToString("R", null);
        }

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
        [SuppressMessage( "Style", "IDE0046:Convert to conditional expression", Justification = "Result is anything but simpler" )]
        public virtual string ToString( string format, IFormatProvider? formatProvider )
        {
            // default to the C# formatter unless specified.
            formatProvider ??= NamespaceQualifiedNameFormatter.CSharp;

            // if no custom formatter is available, then just produce the raw name.
            var customFormatter = (ICustomFormatter<NamespaceQualifiedName>)formatProvider.GetFormat(typeof(ICustomFormatter<NamespaceQualifiedName>));
            return customFormatter is null
                 ? string.Join(".", FullNameParts)
                 : customFormatter.Format(format, this, formatProvider);
        }

        /// <summary>Implicit conversion to a string (Shorthand for calling <see cref="ToString()"/></summary>
        /// <param name="self">Name to convert</param>
        public static implicit operator string( NamespaceQualifiedName self )
        {
            return self.ToString();
        }
    }
}

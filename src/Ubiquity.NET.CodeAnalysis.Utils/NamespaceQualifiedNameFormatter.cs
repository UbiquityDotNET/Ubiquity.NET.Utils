// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CodeAnalysis.Utils
{
    /// <summary>Custom formatter for <see cref="NamespaceQualifiedName"/></summary>
    public class NamespaceQualifiedNameFormatter
        : IFormatProvider
        , ICustomFormatter<NamespaceQualifiedName>
    {
        /// <summary>Initializes a new instance of the <see cref="NamespaceQualifiedNameFormatter"/> class.</summary>
        /// <param name="globalPrefix">Global prefix for the language this formatter will produce</param>
        /// <param name="aliasMap">Map of aliases for the language (Key is the full namespace name of a type, the values are the alias)</param>
        public NamespaceQualifiedNameFormatter( string globalPrefix, IReadOnlyDictionary<string, string> aliasMap )
        {
            GlobalPrefix = globalPrefix ?? string.Empty;
            AliasMap = aliasMap ?? throw new ArgumentNullException(nameof(aliasMap));
        }

        /// <summary>Gets the global prefix for the language formatting (example: "global::")</summary>
        public string GlobalPrefix { get; }

        /// <summary>Gets the map of type aliases this formatter uses to format an alias</summary>
        /// <remarks>
        /// The keys for this map are a fully qualified type names and the values are the aliases
        /// used in place of the full name.
        /// </remarks>
        public IReadOnlyDictionary<string, string> AliasMap { get; }

        /// <inheritdoc/>
        public string Format( string format, object arg, IFormatProvider? formatProvider )
        {
            return arg is not NamespaceQualifiedName self
                 ? string.Empty
                 : Format(format, self, formatProvider);
        }

        /// <summary>Formats this instance according to the args</summary>
        /// <param name="format">Format string for this instance (see remarks)</param>
        /// <param name="arg">The value to format</param>
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
        public string Format( string format, NamespaceQualifiedName arg, IFormatProvider? formatProvider )
        {
            // default to global prefix or alias if not specified.
            format ??= "R";

            string rawName = string.Join( ".", arg.FullNameParts );
            if(format == "R")
            {
                return rawName;
            }

            // Try an alias first as that might be the return value
            if((format == "A" || format == "AG") && TryFormatLanguageAlias( rawName, out string? alias ))
            {
                return alias;
            }

            // not an alias so, try global prefix if requested.
            // The only reason that won't add a prefix is if the GlobalPrefix is null, empty or all whitespace.
            // In that case (or if the prefix isn't requested), the full name is returned.
            return (format == "G" || format == "AG") && TryPrefixGlobal( rawName, out string? prefixedName )
                 ? prefixedName
                 : rawName;
        }

        /// <inheritdoc/>
        public object? GetFormat( Type formatType )
        {
            return formatType == typeof( ICustomFormatter<NamespaceQualifiedName> )
                  ? this
                  : null;
        }

        private bool TryFormatLanguageAlias( string fullName, [MaybeNullWhen( false )] out string alias )
        {
            return AliasMap.TryGetValue( fullName, out alias );
        }

        private bool TryPrefixGlobal( string fullName, [MaybeNullWhen( false )] out string prefixedName )
        {
            prefixedName = null;
            if(string.IsNullOrWhiteSpace( GlobalPrefix ))
            {
                return false;
            }

            prefixedName = $"{GlobalPrefix}{fullName}";
            return true;
        }

        /// <summary>Gets a formatter for the C# language</summary>
        public static NamespaceQualifiedNameFormatter CSharp { get; }
            = new NamespaceQualifiedNameFormatter(
                "global::",
                /* see: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types */
                new Dictionary<string, string>()
                {
                    [ "System.Boolean" ] = "bool",
                    [ "System.Byte" ] = "byte",
                    [ "System.SByte" ] = "sbyte",
                    [ "System.Char" ] = "char",
                    [ "System.Decimal" ] = "decimal",
                    [ "System.Double" ] = "double",
                    [ "System.Single" ] = "float",
                    [ "System.Int32" ] = "int",
                    [ "System.UInt32" ] = "uint",

                    // These aren't quite an alias in C# 9 & 10, but C# 11 made them FULL aliases as used here
                    [ "System.IntPtr" ] = "nint",
                    [ "System.UIntPtr" ] = "nuint",

                    [ "System.Int64" ] = "long",
                    [ "System.UInt64" ] = "ulong",
                    [ "System.Int16" ] = "short",
                    [ "System.UInt16" ] = "ushort",

                    [ "System.Object" ] = "object",
                    [ "System.String" ] = "string",
                    [ "System.Delegate" ] = "delegate",

                    // ["????"] = "dynamic", // No way to map in this direction
                }
            );
    }
}

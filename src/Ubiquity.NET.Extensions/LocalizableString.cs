// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>A string that may possibly be formatted differently depending on culture.</summary>
    /// <remarks>This is similar to
    /// <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.localizablestring">Micorosoft.CodeAnalysis.LocalizableString</see>
    /// except that it <em><b>DOES NOT</b></em> transform exceptions to an event. No such event exists
    /// and exceptions are handled in the usual way.</remarks>
    public abstract class LocalizableString
        : IFormattable
        , IEquatable<LocalizableString?>
    {
        /// <inheritdoc/>
        string IFormattable.ToString( string? format, IFormatProvider? formatProvider )
        {
            // NO format specifiers are known so null/empty are the only valid values
            return string.IsNullOrEmpty( format )
                ? GetText( formatProvider ) ?? string.Empty
                : throw new ArgumentException( "Unknown format specifier", nameof( format ) );
        }

        /// <inheritdoc/>
        public sealed override string? ToString( )
        {
            return GetText( CultureInfo.CurrentCulture );
        }

        /// <inheritdoc/>
        public bool Equals( LocalizableString? other )
        {
            return Equals( (object?)other );
        }

        /// <summary>Gets the formatted text of this string</summary>
        /// <param name="formatProvider">Provider to use for formatting</param>
        /// <param name="culture">Culture to use for the text [Default: null (see remarks)]</param>
        /// <returns>Formatted string</returns>
        /// <remarks>
        /// If <paramref name="culture"/> is null the Default used is <see cref="CultureInfo.CurrentUICulture"/>.
        /// </remarks>
        public abstract string GetText( IFormatProvider? formatProvider, CultureInfo? culture = null );

        /// <inheritdoc/>
        public abstract override int GetHashCode( );

        /// <inheritdoc/>
        public abstract override bool Equals( object? obj );

        /// <summary>Creates a <see cref="LocalizableString"/> from a fixed string</summary>
        /// <param name="fixedResource">Fixed string to wrap</param>
        /// <returns><see cref="LocalizableString"/> instance that wraps <paramref name="fixedResource"/></returns>
        public static LocalizableString From( string? fixedResource )
        {
            return LiteralLocalizableString.Create( fixedResource );
        }

        /// <summary>Explicit conversion to a string (Performs formatting using default provider and culture)</summary>
        /// <param name="localizableResource">String to convert</param>
        public static explicit operator string?( LocalizableString localizableResource )
        {
            return localizableResource.GetText( null );
        }

        /// <summary>Implicit creation of a localized string from a fixed resource</summary>
        /// <param name="fixedResource">Fixed resource to use for the string</param>
        [SuppressMessage( "Usage", "CA2225:Operator overloads have named alternates", Justification = "Exists with simpler naming" )]
        public static implicit operator LocalizableString( string? fixedResource )
        {
            return From( fixedResource );
        }
    }

    [SuppressMessage( "StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "DUH, it's file scoped..." )]
    file sealed class LiteralLocalizableString
        : LocalizableString
    {
        public LiteralLocalizableString( string fixedString )
        {
            FixedString = fixedString;
        }

        public override string GetText( IFormatProvider? formatProvider, CultureInfo? culture = null )
        {
            return FixedString;
        }

        public override int GetHashCode( )
        {
            return FixedString?.GetHashCode(StringComparison.Ordinal) ?? 0;
        }

        public override bool Equals( object? obj )
        {
            return obj is LiteralLocalizableString fixedStr
                && string.Equals( FixedString, fixedStr.FixedString, StringComparison.Ordinal );
        }

        /// <summary>Creates a LiteralLocalizableString from nullable string</summary>
        /// <param name="fixedResource">String to wrap</param>
        /// <returns>Wrapped string</returns>
        /// <remarks>if <paramref name="fixedResource"/> is null then the result is <see cref="LiteralLocalizableString.Empty"/></remarks>
        public static LiteralLocalizableString Create( string? fixedResource )
        {
            return string.IsNullOrEmpty( fixedResource ) // .NET Standard 2.0 is missing the NotNullAttribute on this
                 ? Empty
                 : new LiteralLocalizableString( fixedResource! );
        }

        private readonly string FixedString;

        private static readonly LiteralLocalizableString Empty = new(string.Empty);
    }
}

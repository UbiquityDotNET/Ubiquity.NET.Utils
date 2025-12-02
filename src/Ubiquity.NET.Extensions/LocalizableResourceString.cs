// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

// origin: https://github.com/dotnet/roslyn/blob/main/src/Compilers/Core/Portable/Diagnostic/LocalizableResourceString.cs
// heavily modified to operate independently and conform to repo standards and across multiple runtimes.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System.Resources;

namespace Ubiquity.NET.Extensions
{
    /// <summary>A localizable resource string that may possibly be formatted differently depending on culture.</summary>
    /// <remarks>
    /// This is similar to <see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.localizableresourcestring">Microsoft.CodeAnalysis.LocalizableResourceString</see>
    /// but is adapted to multiple runtimes and does not use the <c>resourceSource</c> AND the format args are of type <see cref="object"/>
    /// instead of restricted to <see cref="string"/>.
    /// </remarks>
    /// <seealso href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.localizableresourcestring"/>
    public sealed class LocalizableResourceString
        : LocalizableString
    {
        /// <inheritdoc cref="LocalizableResourceString.LocalizableResourceString(string, ResourceManager, object?[])"/>
        public LocalizableResourceString( string nameOfLocalizableResource, ResourceManager resourceManager )
            : this( nameOfLocalizableResource, resourceManager, [] )
        {
        }

        /// <summary>Initializes a new instance of the <see cref="LocalizableResourceString"/> class.</summary>
        /// <param name="nameOfLocalizableResource">nameof the resource that needs to be localized.</param>
        /// <param name="resourceManager"><see cref="System.Resources.ResourceManager"/> for the calling assembly.</param>
        /// <param name="formatArguments">Optional arguments for formatting the localizable resource string.</param>
        public LocalizableResourceString(
            string nameOfLocalizableResource,
            ResourceManager resourceManager,
            params object?[] formatArguments
            )
        {
#if NETCOREAPP2_1_OR_GREATER
            ArgumentNullException.ThrowIfNull( nameOfLocalizableResource );
            ArgumentNullException.ThrowIfNull( resourceManager );
            ArgumentNullException.ThrowIfNull( formatArguments );
#else
            PolyFillExceptionValidators.ThrowIfNull( nameOfLocalizableResource );
            PolyFillExceptionValidators.ThrowIfNull( resourceManager );
            PolyFillExceptionValidators.ThrowIfNull( formatArguments );
#endif
            ResourceManager = resourceManager;
            NameOfLocalizableResource = nameOfLocalizableResource;
            FormatArguments = formatArguments;
        }

        /// <inheritdoc/>
        public override string GetText( IFormatProvider? formatProvider, CultureInfo? culture = null )
        {
            string resourceString = ResourceManager.GetString(NameOfLocalizableResource, culture) ?? string.Empty;
            return FormatArguments.Length == 0
                 ? resourceString
                 : string.Format( formatProvider, resourceString, FormatArguments );
        }

        /// <inheritdoc/>
        public override int GetHashCode( )
        {
            return HashCode.Combine(
                NameOfLocalizableResource,
                ResourceManager,
                FormatArguments
                );
        }

        /// <inheritdoc/>
        public override bool Equals( object? obj )
        {
            return obj is LocalizableResourceString otherResourceString
                && NameOfLocalizableResource == otherResourceString.NameOfLocalizableResource
                && ResourceManager == otherResourceString.ResourceManager
#if NET6_0_OR_GREATER
                && FormatArguments.SequenceEqual( otherResourceString.FormatArguments, EqualityComparer<object?>.Default );
#else
                && PolyFillSequenceEqual( FormatArguments, otherResourceString.FormatArguments );
#endif
        }

#if !NET6_0_OR_GREATER
        private static bool PolyFillSequenceEqual( ReadOnlySpan<object?> lhs, ReadOnlySpan<object?> rhs )
        {
            // For older runtimes the type 'object' used for format args is not usable as it
            // doesn't implement IEquatable and there's no overload available that accepts a
            // comparer. So, do it the hard way but optimize if possible.
            if(lhs == rhs)
            {
                return true;
            }

            if(lhs.Length != rhs.Length)
            {
                return false;
            }

            for(int i = 0; i < lhs.Length; ++i)
            {
                if(!EqualityComparer<object?>.Default.Equals( lhs[ i ], rhs[ i ] ))
                {
                    return false;
                }
            }

            return true;
        }
#endif

        private readonly string NameOfLocalizableResource;
        private readonly ResourceManager ResourceManager;
        private readonly object?[] FormatArguments;
    }
}

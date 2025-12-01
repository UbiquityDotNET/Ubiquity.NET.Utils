// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions.Properties
{
    // TODO: Generate this from an attributed partial type; Attribute should include the ResX generated type
    // that has the ResourceManager property. The class has no base nor does it implement an interface
    // and there's no .NET equivalent to a C++ 'concept' yet this is all boiler plate that is generalizable.

    internal static class SR
    {
        internal static LocalizableResourceString Build( [NotNull] string resourceName, params object?[] args )
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull(resourceName);
#else
            if( resourceName is null )
            {
                throw new ArgumentNullException(nameof(resourceName));
            }
#endif
            return new LocalizableResourceString( resourceName, Resources.ResourceManager, args );
        }

        // If an overload of this exists that has the same parameters except for format provider
        // then you get; `CA1305: Specify IFormatProvider` which would have to be suppressed, so
        // just specify it, and move on...
        internal static string Format( IFormatProvider formatProvider, [NotNull] string resourceName, params object?[] args )
        {
            return Build( resourceName, args ).GetText(formatProvider) ?? string.Empty;
        }
    }
}

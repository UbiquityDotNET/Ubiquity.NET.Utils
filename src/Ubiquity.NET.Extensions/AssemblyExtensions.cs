// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions
{
    /// <summary>Utility class to provide extensions for consumers</summary>
    [SuppressMessage( "Design", "CA1034:Nested types should not be visible", Justification = "Extension" )]
    public static class AssemblyExtensions
    {
        /// <summary>Gets the informational version from an assembly</summary>
        /// <param name="self">Assembly to extract the version from</param>
        /// <param name="exp">Expression for the assembly to retrieve the attribute data from; normally provided by compiler</param>
        /// <returns>String contents from the <see cref="AssemblyInformationalVersionAttribute"/> in the assembly or <see cref="string.Empty"/></returns>
        public static string GetInformationalVersion(this Assembly self, [CallerArgumentExpression( nameof( self ) )] string? exp = null )
        {
#if NET6_0_OR_GREATER
            ArgumentNullException.ThrowIfNull( self, exp );
#else
            PolyFillExceptionValidators.ThrowIfNull( self, exp );
#endif

            var assemblyVersionAttribute = self.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            return assemblyVersionAttribute is not null
                ? assemblyVersionAttribute.InformationalVersion
                : self.GetName().Version?.ToString() ?? string.Empty;
        }
    }
}

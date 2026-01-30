// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.
#if !NET5_0_OR_GREATER

#pragma warning disable IDE0130 // Namespace does not match folder structure

namespace System
{
    /// <summary>Poly fill extensions for <see cref="OperatingSystem"/></summary>
    /// <remarks>
    /// This cannot use the C# 14 extension keyword to add a static method to the existing type. As it only applies IF
    /// that's not available in the first place.
    /// </remarks>
    [SuppressMessage("Design", "CA1034: Nested types should not be visible", Justification = "extension, broken analyzer")]
    public static class PolyFillOperatingSystem
    {
        /// <summary>Indicates whether the current application is running on Windows.</summary>
        /// <returns><see langword="true"/> if the current application is running on Windows; <see langword="false"/> otherwise.</returns>
        [SuppressMessage( "MicrosoftCodeAnalysis", "RS1035:Banned Symbol", Justification = "Poly Fill extension API" )]
        public static bool IsWindows()
        {
            return Environment.OSVersion.Platform switch
            {
                PlatformID.Win32S or
                PlatformID.Win32Windows or
                PlatformID.Win32NT or
                PlatformID.WinCE => true,
                _ => false,
            };
        }

        // other forms of Is* are more difficult to poly fill as Linux, macOS, iOS, and android, are all apparently reported as PlatformId.Unix
        // So they need to rely on additional native interop APIs for Unix AND type name searches
        // see: https://github.com/ryancheung/PlatformUtil/blob/master/PlatformUtil/PlatformInfo.cs
    }
}
#endif

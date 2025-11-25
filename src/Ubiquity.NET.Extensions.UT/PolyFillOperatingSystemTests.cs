// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions.UT
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public sealed class PolyFillOperatingSystemTests
    {
        [TestMethod]
        public void IsWindows_reports_correct_value( )
        {
#if NET5_0_OR_GREATER
            Assert.Inconclusive("Test not applicable if targeting runtime >= 5");
#else
            bool isWindows = Environment.OSVersion.Platform switch
            {
                PlatformID.Win32S or
                PlatformID.Win32Windows or
                PlatformID.Win32NT or
                PlatformID.WinCE => true,
                _ => false,
            };

            Assert.AreEqual(isWindows, PolyFillOperatingSystem.IsWindows());
#endif
        }
    }
}

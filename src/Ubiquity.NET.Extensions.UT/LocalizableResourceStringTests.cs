// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Globalization;

namespace Ubiquity.NET.Extensions.UT
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LocalizableResourceStringTests
    {
        [TestMethod]
        public void Constructor_throws_on_null_arguments( )
        {
            // Point of this test is to VERIFY the nullability checks
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var mgr = new ResourceManager(typeof(LocalizableResourceStringTests));
            var nex = Assert.ThrowsExactly<ArgumentNullException>( ( ) =>
            {
                _ = new LocalizableResourceString( null, mgr );
            } );

            Assert.AreEqual( "nameOfLocalizableResource", nex.ParamName );

            nex = Assert.ThrowsExactly<ArgumentNullException>( ( ) =>
            {
                _ = new LocalizableResourceString( "someName", null );
            } );

            Assert.AreEqual( "resourceManager", nex.ParamName );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void GetText_produces_expected_resource( )
        {
            const string expected1 = "[TestResources1] Test Message";
            const string expected2 = "[TestResources2] Test Message";
            const string resName = "TestString";

            // for testing only care about the neutral language as that's the only one
            // created for this test.
            var testResourceNeutralLang = CultureInfo.GetCultureInfo( "en-US" );

            var localized1 = new LocalizableResourceString( resName, TestResources1.ResourceManager );
            var localized2 = new LocalizableResourceString( resName, TestResources2.ResourceManager );

            Assert.AreEqual( expected1, localized1.GetText( null, testResourceNeutralLang ) );
            Assert.AreEqual( expected2, localized2.GetText( null, testResourceNeutralLang ) );
        }

        [TestMethod]
        public void GetText_produces_expected_resource_for_formatted_strings( )
        {
            const double testValue = 1.23;
            const string expected1 = "[TestResources1] This is a test of a formatted value: 1,23";
            const string expected2 = "[TestResources1] This is a test of a formatted value: 1,23";
            const string resName = "TestFormat";

            // for testing only care about the neutral language as that's the only one
            // created for this test.
            var testResourceNeutralLang = CultureInfo.GetCultureInfo( "en-US" );

            // formatter chosen specifically as different for the culture source uses distinct number formatting
            // CultureInfo IS IFormatter but not all IFormatter instances are a CultureInfo.
            var formatter = CultureInfo.GetCultureInfo("fr");

            var localized1 = new LocalizableResourceString( resName, TestResources1.ResourceManager, testValue );
            var localized2 = new LocalizableResourceString( resName, TestResources2.ResourceManager, testValue );
            var sameAsLocalized1 = new LocalizableResourceString( resName, TestResources1.ResourceManager, testValue );

            Assert.AreEqual( expected1, localized1.GetText( formatter, testResourceNeutralLang ) );
            Assert.AreEqual( expected2, localized2.GetText( formatter, testResourceNeutralLang ) );

            // while available test equality checks
            Assert.AreNotEqual( localized1, localized2 );
            Assert.AreEqual( localized1, sameAsLocalized1 );
        }
    }
}

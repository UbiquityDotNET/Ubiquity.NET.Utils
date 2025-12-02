// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ubiquity.NET.Extensions.UT
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class LocalizableStringTests
    {
        [TestMethod]
        public void Create_from_null_produces_empty_value( )
        {
            var str = LocalizableString.From( null );
            string? actualStr = str.GetText( null );
            Assert.IsNotNull( actualStr, "Should not be null" );
            Assert.IsTrue( string.IsNullOrEmpty( actualStr ), "Should be empty" );
        }

        [TestMethod]
        public void Create_from_empty_string_empty_value( )
        {
            var str = LocalizableString.From( string.Empty );
            string? actualStr = str.GetText( null );
            Assert.IsNotNull( actualStr, "Should not be null" );
            Assert.IsTrue( string.IsNullOrEmpty( actualStr ), "Should be empty" );
        }

        [TestMethod]
        public void Equal_values_result_in_Equality_checks_passing( )
        {
            var str1 = LocalizableString.From( string.Empty );
            var str2 = LocalizableString.From( string.Empty );
            Assert.IsTrue( str1.Equals( str2 ) );
            Assert.IsTrue( str2.Equals( str1 ) );

            var str3 = LocalizableString.From( "Testing 1, 2, 3" );
            var str4 = LocalizableString.From( "Testing 1, 2, 3" );
            Assert.IsTrue( str3.Equals( str4 ) );
            Assert.IsTrue( str4.Equals( str3 ) );
        }

        [TestMethod]
        public void GetHashCode_produces_expected_value( )
        {
            // This tests ONLY fixed strings that are
            // simple wrappers using the adapter pattern
            const string value = "Testing 1, 2, 3";
            var str3 = LocalizableString.From( value );

#if NETCOREAPP3_0_OR_GREATER
            // Should use ordinal as the default, such a method is not available
            // in earlier runtimes.
            Assert.AreEqual( value.GetHashCode(StringComparison.Ordinal), str3.GetHashCode() );
#else
            Assert.AreEqual( value.GetHashCode(), str3.GetHashCode() );
#endif
        }
    }
}

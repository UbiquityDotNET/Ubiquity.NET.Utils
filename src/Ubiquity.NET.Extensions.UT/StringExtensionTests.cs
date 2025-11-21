// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.Extensions.UT
{
    [TestClass]
    public class StringExtensionTests
    {
        [TestMethod]
        public void HasLineEndings_with_null_throws( )
        {
#pragma warning disable CS8604 // Possible null reference argument.
            // nullability guarantees is the point of this test
            string? test = null;
            var ex = Assert.ThrowsExactly<ArgumentNullException>(()=>StringExtensions.HasLineEndings( test ));
            Assert.AreEqual("self", ex.ParamName);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        [TestMethod]
        public void HasLineEndings_detects_all_line_endings( )
        {
            // valid Unicode line endings are "(\r\n|\r|\n|\f|\u0085|\u2028|\u2029)"
            Assert.IsFalse("This is a test".HasLineEndings());
            Assert.IsTrue("This\r is a test".HasLineEndings());
            Assert.IsTrue("This\n is a test".HasLineEndings());
            Assert.IsTrue("This\f is a test".HasLineEndings());
            Assert.IsTrue("This\u0085 is a test".HasLineEndings());
            Assert.IsTrue("This\u2028 is a test".HasLineEndings());
            Assert.IsTrue("This\u2029 is a test".HasLineEndings());
        }
    }
}

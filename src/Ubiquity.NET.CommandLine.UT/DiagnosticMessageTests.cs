// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ubiquity.NET.Extensions;

// Disambiguate from test framework type
using MessageLevel = Ubiquity.NET.Extensions.MessageLevel;

namespace Ubiquity.NET.CommandLine.UT
{
    [TestClass]
    public class DiagnosticMessageTests
    {
        [TestMethod]
        public void Construction_throws_on_invalid_init( )
        {
            Assert.AreEqual( default, MessageLevel.None, "None should be the default (invalid) level" );

            Assert.ThrowsExactly<ArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = "Has Whitespace",
                        Level = MessageLevel.Verbose,
                        Subcategory = default,
                        Text = string.Empty,
                    };
                },
                "Expect exception when code contains whitespace"
            );

            Assert.ThrowsExactly<ArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = MessageLevel.Verbose,
                        Subcategory = "Has Whitespace",
                        Text = string.Empty,
                    };
                },
                "Expect exception when SubCategory contains whitespace"
            );

            Assert.ThrowsExactly<InvalidEnumArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = default,
                        Subcategory = default,
                        Text = string.Empty,
                    };
                },
                "Expect exception on level == none (default value is invalid)"
            );

            Assert.ThrowsExactly<InvalidEnumArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = (MessageLevel)int.MaxValue,
                        Subcategory = default,
                        Text = string.Empty,
                    };
                },
                "Expect exception on undefined value"
            );

            Assert.ThrowsExactly<ArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = MessageLevel.Verbose,
                        Subcategory = default,
                        Text = string.Empty,
                    };
                },
                "Expect exception if text is empty"
            );

            Assert.ThrowsExactly<ArgumentException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = MessageLevel.Verbose,
                        Subcategory = default,
                        Text = " \t\n\r\f",
                    };
                },
                "Expect exception if text is all whitespace"
            );

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            // VERIFYING null behavior
            Assert.ThrowsExactly<ArgumentNullException>(
                static ( ) =>
                {
                    _ = new DiagnosticMessage()
                    {
                        SourceLocation = default,
                        Code = default,
                        Level = MessageLevel.Verbose,
                        Subcategory = default,
                        Text = null,
                    };
                },
                "Expect exception if text is null"
            );
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }

        [TestMethod]
        public void Initialization_sets_proerties_as_expected( )
        {
            const string testCode = "CODE1";
            const string testSubCategory = "TestSubcategory";
            const string testMsg = "This is a test message";
            const MessageLevel testLevel = MessageLevel.Verbose;

            var testLoc = new SourceRange(new SourcePosition(1,2,3), new SourcePosition(2,1,4));
            var testOrigin = new Uri("file://MyOrigin");

            var msg = new DiagnosticMessage( )
            {
                SourceLocation = new SourceLocation(testOrigin, testLoc),
                Code = testCode,
                Level = testLevel,
                Subcategory = testSubCategory,
                Text = testMsg,
            };

            Assert.AreEqual( testCode, msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.AreEqual( testLoc, msg.SourceLocation.Range );
            Assert.AreEqual( testOrigin, msg.SourceLocation.Source );
            Assert.AreEqual( testSubCategory, msg.Subcategory );
            Assert.AreEqual( testMsg, msg.Text );
        }

        [TestMethod]
        public void ToString_produces_Text_if_origin_is_null_or_whitespace( )
        {
            const string testMsg = "This is a test";
            var msg = new DiagnosticMessage( )
            {
                SourceLocation = default,
                Code = default,
                Level = MessageLevel.Error,
                Subcategory = default,
                Text = testMsg,
            };

            Assert.AreSame( testMsg, msg.ToString() );
            Assert.AreSame( testMsg, msg.ToString( "G", CultureInfo.GetCultureInfo( "fr" ) ) );
            Assert.AreSame( testMsg, msg.ToString( "M", CultureInfo.GetCultureInfo( "fr" ) ) );
        }

        [TestMethod]
        public void ToString_produces_correctly_formatted_msg( )
        {
            const string testCode = "CODE1";
            const string testSubCategory = "Subcategory";
            const string testMsg = "This is a test message";
            const MessageLevel testLevel = MessageLevel.Verbose;

            var testRange = new SourceRange(new SourcePosition(1,2,3), new SourcePosition(2,1,4));
            var testOrigin = new Uri("file://C:/MyOrigin.txt");

            // MSBuild format: 'Origin : Subcategory Category Code : Text'
            string expectedMsBuildMsg = $"{Path.Combine("C:", "MyOrigin.txt")}(1, 2, 2, 1) : Subcategory Verbose CODE1 : This is a test message";

            var msg = new DiagnosticMessage( )
            {
                SourceLocation = new SourceLocation(testOrigin, testRange),
                Code = testCode,
                Level = testLevel,
                Subcategory = testSubCategory,
                Text = testMsg,
            };

            string actual = msg.ToString("M", null);
            Assert.AreEqual( expectedMsBuildMsg, msg.ToString( "M", null ) );

            // Test platform specific forms

            // TODO: Support for other runtimes
            //       Test currently assumes all runtime specific forms are MSBuild
            //       As new runtimes are supported this should validate them.
            Assert.AreEqual( expectedMsBuildMsg, msg.ToString() );
            if(OperatingSystem.IsWindows())
            {
                Assert.AreEqual( expectedMsBuildMsg, msg.ToString( "G", CultureInfo.CurrentCulture ) );
            }
            else
            {
                Assert.Inconclusive("Non-Windows platforms use platform specific generic form, not yet accounted for in this test");
            }
        }
    }
}

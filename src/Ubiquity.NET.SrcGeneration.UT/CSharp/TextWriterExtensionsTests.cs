// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.SrcGeneration.UT.CSharp
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TextWriterExtensionsTests
    {
        #region Basic API Validation
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        /// <summary>Validates extension APIs handle nulls, empty strings, etc...</summary>
        [TestMethod]
        public void ExtensionMethods_throw_on_null_or_whitespace_args( )
        {
            var ex = Assert.ThrowsExactly<ArgumentNullException>(()=>TextWriterExtensions.WriteAttribute( null, "fooAttrib" ));
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteAttributeLine( null, "fooAttrib" ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteRemarksComment( null, null ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteSummaryAndRemarksComments( null, null ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteSummaryComment( null, null ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteUsingDirective( null, string.Empty ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteEmptyCommentLine( null ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( null, ["test"] ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( null, (IEnumerable<string>)["test"] ) );
            Assert.AreEqual( "self", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( null, "test" ) );
            Assert.AreEqual( "self", ex.ParamName );

            using var writer = new StringWriter();
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteSummaryAndRemarksComments( writer, "non-null remarks", null ) );
            Assert.AreEqual( "defaultSummary", ex.ParamName );

            var argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteSummaryAndRemarksComments( writer, "non-null remarks", " \t " ) );
            Assert.AreEqual( "defaultSummary", argEx.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( () => TextWriterExtensions.WriteAttributeLine(writer, null));
            Assert.AreEqual("attributeName", ex.ParamName);

            argEx = Assert.ThrowsExactly<ArgumentException>( () => TextWriterExtensions.WriteAttributeLine(writer, string.Empty));
            Assert.AreEqual( "attributeName", argEx.ParamName );

            argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteAttributeLine( writer, "   " ) );
            Assert.AreEqual( "attributeName", argEx.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( () => TextWriterExtensions.WriteAttribute(writer, null));
            Assert.AreEqual( "attributeName", ex.ParamName );

            argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteAttribute( writer, string.Empty ) );
            Assert.AreEqual( "attributeName", argEx.ParamName );

            argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteAttribute( writer, "   " ) );
            Assert.AreEqual( "attributeName", argEx.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteUsingDirective( writer, null ) );
            Assert.AreEqual( "namespaceName", ex.ParamName );

            argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteUsingDirective( writer, string.Empty ) );
            Assert.AreEqual( "namespaceName", argEx.ParamName );

            argEx = Assert.ThrowsExactly<ArgumentException>( ( ) => TextWriterExtensions.WriteUsingDirective( writer, "   " ) );
            Assert.AreEqual( "namespaceName", argEx.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( writer, (string[]?)null ) );
            Assert.AreEqual( "commentLines", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( writer, (IEnumerable<string>?)null ) );
            Assert.AreEqual( "commentLines", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => TextWriterExtensions.WriteCommentLines( writer, (string?)null ) );
            Assert.AreEqual( "commentTextBlock", ex.ParamName );
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        #endregion

        [TestMethod]
        public void WriteAttributeLine_with_no_args_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteAttributeLine("TestIt");
            Assert.AreEqual("[TestIt]" + Environment.NewLine, writer.ToString());

            writer.GetStringBuilder().Clear();
            writer.WriteAttributeLine("AnotherAttribute");
            Assert.AreEqual( "[AnotherAttribute]" + Environment.NewLine, writer.ToString() );
        }

        [TestMethod]
        public void WriteAttribute_with_no_args_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteAttribute( "TestIt" );
            Assert.AreEqual( "[TestIt]", writer.ToString() );

            writer.GetStringBuilder().Clear();
            writer.WriteAttribute( "AnotherAttribute" );
            Assert.AreEqual( "[AnotherAttribute]", writer.ToString() );
        }

        [TestMethod]
        public void WriteAttributeLine_with_args_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteAttributeLine( "TestIt", "baz", "foo=bar" );
            Assert.AreEqual( "[TestIt(baz, foo=bar)]" + Environment.NewLine, writer.ToString() );

            writer.GetStringBuilder().Clear();
            writer.WriteAttributeLine( "AnotherAttribute", "baz:123", "foo=bar" );
            Assert.AreEqual( "[AnotherAttribute(baz:123, foo=bar)]" + Environment.NewLine, writer.ToString() );
        }

        [TestMethod]
        public void WriteAttribute_with_args_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteAttribute( "TestIt", "baz", "foo=bar" );
            Assert.AreEqual( "[TestIt(baz, foo=bar)]", writer.ToString() );

            writer.GetStringBuilder().Clear();
            writer.WriteAttribute( "AnotherAttribute", "baz:123", "foo=bar" );
            Assert.AreEqual( "[AnotherAttribute(baz:123, foo=bar)]", writer.ToString() );
        }

        [TestMethod]
        public void WriteSummaryContent_with_null_or_empty_description_is_nop( )
        {
            using var writer = new StringWriter();
            writer.WriteSummaryComment( null );
            Assert.AreEqual(0, writer.GetStringBuilder().Length);

            writer.WriteSummaryComment( string.Empty );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length );
        }

        [TestMethod]
        public void WriteSummaryContent_with_description_succeeds( )
        {
            // NOTE: Literal strings requires a blank line to indicate a line terminator.
            //       Otherwise, the compiler generates a string without a terminating new line!
            const string expected = """
            /// <summary>description of this API</summary>

            """;

            using var writer = new StringWriter();
            writer.WriteSummaryComment( "description of this API" );
            Assert.AreEqual( expected, writer.ToString() );

            // ensure trimming is applied correctly
            writer.GetStringBuilder().Clear();
            writer.WriteSummaryComment( " \t  description of this API   " );
            Assert.AreEqual( expected, writer.ToString() );
        }

        [TestMethod]
        public void WriteRemarksComment_with_null_or_empty_txt_is_nop( )
        {
            using var writer = new StringWriter();
            writer.WriteRemarksComment( null );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length );

            writer.WriteRemarksComment( string.Empty );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length );
        }

        [TestMethod]
        public void WriteRemarksComment_with_txt_succeeds( )
        {
            const string input = """
            This is a remarks line


            This is another discrete line. The preceding duplicate blank is removed.
            """;

            // NOTE: Literal strings require a blank line to indicate a line terminator.
            //       Otherwise, the compiler generates a string without a terminating new line!
            //       Also note, that there is an intentional trailing whitespace to indicate a
            //       non-empty line for this test.
            const string expected = """
            /// <remarks>
            /// This is a remarks line
            /// 
            /// This is another discrete line. The preceding duplicate blank is removed.
            /// </remarks>

            """;

            using var writer = new StringWriter();
            writer.WriteRemarksComment( input );
            Assert.AreEqual( expected, writer.ToString() );
        }

        [TestMethod]
        public void WriteSummaryAndRemarksComments_with_null_or_empty_txt_is_nop( )
        {
            using var writer = new StringWriter();
            writer.WriteSummaryAndRemarksComments( null );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "null should be nop" );

            writer.WriteSummaryAndRemarksComments( string.Empty );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "empty string should be NOP" );

            writer.WriteSummaryAndRemarksComments( " \t " );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "All whitespace should be nop" );
        }

        [TestMethod]
        public void WriteSummaryAndRemarksComments_with_valid_inputs_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteSummaryAndRemarksComments( null );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "null should be nop" );

            writer.WriteSummaryAndRemarksComments( string.Empty );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "empty string should be NOP" );

            writer.WriteSummaryAndRemarksComments( " \t " );
            Assert.AreEqual( 0, writer.GetStringBuilder().Length, "All whitespace should be nop" );

            const string inputRemarks = """
            This is a remarks line


            This is another discrete line. The preceding duplicate blank is removed.
            """;

            const string defaultSummary = " \t  description of this API   ";

            writer.WriteSummaryAndRemarksComments(inputRemarks, defaultSummary);

            // NOTE: Literal strings requires a blank line to indicate a line terminator.
            //       Otherwise, the compiler generates a string without a terminating new line!
            //       Also note, that there is an intentional trailing whitespace to indicate a
            //       non-empty line for this test.
            const string expected = """
            /// <summary>description of this API</summary>
            /// <remarks>
            /// This is a remarks line
            /// 
            /// This is another discrete line. The preceding duplicate blank is removed.
            /// </remarks>

            """;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteUsingDirective_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteUsingDirective( "My.Namespace" );
            string expected = "using My.Namespace;" + Environment.NewLine;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteEmptyCommentLine_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteEmptyCommentLine( );
            string expected = "//" + Environment.NewLine;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteCommentLine_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteCommentLine( "This is a test" );
            string expected = "// This is a test" + Environment.NewLine;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteCommentLine_with_newLine_throws( )
        {
            using var writer = new StringWriter();
            string[] inputs = [
                "This\r is a test",
                "This\n is a test",
                "This\f is a test",
                "This\u0085 is a test",
                "This\u2028 is a test",
                "This\u2029 is a test",
            ];

            foreach(string test in inputs)
            {
                Assert.ThrowsExactly<FormatException>( ( ) => writer.WriteCommentLine(test));
            }
        }

        [TestMethod]
        public void WriteCommentLines_params_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteCommentLines( "This is a test", "This is another test" );
            string expected = "// This is a test" + Environment.NewLine
                            + "// This is another test" + Environment.NewLine;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteCommentLines_enumerable_succeeds( )
        {
            using var writer = new StringWriter();
            writer.WriteCommentLines( (IEnumerable<string>)["This is a test", "This is another test"] );
            string expected = "// This is a test" + Environment.NewLine
                            + "// This is another test" + Environment.NewLine;
            Assert.AreEqual(expected, writer.ToString());
        }

        [TestMethod]
        public void WriteCommentLines_from_string_succeeds( )
        {
            using var writer = new StringWriter();
            var inputs = new Dictionary<string, string>
            {
                ["This\r is a test"] = "// This" + Environment.NewLine + "// is a test" + Environment.NewLine,
                ["This is\n a test"] = "// This is" + Environment.NewLine + "// a test" + Environment.NewLine,
                ["This is \fa test"] = "// This is" + Environment.NewLine + "// a test" + Environment.NewLine,
                ["This\u0085 is a test"] = "// This" + Environment.NewLine + "// is a test" + Environment.NewLine,
                ["This is\u2028 a test"] = "// This is" + Environment.NewLine + "// a test" + Environment.NewLine,
                ["This is \u2029a test"] = "// This is" + Environment.NewLine + "// a test" + Environment.NewLine,
            };

            int i = 0;
            foreach(var test in inputs)
            {
                writer.WriteCommentLines(test.Key);
                Assert.AreEqual(test.Value, writer.ToString(), $"Result should match for input {test.Key}[{i}]");
                writer.GetStringBuilder().Clear();
                ++i;
            }
        }
    }
}

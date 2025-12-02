// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.Collections.Immutable;
using System.Globalization;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ubiquity.NET.Extensions;

namespace Ubiquity.NET.CommandLine.UT
{
    [TestClass]
    public class DiagnosticReporterExtensionsTests
    {
        [TestMethod]
        public void Report_overload_1_Tests( )
        {
            // Overload 1 is the params support which depends on the runtime version.
            // In runtimes < .NET 9 it is a simple wrapper around an overload for
            // IEnumerable<DiagnosticMesssage>.
            var reporter = new TestReporter();

            DiagnosticReporterExtensions.Report( reporter, TestMessages );
            Assert.HasCount( 1, reporter.ErrorMessages );
            Assert.AreEqual( TestMessages[ 0 ], reporter.ErrorMessages[ 0 ] );

            Assert.HasCount( 1, reporter.WarningMessages );
            Assert.AreEqual( TestMessages[ 1 ], reporter.WarningMessages[ 0 ] );

            Assert.HasCount( 1, reporter.InformationMessages );
            Assert.AreEqual( TestMessages[ 2 ], reporter.InformationMessages[ 0 ] );

            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.AreEqual( TestMessages[ 3 ], reporter.VerboseMessages[ 0 ] );

            reporter = new TestReporter();

            // params overload, for runtimes < 9.0 (C# 13) this is an array params
            // that re-directs to the enumeration form. For later it is an IEnumerable<>
            // directly (https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters#params-modifier)
            DiagnosticReporterExtensions.Report( reporter, TestMessages[ 0 ], TestMessages[ 1 ], TestMessages[ 2 ], TestMessages[ 3 ] );
            Assert.HasCount( 1, reporter.ErrorMessages );
            Assert.AreEqual( TestMessages[ 0 ], reporter.ErrorMessages[ 0 ] );

            Assert.HasCount( 1, reporter.WarningMessages );
            Assert.AreEqual( TestMessages[ 1 ], reporter.WarningMessages[ 0 ] );

            Assert.HasCount( 1, reporter.InformationMessages );
            Assert.AreEqual( TestMessages[ 2 ], reporter.InformationMessages[ 0 ] );

            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.AreEqual( TestMessages[ 3 ], reporter.VerboseMessages[ 0 ] );
        }

        [TestMethod]
        public void Report_overload_2_Tests( )
        {
            // Overload 2 =>void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     [InterpolatedStringHandlerArgument( "self", "level" )] DiagnosticReporterInterpolatedStringHandler handler
            //)

            const MsgLevel testLevel = MsgLevel.Verbose;
            const string testValue = "[Boo]";
            const string expectedMsg = "This is a test value: [Boo]";

            var reporter = new TestReporter(); // Default is capture all
            DiagnosticReporterExtensions.Report( reporter, testLevel, $"This is a test value: {testValue}" );
            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.HasCount( 1, reporter.AllMessages );

            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.IsNull( msg.Location );
            Assert.IsNull( msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( expectedMsg, msg.Text );

            bool setValue = false;

            // Validate interpolated string handler filters correctly
            // Messages captured must be Information+
            reporter = new TestReporter( MsgLevel.Information );
            DiagnosticReporterExtensions.Report( reporter, testLevel, $"This is a test value: {SetValue( testValue )}" );
            Assert.HasCount( 0, reporter.VerboseMessages );
            Assert.HasCount( 0, reporter.AllMessages );
            Assert.IsFalse( setValue, "Interpolated value producer should not be called" );

            // Local function to detect if interpolated string methods are called.
            // The test string includes a prefix so this is valid, though callers
            // should NOT depend on this (Side-effects are confusing enough, code
            // that implicitly depends on them is a nightmare).
            string SetValue( string v )
            {
                setValue = true;
                return v;
            }
        }

        [TestMethod]
        public void Report_overload_3_Tests( )
        {
            // Overload 3 =>void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     string msg
            //)

            const MsgLevel testLevel = MsgLevel.Verbose;
            const string testMsg = "This is a test";

            var reporter = new TestReporter(); // Default is capture all
            DiagnosticReporterExtensions.Report(reporter, testLevel, testMsg );
            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.HasCount( 1, reporter.AllMessages );

            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.IsNull( msg.Location );
            Assert.IsNull( msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( testMsg, msg.Text );

            // Messages captured must be Information+
            reporter = new TestReporter( MsgLevel.Information );
            DiagnosticReporterExtensions.Report( reporter, testLevel, testMsg );
            Assert.HasCount( 0, reporter.VerboseMessages );
            Assert.HasCount( 0, reporter.AllMessages );
        }

        [TestMethod]
        public void Report_overload_4_Tests( )
        {
            // Overload 4 =>void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     SourceRange location,
            //     [InterpolatedStringHandlerArgument( "self", "level" )] DiagnosticReporterInterpolatedStringHandler handler
            //)

            const MsgLevel testLevel = MsgLevel.Verbose;
            var testLocation = new SourceRange(new SourcePosition(2,3,4), new SourcePosition(3,4,5));
            const string testValue = "[Boo]";
            const string expectedMsg = "This is a test value: [Boo]";

            var reporter = new TestReporter(); // Default is capture all
            DiagnosticReporterExtensions.Report( reporter, testLevel, testLocation, $"This is a test value: {testValue}" );
            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.HasCount( 1, reporter.AllMessages );

            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.AreEqual( testLocation, msg.Location );
            Assert.IsNull( msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( expectedMsg, msg.Text );

            bool setValue = false;

            // Validate interpolated string handler filters correctly
            // Messages captured must be Information+
            reporter = new TestReporter( MsgLevel.Information );
            DiagnosticReporterExtensions.Report( reporter, testLevel, testLocation, $"This is a test value: {SetValue( testValue )}" );
            Assert.HasCount( 0, reporter.VerboseMessages );
            Assert.HasCount( 0, reporter.AllMessages );
            Assert.IsFalse( setValue, "Interpolated value producer should not be called" );

            // Local function to detect if interpolated string methods are called.
            // The test string includes a prefix so this is valid, though callers
            // should NOT depend on this (Side-effects are confusing enough, code
            // that implicitly depends on them is a nightmare).
            string SetValue( string v )
            {
                setValue = true;
                return v;
            }
        }

        [TestMethod]
        public void Report_overload_5_Tests( )
        {
            // Overload 5 => void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     Uri? origin,
            //     SourceRange location,
            //     [StringSyntax( StringSyntaxAttribute.CompositeFormat )] string fmt,
            //     params object[] args
            // )

            const MsgLevel testLevel = MsgLevel.Verbose;
            var testLocation = new SourceRange(new SourcePosition(2,3,4), new SourcePosition(3,4,5));
            var testOrigin = new Uri("file://foo");
            const string testFormat = "This is a test value: {0}";
            const string testValue = "[Boo]";
            const string expectedMsg = "This is a test value: [Boo]";

            var reporter = new TestReporter(); // Default is capture all
            DiagnosticReporterExtensions.Report( reporter, testLevel, testOrigin, testLocation, testFormat, testValue );
            Assert.HasCount( 1, reporter.VerboseMessages );
            Assert.HasCount( 1, reporter.AllMessages );

            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.AreEqual( testLocation, msg.Location );
            Assert.AreEqual( testOrigin, msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( expectedMsg, msg.Text );

            // Messages captured must be Information+
            reporter = new TestReporter( MsgLevel.Information );
            DiagnosticReporterExtensions.Report( reporter, testLevel, testOrigin, testLocation, testFormat, testValue );
            Assert.HasCount( 0, reporter.VerboseMessages );
            Assert.HasCount( 0, reporter.AllMessages );
        }

        [TestMethod]
        public void Report_overload6_Tests( )
        {
            // Overload 6 => void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     Uri? origin,
            //     SourceRange location,
            //     [InterpolatedStringHandlerArgument( "self", "level" )] DiagnosticReporterInterpolatedStringHandler handler
            //)

            const MsgLevel level = MsgLevel.Verbose;
            var location = new SourceRange(new SourcePosition(2,3,4), new SourcePosition(3,4,5));
            var origin = new Uri("file://foo");
            string testValue = "[Boo]";
            const string expectedMsg = "This is a test value: [Boo]";

            var reporter = new TestReporter(); // Default is capture all
            DiagnosticReporterExtensions.Report( reporter, level, origin, location, $"This is a test value: {testValue}" );
            Assert.HasCount( 1, reporter.VerboseMessages );
            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( MsgLevel.Verbose, msg.Level );
            Assert.AreEqual( location, msg.Location );
            Assert.AreEqual( origin, msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( expectedMsg, msg.Text );

            bool setValue = false;

            // Validate interpolated string handler filters correctly
            // Messages captured must be Information+
            reporter = new TestReporter( MsgLevel.Information );
            DiagnosticReporterExtensions.Report( reporter, level, $"This is a test value: {SetValue( testValue )}" );
            Assert.HasCount( 0, reporter.VerboseMessages );
            Assert.HasCount( 0, reporter.AllMessages );
            Assert.IsFalse( setValue, "Interpolated value producer should not be called" );

            // Local function to detect if interpolated string methods are called.
            // The test string includes a prefix so this is valid, though callers
            // should NOT depend on this (Side-effects are confusing enough, code
            // that implicitly depends on them is a nightmare).
            string SetValue( string v )
            {
                setValue = true;
                return v;
            }
        }

        [TestMethod]
        public void Report_overload7_Tests( )
        {
            // Overload 7 => void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     Uri? origin,
            //     SourceRange location,
            //     [StringSyntax( StringSyntaxAttribute.CompositeFormat )] string fmt,
            //     params object[] args
            //)

            var reporter = new TestReporter();
            var location = new SourceRange(new SourcePosition(2,3,4), new SourcePosition(3,4,5));
            var origin = new Uri("file://foo");
            string expecteMsg = $"Testing 1, 2, {1.23.ToString(CultureInfo.CurrentCulture)}";

            DiagnosticReporterExtensions.Report( reporter, MsgLevel.Verbose, origin, location, "Testing 1, 2, {0}", 1.23 );
            Assert.HasCount( 1, reporter.VerboseMessages );
            var msg = reporter.VerboseMessages[ 0 ];
            Assert.IsNull( msg.Code );
            Assert.AreEqual( MsgLevel.Verbose, msg.Level );
            Assert.AreEqual( location, msg.Location );
            Assert.AreEqual( origin, msg.Origin );
            Assert.IsNull( msg.Subcategory );
            Assert.AreEqual( expecteMsg, msg.Text );
        }

        [TestMethod]
        public void Report_overload8_Tests( )
        {
            // Overload 8 => void Report(
            //     this IDiagnosticReporter self,
            //     MsgLevel level,
            //     LocalizableString locStr,
            //     IFormatProvider? formatProvider = default,
            //     Uri? origin = default,
            //     SourceRange? location = default,
            //     string? subCategory = default,
            //     string? code = default
            // )

            const MsgLevel testLevel = MsgLevel.Verbose;
            var testLocation = new SourceRange(new SourcePosition(2,3,4), new SourcePosition(3,4,5));
            var testOrigin = new Uri("file://foo");
            const string testMsg = "This is a test";
            var testLocStr = LocalizableString.From("This is a test");
            IFormatProvider testFormatProvider = CultureInfo.GetCultureInfo( "en-US" );
            const string testSubcategory = "subcategory";
            const string testCode = "CODE0001";

            var reporter = new TestReporter();
            DiagnosticReporterExtensions.Report(reporter, testLevel, testLocStr, testFormatProvider, testOrigin, testLocation, testSubcategory, testCode);
            var msg = reporter.VerboseMessages[ 0 ];
            Assert.AreEqual( testCode, msg.Code );
            Assert.AreEqual( testLevel, msg.Level );
            Assert.AreEqual( testLocation, msg.Location );
            Assert.AreEqual( testOrigin, msg.Origin );
            Assert.AreEqual( testSubcategory, msg.Subcategory );
            Assert.AreEqual( testMsg, msg.Text );
        }

        private ImmutableArray<DiagnosticMessage> TestMessages { get; }
          = [
                new DiagnosticMessage( )
                {
                    Code = "Code0",
                    Level = MsgLevel.Error,
                    Location = new Extensions.SourceRange( new SourcePosition( 2, 3, 10 ), new SourcePosition( 3, 3, 12 ) ),
                    Origin = new System.Uri( @"file://foo" ),
                    Subcategory = "subcategory",
                    Text = "Text"
                },
                new DiagnosticMessage( )
                {
                    Code = "Code1",
                    Level = MsgLevel.Warning,
                    Location = new Extensions.SourceRange( new SourcePosition( 2, 3, 10 ), new SourcePosition( 3, 3, 12 ) ),
                    Origin = new System.Uri( @"file://foo" ),
                    Subcategory = "subcategory",
                    Text = "Text"
                },
                new DiagnosticMessage( )
                {
                    Code = "Code2",
                    Level = MsgLevel.Information,
                    Location = new Extensions.SourceRange( new SourcePosition( 2, 3, 10 ), new SourcePosition( 3, 3, 12 ) ),
                    Origin = new System.Uri( @"file://foo" ),
                    Subcategory = "subcategory",
                    Text = "Text"
                },
                new DiagnosticMessage( )
                {
                    Code = "Code3",
                    Level = MsgLevel.Verbose,
                    Location = new Extensions.SourceRange( new SourcePosition( 2, 3, 10 ), new SourcePosition( 3, 3, 12 ) ),
                    Origin = new System.Uri( @"file://foo" ),
                    Subcategory = "subcategory",
                    Text = "Text"
                }
            ];
    }
}

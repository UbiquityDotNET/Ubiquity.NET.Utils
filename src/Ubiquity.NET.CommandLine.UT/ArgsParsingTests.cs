// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.CommandLine;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ubiquity.NET.Extensions;

namespace Ubiquity.NET.CommandLine.UT
{
    [TestClass]
    public class ArgsParsingTests
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        [TestMethod]
        public void ArgsParsing_Methods_handle_args_correctly( )
        {
            var settings = new CommandLineSettings();
            var reporter = new TestReporter();
            ParseResult parseResult = new RootCommand().Parse([]);

            var nullArgEx = Assert.ThrowsExactly<ArgumentNullException>(()=> _ = ArgsParsing.Parse<TestOptions>( null, null ) );
            Assert.AreEqual( "args", nullArgEx.ParamName );

#pragma warning disable CS0618 // Type or member is obsolete
            // Overload 1
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( null, out var _, out int _ ) );
            Assert.AreEqual( "args", nullArgEx.ParamName );

            // Overload 2
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( null, settings, out var _, out int _ ) );
            Assert.AreEqual( "args", nullArgEx.ParamName );

            // Overload 2 [param 2]
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( [], (CommandLineSettings?)null, out var _, out int _ ) );
            Assert.AreEqual( "settings", nullArgEx.ParamName );

            // Overload 3
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( null, reporter, out var _, out int _ ) );
            Assert.AreEqual( "args", nullArgEx.ParamName );

            // Overload 3 [param 2]
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( [], (IDiagnosticReporter?)null, out var _, out int _ ) );
            Assert.AreEqual( "diagnosticReporter", nullArgEx.ParamName );

            // Overload 4
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( null, settings, reporter, out var _, out int _ ) );
            Assert.AreEqual( "args", nullArgEx.ParamName );

            // Overload 4 [param 3] (param 2 [settings] is nullable)
            nullArgEx = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = ArgsParsing.TryParse<TestOptions>( [], null, null, out var _, out int _ ) );
            Assert.AreEqual( "diagnosticReporter", nullArgEx.ParamName );
#pragma warning restore CS0618 // Type or member is obsolete
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [TestMethod]
        public void Parse_succeeds( )
        {
            ParseResult results = ArgsParsing.Parse<TestOptions>(["--option1", "value"]);
            var options = TestOptions.Bind(results);

            Assert.AreEqual( "value", options.Option1 );
        }

        [TestMethod]
        public void TryParse_return_uses_corret_semantics( )
        {
            // semantics of the return is "should exit" to disambiguate from
            // parsed correctly AND invoked default option (like help).
            var settings = new CommandLineSettings();
            var reporter = new TestReporter();
            string[] args = ["--option1", "value"];

#pragma warning disable CS0618 // Type or member is obsolete
            bool shouldExit = ArgsParsing.TryParse<TestOptions>( args, out TestOptions? options, out int exitCode );
            Assert.IsFalse( shouldExit );
            Assert.IsNotNull( options );
            Assert.AreEqual( 0, exitCode );
            Assert.AreEqual( "value", options.Option1 );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, settings, out options, out exitCode );
            Assert.IsFalse( shouldExit );
            Assert.IsNotNull( options );
            Assert.AreEqual( 0, exitCode );
            Assert.AreEqual( "value", options.Option1 );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, reporter, out options, out exitCode );
            Assert.IsFalse( shouldExit );
            Assert.IsNotNull( options );
            Assert.AreEqual( 0, exitCode );
            Assert.AreEqual( "value", options.Option1 );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, settings, reporter, out options, out exitCode );
            Assert.IsFalse( shouldExit );
            Assert.IsNotNull( options );
            Assert.AreEqual( 0, exitCode );
            Assert.AreEqual( "value", options.Option1 );

            // default options present and invoked
            args = ["--help"];
            shouldExit = ArgsParsing.TryParse<TestOptions>( args, out options, out exitCode );
            Assert.IsTrue( shouldExit );
            Assert.IsNull( options );
            Assert.AreEqual( 0, exitCode );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, settings, out options, out exitCode );
            Assert.IsTrue( shouldExit );
            Assert.IsNull( options );
            Assert.AreEqual( 0, exitCode );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, reporter, out options, out exitCode );
            Assert.IsTrue( shouldExit );
            Assert.IsNull( options );
            Assert.AreEqual( 0, exitCode );

            shouldExit = ArgsParsing.TryParse<TestOptions>( args, settings, reporter, out options, out exitCode );
            Assert.IsTrue( shouldExit );
            Assert.IsNull( options );
            Assert.AreEqual( 0, exitCode );
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System;
using System.CommandLine;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ubiquity.NET.Extensions;

// Disambiguate from test framework type
using MessageLevel = Ubiquity.NET.Extensions.MessageLevel;

namespace Ubiquity.NET.CommandLine.UT
{
    [TestClass]
    public class CommandLineTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void CommandLine_parse_with_version_option_only_succeeds( )
        {
            var settings = CreateTestSettings();
            var result = ArgsParsing.Parse<TestOptions>(["--version"], settings);

            Assert.HasCount( 0, result.Errors, "Version alone should not produce errors" );

            var versionOption = result.GetVersionOption();
            Assert.IsNotNull( versionOption );
            Assert.AreEqual( versionOption.Action, result.Action );
        }

        [TestMethod]
        public void CommandLine_with_help_option_only_succeeds( )
        {
            var settings = CreateTestSettings();

            var result = ArgsParsing.Parse<TestOptions>(["--help"], settings);
            Assert.HasCount( 0, result.Errors );
        }

        [TestMethod]
        public void CommandLine_with_unknown_option_has_errors( )
        {
            var settings = CreateTestSettings();
            ParseResult result = ArgsParsing.Parse<TestOptions>(["--FooBar"], settings );
            Assert.HasCount( 2, result.Errors, "Errors should include missing Required, and invalid param" );
        }

        [TestMethod]
        public void CommandLine_with_valid_options_succeeds( )
        {
            var settings = CreateTestSettings();
            ParseResult result = ArgsParsing.Parse<TestOptions>(["--option1", "value1"], settings );
            Assert.HasCount( 0, result.Errors, "Should succeed without errors" );
            var options = TestOptions.Bind(result);
            Assert.AreEqual( "value1", options.Option1 );
        }

        [TestMethod]
        public void CommandLine_with_known_option_and_version_has_errors( )
        {
            var settings = CreateTestSettings();
            ParseResult result = ArgsParsing.Parse<TestOptions>(["--version", "--option1"], settings );

            Assert.HasCount( 1, result.Errors, "Should be one error (--version must be set alone) [Other errors ignored for --version]" );
        }

        // until https://github.com/dotnet/command-line-api/issues/2664 is resolved this will fail
        [TestMethod]
        [Ignore( "https://github.com/dotnet/command-line-api/issues/2664" )]
        public void CommandLine_with_known_option_requiring_arg_and_version_has_errors( )
        {
            var settings = CreateTestSettings();
            ParseResult result = ArgsParsing.Parse<TestOptions>(["--option1", "--version"], settings );

            Assert.HasCount( 2, result.Errors, "Should be two errors ([0]--version must be set alone, [1] missing arg for --option1)" );
        }

        [TestMethod]
        public void Extension_BuildRootCommand_succeeds( )
        {
            var settings = CreateTestSettings();

#if NET10_0_OR_GREATER
            // With C# 14 extensions the type of options is known, so more inference is possible
            var cmd = TestOptions.BuildRootCommand( settings, ( o ) => { } );
            Assert.IsNotNull( cmd );

            cmd = TestOptions.BuildRootCommand( settings, ( o ) => 1 );
            Assert.IsNotNull( cmd );

            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) => Task.CompletedTask );
            Assert.IsNotNull( cmd );

            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) => Task.FromResult(1) );
            Assert.IsNotNull( cmd );
#else
            var cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) => { } );
            Assert.IsNotNull( cmd );

            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) => 1 );
            Assert.IsNotNull( cmd );

            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) => Task.CompletedTask );
            Assert.IsNotNull( cmd );

            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) => Task.FromResult( 1 ) );
            Assert.IsNotNull( cmd );
#endif
        }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        // Nullability checks is the whole point of this test case
        [TestMethod]
        public void Extentions_BuildRootCommand_throws_if_null( )
        {
            var settings = CreateTestSettings();

#if NET10_0_OR_GREATER
            // overload 1
            var ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) =>_ = TestOptions.BuildRootCommand( null, ( o ) => { }));
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( settings, (Action<TestOptions>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 2
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( null, ( o ) => 1 ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( settings, (Func<TestOptions, int>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 3
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( null, ( o, ct ) => Task.CompletedTask ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( settings, (Func<TestOptions, CancellationToken, Task>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 4
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( null, ( o, ct ) => Task.FromResult( 1 ) ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = TestOptions.BuildRootCommand( settings, (Func<TestOptions, CancellationToken, Task<int>>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );
#else

            // overload 1
            var ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) =>_ = RootCommandBuilder.BuildRootCommand( null, ( TestOptions o ) => { }));
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( settings, (Action<TestOptions>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 2
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( null, ( TestOptions o ) => 1 ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( settings, (Func<TestOptions, int>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 3
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( null, ( TestOptions o, CancellationToken ct ) => Task.CompletedTask ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( settings, (Func<TestOptions, CancellationToken, Task>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );

            // overload 4
            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( null, ( TestOptions o, CancellationToken ct ) => Task.FromResult( 1 ) ) );
            Assert.AreEqual( "settings", ex.ParamName );

            ex = Assert.ThrowsExactly<ArgumentNullException>( ( ) => _ = RootCommandBuilder.BuildRootCommand( settings, (Func<TestOptions, CancellationToken, Task<int>>?)null ) );
            Assert.AreEqual( "action", ex.ParamName );
#endif
        }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        [TestMethod]
        public void Extensions_ParseAndInvokeResult_invokes_provided_action( )
        {
            bool actionCalled = false;
            var settings = CreateTestSettings();
            using var stringWriter = new StringWriter();
            var reporter = new TextWriterReporter( MessageLevel.Error, error: stringWriter, warning: stringWriter, information: stringWriter, verbose: stringWriter );

            string[] testArgs = ["--option1", "value1"];

#if NET10_0_OR_GREATER
            // Overload 1
            var cmd = TestOptions.BuildRootCommand( settings, ( o ) => actionCalled = true);
            Assert.IsNotNull( cmd );
            int exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs );
            Assert.IsTrue( actionCalled, "action should be called" );
            Assert.AreEqual( 0, exitCode );

            // Overload 2
            cmd = TestOptions.BuildRootCommand( settings, ( o ) =>
            {
                actionCalled = true;
                return 1;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs );
            Assert.IsTrue( actionCalled, "action should be called" );
            Assert.AreEqual( 1, exitCode );

            // Overload 3
            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs );
            Assert.IsTrue( actionCalled, "action should be called" );
            Assert.AreEqual( 0, exitCode );

            // Overload 4
            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) =>
            {
                actionCalled = true;
                return Task.FromResult( 1 );
            } );

            Assert.IsNotNull( cmd );
            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );
#else
            // Overload 1
            var cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) => actionCalled = true);
            Assert.IsNotNull( cmd );
            int exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 2
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) =>
            {
                actionCalled = true;
                return 1;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );

            // Overload 3
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            });
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 4
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) =>
            {
                actionCalled = true;
                return Task.FromResult( 1 );
            });

            Assert.IsNotNull( cmd );
            actionCalled = false;
            exitCode = cmd.ParseAndInvokeResult( reporter, settings, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );
#endif
        }

        [TestMethod]
        public async Task Extensions_ParseAndInvokeResultAsync_invokes_provided_action( )
        {
            bool actionCalled = false;
            var settings = CreateTestSettings();
            using var stringWriter = new StringWriter();
            var reporter = new TextWriterReporter( MessageLevel.Error, error: stringWriter, warning: stringWriter, information: stringWriter, verbose: stringWriter );

            string[] testArgs = ["--option1", "value1"];

#if NET10_0_OR_GREATER
            // Overload 1
            var cmd = TestOptions.BuildRootCommand( settings, ( o ) => actionCalled = true);
            Assert.IsNotNull( cmd );
            int exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 2
            cmd = TestOptions.BuildRootCommand( settings, ( o ) =>
            {
                actionCalled = true;
                return 1;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );

            // Overload 3
            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 4
            cmd = TestOptions.BuildRootCommand( settings, ( o, ct ) =>
            {
                actionCalled = true;
                return Task.FromResult( 1 );
            } );

            Assert.IsNotNull( cmd );
            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );
#else
            // Overload 1
            var cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) => actionCalled = true);
            Assert.IsNotNull( cmd );
            int exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs );
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 2
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o ) =>
            {
                actionCalled = true;
                return 1;
            } );
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );

            // Overload 3
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) =>
            {
                actionCalled = true;
                return Task.CompletedTask;
            });
            Assert.IsNotNull( cmd );

            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 0, exitCode );

            // Overload 4
            cmd = RootCommandBuilder.BuildRootCommand( settings, ( TestOptions o, CancellationToken ct ) =>
            {
                actionCalled = true;
                return Task.FromResult( 1 );
            });

            Assert.IsNotNull( cmd );
            actionCalled = false;
            exitCode = await cmd.ParseAndInvokeResultAsync( reporter, settings, TestContext.CancellationToken, testArgs);
            Assert.IsTrue( actionCalled );
            Assert.AreEqual( 1, exitCode );
#endif
        }

        internal static CommandLineSettings CreateTestSettings( DefaultOption defaultOptions = DefaultOption.Help | DefaultOption.Version )
        {
            return new CommandLineSettings()
            {
                DefaultOptions = defaultOptions,
            };
        }
    }
}

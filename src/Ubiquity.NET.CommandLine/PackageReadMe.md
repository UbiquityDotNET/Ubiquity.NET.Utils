# Ubiquity.NET.CommandLine
Common Text CommandLine support. This provides a number of support classes for
Text based UI/UX, including command line parsing extensions. This is generally only relevant
for console based apps.

## Example Command line parsing
Normal single command application:
Options binding types are normally split into two partial classes. The normal types that
would contain attributes for source generation and the generated partial implementation.
While there is no source generator (yet) for the options [That is intended for a future
release] this keeps the responsibilities clearer and aids in migration to a source generator.

### Options type
Options.cs:
```
using System.IO;

using Ubiquity.NET.CommandLine;

namespace TestSample
{
    internal partial class Options
    {
        // Option; DefaultValue = MsgLevel.Information
        internal MsgLevel Verbosity { get; init; }

        // Additional option here...
    }
}
```
Options.g.cs
``` C#
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Ubiquity.NET.CommandLine;

namespace TestSample
{
    internal partial class Options
        : ICommandLineOptions<Options>
    {
        private Options()
        {
        }

        public static Options Bind(ParseResult parseResults)
        {
            return new()
            {
                Verbosity = parseResults.GetValue(Descriptors.Verbosity),
            };
        }

        public static AppControlledDefaultsRootCommand BuildRootCommand(CmdLineSettings settings)
        {
            return new AppControlledDefaultsRootCommand( settings, "TestSample App" )
            {
                Descriptors.Verbosity,
            };
        }
    }

    file static class Descriptors
    {
        internal static readonly Option<MsgLevel> Verbosity
            = new Option<MsgLevel>("-v")
            {
                Aliases = {"--verbosity"},
                Description = "Verbosity level",
                DefaultValueFactory = ar => MsgLevel.Information,
            };
    }
}
```
### Common single command/no command case
Most applications don't have or use any concept of commands/sub-commands. Thus, there is
support to make this scenario as simple as possible.
``` C#
public static int Main( string[] args )
{
    // start with information level for parsing; parsed options might specify different level
    var reporter = new ColoredConsoleReporter( MsgLevel.Information );
    var settings = new CmdLineSettings()
    {
        ShowHelpOnErrors = false, // Errors in parsing should NOT show help, only show it if asked to do so.
    };

    return Options.BuildRootCommand( settings, options => AppMain( options, reporter ) )
                  .ParseAndInvokeResult( reporter, settings, args );
}

private static int AppMain( Options options, ColoredConsoleReporter reporter )
{
    // Real main here.
    // The pre-parsed and validated options and the reporter are provided as arguments
    // to this function
}
```
This is simplified usage for the common case of an app without commands/sub-commands.

### Advanced app control of dispatching and validation
Advanced, parsing for customized behavior
``` C#
public static int Main( string[] args )
{
    var reporter = new ColoredConsoleReporter(MsgLevel.Information);
    var settings = new CmdLineSettings()
    {
        ShowHelpOnErrors = false, // Errors in parsing won't show help, only show it if asked to do so.
    };

    if(!ArgsParsing.TryParse<Options>( args, settings, reporter, out Options? options, out int exitCode ))
    {
        return exitCode;
    }

    // ... Deal with options as parsed.
}
```
The ONLY automatic invocation applied here is the default `Help` and `version` options
However, even those can be disabled using a `CmdLineSettings` instance. `Options` is a
custom class that has properties for all parsed commands, arguments and options allowing for
validation of them all in context (including each other). The App can then dispatch behavior
based on the commands/options etc... as needed. NO ASSUMPTION IS MADE ABOUT THE USE OF
COMMANDS NOR THE BEHAVIOR OF THEM. The app is entirely in control of how they are used.
While the underlying System.Commandline does have validation built-in, it is only possible
to validate an item individually. Mutual exclusion or other complex validation is not
supported. Thus this more advanced use case allows such validations before dispatching to a
handler.

## Supported Functionality
`IDiagnosticReporter` interface is at the core of the UX. It is similar in many ways to many
of the logging interfaces available. The primary distinction is with the ***intention*** of
use. `IDiagnosticReporter` specifically assumes the use for UI/UX rather than a
debugging/diagnostic log. These have VERY distinct use cases and purposes and generally show
very different information. (Not to mention the overly complex requirements of
the anti-pattern DI container assumed in `Microsoft.Extensions.Logging`)

### Messages
All messages for the UX use a simple immutable structure to store the details of a message
represented as `DiagnosticMessage`.

### Pre-Built Reporters
There are a few pre-built implementation of the `IDiagnosticReporter` interface.
* `TextWriterReporter`
    * Base class for writing UX to a `TextWriter`
* `ConsoleReporter`
    * Reporter that reports errors to `Console.Error` and all other messages to
      `Console.Out`
* `ColoredConsoleReporter`
    * `ConsoleReporter` that colorizes output using ANSI color codes
        * Colors are customizable, but contains a common default


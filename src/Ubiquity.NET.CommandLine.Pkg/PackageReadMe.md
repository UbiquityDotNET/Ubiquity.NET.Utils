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

### Root command type
Options.cs:
```
using System.IO;

using Ubiquity.NET.CommandLine;

namespace TestSample
{
    // Root command for the app, Disable the default options and directive support
    // This puts the app in total control of the parsing. Normally an app would leave the
    // defaults but it might not want to for command line syntax compat with a previous
    // release...
    [RootCommand(DefaultOptions = DefaultOption.None, DefaultDirectives = DefaultDirective.None )]
    internal partial class ParsedArgs
    {
        [Option("-v", Description="Verbosity Level")]
        internal MsgLevel Verbosity? { get; init; }

        // Additional options here...
    }
}
```

### Usage in common single command/no command case
Most applications don't have or use any concept of commands/sub-commands. Thus, there is
support to make this scenario as simple as possible. [This uses the previously listed
`ParsedArgs` class.

``` C#
public static async Task<int> Main( string[] args )
{
    // pressing CTRL-C cancels the entire operation
    using CancellationTokenSource cts = new();
    Console.CancelKeyPress += ( _, e ) =>
    {
        e.Cancel = true;
        cts.Cancel();
    };

    // start with information level for parsing; parsed options might specify a different
    // level `AppMain` will adapt to any new value.
    var reporter = new ColoredConsoleReporter( MsgLevel.Information );

    return await TestOptions.BuildRootCommand( ( options, ct ) => AppMainAsync( options, reporter, ct ) )
                            .ParseAndInvokeResultAsync( reporter, cts.Token, args );
}

private static int AppMain( ParsedArgs args, ColoredConsoleReporter reporter )
{
    // Real main here.
    // The pre-parsed and validated command line arguments and the reporter are provided as
    // arguments to this function

    // Now that args are parsed, if a distinct verbosity level is specified use that
    if(args.Verbosity != reporter.Level)
    {
        reporter = new ColoredConsoleReporter( args.Verbosity );
    }

    // ...
}
```
This is a simplified usage for the common case of an app without commands/sub-commands.

## Supported Functionality
`IDiagnosticReporter` interface is at the core of the UX. It is similar in many ways to many
of the logging interfaces available. The primary distinction is with the ***intention*** of
use. `IDiagnosticReporter`, specifically, assumes the use for UI/UX rather than a
debugging/diagnostic log. These have VERY distinct use cases and purposes and generally show
very different information. (Not to mention the overly complex requirements of
the anti-pattern DI container/Service Locator assumed in `Microsoft.Extensions.Logging`)



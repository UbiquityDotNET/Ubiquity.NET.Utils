# Ubiquity.NET.CommandLine.SrcGen
This is the Roslyn Source generator for Ubiquity.NET.CommandLine.

## Attributes Controlling generation
| Name             | Description |
|------------------|-------------|
| RootCommandAttribute | Applied to a class to generate bindings for the root command |
| CommandAttribute | Applied to a sub-command class to indicate support for a sub-command |
| FileValidationAttribute | Applied `System.IO.FileInfo` properties to validate the input |
| FolderValidationAttribute | Applied `System.IO.DirectoryInfo` properties to validate the input |
| OptionAttribute | Applied to properties of a class marked with either 'CommandAttribute' or `RootCommandAttribute`]

### Example

CommandLineOptions.cs
``` C#
using System.IO;
using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

[RootCommand( Description = "Root command for tests" )]
internal partial class TestOptions
{
    [Option( "-o", Description = "Test SomePath" )]
    [FolderValidation( FolderValidation.CreateIfNotExist )]
    public required DirectoryInfo SomePath { get; init; }

    [Option( "-v", Description = "Verbosity Level" )]
    public MsgLevel Verbosity { get; init; } = MsgLevel.Information;

    [Option( "-b", Description = "Test Some existing Path" )]
    [FolderValidation( FolderValidation.ExistingOnly )]
    public required DirectoryInfo SomeExistingPath { get; init; }

    [Option( "--thing1", Aliases = [ "-t" ], Required = true, Description = "Test Thing1", HelpName = "Help name for thing1" )]
    public bool Thing1 { get; init; }

    // This should be ignored by generator
    public string? NotAnOption { get; set; }

    [Option( "-a", Hidden = true, Required = false, ArityMin = 0, ArityMax = 3, Description = "Test SomeOtherPath" )]
    [FileValidation( FileValidation.ExistingOnly )]
    public required FileInfo SomeOtherPath { get; init; }
}
```

Program.cs
``` C#
using System;
using System.Threading;
using System.Threading.Tasks;

using Ubiquity.NET.CommandLine;

namespace TestNamespace
{
    public static class Program
    {
        public static async Task<int> Main( string[] args )
        {
            // pressing CTRL-C cancels the entire operation
            using CancellationTokenSource cts = new();
            Console.CancelKeyPress += ( _, e ) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            // start with information level for parsing; parsed options might specify different level
            var reporter = new ColoredConsoleReporter( MsgLevel.Information );

            return await TestOptions.BuildRootCommand( ( options, ct ) => AppMainAsync( options, reporter, ct ) )
                                    .ParseAndInvokeResultAsync( reporter, cts.Token, args );
        }

        private static async Task<int> AppMainAsync(
                TestOptions options,
                ColoredConsoleReporter reporter,
                CancellationToken ct
            )
        {
            // Now that args are parsed, if a distinct verbosity level is specified use that
            if(options.Verbosity != reporter.Level)
            {
                reporter = new ColoredConsoleReporter( options.Verbosity );
            }

            reporter.Verbose( "AppMainAsync" );

            // Core application code here...

            // Use the cancellation token to indicate cancellation
            // This is set when CTRL-C is pressed in Main() above.
            ct.ThrowIfCancellationRequested();
            return 0;
        }
    }
}
```

---
> [!NOTE]
> `CommandAttribute`, and sub-commands in general, is not ***currently*** supported.


using System.IO;
using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

[RootCommand( Description = "Root command for tests" )]
internal partial class TestOptions
{
    // Warning : UNC005 : Property 'Thing1' has type of 'bool' which does not support an arity of (3, 5).
    [Option( "--thing1", Aliases = [ "-t" ], ArityMin = 3, ArityMax = 5, Description = "Test Thing1", HelpName = "Help name for thing1" )]
    public bool Thing1 { get; init; }

    // No diagnostic on this
    [Option( "--thing2", Aliases = [ "-t" ], Description = "Test Thing2", HelpName = "Help name for thing2" )]
    public bool Thing2 { get; init; }

    // No diagnostic on this
    [Option("-i", ArityMin = 0, Description = "include path")]
    public required DirectoryInfo[] IncludePath { get; init; }
}

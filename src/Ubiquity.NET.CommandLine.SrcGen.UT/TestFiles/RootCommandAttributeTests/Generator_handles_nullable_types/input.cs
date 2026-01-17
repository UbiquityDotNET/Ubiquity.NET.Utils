using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

[RootCommand( Description = "Root command for tests" )]
internal partial class TestOptions
{
    [Option( "--thing1", Aliases = [ "-t" ], Description = "Test Thing1", HelpName = "Help name for thing1" )]
    public bool? Thing1 { get; init; }
}

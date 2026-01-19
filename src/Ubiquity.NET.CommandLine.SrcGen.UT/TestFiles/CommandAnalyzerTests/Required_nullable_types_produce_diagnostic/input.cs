using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

[RootCommand( Description = "Root command for tests" )]
internal partial class TestOptions
{
    // Warning : UNC004 : Type 'bool?' for property 'Thing1' is nullable but marked as required; These annotations conflict resulting in behavior that is explicitly UNDEFINED.
    [Option( "--thing1", Aliases = [ "-t" ], Required = true, Description = "Test Thing1", HelpName = "Help name for thing1" )]
    public bool? Thing1 { get; init; }

    [Option( "--thing2", Aliases = [ "-t" ], Description = "Test Thing2", HelpName = "Help name for thing2" )]
    public bool Thing2 { get; init; }
}

using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

[RootCommand( Description = "Root command for tests" )]
internal class testInput1
{
    [Option( "-o", Description = "Test SomePath" )]
    [FileValidation( FileValidation.ExistingOnly )] // UNC003 - Property attribute 'FileValidation' requires a property of type 'FileInfo'.
    public required string SomePath { get; init; }
}

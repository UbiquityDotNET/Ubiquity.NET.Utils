using System.IO;

using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

internal class testInput1
{
    [Option( "-o", Description = "Test SomePath" )]
    [FileValidation( FileValidation.ExistingOnly )] // UNC002 : Property attribute not allowed standalone
    public required FileInfo SomePath { get; init; }
}

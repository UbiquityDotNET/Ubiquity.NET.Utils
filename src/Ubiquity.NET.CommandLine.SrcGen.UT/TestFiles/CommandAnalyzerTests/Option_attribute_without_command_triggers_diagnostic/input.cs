using System.IO;

using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

internal class testInput1
{
    [Option( "-o", Description = "Use of this attribute should generate diagnostic UNC001" )]
    public required DirectoryInfo SomePath { get; init; }
}

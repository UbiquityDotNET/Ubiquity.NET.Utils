using System.IO;

using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

internal class testInput1
{
    // This attribute alone should trigger UNC002 - Property attribute FileValidation is not allowed on a property independent of a qualifying attribute such as OptionAttribute.
    [FileValidation( FileValidation.ExistingOnly )]
    public required FileInfo SomePath { get; init; }
}

using System.IO;

using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace;

internal class testInput1
{
    // UNC001 - Property attribute 'FolderValidation' is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
    // UNC002 - Property attribute 'FolderValidationAttribute' is not allowed on a property independent of a qualifying attribute such as OptionAttribute.
    [FolderValidation( FolderValidation.ExistingOnly )]
    public required DirectoryInfo SomePath { get; init; }
}

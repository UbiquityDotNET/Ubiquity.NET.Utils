# About
Ubiquity.NET.CommandLine contains general extensions for .NET. to support command line
parsing using `System.CommandLine`.

A source generator is included that will generate the boilerplate code for command line
parsing and binding. Additionally an analyzer is provided to aid in identifying problems
with usage of the attributes for generation.

## Analyzer Diagnostics
Rule ID | Title |
--------|-------|
[UNC000](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC000.html) | An internal analyzer exception occurred. |
[UNC001](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC001.html) | Missing command attribute on containing type. |
[UNC002](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC002.html) | Property attribute not allowed standalone. |
[UNC003](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC003.html) | Property has incorrect type for attribute. |
[UNC004](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC004.html) | Property type is nullable but marked as required. |
[UNC005](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/UNC004.html) | Arity specified for property type is invalid. |

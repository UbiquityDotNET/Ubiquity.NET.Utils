# Ubiquity.NET.Utils
Utility packages To support a variety of scenarios. This repository is mostly a [Grab Bag](https://www.wordnik.com/words/grab%20bag)
of multiple small libraries that didn't warrant a distinct repository.

| Library | Description |
|---------|-------------|
| [Ubiquity.NET.Antlr.Utils](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/antlr-utils/index.html) | This library contains extensions and helpers for using ANTLR with .NET |
| [Ubiquity.NET.CodeAnalysis.Utils](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CodeAnalysis/index.html) | This library contains extensions and helpers for using Roslyn CodeAnalysis |
| [Ubiquity.NET.CommandLine](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/index.html) | This library contains extensions and helpers for command line parsing via `System.CommandLine` |
| [Ubiquity.NET.Extensions](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/extensions/index.html) | This library contains general extensions and helpers for many scenarios using .NET |
| [Ubiquity.NET.InteropHelpers](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/interop-helpers/index.html) | This library contains extensions and helpers for implementing interop support for native libraries |
| [Ubiquity.NET.Runtime.Utils](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/runtime-utils/index.html) | This library contains common support for DSL runtime and language implementers |
| [Ubiquity.NET.SourceGenerator.Test.Utils](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/src-gen-test-utils/index.html) | This library contains extensions and helpers for testing source generators |
| [Ubiquity.NET.SrcGeneration](https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/SrcGeneration/index.html) | This library contains extensions and helpers for implementing source generators |

>[!IMPORTANT]
> When editing code in this repository make certain that any extensions or tooling that
> ***automatically*** removes trailing whitespace is disabled. It is fine to highlight such
> cases and most of the time remove any. However, there are some tests where a trailing
> whitespace is required and a critical part of the tests.

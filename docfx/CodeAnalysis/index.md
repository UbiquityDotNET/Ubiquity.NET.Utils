# About
The Ubiquity.NET.CodeAnalysis.Utils package contains support for building Roslyn analyzers,
fixers, and source generator. As such it specifically targets .NET Standard 2.0 and has no
dependencies on anything targeting a later runtime. If an analyzer (or any Roslyn extension)
depends on this package the dependencies for the package must be included in the extension's
package.

## Key support

* Support for caching of generation scan results (via `IEquatable<T>`)
* Debug diagnostic asserts for class vs. struct trade-offs 
* Capturing symbol information in a cacheable fashion
* Generation of diagnostics for issues detected in a generator<a href="#footnote_1"><sup>1</sup></a>
* Create a `SourceText` from a `StringBuilder` to allow generation to remain independent of
  the Roslyn CodeAnalysis types.

------
<sup id="footnote_1">1</sup> Generators creating diagnostics is generally not recommended.
The official
[Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.cookbook.md)
cook book recommends against it. A generator should normally ignore invalid input and fail
silently by ignoring the problem. An analyzer can produce the diagnostics for problems. At
most a generator can use this support to report critical problems that prevent the generator
from running at all.

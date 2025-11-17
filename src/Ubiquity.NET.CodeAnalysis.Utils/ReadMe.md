# About
The Ubiquity.NET.CodeAnalysis.Utils contains support for building Roslyn analyzers, fixers,
and source generator. As such it specifically targets .NET Standard 2.0 and has no
dependencies on anything targeting a later runtime. If an analyzer (or any Roslyn extension)
depends on this package the dependencies for the package must be included in the extension's
package.

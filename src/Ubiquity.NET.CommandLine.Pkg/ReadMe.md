# Ubiquity.NET.CommandLine.Pkg
This is a special project that creates the meta package to reference the library AND the
associated Roslyn components. This is done without restoring any packages by a "NOTARGETS"
project that uses an explicit NUSPEC. This is used instead of the CSPROJ built-in support
for generation of the NuSpec as that would try and restore ALL references first. This causes
problems for build ordering. Since a CSPROJ will try to restore all packages it will fail to
restore anything that isn't built yet. While it is plausible to construct build order
dependencies, such things are not honored when restoring. NuGet package restore happens
***BEFORE*** build so the packages won't exist.

That limitation is strictly in how CSPROJ system works and not an actual limit of NuGet
itself. Thus, to workaround that, this project uses a NuSpec file and generates the meta
package directly (even if the dependencies are not resolved). The package is created
directly referencing what will exist in the future breaking the dependency cycle/problem.

## Build Caveat
Due to the nature of this, the version reference must remain constant across all project
builds. This is true for command line builds but NOT for IDE builds. Each project in an IDE
build gets a new version. This means that the version references in this package are wrong
and won't exist. The solution is to build this and all packages from the command line to
guarantee correct references. This is done automatically in all automated builds as they
don't use any IDE. Thus, the problem is for local loop development only. It is only an issue
when using the demo project as that is designed to leverage ONLY the package without any
project references to validate that works as expected.

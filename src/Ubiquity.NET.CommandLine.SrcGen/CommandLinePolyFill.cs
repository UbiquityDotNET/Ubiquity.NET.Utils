// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.SrcGen
{
    // Roslyn generators **MUST** target .NET standard 2.0; However the CommandLine assembly
    // only supports .NET 8.0 and .NET 10. So this fills in the gaps on enumerations as the
    // library with the declarations can't be referenced directly.

    /// <summary>Flags to determine the default Options for an <c>AppControlledDefaultsRootCommand</c></summary>
    [Flags]
    internal enum DefaultOption
    {
        /// <summary>No default options used</summary>
        None = 0,

        /// <summary>Include the default help option</summary>
        Help,

        /// <summary>Include the default version option</summary>
        Version,
    }

    /// <summary>Flags to determine the default directives supported for an <c>AppControlledDefaultsRootCommand</c></summary>
    [Flags]
    internal enum DefaultDirective
    {
        /// <summary>No default directives included</summary>
        None = 0,

        /// <summary>Include support for <c>SuggestDirective</c></summary>
        Suggest,

        /// <summary>Include support for <c>DiagramDirective</c></summary>
        Diagram,

        /// <summary>Include support for <c>EnvironmentVariablesDirective</c></summary>
        EnvironmentVariables,
    }

    /// <summary>Enumeration for folder validation</summary>
    internal enum FileValidation
    {
        /// <summary>No validation</summary>
        None,

        /// <summary>Existing files only accepted.</summary>
        /// <remarks>
        /// If a file specified does not exist then an exception results from the validation stage of
        /// processing the command line.
        /// </remarks>
        ExistingOnly,
    }

    /// <summary>Enumeration for folder validation</summary>
    internal enum FolderValidation
    {
        /// <summary>No validation</summary>
        None,

        /// <summary>Creates the folder if it doesn't exist</summary>
        CreateIfNotExist,

        /// <summary>Existing folders only accepted.</summary>
        /// <remarks>
        /// If a folder specified does not exist then an exception results from the validation stage of
        /// processing the command line.
        /// </remarks>
        ExistingOnly,
    }
}

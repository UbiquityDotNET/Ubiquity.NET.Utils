// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

#pragma warning disable IDE0130 // Namespace does not match folder structure

using System.CommandLine;
using System.IO;

using Ubiquity.NET.CommandLine;
using Ubiquity.NET.CommandLine.GeneratorAttributes;

namespace TestNamespace
{
    [RootCommand( Description = "Root command for tests" )]
    internal partial class TestOptions
    {
        [Option( "-o", Description = "Test SomePath" )]
        [FolderValidation( FolderValidation.CreateIfNotExist )]
        public required DirectoryInfo SomePath { get; init; }

        [Option( "-v", Description = "Verbosity Level" )]
        public MsgLevel Verbosity { get; init; } = MsgLevel.Information;

        [Option( "-b", Description = "Test Some existing Path" )]
        [FolderValidation( FolderValidation.ExistingOnly )]
        public required DirectoryInfo SomeExistingPath { get; init; }

        [Option( "--thing1", Aliases = [ "-t" ], Required = true, Description = "Test Thing1", HelpName = "Help name for thing1" )]
        public bool Thing1 { get; init; }

        // This should be ignored by generator
        public string? NotAnOption { get; set; }

        [Option( "-a", Hidden = true, Required = false, ArityMin = 0, ArityMax = 3, Description = "Test SomeOtherPath" )]
        [FileValidation( FileValidation.ExistingOnly )]
        public required FileInfo SomeOtherPath { get; init; }
    }
}

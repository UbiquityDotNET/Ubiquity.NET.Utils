// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using Ubiquity.NET.Extensions;

namespace Ubiquity.NET.CommandLine.SrcGen.Templates
{
    internal static class ToolInfo
    {
        public static string Name => ProcessInfo.ExecutableName;

        public static string Version => ProcessInfo.ActiveAssembly?.GetInformationalVersion() ?? "<Unkown version>";
    }
}

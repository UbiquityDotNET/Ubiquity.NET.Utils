// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.GeneratorAttributes
{
    /// <summary>Attribute to mark a command for generation of the backing implementation</summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = false, AllowMultiple = false )]
    public sealed class CommandAttribute
        : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="CommandAttribute"/> class.</summary>
        /// <param name="description">Description for the command (Default: string.Empty)</param>
        public CommandAttribute( string? description = null )
        {
            Description = description ?? string.Empty;
        }

        /// <summary>Gets the Description for this command</summary>
        public string Description { get; }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.GeneratorAttributes
{
    /// <summary>Enumeration for folder validation</summary>
    public enum FolderValidation
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

    // Analyzer checks: Only used on a type that is DirectoryInfo
    //                  Only used on a type that has an OptionAttribute as well.
    //
    // Generator ignores this attribute if all analyzer checks are not valid

    /// <summary>Attribute to declare common validation for a property of type <see cref="DirectoryInfo"/></summary>
    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
    public sealed class FolderValidationAttribute
        : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="FolderValidationAttribute"/> class.</summary>
        /// <param name="validation">Validation to perform for this folder</param>
        /// <remarks>
        /// A <paramref name="validation"/> that is <see cref="FolderValidation.None"/> is the same
        /// as not specifying this attribute.
        /// </remarks>
        public FolderValidationAttribute( FolderValidation validation )
        {
            Validation = validation;
        }

        /// <summary>Gets the <see cref="FolderValidation"/> to use for this <see cref="DirectoryInfo"/></summary>
        public FolderValidation Validation { get; }
    }
}

// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.GeneratorAttributes
{
    /// <summary>Enumeration for folder validation</summary>
    public enum FileValidation
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

    // Analyzer checks: Only used on a type that is FileInfo
    //                  Only used on a type that has an OptionAttribute as well.
    //
    // Generator ignores this attribute if all analyzer checks are not valid

    /// <summary>Attribute to declare common validation for a property of type <see cref="DirectoryInfo"/></summary>
    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
    public sealed class FileValidationAttribute
        : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="FileValidationAttribute"/> class.</summary>
        /// <param name="validation">Validation to perform for this folder</param>
        /// <remarks>
        /// A <paramref name="validation"/> that is <see cref="FileValidation.None"/> is the same
        /// as not specifying this attribute.
        /// </remarks>
        public FileValidationAttribute( FileValidation validation )
        {
            Validation = validation;
        }

        /// <summary>Gets the <see cref="FileValidation"/> to use for this <see cref="DirectoryInfo"/></summary>
        public FileValidation Validation { get; }
    }
}

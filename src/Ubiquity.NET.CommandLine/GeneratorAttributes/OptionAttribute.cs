// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

namespace Ubiquity.NET.CommandLine.GeneratorAttributes
{
    /// <summary>Attribute to apply to properties of a command that indicates generation of implementation</summary>
    /// <seealso cref="Option"/>
    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false )]
    public sealed class OptionAttribute
        : Attribute
    {
        /// <summary>Initializes a new instance of the <see cref="OptionAttribute"/> class.</summary>
        /// <param name="name">Name of the option</param>
        public OptionAttribute( string name )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace( name );
            Name = name;
        }

        /// <summary>Gets the name of the option (Including any "prefix" for the switch character)</summary>
        public string Name { get; }

        /// <summary>Gets the help name for this option</summary>
        /// <seealso cref="Option.HelpName"/>
        public string? HelpName { get; init; }

        /*
        ANALYZER VALIDATION:
            var bldr = ImmutableArray.CreateBuilder<int>(value.Length);
            for(int i = 0; i < value.Length; ++i)
            {
                if(string.IsNullOrWhiteSpace(value[i]))
                {
                    bldr.Add(i);
                }
            }

            if(bldr.Count > 0)
            {
                var badIndexArray = bldr.ToImmutable();
                throw new ArgumentException($"Aliases cannot be null or whitespace. Invalid indexes: {badIndexArray.Format()}", nameof(value));
            }
        */

        /// <summary>Gets the aliases for this option</summary>
        public string[] Aliases { get; init; } = [];

        /// <summary>Gets the description for this option</summary>
        public string? Description { get; init; }

        /// <summary>Gets a value indicating whether this option is required for execution of the command</summary>
        /// <remarks>
        /// Options that are required and not provided by the user AND don't have a default are reported as an error.
        /// That is, the "required" concept applies to the execution of the command and ***not*** the parsing of data.
        /// </remarks>
        public bool Required { get; init; }

        /// <summary>Gets a value indicating whether this option is hidden in help information</summary>
        /// <remarks>
        /// This is often used for internal debugging/diagnostics and potentially preview functionality.
        /// </remarks>
        /// <seealso cref="Symbol.Hidden"/>
        public bool Hidden { get; init; }

        // ANALYZER VERIFY: ArityMin > = 0
        //                  ArityMin < = ArgumentArity.MaximumArity [Sadly, an internal const]
        //                  BOTH values are set or NONE is set.
        // GENERATOR: ignore if not valid

        /// <summary>Gets the minimum arity</summary>
        /// <remarks>
        /// Instances of <see cref="ArgumentArity"/> are not allowed as const args for an attribute.
        /// Thus, this pair of properties represents the arity of an option. If not specified the default depends
        /// on the type as defined by the underlying command line system.
        /// </remarks>
        /// <seealso cref="Option"/>
        /// <seealso cref="ArgumentArity"/>
        public int ArityMin { get; init; }

        /// <summary>Gets the maximum arity</summary>
        /// <inheritdoc cref="ArityMin"/>
        public int ArityMax { get; init; }

        // ANALYZER check: name refers to a Func<ArgumentResult, T>
        //                 where T is the type of the property
        // GENERATOR: Ignore if analyzer validity checks fail

        /// <summary>Gets the default value factory for the option</summary>
        /// <remarks>If no value is provided via parsing the command line, this is called to produce the default</remarks>
        public string? DefaultValueFactoryName { get; init; }

        // ANALYZER check: name refers to a Func<ArgumentResult, T>
        //                 where T is the type of the property
        // GENERATOR: Ignore if analyzer validity checks fail

        /// <summary>Gets a Custom parser for the option</summary>
        /// <remarks>If provided, this parser is called to convert the argument to a value</remarks>
        public string? CustomParserName { get; init; }
    }
}

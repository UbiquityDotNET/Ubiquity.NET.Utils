// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    /// <summary>Wrapper for <see cref="EquatableAttributeData"/> for an Option Attribute</summary>
    /// <remarks>
    /// This is a simple wrapper around a managed reference. The size of this struct is no different
    /// than the managed reference it wraps as that is the only field. This type supplies a validating
    /// constructor and accessor to simplify access to the captured data.
    /// </remarks>
    internal readonly record struct OptionInfo
    {
        public OptionInfo( EquatableAttributeData attributeInfo )
        {
            // Runtime sanity check in debug mode
            DebugAssert.StructSizeOK<OptionInfo>();

            if( attributeInfo.Name != Constants.OptionAttribute)
            {
                throw new ArgumentException("Not an option attribute!");
            }

            AttributeInfo = attributeInfo;
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Name);

        public string Name => AttributeInfo.ConstructorArguments.Length < 1
                              ? string.Empty
                              : AttributeInfo.ConstructorArguments[0].Value as string ?? string.Empty;

        public Optional<string> HelpName => AttributeInfo.GetNamedArgValue<string>( Constants.OptionAttributeNamedArgs.HelpName );

        public Optional<ImmutableArray<string>> Aliases => AttributeInfo.GetNamedArgValueArray<string>( Constants.OptionAttributeNamedArgs.Aliases );

        public Optional<string> Description => AttributeInfo.GetNamedArgValue<string>( Constants.OptionAttributeNamedArgs.Description );

        public Optional<bool> Required => AttributeInfo.GetNamedArgValue<bool>( Constants.OptionAttributeNamedArgs.Required );

        public Optional<bool> Hidden => AttributeInfo.GetNamedArgValue<bool>( Constants.OptionAttributeNamedArgs.Hidden );

        public Optional<(int Min, int Max)> Arity
        {
            get
            {
                // ignore argument if both aren't available
                Optional<int> min = AttributeInfo.GetNamedArgValue<int>( Constants.OptionAttributeNamedArgs.ArityMin);
                Optional<int> max = AttributeInfo.GetNamedArgValue<int>( Constants.OptionAttributeNamedArgs.ArityMax);

                return min.HasValue && max.HasValue
                     ? new((min.Value, max.Value))
                     : default;
            }
        }

        private readonly EquatableAttributeData AttributeInfo;

        public static implicit operator OptionInfo(EquatableAttributeData attributeInfo)
        {
            return new(attributeInfo);
        }
    }
}

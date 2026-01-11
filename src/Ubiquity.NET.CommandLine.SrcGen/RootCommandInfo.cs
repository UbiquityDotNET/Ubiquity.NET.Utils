// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections;
using System.Collections.Immutable;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    internal readonly struct RootCommandInfo
        : IEquatable<RootCommandInfo>
    {
        public RootCommandInfo(
            NamespaceQualifiedName targetName,
            EquatableAttributeData attributeInfo,
            ImmutableArray<PropertyInfo> properties
        )
        {
            TargetName = targetName ?? throw new ArgumentNullException( nameof( targetName ) );
            AttributeInfo = attributeInfo;
            Properties = properties;
        }

        public NamespaceQualifiedName TargetName { get; }

        public EquatableAttributeData AttributeInfo { get; }

        public ImmutableArray<PropertyInfo> Properties { get; }

        // This is used to simplify the code generation to limit
        // to the default settings if not explicitly overridden.
        public bool HasSettings
        {
            get
            {
                // If the attribute has any named args, beside "Description" it's a setting
                // so as soon as one is found report settings are present.
                foreach(string argName in AttributeInfo.NamedArguments.Keys)
                {
                    if(argName != Constants.RootCommandAttributeNamedArgs.Description)
                    {
                        // Currently, all non-description args are settings
                        return true;
                    }
                }

                // no named args or none that are not the description
                return false;
            }
        }

        public Optional<string> Description => AttributeInfo.GetNamedArgValue<string>( Constants.RootCommandAttributeNamedArgs.Description );

        public Optional<bool> ShowHelpOnErrors => AttributeInfo.GetNamedArgValue<bool>( Constants.RootCommandAttributeNamedArgs.ShowHelpOnErrors );

        public Optional<bool> ShowTypoCorrections => AttributeInfo.GetNamedArgValue<bool>( Constants.RootCommandAttributeNamedArgs.ShowTypoCorrections );

        public Optional<bool> EnablePosixBundling => AttributeInfo.GetNamedArgValue<bool>( Constants.RootCommandAttributeNamedArgs.EnablePosixBundling );

        public Optional<DefaultOption> DefaultOptions => AttributeInfo.GetNamedArgValue<DefaultOption>( Constants.RootCommandAttributeNamedArgs.DefaultOptions );

        public Optional<DefaultDirective> DefaultDirectives => AttributeInfo.GetNamedArgValue<DefaultDirective>( Constants.RootCommandAttributeNamedArgs.DefaultDirectives );

        public bool Equals( RootCommandInfo other )
        {
            bool retVal = TargetName.Equals(other.TargetName)
                && AttributeInfo.Equals(other.AttributeInfo)
                && StructuralComparisons.StructuralEqualityComparer.Equals(Properties, other.Properties);

            return retVal;
        }

        public override bool Equals( object obj )
        {
            return obj is RootCommandInfo other
                && Equals( other );
        }

        public override int GetHashCode( )
        {
            return HashCode.Combine(TargetName, AttributeInfo, Properties);
        }
    }
}

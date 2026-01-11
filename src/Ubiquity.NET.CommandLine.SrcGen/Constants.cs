// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    // Implementation Note:
    //   use of pattern `string X = nameof(X)` allows for refactoring the name without the problem
    //   of repetitive typo errors.
    //
    // These name values are needed as the target DLL is NOT referenceable by this generator
    // and therefore does not have anyway to resolve the names.

    internal static class Constants
    {
        internal static readonly IEnumerable<string> SystemIONamespaceParts = [ "System", "IO" ];

        internal static readonly IEnumerable<string> SystemCommandLineNamespaceParts = [ "System", "CommandLine" ];

        internal static readonly IEnumerable<string> UbiquityNETCommandLineNamespaceParts = [ "Ubiquity", "NET", "CommandLine" ];

        internal static readonly IEnumerable<string> GeneratorAttributesNamespaceParts = UbiquityNETCommandLineNamespaceParts.Append("GeneratorAttributes");

        internal static string UbiquityNETCommandLineNamespaceName { get; } = string.Join(".", UbiquityNETCommandLineNamespaceParts);

        internal static readonly NamespaceQualifiedName RootCommandAttribute
            = new( GeneratorAttributesNamespaceParts, nameof(RootCommandAttribute));

        internal static readonly NamespaceQualifiedName OptionAttribute
            = new( GeneratorAttributesNamespaceParts, nameof(OptionAttribute));

        internal static readonly NamespaceQualifiedName FileValidationAttribute
            = new( GeneratorAttributesNamespaceParts, nameof(FileValidationAttribute));

        internal static readonly NamespaceQualifiedName FolderValidationAttribute
            = new( GeneratorAttributesNamespaceParts, nameof(FolderValidationAttribute));

        internal static readonly NamespaceQualifiedName IRootCommandBuilderWithSettings
            = new( UbiquityNETCommandLineNamespaceParts, nameof(IRootCommandBuilderWithSettings));

        [SuppressMessage( "StyleCop.CSharp.NamingRules", "SA1310:Field names should not contain underscore", Justification = "Generic type - no other way to represent name in C# literal" )]
        internal static readonly NamespaceQualifiedName ICommandBinder
            = new( UbiquityNETCommandLineNamespaceParts, nameof(ICommandBinder)); // Actual name is ICommandBinder`1, but that is not needed for code gen

        internal static readonly NamespaceQualifiedName AppControlledDefaultsRootCommand
            = new( UbiquityNETCommandLineNamespaceParts, nameof(AppControlledDefaultsRootCommand));

        internal static readonly NamespaceQualifiedName SymbolValidationExtensions
            = new( UbiquityNETCommandLineNamespaceParts, nameof(SymbolValidationExtensions));

        internal static readonly NamespaceQualifiedName CommandLineSettings
            = new( UbiquityNETCommandLineNamespaceParts, nameof(CommandLineSettings));

        internal static readonly NamespaceQualifiedName ArgumentArity
            = new( SystemCommandLineNamespaceParts, nameof(ArgumentArity));

        internal static readonly NamespaceQualifiedName Option
            = new( SystemCommandLineNamespaceParts, nameof(Option));

        internal static readonly NamespaceQualifiedName ParseResult
            = new( SystemCommandLineNamespaceParts, nameof(ParseResult));

        internal static readonly NamespaceQualifiedName DirectoryInfo
            = new( SystemIONamespaceParts, nameof(DirectoryInfo));

        internal static readonly NamespaceQualifiedName FileInfo
            = new( SystemIONamespaceParts, nameof(FileInfo));

        // Names for the attributes to allow quickly filtering to a matching
        // simple name before testing if it's full name is a match
        internal static readonly ImmutableArray<NamespaceQualifiedName> GeneratingAttributeNames
        = [
            RootCommandAttribute,
            OptionAttribute,
            FolderValidationAttribute,
            FileValidationAttribute,
          ];

        internal static class CommonAttributeNamedArgs
        {
            internal const string Required = nameof(Required);
        }

        internal static class RootCommandAttributeNamedArgs
        {
            internal const string Description = nameof(Description);
            internal const string ShowHelpOnErrors = nameof(ShowHelpOnErrors);
            internal const string ShowTypoCorrections = nameof(ShowTypoCorrections);
            internal const string EnablePosixBundling = nameof(EnablePosixBundling);
            internal const string DefaultOptions = nameof(DefaultOptions);
            internal const string DefaultDirectives = nameof(DefaultDirectives);
        }

        internal static class OptionAttributeNamedArgs
        {
            internal const string HelpName = nameof(HelpName);
            internal const string Aliases = nameof(Aliases);
            internal const string Description = nameof(Description);
            internal const string Required = CommonAttributeNamedArgs.Required;
            internal const string Hidden = nameof(Hidden);
            internal const string ArityMin = nameof(ArityMin);
            internal const string ArityMax = nameof(ArityMax);
            internal const string DefaultValueFactoryName = nameof(DefaultValueFactoryName);
            internal const string CustomParserName = nameof(CustomParserName);
        }
    }
}

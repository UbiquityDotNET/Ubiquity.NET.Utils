// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;

using Ubiquity.NET.CommandLine.SrcGen.Properties;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    internal class Diagnostics
    {
        internal static class IDs
        {
            internal const string InternalErrorDescriptor = "UNC000";
            internal const string MissingCommandAttributeDescriptor = "UNC001";
            internal const string MissingConstraintAttributeDescriptor = "UNC002";
            internal const string IncorrectPropertyTypeDescriptor = "UNC003";
            internal const string RequiredNullableTypeDescriptor = "UNC004";
            internal const string PropertyTypeArityMismatchDescriptor = "UNC005";
        }

        internal static ImmutableArray<DiagnosticDescriptor> CommandLineAnalyzerDiagnostics
            => [
                InternalErrorDescriptor,
                MissingCommandAttributeDescriptor,
                MissingConstraintAttributeDescriptor,
                IncorrectPropertyTypeDescriptor,
                RequiredNullableTypeDescriptor,
                PropertyTypeArityMismatchDescriptor,
            ];

        internal static DiagnosticInfo InternalErrorInfo(Location? loc, Exception ex)
        {
            return new(InternalErrorDescriptor, loc, ex );
        }

        internal static Diagnostic InternalError( Location? loc, Exception ex )
        {
            return Diagnostic.Create( InternalErrorDescriptor, loc, ex.Message );
        }

        internal static Diagnostic MissingCommandAttribute( Location? loc, string attributeName )
        {
            return Diagnostic.Create( MissingCommandAttributeDescriptor, loc, attributeName );
        }

        internal static Diagnostic MissingConstraintAttribute( Location? loc, string attributeName )
        {
            return Diagnostic.Create( MissingConstraintAttributeDescriptor, loc, attributeName );
        }

        internal static Diagnostic IncorrectPropertyType(
            Location? loc,
            string attributeName,
            string expectedTypeName
            )
        {
            return Diagnostic.Create( IncorrectPropertyTypeDescriptor, loc, attributeName, expectedTypeName );
        }

        internal static Diagnostic RequiredNullableType(
            Location? loc,
            NamespaceQualifiedTypeName propertyType,
            string propertyName
            )
        {
            string propertyTypeName = propertyType.ToString( "A", null );
            return Diagnostic.Create( RequiredNullableTypeDescriptor, loc, propertyTypeName, propertyName );
        }

        internal static Diagnostic PropertyTypeArityMismatch(
            Location? loc,
            IPropertySymbol propertySymbol,
            int minArity,
            int maxArity
            )
        {
            string propertyTypeName = propertySymbol.Type.GetNamespaceQualifiedName().ToString("A", null);
            return Diagnostic.Create( PropertyTypeArityMismatchDescriptor, loc, propertySymbol.Name, propertyTypeName, minArity, maxArity );
        }

        // Exception message is: '{0}'
        private static readonly DiagnosticDescriptor InternalErrorDescriptor = new(
            id: IDs.InternalErrorDescriptor,
            title: Localized(nameof(Resources.InternalError_Title)),
            messageFormat: Localized(nameof(Resources.InternalError_MessageFormat)),
            category: "Internal",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.InternalError_Description)),
            helpLinkUri: FormatHelpUri(IDs.InternalErrorDescriptor),
            WellKnownDiagnosticTags.NotConfigurable
            );

        // Property attribute '{0}' is only allowed on a property in a type attributed with a command attribute. This use will be ignored by the generator.
        private static readonly DiagnosticDescriptor MissingCommandAttributeDescriptor = new(
            id: IDs.MissingCommandAttributeDescriptor,
            title: Localized(nameof(Resources.MissingCommandAttribute_Title)),
            messageFormat: Localized(nameof(Resources.MissingCommandAttribute_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.MissingCommandAttribute_Description)),
            helpLinkUri: FormatHelpUri(IDs.MissingCommandAttributeDescriptor),
            WellKnownDiagnosticTags.Unnecessary,
            WellKnownDiagnosticTags.Compiler
            );

        // Property attribute '{0}' is not allowed on a property independent of a qualifying attribute such as OptionAttribute.
        private static readonly DiagnosticDescriptor MissingConstraintAttributeDescriptor = new(
            id: IDs.MissingConstraintAttributeDescriptor,
            title: Localized(nameof(Resources.MissingConstraintAttribute_Title)),
            messageFormat: Localized(nameof(Resources.MissingConstraintAttribute_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.MissingConstraintAttribute_Description)),
            helpLinkUri: FormatHelpUri(IDs.MissingConstraintAttributeDescriptor),
            WellKnownDiagnosticTags.Compiler
            );

        // Property attribute '{0}' requires a property of type '{1}'.
        private static readonly DiagnosticDescriptor IncorrectPropertyTypeDescriptor = new(
            id: IDs.IncorrectPropertyTypeDescriptor,
            title: Localized(nameof(Resources.IncorrectPropertyType_Title)),
            messageFormat: Localized(nameof(Resources.IncorrectPropertyType_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.IncorrectPropertyType_Description)),
            helpLinkUri: FormatHelpUri(IDs.IncorrectPropertyTypeDescriptor),
            WellKnownDiagnosticTags.Compiler,
            WellKnownDiagnosticTags.NotConfigurable
            );

        // Type '{0}' for property '{1}' is nullable but marked as required; These annotations conflict resulting in behavior that is explicitly UNDEFINED.
        private static readonly DiagnosticDescriptor RequiredNullableTypeDescriptor = new(
            id: IDs.RequiredNullableTypeDescriptor,
            title: Localized(nameof(Resources.RequiredNullableType_Title)),
            messageFormat: Localized(nameof(Resources.RequiredNullableType_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.RequiredNullableType_Description)),
            helpLinkUri: FormatHelpUri(IDs.RequiredNullableTypeDescriptor),
            WellKnownDiagnosticTags.Compiler
            );

        // Property '{0}' has type of '{1}' which does not support an arity of ({2}, {3}).
        private static readonly DiagnosticDescriptor PropertyTypeArityMismatchDescriptor = new(
            id: IDs.PropertyTypeArityMismatchDescriptor,
            title: Localized(nameof(Resources.PropertyTypeArityMismatch_Title)),
            messageFormat: Localized(nameof(Resources.PropertyTypeArityMismatch_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.PropertyTypeArityMismatch_Description)),
            helpLinkUri: FormatHelpUri(IDs.PropertyTypeArityMismatchDescriptor),
            WellKnownDiagnosticTags.Compiler
            );

        private static string FormatHelpUri( string id )
        {
            return $"https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/{id}.html";
        }

        private static LocalizableResourceString Localized( string resName )
        {
            return new LocalizableResourceString( resName, Resources.ResourceManager, typeof( Resources ) );
        }
    }
}

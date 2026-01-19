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
            internal const string InternalError = "UNC000";
            internal const string MissingCommandAttribute = "UNC001";
            internal const string MissingConstraintAttribute = "UNC002";
            internal const string IncorrectPropertyType = "UNC003";
            internal const string RequiredNullableType = "UNC004";
        }

        private static LocalizableResourceString Localized( string resName )
        {
            return new LocalizableResourceString( resName, Resources.ResourceManager, typeof( Resources ) );
        }

        internal static ImmutableArray<DiagnosticDescriptor> CommandLineAnalyzerDiagnostics
            => [
                InternalError,
                MissingCommandAttribute,
                MissingConstraintAttribute,
                IncorrectPropertyType,
                RequiredNullableType,
            ];

        internal static readonly DiagnosticDescriptor InternalError = new(
            id: IDs.InternalError,
            title: Localized(nameof(Resources.InternalError_Title)),
            messageFormat: Localized(nameof(Resources.InternalError_MessageFormat)),
            category: "Internal",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.InternalError_Description)),
            helpLinkUri: FormatHelpUri(IDs.InternalError),
            WellKnownDiagnosticTags.NotConfigurable
            );

        internal static readonly DiagnosticDescriptor MissingCommandAttribute = new(
            id: IDs.MissingCommandAttribute,
            title: Localized(nameof(Resources.MissingCommandAttribute_Title)),
            messageFormat: Localized(nameof(Resources.MissingCommandAttribute_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.MissingCommandAttribute_Description)),
            helpLinkUri: FormatHelpUri(IDs.MissingCommandAttribute),
            WellKnownDiagnosticTags.Unnecessary,
            WellKnownDiagnosticTags.Compiler
            );

        internal static readonly DiagnosticDescriptor MissingConstraintAttribute = new(
            id: IDs.MissingConstraintAttribute,
            title: Localized(nameof(Resources.MissingConstraintAttribute_Title)),
            messageFormat: Localized(nameof(Resources.MissingConstraintAttribute_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.MissingConstraintAttribute_Description)),
            helpLinkUri: FormatHelpUri(IDs.MissingConstraintAttribute),
            WellKnownDiagnosticTags.Compiler
            );

        internal static readonly DiagnosticDescriptor IncorrectPropertyType = new(
            id: IDs.IncorrectPropertyType,
            title: Localized(nameof(Resources.IncorrectPropertyType_Title)),
            messageFormat: Localized(nameof(Resources.IncorrectPropertyType_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.IncorrectPropertyType_Description)),
            helpLinkUri: FormatHelpUri(IDs.IncorrectPropertyType),
            WellKnownDiagnosticTags.Compiler,
            WellKnownDiagnosticTags.NotConfigurable
            );

        // Type '{0}' for property '{1}' is nullable but marked as required; These annotations conflict resulting in behavior that is explicitly UNDEFINED.
        internal static readonly DiagnosticDescriptor RequiredNullableType = new(
            id: IDs.RequiredNullableType,
            title: Localized(nameof(Resources.RequiredNullableType_Title)),
            messageFormat: Localized(nameof(Resources.RequiredNullableType_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.RequiredNullableType_Description)),
            helpLinkUri: FormatHelpUri(IDs.RequiredNullableType),
            WellKnownDiagnosticTags.Compiler
            );

        private static string FormatHelpUri(string id)
        {
            return $"https://ubiquitydotnet.github.io/Ubiquity.NET.Utils/CommandLine/diagnostics/{id}.html";
        }
    }
}

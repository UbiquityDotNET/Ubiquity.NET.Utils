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
                IncorrectPropertyType
            ];

        internal static readonly DiagnosticDescriptor InternalError = new(
            id: IDs.InternalError,
            title: Localized(nameof(Resources.InternalError_Title)),
            messageFormat: Localized(nameof(Resources.InternalError_MessageFormat)),
            category: "Internal",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.InternalError_Description)),
            WellKnownDiagnosticTags.NotConfigurable
            );

        internal static readonly DiagnosticDescriptor MissingCommandAttribute = new(
            id: IDs.MissingCommandAttribute,
            title: Localized(nameof(Resources.MissingCommandAttribute_Title)),
            messageFormat: Localized(nameof(Resources.MissingCommandAttribute_MessageFormat)),
            category: "Usage",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: Localized(nameof(Resources.MissingCommandAttribute_Description)),
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
            WellKnownDiagnosticTags.Compiler,
            WellKnownDiagnosticTags.NotConfigurable
            );
    }
}

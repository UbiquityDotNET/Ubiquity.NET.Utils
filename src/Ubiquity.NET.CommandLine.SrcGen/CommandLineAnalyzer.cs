// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

using Ubiquity.NET.Extensions;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    // Sadly C# (and .NET) doesn't have any real concept of a type alias (typedef in C++)
    // This simplifies use within this file at least... [Sigh...]
#pragma warning disable IDE0065 // Misplaced using directive
#pragma warning disable SA1200 // Using directives should be placed correctly
#pragma warning disable SA1135 // Using directives should be qualified
    using PropertyAttributeHandlerAction = Action<SymbolAnalysisContext, IPropertySymbol, Location?, EquatableAttributeDataCollection>;

    // Sadly, this can't use the `PropertyAttributeHandlerAction`... symbol and it must use the actual type... [Sigh...]
    using PropertyAttributeHandlerMap = ImmutableDictionary<NamespaceQualifiedName, Action<SymbolAnalysisContext, IPropertySymbol, Location?, EquatableAttributeDataCollection>>;

    using SymbolHandlerAction = Action<SymbolAnalysisContext>;
    using SymbolHandlerMap = ImmutableDictionary<SymbolKind, Action<SymbolAnalysisContext>>;
#pragma warning restore SA1135 // Using directives should be qualified
#pragma warning restore SA1200 // Using directives should be placed correctly
#pragma warning restore IDE0065 // Misplaced using directive

    /// <summary>Analyzer for the command line attribute usage</summary>
    [DiagnosticAnalyzer( LanguageNames.CSharp )]
    public class CommandLineAnalyzer
        : DiagnosticAnalyzer
    {
        /// <inheritdoc/>
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => Diagnostics.CommandLineAnalyzerDiagnostics;

        /// <inheritdoc/>
        public override void Initialize( AnalysisContext context )
        {
            context.ConfigureGeneratedCodeAnalysis( GeneratedCodeAnalysisFlags.None );
            context.EnableConcurrentExecution();

            // As of right now there is no validation of attributes on a type
            // [A type for a command is allowed to have no Options/Arguments.]
            context.RegisterSymbolAction( OnTypeOrProperty, /*SymbolKind.NamedType,*/ SymbolKind.Property );
        }

        [SuppressMessage( "Design", "CA1031:Do not catch general exception types", Justification = "No loss of information, exception transformed to a diagnostic" )]
        private void OnTypeOrProperty( SymbolAnalysisContext context )
        {
            try
            {
                if(!SymbolHandlerMap.TryGetValue( context.Symbol.Kind, out var handler ))
                {
                    Debug.Assert( false, "Unexpected value for kind!" );
                    return;
                }

                handler( context );
                return;
            }
            catch(Exception ex)
            {
                Location? loc = context.Symbol.Locations.Length < 1 ? default : context.Symbol.Locations[0];
                ReportDiagnostic( context, Diagnostics.InternalError, loc, ex.Message );
            }
        }

        //private static void OnNamedType( SymbolAnalysisContext context )
        //{
        //    if(context.Symbol is not INamedTypeSymbol symbol)
        //    {
        //        Debug.Assert( false, "Non type symbol for named type..." );
        //        return;
        //    }
        //
        //    // Currently no validation on a named type symbol
        //}

        private static void OnProperty( SymbolAnalysisContext context )
        {
            if(context.Symbol is not IPropertySymbol propSymbol)
            {
                Debug.Assert( false, "Non-property symbol provided to property symbol handler!" );
                return;
            }

            // If there is a handler for an attribute, then call it to verify the attribute's information
            var attribData = context.Symbol.MatchingAttributes( Constants.GeneratingAttributeNames );
            var attribs = new EquatableAttributeDataCollection( attribData.Select(a=>new EquatableAttributeData(a)) );
            foreach(var attrib in attribData)
            {
                // lookup a handler for this type of attribute, any other attribute is left alone
                // it might be complete bogus, but that's not validated by this analyzer.
                if(PropertyHandlerMap.TryGetValue( attrib.GetNamespaceQualifiedName(), out var handler ))
                {
                    handler( context, propSymbol, attrib.GetLocation(), attribs );
                }
            }

            // TODO: WARN if type of property is nullable AND marked as required. That won't produce an error but probably
            // not the behavior intended...
        }

        private static void OnOptionAttribute(
            SymbolAnalysisContext context,
            IPropertySymbol symbol,
            Location? attribLoc,
            EquatableAttributeDataCollection attributes
            )
        {
            // Verify it is applied to a property in a type with a command attribute
            VerifyCommandAttribute( context, attribLoc, Constants.OptionAttribute.SimpleName );

            EquatableAttributeData attribute = attributes[Constants.OptionAttribute];
            VerifyNotNullableRequired( context, symbol, attribute, attribLoc );

            // Additional validations...
        }

        private static void OnFileValidationAttribute(
            SymbolAnalysisContext context,
            IPropertySymbol symbol,
            Location? attribLoc,
            EquatableAttributeDataCollection attribs
            )
        {
            // Verify it is applied to a property in a type with a command attribute
            VerifyCommandAttribute( context, attribLoc, Constants.FileValidationAttribute.SimpleName );

            // Verify an Option property (or maybe an argument attribute once supported) exists
            VerifyHasConstrainedAttribute( context, attribLoc, attribs, Constants.FileValidationAttribute.SimpleName );

            // Verify type of the property is System.IO.FileInfo.
            VerifyRequiredPropertyType( context, symbol, Constants.FileInfo, attribLoc, Constants.FileValidationAttribute.SimpleName );

            // Additional validations...
        }

        private static void OnFolderValidationAttribute(
            SymbolAnalysisContext context,
            IPropertySymbol symbol,
            Location? attribLoc,
            EquatableAttributeDataCollection attribs
            )
        {
            // Verify it is applied to a property in a type with a command attribute
            VerifyCommandAttribute( context, attribLoc, Constants.FolderValidationAttribute.SimpleName );

            // Verify an Option property (or maybe an argument attribute once supported) exists
            VerifyHasConstrainedAttribute( context, attribLoc, attribs, Constants.FolderValidationAttribute.SimpleName );

            // Verify type of the property is System.IO.DirectoryInfo.
            VerifyRequiredPropertyType( context, symbol, Constants.DirectoryInfo, attribLoc, Constants.FolderValidationAttribute.SimpleName );

            // Additional validations...
        }

        /// <summary>Reports a diagnostic if the containing type for the current symbol has a command attribute</summary>
        /// <param name="context">Context for the symbol to test</param>
        /// <param name="attribLoc">Location of the attribute requiring the attribute on the parent type</param>
        /// <param name="attribName">Name of the attribute (For reporting diagnostics)</param>
        /// <remarks>
        /// <para>At present, the only attribute that qualifies as a command attribute is <see cref="Constants.RootCommandAttribute"/>.
        /// In the future it is possible that this tests for another attribute for a sub-command.</para>
        /// <para>
        /// <paramref name="attribLoc"/> and <paramref name="attribName"/> are for the actual attribute that requires a command
        /// attribute on the containing type. These are used in forming the diagnostic message text.</para>
        /// </remarks>
        private static void VerifyCommandAttribute( SymbolAnalysisContext context, Location? attribLoc, string attribName )
        {
            // At present, there is only RootCommandAttribute, but in future a sub command attribute may exist
            var parentAttributes = context.Symbol.ContainingType.MatchingAttributes([Constants.RootCommandAttribute]);
            if(parentAttributes.IsDefaultOrEmpty)
            {
                ReportDiagnostic( context, Diagnostics.MissingCommandAttribute, attribLoc, attribName );
            }
        }

        /// <summary>Verifies (and reports a diagnostic if not) that a set of attributes contains a required constraint</summary>
        /// <param name="context">Context to use for reporting diagnostics</param>
        /// <param name="attribLoc">Location to use for any diagnostics reported</param>
        /// <param name="attribs">Set of attributes to check</param>
        /// <param name="typeConstraintName">Name of the attribute that requires a constraint (For diagnostic message)</param>
        /// <remarks>
        /// The <paramref name="attribLoc"/> is used to report diagnostics that normally references the attribute
        /// that is missing the constrained attribute. (ex. The `FileValidation` or `FolderValidation` that does NOT
        /// have an `OptionAttribute` that it validates.
        /// </remarks>
        private static void VerifyHasConstrainedAttribute(
            SymbolAnalysisContext context,
            Location? attribLoc,
            EquatableAttributeDataCollection attribs,
            string typeConstraintName
            )
        {
            // Verify an Option property
            if(!attribs.TryGetValue( Constants.OptionAttribute, out _ ))
            {
                ReportDiagnostic( context, Diagnostics.MissingConstraintAttribute, attribLoc, typeConstraintName );
            }

            // TODO: validate an Argument attribute or Option attribute
        }

        /// <summary>Verifies a property has an expected type</summary>
        /// <param name="context">Context to use for reporting diagnostics</param>
        /// <param name="symbol">Property symbol to test</param>
        /// <param name="expectedType">Name of the expected type</param>
        /// <param name="attribLoc">Location of the attribute that requiring a specific type</param>
        /// <param name="attribName">Name of the attribute that requires a specific type</param>
        /// <remarks>
        /// The <paramref name="attribLoc"/> and <paramref name="attribName"/> are used in any diagnostics
        /// reported. That is, they reference the attribute that requires a specific type. It is debatable
        /// if the location of the return type is wrong or the attribute is wrong...
        /// </remarks>
        private static void VerifyRequiredPropertyType(
            SymbolAnalysisContext context,
            IPropertySymbol symbol,
            NamespaceQualifiedName expectedType,
            Location? attribLoc,
            string attribName
            )
        {
            if(symbol.Type.GetNamespaceQualifiedName() != expectedType)
            {
                ReportDiagnostic( context, Diagnostics.IncorrectPropertyType, attribLoc, attribName, expectedType.ToString( "A", null ) );
            }
        }

        private static void VerifyNotNullableRequired(
            SymbolAnalysisContext context,
            IPropertySymbol property,
            EquatableAttributeData attribute,
            Location? attribLoc )
        {
            if(attribute.NamedArguments.TryGetValue( Constants.CommonAttributeNamedArgs.Required, out StructurallyEquatableTypedConstant tc ))
            {
                bool isRequired = !tc.IsNull && (bool)tc.Value!;
                NamespaceQualifiedTypeName propType = property.Type.GetNamespaceQualifiedName();
                if(isRequired && propType.IsNullable)
                {
                    ReportDiagnostic( context, Diagnostics.RequiredNullableType, attribLoc, $"{propType:A}", property.Name );
                }
            }
        }

        private static void ReportDiagnostic( SymbolAnalysisContext context, DiagnosticDescriptor descriptor, Location? loc, params object[] args )
        {
            context.ReportDiagnostic( Diagnostic.Create( descriptor, loc, args ) );
        }

        private static readonly SymbolHandlerMap SymbolHandlerMap
            = new DictionaryBuilder<SymbolKind, SymbolHandlerAction>()
            {
                // [SymbolKind.NamedType] = OnNamedType,
                [SymbolKind.Property] = OnProperty,
            }.ToImmutable();

        private static readonly PropertyAttributeHandlerMap PropertyHandlerMap
            = new DictionaryBuilder<NamespaceQualifiedName, PropertyAttributeHandlerAction>()
            {
                [Constants.OptionAttribute] = OnOptionAttribute,
                [Constants.FileValidationAttribute] = OnFileValidationAttribute,
                [Constants.FolderValidationAttribute] = OnFolderValidationAttribute,
            }.ToImmutable();
    }
}

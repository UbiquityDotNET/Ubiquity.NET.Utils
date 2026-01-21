// Copyright (c) Ubiquity.NET Contributors. All rights reserved.
// Licensed under the Apache-2.0 WITH LLVM-exception license. See the LICENSE.md file in the project root for full license information.

using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp;

namespace Ubiquity.NET.CommandLine.SrcGen
{
    /// <summary>Roslyn Source generator to generate backing support for command line parsing</summary>
    [Generator]
    public class CommandGenerator
        : IIncrementalGenerator
    {
        /// <inheritdoc/>
        public void Initialize( IncrementalGeneratorInitializationContext context )
        {
            // NOTE: if the tests aren't finding anything double check that Constants.* contains the correct namespace name.

            string rootCommandAttribName = Constants.RootCommandAttribute.ToString(); // Don't use alias or global prefix for this

            var optionClasses
               = context.SyntaxProvider
                        .ForAttributeWithMetadataName(
                            rootCommandAttribName,
                            predicate: static (s, _) => true,
                            transform: static (ctx, _) => CollectCommandAttributeData( ctx )
                         ) // filter for command attribute only
                        .Where( static m => m is not null )
                        .Select( static (m, ct) => (RootCommandInfo)m! ) // convert nullable type to non null as preceding where clause filters out null values
                        .WithTrackingName( TrackingNames.CommandClass );

            context.RegisterSourceOutput( optionClasses, Execute );
        }

        private static RootCommandInfo? CollectCommandAttributeData( GeneratorAttributeSyntaxContext context )
        {
            // Do nothing if the target doesn't support what the generated code needs or something is wrong.
            // Errors are detected by a distinct analyzer; code generators just NOP as fast as possible.
            // see: https://csharp-evolution.com/guides/language-by-platform
            var compilation = context.SemanticModel.Compilation;
            if( context.Attributes.Length != 1 // Multiple instances not allowed and 0 is just broken.
                || compilation.Language != "C#"
                || !compilation.HasLanguageVersionAtLeastEqualTo( LanguageVersion.CSharp12 ) // C# 12 => .NET 8.0 => supported until 2026-11-10 (LTS)
                || context.TargetSymbol is not INamedTypeSymbol namedTypeSymbol
                || context.TargetNode is not ClassDeclarationSyntax commandClass
            )
            {
                return null;
            }

            var theAttribute = context.Attributes[0];

            // Capture the names of all properties with a generating attribute.
            // This starts out assuming all members are a property. This is a bit aggressive
            // but ensures only one allocation of the result is needed for all (That is, all
            // members is the max size needed so allocate that amount once).
            var members = namedTypeSymbol.GetMembers();
            var propertyInfoBuilder = ImmutableArray.CreateBuilder<PropertyInfo>( members.Length );
            foreach(ISymbol member in members)
            {
                // filter to referenceable properties
                // ignore nullable value types as that would produce an error
                // (Can't use nullable annotations for generic types
                if(member.CanBeReferencedByName && member is IPropertySymbol propSym /*&& !propSym.Type.IsNullableValueType()*/)
                {
                    propertyInfoBuilder.Add( new PropertyInfo( propSym, Constants.GeneratingAttributeNames ) );
                }
            }

            return new RootCommandInfo(
                        namedTypeSymbol.GetNamespaceQualifiedName(),
                        theAttribute,
                        propertyInfoBuilder.ToImmutable()
                   );
        }

        private static void Execute( SourceProductionContext context, RootCommandInfo source )
        {
            var template = new Templates.RootCommandClassTemplate(source);
            var generatedSource = template.GenerateText();
            string hintPath = $"{source.TargetName:R}.g.cs";
            context.AddSource( hintPath, generatedSource );
        }
    }
}

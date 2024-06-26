using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Strongly.Options.SourceGenerators.Extensions;

namespace Strongly.Options.SourceGenerators;

using OptionsMetadata = (string FullyQualifiedTypeName, string Section);

[Generator]
public class StronglyOptionsRegistrationSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsMetadataProvider = context
           .SyntaxProvider
           .ForAttributeWithMetadataName(
                WellKnownNamings.StronglyOptionsAttribute,
                predicate: (node, _) => node
                    is ClassDeclarationSyntax
                    or RecordDeclarationSyntax,
                transform: GetOptionsMetadata)
           .Collect();

        var moduleNameProvider = context
           .CompilationProvider
           .Select((x, _) => x.Assembly.GetModuleNameFromAttributeOrShortAssemblyNameByDefault());

        context.RegisterSourceOutput(
            optionsMetadataProvider.Combine(moduleNameProvider),
            GenerateCode);
    }

    private OptionsMetadata GetOptionsMetadata(
        GeneratorAttributeSyntaxContext context,
        CancellationToken _)
    {
        var optionsType = context
           .TargetSymbol
           .ToDisplayString();

        var optionsAttributeData = context
           .Attributes
           .First(x => x.AttributeClass!.ToDisplayString() == WellKnownNamings.StronglyOptionsAttribute);

        var section = optionsAttributeData
           .ConstructorArguments
           .First()
           .Value!
           .ToString();

        return ($"global::{optionsType}", section);
    }

    private void GenerateCode(
        SourceProductionContext context,
        (ImmutableArray<OptionsMetadata> Options, string ModuleName) sourceGenData)
    {
        var configureMethods = sourceGenData
           .Options
           .Select(x => Templates
               .CreateConfigureMethodInvokeText(
                    x.FullyQualifiedTypeName,
                    x.Section));

        var mergedConfigureMethods = string.Join("\n", configureMethods);

        var addStronglyOptionsExtensionMethod = Templates
           .CreateDependencyInjectionExtensionText(
                sourceGenData.ModuleName,
                mergedConfigureMethods);

        context.AddSource(
            "StronglyOptionsServiceCollectionExtensions.g.cs",
            SourceText.From(addStronglyOptionsExtensionMethod, Encoding.UTF8));
    }
}

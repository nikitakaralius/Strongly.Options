using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Strongly.Options.SourceGenerators;

using OptionsMetadata = (string FullyQualifiedTypeName, string Section);

[Generator]
public class StronglyOptionsRegistrationSourceGenerator : IIncrementalGenerator
{
    private const string StronglyOptionsNamespace = "Strongly.Options";

    private const string StronglyOptionsAttribute = $"{StronglyOptionsNamespace}.StronglyOptionsAttribute";

    private const string RootSection = "";

    private const string ConfigurationParameterName = "configuration";

    private const string ConfigureTemplate =
        """
                    services.Configure<{Type}>({GetSection});
        """;

    private const string GetRequiredSectionTemplate =
        """
        configuration.GetRequiredSection("{Section}")
        """;

    private const string MethodTemplate =
        $$"""
        using Microsoft.Extensions.Configuration;
        using Microsoft.Extensions.DependencyInjection;
        
        namespace {{StronglyOptionsNamespace}}
        {
            public static class StronglyOptionsServiceCollectionExtensions
            {
                /// <summary>
                /// Scans current assembly and registers all options types marked with the <see cref="StronglyOptionsAttribute"/>.
                /// </summary>
                /// <example>
                /// <code>
                /// var configuration = builder.Configuration;
                /// builder.Services.AddStronglyOptions(configuration);
                /// </code>
                /// </example>
                public static IServiceCollection AddStronglyOptions(
                    this IServiceCollection services,
                    IConfiguration {{ConfigurationParameterName}})
                {
        {Configure}
            
                    return services;
                }
            }
        }

        """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var optionsMetadataProvider = context
           .SyntaxProvider
           .ForAttributeWithMetadataName(
                StronglyOptionsAttribute,
                predicate: (node, _) => node
                    is ClassDeclarationSyntax
                    or RecordDeclarationSyntax,
                transform: GetOptionsMetadata)
           .Collect();

        context.RegisterSourceOutput(
            optionsMetadataProvider,
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
           .First(x => x.AttributeClass!.ToDisplayString() == StronglyOptionsAttribute);

        var section = optionsAttributeData
           .ConstructorArguments
           .First()
           .Value!
           .ToString();

        return ($"global::{optionsType}", section);
    }

    private void GenerateCode(
        SourceProductionContext context,
        ImmutableArray<OptionsMetadata> options)
    {
        var configureMethods = options
           .Select(x => ConfigureTemplate
               .Replace("{Type}", x.FullyQualifiedTypeName)
               .Replace("{GetSection}", CreateGetSectionFromTemplate(x.Section)));

        var mergedConfigureMethods = string.Join("\n", configureMethods);

        var addStronglyOptionsExtensionMethod = MethodTemplate.Replace("{Configure}", mergedConfigureMethods);

        context.AddSource(
            "StronglyOptionsServiceCollectionExtensions.g.cs",
            SourceText.From(addStronglyOptionsExtensionMethod, Encoding.UTF8));
    }

    private string CreateGetSectionFromTemplate(string section)
    {
        return section == RootSection
            ? ConfigurationParameterName
            : GetRequiredSectionTemplate.Replace("{Section}", section);
    }
}

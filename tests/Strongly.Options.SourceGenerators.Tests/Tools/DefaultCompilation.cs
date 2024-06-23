using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Strongly.Options.SourceGenerators.Tests.Tools;

public static class DefaultCompilation
{
    public static CSharpCompilation Create(
        string assemblyName,
        IEnumerable<SyntaxTree> syntaxTrees)
    {
        return CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            [
                MetadataReference.CreateFromFile(typeof(StronglyOptionsAttribute).Assembly.Location),
                ..Basic.Reference.Assemblies.Net80.References.All
            ],
            new(OutputKind.DynamicallyLinkedLibrary));
    }
}

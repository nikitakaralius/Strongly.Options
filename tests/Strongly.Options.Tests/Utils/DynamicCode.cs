using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Strongly.Options.Tests.Utils;

internal sealed class DynamicCode
{
    private static readonly IReadOnlyList<PortableExecutableReference> References = AppDomain
       .CurrentDomain
       .GetAssemblies()
       .Where(a => !a.IsDynamic)
       .Select(a => MetadataReference.CreateFromFile(a.Location))
       .ToList();

    private readonly string _code;

    private DynamicCode(string code)
    {
        _code = code;
    }

    public static implicit operator DynamicCode(string code) => new(code);

    public Assembly EmitAssembly(string? assemblyName = null)
    {
        assemblyName ??= Guid.NewGuid().ToString();

        var syntaxTree = CSharpSyntaxTree.ParseText(_code);

        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: [syntaxTree],
            references: References,
            options: new(OutputKind.DynamicallyLinkedLibrary));

        using var peStream = new MemoryStream();
        var emitResult = compilation.Emit(peStream);

        if (!emitResult.Success)
        {
            var errorMessages = emitResult
               .Diagnostics
               .Select(CreateCompilationErrorMessage);

            throw new InvalidOperationException(
                $"Compilation failed: {string.Join('\n', errorMessages)}");
        }

        peStream.Seek(0, SeekOrigin.Begin);
        return AssemblyLoadContext.Default.LoadFromStream(peStream);
    }

    private static string CreateCompilationErrorMessage(Diagnostic x) =>
        $"""
         ID: {x.Id}, Message: {x.GetMessage()}
         Location: {x.Location.GetLineSpan()}
         Severity: {x.Severity}
         """;
}

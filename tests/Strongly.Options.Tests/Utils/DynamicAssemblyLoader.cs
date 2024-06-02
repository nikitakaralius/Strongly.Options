using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Strongly.Options.Tests.Utils;

internal static class DynamicAssemblyLoader
{
    public static Assembly CreateFromCode(
        string code,
        string assemblyName)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);

        var references = AppDomain.CurrentDomain
           .GetAssemblies()
           .Where(a => !a.IsDynamic)
           .Select(a => MetadataReference.CreateFromFile(a.Location))
           .ToList();

        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: [syntaxTree],
            references: references,
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

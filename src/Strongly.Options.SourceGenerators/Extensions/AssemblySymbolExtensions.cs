using System.Linq;
using Microsoft.CodeAnalysis;

namespace Strongly.Options.SourceGenerators.Extensions;

using static WellKnownNamings;

public static class AssemblySymbolExtensions
{
    public static string GetModuleNameFromAttributeOrShortAssemblyNameByDefault(this IAssemblySymbol assembly)
    {
        var userSpecifiedModuleName = assembly
           .GetAttributes()
           .FirstOrDefault(x => x.AttributeClass!.ToDisplayString() == StronglyOptionsModuleAttribute)?
           .ConstructorArguments
           .FirstOrDefault()
           .Value?
           .ToString();

        return userSpecifiedModuleName ?? assembly.GetLastAssemblyNamePart();
    }

    private static string GetLastAssemblyNamePart(this IAssemblySymbol assembly)
    {
        var assemblyName = assembly.Name;
        var lastIndexOfDot = assemblyName.LastIndexOf('.');

        if (lastIndexOfDot == -1)
            return assemblyName;

        return assemblyName.Substring(lastIndexOfDot + 1);
    }
}

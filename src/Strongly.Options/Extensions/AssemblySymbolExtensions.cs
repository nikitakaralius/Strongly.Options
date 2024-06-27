using System.Linq;
using Microsoft.CodeAnalysis;

namespace Strongly.Options.Extensions;

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

        return userSpecifiedModuleName ?? assembly.GetLastPartOfName();
    }

    private static string GetLastPartOfName(this IAssemblySymbol assembly)
    {
        var assemblyName = assembly.Name;
        var lastDotIndex = assemblyName.LastIndexOf('.');

        if (lastDotIndex == -1)
            return assemblyName;

        return assemblyName.Substring(lastDotIndex + 1);
    }
}

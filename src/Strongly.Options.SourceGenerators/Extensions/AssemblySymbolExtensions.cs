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

        return userSpecifiedModuleName ?? assembly.Name;
    }
}

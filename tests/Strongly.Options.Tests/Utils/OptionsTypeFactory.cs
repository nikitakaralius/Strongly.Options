using System.Reflection;
using Microsoft.Extensions.Options;

namespace Strongly.Options.Tests.Utils;

internal static class OptionsTypeFactory
{
    public static Type MakeGenericType(
        string optionsTypeName,
        Assembly assembly)
    {
        var optionsType = assembly.GetType(optionsTypeName);

        if (optionsType is null)
            throw new InvalidOperationException(
                $"Unable to find options of type {optionsTypeName} in assembly {assembly.GetName()}");

        return typeof(IOptions<>).MakeGenericType(optionsType);
    }
}

using Microsoft.Extensions.Options;

namespace Strongly.Options.Tests.Utils;

internal static class FluentAssertionsExtensions
{
    public static object GetOptionsPropertyValue(
        this IOptions<object> options,
        string propertyName)
    {
        return options.GetOptionsPropertyValue<object>(propertyName);
    }

    public static T GetOptionsPropertyValue<T>(
        this IOptions<object> options,
        string propertyName)
    {
        var optionsValue = options
           .GetType()
           .GetProperty("Value")!
           .GetValue(options)!;

        return (T) optionsValue
           .GetType()
           .GetProperty(propertyName)!
           .GetValue(optionsValue)!;
    }
}

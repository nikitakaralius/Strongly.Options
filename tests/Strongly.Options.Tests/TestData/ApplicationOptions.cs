using Microsoft.Extensions.Configuration;

namespace Strongly.Options.Tests.TestData;

[StronglyOptions("App")]
public sealed record ApplicationOptions
{
    [ConfigurationKeyName("app.alias")]
    public string Alias { get; init; } = default!;
}

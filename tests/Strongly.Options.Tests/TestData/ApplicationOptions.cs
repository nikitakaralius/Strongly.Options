using Microsoft.Extensions.Configuration;

namespace Strongly.Options.Tests.TestData;

/// <summary>
/// This type is being used in <see cref="StronglyOptionsExtensionsTests"/>. Do not change anything here.
/// </summary>
[StronglyOptions("App")]
public sealed record ApplicationOptions
{
    [ConfigurationKeyName("app.alias")]
    public string Alias { get; init; } = default!;
}

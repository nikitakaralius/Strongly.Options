namespace Strongly.Options.Tests.TestData;

[StronglyOptions("App")]
public sealed record ApplicationOptions
{
    public string ApplicationCode { get; init; } = default!;
}

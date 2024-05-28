namespace Strongly.Options.Tests.TestData;

[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record ApplicationOptions
{
    public string ApplicationCode { get; init; } = default!;
}

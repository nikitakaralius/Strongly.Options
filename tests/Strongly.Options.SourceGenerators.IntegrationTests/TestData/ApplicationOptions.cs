namespace Strongly.Options.SourceGenerators.IntegrationTests.TestData;

[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record ApplicationOptions
{
    public string Alias { get; init; } = default!;
}

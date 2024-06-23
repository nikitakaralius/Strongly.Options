namespace Strongly.Options.SourceGenerators.IntegrationTests.TestData;

[StronglyOptions("Service")]
public sealed record ServiceOptions
{
    public string Url { get; init; } = string.Empty;

    public Guid Key { get; init; } = Guid.Empty;

    public int RequestsPerHour { get; init; } = 0;
}

[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record FeatureOptions
{
    public bool EnableExperimentalFeatures { get; init; } = false;
}

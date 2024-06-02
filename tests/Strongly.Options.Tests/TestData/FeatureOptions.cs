namespace Strongly.Options.Tests.TestData;

[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record FeatureOptions
{
    public bool EnableExperimentalFeatures { get; init; } = false;
}

namespace Strongly.Options.Tests.TestData;

/// <summary>
/// This type is being used in <see cref="StronglyOptionsExtensionsTests"/> tests. Do not change anything here.
/// </summary>
[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record FeatureOptions
{
    public bool EnableExperimentalFeatures { get; init; } = false;
}

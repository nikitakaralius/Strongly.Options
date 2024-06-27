namespace Strongly.Options.TestsData.CausingSectionNotFound;

[StronglyOptions("NoSectionLikeThat")]
public sealed record SectionNotFoundOptions
{
    public string Alias { get; init; } = default!;
}

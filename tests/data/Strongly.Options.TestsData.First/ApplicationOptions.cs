namespace Strongly.Options.TestsData.First;

[StronglyOptions(StronglyOptionsSection.Root)]
public sealed record ApplicationOptions
{
    public string Alias { get; init; } = default!;
}

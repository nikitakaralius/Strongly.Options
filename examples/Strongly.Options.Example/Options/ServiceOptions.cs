namespace Strongly.Options.Example.Options;

// We strongly suggest to use immutable records
[StronglyOptions("Service")]
public sealed record ServiceOptions
{
    public required string Url { get; init; }

    public required string Key { get; init; }

    public required int RequestsPerHour { get; init; }
}

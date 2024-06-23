namespace Strongly.Options.Sample.Options;

// We strongly suggest to use immutable records
[StronglyOptions("Service")]
public sealed record ServiceOptions
{
    public required string Url { get; init; }

    public required Guid Key { get; init; }

    public required int RequestsPerHour { get; init; }
}

namespace Strongly.Options.Tests.TestData;

[StronglyOptions("Logging")]
public class LoggingOptions
{
    public IReadOnlyList<WriteToSection> WriteTo { get; set; } = [];
}

public class WriteToSection
{
    public string Name { get; set; } = string.Empty;

    public LogLevel Level { get; set; } = LogLevel.Information;
}

namespace Strongly.Options;

public sealed class SectionNotFoundException : Exception
{
    public SectionNotFoundException(string message) : base(message) { }
}

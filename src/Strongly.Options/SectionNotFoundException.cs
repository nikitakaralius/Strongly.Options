namespace Strongly.Options;

public sealed class SectionNotFoundException : Exception
{
    internal SectionNotFoundException(string message) : base(message) { }
}

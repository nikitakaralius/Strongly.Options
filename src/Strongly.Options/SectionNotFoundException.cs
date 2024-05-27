namespace Strongly.Options;

internal sealed class SectionNotFoundException : Exception
{
    public SectionNotFoundException(string message) : base(message) { }
}

namespace Strongly.Options;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class StronglyOptionsAttribute : Attribute
{
    public StronglyOptionsAttribute(string section)
    {
        Section = section;
    }

    public string Section { get; }
}

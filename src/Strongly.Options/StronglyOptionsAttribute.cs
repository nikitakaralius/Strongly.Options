namespace Strongly.Options;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StronglyOptionsAttribute : Attribute
{
    public StronglyOptionsAttribute(string section)
    {
        Section = section;
    }

    public string Section { get; }
}

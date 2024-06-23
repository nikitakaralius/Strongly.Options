namespace Strongly.Options;

[AttributeUsage(AttributeTargets.Assembly)]
public sealed class StronglyOptionsModuleAttribute : Attribute
{
    public StronglyOptionsModuleAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

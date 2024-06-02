namespace Strongly.Options.Example.Options;

// However you can still use classes with getters and setters
[StronglyOptions("Auth")]
public class AuthOptions
{
    public Uri AuthorityUrl { get; set; } = null!;

    public IReadOnlyList<string> Audiences { get; set; } = [];
}

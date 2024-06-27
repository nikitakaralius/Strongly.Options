namespace Strongly.Options.TestsData.First;

[StronglyOptions("Auth")]
public class AuthOptions
{
    public Uri AuthorityUrl { get; set; } = null!;

    public IReadOnlyList<string> Audiences { get; set; } = [];
}

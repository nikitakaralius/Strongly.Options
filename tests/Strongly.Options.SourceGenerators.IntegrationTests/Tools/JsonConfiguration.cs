using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Strongly.Options.SourceGenerators.IntegrationTests.Tools;

internal sealed class JsonConfiguration : IConfiguration
{
    private readonly IConfiguration _configuration;

    private JsonConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public static implicit operator JsonConfiguration(string settings)
    {
        using var memoryStream = new MemoryStream();
        memoryStream.Write(Encoding.Default.GetBytes(settings));
        memoryStream.Seek(0, SeekOrigin.Begin);

        var configuration = new ConfigurationBuilder()
           .AddJsonStream(memoryStream)
           .Build();

        return new(configuration);
    }

    public string? this[string key]
    {
        get => _configuration[key];
        set => _configuration[key] = value;
    }

    public IConfigurationSection GetSection(string key) =>
        _configuration.GetSection(key);

    public IEnumerable<IConfigurationSection> GetChildren() =>
        _configuration.GetChildren();

    public IChangeToken GetReloadToken() =>
        _configuration.GetReloadToken();
}

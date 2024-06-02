using System.Text;
using Microsoft.Extensions.Configuration;

namespace Strongly.Options.Tests.Utils;

public static class ConfigurationFactory
{
    public static IConfiguration CreateFromJson(string jsonSettings)
    {
        using var memoryStream = new MemoryStream();
        memoryStream.Write(Encoding.Default.GetBytes(jsonSettings));
        memoryStream.Seek(0, SeekOrigin.Begin);

        var configuration = new ConfigurationBuilder()
           .AddJsonStream(memoryStream)
           .Build();

        return configuration;
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Strongly.Options.SourceGenerators.IntegrationTests.Tools;
using Strongly.Options.TestsData.First;
using Strongly.Options.TestsData.Second;

namespace Strongly.Options.SourceGenerators.IntegrationTests;

public class StronglyOptionsServiceCollectionExtensionsTests
{
    [Fact]
    public void Maps_options_values()
    {
        JsonConfiguration configuration =
            """
            {
              "Auth": {
                "AuthorityUrl": "https://auth-url.com",
                "Audiences": [
                  "hello",
                  "world"
                ]
              }
            }
            """;

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration)
           .BuildServiceProvider();

        var authOptions = provider
           .GetRequiredService<IOptions<AuthOptions>>()
           .Value;

        // Assert
        authOptions
           .AuthorityUrl
           .Should()
           .Be(new Uri("https://auth-url.com"));

        authOptions
           .Audiences
           .Should()
           .ContainInOrder(["hello", "world"])
           .And
           .HaveCount(2);
    }

    [Fact]
    public void Should_throw_when_section_not_found()
    {
        // Arrange
        JsonConfiguration configuration =
            """
            {
              "Auth": {
                "AuthorityUrl": "https://auth-url.com",
                "Audiences": [
                  "hello",
                  "world"
                ]
              }
            }
            """;

        // Act
        var act = () => new ServiceCollection()
           .AddCausingSectionNotFoundStronglyOptions(configuration);

        // Assert
        act
           .Should()
           .Throw<SectionNotFoundException>();
    }

    [Fact]
    public void Registers_all_options_in_assembly()
    {
        // Arrange
        JsonConfiguration configuration =
            """
            {
              "Service": {
                "Url": "https://some-url-goes-here.com",
                "Key": "e324a183-54df-4f24-9db8-66322d066214",
                "RequestsPerHour": 5
              },
              "Auth": {
                "AuthorityUrl": "https://auth-url.com",
                "Audiences": [
                  "hello",
                  "world"
                ]
              }
            }
            """;

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration)
           .BuildServiceProvider();

        var authOptions = provider
           .GetRequiredService<IOptions<AuthOptions>>()
           .Value;

        var serviceOptions = provider
           .GetRequiredService<IOptions<ServiceOptions>>()
           .Value;

        // Assert
        authOptions
           .AuthorityUrl
           .Should()
           .NotBeNull();

        serviceOptions
           .Url
           .Should()
           .NotBeNull();
    }

     [Fact]
     public void Registers_all_options_in_multiple_assemblies()
     {
         JsonConfiguration configuration =
             """
             {
               "Service": {
                 "Url": "https://some-url-goes-here.com",
                 "Key": "e324a183-54df-4f24-9db8-66322d066214",
                 "RequestsPerHour": 5
               },
               "Auth": {
                 "AuthorityUrl": "https://auth-url.com",
                 "Audiences": [
                   "hello",
                   "world"
                 ]
               }
             }
             """;

         // Act
         var provider = new ServiceCollection()
            .AddStronglyOptions(configuration)
            .AddSomeAnotherProjectStronglyOptions(configuration)
            .BuildServiceProvider();

         var authOptions = provider
            .GetRequiredService<IOptions<AuthOptions>>()
            .Value;

         var serviceOptions = provider
            .GetRequiredService<IOptions<ServiceOptions>>()
            .Value;

         // Assert
         authOptions
            .AuthorityUrl
            .Should()
            .NotBeNull();

         serviceOptions
            .Url
            .Should()
            .NotBeNull();
     }

     [Fact]
     public void Maps_multiple_top_level_options()
     {
         // Arrange
         JsonConfiguration configuration =
             """
             {
               "Alias": "Strongly.Options",
               "EnableExperimentalFeatures": true
             }
             """;

         // Act
         var provider = new ServiceCollection()
            .AddStronglyOptions(configuration)
            .AddSomeAnotherProjectStronglyOptions(configuration)
            .BuildServiceProvider();

         var applicationOptions = provider
            .GetRequiredService<IOptions<ApplicationOptions>>()
            .Value;

         var featureOptions = provider
            .GetRequiredService<IOptions<FeatureOptions>>()
            .Value;

         // Assert
         applicationOptions
            .Alias
            .Should()
            .Be("Strongly.Options");

         featureOptions
            .EnableExperimentalFeatures
            .Should()
            .BeTrue();
     }
}

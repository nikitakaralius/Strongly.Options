using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Strongly.Options.SourceGenerators.IntegrationTests.TestData;
using Strongly.Options.SourceGenerators.IntegrationTests.Tools;

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

    [Fact] // TODO: create a separate project for that, you also need to define a mechanism to override method name
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
           .AddStronglyOptions(configuration);

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

//     [Fact]
//     public void Registers_all_options_in_multiple_assemblies()
//     {
//         // Arrange
//         DynamicCode authOptionsCode =
//             """
//             using System;
//             using System.Collections.Generic;
//
//             using Strongly.Options;
//
//             [StronglyOptions("Auth")]
//             public class AuthOptions
//             {
//                 public Uri AuthorityUrl { get; set; } = null!;
//
//                 public IReadOnlyList<string> Audiences { get; set; } = [];
//             }
//
//             """;
//
//         DynamicCode serviceOptionsCode =
//             """
//             using System;
//             using System.Collections.Generic;
//
//             using Strongly.Options;
//
//             [StronglyOptions("Service")]
//             public sealed record ServiceOptions
//             {
//                 public required string Url { get; init; }
//
//                 public required Guid Key { get; init; }
//
//                 public required int RequestsPerHour { get; init; }
//             }
//
//             """;
//
//         JsonConfiguration configuration =
//             """
//             {
//               "Service": {
//                 "Url": "https://some-url-goes-here.com",
//                 "Key": "e324a183-54df-4f24-9db8-66322d066214",
//                 "RequestsPerHour": 5
//               },
//               "Auth": {
//                 "AuthorityUrl": "https://auth-url.com",
//                 "Audiences": [
//                   "hello",
//                   "world"
//                 ]
//               }
//             }
//             """;
//
//         var authOptionsAssembly = authOptionsCode.EmitAssembly();
//         var serviceOptionsAssembly = serviceOptionsCode.EmitAssembly();
//
//         var authOptionsType = OptionsTypeFactory.MakeGenericType("AuthOptions", authOptionsAssembly);
//         var serviceOptionsType = OptionsTypeFactory.MakeGenericType("ServiceOptions", serviceOptionsAssembly);
//
//         // Act
//         var provider = new ServiceCollection()
//            .AddStronglyOptions(configuration, authOptionsAssembly)
//            .AddStronglyOptions(configuration, serviceOptionsAssembly)
//            .BuildServiceProvider();
//
//         var authOptions = (IOptions<object>) provider.GetRequiredService(authOptionsType);
//         var serviceOptions = (IOptions<object>) provider.GetRequiredService(serviceOptionsType);
//
//         // Assert
//         authOptions
//            .GetOptionsPropertyValue("AuthorityUrl")
//            .Should()
//            .NotBeNull();
//
//         serviceOptions
//            .GetOptionsPropertyValue("Url")
//            .Should()
//            .NotBeNull();
//     }

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

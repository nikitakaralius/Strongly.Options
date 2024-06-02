using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Strongly.Options.Tests.TestData;
using Strongly.Options.Tests.Utils;

namespace Strongly.Options.Tests;

public class StronglyOptionsExtensionsTests
{
    [Fact]
    public void Maps_options_values()
    {
        DynamicCode code =
            """
            using System;
            using System.Collections.Generic;

            using Strongly.Options;

            [StronglyOptions("Auth")]
            public class AuthOptions
            {
                public Uri AuthorityUrl { get; set; } = null!;
            
                public IReadOnlyList<string> Audiences { get; set; } = [];
            }

            """;

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

        var assembly = code.EmitAssembly();

        var authOptionsType = OptionsTypeFactory.MakeGenericType("AuthOptions", assembly);

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration, assembly)
           .BuildServiceProvider();

        var authOptions = (IOptions<object>) provider.GetRequiredService(authOptionsType);

        // Assert
        authOptions
           .GetOptionsPropertyValue<Uri>("AuthorityUrl")
           .Should()
           .Be(new Uri("https://auth-url.com"));

        authOptions
           .GetOptionsPropertyValue<IReadOnlyList<string>>("Audiences")
           .Should()
           .ContainInOrder(["hello", "world"])
           .And
           .HaveCount(2);
    }

    [Fact]
    public void Should_throw_when_section_not_found()
    {
        // Arrange
        DynamicCode code =
            """
            using Strongly.Options;

            [StronglyOptions("NoSectionLikeThat")]
            public sealed record FeatureOptions
            {
                public bool EnableExperimentalFeatures { get; } = false;
            }

            """;

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
           .AddStronglyOptions(configuration, code.EmitAssembly());

        // Assert
        act
           .Should()
           .Throw<SectionNotFoundException>();
    }

    [Fact]
    public void Registers_all_options_in_assembly()
    {
        // Arrange
        DynamicCode code =
            """
            using System;
            using System.Collections.Generic;
            
            using Strongly.Options;
            
            [StronglyOptions("Auth")]
            public class AuthOptions
            {
                public Uri AuthorityUrl { get; set; } = null!;
            
                public IReadOnlyList<string> Audiences { get; set; } = [];
            }

            [StronglyOptions("Service")]
            public sealed record ServiceOptions
            {
                public required string Url { get; init; }
            
                public required Guid Key { get; init; }
            
                public required int RequestsPerHour { get; init; }
            }

            """;

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

        var assembly = code.EmitAssembly();

        var authOptionsType = OptionsTypeFactory.MakeGenericType("AuthOptions", assembly);
        var serviceOptionsType = OptionsTypeFactory.MakeGenericType("ServiceOptions", assembly);

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration, assembly)
           .BuildServiceProvider();

        var authOptions = (IOptions<object>) provider.GetRequiredService(authOptionsType);
        var serviceOptions = (IOptions<object>) provider.GetRequiredService(serviceOptionsType);

        // Assert
        authOptions
           .GetOptionsPropertyValue("AuthorityUrl")
           .Should()
           .NotBeNull();

        serviceOptions
           .GetOptionsPropertyValue("Url")
           .Should()
           .NotBeNull();
    }

    [Fact]
    public void Registers_all_options_in_multiple_assemblies()
    {
        // Arrange
        DynamicCode authOptionsCode =
            """
            using System;
            using System.Collections.Generic;

            using Strongly.Options;

            [StronglyOptions("Auth")]
            public class AuthOptions
            {
                public Uri AuthorityUrl { get; set; } = null!;
            
                public IReadOnlyList<string> Audiences { get; set; } = [];
            }

            """;

        DynamicCode serviceOptionsCode =
            """
            using System;
            using System.Collections.Generic;

            using Strongly.Options;

            [StronglyOptions("Service")]
            public sealed record ServiceOptions
            {
                public required string Url { get; init; }
            
                public required Guid Key { get; init; }
            
                public required int RequestsPerHour { get; init; }
            }

            """;

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

        var authOptionsAssembly = authOptionsCode.EmitAssembly();
        var serviceOptionsAssembly = serviceOptionsCode.EmitAssembly();

        var authOptionsType = OptionsTypeFactory.MakeGenericType("AuthOptions", authOptionsAssembly);
        var serviceOptionsType = OptionsTypeFactory.MakeGenericType("ServiceOptions", serviceOptionsAssembly);

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration, authOptionsAssembly)
           .AddStronglyOptions(configuration, serviceOptionsAssembly)
           .BuildServiceProvider();

        var authOptions = (IOptions<object>) provider.GetRequiredService(authOptionsType);
        var serviceOptions = (IOptions<object>) provider.GetRequiredService(serviceOptionsType);

        // Assert
        authOptions
           .GetOptionsPropertyValue("AuthorityUrl")
           .Should()
           .NotBeNull();

        serviceOptions
           .GetOptionsPropertyValue("Url")
           .Should()
           .NotBeNull();
    }

    [Fact]
    public void Registers_all_options_in_T_assembly()
    {
        // Arrange
        JsonConfiguration configuration =
            """
            {
              "App": {
                "app.alias": "Strongly.Options"
              },
              "EnableExperimentalFeatures": true
            }
            """;

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptionsFromAssemblyContainingType<FeatureOptions>(configuration)
           .BuildServiceProvider();

        var appOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>();
        var featureOptions = provider.GetRequiredService<IOptions<FeatureOptions>>();

        // Assert
        appOptions
           .Value
           .Alias
           .Should()
           .Be("Strongly.Options");

        featureOptions
           .Value
           .EnableExperimentalFeatures
           .Should()
           .BeTrue();
    }

    [Fact]
    public void When_assembly_is_null_should_use_calling_assembly()
    {
        // Arrange
        JsonConfiguration configuration =
            """
            {
              "App": {
                "app.alias": "Strongly.Options"
              },
              "EnableExperimentalFeatures": true
            }
            """;

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration)
           .BuildServiceProvider();

        var appOptions = provider.GetRequiredService<IOptions<ApplicationOptions>>();
        var featureOptions = provider.GetRequiredService<IOptions<FeatureOptions>>();

        // Assert
        appOptions
           .Value
           .Alias
           .Should()
           .Be("Strongly.Options");

        featureOptions
           .Value
           .EnableExperimentalFeatures
           .Should()
           .BeTrue();
    }

    [Fact]
    public void Maps_multiple_top_level_options()
    {
        // Arrange
        DynamicCode code =
            """
            using Strongly.Options;
            
            [StronglyOptions(StronglyOptionsSection.Root)]
            public sealed record ApplicationOptions
            {
                public string Alias { get; init; } = default!;
            }

            [StronglyOptions(StronglyOptionsSection.Root)]
            public sealed record FeatureOptions
            {
                public bool EnableExperimentalFeatures { get; init; } = false;
            }

            """;

        JsonConfiguration configuration =
            """
            {
              "Alias": "Strongly.Options",
              "EnableExperimentalFeatures": true
            }
            """;

        var assembly = code.EmitAssembly();

        var appOptionsType = OptionsTypeFactory.MakeGenericType("ApplicationOptions", assembly);
        var featureOptionsType = OptionsTypeFactory.MakeGenericType("FeatureOptions", assembly);

        // Act
        var provider = new ServiceCollection()
           .AddStronglyOptions(configuration, assembly)
           .BuildServiceProvider();

        var authOptions = (IOptions<object>) provider.GetRequiredService(appOptionsType);
        var serviceOptions = (IOptions<object>) provider.GetRequiredService(featureOptionsType);

        // Assert
        authOptions
           .GetOptionsPropertyValue<string>("Alias")
           .Should()
           .Be("Strongly.Options");

        serviceOptions
           .GetOptionsPropertyValue<bool>("EnableExperimentalFeatures")
           .Should()
           .BeTrue();
    }
}

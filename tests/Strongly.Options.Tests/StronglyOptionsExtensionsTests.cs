using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Strongly.Options.Tests.Utils;

namespace Strongly.Options.Tests;

public class StronglyOptionsExtensionsTests
{
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
           .GetOptionsValue("AuthorityUrl")
           .Should()
           .NotBeNull();

        serviceOptions
           .GetOptionsValue("Url")
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
           .GetOptionsValue("AuthorityUrl")
           .Should()
           .NotBeNull();

        serviceOptions
           .GetOptionsValue("Url")
           .Should()
           .NotBeNull();
    }

    // Registers all options in assembly

    // Registers all options in T's assembly

    // Maps property values

    // when null should use calling assembly

    // works with root options

    // nested section with underscores or :

    // namings with dot

    // handles multiple top level options

    // uses default value when not specified

    // no setter
}

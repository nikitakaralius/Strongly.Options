using Microsoft.Extensions.DependencyInjection;
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

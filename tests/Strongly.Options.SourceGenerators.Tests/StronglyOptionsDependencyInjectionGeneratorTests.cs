using Microsoft.CodeAnalysis.CSharp;
using Strongly.Options.SourceGenerators.Tests.Tools;

namespace Strongly.Options.SourceGenerators.Tests;

public class StronglyOptionsDependencyInjectionGeneratorTests
{
    private const string AuthOptionsClassText =
        """
        using System;
        using System.Collections.Generic;

        using Strongly.Options;
        
        namespace Company.Application;

        [StronglyOptions("Auth")]
        public class AuthOptions
        {
            public Uri AuthorityUrl { get; set; }
        
            public IReadOnlyList<string> Audiences { get; set; }
        }

        """;

    private const string ServiceOptionsRecordText =
        """
        using System;
        using System.Collections.Generic;

        using Strongly.Options;

        [StronglyOptions("Service")]
        public sealed record ServiceOptions
        {
            public string Url { get; init; }
        
            public Guid Key { get; init; }
        
            public int RequestsPerHour { get; init; }
        }

        """;

    private const string RootOptionsText =
        """
        using Strongly.Options;

        [StronglyOptions(StronglyOptionsSection.Root)]
        public sealed record FeatureOptions
        {
            public bool EnableExperimentalFeatures { get; } = false;
        }

        """;

    private const string AssemblyInfoText =
        """
        using Strongly.Options;
        
        [assembly: StronglyOptionsModule("Tests")]
        
        """;

    [Fact]
    public async Task Registers_all_options_in_compilation()
    {
        // Arrange
        var generator = new StronglyOptionsDependencyInjectionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = DefaultCompilation.Create(
            nameof(Registers_all_options_in_compilation),
            [
                CSharpSyntaxTree.ParseText(AuthOptionsClassText),
                CSharpSyntaxTree.ParseText(ServiceOptionsRecordText)
            ]);

        // Act
        var result = driver.RunGenerators(compilation);

        // Assert
        await Verify(result)
           .UseDirectory(TestConstants.SnapshotsDirectory);
    }

    [Fact]
    public async Task Registers_root_options()
    {
        // Arrange
        var generator = new StronglyOptionsDependencyInjectionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = DefaultCompilation.Create(
            nameof(Registers_root_options),
            [
                CSharpSyntaxTree.ParseText(RootOptionsText)
            ]);

        // Act
        var result = driver.RunGenerators(compilation);

        // Assert
        await Verify(result)
           .UseDirectory(TestConstants.SnapshotsDirectory);
    }

    [Fact]
    public async Task Should_insert_module_in_extension_method_when_assembly_attribute_defined()
    {
        // Arrange
        var generator = new StronglyOptionsDependencyInjectionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = DefaultCompilation.Create(
            nameof(Registers_all_options_in_compilation),
            [
                CSharpSyntaxTree.ParseText(AuthOptionsClassText),
                CSharpSyntaxTree.ParseText(AssemblyInfoText)
            ]);

        // Act
        var result = driver.RunGenerators(compilation);

        // Assert
        await Verify(result)
           .UseDirectory(TestConstants.SnapshotsDirectory);
    }
}

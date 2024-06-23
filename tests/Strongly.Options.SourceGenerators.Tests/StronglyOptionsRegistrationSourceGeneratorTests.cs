using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Strongly.Options.SourceGenerators.Tests;

public class StronglyOptionsRegistrationSourceGeneratorTests
{
    private const string AuthOptionsRecordText =
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

    [Fact]
    public async Task GeneratesAddStronglyOptionsMethod()
    {
        // Arrange
        var generator = new StronglyOptionsRegistrationSourceGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);

        var compilation = CSharpCompilation.Create(
            nameof(StronglyOptionsRegistrationSourceGenerator),
            [
               CSharpSyntaxTree.ParseText(AuthOptionsRecordText),
               CSharpSyntaxTree.ParseText(ServiceOptionsRecordText)
            ],
            [
                MetadataReference.CreateFromFile(typeof(StronglyOptionsAttribute).Assembly.Location),
                ..Basic.Reference.Assemblies.Net80.References.All
            ],
            new(OutputKind.DynamicallyLinkedLibrary));

        // Act
        var result = driver
           .RunGenerators(compilation);

        // Assert
        await Verify(result).UseDirectory("Snapshots");
    }
}

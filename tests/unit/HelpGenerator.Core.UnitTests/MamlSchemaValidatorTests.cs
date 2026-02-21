using System;
using System.IO;
using Xunit;
using HelpGenerator.Core.HelpModel;
using HelpGenerator.Core.Maml;
using HelpGenerator.Core.Validation;

namespace HelpGenerator.Core.UnitTests;

public sealed class MamlSchemaValidatorTests
{
    [Fact]
    public void Validator_Loads_Schemas_And_Validates_Document()
    {
        // Arrange: schema folder copied to output by csproj ItemGroup
        var schemaFolder = Path.Combine(AppContext.BaseDirectory, "Schemas", "PSMaml");
        var schemaSet = MamlSchemaValidator.LoadSchemaSetFromFolder(schemaFolder);

        var module = new ModuleHelp
        {
            Name = "TestModule",
            Culture = "en-US",
            Commands =
            [
                new CommandHelp
                {
                    Name = "Get-Thing",
                    Synopsis = "Gets a thing.",
                    Description = "Gets a thing.",
                    Parameters = [],
                    Examples = [],
                    Links = []
                }
            ]
        };

        var renderer = new MamlRenderer();
        var doc = renderer.Render(module);

        // Act
        var result = MamlSchemaValidator.Validate(doc, schemaSet);

        // Assert (dump issues on failure)
        if (!result.IsValid)
        {
            var msg = string.Join(Environment.NewLine, result.Issues.Select(i =>
                $"{i.Severity}: L{i.LineNumber}:{i.LinePosition} {i.Message}"));

            throw new Xunit.Sdk.XunitException(msg + Environment.NewLine + doc);
        }
    }
}
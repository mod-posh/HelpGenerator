using HelpGenerator.Core.HelpModel;
using HelpGenerator.Core.Maml;
using Xunit;

namespace HelpGenerator.Core.UnitTests;

public class MamlRendererTests
{
    [Fact]
    public void Renders_Single_Command_To_Maml()
    {
        var module = new ModuleHelp
        {
            Name = "TestModule",
            Commands = new[]
            {
                new CommandHelp
                {
                    Name = "Get-Thing",
                    Synopsis = "Gets a thing",
                    Parameters = new[]
                    {
                        new ParameterHelp { Name = "Name", Description = "The name" }
                    }
                }
            }
        };

        var renderer = new MamlRenderer();
        var doc = renderer.Render(module);

        var xml = doc.ToString();

        Assert.Contains("Get-Thing", xml);
        Assert.Contains("Gets a thing", xml);
        Assert.Contains("Name", xml);
        Assert.Contains("The name", xml);
    }
}
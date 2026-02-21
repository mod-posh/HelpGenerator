using HelpGenerator.Core.HelpModel;
using Xunit;

namespace HelpGenerator.Core.UnitTests.HelpModel;

public class HelpModelTests
{
    [Fact]
    public void ModuleHelp_Defaults_AreSafe()
    {
        var module = new ModuleHelp { Name = "TestModule" };

        Assert.Equal("TestModule", module.Name);
        Assert.Equal("en-US", module.Culture);
        Assert.NotNull(module.Commands);
        Assert.Empty(module.Commands);
    }

    [Fact]
    public void CommandHelp_DefaultCollections_AreSafe()
    {
        var cmd = new CommandHelp { Name = "Get-Thing" };

        Assert.NotNull(cmd.Parameters);
        Assert.NotNull(cmd.Examples);
        Assert.NotNull(cmd.Links);

        Assert.Empty(cmd.Parameters);
        Assert.Empty(cmd.Examples);
        Assert.Empty(cmd.Links);
    }

    [Fact]
    public void ExampleHelp_RequiresCode()
    {
        var ex = new ExampleHelp { Code = "Get-Thing -Name Foo" };
        Assert.Equal("Get-Thing -Name Foo", ex.Code);
    }
}
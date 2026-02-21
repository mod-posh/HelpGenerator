using HelpGenerator.Powershell.Parsing;
using Xunit;

namespace HelpGenerator.Powershell.UnitTests;

public class CommentHelpParserTests
{
    [Fact]
    public void Parses_Example_ModulePath()
    {
        var parser = new CommentHelpParser();

        var modulePath = TestPaths.ExamplesWithCommentHelp();
        var module = parser.ParseModulePath(modulePath);

        Assert.NotNull(module);
        Assert.NotEmpty(module.Commands);
        Assert.All(module.Commands, c => Assert.False(string.IsNullOrWhiteSpace(c.Name)));
    }
}
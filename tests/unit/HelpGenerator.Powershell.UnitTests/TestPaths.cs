using System;
using System.IO;

namespace HelpGenerator.Powershell.UnitTests;

internal static class TestPaths
{
    public static string GetRepoRoot()
    {
        // Start from test output directory (bin/Debug/netX)
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null)
        {
            // Pick one reliable marker in your repo
            var sln = Path.Combine(dir.FullName, "src", "HelpGenerator.sln");
            if (File.Exists(sln))
                return dir.FullName;

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate repo root (src/HelpGenerator.sln not found in any parent directory).");
    }

    public static string ExamplesWithCommentHelp()
        => Path.Combine(GetRepoRoot(), "examples", "with-comment-help");
}
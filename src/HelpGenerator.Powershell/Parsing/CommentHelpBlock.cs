namespace HelpGenerator.Powershell.Parsing;

internal sealed record CommentHelpBlock(
    string? Synopsis,
    string? Description,
    IReadOnlyDictionary<string, string> ParameterDescriptions,
    IReadOnlyList<(string Code, string? Remarks)> Examples,
    string? Notes,
    IReadOnlyList<string> Links
);
using System.Management.Automation.Language;
using System.Text;
using System.Text.RegularExpressions;
using HelpGenerator.Core.HelpModel;

namespace HelpGenerator.Powershell.Parsing
{
    public sealed class CommentHelpParser
    {
        public ModuleHelp ParseModulePath(string modulePath, string culture = "en-US")
        {
            var files = PowerShellFileScanner.GetPowerShellFiles(modulePath);
            var moduleName = new DirectoryInfo(modulePath).Name;

            var commands = new List<CommandHelp>();

            foreach (var file in files)
            {
                commands.AddRange(ParseFile(file));
            }

            // Deterministic ordering
            commands = commands
                .OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new ModuleHelp
            {
                Name = moduleName,
                Culture = culture,
                Commands = commands
            };
        }

        public IReadOnlyList<CommandHelp> ParseFile(string filePath)
        {
            var ast = Parser.ParseFile(filePath, out Token[] tokens, out ParseError[] errors);

            // For M1: fail fast if the file doesn't parse
            if (errors is { Length: > 0 })
            {
                var msg = string.Join(Environment.NewLine, errors.Select(e => $"{e.Extent.File}:{e.Extent.StartLineNumber}:{e.Extent.StartColumnNumber} {e.Message}"));
                throw new InvalidOperationException($"PowerShell parse errors in '{filePath}':{Environment.NewLine}{msg}");
            }

            var tokenList = tokens.ToList();

            var functions = ast.FindAll(a => a is FunctionDefinitionAst, searchNestedScriptBlocks: true)
                .Cast<FunctionDefinitionAst>()
                .ToList();

            var results = new List<CommandHelp>();

            foreach (var func in functions)
            {
                var helpText = TryGetHelpCommentText(func, tokenList);
                var block = helpText is null ? null : ParseHelpBlock(helpText);

                results.Add(new CommandHelp
                {
                    Name = func.Name,
                    Synopsis = block?.Synopsis,
                    Description = block?.Description,
                    Notes = block?.Notes,
                    Links = (block?.Links ?? Array.Empty<string>())
                        .Select(u => new LinkHelp { Uri = u })
                        .ToList(),
                    Parameters = GetFunctionParameters(func)
                        .Select(p =>
                        {
                            var pName = p.Name.VariablePath.UserPath;

                            string? pDesc = null;
                            if (block is not null)
                            {
                                block.ParameterDescriptions.TryGetValue(pName, out var d);
                                pDesc = d;
                            }

                            // Try to capture type name if specified: [string]$Name
                            var typeName = p.StaticType?.FullName;
                            if (string.IsNullOrWhiteSpace(typeName) || typeName == "System.Object")
                            {
                                // StaticType is often object; use the extent text if attributes exist
                                // (optional, can remove if you want super-minimal)
                                typeName = null;
                            }

                            return new ParameterHelp
                            {
                                Name = pName,
                                TypeName = typeName,
                                Description = pDesc
                            };
                        })
                        .OrderBy(p => p.Position ?? int.MaxValue)
                        .ThenBy(p => p.Name, StringComparer.OrdinalIgnoreCase)
                        .ToList(),
                    Examples = (block?.Examples ?? Array.Empty<(string Code, string? Remarks)>())
                        .Select(ex => new ExampleHelp { Code = ex.Code, Remarks = ex.Remarks })
                        .ToList()
                });
            }

            return results;
        }

        private static string NormalizeHelpText(string text)
        {
            // Normalize line endings first
            var lines = text.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

            // Trim trailing whitespace
            for (var i = 0; i < lines.Length; i++)
                lines[i] = lines[i].TrimEnd();

            // Compute common leading indentation across non-empty lines
            var nonEmpty = lines.Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            if (nonEmpty.Count == 0)
                return string.Empty;

            int CommonIndent(string s)
            {
                var count = 0;
                while (count < s.Length && char.IsWhiteSpace(s[count]))
                    count++;
                return count;
            }

            var minIndent = nonEmpty.Min(CommonIndent);

            if (minIndent > 0)
            {
                for (var i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length >= minIndent)
                        lines[i] = lines[i].Substring(minIndent);
                }
            }

            return string.Join("\n", lines).Trim();
        }

        private static string? TryGetHelpCommentText(FunctionDefinitionAst func, List<Token> tokens)
        {
            return TryGetHelpCommentTextImmediatelyBefore(func, tokens)
                ?? TryGetHelpCommentTextInsideFunction(func, tokens);
        }

        private static string? TryGetHelpCommentTextInsideFunction(FunctionDefinitionAst func, List<Token> tokens)
        {
            // Find comment tokens within the function extent.
            // Prefer the first comment-help block in source order (deterministic).
            foreach (var token in tokens)
            {
                if (token.Kind != TokenKind.Comment)
                    continue;

                if (token.Extent.StartOffset < func.Extent.StartOffset || token.Extent.EndOffset > func.Extent.EndOffset)
                    continue;

                var stripped = StripBlockCommentDelimiters(token.Text).Trim();
                if (LooksLikeCommentHelp(stripped))
                    return stripped;
            }

            return null;
        }

        private static bool LooksLikeCommentHelp(string commentText)
        {
            // A simple heuristic to avoid picking up random comments.
            // This catches the standard comment-help directives used in your sample.
            return commentText.Contains(".SYNOPSIS", StringComparison.OrdinalIgnoreCase)
                || commentText.Contains(".DESCRIPTION", StringComparison.OrdinalIgnoreCase)
                || commentText.Contains(".PARAMETER", StringComparison.OrdinalIgnoreCase)
                || commentText.Contains(".EXAMPLE", StringComparison.OrdinalIgnoreCase)
                || commentText.Contains(".NOTES", StringComparison.OrdinalIgnoreCase)
                || commentText.Contains(".LINK", StringComparison.OrdinalIgnoreCase);
        }

        private static string StripBlockCommentDelimiters(string text)
        {
            if (text.StartsWith("<#", StringComparison.Ordinal) && text.EndsWith("#>", StringComparison.Ordinal))
            {
                return text.Substring(2, text.Length - 4);
            }

            // Line comments will come through as "# ..." — we leave them as-is for now.
            return text;
        }
        
        private static string? TryGetHelpCommentTextImmediatelyBefore(FunctionDefinitionAst func, List<Token> tokens)
        {
            var funcStartLine = func.Extent.StartLineNumber;

            // Candidate: closest comment token that ends on the previous line.
            var candidate = tokens
                .Where(t => t.Kind == TokenKind.Comment && t.Extent.EndLineNumber <= funcStartLine - 1)
                .OrderByDescending(t => t.Extent.EndLineNumber)
                .ThenByDescending(t => t.Extent.EndColumnNumber)
                .FirstOrDefault();

            if (candidate is null)
                return null;

            // Require comment ends exactly on line before function starts (strict for M1)
            if (candidate.Extent.EndLineNumber != funcStartLine - 1)
                return null;

            // Ensure no non-comment tokens between candidate and function start.
            var between = tokens.Where(t =>
                t.Extent.StartOffset >= candidate.Extent.EndOffset &&
                t.Extent.EndOffset <= func.Extent.StartOffset);

            if (between.Any(t => t.Kind != TokenKind.Comment))
                return null;

            var stripped = StripBlockCommentDelimiters(candidate.Text).Trim();
            return LooksLikeCommentHelp(stripped) ? stripped : null;
        }

        private static IEnumerable<ParameterAst> GetFunctionParameters(FunctionDefinitionAst func)
        {
            // Header-style parameters: function Foo($a) { }
            if (func.Parameters is { Count: > 0 })
                return func.Parameters;

            // Body-style param block: function Foo { param($a) ... }
            var paramBlock = func.Body?.ParamBlock;
            if (paramBlock?.Parameters is { Count: > 0 })
                return paramBlock.Parameters;

            return Enumerable.Empty<ParameterAst>();
        }

        private static CommentHelpBlock ParseHelpBlock(string raw)
        {
            // Very small, deterministic parser:
            // Split into lines; treat directives ".SYNOPSIS", ".DESCRIPTION", ".PARAMETER X", ".EXAMPLE", ".NOTES", ".LINK"
            // as section headers; accumulate until next directive.

            var lines = raw.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

            string? synopsis = null;
            string? description = null;
            string? notes = null;
            var links = new List<string>();
            var paramDescs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var examples = new List<(string Code, string? Remarks)>();

            string? currentSection = null;
            string? currentParam = null;
            var buffer = new StringBuilder();

            void Flush()
            {
                var content = NormalizeHelpText(buffer.ToString());
                buffer.Clear();

                if (string.IsNullOrEmpty(currentSection))
                    return;

                switch (currentSection)
                {
                    case "SYNOPSIS":
                        synopsis = string.IsNullOrEmpty(content) ? synopsis : content;
                        break;
                    case "DESCRIPTION":
                        description = string.IsNullOrEmpty(content) ? description : content;
                        break;
                    case "NOTES":
                        notes = string.IsNullOrEmpty(content) ? notes : content;
                        break;
                    case "LINK":
                        if (!string.IsNullOrEmpty(content))
                            links.Add(content);
                        break;
                    case "PARAMETER":
                        if (!string.IsNullOrEmpty(currentParam) && !string.IsNullOrEmpty(content))
                            paramDescs[currentParam] = content;
                        break;
                    case "EXAMPLE":
                        if (!string.IsNullOrEmpty(content))
                        {
                            // naive split: first line(s) are "code", rest are remarks
                            var parts = content.Split('\n');
                            var code = parts.FirstOrDefault()?.Trim() ?? "";
                            var remarksPart = string.Join('\n', parts.Skip(1)).Trim();
                            examples.Add((code, string.IsNullOrEmpty(remarksPart) ? null : remarksPart));
                        }
                        break;
                }
            }

            var directive = new Regex(@"^\s*\.(SYNOPSIS|DESCRIPTION|NOTES|LINK|EXAMPLE|PARAMETER)\b(?:\s+(.+))?\s*$",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            foreach (var line in lines)
            {
                var m = directive.Match(line);
                if (m.Success)
                {
                    Flush();

                    currentSection = m.Groups[1].Value.ToUpperInvariant();
                    currentParam = currentSection == "PARAMETER" ? (m.Groups[2].Value?.Trim() ?? "") : null;
                    continue;
                }

                buffer.AppendLine(line);
            }

            Flush();

            return new CommentHelpBlock(
                synopsis,
                description,
                paramDescs,
                examples,
                notes,
                links
            );
        }
    }
}

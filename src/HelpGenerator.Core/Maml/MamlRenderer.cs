using System.Xml.Linq;
using HelpGenerator.Core.HelpModel;

namespace HelpGenerator.Core.Maml
{
    public sealed class MamlRenderer
    {
        private static readonly XNamespace nsHelp = "http://msh";
        private static readonly XNamespace nsCommand = "http://schemas.microsoft.com/maml/dev/command/2004/10";
        private static readonly XNamespace nsMaml = "http://schemas.microsoft.com/maml/2004/10";
        private static readonly XNamespace nsDev = "http://schemas.microsoft.com/maml/dev/2004/10";

        public XDocument Render(ModuleHelp module)
        {
            var root = new XElement(nsHelp + "helpItems",
                new XAttribute("schema", "maml")
            );

            foreach (var command in module.Commands.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
            {
                root.Add(RenderCommand(command));
            }

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                root
            );
        }

        private IEnumerable<XElement> RenderParas(string? text)
        {
            // Schema expects one or more <maml:para>. We’ll always emit at least one.
            if (string.IsNullOrWhiteSpace(text))
                return new[] { new XElement(nsMaml + "para", string.Empty) };

            // Preserve author formatting; split into paragraphs on blank lines.
            var normalized = text.Replace("\r\n", "\n").Replace("\r", "\n").Trim();
            var blocks = normalized.Split("\n\n", StringSplitOptions.None)
                .Select(b => b.Trim())
                .Where(b => b.Length > 0)
                .ToList();

            if (blocks.Count == 0)
                return new[] { new XElement(nsMaml + "para", string.Empty) };

            return blocks.Select(b => new XElement(nsMaml + "para", b));
        }

        private XElement RenderCommand(CommandHelp command)
        {
            var cmd = new XElement(nsCommand + "command",
                new XAttribute(XNamespace.Xmlns + "command", nsCommand),
                new XAttribute(XNamespace.Xmlns + "maml", nsMaml),
                new XAttribute(XNamespace.Xmlns + "dev", nsDev)
            );

            // details (name -> description -> synonyms -> copyright)
            var (verb, noun) = SplitVerbNoun(command.Name);

            cmd.Add(new XElement(nsCommand + "details",
                new XElement(nsCommand + "name", command.Name),
                new XElement(nsMaml + "description", RenderParas(command.Synopsis)),
                new XElement(nsCommand + "synonyms",
                    new XElement(nsCommand + "synonym", command.Name)
                ),
                new XElement(nsMaml + "copyright",
                    new XElement(nsMaml + "para", string.Empty)
                ),
                new XElement(nsCommand + "verb", verb),
                new XElement(nsCommand + "noun", noun),
                new XElement(nsDev + "version", "1.0.0.0")
            ));

            // top-level description (must appear before syntax)
            cmd.Add(new XElement(nsMaml + "description",
                RenderParas(command.Description ?? command.Synopsis)
            ));

            cmd.Add(RenderSyntax(command));

            if (command.Parameters.Count > 0)
                cmd.Add(RenderParameters(command));

            cmd.Add(RenderInputTypes());
            cmd.Add(RenderReturnValues());
            cmd.Add(RenderTerminatingErrors());
            cmd.Add(RenderNonTerminatingErrors());

            // required later in sequence
            cmd.Add(RenderAlertSet());
            cmd.Add(RenderExamples(command));
            cmd.Add(RenderRelatedLinks(command));

            return cmd;
        }

        private XElement RenderSyntax(CommandHelp command)
        {
            var syntax = new XElement(nsCommand + "syntax");

            var syntaxItem = new XElement(nsCommand + "syntaxItem",
                new XElement(nsMaml + "name", command.Name) // <-- maml:name, not command:name
            );

            foreach (var param in command.Parameters.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                syntaxItem.Add(
                    new XElement(nsCommand + "parameter",
                        new XAttribute("required", "false"),
                        new XAttribute("position", "named"),
                        new XAttribute("pipelineInput", "false"),
                        new XAttribute("globbing", "false"),
                        new XElement(nsMaml + "name", param.Name),
                        new XElement(nsCommand + "parameterValue",
                            new XAttribute("required", "false"),
                            param.TypeName ?? "System.Object"
                        )
                    )
                );
            }

            syntax.Add(syntaxItem);
            return syntax;
        }

        private XElement RenderParameters(CommandHelp command)
        {
            var parameters = new XElement(nsCommand + "parameters");

            foreach (var param in command.Parameters.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                var p = new XElement(nsCommand + "parameter",
                    new XAttribute("required", "false"),
                    new XAttribute("position", "named"),
                    new XAttribute("pipelineInput", "false"),
                    new XAttribute("globbing", "false"),
                    new XElement(nsMaml + "name", param.Name),
                    new XElement(nsMaml + "description", RenderParas(param.Description)),
                    new XElement(nsCommand + "parameterValue",
                        new XAttribute("required", "false"),
                        param.TypeName ?? "System.Object"
                    ),
                    RenderDevType(param.TypeName ?? "System.Object")
                    );

                parameters.Add(p);
            }

            return parameters;
        }

        private XElement RenderExamples(CommandHelp command)
        {
            var examples = new XElement(nsCommand + "examples");

            // Always include at least one example (schema requires it)
            var list = command.Examples.Count == 0
                ? new[] { new ExampleHelp { Code = string.Empty, Remarks = string.Empty } }
                : command.Examples;

            int index = 1;

            foreach (var ex in list)
            {
                examples.Add(
                    new XElement(nsCommand + "example",
                        new XElement(nsMaml + "title", $"Example {index++}"),
                        new XElement(nsDev + "code", ex.Code ?? string.Empty),
                        new XElement(nsDev + "remarks",
                            new XElement(nsMaml + "para", ex.Remarks ?? string.Empty)
                        )
                    )
                );
            }

            return examples;
        }

        private XElement RenderInputTypes()
        {
            return new XElement(nsCommand + "inputTypes",
                new XElement(nsCommand + "inputType",
                    RenderDevType("System.Object"),
                    new XElement(nsMaml + "description",
                        new XElement(nsMaml + "para", string.Empty)
                    )
                )
            );
        }

        private XElement RenderReturnValues()
        {
            return new XElement(nsCommand + "returnValues",
                new XElement(nsCommand + "returnValue",
                    RenderDevType("System.Object"),
                    new XElement(nsMaml + "description",
                        new XElement(nsMaml + "para", string.Empty)
                    )
                )
            );
        }

        private XElement RenderTerminatingErrors()
        {
            return new XElement(nsCommand + "terminatingErrors",
                new XElement(nsCommand + "terminatingError",
                    RenderDevType("System.Exception"),
                    new XElement(nsMaml + "description",
                        new XElement(nsMaml + "para", string.Empty)
                    ),
                    new XElement(nsCommand + "category", "NotSpecified"),
                    new XElement(nsCommand + "errorID", string.Empty),
                    RenderRecommendedAction(),
                    RenderTargetObjectType("System.Object")
                )
            );
        }

        private XElement RenderNonTerminatingErrors()
        {
            return new XElement(nsCommand + "nonTerminatingErrors",
                new XElement(nsCommand + "nonTerminatingError",
                    RenderDevType("System.Exception"),
                    new XElement(nsMaml + "description",
                        new XElement(nsMaml + "para", string.Empty)
                    ),
                    new XElement(nsCommand + "category", "NotSpecified"),
                    new XElement(nsCommand + "errorID", string.Empty),
                    RenderRecommendedAction(),
                    RenderTargetObjectType("System.Object")
                )
            );
        }

        private XElement RenderAlertSet()
        {
            // Minimal valid alertSet with one empty alert
            return new XElement(nsMaml + "alertSet",
                new XElement(nsMaml + "alert",
                    new XElement(nsMaml + "para", string.Empty)
                )
            );
        }

        private XElement RenderRelatedLinks(CommandHelp command)
        {
            var rl = new XElement(nsMaml + "relatedLinks");

            if (command.Links.Count == 0)
            {
                rl.Add(new XElement(nsMaml + "navigationLink",
                    new XElement(nsMaml + "linkText", string.Empty),
                    new XElement(nsMaml + "uri", string.Empty)
                ));
                return rl;
            }

            foreach (var link in command.Links)
            {
                rl.Add(new XElement(nsMaml + "navigationLink",
                    new XElement(nsMaml + "linkText", link.Uri),
                    new XElement(nsMaml + "uri", link.Uri)
                ));
            }

            return rl;
        }

        private XElement RenderDevType(string typeName)
        {
            return new XElement(nsDev + "type",
                new XElement(nsMaml + "name", typeName),
                new XElement(nsMaml + "uri", string.Empty)
            );
        }

        private XElement RenderRecommendedAction()
        {
            // recommendedAction is "block content" directly (para/list/table/etc) — no <maml:description>
            return new XElement(nsCommand + "recommendedAction",
                new XElement(nsMaml + "para", string.Empty)
            );
        }

        private XElement RenderTargetObjectType(string typeName = "System.Object")
        {
            // targetObjectType expects maml:name + maml:uri, NOT dev:type
            return new XElement(nsCommand + "targetObjectType",
                new XElement(nsMaml + "name", typeName),
                new XElement(nsMaml + "uri", string.Empty)
            );
        }

        private static (string Verb, string Noun) SplitVerbNoun(string name)
        {
            var idx = name.IndexOf('-', StringComparison.Ordinal);
            if (idx <= 0 || idx == name.Length - 1)
                return (name, string.Empty);

            return (name.Substring(0, idx), name.Substring(idx + 1));
        }
    }
}

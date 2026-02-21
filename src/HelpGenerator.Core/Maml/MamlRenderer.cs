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
            var root = new XElement(nsHelp + "helpItems");

            foreach (var command in module.Commands.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
            {
                root.Add(RenderCommand(command));
            }

            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                root
            );
        }

        private XElement RenderCommand(CommandHelp command)
        {
            var cmd = new XElement(nsCommand + "command",
                new XAttribute(XNamespace.Xmlns + "command", nsCommand),
                new XAttribute(XNamespace.Xmlns + "maml", nsMaml),
                new XAttribute(XNamespace.Xmlns + "dev", nsDev)
            );

            cmd.Add(
                new XElement(nsCommand + "details",
                    new XElement(nsCommand + "name", command.Name),
                    new XElement(nsMaml + "description",
                        new XElement(nsMaml + "para", command.Synopsis ?? string.Empty)
                    )
                )
            );

            cmd.Add(RenderSyntax(command));
            cmd.Add(RenderParameters(command));
            cmd.Add(RenderExamples(command));

            return cmd;
        }

        private XElement RenderSyntax(CommandHelp command)
        {
            var syntax = new XElement(nsCommand + "syntax");

            var syntaxItem = new XElement(nsCommand + "syntaxItem",
                new XElement(nsCommand + "name", command.Name)
            );

            foreach (var param in command.Parameters.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                syntaxItem.Add(
                    new XElement(nsCommand + "parameter",
                        new XAttribute("required", "false"),
                        new XAttribute("position", "named"),
                        new XAttribute("pipelineInput", "false"),
                        new XAttribute("globbing", "false"),
                        new XElement(nsMaml + "name", param.Name)
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
                parameters.Add(
                    new XElement(nsCommand + "parameter",
                        new XAttribute("required", "false"),
                        new XAttribute("position", "named"),
                        new XAttribute("pipelineInput", "false"),
                        new XAttribute("globbing", "false"),
                        new XElement(nsMaml + "name", param.Name),
                        new XElement(nsMaml + "description",
                            new XElement(nsMaml + "para", param.Description ?? string.Empty)
                        )
                    )
                );
            }

            return parameters;
        }

        private XElement RenderExamples(CommandHelp command)
        {
            var examples = new XElement(nsCommand + "examples");

            int index = 1;

            foreach (var example in command.Examples)
            {
                examples.Add(
                    new XElement(nsCommand + "example",
                        new XElement(nsCommand + "title", $"Example {index++}"),
                        new XElement(nsCommand + "code", example.Code),
                        new XElement(nsCommand + "remarks",
                            new XElement(nsMaml + "para", example.Remarks ?? string.Empty)
                        )
                    )
                );
            }

            return examples;
        }
    }
}

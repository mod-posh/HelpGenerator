// NOTE (ADR-001 M1):
// This canonical model is the contract between parsing and rendering.
// It must remain PowerShell-agnostic and format-agnostic.
namespace HelpGenerator.Core.HelpModel
{
    public sealed record ModuleHelp
    {
        public required string Name { get; init; }

        /// <summary>
        /// Culture used for localized help output folders (e.g. en-US).
        /// </summary>
        public string Culture { get; init; } = "en-US";

        public string? Description { get; init; }

        /// <summary>
        /// Commands included in this module help set.
        /// </summary>
        public IReadOnlyList<CommandHelp> Commands { get; init; } = Array.Empty<CommandHelp>();
    }
}
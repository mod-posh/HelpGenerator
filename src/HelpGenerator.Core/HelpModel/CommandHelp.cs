namespace HelpGenerator.Core.HelpModel
{
    public sealed record CommandHelp
    {
        public required string Name { get; init; }

        public string? Synopsis { get; init; }
        public string? Description { get; init; }

        /// <summary>
        /// Parameter help entries. Renderer/validator can impose ordering rules later.
        /// </summary>
        public IReadOnlyList<ParameterHelp> Parameters { get; init; } = Array.Empty<ParameterHelp>();

        /// <summary>
        /// Usage examples, in source order.
        /// </summary>
        public IReadOnlyList<ExampleHelp> Examples { get; init; } = Array.Empty<ExampleHelp>();

        public string? Notes { get; init; }

        public IReadOnlyList<LinkHelp> Links { get; init; } = Array.Empty<LinkHelp>();
    }
}

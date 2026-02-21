namespace HelpGenerator.Core.HelpModel
{
    public sealed record ExampleHelp
    {
        public string? Title { get; init; }

        /// <summary>
        /// Example command text (code).
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Example remarks/explanation.
        /// </summary>
        public string? Remarks { get; init; }
    }
}

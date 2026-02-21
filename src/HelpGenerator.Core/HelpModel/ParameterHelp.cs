namespace HelpGenerator.Core.HelpModel
{
    public sealed record ParameterHelp
    {
        public required string Name { get; init; }

        /// <summary>
        /// Type name string as displayed (e.g. System.String, string, PSCredential).
        /// Optional for M1.
        /// </summary>
        public string? TypeName { get; init; }

        public bool? Required { get; init; }
        public int? Position { get; init; }

        public string? Description { get; init; }
    }
}

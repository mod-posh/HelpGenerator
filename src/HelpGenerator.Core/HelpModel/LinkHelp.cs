namespace HelpGenerator.Core.HelpModel
{
    public sealed record LinkHelp
    {
        public string? Text { get; init; }
        public required string Uri { get; init; }
    }
}

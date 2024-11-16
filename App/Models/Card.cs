namespace PokemonTCGPocketScrapper.Models
{
    internal class Card
    {
        public required string Name { get; set; }
        public string? Stage { get; set; }
        public string? PreviousForm { get; set; }
        public required string Set { get; set; }
        public required string Pack { get; set; }
        public required Rate Rate { get; set; }
        public required int Number { get; set; }
        public required CardDetails Details { get; set; }
    }
}
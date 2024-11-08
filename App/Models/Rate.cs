namespace PokemonTCGPocketScrapper.Models
{
    internal class Rate
    {
        public string? BasicSlots { get; set; }
        public string? FourthSlot { get; set; }
        public string? FifthSlot { get; set; }
        public required string Rarity { get; set; } = "Unknown";
    }
}
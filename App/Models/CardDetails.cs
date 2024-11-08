namespace PokemonTCGPocketScrapper.Models
{
    internal class CardDetails
    {
        public required string Type { get; set; }
        public int? HP { get; set; }
        public List<Attack>? Attacks { get; set; }
        public string? Weakness { get; set; }
        public int? RetreatCost { get; set; }
        public List<Ability>? Abilities { get; set; }
        public string? Effect { get; set; }
    }
}
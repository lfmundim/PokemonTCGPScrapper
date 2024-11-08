namespace PokemonTCGPocketScrapper.Models
{
    internal class Attack
    {
        public required string Name { get; set; }
        public List<string> EnergyCost { get; set; } = [];
        public int MinDamage { get; set; }
        public int MaxDamage { get; set; }
        public bool IsVariableDamage { get; set; }
        public int? CoinFlips { get; set; } // Optional, for attacks using coin flips
        public string? AdditionalText { get; set; }
    }
}
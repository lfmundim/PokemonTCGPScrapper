using System.Text.Json.Serialization;

namespace PokemonTCGPocketScrapper
{
    public class AppSettings
    {
        [JsonPropertyName("collections")]
        public List<Collection> Collections { get; set; } = [];
    }

    public class Collection
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("pokemonCardCount")]
        public long PokemonCardCount { get; set; }

        [JsonPropertyName("uniqueCardCount")]
        public long UniqueCardCount { get; set; }

        [JsonPropertyName("cardCount")]
        public long CardCount { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; } = null!;
    }
}

using FluentAssertions;
using PokemonTCGPScrapper;
using Xunit;

namespace PokemonTCGPocketScrapper.Tests
{
    public class SerebiiScrapperTest
    {
        private static readonly Collection Collection = new()
        {
            CardCount = 1,
            Name = "Test",
        };

        [Fact]
        public async Task Scrapping_RegularPokemon_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiBulbasaur.html");

            // Act
            var cards = await SerebiiScrapper.RunAsync([Collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var pokemon = cards[0];
            pokemon.Name.Should().Be("Bulbasaur");
            pokemon.Number.Should().Be(1);
            pokemon.Pack.Should().Be("Mewtwo");
            pokemon.Set.Should().Be("Genetic Apex");
            pokemon.Rate.BasicSlots.Should().Be("2%");
            pokemon.Rate.FourthSlot.Should().BeNull();
            pokemon.Rate.FifthSlot.Should().BeNull();
            pokemon.Rate.Rarity.Should().Be("1 Diamond");
            pokemon.Details.Type.Should().Be("grass");
            pokemon.Details.HP.Should().Be(70);
            pokemon.Details.Weakness.Should().Be("fire");
            pokemon.Details.RetreatCost.Should().Be(1);
            pokemon.Details.Abilities.Should().BeEmpty();
            pokemon.Details.Attacks.Should().HaveCount(1);
            pokemon.Details.Attacks![0].Name.Should().Be("Vine Whip");
            pokemon.Details.Attacks[0].MinDamage.Should().Be(20);
            pokemon.Details.Attacks[0].MaxDamage.Should().Be(20);
            pokemon.Details.Attacks[0].AdditionalText.Should().BeNull();
            pokemon.Details.Attacks[0].EnergyCost.Should().BeEquivalentTo(["Grass","Colorless"]);
        }

        [Fact]
        public async Task Scrapping_PokemonWithMultipleAttacks_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiVenusaurEx.html");

            // Act
            var cards = await SerebiiScrapper.RunAsync([Collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var pokemon = cards[0];
            pokemon.Name.Should().Be("Venusaur ex");
            pokemon.Number.Should().Be(4);
            pokemon.Pack.Should().Be("Mewtwo");
            pokemon.Set.Should().Be("Genetic Apex");
            pokemon.Rate.BasicSlots.Should().BeNull();
            pokemon.Rate.FourthSlot.Should().Be("0.333%");
            pokemon.Rate.FifthSlot.Should().Be("1.332%");
            pokemon.Rate.Rarity.Should().Be("4 Diamonds");
            pokemon.Details.Type.Should().Be("grass");
            pokemon.Details.HP.Should().Be(190);
            pokemon.Details.Weakness.Should().Be("fire");
            pokemon.Details.RetreatCost.Should().Be(3);
            pokemon.Details.Abilities.Should().BeEmpty();
            pokemon.Details.Attacks.Should().HaveCount(2);
            pokemon.Details.Attacks![0].Name.Should().Be("Razor Leaf");
            pokemon.Details.Attacks[0].MinDamage.Should().Be(60);
            pokemon.Details.Attacks[0].MaxDamage.Should().Be(60);
            pokemon.Details.Attacks[0].AdditionalText.Should().BeNull();
            pokemon.Details.Attacks[0].EnergyCost.Should().BeEquivalentTo(["Grass","Colorless","Colorless"]);
            pokemon.Details.Attacks![1].Name.Should().Be("Giant Bloom");
            pokemon.Details.Attacks[1].MinDamage.Should().Be(100);
            pokemon.Details.Attacks[1].MaxDamage.Should().Be(100);
            pokemon.Details.Attacks[1].AdditionalText.Should().Be("Heal 30 damage from this Pokémon");
            pokemon.Details.Attacks[1].EnergyCost.Should().BeEquivalentTo(["Grass","Grass","Colorless","Colorless"]);
        }

        [Fact] 
        public async Task Scrapping_PokemonWithAbility_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiButterfree.html");

            // Act
            var cards = await SerebiiScrapper.RunAsync([Collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var pokemon = cards[0];
            pokemon.Name.Should().Be("Butterfree");
            pokemon.Number.Should().Be(7);
            pokemon.Pack.Should().Be("Pikachu");
            pokemon.Set.Should().Be("Genetic Apex");
            pokemon.Rate.BasicSlots.Should().BeNull();
            pokemon.Rate.FourthSlot.Should().Be("0.357%");
            pokemon.Rate.FifthSlot.Should().Be("1.428%");
            pokemon.Rate.Rarity.Should().Be("3 Diamonds");
            pokemon.Details.Type.Should().Be("grass");
            pokemon.Details.HP.Should().Be(120);
            pokemon.Details.Weakness.Should().Be("fire");
            pokemon.Details.RetreatCost.Should().Be(1);
            pokemon.Details.Abilities.Should().HaveCount(1);
            pokemon.Details.Abilities![0].Name.Should().Be("Powder Heal");
            pokemon.Details.Abilities![0].Description.Should().Be("Once during your turn, you may heal 20 damage from each of your Pokémon");
            pokemon.Details.Attacks.Should().HaveCount(1);
            pokemon.Details.Attacks![0].Name.Should().Be("Gust");
            pokemon.Details.Attacks[0].MinDamage.Should().Be(60);
            pokemon.Details.Attacks[0].MaxDamage.Should().Be(60);
            pokemon.Details.Attacks[0].AdditionalText.Should().BeNull();
            pokemon.Details.Attacks[0].EnergyCost.Should().BeEquivalentTo(["Grass","Colorless","Colorless"]);
        }

        [Fact] 
        public async Task Scrapping_Trainer_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiHelixFossil.html");

            // Act
            var cards = await SerebiiScrapper.RunAsync([Collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var fossil = cards[0];
            fossil.Name.Should().Be("Helix Fossil");
            fossil.Number.Should().Be(216);
            fossil.Pack.Should().Be("Pikachu");
            fossil.Set.Should().Be("Genetic Apex");
            fossil.Rate.BasicSlots.Should().Be("2%");
            fossil.Rate.FourthSlot.Should().BeNull();
            fossil.Rate.FifthSlot.Should().BeNull();
            fossil.Rate.Rarity.Should().Be("1 Diamond");
            fossil.Details.Type.Should().Be("Trainer");
            fossil.Details.Effect.Should().NotBeNullOrWhiteSpace();
            fossil.Details.Weakness.Should().BeNull();
            fossil.Details.RetreatCost.Should().BeNull();
            fossil.Details.Abilities.Should().BeNull();
            fossil.Details.Attacks.Should().BeNull();
        }

        [Fact] 
        public async Task Scrapping_Item_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiPotion.html");

            // Act
            var cards = await SerebiiScrapper.RunAsync([Collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var fossil = cards[0];
            fossil.Name.Should().Be("Potion");
            fossil.Number.Should().Be(1);
            fossil.Pack.Should().Be("Unknown");
            fossil.Set.Should().Be("Promo");
            fossil.Rate.BasicSlots.Should().BeNull();
            fossil.Rate.FourthSlot.Should().BeNull();
            fossil.Rate.FifthSlot.Should().BeNull();
            fossil.Rate.Rarity.Should().Be("Unknown");
            fossil.Details.Type.Should().Be("Trainer");
            fossil.Details.Effect.Should().NotBeNullOrWhiteSpace();
            fossil.Details.Weakness.Should().BeNull();
            fossil.Details.RetreatCost.Should().BeNull();
            fossil.Details.Abilities.Should().BeNull();
            fossil.Details.Attacks.Should().BeNull();
        }
    }
}
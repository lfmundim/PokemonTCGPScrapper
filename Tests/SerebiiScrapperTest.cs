using FluentAssertions;
using PokemonTCGPScrapper;
using Xunit;

namespace PokemonTCGPocketScrapper.Tests
{
    public class SerebiiScrapperTest
    {
        [Fact] // Simple pokémon with 1 move
        public async Task Scrapping_Bulbasaur_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiBulbasaur.html");

            Collection collection = new()
            {
                CardCount = 1,
                Name = "Test",
            };

            // Act
            var cards = await SerebiiScrapper.RunAsync([collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var bulbasaur = cards[0];
            bulbasaur.Name.Should().Be("Bulbasaur");
            bulbasaur.Number.Should().Be(1);
            bulbasaur.Pack.Should().Be("Mewtwo");
            bulbasaur.Set.Should().Be("Genetic Apex");
            bulbasaur.Rate.BasicSlots.Should().Be("2%");
            bulbasaur.Rate.FourthSlot.Should().BeNull();
            bulbasaur.Rate.FifthSlot.Should().BeNull();
            bulbasaur.Rate.Rarity.Should().Be("1 Diamond");
            bulbasaur.Details.Type.Should().Be("grass");
            bulbasaur.Details.HP.Should().Be(70);
            bulbasaur.Details.Weakness.Should().Be("fire");
            bulbasaur.Details.RetreatCost.Should().Be(1);
            bulbasaur.Details.Abilities.Should().BeEmpty();
            bulbasaur.Details.Attacks.Should().HaveCount(1);
            bulbasaur.Details.Attacks![0].Name.Should().Be("Vine Whip");
            bulbasaur.Details.Attacks[0].MinDamage.Should().Be(20);
            bulbasaur.Details.Attacks[0].MaxDamage.Should().Be(20);
            bulbasaur.Details.Attacks[0].AdditionalText.Should().BeNull();
            bulbasaur.Details.Attacks[0].EnergyCost.Should().BeEquivalentTo(["Grass","Colorless"]);
        }

        [Fact] // Pokémon with 2 moves
        public async Task Scrapping_VenusaurEx_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetHttpClient("SerebiiVenusaurEx.html");

            Collection collection = new()
            {
                CardCount = 1,
                Name = "Test",
            };

            // Act
            var cards = await SerebiiScrapper.RunAsync([collection], httpClient);

            // Assert
            cards.Should().HaveCount(1);
            var venusaur = cards[0];
            venusaur.Name.Should().Be("Venusaur ex");
            venusaur.Number.Should().Be(4);
            venusaur.Pack.Should().Be("Mewtwo");
            venusaur.Set.Should().Be("Genetic Apex");
            venusaur.Rate.BasicSlots.Should().BeNull();
            venusaur.Rate.FourthSlot.Should().Be("0.333%");
            venusaur.Rate.FifthSlot.Should().Be("1.332%");
            venusaur.Rate.Rarity.Should().Be("4 Diamonds");
            venusaur.Details.Type.Should().Be("grass");
            venusaur.Details.HP.Should().Be(190);
            venusaur.Details.Weakness.Should().Be("fire");
            venusaur.Details.RetreatCost.Should().Be(3);
            venusaur.Details.Abilities.Should().BeEmpty();
            venusaur.Details.Attacks.Should().HaveCount(2);
            venusaur.Details.Attacks![0].Name.Should().Be("Razor Leaf");
            venusaur.Details.Attacks[0].MinDamage.Should().Be(60);
            venusaur.Details.Attacks[0].MaxDamage.Should().Be(60);
            venusaur.Details.Attacks[0].AdditionalText.Should().BeNull();
            venusaur.Details.Attacks[0].EnergyCost.Should().BeEquivalentTo(["Grass","Colorless","Colorless"]);
            venusaur.Details.Attacks![1].Name.Should().Be("Giant Bloom");
            venusaur.Details.Attacks[1].MinDamage.Should().Be(100);
            venusaur.Details.Attacks[1].MaxDamage.Should().Be(100);
            venusaur.Details.Attacks[1].AdditionalText.Should().Be("Heal 30 damage from this Pokémon");
            venusaur.Details.Attacks[1].EnergyCost.Should().BeEquivalentTo(["Grass","Grass","Colorless","Colorless"]);
        }
    }
}
using FluentAssertions;
using PokemonTCGPScrapper;
using Xunit;

namespace PokemonTCGPocketScrapper.Tests
{
    public class SerebiiScrapperTest
    {
        [Fact]
        public async Task Scrapping_Bulbasaur_Should_GetExpectedValues()
        {
            // Arrange
            using HttpClient httpClient = SerebiiScrapperTestHelper.GetBulbasaurHttpClient();

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
    }
}
using HtmlAgilityPack;
using PokemonTCGPocketScrapper;
using PokemonTCGPocketScrapper.Models;

namespace PokemonTCGPScrapper
{
    /// <summary>
    /// Provides a base implementation for scrapers, including core logic and method definitions 
    /// for extracting card details. Subclasses must implement the abstract methods.
    /// </summary>
    internal abstract class BaseScrapper
    {
        /// <summary>
        /// Executes the scraping process for a collection of cards from a given set of URLs.
        /// </summary>
        /// <param name="collections">The list of collections to process.</param>
        /// <param name="httpClient">The HTTP client to fetch data from URLs.</param>
        /// <returns>A list of scraped cards with their details.</returns>
        internal async Task<List<Card>> RunAsync(IEnumerable<Collection> collections, HttpClient httpClient)
        {
            List<Card> cards = [];
            List<string> nonPokemonTypes = ["Supporter", "Trainer"];

            foreach (var collection in collections)
            {
                for (int i = 1; i <= collection.CardCount; i++)
                {
                    string cardNumber = i.ToString("D3");
                    string url = $"https://www.serebii.net/tcgpocket/{collection.Name}/{cardNumber}.shtml";

                    try
                    {
                        string html = await httpClient.GetStringAsync(url);
                        var doc = new HtmlDocument();
                        doc.LoadHtml(html);

                        string type = ExtractType(doc);
                        bool isPokemon = !nonPokemonTypes.Contains(type);

                        var card = new Card
                        {
                            Name = ExtractName(doc, isPokemon),
                            Set = ExtractSetName(doc),
                            Pack = ExtractPack(doc),
                            Rate = ExtractRate(doc),
                            Number = ExtractNumber(doc),
                            Details = new CardDetails
                            {
                                Type = type,
                                HP = ExtractHP(doc), // Fossils are Items that have HP
                                Attacks = isPokemon ? ExtractAttacks(doc) : null,
                                Weakness = isPokemon ? ExtractWeakness(doc) : null,
                                RetreatCost = isPokemon ? ExtractRetreatCost(doc) : null,
                                Abilities = isPokemon ? ExtractAbilities(doc) : null,
                                Effect = !isPokemon ? ExtractEffect(doc) : null
                            }
                        };

                        cards.Add(card);
                        Console.WriteLine($"Processed card #{i}: {card.Name}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing card #{i}: {ex.Message}");
                    }
                }
            }

            return cards;
        }

        /// <summary>
        /// Extracts the type of the card (e.g., Pokémon, Trainer, etc.) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The card type as a string.</returns>
        protected abstract string ExtractType(HtmlDocument doc);

        /// <summary>
        /// Extracts the name of the card from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <param name="isPokemon">Indicates whether the card is a Pokémon card.</param>
        /// <returns>The card name as a string.</returns>
        protected abstract string ExtractName(HtmlDocument doc, bool isPokemon);

        /// <summary>
        /// Extracts the set name from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The set name as a string.</returns>
        protected abstract string ExtractSetName(HtmlDocument doc);

        /// <summary>
        /// Extracts the pack name from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The pack name as a string.</returns>
        protected abstract string ExtractPack(HtmlDocument doc);

        /// <summary>
        /// Extracts the pull rate information from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>A <see cref="Rate"/> object containing pull rate details.</returns>
        protected abstract Rate ExtractRate(HtmlDocument doc);

        /// <summary>
        /// Extracts the card number from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The card number as an integer.</returns>
        protected abstract int ExtractNumber(HtmlDocument doc);

        /// <summary>
        /// Extracts the card's HP (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The HP value as an integer, or null if not applicable.</returns>
        protected abstract int? ExtractHP(HtmlDocument doc);

        /// <summary>
        /// Extracts a list of attacks for the card (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>A list of <see cref="Attack"/> objects, or null if not applicable.</returns>
        protected abstract List<Attack>? ExtractAttacks(HtmlDocument doc);

        /// <summary>
        /// Extracts the weakness of the card (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The weakness as a string, or null if not applicable.</returns>
        protected abstract string? ExtractWeakness(HtmlDocument doc);

        /// <summary>
        /// Extracts the retreat cost of the card (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The retreat cost as an integer, or null if not applicable.</returns>
        protected abstract int? ExtractRetreatCost(HtmlDocument doc);

        /// <summary>
        /// Extracts the abilities of the card (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>A list of <see cref="Ability"/> objects, or null if not applicable.</returns>
        protected abstract List<Ability>? ExtractAbilities(HtmlDocument doc);

        /// <summary>
        /// Extracts the card's effect (if applicable) from the HTML document.
        /// </summary>
        /// <param name="doc">The HTML document to parse.</param>
        /// <returns>The effect as a string, or null if not applicable.</returns>
        protected abstract string? ExtractEffect(HtmlDocument doc);
    }
}
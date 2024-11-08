using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PokemonTCGPocketScrapper;
using PokemonTCGPocketScrapper.Models;

namespace PokemonTCGPScrapper
{
    internal static class SerebiiScrapper
    {
        internal static async Task<List<Card>> RunAsync(IEnumerable<Collection> collections, HttpClient httpClient)
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
                            Number = i,
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

        private static string ExtractName(HtmlDocument doc, bool isPokemon)
        {
            // Select the main name node within the <font> tag
            HtmlNode? nameNode;

            // Pokémon and Trainer/Supporter cards have different nodes for the name
            if (isPokemon)
                nameNode = doc.DocumentNode.SelectSingleNode("//td[@class='cardinfo']/table/tr/td[@class='main']/b/font");
            else
                nameNode = doc.DocumentNode.SelectSingleNode("//tr/td/b");

            if (nameNode == null)
                return string.Empty;

            // Extract the main name text
            string name = nameNode.InnerText.Trim();

            // Check for additional text in the <b> tag following the <font> tag, such as "ex"
            var additionalTextNode = nameNode.ParentNode.SelectSingleNode("text()[normalize-space()]");
            if (additionalTextNode != null)
            {
                name += " " + additionalTextNode.InnerText.Trim();
            }

            // Nidoran shenanigans...
            if (name.Contains("&#9792"))
                name = name.Replace("&#9792", "-F");

            if (name.Contains("&#9794"))
                name = name.Replace("&#9794", "-M");

            return name;
        }

        private static string ExtractType(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectSingleNode("//td[@class='cardinfo']//td[@align='center']/img/@src");

            // card is supporter
            if (node is null)
            {
                node = doc.DocumentNode.SelectSingleNode("//tr/td[2]/div/i");

                return node.InnerText.Trim();
            }

            var imageSlug = node.GetAttributeValue("src", string.Empty);

            return GetImageName(imageSlug);
        }

        private static int? ExtractHP(HtmlDocument doc)
        {
            var node = doc.DocumentNode.SelectSingleNode("//td[@class='cardinfo']//td[@align='right']/b");
            var hpValue = node?.InnerText.Trim().Split().FirstOrDefault();

            return int.TryParse(hpValue, out var hp) ? hp : null;
        }

        private static List<Attack> ExtractAttacks(HtmlDocument doc)
        {
            var attacks = new List<Attack>();

            // Locate all attack rows within the table
            var attackRows = doc.DocumentNode.SelectNodes("//td[@class='cardinfo']/table/tr");

            if (attackRows != null)
            {
                foreach (var row in attackRows)
                {
                    // Check if this row contains attack data by checking if it has an image and bold text
                    var energyNodes = row.SelectNodes(".//td[1]/img");
                    var nameNode = row.SelectSingleNode(".//td[2]/span[@class='main']/a/b");
                    var damageNode = row.SelectSingleNode(".//td[3]//b");
                    var additionalTextNode = row.SelectSingleNode(".//td[2]");

                    if (energyNodes != null && nameNode != null && damageNode != null)
                    {
                        // Create a new Attack object for this row
                        var attack = new Attack
                        {
                            Name = nameNode.InnerText.Trim()
                        };

                        // Extract energy types
                        foreach (var energyNode in energyNodes)
                        {
                            var energyType = energyNode.GetAttributeValue("alt", "").Trim();
                            attack.EnergyCost.Add(energyType);
                        }

                        // Check for variable damage (e.g., "50x", "60+")
                        string damageText = damageNode.InnerText.Trim();
                        attack.AdditionalText = additionalTextNode != null ? ExtractTextWithImages(additionalTextNode) : null;

                        if (damageText.EndsWith("x"))
                        {
                            // Handle variable damage based on benched Pokémon or coin flips
                            int multiplier = int.Parse(damageText.TrimEnd('x'));
                            var coinFlipMatch = Regex.Match(attack.AdditionalText, @"Flip (\d+) coins");
                            if (coinFlipMatch.Success && int.TryParse(coinFlipMatch.Groups[1].Value, out int numFlips))
                            {
                                attack.MinDamage = 0;
                                attack.MaxDamage = multiplier * numFlips;
                                attack.CoinFlips = numFlips;
                                attack.IsVariableDamage = true;
                            }
                            else
                            {
                                attack.MinDamage = 0;
                                attack.MaxDamage = multiplier * 3;
                                attack.IsVariableDamage = true;
                            }
                        }
                        else if (damageText.EndsWith("+"))
                        {
                            // Handle conditional extra damage (e.g., "60+")
                            int baseDamage = int.Parse(damageText.TrimEnd('+'));
                            attack.MinDamage = baseDamage;
                            attack.MaxDamage = baseDamage;

                            // Check additional text for conditional extra damage
                            var extraDamageMatch = Regex.Match(attack.AdditionalText, @"(\d+) more damage");
                            if (extraDamageMatch.Success && int.TryParse(extraDamageMatch.Groups[1].Value, out int extraDamage))
                            {
                                attack.MaxDamage = baseDamage + extraDamage;

                                // Set specific conditions based on additional text
                                if (attack.AdditionalText.Contains("Flip a coin. If heads"))
                                {
                                    attack.CoinFlips = 1;
                                }
                            }
                        }
                        else
                        {
                            // Fixed damage
                            attack.MinDamage = int.TryParse(damageText, out var dmg) ? dmg : 0;
                            attack.MaxDamage = attack.MinDamage;
                            attack.IsVariableDamage = false;
                        }

                        attacks.Add(attack);
                    }
                }
            }

            return attacks;
        }

        private static string ExtractWeakness(HtmlDocument doc)
        {
            var weaknessNode = doc.DocumentNode.SelectSingleNode("//td[b[contains(text(), 'Weakness')]]/following-sibling::td[1]/img");
            if (weaknessNode is null)
                return "None";

            string weaknessType = GetImageName(weaknessNode!.GetAttributeValue("src", ""));

            return weaknessType;
        }

        private static int ExtractRetreatCost(HtmlDocument doc)
        {
            var retreatCostNodes = doc.DocumentNode.SelectNodes("//td[b[contains(text(), 'Retreat Cost')]]/following-sibling::td[1]/img");

            // Count the number of Colorless energy icons
            return retreatCostNodes?.Count ?? 0;
        }

        private static string GetImageName(string slug)
        {
            return slug.Split('/').Last().Replace(".png", "") ?? "None";
        }

        // Helper method to extract additional text with image alt attributes
        private static string? ExtractTextWithImages(HtmlNode node)
        {
            string result = "";

            foreach (var childNode in node.ChildNodes)
            {
                if (childNode.Name == "#text") // Plain text node
                {
                    result += HtmlEntity.DeEntitize(childNode.InnerText.Trim());
                }
                else if (childNode.Name == "img") // Image node
                {
                    string altText = childNode.GetAttributeValue("alt", "");
                    if (!string.IsNullOrEmpty(altText))
                    {
                        result += $" {altText} ";
                    }
                }
                else if (childNode.Name == "br") // Line break
                {
                    result += "\n";
                }
            }

            result = result.Trim();

            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        private static List<Ability> ExtractAbilities(HtmlDocument doc)
        {
            var abilities = new List<Ability>();

            // Locate the rows containing abilities by finding the tr element with the ability icon
            var abilityRows = doc.DocumentNode.SelectNodes("//tr[td/font/img[@alt='Ability']]");

            if (abilityRows != null)
            {
                foreach (var row in abilityRows)
                {
                    // Extract the ability name
                    var nameNode = row.SelectSingleNode(".//td[2]//span[@class='main']/b");
                    string name = nameNode?.InnerText.Trim() ?? "Unknown";

                    // Extract the description by collecting remaining text in the td
                    var descriptionNode = row.SelectSingleNode(".//td[2]");
                    string description = descriptionNode != null ? ExtractTextWithImages(descriptionNode) : "";

                    // Add the ability to the list
                    abilities.Add(new Ability { Name = name, Description = description });
                }
            }

            return abilities;
        }

        private static string ExtractPack(HtmlDocument doc)
        {
            // Example XPath; adjust based on observed structure in HTML
            var packNode = doc.DocumentNode.SelectSingleNode("//td[@class='fooevo']");

            return packNode?.InnerText.Trim() ?? "Unknown";
        }

        private static string ExtractSetName(HtmlDocument doc)
        {
            // Select the title element in the head
            var titleNode = doc.DocumentNode.SelectSingleNode("//head/title");

            if (titleNode != null)
            {
                // Extract the text and split by the dash "-"
                var titleText = titleNode.InnerText.Trim();
                var segments = titleText.Split('-');

                // The first segment is the set name
                return segments[0].Trim();
            }

            return "Unknown";
        }

        private static Rate ExtractRate(HtmlDocument doc)
        {
            Rate rate;

            // Extract rarity
            var rarityNode = doc.DocumentNode.SelectSingleNode("//td[@class='small' and @align='right']/img");
            string raritySrc = rarityNode.GetAttributeValue("src", "");

            rate = new Rate
            {
                Rarity = ParseRarityFromSrc(raritySrc)
            };

            // Extract pull rate by slot
            var rateNode = doc.DocumentNode.SelectSingleNode("//td[@class='cen' and b[contains(text(), 'Rate')]]");
            if (rateNode != null)
            {
                var rateText = rateNode.InnerText;
                rate.BasicSlots = ExtractSlotRate(rateText, "1st - 3rd Slot:");
                rate.FourthSlot = ExtractSlotRate(rateText, "4th Slot:");
                rate.FifthSlot = ExtractSlotRate(rateText, "5th Slot:");
            }

            return rate;
        }

        // Helper method to extract slot rate from rate text
        private static string? ExtractSlotRate(string rateText, string slotLabel)
        {
            var match = Regex.Match(rateText, $@"{slotLabel}\s*(\d+(\.\d+)?)%");
            if (match.Success)
            {
                return match.Groups[1].Value + '%';
            }
            return null;
        }

        // Helper method to parse rarity from image src
        private static string ParseRarityFromSrc(string src)
        {
            if (src.Contains("diamond"))
            {
                return src.Contains("diamond1") ? "1 Diamond" :
                    src.Contains("diamond2") ? "2 Diamonds" :
                    src.Contains("diamond3") ? "3 Diamonds" :
                    src.Contains("diamond4") ? "4 Diamonds" : "Unknown";
            }
            else if (src.Contains("star"))
            {
                return src.Contains("star1") ? "1 Star" :
                    src.Contains("star2") ? "2 Stars" :
                    src.Contains("star3") ? "3 Stars" : "Unknown";
            }
            else if (src.Contains("crown"))
            {
                return "Crown";
            }
            return "Unknown";
        }

        private static string ExtractEffect(HtmlDocument doc)
        {
            // Locate the <td> that contains the effect text for a Supporter card
            var effectNode = doc.DocumentNode.SelectSingleNode("//td[@colspan='3' and @align='left']/p");

            if (effectNode != null)
            {
                // Extract and format the text, including handling images and HTML entities
                return ExtractTextWithImages(effectNode);
            }

            return "No effect found";
        }
    }
}
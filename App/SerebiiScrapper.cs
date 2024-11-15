using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using PokemonTCGPocketScrapper.Models;

namespace PokemonTCGPScrapper
{
    internal class SerebiiScrapper : BaseScrapper
    {
        /// <inheritdoc/>
        protected override string ExtractName(HtmlDocument doc, bool isPokemon)
        {
            // Determine the appropriate XPath based on whether it's a Pokémon or non-Pokémon card
            string xpath = isPokemon
                ? "//td[@class='cardinfo']/table/tr/td[@class='main']/b/font"
                : "//tr/td/b";

            // Locate the name node
            HtmlNode? nameNode = doc.DocumentNode.SelectSingleNode(xpath);
            if (nameNode == null)
            {
                return string.Empty;
            }

            // Extract and clean the main name text
            string name = nameNode.InnerText.Trim();

            // Check for additional text (e.g., "ex") following the name node
            var additionalTextNode = nameNode.ParentNode?.SelectSingleNode("text()[normalize-space()]");
            if (additionalTextNode != null)
            {
                name += $" {additionalTextNode.InnerText.Trim()}";
            }

            // Handle gender symbols for Nidoran
            name = HandleNidoranGender(name);

            return name;
        }

        private static string HandleNidoranGender(string name)
        {
            // Replace gender symbols with readable suffixes
            return name.Replace("&#9792", "-F").Replace("&#9794", "-M");
        }

        /// <inheritdoc/>
        protected override string ExtractType(HtmlDocument doc)
        {
            // Attempt to locate the primary type node
            var typeNode = doc.DocumentNode.SelectSingleNode("//td[@class='cardinfo']//td[@align='center']/img");
            if (typeNode != null)
            {
                // Extract type from image's "src" attribute
                var imageSlug = typeNode.GetAttributeValue("src", string.Empty);
                return GetImageName(imageSlug);
            }

            // Fallback: Check if the card is a Supporter or Trainer
            var textNode = doc.DocumentNode.SelectSingleNode("//tr/td[2]/div/i");
            if (textNode != null)
            {
                return textNode.InnerText.Trim();
            }

            // Default to "Unknown" if no type is found
            return "Unknown";
        }

        /// <inheritdoc/>
        protected override int? ExtractHP(HtmlDocument doc)
        {
            // Locate the node containing HP information
            var hpNode = doc.DocumentNode.SelectSingleNode("//td[@class='cardinfo']//td[@align='right']/b");

            if (hpNode == null)
            {
                // Return null if the node is not found
                return null;
            }

            // Extract and clean the HP value
            string? hpText = hpNode.InnerText.Trim().Split().FirstOrDefault();

            // Attempt to parse the HP value
            if (int.TryParse(hpText, out int hp))
            {
                return hp;
            }

            // Return null if parsing fails
            return null;
        }

        /// <inheritdoc/>
        protected override List<Attack> ExtractAttacks(HtmlDocument doc)
        {
            // Locate all attack rows within the table
            var attackRows = doc.DocumentNode.SelectNodes("//td[@class='cardinfo']/table/tr");
            if (attackRows == null) return new List<Attack>();

            return attackRows
                .Select(ParseAttackRow)
                .Where(attack => attack != null)
                .ToList()!;
        }

        private static Attack? ParseAttackRow(HtmlNode row)
        {
            // Locate nodes for energy, name, damage, and additional text
            var energyNodes = row.SelectNodes(".//td[1]/img");
            var nameNode = row.SelectSingleNode(".//td[2]/span[@class='main']/a/b");
            var damageNode = row.SelectSingleNode(".//td[3]//b");
            var additionalTextNode = row.SelectSingleNode(".//td[2]");

            if (energyNodes == null || nameNode == null || damageNode == null) return null;

            var attack = new Attack
            {
                Name = nameNode.InnerText.Trim(),
                EnergyCost = ExtractEnergyCost(energyNodes),
                AdditionalText = additionalTextNode != null ? ExtractTextWithImages(additionalTextNode) : null
            };

            ParseDamage(damageNode.InnerText.Trim(), attack);

            return attack;
        }

        private static List<string> ExtractEnergyCost(HtmlNodeCollection energyNodes)
        {
            // Extract energy types from energy nodes
            return energyNodes
                .Select(node => node.GetAttributeValue("alt", "").Trim())
                .ToList();
        }

        private static void ParseDamage(string damageText, Attack attack)
        {
            if (damageText.EndsWith("x"))
            {
                // Handle variable damage (e.g., "50x")
                HandleVariableDamage(damageText, attack);
            }
            else if (damageText.EndsWith("+"))
            {
                // Handle conditional extra damage (e.g., "60+")
                HandleConditionalDamage(damageText, attack);
            }
            else
            {
                // Handle fixed damage
                attack.MinDamage = int.TryParse(damageText, out var dmg) ? dmg : 0;
                attack.MaxDamage = attack.MinDamage;
                attack.IsVariableDamage = false;
            }
        }

        private static void HandleVariableDamage(string damageText, Attack attack)
        {
            // Extract multiplier (e.g., "50x")
            if (!int.TryParse(damageText.TrimEnd('x'), out var multiplier)) return;

            attack.MinDamage = 0;
            attack.IsVariableDamage = true;

            // Extract coin flips or default multiplier
            var coinFlipMatch = Regex.Match(attack.AdditionalText ?? "", @"Flip (\d+) coins");
            if (coinFlipMatch.Success && int.TryParse(coinFlipMatch.Groups[1].Value, out int numFlips))
            {
                attack.MaxDamage = multiplier * numFlips;
                attack.CoinFlips = numFlips;
            }
            else
            {
                attack.MaxDamage = multiplier * 3; // Default to 3 coin flips if unspecified
            }
        }

        private static void HandleConditionalDamage(string damageText, Attack attack)
        {
            // Extract base damage (e.g., "60+")
            if (!int.TryParse(damageText.TrimEnd('+'), out var baseDamage)) return;

            attack.MinDamage = baseDamage;
            attack.MaxDamage = baseDamage;

            // Extract additional damage from the description
            var extraDamageMatch = Regex.Match(attack.AdditionalText ?? "", @"(\d+) more damage");
            if (extraDamageMatch.Success && int.TryParse(extraDamageMatch.Groups[1].Value, out int extraDamage))
            {
                attack.MaxDamage = baseDamage + extraDamage;

                // Check for coin flip condition
                if (attack.AdditionalText?.Contains("Flip a coin. If heads") == true)
                {
                    attack.CoinFlips = 1;
                }
            }
        }

        /// <inheritdoc/>
        protected override string ExtractWeakness(HtmlDocument doc)
        {
            // Find the weakness image node
            var weaknessNode = doc.DocumentNode.SelectSingleNode("//td[b[contains(text(), 'Weakness')]]/following-sibling::td[1]/img");
            if (weaknessNode == null)
            {
                return "None";
            }

            // Extract and return the weakness type from the image's "src" attribute
            return GetImageName(weaknessNode.GetAttributeValue("src", ""));
        }

        /// <inheritdoc/>
        protected override int? ExtractRetreatCost(HtmlDocument doc)
        {
            // Locate all retreat cost image nodes
            var retreatCostNodes = doc.DocumentNode.SelectNodes("//td[b[contains(text(), 'Retreat Cost')]]/following-sibling::td[1]/img");

            // Return the count of nodes or 0 if none are found
            return retreatCostNodes?.Count ?? 0;
        }

        private static string GetImageName(string slug)
        {
            // Extract the image name by splitting the URL and removing the file extension
            return slug.Split('/').Last().Replace(".png", "") ?? "None";
        }

        // Helper method to extract additional text with image alt attributes
        private static string? ExtractTextWithImages(HtmlNode node)
        {
            var builder = new StringBuilder();

            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "#text":
                        // Plain text node
                        builder.Append(HtmlEntity.DeEntitize(childNode.InnerText.Trim()));
                        break;

                    case "img":
                        // Image node
                        string altText = childNode.GetAttributeValue("alt", "");
                        if (!string.IsNullOrWhiteSpace(altText))
                        {
                            builder.Append($" {altText} ");
                        }
                        break;

                    case "br":
                        // Line break
                        builder.Append("\n");
                        break;
                }
            }

            string result = builder.ToString().Trim();

            return string.IsNullOrWhiteSpace(result) ? null : result;
        }

        /// <inheritdoc/>
        protected override List<Ability> ExtractAbilities(HtmlDocument doc)
        {
            var abilities = new List<Ability>();

            // Locate the rows containing abilities by finding the tr element with the ability icon
            var abilityRows = doc.DocumentNode.SelectNodes("//tr[td/font/img[@alt='Ability']]");
            if (abilityRows == null) return abilities;

            foreach (var row in abilityRows)
            {
                // Extract the ability name
                string name = ExtractAbilityName(row);

                // Extract the description
                string description = ExtractAbilityDescription(row);

                // Add the ability to the list
                abilities.Add(new Ability { Name = name, Description = description });
            }

            return abilities;
        }

        private static string ExtractAbilityName(HtmlNode row)
        {
            // Extract the ability name node and clean the text
            var nameNode = row.SelectSingleNode(".//td[2]//span[@class='main']/b");
            return nameNode?.InnerText.Trim() ?? "Unknown";
        }

        private static string ExtractAbilityDescription(HtmlNode row)
        {
            // Extract and format the ability description
            var descriptionNode = row.SelectSingleNode(".//td[2]");
            return descriptionNode != null ? ExtractTextWithImages(descriptionNode) ?? "No description" : "No description";
        }

        /// <inheritdoc/>
        protected override string ExtractPack(HtmlDocument doc)
        {
            // Define the XPath for the pack node
            const string packXPath = "//td[@class='fooevo']";

            // Locate the pack node
            var packNode = doc.DocumentNode.SelectSingleNode(packXPath);

            // Return the cleaned text or a default value if the node is not found
            return packNode?.InnerText.Trim() ?? "Unknown";
        }

        /// <inheritdoc/>
        protected override string ExtractSetName(HtmlDocument doc)
        {
            // XPath to locate the title node in the head section
            const string titleXPath = "//head/title";

            // Locate the title node
            var titleNode = doc.DocumentNode.SelectSingleNode(titleXPath);

            if (titleNode == null)
            {
                // Return a default value if the title node is not found
                return "Unknown";
            }

            // Extract the text and split it by the dash ("-")
            string titleText = titleNode.InnerText.Trim();
            var segments = titleText.Split('-');

            // Return the first segment as the set name or a default value if empty
            return segments.Length > 0 ? segments[0].Trim() : "Unknown";
        }

        /// <inheritdoc/>
        protected override Rate ExtractRate(HtmlDocument doc)
        {
            // Initialize the rate object
            var rate = new Rate
            {
                Rarity = ExtractRarity(doc)
            };

            // Extract pull rates for each slot
            var rateNode = doc.DocumentNode.SelectSingleNode("//td[@class='cen' and b[contains(text(), 'Rate')]]");
            if (rateNode != null)
            {
                string rateText = rateNode.InnerText;
                rate.BasicSlots = ExtractSlotRate(rateText, "1st - 3rd Slot:");
                rate.FourthSlot = ExtractSlotRate(rateText, "4th Slot:");
                rate.FifthSlot = ExtractSlotRate(rateText, "5th Slot:");
            }

            return rate;
        }

        private static string ExtractRarity(HtmlDocument doc)
        {
            // Locate the rarity image node
            var rarityNode = doc.DocumentNode.SelectSingleNode("//td[@class='small' and @align='right']/img");
            if (rarityNode != null)
            {
                string raritySrc = rarityNode.GetAttributeValue("src", "");
                return ParseRarityFromSrc(raritySrc);
            }

            return "Unknown";
        }

        private static string? ExtractSlotRate(string rateText, string slotLabel)
        {
            // Use a regex to extract the rate percentage for a given slot
            var match = Regex.Match(rateText, $@"{slotLabel}\s*(\d+(\.\d+)?)%");
            return match.Success ? $"{match.Groups[1].Value}%" : null;
        }

        /// <inheritdoc/>
        protected override int ExtractNumber(HtmlDocument doc)
        {
            // Locate the card number node and clean the text
            var numberNode = doc.DocumentNode.SelectSingleNode("//td/font[@size='4']/b");
            if (numberNode == null)
            {
                throw new InvalidOperationException("Card number not found.");
            }

            string text = numberNode.InnerHtml.Replace("&nbsp;", "").Trim();
            string[] tokens = text.Split('/');

            // Parse the card number (e.g., "123" from "123/456")
            if (int.TryParse(tokens[0], out int cardNumber))
            {
                return cardNumber;
            }

            throw new FormatException("Invalid card number format.");
        }

        private static string ParseRarityFromSrc(string src)
        {
            // Check for rarity symbols in the image source
            if (src.Contains("diamond"))
            {
                return src.Contains("diamond1") ? "1 Diamond" :
                       src.Contains("diamond2") ? "2 Diamonds" :
                       src.Contains("diamond3") ? "3 Diamonds" :
                       src.Contains("diamond4") ? "4 Diamonds" : "Unknown";
            }

            if (src.Contains("star"))
            {
                return src.Contains("star1") ? "1 Star" :
                       src.Contains("star2") ? "2 Stars" :
                       src.Contains("star3") ? "3 Stars" : "Unknown";
            }

            if (src.Contains("crown"))
            {
                return "Crown";
            }

            return "Unknown";
        }

        /// <inheritdoc/>
        protected override string ExtractEffect(HtmlDocument doc)
        {
            // Define the XPath for locating the effect node
            const string effectXPath = "//td[@colspan='3' and @align='left']/p";

            // Locate the effect node
            var effectNode = doc.DocumentNode.SelectSingleNode(effectXPath);

            if (effectNode == null)
            {
                // Return a default message if no effect is found
                return "No effect found";
            }

            // Extract and format the effect text
            return ExtractTextWithImages(effectNode) ?? "No effect found";
        }
    }
}
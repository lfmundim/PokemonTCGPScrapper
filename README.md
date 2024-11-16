# Pokémon TCG Pocket Scraper

## Overview

Welcome to the **Pokémon TCG Pocket Scraper** project! 🎉

This project was born out of my enthusiasm for Pokémon TCG Pocket during its early days. At the time, the game had limited in-game activities, so I decided to create a comprehensive database of card information for analysis, collection tracking, and other fun uses.

In addition to serving my passion for Pokémon, this project was also a great opportunity to experiment with web scraping and enhance my coding skills. I'm making it public in case it proves helpful to others who share the same interests. 🤓

---

## Features

### What Does This Project Do?

The scraper navigates to:
- [Serebii.net](https://www.serebii.net) (Special thanks to Joe for maintaining this amazing resource over the years. If this project causes any issues, please let me know, and I’ll take it down! 🙏)
- [Limitless](https://pocket.limitlesstcg.com/cards)

It scrapes Pokémon TCG Pocket card information, including:
- Name
- Stage and previous forms
- Set and pack details
- Rarity and appearance rates
- Stats like type, HP, and retreat cost
- Attack details, including damage and energy cost
- Weaknesses and abilities

The scraped data is saved as JSON files for easy use in analysis, application development, or collection tracking.

---

### Example Output

Here’s a sample JSON object representing a card:

```json
{
  "Name": "Charizard ex",
  "Stage": "Stage 2",
  "PreviousForm": "Charmeleon",
  "Set": "Genetic Apex",
  "Pack": "Charizard",
  "Rate": {
    "FourthSlot": "0.05%",
    "FifthSlot": "0.2%",
    "Rarity": "2 Stars"
  },
  "Number": 253,
  "Details": {
    "Type": "Fire",
    "HP": 180,
    "Attacks": [
      {
        "Name": "Slash",
        "EnergyCost": ["Fire", "Colorless", "Colorless"],
        "MinDamage": 60,
        "MaxDamage": 60
      },
      {
        "Name": "Crimson Stomp",
        "EnergyCost": ["Fire", "Fire", "Colorless", "Colorless"],
        "MinDamage": 200,
        "MaxDamage": 200,
        "AdditionalText": "Discard 2 Fire Energy from this Pokémon"
      }
    ],
    "Weakness": "Water",
    "RetreatCost": 2,
    "Abilities": []
  }
}
```

---

## Getting Started

### Prerequisites
To run this project, you’ll need:
- **.NET 8 SDK** (or later)
- Basic knowledge of C# programming
- Internet access for web scraping

---

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/YourUsername/PokemonTCGPocketScraper.git
   cd PokemonTCGPocketScraper
   ```

2. Build the project:
   ```bash
   dotnet build
   ```

3. Run the scraper:
   ```bash
   dotnet run
   ```

4. The output JSON file will be saved in the `cards.json` file.

---

### How It Works

1. **Scraping Websites**:
   - The scraper targets Serebii.net and Limitless for Pokémon TCG Pocket card data.
   - It iterates through card sets, extracting details about each card.

2. **Data Processing**:
   - Collected data is structured into a JSON format, making it easy to integrate with other applications.

3. **Customizable Scraping**:
   - You can configure the scraper to focus on specific sets, types, or other attributes.

---

## Use Cases

- **Collection Tracking**: Organize and manage your Pokémon TCG Pocket collection.
- **Data Analysis**: Analyze card stats, rarity, and set distribution.
- **Custom Applications**: Build apps or tools using the card data.

---

## Contributing

Contributions are welcome! If you’d like to improve this project, please:
1. Fork the repository
2. Create a feature branch:
   ```bash
   git checkout -b feature-name
   ```
3. Commit your changes:
   ```bash
   git commit -m "Add your message here"
   ```
4. Push your branch:
   ```bash
   git push origin feature-name
   ```
5. Open a pull request on GitHub.

---

## Feedback and Support

Have questions or feedback? Feel free to reach out via [GitHub Issues](https://github.com/YourUsername/PokemonTCGPocketScraper/issues). I’d love to hear your thoughts and suggestions!

---

## Acknowledgments

- **Serebii.net**: For the invaluable Pokémon TCG data.
- **Limitless**: For additional card details.
- Pokémon TCG Pocket fans and community members who inspired this project.

---

### License

This project is open-source under the [MIT License](LICENSE). Feel free to use, modify, or share it as long as you give proper credit.

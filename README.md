# Pokémon TCG Pocket Scraper

## Overview

This project is a personal endeavor I started because of my enthusiasm for Pokémon TCG Pocket during its early days. At that time, the game had limited in-game activities, and I wanted to explore more by creating a database of card information for analysis and other fun uses.

I also took this opportunity to experiment with web scraping and enhance my coding skills. Since the project might be helpful to others, I'm making it public for anyone interested! 🤓

## What Does This Project Do?

The scraper visits the [Serebii.net](https://serebii.net) website (shout-out to Joe for the incredible effort over the years! If this causes any issues, I'm more than happy to take it down). It navigates to the Pokémon TCG Pocket section and iterates through all available items, scraping details about each card and formatting the information into JSON.

You can find the latest JSON output under `cards.json`. If you only need the card data, feel free to grab that directly.

**Note:** I've intentionally excluded duplicate cards like alternate versions of _Pikachu ex_, as they contain the same core information and are effectively identical.

## Example Card Information

```json
{
    "Name": "Pikachu ex",
    "Set": "Genetic Apex",
    "Pack": "Pikachu",
    "Rate": {
        "FourthSlot": "0.333%",
        "FifthSlot": "1.332%",
        "Rarity": "4 Diamonds"
    },
    "Number": 96,
    "Details": {
        "Type": "electric",
        "HP": 120,
        "Attacks": [
            {
                "Name": "Circle Circuit",
                "EnergyCost": [
                    "Electric",
                    "Electric"
                ],
                "MaxDamage": 90,
                "IsVariableDamage": true,
                "AdditionalText": "This attack does 30 damage for each of your Benched Electric Pokémon"
            }
        ],
        "Weakness": "fighting",
        "RetreatCost": 1,
        "Abilities": []
    }
}
```

## Why Make This Public?

This project started as a way for me to explore web scraping and learn more about Pokémon TCG data, but I figured others might find it useful or interesting. Whether you're a fellow Pokémon fan or just curious about scraping and data analysis, I hope you can benefit from this work.

Feel free to use, adapt, or build upon it! If you have any feedback, I'd love to hear from you.

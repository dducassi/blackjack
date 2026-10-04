# Blackjack

A complete blackjack game built in Godot 4 with C#. Playable, configurable,
and structured with a clean separation between game rules and presentation.

## What it is

A fully-featured single-player blackjack game against the dealer. Not a
prototype — a complete implementation of standard Vegas rules with
configurable table settings, animated card dealing, sound, and a proper
menu system.

![Blackjack screenshot](docs/screenshot.png)

## Rules implemented

- Multi-deck shoe (1, 2, 4, 6, or 8 decks, configurable)
- Dealer stands on all 17s, or hits soft 17 (configurable)
- Blackjack pays 3:2
- Hit, stand, double down
- Split up to 4 hands, with resplitting
- Double after split (except split Aces)
- Split Aces receive one card each; 21 on a split Ace pays 1:1
- Insurance on dealer Ace, pays 2:1
- Dealer peek on blackjack
- Cut-card reshuffle at 25% penetration

## Features

- Animated card dealing with slide-in and hole-card flip
- Procedural crosshatch card backs, generated at runtime
- Chip, card, and shuffle sound effects with pitch variation
- Looping background music
- Bankroll flash on win/loss
- Options menu for sound, music, deck count, and H17/S17
- Dynamic house rules screen that reflects current settings
- Credits and legal screens with third-party license attribution

## Architecture

The project is split into two layers:

**`core/` — pure game logic, no Godot dependency**

    core/
    ├── Card.cs
    ├── Dealer.cs
    ├── GameSettings.cs
    ├── Hand.cs
    ├── Player.cs
    ├── PlayerHand.cs
    ├── Rank.cs
    ├── Shoe.cs
    └── Suit.cs

**Godot layer — orchestration and presentation**

    GameManager.cs    — scene orchestration, wires core to UI
    CardVisual.cs     — card scene behavior
    Game.tscn         — main scene
    Card.tscn         — card scene
    DefaultTheme.tres — UI theme
    GameSettings.tres — table rules resource

The `core/` classes have no knowledge of Godot. They're plain C# and could be
dropped into a console app, a test project, or a different engine without
modification. `GameManager` is the only class that touches both the domain
model and the Godot scene tree.

This separation is deliberate. It means the rules, which are genuinely
fiddly in blackjack (particularly around Aces and splits), can be tested in
isolation, and the presentation layer can be refactored without risking
rule regressions.

## Building

Requires:
- Godot 4.7+ (.NET version)
- .NET 8 SDK or later

Open the project in Godot, build once (hammer icon), and run. The main scene
is `Game.tscn`.

For an exported build, make sure the export preset includes:
licenses/*.txt


under **Resources → Filters to export non-resource files/folders**. Without
this, the Legal screen will be empty in the exported game.

## Configuration

Table rules are defined in `GameSettings.tres`, a Godot `Resource` assigned
to the `Game` node. It can be edited in the Godot Inspector or at runtime
via the in-game Options menu.

| Setting | Default | Notes |
|---|---|---|
| `DeckCount` | 6 | 1, 2, 4, 6, or 8 |
| `StartingBankroll` | 100 | In chips |
| `MinimumBet` | 10 | Game-over threshold |
| `HitSoft17` | false | Dealer stands on soft 17 by default |
| `SoundEnabled` | true | |
| `MusicEnabled` | true | |

## Credits

**Game code:** Daniel Ducassi

**Engine:** [Godot Engine](https://godotengine.org) — MIT License

**Music:** Scott Joplin, *The Entertainer* (1902, public domain), rendered
with a Honkytonk soundfont.

**Sounds:** Original work plus CC0 from [Freesound](https://freesound.org)

**Font:** Licensed under the SIL Open Font License 1.1

Third-party license texts are included in the `licenses/` folder and
displayed in-game under CREDITS → LEGAL.

## License

Copyright © 2026 Daniel Ducassi. All rights reserved.

This project is provided as a portfolio example. See `licenses/game_license.txt`
for full terms.

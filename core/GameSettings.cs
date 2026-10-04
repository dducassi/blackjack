using Godot;

namespace Blackjack;

[GlobalClass]
public partial class GameSettings : Resource
{
    [Export] public int DeckCount { get; set; } = 6;
    [Export] public int StartingBankroll { get; set; } = 100;
    [Export] public int MinimumBet { get; set; } = 10;
    [Export] public bool SoundEnabled { get; set; } = true;
    [Export] public bool MusicEnabled { get; set; } = true;
    [Export] public bool HitSoft17 { get; set; } = false;
}
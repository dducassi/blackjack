namespace Blackjack;

public class PlayerHand
{
    public Hand Hand { get; } = new();
    public int Bet { get; private set; }
    public bool IsFinished { get; set; }
    public bool IsFromAceSplit { get; set; }

    public PlayerHand(int bet)
    {
        Bet = bet;
    }

    public void IncreaseBet(int amount)
    {
        Bet += amount;
    }
}
namespace Blackjack;

public record Card(Suit Suit, Rank Rank)
{
    /// <summary>
    /// The card's value for blackjack, treating Ace as 11.
    /// The Hand class is responsible for adjusting Aces to 1 when needed.
    /// </summary>
    public int Value => Rank switch
    {
        Rank.Ace => 11,
        Rank.Jack or Rank.Queen or Rank.King => 10,
        _ => (int)Rank
    };
}
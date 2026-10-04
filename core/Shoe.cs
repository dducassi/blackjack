namespace Blackjack;

public class Shoe
{
    private readonly List<Card> _cards = new();
    private readonly Random _random = new();
    private readonly int _deckCount;

    public Shoe(int deckCount)
    {
        if (deckCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(deckCount), "Shoe must contain at least one deck.");
        }

        _deckCount = deckCount;
        Reset();
    }

    public int DeckCount => _deckCount;
    public int TotalCards => _deckCount * 52;
    public int Count => _cards.Count;

    /// <summary>
    /// True when the shoe has passed the "cut card" — fewer than 25% of cards remaining.
    /// Real casinos set a cut card in the shoe; when the dealer reaches it,
    /// they finish the current round, then reshuffle.
    /// </summary>
    public bool NeedsReshuffle => Count < TotalCards / 4;

    public void Reset()
    {
        _cards.Clear();

        for (int d = 0; d < _deckCount; d++)
        {
            foreach (Suit suit in Enum.GetValues<Suit>())
            {
                foreach (Rank rank in Enum.GetValues<Rank>())
                {
                    _cards.Add(new Card(suit, rank));
                }
            }
        }
    }

    public void Shuffle()
    {
        for (int i = _cards.Count - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public void Reshuffle()
    {
        Reset();
        Shuffle();
    }

    public Card Draw()
    {
        if (_cards.Count == 0)
        {
            throw new InvalidOperationException("Cannot draw from an empty shoe.");
        }

        Card card = _cards[^1];
        _cards.RemoveAt(_cards.Count - 1);
        return card;
    }
}
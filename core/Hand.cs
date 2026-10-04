namespace Blackjack;

public class Hand
{
    private readonly List<Card> _cards = new();

    public IReadOnlyList<Card> Cards => _cards;
    public int Count => _cards.Count;

    public void Add(Card card)
    {
        _cards.Add(card);
    }

    public void Clear()
    {
        _cards.Clear();
    }

    public Card TakeCard(int index)
    /// Splitting
    {
        if (index < 0 || index >= _cards.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        Card card = _cards[index];
        _cards.RemoveAt(index);
        return card;
    }

    private (int total, bool soft) Evaluate()
    {
        int total = 0;
        int aces = 0;

        foreach (Card card in _cards)
        {
            total += card.Value;
            if (card.Rank == Rank.Ace) aces++;
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return (total, aces > 0);
    }

    public int Total => Evaluate().total;
    public bool IsSoft => Evaluate().soft;
    public bool IsBust => Total > 21;
    public bool IsBlackjack => _cards.Count == 2 && Total == 21;
}
namespace Blackjack;

public class Dealer
{
    private bool _holeCardHidden = true;
    private readonly bool _hitSoft17;

    public Dealer(bool hitSoft17 = false)
    {
        _hitSoft17 = hitSoft17;
    }

    public string Name { get; } = "Dealer";
    public Hand Hand { get; } = new();
    public bool IsBust => Hand.IsBust;
    public bool IsBlackjack => Hand.IsBlackjack;
    public bool HasHiddenCard => _holeCardHidden;

    public bool MustHit => Hand.Total < 17
        || (_hitSoft17 && Hand.Total == 17 && Hand.IsSoft);

    public void RevealHoleCard() => _holeCardHidden = false;
    public void HideHoleCard() => _holeCardHidden = true;

    public Card UpCard => Hand.Cards[0];
    public bool UpCardIsAce => UpCard.Rank == Rank.Ace;
}
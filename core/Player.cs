namespace Blackjack;

public class Player
{
    private readonly List<PlayerHand> _hands = new();

    public string Name { get; }
    public IReadOnlyList<PlayerHand> Hands => _hands;
    public int Bankroll { get; private set; }

    public Player(string name, int startingBankroll)
    {
        Name = name;
        Bankroll = startingBankroll;
    }

    public bool CanAfford(int amount) => amount > 0 && amount <= Bankroll;

    public void StartRound(int bet)
    {
        if (!CanAfford(bet))
        {
            throw new InvalidOperationException($"Cannot bet {bet}.");
        }

        Bankroll -= bet;
        _hands.Clear();
        _hands.Add(new PlayerHand(bet));
    }

    public const int MaxHands = 4;

    public bool CanSplitHand(PlayerHand ph) =>
        _hands.Count < MaxHands &&
        ph.Hand.Cards.Count == 2 &&
        !ph.IsFinished &&
        !ph.IsFromAceSplit &&
        ph.Hand.Cards[0].Rank == ph.Hand.Cards[1].Rank &&
        CanAfford(ph.Bet);

    
    /// <summary>
    /// Splits the given hand into two. The caller supplies one new card for the
    /// original hand. The new hand starts with only the moved card — its second
    /// card is dealt later, unless it's an Ace split, in which case both hands
    /// finish immediately with one card each and the caller is responsible for
    /// adding a second card to the new hand if desired.
    /// </summary>
    public PlayerHand Split(PlayerHand ph, Card newCardForOriginal)
    {
        if (!CanSplitHand(ph))
        {
            throw new InvalidOperationException("Cannot split this hand.");
        }

        Card moved = ph.Hand.TakeCard(1);
        Bankroll -= ph.Bet;

        ph.Hand.Add(newCardForOriginal);

        PlayerHand newHand = new PlayerHand(ph.Bet);
        newHand.Hand.Add(moved);

        int index = _hands.IndexOf(ph);
        _hands.Insert(index + 1, newHand);

        if (moved.Rank == Rank.Ace)
        {
            ph.IsFromAceSplit = true;
            ph.IsFinished = true;
            newHand.IsFromAceSplit = true;
            newHand.IsFinished = true;
    }

    return newHand;
}

    public bool CanAffordInsurance(int mainBet) => Bankroll >= mainBet / 2;

    /// <summary>
    /// Places an insurance side bet of half the main bet and returns the amount wagered.
    /// </summary>
    public int PlaceInsurance(int mainBet)
    {
        int insuranceBet = mainBet / 2;
        if (!CanAffordInsurance(mainBet))
        {
            throw new InvalidOperationException("Not enough chips to insure.");
        }

        Bankroll -= insuranceBet;
        return insuranceBet;
    }

    /// <summary>
    /// Insurance pays 2:1: the player receives their stake back plus twice the stake.
    /// </summary>
    public void WinInsurance(int insuranceBet)
    {
        Bankroll += insuranceBet * 3;
    }

    public bool CanDoubleFor(PlayerHand ph) =>
        ph.Hand.Cards.Count == 2 &&
        !ph.IsFinished &&
        Bankroll >= ph.Bet;

    public void DoubleBet(PlayerHand ph)
    {
        if (Bankroll < ph.Bet)
        {
            throw new InvalidOperationException("Not enough chips to double.");
        }

        Bankroll -= ph.Bet;
        ph.IncreaseBet(ph.Bet);
    }

    public PlayerHand? NextUnfinishedHand()
    {
        foreach (PlayerHand ph in _hands)
        {
            if (!ph.IsFinished)
            {
                return ph;
            }
        }

        return null;
    }

    public void WinBet(PlayerHand ph) => Bankroll += ph.Bet * 2;

    public void WinBlackjack(PlayerHand ph)
    {
        int winnings = ph.Bet * 3 / 2;
        Bankroll += ph.Bet + winnings;
    }

    public void PushBet(PlayerHand ph) => Bankroll += ph.Bet;
}
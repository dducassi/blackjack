using Godot;

namespace Blackjack;

public partial class CardVisual : PanelContainer
{
    private Label _rankLabel = null!;
    private Label _suitLabel = null!;

    private static ImageTexture? _cardBackPattern;

    public override void _Ready()
    {
        _rankLabel = GetNode<Label>("Layout/RankLabel");
        _suitLabel = GetNode<Label>("Layout/SuitLabel");
    }

    public void SetCard(Card card)
    {
        _rankLabel.Text = RankToString(card.Rank);
        _suitLabel.Text = SuitToString(card.Suit);

        Color textColor = (card.Suit == Suit.Hearts || card.Suit == Suit.Diamonds)
            ? new Color(0.8f, 0.1f, 0.1f)
            : new Color(0.05f, 0.05f, 0.05f);
        _rankLabel.AddThemeColorOverride("font_color", textColor);
        _suitLabel.AddThemeColorOverride("font_color", textColor);

        var style = new StyleBoxFlat
        {
            BgColor = new Color(1, 1, 1),
            BorderColor = new Color(0.2f, 0.2f, 0.2f),
        };
        style.SetCornerRadiusAll(6);
        style.SetBorderWidthAll(2);
        AddThemeStyleboxOverride("panel", style);
    }

    public void SetFaceDown()
    {
        _rankLabel.Text = "";
        _suitLabel.Text = "";

        var style = new StyleBoxTexture
        {
            Texture = GetCardBackPattern(),
            AxisStretchHorizontal = StyleBoxTexture.AxisStretchMode.Stretch,
            AxisStretchVertical = StyleBoxTexture.AxisStretchMode.Stretch,
        };
        AddThemeStyleboxOverride("panel", style);
    }

    /// <summary>
    /// Generates (once) a tileable crosshatch pattern with rounded corners
    /// baked in, sized to match the card. Cached statically so the image is
    /// only rasterized on the first card back.
    /// </summary>
    private static ImageTexture GetCardBackPattern()
    {
        if (_cardBackPattern != null) return _cardBackPattern;

        const int w = 80;
        const int h = 120;
        const int spacing = 8;
        const int cornerRadius = 6;

        Color background = new(0.35f, 0.08f, 0.12f);   // dark burgundy red
		Color line = new(0.85f, 0.75f, 0.45f);          // muted gold

        Image img = Image.CreateEmpty(w, h, false, Image.Format.Rgba8);
        img.Fill(background);

        // Crosshatch: two sets of diagonal lines crossing to form a grid.
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int sum = (x + y) % spacing;
                int diff = ((x - y) % spacing + spacing) % spacing;
                if (sum == 0 || diff == 0)
                {
                    img.SetPixel(x, y, line);
                }
            }
        }

        // Round the corners by making pixels outside the rounded rect transparent.
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                if (!InsideRoundedRect(x, y, w, h, cornerRadius))
                {
                    img.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
        }

        _cardBackPattern = ImageTexture.CreateFromImage(img);
        return _cardBackPattern;
    }

    private static bool InsideRoundedRect(int x, int y, int w, int h, int r)
    {
        int cx = Mathf.Clamp(x, r, w - 1 - r);
        int cy = Mathf.Clamp(y, r, h - 1 - r);
        int dx = x - cx;
        int dy = y - cy;
        return dx * dx + dy * dy <= r * r;
    }

    private static string RankToString(Rank rank) => rank switch
    {
        Rank.Two => " 2", Rank.Three => " 3", Rank.Four => " 4", Rank.Five => " 5",
        Rank.Six => " 6", Rank.Seven => " 7", Rank.Eight => " 8", Rank.Nine => " 9",
        Rank.Ten => " 10", Rank.Jack => " J", Rank.Queen => " Q", Rank.King => " K",
        Rank.Ace => " A", _ => "?"
    };

    private static string SuitToString(Suit suit) => suit switch
    {
        Suit.Hearts => "♥", Suit.Diamonds => "♦",
        Suit.Clubs => "♣", Suit.Spades => "♠", _ => "?"
    };
}
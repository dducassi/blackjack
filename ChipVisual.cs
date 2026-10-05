using Godot;

namespace Blackjack;

/// <summary>
/// A single casino chip rendered as concentric circles:
///   - outermost rim (uniform dark grey, except $5 which uses black)
///   - a ring in the chip's main color
///   - a white border (black for the $5 chip)
///   - an inner fill in the chip's main color where the denomination sits
/// </summary>
public partial class ChipVisual : Control
{
    public const int ChipSize = 50;
    private const int BorderWidth = 3;       // width of the white (or black) ring
    private const int OuterRingWidth = 3;    // ring in the chip's main color, outside the white border
    private const int RimWidth = 1;          // outermost rim

    public int Denomination { get; private set; }

    private Label _label = null!;

    public override void _Ready()
    {
        CustomMinimumSize = new Vector2(ChipSize, ChipSize);
        Size = new Vector2(ChipSize, ChipSize);
        MouseFilter = MouseFilterEnum.Ignore;

        _label = new Label
        {
            Position = Vector2.Zero,
            Size = new Vector2(ChipSize, ChipSize),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore,
        };
        AddChild(_label);

        UpdateLabel();
        QueueRedraw();
    }

    public void SetDenomination(int denom)
    {
        Denomination = denom;
        if (IsNodeReady())
        {
            UpdateLabel();
            QueueRedraw();
        }
    }

    private void UpdateLabel()
    {
        var (_, _, text, _) = GetColors();
        _label.Text = $"${Denomination}";
        _label.AddThemeColorOverride("font_color", text);
        _label.AddThemeFontSizeOverride("font_size", 14);
    }

    public override void _Draw()
    {
        var (fill, border, _, rim) = GetColors();
        Vector2 center = new Vector2(ChipSize / 2f, ChipSize / 2f);
        float outerR = ChipSize / 2f;

        // Outermost rim.
        DrawCircle(center, outerR, rim, true, -1f, true);

        // Chip's main color ring inside the rim.
        DrawCircle(center, outerR - RimWidth, fill, true, -1f, true);

        // White (or black for $5) border inside that.
        DrawCircle(center, outerR - RimWidth - OuterRingWidth, border, true, -1f, true);

        // Inner fill in the chip's main color.
        DrawCircle(center, outerR - RimWidth - OuterRingWidth - BorderWidth, fill, true, -1f, true);
    }

    private (Color fill, Color border, Color text, Color rim) GetColors()
    {
        var black = new Color(0f, 0f, 0f);
        var white = new Color(1f, 1f, 1f);
        var rim = new Color(0.25f, 0.25f, 0.25f);   // uniform dark grey rim

        return Denomination switch
        {
            5   => (white,                          black, black, rim),
            10  => (new Color(0.75f, 0.15f, 0.15f), white, white, rim),
            20  => (new Color(0.15f, 0.35f, 0.75f), white, white, rim),
            50  => (new Color(0.15f, 0.6f, 0.3f),   white, white, rim),
            100 => (new Color(0.1f, 0.1f, 0.1f),    white, white, rim),
            _   => (new Color(0.5f, 0.5f, 0.5f),    white, white, rim)
        };
    }
}
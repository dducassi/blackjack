using Godot;

namespace Blackjack;

/// <summary>
/// A single casino chip rendered as concentric circles:
///   - outermost black outline
///   - a ring in the chip's main color
///   - a white border (black for the $5 chip)
///   - an inner fill in the chip's main color where the denomination sits
/// </summary>
public partial class ChipVisual : Control
{
    public const int ChipSize = 50;
    private const int BorderWidth = 3;       // width of the white (or black) ring
    private const int OuterRingWidth = 3;    // ring in the chip's main color, outside the white border
    private const int BlackOutlineWidth = 1; // outermost black outline

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
        var (_, _, text) = GetColors();
        _label.Text = $"${Denomination}";
        _label.AddThemeColorOverride("font_color", text);
        _label.AddThemeFontSizeOverride("font_size", 14);
    }

    public override void _Draw()
    {
        var (fill, border, _) = GetColors();
        Vector2 center = new Vector2(ChipSize / 2f, ChipSize / 2f);
        float outerR = ChipSize / 2f;
        var black = new Color(0f, 0f, 0f);

        // Outermost black outline.
        DrawCircle(center, outerR, black, true, -1f, true);

        // Chip's main color ring inside the black outline.
        DrawCircle(center, outerR - BlackOutlineWidth, fill, true, -1f, true);

        // White (or black for $5) border inside that.
        DrawCircle(center, outerR - BlackOutlineWidth - OuterRingWidth, border, true, -1f, true);

        // Inner fill in the chip's main color.
        DrawCircle(center, outerR - BlackOutlineWidth - OuterRingWidth - BorderWidth, fill, true, -1f, true);
    }

    private (Color fill, Color border, Color text) GetColors()
    {
        return Denomination switch
        {
            5   => (new Color(1f, 1f, 1f),          new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)),
            10  => (new Color(0.75f, 0.15f, 0.15f), new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)),
            20  => (new Color(0.15f, 0.35f, 0.75f), new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)),
            50  => (new Color(0.15f, 0.6f, 0.3f),   new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)),
            100 => (new Color(0.1f, 0.1f, 0.1f),    new Color(1f, 1f, 1f), new Color(1f, 1f, 1f)),
            _   => (new Color(0.5f, 0.5f, 0.5f),    new Color(1f, 1f, 1f), new Color(1f, 1f, 1f))
        };
    }
}
using Godot;

namespace Blackjack;

/// <summary>
/// Draws a shallow upward-bowing arc — the classic blackjack table line.
/// The arc's endpoints sit near the bottom of the control, and the apex
/// bows up by ArcDepth pixels.
/// </summary>
public partial class BettingArc : Control
{
    [Export] public Color LineColor { get; set; } = new Color(1, 1, 1);
    [Export] public float LineWidth { get; set; } = 2f;
    [Export] public float ArcDepth { get; set; } = 30f;
    [Export] public float BottomPadding { get; set; } = 5f;
    [Export] public int PointCount { get; set; } = 96;

    public override void _Draw()
    {
        float w = Size.X;
        float h = Size.Y;
        if (w <= 0 || h <= 0) return;

        float depth = Mathf.Min(ArcDepth, h - BottomPadding);
        if (depth <= 0) return;

        float y0 = h - BottomPadding;

        // Compute the circle that passes through the arc's endpoints at
        // (0, y0) and (w, y0) with an apex at (w/2, y0 - depth).
        // Sagitta-based radius formula: R = ((w/2)^2 + depth^2) / (2*depth).
        float halfW = w / 2f;
        float radius = (halfW * halfW + depth * depth) / (2f * depth);
        Vector2 center = new Vector2(halfW, y0 - radius + depth);

        float startAngle = Mathf.Atan2(y0 - center.Y, 0f - center.X);
        float endAngle   = Mathf.Atan2(y0 - center.Y, w - center.X);

        DrawArc(center, radius, startAngle, endAngle, PointCount, LineColor, LineWidth, true);
    }
}
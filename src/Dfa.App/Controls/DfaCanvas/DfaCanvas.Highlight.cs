namespace Dfa.App.Controls;

using System.Globalization;
using Avalonia;
using Avalonia.Media;
using Dfa.Core.Model;

// E-05: highlighting for the simulation. Only the style of the existing elements changes:
// a thick outline in the theme's color, plus ✓ / ✗ next to an accepted / rejected state.
public sealed partial class DfaCanvas
{
    private const double HighlightThickness = 4; // Stroke.Highlight (contract 3)
    private const double LastEdgeThickness = 3; // Stroke.LastEdge
    private const double VerdictFontSize = 20; // FontSize.Verdict

    // The theme brush and the mark (contract 3, visual conventions) for each kind of highlight.
    public static (string BrushKey, string? Mark) HighlightStyle(HighlightKind kind) => kind switch
    {
        HighlightKind.Accepted => ("Brush.Accepted", "✓"),
        HighlightKind.Rejected => ("Brush.Rejected", "✗"),
        _ => ("Brush.StateCurrent", null),
    };

    // The arrow just taken: thick and in Brush.EdgeLast; null when the edge is not highlighted.
    private IBrush? LastEdgeBrush(Edge edge, IBrush fallback) =>
        HighlightedEdge == new EdgeRef(edge.From, edge.To) ? ThemeBrush.Find(this, "Brush.EdgeLast", fallback) : null;

    private void DrawStateHighlight(DrawingContext context, string state, Point center, IBrush fallback)
    {
        if (state != HighlightedState || HighlightKind == HighlightKind.None)
        {
            return;
        }

        var (key, mark) = HighlightStyle(HighlightKind);
        var brush = ThemeBrush.Find(this, key, fallback);
        context.DrawEllipse(null, new Pen(brush, HighlightThickness), center, CanvasGeometry.Radius, CanvasGeometry.Radius);

        if (mark is not null)
        {
            var text = new FormattedText(
                mark, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface(FontFamily, FontStyle.Normal, FontWeight.Bold), VerdictFontSize, brush);
            context.DrawText(text, center + new Vector(CanvasGeometry.Radius, -CanvasGeometry.Radius - (text.Height / 2)));
        }
    }
}

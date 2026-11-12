namespace Dfa.App.Controls;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Dfa.Core.Documents;
using Dfa.Core.Model;

// Painting only: the whole automaton is repainted from Document on every change (a handful of shapes, so cheap).
public sealed partial class DfaCanvas
{
    // Values from the design tokens (DESIGN_RULES.md §2); colors and fonts come from resources at render time.
    private const double LabelFontSize = 14; // FontSize.CanvasLabel
    private const double EdgeThickness = 1.5; // Stroke.Edge (contract 3)
    private const double SelectedThickness = 3; // Stroke.LastEdge

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Document is not { } document)
        {
            return;
        }

        var brush = Foreground ?? Brushes.Black;
        var pen = new Pen(brush, EdgeThickness);
        var typeface = new Typeface(FontFamily);
        var descriptionBrush = ThemeBrush.Find(this, "Brush.TextMuted", brush);
        var dfa = document.Dfa;
        var edges = dfa.Edges;

        foreach (var edge in edges)
        {
            // The arrow just taken (E-05) and the selected arrow (E-03) are drawn thicker, not by color alone.
            var shape = ShapeOf(document, edge, edges);
            if (LastEdgeBrush(edge, brush) is { } last)
            {
                DrawEdge(context, new Pen(last, LastEdgeThickness), last, typeface, shape, SymbolList.Format(edge.Symbols));
                continue;
            }

            var edgePen = _selectedEdge == new EdgeRef(edge.From, edge.To) ? new Pen(brush, SelectedThickness) : pen;
            DrawEdge(context, edgePen, brush, typeface, shape, SymbolList.Format(edge.Symbols));
        }

        foreach (var state in dfa.States)
        {
            var center = Center(document, state);
            context.DrawEllipse(null, pen, center, CanvasGeometry.Radius, CanvasGeometry.Radius);
            if (state == SelectedState)
            {
                var dashed = new Pen(brush, EdgeThickness) { DashStyle = DashStyle.Dash };
                context.DrawEllipse(null, dashed, center, CanvasGeometry.Radius + 5, CanvasGeometry.Radius + 5);
            }

            if (dfa.IsFinal(state))
            {
                context.DrawEllipse(null, pen, center, CanvasGeometry.FinalRadius, CanvasGeometry.FinalRadius);
            }

            DrawStateHighlight(context, state, center, brush);
            DrawText(context, brush, typeface, state, center);
            if (document.Descriptions.TryGetValue(state, out var description))
            {
                DrawDescription(context, descriptionBrush, typeface, description, center);
            }
        }

        if (dfa.StartState is { } start)
        {
            var end = Center(document, start) - new Vector(CanvasGeometry.Radius, 0);
            var begin = end - new Vector(CanvasGeometry.StartArrowLength, 0);
            var label = begin - new Vector(0, LabelFontSize);
            DrawEdge(context, pen, brush, typeface, new EdgeShape(begin, begin, end, end, label), "start");
        }

        // Shift + drag: the new transition follows the mouse until it is released on a state.
        if (_link is { } link)
        {
            context.DrawLine(new Pen(brush, EdgeThickness) { DashStyle = DashStyle.Dash }, Center(document, link.From), link.To);
        }
    }

    // The canvas is as big as its drawing (plus room for loops and labels), so a ScrollViewer can reach every state.
    protected override Size MeasureOverride(Size availableSize)
    {
        return Document is { } document ? DrawingSize(document) : default;
    }

    // Shared with the SVG export, so the image has the same size as the canvas.
    public static Size DrawingSize(DfaDocument document)
    {
        if (document.Layout.Count == 0)
        {
            return default;
        }

        var margin = CanvasGeometry.Radius * 2;
        var below = document.Descriptions.Count > 0 ? margin + DescriptionRoom : margin;
        return new Size(document.Layout.Values.Max(p => p.X) + margin, document.Layout.Values.Max(p => p.Y) + below);
    }

    private EdgeShape ShapeOf(DfaDocument document, Edge edge, IReadOnlyList<Edge> edges) =>
        CanvasGeometry.ShapeOf(edge, edges, document.Dfa.States, state => Center(document, state));

    // While a state is dragged it is drawn at the mouse; MoveState happens only on release (E-03).
    private Point Center(DfaDocument document, string state)
    {
        if (_drag is { } drag && drag.State == state)
        {
            return drag.Position;
        }

        return document.Layout.TryGetValue(state, out var p) ? new Point(p.X, p.Y) : default;
    }

    private static void DrawEdge(DrawingContext context, Pen pen, IBrush brush, Typeface typeface, EdgeShape shape, string label)
    {
        var line = new StreamGeometry();
        using (var c = line.Open())
        {
            c.BeginFigure(shape.Start, false);
            c.CubicBezierTo(shape.Control1, shape.Control2, shape.End);
            c.EndFigure(false);
        }

        var (tip, left, right) = CanvasGeometry.ArrowHead(shape);
        var head = new StreamGeometry();
        using (var c = head.Open())
        {
            c.BeginFigure(tip, true);
            c.LineTo(left);
            c.LineTo(right);
            c.EndFigure(true);
        }

        context.DrawGeometry(null, pen, line);
        context.DrawGeometry(brush, null, head);
        DrawText(context, brush, typeface, label, shape.Label);
    }

    private static void DrawText(DrawingContext context, IBrush brush, Typeface typeface, string text, Point center)
    {
        var formatted = new FormattedText(
            text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, LabelFontSize, brush);
        context.DrawText(formatted, center - new Vector(formatted.Width / 2, formatted.Height / 2));
    }
}

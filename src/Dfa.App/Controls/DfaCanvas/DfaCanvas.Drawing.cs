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
    private const double LabelFontSize = 14;

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (Document is not { } document)
        {
            return;
        }

        var brush = Foreground ?? Brushes.Black;
        var pen = new Pen(brush, 1.5);
        var dfa = document.Dfa;
        var edges = dfa.Edges;

        foreach (var edge in edges)
        {
            // The arrow just taken (E-05) and the selected arrow (E-03) are drawn thicker, not by color alone.
            var shape = ShapeOf(document, edge, edges);
            if (LastEdgeBrush(edge, brush) is { } last)
            {
                DrawEdge(context, new Pen(last, LastEdgeThickness), last, shape, SymbolList.Format(edge.Symbols));
                continue;
            }

            var edgePen = _selectedEdge == new EdgeRef(edge.From, edge.To) ? new Pen(brush, 3) : pen;
            DrawEdge(context, edgePen, brush, shape, SymbolList.Format(edge.Symbols));
        }

        foreach (var state in dfa.States)
        {
            var center = Center(document, state);
            context.DrawEllipse(null, pen, center, CanvasGeometry.Radius, CanvasGeometry.Radius);
            if (state == SelectedState)
            {
                var dashed = new Pen(brush, 1.5) { DashStyle = DashStyle.Dash };
                context.DrawEllipse(null, dashed, center, CanvasGeometry.Radius + 5, CanvasGeometry.Radius + 5);
            }

            if (dfa.IsFinal(state))
            {
                context.DrawEllipse(null, pen, center, CanvasGeometry.FinalRadius, CanvasGeometry.FinalRadius);
            }

            DrawStateHighlight(context, state, center, brush);
            DrawText(context, brush, state, center);
            if (document.Descriptions.TryGetValue(state, out var description))
            {
                DrawDescription(context, Brushes.Gray, description, center);
            }
        }

        if (dfa.StartState is { } start)
        {
            var end = Center(document, start) - new Vector(CanvasGeometry.Radius, 0);
            var begin = end - new Vector(CanvasGeometry.StartArrowLength, 0);
            var label = begin - new Vector(0, LabelFontSize);
            DrawEdge(context, pen, brush, new EdgeShape(begin, begin, end, end, label), "start");
        }

        // Shift + drag: the new transition follows the mouse until it is released on a state.
        if (_link is { } link)
        {
            context.DrawLine(new Pen(brush, 1.5) { DashStyle = DashStyle.Dash }, Center(document, link.From), link.To);
        }
    }

    // The canvas is as big as its drawing (plus room for loops and labels), so a ScrollViewer can reach every state.
    protected override Size MeasureOverride(Size availableSize)
    {
        if (Document is not { Layout.Count: > 0 } document)
        {
            return default;
        }

        var margin = CanvasGeometry.Radius * 2;
        var below = document.Descriptions.Count > 0 ? margin + DescriptionRoom : margin;
        return new Size(document.Layout.Values.Max(p => p.X) + margin, document.Layout.Values.Max(p => p.Y) + below);
    }

    private EdgeShape ShapeOf(DfaDocument document, Edge edge, IReadOnlyList<Edge> edges)
    {
        var from = Center(document, edge.From);
        var to = Center(document, edge.To);
        if (edge.From == edge.To)
        {
            return CanvasGeometry.Loop(from);
        }

        var blocked = document.Dfa.States.Any(s =>
            s != edge.From && s != edge.To && CanvasGeometry.PassesThrough(from, to, Center(document, s)));
        if (blocked)
        {
            return CanvasGeometry.Curved(from, to, CanvasGeometry.AvoidBend);
        }

        return edges.Any(e => e.From == edge.To && e.To == edge.From)
            ? CanvasGeometry.Curved(from, to)
            : CanvasGeometry.Straight(from, to);
    }

    // While a state is dragged it is drawn at the mouse; MoveState happens only on release (E-03).
    private Point Center(DfaDocument document, string state)
    {
        if (_drag is { } drag && drag.State == state)
        {
            return drag.Position;
        }

        return document.Layout.TryGetValue(state, out var p) ? new Point(p.X, p.Y) : default;
    }

    private static void DrawEdge(DrawingContext context, Pen pen, IBrush brush, EdgeShape shape, string label)
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
        DrawText(context, brush, label, shape.Label);
    }

    private static void DrawText(DrawingContext context, IBrush brush, string text, Point center)
    {
        var formatted = new FormattedText(
            text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Typeface.Default, LabelFontSize, brush);
        context.DrawText(formatted, center - new Vector(formatted.Width / 2, formatted.Height / 2));
    }
}

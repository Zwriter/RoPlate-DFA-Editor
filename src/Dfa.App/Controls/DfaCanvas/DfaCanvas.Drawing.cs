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
            DrawEdge(context, pen, brush, ShapeOf(document, edge, edges), string.Join(", ", edge.Symbols));
        }

        foreach (var state in dfa.States)
        {
            var center = Center(document, state);
            context.DrawEllipse(null, pen, center, CanvasGeometry.Radius, CanvasGeometry.Radius);
            if (dfa.IsFinal(state))
            {
                context.DrawEllipse(null, pen, center, CanvasGeometry.FinalRadius, CanvasGeometry.FinalRadius);
            }

            DrawText(context, brush, state, center);
        }

        if (dfa.StartState is { } start)
        {
            var end = Center(document, start) - new Vector(CanvasGeometry.Radius, 0);
            var begin = end - new Vector(CanvasGeometry.StartArrowLength, 0);
            var label = begin - new Vector(0, LabelFontSize);
            DrawEdge(context, pen, brush, new EdgeShape(begin, begin, end, end, label), "start");
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
        return new Size(document.Layout.Values.Max(p => p.X) + margin, document.Layout.Values.Max(p => p.Y) + margin);
    }

    private static EdgeShape ShapeOf(DfaDocument document, Edge edge, IReadOnlyList<Edge> edges)
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

    private static Point Center(DfaDocument document, string state) =>
        document.Layout.TryGetValue(state, out var p) ? new Point(p.X, p.Y) : default;

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

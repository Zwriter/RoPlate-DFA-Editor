namespace Dfa.App.Controls;

using System;
using Avalonia;

// Pure geometry of the drawing (contract 3, visual conventions), kept apart from painting so it can be tested.
public static class CanvasGeometry
{
    public const double Radius = 28;
    public const double FinalRadius = 22;
    public const double StartArrowLength = 40;

    public const double AvoidBend = Radius * 2.5;

    private const double Bend = 28;
    private const double LabelGap = 12;
    private const double ArrowLength = 10;
    private const double ArrowHalfWidth = 5;

    // Arrows start and end on the circles, along the direction between the centers.
    public static EdgeShape Straight(Point from, Point to)
    {
        var d = Unit(to - from);
        var start = from + (d * Radius);
        var end = to - (d * Radius);
        return new EdgeShape(start, start, end, end, Mid(start, end) + (Normal(d) * LabelGap));
    }

    // True when the straight arrow from → to would cross the circle of another state.
    public static bool PassesThrough(Point from, Point to, Point other)
    {
        var d = (Vector)(to - from);
        var t = Math.Clamp(Vector.Dot(other - from, d) / Math.Max(d.SquaredLength, 1e-9), 0, 1);
        return ((Vector)(other - (from + (d * t)))).Length < Radius;
    }

    // Used when both directions exist: each arc bends to its own left, so the two never overlap.
    // A larger bend takes an arrow around a state that lies on the straight line.
    public static EdgeShape Curved(Point from, Point to, double bend = Bend)
    {
        var n = Normal(Unit(to - from));
        var apex = Mid(from, to) + (n * bend);
        var control = Mid(from, to) + (n * bend * 2); // a quadratic curve through apex
        var start = from + (Unit(control - from) * Radius);
        var end = to + (Unit(control - to) * Radius);
        return new EdgeShape(
            start, start + ((control - start) * 2 / 3), end + ((control - end) * 2 / 3), end, apex + (n * LabelGap));
    }

    // A loop is an arc above the state, leaving and entering the circle 30° either side of the top.
    public static EdgeShape Loop(Point center)
    {
        var side = Radius / 2;
        var up = Radius * Math.Sqrt(3) / 2;
        return new EdgeShape(
            new Point(center.X - side, center.Y - up),
            new Point(center.X - Radius * 1.3, center.Y - Radius * 2.8),
            new Point(center.X + Radius * 1.3, center.Y - Radius * 2.8),
            new Point(center.X + side, center.Y - up),
            new Point(center.X, center.Y - Radius * 2.6 - LabelGap));
    }

    // The arrowhead triangle (tip, left, right), pointing along the curve's last tangent.
    public static (Point Tip, Point Left, Point Right) ArrowHead(EdgeShape edge)
    {
        var d = Unit(edge.End - (edge.Control2 == edge.End ? edge.Start : edge.Control2));
        var back = edge.End - (d * ArrowLength);
        var n = Normal(d) * ArrowHalfWidth;
        return (edge.End, back + n, back - n);
    }

    private static Point Mid(Point a, Point b) => new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

    private static Vector Normal(Vector d) => new(d.Y, -d.X);

    // Two states on the same spot have no direction; any fixed one keeps the drawing finite.
    private static Vector Unit(Vector v) => v.Length < 1e-9 ? new Vector(1, 0) : v / v.Length;
}

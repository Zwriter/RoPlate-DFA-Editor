namespace Dfa.App.Controls;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Avalonia;
using Dfa.Core.Documents;
using Dfa.Core.Model;

// E-08 (ED-12, optional part): the drawing as an SVG, built from the same CanvasGeometry as the canvas.
// Black on white like the PNG (DESIGN_RULES.md: the export is theme-independent). No selection or highlight.
public static class DfaCanvasSvg
{
    private const string Ink = "black";
    private const string Muted = "gray";
    private const string Font = "Inter, Segoe UI, sans-serif"; // Font.Body
    private const double LabelFontSize = 14; // FontSize.CanvasLabel
    private const double DescriptionFontSize = 11; // FontSize.Caption
    private const double EdgeThickness = 1.5; // Stroke.Edge
    private const double DescriptionGap = 6;
    private const int DescriptionLineChars = 24; // about the canvas's 140 px wrap width at 11 px
    private static readonly XNamespace _svg = "http://www.w3.org/2000/svg";

    public static OpResult SaveSvg(DfaDocument document, string path)
    {
        try
        {
            ToSvg(document).Save(path);
            return OpResult.Success;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            return OpResult.Fail($"Could not save the file: {e.Message}");
        }
    }

    public static XDocument ToSvg(DfaDocument document)
    {
        var size = DfaCanvas.DrawingSize(document);
        var dfa = document.Dfa;
        var root = new XElement(
            _svg + "svg",
            new XAttribute("width", Num(size.Width)),
            new XAttribute("height", Num(size.Height)),
            new XAttribute("viewBox", $"0 0 {Num(size.Width)} {Num(size.Height)}"),
            new XAttribute("font-family", Font),
            new XElement(_svg + "rect", new XAttribute("width", "100%"), new XAttribute("height", "100%"), new XAttribute("fill", "white")));

        Point Center(string state) => document.Layout.TryGetValue(state, out var p) ? new Point(p.X, p.Y) : default;

        var edges = dfa.Edges;
        foreach (var edge in edges)
        {
            root.Add(Edge(CanvasGeometry.ShapeOf(edge, edges, dfa.States, Center), SymbolList.Format(edge.Symbols)));
        }

        foreach (var state in dfa.States)
        {
            var center = Center(state);
            root.Add(Circle(center, CanvasGeometry.Radius));
            if (dfa.IsFinal(state))
            {
                root.Add(Circle(center, CanvasGeometry.FinalRadius));
            }

            root.Add(Text(state, center, LabelFontSize, Ink));
            if (document.Descriptions.TryGetValue(state, out var description))
            {
                root.Add(Description(description, center));
            }
        }

        if (dfa.StartState is { } start)
        {
            var end = Center(start) - new Vector(CanvasGeometry.Radius, 0);
            var begin = end - new Vector(CanvasGeometry.StartArrowLength, 0);
            root.Add(Edge(new EdgeShape(begin, begin, end, end, begin - new Vector(0, LabelFontSize)), "start"));
        }

        return new XDocument(root);
    }

    // Splits a description into lines of at most `width` characters, at spaces when it can.
    public static IReadOnlyList<string> Wrap(string text, int width)
    {
        var lines = new List<string>();
        var line = "";
        foreach (var word in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (line.Length > 0 && line.Length + 1 + word.Length > width)
            {
                lines.Add(line);
                line = "";
            }

            line = line.Length == 0 ? word : $"{line} {word}";
        }

        if (line.Length > 0)
        {
            lines.Add(line);
        }

        return lines;
    }

    private static IEnumerable<XElement> Edge(EdgeShape shape, string label)
    {
        var (tip, left, right) = CanvasGeometry.ArrowHead(shape);
        yield return new XElement(
            _svg + "path",
            new XAttribute("d", $"M {Pt(shape.Start)} C {Pt(shape.Control1)} {Pt(shape.Control2)} {Pt(shape.End)}"),
            new XAttribute("fill", "none"),
            new XAttribute("stroke", Ink),
            new XAttribute("stroke-width", Num(EdgeThickness)));
        yield return new XElement(_svg + "polygon", new XAttribute("points", $"{Pt(tip)} {Pt(left)} {Pt(right)}"), new XAttribute("fill", Ink));
        yield return Text(label, shape.Label, LabelFontSize, Ink);
    }

    private static XElement Circle(Point center, double radius) => new(
        _svg + "circle",
        new XAttribute("cx", Num(center.X)),
        new XAttribute("cy", Num(center.Y)),
        new XAttribute("r", Num(radius)),
        new XAttribute("fill", "none"),
        new XAttribute("stroke", Ink),
        new XAttribute("stroke-width", Num(EdgeThickness)));

    private static XElement Text(string text, Point center, double fontSize, string fill) => new(
        _svg + "text",
        new XAttribute("x", Num(center.X)),
        new XAttribute("y", Num(center.Y)),
        new XAttribute("font-size", Num(fontSize)),
        new XAttribute("fill", fill),
        new XAttribute("text-anchor", "middle"),
        new XAttribute("dominant-baseline", "central"),
        text);

    private static XElement Description(string text, Point center)
    {
        var top = center.Y + CanvasGeometry.Radius + DescriptionGap + DescriptionFontSize;
        var element = Text("", new Point(center.X, top), DescriptionFontSize, Muted);
        element.SetAttributeValue("dominant-baseline", null);
        element.Add(Wrap(text, DescriptionLineChars).Select((line, i) => new XElement(
            _svg + "tspan",
            new XAttribute("x", Num(center.X)),
            new XAttribute("dy", i == 0 ? "0" : Num(DescriptionFontSize * 1.3)),
            line)));
        return element;
    }

    private static string Pt(Point p) => $"{Num(p.X)},{Num(p.Y)}";

    private static string Num(double value) => value.ToString("0.##", CultureInfo.InvariantCulture);
}

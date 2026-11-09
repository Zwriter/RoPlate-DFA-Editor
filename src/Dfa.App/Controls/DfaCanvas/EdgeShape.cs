namespace Dfa.App.Controls;

using Avalonia;

// One arrow as a cubic Bézier (straight arrows have their control points on the ends), plus where its label goes.
public sealed record EdgeShape(Point Start, Point Control1, Point Control2, Point End, Point Label);

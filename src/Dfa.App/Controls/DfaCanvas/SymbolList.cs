namespace Dfa.App.Controls;

using System;
using System.Collections.Generic;

// The text typed for an arrow's symbols ("e.g. a,b") and back, as the arrow label shows it ("a, b").
public static class SymbolList
{
    public static IReadOnlyList<string> Parse(string text) =>
        [.. text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)];

    public static string Format(IEnumerable<string> symbols) => string.Join(", ", symbols);
}

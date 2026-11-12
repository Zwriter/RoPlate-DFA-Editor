namespace Dfa.App.Views.TransitionTable;

using System.Collections.Generic;

// One δ cell: δ(State, Symbol) = Target (null when missing), and the targets it can be changed to (ED-11).
public sealed record TransitionCell(string State, string Symbol, string? Target, IReadOnlyList<TransitionChoice> Choices)
{
    public string Name => $"δ({State}, {Symbol})";
}

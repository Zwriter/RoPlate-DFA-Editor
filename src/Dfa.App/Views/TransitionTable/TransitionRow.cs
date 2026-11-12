namespace Dfa.App.Views.TransitionTable;

using System.Collections.Generic;

// One row of the δ table: the state label (with → and *) and one cell per symbol of Σ.
public sealed record TransitionRow(string Label, IReadOnlyList<TransitionCell> Cells);

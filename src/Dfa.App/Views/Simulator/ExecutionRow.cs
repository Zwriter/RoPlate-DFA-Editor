namespace Dfa.App.Views.Simulator;

using System.Collections.Generic;
using Dfa.Core.Simulation;

// One row of the execution table (S-03): Step, Read, Class, Remaining, State.
public sealed record ExecutionRow(int Step, string Read, string Class, string Remaining, string State, bool IsCurrent)
{
    // Row 0 is the initial configuration; only the rows up to the current position are shown.
    public static IReadOnlyList<ExecutionRow> For(DfaRun run)
    {
        var rows = new List<ExecutionRow>
        {
            new(0, "", "", Show(run.Input), run.Dfa.StartState ?? "", run.IsAtStart),
        };

        for (var i = 0; i < run.Position; i++)
        {
            var step = run.Steps[i];
            rows.Add(new ExecutionRow(
                step.Number,
                Show(step.Character.ToString()),
                step.Symbol ?? "",
                Show(run.Input[(step.Position + 1)..]),
                step.To ?? step.From,
                step.Number == run.Position));
        }

        return rows;
    }

    // Nothing left to read is ε; a space is ␣, as on the input tape.
    private static string Show(string text) => text.Length == 0 ? "ε" : text.Replace(' ', '␣');
}

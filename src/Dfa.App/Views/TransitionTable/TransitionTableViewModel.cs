namespace Dfa.App.Views.TransitionTable;

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Services;
using Dfa.Core.Documents;

public sealed partial class TransitionTableViewModel : ObservableObject
{
    private readonly DocumentSession _session;

    [ObservableProperty]
    private IReadOnlyList<string> _symbols = [];

    private const string Missing = "—";

    [ObservableProperty]
    private IReadOnlyList<TransitionRow> _rows = [];

    // The OpResult message of a refused cell edit, shown below the table; null hides it.
    [ObservableProperty]
    private string? _error;

    public TransitionTableViewModel(DocumentSession session)
    {
        _session = session;
        session.DocumentReplaced += (_, _) => Refresh();
        session.DocumentEdited += (_, e) =>
        {
            // Only δ, Σ, the states and start/final show in the table; moves and descriptions do not.
            if (e.Kind is DocumentChangeKind.Structure or DocumentChangeKind.Reset)
            {
                Refresh();
            }
        };
        Refresh();
    }

    private void Refresh()
    {
        var dfa = _session.Document.Dfa;
        Symbols = [.. dfa.Alphabet];
        Rows = [.. dfa.States.Select(state => new TransitionRow(
            $"{(dfa.StartState == state ? "→" : "")}{(dfa.FinalStates.Contains(state) ? "*" : "")}{state}",
            [.. dfa.Alphabet.Select(symbol => Cell(state, symbol, dfa.Next(state, symbol)))]))];
    }

    // ED-11: the cell's menu lists "—" (no transition) and every state; picking one is a single, undoable edit.
    private TransitionCell Cell(string state, string symbol, string? target)
    {
        IEnumerable<string?> targets = [null, .. _session.Document.Dfa.States];
        return new TransitionCell(state, symbol, target, [.. targets.Select(to => new TransitionChoice(
            to ?? Missing,
            to == target,
            new RelayCommand(() => Error = _session.Document.SetTransition(state, symbol, to).Error)))]);
    }
}

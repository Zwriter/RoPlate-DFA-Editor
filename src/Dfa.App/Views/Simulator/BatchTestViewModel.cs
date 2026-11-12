namespace Dfa.App.Views.Simulator;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Simulation;

// The batch testing panel (S-07, SI-08). The list is saved with the automaton through Document.SetTests.
public sealed partial class BatchTestViewModel : ObservableObject
{
    private const int GeneratedCount = 10;
    private const int GeneratedMaxLength = 12;

    private readonly DocumentSession _session;
    private bool _saving;

    [ObservableProperty]
    private string? _summary;

    // SI-11: the shortest accepted words, read-only; null until asked for and again after the automaton changes.
    [ObservableProperty]
    private string? _acceptedWords;

    public BatchTestViewModel(DocumentSession session)
    {
        _session = session;
        session.DocumentReplaced += (_, _) =>
        {
            AcceptedWords = null;
            Load();
        };
        session.DocumentEdited += (_, e) =>
        {
            if (e.Kind is DocumentChangeKind.Structure or DocumentChangeKind.Reset)
            {
                AcceptedWords = null;
            }

            // Our own SetTests: keep the rows (and the focus in the edited one), only the results are stale.
            if (_saving)
            {
                return;
            }

            // Moves and descriptions do not change any verdict.
            if (e.Kind is DocumentChangeKind.Tests or DocumentChangeKind.Reset)
            {
                Load();
            }
            else if (e.Kind is DocumentChangeKind.Structure)
            {
                ClearResults();
            }
        };
        Load();
    }

    public ObservableCollection<BatchTestRow> Rows { get; } = [];

    // Double-click on a row: the window opens the word in the simulator.
    public event EventHandler<string>? OpenInSimulatorRequested;

    [RelayCommand]
    private void Add()
    {
        Rows.Add(NewRow(new TestCase("", false)));
        Save();
    }

    [RelayCommand]
    private void Remove(BatchTestRow row)
    {
        Rows.Remove(row);
        Save();
    }

    [RelayCommand]
    private void RunTests()
    {
        var result = BatchTester.Run(_session.Document.Dfa, Rows.Select(r => r.ToTestCase()));
        for (var i = 0; i < Rows.Count; i++)
        {
            Rows[i].Passed = result.Results[i].Passed;
        }

        Summary = result.Summary;
    }

    [RelayCommand]
    private void GenerateWords()
    {
        var words = LanguageGenerator.FirstAccepted(_session.Document.Dfa, GeneratedCount, GeneratedMaxLength);
        AcceptedWords = words.Count == 0
            ? $"No accepted words up to length {GeneratedMaxLength}."
            : string.Join(", ", words.Select(w => w.Length == 0 ? "ε" : w));
    }

    [RelayCommand]
    private void OpenInSimulator(BatchTestRow row) => OpenInSimulatorRequested?.Invoke(this, row.Word);

    private void Load()
    {
        Rows.Clear();
        foreach (var testCase in _session.Document.Tests)
        {
            Rows.Add(NewRow(testCase));
        }

        Summary = null;
    }

    private BatchTestRow NewRow(TestCase testCase) => new(testCase, Save);

    private void Save()
    {
        ClearResults();
        _saving = true;
        try
        {
            _session.Document.SetTests(Rows.Select(r => r.ToTestCase()));
        }
        finally
        {
            _saving = false;
        }
    }

    private void ClearResults()
    {
        foreach (var row in Rows)
        {
            row.Passed = null;
        }

        Summary = null;
    }
}

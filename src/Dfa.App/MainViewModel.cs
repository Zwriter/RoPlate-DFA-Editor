namespace Dfa.App;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Controls;
using Dfa.App.Services;
using Dfa.App.Views.Editor;
using Dfa.App.Views.Simulator;
using Dfa.App.Views.TransitionTable;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Dfa.Core.Serialization;

public sealed partial class MainViewModel : ObservableObject
{
    private const double TrapGap = 150;

    private readonly IDialogService _dialogs;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public MainViewModel(IDialogService dialogs)
    {
        _dialogs = dialogs;
        Navigation = new NavigationService(this, Session);
        Editor = new EditorViewModel(Session, dialogs);
        Table = new TransitionTableViewModel(Session);
        Simulator = new SimulatorViewModel(Session, dialogs);
        BatchTests = new BatchTestViewModel(Session);
        BatchTests.OpenInSimulatorRequested += (_, word) =>
        {
            Navigation.ShowTab(MainTab.Simulator);
            Simulator.Start(word);
        };

        Simulator.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SimulatorViewModel.RequestedCanvasMode))
            {
                OnPropertyChanged(nameof(CurrentCanvasMode));
            }
        };

        Session.PropertyChanged += OnSessionPropertyChanged;
        Session.DocumentReplaced += (_, _) => OnPropertyChanged(nameof(Title));
        Session.History.Changed += (_, _) =>
        {
            UndoCommand.NotifyCanExecuteChanged();
            RedoCommand.NotifyCanExecuteChanged();
        };
    }

    public DocumentSession Session { get; } = new();

    // S-06 (contract 3, section 6): the canvas follows the simulator only on the Simulate tab.
    public CanvasMode CurrentCanvasMode =>
        SelectedTabIndex == (int)MainTab.Simulator ? Simulator.RequestedCanvasMode : CanvasMode.Edit;

    public INavigationService Navigation { get; }

    public EditorViewModel Editor { get; }

    public TransitionTableViewModel Table { get; }

    public SimulatorViewModel Simulator { get; }

    public BatchTestViewModel BatchTests { get; }

    public IReadOnlyList<ExampleInfo> Examples => ExampleCatalog.All;

    public string Title => $"{Session.Document.Name}{(Session.IsDirty ? "*" : "")} — DFA Simulator";

    [RelayCommand]
    private async Task NewAsync()
    {
        if (await ConfirmDiscardAsync())
        {
            Session.Replace(DfaDocument.CreateEmpty(), null);
            StatusMessage = "New automaton.";
        }
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (!await ConfirmDiscardAsync() || await _dialogs.PickOpenFileAsync() is not { } path)
        {
            return;
        }

        var result = DfaJson.LoadFile(path);
        if (result.Document is null)
        {
            await _dialogs.ShowErrorsAsync("The file could not be opened", result.Errors);
            return;
        }

        Session.Replace(result.Document, path);
        StatusMessage = $"Opened {path}";
    }

    [RelayCommand]
    private async Task OpenExampleAsync(ExampleInfo example)
    {
        if (await ConfirmDiscardAsync())
        {
            Session.Replace(ExampleCatalog.Load(example.Id), null);
            StatusMessage = $"Example: {example.Title}";
        }
    }

    [RelayCommand]
    private Task SaveAsync() => SaveCoreAsync(Session.FilePath);

    [RelayCommand]
    private Task SaveAsAsync() => SaveCoreAsync(null);

    // E-08: the drawing as a PNG image.
    [RelayCommand]
    private async Task ExportPngAsync()
    {
        if (await _dialogs.PickSaveImageAsync(Session.Document.Name + ".png") is not { } path)
        {
            return;
        }

        var result = DfaCanvasImage.SavePng(Session.Document, path);
        if (!result.Ok)
        {
            await _dialogs.ShowErrorAsync(result.Error!);
            return;
        }

        StatusMessage = $"Exported {path}";
    }

    // DfaCanvas.StateClicked goes to the simulator (the "Guess" mode, S-08).
    [RelayCommand]
    private void StateClicked(string state) => Simulator.OnStateClicked(state);

    [RelayCommand]
    private void ShowRoPlate() => Navigation.ShowRoPlate();

    // E-06: undo / redo (Ctrl+Z / Ctrl+Y), enabled by CanUndo / CanRedo.
    [RelayCommand(CanExecute = nameof(CanUndo))]
    private void Undo() => Session.History.Undo();

    [RelayCommand(CanExecute = nameof(CanRedo))]
    private void Redo() => Session.History.Redo();

    private bool CanUndo() => Session.History.CanUndo;

    private bool CanRedo() => Session.History.CanRedo;

    // E-06: the trap state goes to the right of the drawing, at the states' average height.
    [RelayCommand]
    private void CompleteWithTrap()
    {
        var layout = Session.Document.Layout.Values;
        var position = Session.Document.Layout.Count == 0
            ? new Point2(TrapGap, TrapGap)
            : new Point2(layout.Max(p => p.X) + TrapGap, layout.Average(p => p.Y));
        if (Session.Document.CompleteWithTrap(position).Error is { } error)
        {
            StatusMessage = error;
        }
    }

    // DfaCanvas.StatusMessage (contract 3): errors and info while editing.
    [RelayCommand]
    private void ShowStatus(string text) => StatusMessage = text;

    // Returns false when the current document must be kept (unsaved changes and the save was cancelled or failed).
    // Also used by NavigationService.OpenInSimulator ("Show automaton" from RoPlate).
    public async Task<bool> ConfirmDiscardAsync() =>
        !Session.IsDirty || !await _dialogs.ConfirmAsync("Save changes?") || await SaveCoreAsync(Session.FilePath);

    private async Task<bool> SaveCoreAsync(string? path)
    {
        path ??= await _dialogs.PickSaveFileAsync(Session.Document.Name + ".dfa.json");
        if (path is null)
        {
            return false;
        }

        var result = DfaJson.SaveFile(Session.Document, path);
        if (!result.Ok)
        {
            await _dialogs.ShowErrorAsync(result.Error!);
            return false;
        }

        Session.FilePath = path;
        Session.IsDirty = false;
        StatusMessage = $"Saved {path}";
        return true;
    }

    // Leaving the Simulate tab stops the simulation, so nothing stays highlighted on the drawing.
    partial void OnSelectedTabIndexChanged(int value)
    {
        if (value != (int)MainTab.Simulator)
        {
            Simulator.Stop();
        }

        OnPropertyChanged(nameof(CurrentCanvasMode));
    }

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentSession.IsDirty))
        {
            OnPropertyChanged(nameof(Title));
        }
    }
}

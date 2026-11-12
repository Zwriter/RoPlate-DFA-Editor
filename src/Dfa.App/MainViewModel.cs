namespace Dfa.App;

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Services;
using Dfa.App.Views.Simulator;
using Dfa.App.Views.TransitionTable;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Dfa.Core.Serialization;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _statusMessage = "Ready";

    public MainViewModel(IDialogService dialogs)
    {
        _dialogs = dialogs;
        Navigation = new NavigationService(this, Session);
        Table = new TransitionTableViewModel(Session);
        Simulator = new SimulatorViewModel(Session, dialogs);
        BatchTests = new BatchTestViewModel(Session);
        BatchTests.OpenInSimulatorRequested += (_, word) =>
        {
            Navigation.ShowTab(MainTab.Simulator);
            Simulator.Start(word);
        };

        Session.PropertyChanged += OnSessionPropertyChanged;
        Session.DocumentReplaced += (_, _) => OnPropertyChanged(nameof(Title));
    }

    public DocumentSession Session { get; } = new();

    public INavigationService Navigation { get; }

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

    [RelayCommand]
    private void ShowRoPlate() => Navigation.ShowRoPlate();

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

    private void OnSessionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DocumentSession.IsDirty))
        {
            OnPropertyChanged(nameof(Title));
        }
    }
}

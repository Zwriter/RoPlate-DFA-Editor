namespace Dfa.App.Views.Editor;

using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Model;

// The alphabet and the symbol classes (E-04, ED-05, ED-10). Errors from OpResult appear below the field.
public sealed partial class EditorViewModel : ObservableObject
{
    private readonly DocumentSession _session;
    private readonly IDialogService _dialogs;

    [ObservableProperty]
    private IReadOnlyList<string> _symbols = [];

    [ObservableProperty]
    private IReadOnlyList<SymbolClass> _classes = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddSymbolCommand))]
    private string _newSymbol = "";

    [ObservableProperty]
    private string? _symbolError;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddClassCommand))]
    private string _newClassName = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddClassCommand))]
    private string _newClassCharacters = "";

    [ObservableProperty]
    private string? _classError;

    public EditorViewModel(DocumentSession session, IDialogService dialogs)
    {
        _session = session;
        _dialogs = dialogs;
        session.DocumentReplaced += (_, _) => Refresh();
        session.DocumentEdited += (_, e) =>
        {
            if (e.Kind is DocumentChangeKind.Structure or DocumentChangeKind.Reset)
            {
                Refresh();
            }
        };
        Refresh();
    }

    [RelayCommand(CanExecute = nameof(CanAddSymbol))]
    private void AddSymbol()
    {
        SymbolError = _session.Document.AddSymbol(NewSymbol).Error;
        if (SymbolError is null)
        {
            NewSymbol = "";
        }
    }

    [RelayCommand]
    private void RemoveSymbol(string symbol) => SymbolError = _session.Document.RemoveSymbol(symbol).Error;

    // "A-Z" ⇒ SymbolClass.FromRanges: every letter from A to Z.
    [RelayCommand(CanExecute = nameof(CanAddClass))]
    private void AddClass()
    {
        ClassError = _session.Document.AddSymbolClass(SymbolClass.FromRanges(NewClassName.Trim(), NewClassCharacters)).Error;
        if (ClassError is null)
        {
            NewClassName = "";
            NewClassCharacters = "";
        }
    }

    [RelayCommand]
    private void RemoveClass(SymbolClass symbolClass) =>
        ClassError = _session.Document.RemoveSymbol(symbolClass.Name).Error;

    private bool CanAddSymbol() => NewSymbol.Length > 0;

    private bool CanAddClass() => NewClassName.Trim().Length > 0 && NewClassCharacters.Length > 0;

    private void Refresh()
    {
        var dfa = _session.Document.Dfa;
        Classes = [.. dfa.SymbolClasses];
        Symbols = [.. dfa.Alphabet.Where(symbol => dfa.SymbolClasses.All(c => c.Name != symbol))];
    }
}

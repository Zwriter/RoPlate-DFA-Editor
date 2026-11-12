namespace Dfa.App.RealApps.RoPlate;

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Dfa.Core.Simulation;

// The RoPlate screen (R-02, app A1). The messages come from the state descriptions (data), not from code.
public sealed partial class RoPlateViewModel : ObservableObject
{
    private readonly INavigationService _navigation;
    private readonly DfaDocument _document = ExampleCatalog.Load("license-plate");
    private DfaRun _run;
    private string _text = "";

    public RoPlateViewModel(INavigationService navigation)
    {
        _navigation = navigation;
        _run = new DfaRun(_document.Dfa, "");
    }

    // Lowercase letters automatically become uppercase.
    public string Text
    {
        get => _text;
        set
        {
            var text = value.ToUpperInvariant();
            if (text.Length == _text.Length + 1 && text.StartsWith(_text, StringComparison.Ordinal))
            {
                _run.Append(text[^1]);
            }
            else if (text.Length == _text.Length - 1 && _text.StartsWith(text, StringComparison.Ordinal))
            {
                _run.RemoveLast();
            }
            else if (text != _text)
            {
                // Pasting or editing in the middle.
                _run = new DfaRun(_document.Dfa, text);
                _run.RunToEnd();
            }

            _text = text;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsBlocked));
            OnPropertyChanged(nameof(IsValid));
            OnPropertyChanged(nameof(Message));
        }
    }

    // Red: the automaton is blocked.
    public bool IsBlocked => _run.IsBlocked;

    // Green: the plate is accepted.
    public bool IsValid => _run.Verdict == Verdict.Accepted;

    public string Message
    {
        get
        {
            var description = _document.Descriptions[_run.CurrentState!];
            if (IsBlocked)
            {
                return "Invalid format: " + LowerFirst(description);
            }

            return IsValid || _text.Length == 0 ? description : "Incomplete format: " + LowerFirst(description);
        }
    }

    [RelayCommand]
    private void ShowAutomaton() => _navigation.OpenInSimulator(_document, Text);

    private static string LowerFirst(string text) => char.ToLowerInvariant(text[0]) + text[1..];
}

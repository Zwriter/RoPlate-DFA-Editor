namespace Dfa.App.Views.Simulator;

using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using Dfa.App.Controls;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Simulation;
using Dfa.Core.Validation;

public sealed partial class SimulatorViewModel : ObservableObject
{
    public const string ModifiedMessage = "The automaton was modified: the simulation was reset.";

    private readonly DocumentSession _session;
    private readonly IDialogService _dialogs;
    private DispatcherTimer? _timer;
    private readonly GuessGame _guess = new();

    [ObservableProperty]
    private string _input = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRunning))]
    private DfaRun? _run;

    [ObservableProperty]
    private string? _message;

    // Play / Pause (S-05): the time between two steps, in milliseconds.
    [ObservableProperty]
    private double _interval = 1000;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PlayPauseCommand))]
    private bool _isPlaying;

    // "Guess" mode (S-08): the user clicks the state the automaton goes to before each step.
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StepForwardCommand), nameof(RunToEndCommand), nameof(PlayPauseCommand))]
    private bool _isGuessMode;

    [ObservableProperty]
    private string? _guessMessage;

    public SimulatorViewModel(DocumentSession session, IDialogService dialogs)
    {
        _session = session;
        _dialogs = dialogs;

        // SI-07: Structure and Reset (undo/redo) stop the simulation with the message; Layout does not.
        session.DocumentEdited += (_, e) =>
        {
            if (e.Kind is DocumentChangeKind.Structure or DocumentChangeKind.Reset)
            {
                Stop();
                Message = ModifiedMessage;
            }
        };

        // A replaced automaton (New / Open / Examples) also resets, but it was not "modified".
        session.DocumentReplaced += (_, _) =>
        {
            Stop();
            Message = null;
        };
    }

    public bool IsRunning => Run is not null;

    public bool IsAtStart => Run?.IsAtStart ?? true;

    public bool IsAtEnd => Run?.IsAtEnd ?? true;

    // What the canvas shows (contract 3, section 6).
    public string? CurrentState => Run?.CurrentState;

    public HighlightKind HighlightKind => Run switch
    {
        null or { CurrentState: null } => HighlightKind.None,
        { Verdict: Verdict.Accepted } => HighlightKind.Accepted,
        { Verdict: Verdict.Rejected } => HighlightKind.Rejected,
        _ => HighlightKind.Current,
    };

    public EdgeRef? LastEdge => Run?.LastStep is { To: { } to } step ? new EdgeRef(step.From, to) : null;

    // In "Guess" mode, PickState before each step; Edit otherwise.
    public CanvasMode RequestedCanvasMode => IsGuessMode && !IsAtEnd ? CanvasMode.PickState : CanvasMode.Edit;

    // Shown at the end of a guessed run.
    public string? Score => IsGuessMode && IsRunning && IsAtEnd ? _guess.Score : null;

    // What the input tape shows (S-02, contract 3, section 6).
    public string TapeText => Run?.Input ?? "";

    public int CursorPosition => Run?.LastStep is { } step ? step.Position + 1 : 0;

    public int? ErrorPosition => Run is { IsBlocked: true, LastStep: { } step } ? step.Position : null;

    // The execution table (S-03); the Class column only for automata with symbol classes.
    public IReadOnlyList<ExecutionRow> ExecutionRows => Run is null ? [] : ExecutionRow.For(Run);

    public bool ShowClassColumn => _session.Document.Dfa.SymbolClasses.Count > 0;

    // The verdict (S-04): DfaRun.Explanation, with ✓ when accepted and ✗ when rejected.
    public string Explanation => Run?.Explanation ?? "";

    public bool IsAccepted => Run?.Verdict == Verdict.Accepted;

    public bool IsRejected => Run?.Verdict == Verdict.Rejected;

    // "Guess" mode: the answer is confirmed or corrected, then the step is taken. Otherwise a click does nothing.
    public void OnStateClicked(string state)
    {
        if (!IsGuessMode || Run is not { IsAtEnd: false } run)
        {
            return;
        }

        GuessMessage = _guess.Answer(state, run.Steps[run.Position].To!);
        run.StepForward();
        SkipBlockingStep();
    }

    public void Start(string input)
    {
        Input = input;
        Stop();

        var errors = DfaValidator.Validate(_session.Document.Dfa)
            .Where(i => i.Severity == Severity.Error)
            .Select(i => i.Message)
            .ToList();
        if (errors.Count > 0)
        {
            _ = _dialogs.ShowErrorsAsync("Start", errors);
            return;
        }

        Run = new DfaRun(_session.Document.Dfa, input);
        ClearGuesses();
    }

    public void Stop()
    {
        Pause();
        Run = null;
        Message = null;
    }

    [RelayCommand]
    private void StartSimulation() => Start(Input);

    [RelayCommand(CanExecute = nameof(CanStepForward))]
    private void StepForward() => Run?.StepForward();

    [RelayCommand(CanExecute = nameof(CanStepBack))]
    private void StepBack() => Run?.StepBack();

    [RelayCommand(CanExecute = nameof(CanStepBack))]
    private void Reset()
    {
        Run?.Reset();
        ClearGuesses();
    }

    [RelayCommand(CanExecute = nameof(CanStepForward))]
    private void RunToEnd() => Run?.RunToEnd();

    [RelayCommand(CanExecute = nameof(CanPlayPause))]
    private void PlayPause()
    {
        if (IsPlaying)
        {
            Pause();
            return;
        }

        if (_timer is null)
        {
            _timer = new DispatcherTimer();
            _timer.Tick += (_, _) => Run?.StepForward();
        }

        _timer.Interval = TimeSpan.FromMilliseconds(Interval);
        _timer.Start();
        IsPlaying = true;
    }

    private bool CanPlayPause() => IsPlaying || CanStepForward();

    private void Pause()
    {
        _timer?.Stop();
        IsPlaying = false;
    }

    // In "Guess" mode the only way forward is a guess.
    private bool CanStepForward() => !IsAtEnd && !IsGuessMode;

    partial void OnIsGuessModeChanged(bool value)
    {
        Pause();
        ClearGuesses();
        OnPropertyChanged(nameof(RequestedCanvasMode));
    }

    // A new score; a blocking step has no state to guess, so it is taken without a guess.
    private void ClearGuesses()
    {
        _guess.Clear();
        GuessMessage = null;
        SkipBlockingStep();
        OnPropertyChanged(nameof(Score));
    }

    private void SkipBlockingStep()
    {
        if (IsGuessMode && Run is { IsAtEnd: false } run && run.Steps[run.Position].To is null)
        {
            run.StepForward();
        }
    }

    private bool CanStepBack() => !IsAtStart;

    partial void OnIntervalChanged(double value)
    {
        if (_timer is not null)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(value);
        }
    }

    partial void OnRunChanged(DfaRun? oldValue, DfaRun? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Changed -= OnRunPositionChanged;
        }

        if (newValue is not null)
        {
            newValue.Changed += OnRunPositionChanged;
        }

        OnRunPositionChanged(this, EventArgs.Empty);
    }

    private void OnRunPositionChanged(object? sender, EventArgs e)
    {
        OnPropertyChanged(nameof(IsAtStart));
        OnPropertyChanged(nameof(IsAtEnd));
        OnPropertyChanged(nameof(CurrentState));
        OnPropertyChanged(nameof(HighlightKind));
        OnPropertyChanged(nameof(LastEdge));
        OnPropertyChanged(nameof(TapeText));
        OnPropertyChanged(nameof(CursorPosition));
        OnPropertyChanged(nameof(ErrorPosition));
        OnPropertyChanged(nameof(ExecutionRows));
        OnPropertyChanged(nameof(ShowClassColumn));
        OnPropertyChanged(nameof(Explanation));
        OnPropertyChanged(nameof(IsAccepted));
        OnPropertyChanged(nameof(IsRejected));
        OnPropertyChanged(nameof(RequestedCanvasMode));
        OnPropertyChanged(nameof(Score));
        StepForwardCommand.NotifyCanExecuteChanged();
        StepBackCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
        RunToEndCommand.NotifyCanExecuteChanged();
        PlayPauseCommand.NotifyCanExecuteChanged();

        // Playing stops by itself at the end.
        if (IsPlaying && IsAtEnd)
        {
            Pause();
        }
    }
}

namespace Dfa.Tests.App;

using Dfa.App.Controls;
using Dfa.App.Services;
using Dfa.App.Views.Simulator;
using Dfa.Core.Documents;
using Dfa.Core.Examples;
using Shouldly;

public class SimulatorViewModelTests
{
    private readonly FakeDialogs _dialogs = new();
    private readonly DocumentSession _session = new();
    private readonly SimulatorViewModel _sim;

    public SimulatorViewModelTests()
    {
        _session.Replace(ExampleCatalog.Load("even-a"), null);
        _sim = new SimulatorViewModel(_session, _dialogs);
    }

    [Fact]
    public void BeforeStart_NothingIsHighlighted_AndStepsAreDisabled()
    {
        _sim.IsRunning.ShouldBeFalse();
        _sim.CurrentState.ShouldBeNull();
        _sim.HighlightKind.ShouldBe(HighlightKind.None);
        _sim.StepForwardCommand.CanExecute(null).ShouldBeFalse();
        _sim.RunToEndCommand.CanExecute(null).ShouldBeFalse();
    }

    [Fact]
    public void Start_ShowsTheStartState_AndEnablesForwardOnly()
    {
        _sim.Input = "ab";
        _sim.StartSimulationCommand.Execute(null);

        _sim.IsRunning.ShouldBeTrue();
        _sim.CurrentState.ShouldBe("q0");
        _sim.HighlightKind.ShouldBe(HighlightKind.Current);
        _sim.LastEdge.ShouldBeNull();
        _sim.StepForwardCommand.CanExecute(null).ShouldBeTrue();
        _sim.StepBackCommand.CanExecute(null).ShouldBeFalse();
        _sim.ResetCommand.CanExecute(null).ShouldBeFalse();
    }

    [Fact]
    public void StepForward_MovesAndHighlightsTheEdge()
    {
        _sim.Start("ab");

        _sim.StepForwardCommand.Execute(null);

        _sim.CurrentState.ShouldBe("q1");
        _sim.LastEdge.ShouldBe(new EdgeRef("q0", "q1"));
        _sim.StepBackCommand.CanExecute(null).ShouldBeTrue();
    }

    [Fact]
    public void RunToEnd_ShowsTheVerdict_AndDisablesForward()
    {
        _sim.Start("ab");

        _sim.RunToEndCommand.Execute(null);

        _sim.IsAtEnd.ShouldBeTrue();
        _sim.HighlightKind.ShouldBe(HighlightKind.Rejected);
        _sim.StepForwardCommand.CanExecute(null).ShouldBeFalse();
    }

    [Fact]
    public void EmptyInput_IsTheEmptyWord()
    {
        _sim.Start("");

        _sim.IsAtStart.ShouldBeTrue();
        _sim.IsAtEnd.ShouldBeTrue();
        _sim.HighlightKind.ShouldBe(HighlightKind.Accepted);
    }

    [Fact]
    public void StepBackAndReset_ReturnToEarlierConfigurations()
    {
        _sim.Start("aa");
        _sim.RunToEndCommand.Execute(null);

        _sim.StepBackCommand.Execute(null);
        _sim.CurrentState.ShouldBe("q1");

        _sim.ResetCommand.Execute(null);
        _sim.IsAtStart.ShouldBeTrue();
        _sim.CurrentState.ShouldBe("q0");
    }

    [Fact]
    public void Start_WithValidationErrors_ShowsThemAndDoesNotStart()
    {
        _session.Document.SetStart(null);

        _sim.Start("ab");

        _sim.IsRunning.ShouldBeFalse();
        _dialogs.Errors.ShouldBe(["The automaton has no start state."]);
    }

    [Fact]
    public void StructuralEdit_ResetsTheSimulation_WithTheMessage()
    {
        _sim.Start("ab");

        _session.Document.SetFinal("q1", true);

        _sim.IsRunning.ShouldBeFalse();
        _sim.Message.ShouldBe("The automaton was modified: the simulation was reset.");
    }

    [Fact]
    public void MovingAState_DoesNotResetTheSimulation()
    {
        _sim.Start("ab");

        _session.Document.MoveState("q0", new Point2(10, 10));

        _sim.IsRunning.ShouldBeTrue();
        _sim.Message.ShouldBeNull();
    }

    [Fact]
    public void ReplacingTheDocument_ResetsWithoutTheMessage()
    {
        _sim.Start("ab");

        _session.Replace(ExampleCatalog.Load("div3"), null);

        _sim.IsRunning.ShouldBeFalse();
        _sim.Message.ShouldBeNull();
    }

    [Fact]
    public void Stop_ClearsTheHighlight()
    {
        _sim.Start("ab");

        _sim.Stop();

        _sim.CurrentState.ShouldBeNull();
        _sim.HighlightKind.ShouldBe(HighlightKind.None);
    }

    [Fact]
    public void Tape_CursorStandsBelowTheNextSymbol()
    {
        _sim.Start("ab");
        _sim.TapeText.ShouldBe("ab");
        _sim.CursorPosition.ShouldBe(0);

        _sim.StepForwardCommand.Execute(null);

        _sim.CursorPosition.ShouldBe(1);
        _sim.ErrorPosition.ShouldBeNull();
    }

    [Fact]
    public void Tape_MarksTheBlockingCell()
    {
        _sim.Start("acb");

        _sim.RunToEndCommand.Execute(null);

        _sim.ErrorPosition.ShouldBe(1);
        _sim.CursorPosition.ShouldBe(2);
    }

    [Fact]
    public void Verdict_InProgress_HasNoSymbol()
    {
        _sim.Start("ab");
        _sim.StepForwardCommand.Execute(null);

        _sim.Explanation.ShouldBe("Step 1 of 2.");
        _sim.IsAccepted.ShouldBeFalse();
        _sim.IsRejected.ShouldBeFalse();
    }

    [Fact]
    public void Verdict_Accepted_ShowsTheCheckMark()
    {
        _sim.Start("aa");
        _sim.RunToEndCommand.Execute(null);

        _sim.Explanation.ShouldBe("ACCEPTED: the word ended in the final state q0.");
        _sim.IsAccepted.ShouldBeTrue();
        _sim.IsRejected.ShouldBeFalse();
    }

    [Fact]
    public void Verdict_Rejected_ShowsTheCross()
    {
        _sim.Start("ab");
        _sim.RunToEndCommand.Execute(null);

        _sim.Explanation.ShouldBe("REJECTED: the word ended in q1, which is not final.");
        _sim.IsRejected.ShouldBeTrue();
        _sim.IsAccepted.ShouldBeFalse();
    }
}

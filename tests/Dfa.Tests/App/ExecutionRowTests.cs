namespace Dfa.Tests.App;

using Dfa.App.Views.Simulator;
using Dfa.Core.Examples;
using Dfa.Core.Simulation;
using Shouldly;

public class ExecutionRowTests
{
    [Fact]
    public void AtStart_OnlyRowZero_TheInitialConfiguration()
    {
        var run = new DfaRun(ExampleCatalog.Load("even-a").Dfa, "ab");

        ExecutionRow.For(run).ShouldBe([new ExecutionRow(0, "", "", "ab", "q0", true)]);
    }

    [Fact]
    public void FutureRowsAreHidden_AndTheLastShownRowIsCurrent()
    {
        var run = new DfaRun(ExampleCatalog.Load("even-a").Dfa, "ab");
        run.StepForward();

        ExecutionRow.For(run).ShouldBe(
        [
            new ExecutionRow(0, "", "", "ab", "q0", false),
            new ExecutionRow(1, "a", "a", "b", "q1", true),
        ]);
    }

    [Fact]
    public void NothingLeft_IsEpsilon_AndASpaceIsShownAsOpenBox()
    {
        var run = new DfaRun(ExampleCatalog.Load("license-plate").Dfa, "B ");
        run.RunToEnd();

        var rows = ExecutionRow.For(run);

        rows[1].ShouldBe(new ExecutionRow(1, "B", "L", "␣", "q1", false));
        rows[2].ShouldBe(new ExecutionRow(2, "␣", "_", "ε", "B0", true));
    }

    [Fact]
    public void BlockingRow_KeepsTheStateItBlockedIn()
    {
        var run = new DfaRun(ExampleCatalog.Load("license-plate").Dfa, "B1");
        run.RunToEnd();

        ExecutionRow.For(run)[^1].ShouldBe(new ExecutionRow(2, "1", "C", "ε", "q1", true));
    }
}

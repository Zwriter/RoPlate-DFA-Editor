namespace Dfa.Tests.App;

using Dfa.App.Services;
using Dfa.App.Views.TransitionTable;
using Dfa.Core.Documents;
using Dfa.Core.Model;
using Shouldly;

public class TransitionTableViewModelTests
{
    private readonly DocumentSession _session = new();
    private readonly TransitionTableViewModel _table;

    public TransitionTableViewModelTests()
    {
        _table = new TransitionTableViewModel(_session);
        var doc = _session.Document;
        doc.AddState(new Point2(0, 0));
        doc.AddState(new Point2(100, 0));
        doc.AddSymbol("a");
        doc.AddSymbol("b");
        doc.AddTransitions("q0", ["a"], "q1");
        doc.SetStart("q0");
        doc.SetFinal("q1", true);
    }

    [Fact]
    public void Rows_AreStatesWithMarks_ColumnsAreSigma_MissingCellsAreNull()
    {
        _table.Symbols.ShouldBe(["a", "b"]);
        _table.Rows.Select(r => r.Label).ShouldBe(["→q0", "*q1"]);
        Targets(0).ShouldBe(["q1", null]);
        Targets(1).ShouldBe([null, null]);
    }

    [Fact]
    public void StartAndFinal_OnTheSameState_ShowBothMarks()
    {
        _session.Document.SetFinal("q0", true);

        _table.Rows[0].Label.ShouldBe("→*q0");
    }

    [Fact]
    public void Edit_AndUndo_UpdateTheTable()
    {
        _session.Document.AddTransitions("q1", ["a", "b"], "q1");
        Targets(1).ShouldBe(["q1", "q1"]);

        _session.History.Undo();

        Targets(1).ShouldBe([null, null]);
    }

    [Fact]
    public void Replace_RebuildsTheTable()
    {
        _session.Replace(DfaDocument.CreateEmpty(), null);

        _table.Symbols.ShouldBeEmpty();
        _table.Rows.ShouldBeEmpty();
    }

    [Fact]
    public void Move_DoesNotRebuildTheTable()
    {
        var rows = _table.Rows;

        _session.Document.MoveState("q0", new Point2(5, 5));

        _table.Rows.ShouldBeSameAs(rows);
    }

    [Fact]
    public void Cell_Choices_AreMissingThenEveryState_WithTheCurrentOneChecked()
    {
        var cell = _table.Rows[0].Cells[0];

        cell.Name.ShouldBe("δ(q0, a)");
        cell.Choices.Select(c => c.Label).ShouldBe(["—", "q0", "q1"]);
        cell.Choices.Single(c => c.IsCurrent).Label.ShouldBe("q1");
    }

    [Fact]
    public void PickingAChoice_SetsTheCell_AsOneUndoStep()
    {
        _table.Rows[0].Cells[0].Choices.Single(c => c.Label == "q0").Pick.Execute(null);

        Targets(0).ShouldBe(["q0", null]);
        _session.History.Undo();
        Targets(0).ShouldBe(["q1", null]);
    }

    [Fact]
    public void PickingMissing_RemovesTheTransition()
    {
        _table.Rows[0].Cells[0].Choices[0].Pick.Execute(null);

        Targets(0).ShouldBe([null, null]);
        _table.Error.ShouldBeNull();
    }

    [Fact]
    public void PickingAStateThatWasRemoved_ShowsTheError()
    {
        var stale = _table.Rows[0].Cells[1].Choices.Single(c => c.Label == "q1");
        _session.Document.RemoveState("q1");

        stale.Pick.Execute(null);

        _table.Error.ShouldBe("State 'q1' does not exist.");
    }

    private IEnumerable<string?> Targets(int row) => _table.Rows[row].Cells.Select(c => c.Target);
}

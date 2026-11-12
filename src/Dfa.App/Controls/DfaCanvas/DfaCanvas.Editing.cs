namespace Dfa.App.Controls;

using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Dfa.App.Services;
using Dfa.Core.Documents;
using Dfa.Core.Model;

// Mouse and keyboard editing (E-03, ED-01…ED-04). Every change goes through Document; errors go to StatusMessage.
public sealed partial class DfaCanvas
{
    private (string State, Point Position, Point Offset, bool Moved)? _drag;
    private (string From, Point To)? _link;
    private EdgeRef? _selectedEdge;

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (Document is not { } document)
        {
            return;
        }

        var p = e.GetPosition(this);
        var state = StateAt(document, p);
        if (state is not null)
        {
            OnStateClicked(state);
        }

        if (Mode != CanvasMode.Edit)
        {
            return;
        }

        Focus();
        var point = e.GetCurrentPoint(this).Properties;
        if (state is not null)
        {
            Select(state, null);
            if (point.IsRightButtonPressed)
            {
                StateMenu(document, state).Open(this);
            }
            else if (e.KeyModifiers.HasFlag(KeyModifiers.Shift))
            {
                _link = (state, p);
            }
            else
            {
                _drag = (state, Center(document, state), Center(document, state) - p, false);
            }

            e.Pointer.Capture(this);
        }
        else if (EdgeAt(document, p) is { } edge)
        {
            Select(null, edge);
            if (e.ClickCount == 2)
            {
                _ = EditSymbolsAsync(document, edge);
            }
        }
        else if (point.IsLeftButtonPressed)
        {
            // A click on an empty area clears the selection; a double-click there adds a state.
            Select(null, null);
            if (e.ClickCount == 2)
            {
                Report(document.AddState(new Point2(p.X, p.Y)));
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);
        var p = e.GetPosition(this);
        if (_drag is { } drag)
        {
            _drag = drag with { Position = p + drag.Offset, Moved = true };
            InvalidateVisual();
        }
        else if (_link is { } link)
        {
            _link = link with { To = p };
            InvalidateVisual();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        e.Pointer.Capture(null);
        var (drag, link) = (_drag, _link);
        (_drag, _link) = (null, null);
        InvalidateVisual();
        if (Document is not { } document)
        {
            return;
        }

        // A single MoveState when the mouse is released: one undo step.
        if (drag is { Moved: true } d)
        {
            Report(document.MoveState(d.State, new Point2(d.Position.X, d.Position.Y)));
        }

        if (link is { } l && StateAt(document, e.GetPosition(this)) is { } to)
        {
            _ = AddTransitionsAsync(document, l.From, to);
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (Mode != CanvasMode.Edit || e.Key != Key.Delete || Document is not { } document)
        {
            return;
        }

        if (SelectedState is { } state)
        {
            Report(document.RemoveState(state));
        }
        else if (_selectedEdge is { } edge)
        {
            Report(document.RemoveEdge(edge.From, edge.To));
        }

        Select(null, null);
        e.Handled = true;
    }

    private ContextMenu StateMenu(DfaDocument document, string state)
    {
        var dfa = document.Dfa;
        var start = new MenuItem { Header = "Start state", ToggleType = MenuItemToggleType.CheckBox, IsChecked = dfa.StartState == state };
        start.Click += (_, _) => Report(document.SetStart(dfa.StartState == state ? null : state));
        var final = new MenuItem { Header = "Final state", ToggleType = MenuItemToggleType.CheckBox, IsChecked = dfa.IsFinal(state) };
        final.Click += (_, _) => Report(document.SetFinal(state, !dfa.IsFinal(state)));
        var rename = new MenuItem { Header = "Rename" };
        rename.Click += async (_, _) => await RenameAsync(document, state);
        var delete = new MenuItem { Header = "Delete" };
        delete.Click += (_, _) => Report(document.RemoveState(state));
        return new ContextMenu { ItemsSource = new[] { start, final, rename, DescriptionMenuItem(document, state), delete } };
    }

    private async Task RenameAsync(DfaDocument document, string state)
    {
        if (Dialogs() is { } dialogs && await dialogs.AskTextAsync("Rename", "New name", state) is { } name)
        {
            Report(document.RenameState(state, name.Trim()));
        }
    }

    private async Task AddTransitionsAsync(DfaDocument document, string from, string to)
    {
        if (Dialogs() is { } dialogs && await dialogs.AskTextAsync("Symbols", "e.g. a,b") is { } text)
        {
            Report(document.AddTransitions(from, SymbolList.Parse(text), to));
        }
    }

    // Double-click on an arrow: its symbols are replaced; if the new ones are refused, the old ones are put back.
    private async Task EditSymbolsAsync(DfaDocument document, EdgeRef edge)
    {
        var old = document.Dfa.Edges.First(e => e.From == edge.From && e.To == edge.To).Symbols.ToList();
        if (Dialogs() is not { } dialogs
            || await dialogs.AskTextAsync("Symbols", "e.g. a,b", SymbolList.Format(old)) is not { } text
            || SymbolList.Parse(text) is not { Count: > 0 } symbols)
        {
            return;
        }

        document.RemoveEdge(edge.From, edge.To);
        var result = document.AddTransitions(edge.From, symbols, edge.To);
        if (!result.Ok)
        {
            document.AddTransitions(edge.From, old, edge.To);
        }

        Report(result);
    }

    private string? StateAt(DfaDocument document, Point p) =>
        document.Dfa.States.LastOrDefault(s => CanvasGeometry.HitsState(Center(document, s), p));

    private EdgeRef? EdgeAt(DfaDocument document, Point p)
    {
        var edges = document.Dfa.Edges;
        var edge = edges.FirstOrDefault(e => CanvasGeometry.HitsEdge(ShapeOf(document, e, edges), p));
        return edge is null ? null : new EdgeRef(edge.From, edge.To);
    }

    private void Select(string? state, EdgeRef? edge)
    {
        SelectedState = state;
        _selectedEdge = edge;
        InvalidateVisual();
    }

    private void Report(OpResult result)
    {
        if (!result.Ok)
        {
            OnStatusMessage(result.Error!);
        }
    }

    // The window that holds the canvas owns the dialogs (Rename, Symbols).
    private DialogService? Dialogs() => TopLevel.GetTopLevel(this) is Window window ? new DialogService(window) : null;
}

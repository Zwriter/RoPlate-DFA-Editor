# The `Dfa.App` application

A short guide for whoever adds a panel to the app (P1, P3) or changes the window and the drawing (P2). The exact signatures are in [contract 3](plan/contracts/03-app-ui.md); this page explains **how** the pieces fit. For the engine, see the [engine guide](core.md).

## 1. Layout

| File / folder | Contents |
|---|---|
| `MainWindow.axaml` | the window: menu, drawing, tabs, status bar |
| `MainViewModel.cs` | builds every object by hand and holds the File / Examples / Apps commands |
| `Services/` | `DocumentSession`, `NavigationService`, `DialogService` and their interfaces |
| `Themes/Styles.axaml` | the shared brushes from contract 3 (`Brush.StateCurrent`, `Brush.TapeRead`, …) |
| `Controls/DfaCanvas/` | the drawing: `DfaCanvas` (properties, events), `DfaCanvas.Drawing.cs` (painting), `DfaCanvas.Editing.cs` (mouse and keyboard editing), `DfaCanvas.Highlight.cs` (simulation highlighting), `DfaCanvas.Descriptions.cs` (state descriptions), `DfaCanvasImage` (PNG export), `DfaCanvasSvg` (SVG export), `CanvasGeometry` (arrow math and hit-testing) |
| `Controls/InputTape/` | the input tape of the simulation (P3) |
| `Views/Editor/` | the alphabet and symbol classes: `EditorPanel` + `EditorViewModel` |
| `Views/TransitionTable/` | the δ table panel (P1) |
| `Views/Simulator/` | the simulation panel (`SimulatorPanel`, `SimulatorViewModel`, the execution table) and the batch tests (`BatchTestPanel`, `BatchTestViewModel`) (P3) |
| `RealApps/RoPlate/` | the RoPlate window: `RoPlateWindow` + `RoPlateViewModel` (P3) |

Controls keep the namespace `Dfa.App.Controls` even inside their own folder (a namespace named after the folder would clash with the class name).

## 2. How it is assembled

`MainWindow` creates `new MainViewModel(new DialogService(this))` and uses it as its `DataContext`. `MainViewModel` then creates everything else:

```csharp
Session    = new DocumentSession();
Navigation = new NavigationService(this, Session);
Table      = new TransitionTableViewModel(Session);
```

There is no DI container. `MainViewModel` takes an `IDialogService`, so tests pass a fake one; see `tests/Dfa.Tests/App/MainViewModelTests.cs`.

### Adding your panel

1. Give its ViewModel the constructor from contract 3, for example `SimulatorViewModel(DocumentSession, IDialogService)`.
2. Create it in the `MainViewModel` constructor and expose it as a property (`Simulator`).
3. In `MainWindow.axaml`, replace the tab's `<TextBlock Classes="wip" />` with your view, bound with `DataContext="{Binding Simulator}"`.

The tab order (Edit, Simulate, Tests) matches the `MainTab` enum: `NavigationService.ShowTab` sets `SelectedTabIndex = (int)tab`.

## 3. The document session

`DocumentSession.Document` is **one live `DfaDocument` for the whole session**. Read the automaton from it and change it only through its methods.

| You want to know when… | Listen to |
|---|---|
| the user edited the automaton (including undo/redo) | `DocumentEdited` (check `e.Kind`) |
| the automaton was replaced (New, Open, Examples) | `DocumentReplaced` |
| the file name or the dirty flag changed | `PropertyChanged` (`FilePath`, `IsDirty`) |

`Replace` copies into the live document; it is not an edit, so it does not raise `DocumentEdited` and clears `IsDirty`. See the rules in contract 3, section 3.

## 4. The File menu

| Command | Behaviour |
|---|---|
| New, Open, an example, closing the window | if `IsDirty`, asks `Save changes?` first. **Yes** saves, and the command stops if that save is cancelled or fails; **No** discards |
| Open | `DfaJson.LoadFile`; on failure, every error is shown with `ShowErrorsAsync` and the current document stays |
| Save | saves to `FilePath`, or behaves like Save as when there is none |
| Save as | asks for a path, then `DfaJson.SaveFile` (atomic); on success sets `FilePath` and clears `IsDirty` |
| Export as PNG / SVG | one command, `ExportImageCommand` with `"png"` or `"svg"`; `PickSaveImageAsync` offers the filter that matches the suggested name. PNG draws the canvas with `RenderTargetBitmap`; SVG writes the same `CanvasGeometry` shapes as XML. Both are black on white, sized by `DfaCanvas.DrawingSize` |

The window title is `{name} — DFA Simulator`, with `*` after the name while `IsDirty` is true.

## 5. The drawing: `DfaCanvas`

Bind `Document` and the canvas paints it; it repaints on every `Document.Changed` and grows to fit the states, so it sits inside a `ScrollViewer`.

| Element | How it is drawn |
|---|---|
| state | circle, radius 28, name in the middle |
| final state | a second circle, radius 22 |
| start state | a 40 px arrow from the left, labelled `start` |
| arrow | one per `Dfa.Edges` entry, labelled `a, b`; it starts and ends on the circles |
| both directions | two arcs, each bending to its own left |
| loop | an arc above the state |
| arrow through another state | bent wide around it |

All geometry is in `CanvasGeometry` (pure math, tested in `CanvasGeometryTests`); `DfaCanvas.Drawing.cs` only paints. Colours come from the theme's `Foreground`, so the drawing works in light and dark mode.

## 6. Implementation status

| Task | What | Status |
|---|---|---|
| E-00 | UI stubs: session, navigation, dialogs, empty canvas | done |
| E-01 | main window, File / Examples / Apps menus, title | done |
| E-02 | drawing the automaton | done |
| E-03 | editing on the drawing | done |
| E-04 | alphabet and symbol classes | done |
| E-05 | highlighting for the simulation | done |
| E-06 | undo/redo and the trap state in the UI | done |
| E-07 | state descriptions | done |
| E-08 | image export (bonus) | done (PNG and SVG) |

Closing the window is the one piece of logic in `MainWindow`'s code-behind: `OnClosing` cancels, awaits `MainViewModel.ConfirmDiscardAsync`, and closes again if allowed (`Closing` cannot await and has no command to bind).

# Quick start

Get from a fresh clone to a drawn automaton in about five minutes. For building the engine into your own code, see the [engine guide](core.md); for working on the app itself, see the [app guide](app.md).

## 1. Run the app

You need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Zwriter/RoPlate.git
cd RoPlate
dotnet run --project src/Dfa.App
```

The window opens with an empty automaton called **New automaton**.

## 2. Find your way around

```
┌──────────────────────────────────────────────────────────────┐
│ File  Examples  Apps                                         │  menu
├─────────────────────────────────────┬────────────────────────┤
│                                     │ Edit | Simulate | Tests│
│   the drawing of the automaton      │                        │
│   (scrolls when it is larger        │ δ table                │
│    than the window)                 │                        │
├─────────────────────────────────────┴────────────────────────┤
│ status bar                                                   │
└──────────────────────────────────────────────────────────────┘
```

- **The drawing** (left): every state is a circle. A **double circle** is a final state, and the short arrow labelled **start** points at the start state. One arrow joins two states and lists all its symbols (`a, b`). When the two states have arrows both ways, the arrows are drawn as two curves. A loop over a state is a transition back to the same state.
- **The tabs** (right): **Edit** holds the alphabet, the symbol classes and the δ table (`→` start, `*` final, `—` missing transition); **Simulate** runs a word step by step; **Tests** runs a list of words at once.
- **The status bar** (bottom): what just happened, for example `Saved C:\automata\even-a.dfa.json`.
- **The title bar**: the automaton's name, with a `*` while you have unsaved changes.

## 3. Open an example

Choose **Examples ▸ Even number of a**. The automaton appears on the drawing and in the δ table. Try the others:

| Example | Accepts |
|---|---|
| Even number of a | words with an even number of `a` |
| Starts with a | words that start with `a` |
| Ends in ab | words that end in `ab` |
| Contains aba | words that contain `aba` |
| Binary numbers divisible by 3 | `0`, `11`, `110`, … |
| Length a multiple of 3 | words of length 0, 3, 6, … |
| License plate (RoPlate) | Romanian license plates such as `B 123 ABC` and `CJ 12 ABC` |

If you have unsaved changes, the app first asks **Save changes?** Choose **Yes** to save them or **No** to discard them.

## 4. Save and open files

| Command | Shortcut | What it does |
|---|---|---|
| **File ▸ New** | Ctrl+N | starts an empty automaton |
| **File ▸ Open…** | Ctrl+O | opens a `.dfa.json` file |
| **File ▸ Save** | Ctrl+S | saves to the current file (asks for a name the first time) |
| **File ▸ Save as…** | Ctrl+Shift+S | saves under a new name |
| **File ▸ Export as PNG…** | | saves the drawing as an image |

Opening a broken file never crashes the app: you get a list of **every** problem, with the field that caused it, and the automaton you had stays open. The file format is plain JSON you can edit by hand; see [contract 2](plan/contracts/02-json-format.md).

## 5. Edit, simulate and test

| To… | Do this |
|---|---|
| add a symbol or a class | in **Edit**, type it under **Alphabet (Σ)** or **Symbol classes** (ranges such as `A-Z` work) and press **Add** |
| add a state | double-click an empty area of the drawing |
| move a state | drag it |
| add a transition | Shift + drag from one state to another (or to itself), then type its symbols, e.g. `a,b` |
| change an arrow's symbols | double-click the arrow |
| make a state start or final, rename or delete it | right-click it |
| delete a state or an arrow | click it, then press Delete |
| describe a state | right-click it, **Description…** (the text appears under the state) |
| undo or redo | **↶** / **↷**, Ctrl+Z / Ctrl+Y or the **Edit** menu |
| add the missing transitions | **Complete with trap state** |
| run a word | in **Simulate**, type it (empty = ε), press **Start**, then **Step forward**, **Run to end** or **Play** |
| guess each step | in **Simulate**, tick **Guess**, then click on the drawing the state the automaton goes to |
| test many words | in **Tests**, add words with **Add**, tick the ones that should be accepted, press **Run tests**; double-click a row to simulate it |
| check a license plate | **Apps ▸ RoPlate**, then type a plate; **Show automaton** opens it in the simulator |

Not included yet: SVG export, and closing the window does not ask to save; see the [team plan](plan/README.md).

## Troubleshooting

| Problem | Fix |
|---|---|
| `dotnet` is not found | install the .NET 10 SDK and open a new terminal |
| a state is not visible | scroll the drawing: it grows to fit every state |
| a file will not open | read the list of problems the app shows; each one names the field to fix |

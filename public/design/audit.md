# UI Audit — RoPlate (DFA Simulator)

> Status: the current UI is a functional test bench / mockup. This audit records what exists so the
> redesign can keep the parts that matter (mostly the contract-mandated conventions) and replace the rest.

## Stack

| Aspect | Finding |
|---|---|
| Framework | Avalonia 12.1.4 on .NET 10, desktop (`WinExe`), MVVM via CommunityToolkit.Mvvm |
| Component library | `Avalonia.Themes.Fluent` (`<FluentTheme />` in `src/Dfa.App/App.axaml`), stock controls with almost no overrides |
| Font | `Avalonia.Fonts.Inter` is registered (`.WithInterFont()` in `Program.cs`), but no `FontFamily` points at it; the canvas uses `Typeface.Default` |
| Theme file | `src/Dfa.App/Themes/Styles.axaml`: 7 `SolidColorBrush` keys in `ThemeDictionaries` (Light / Dark) |
| Dark mode | `RequestedThemeVariant="Default"` (follows the OS). Custom brushes have both variants. Code-drawn controls read brushes through `Controls/ThemeBrush.cs` |
| Accent | Not set; Fluent takes the **Windows system accent** (purple on this machine) |
| Constraints | Contract 3 §5 fixes the brush keys and the drawing conventions (radius 28 / 22, 4 px current outline, ✓/✗, "not color alone"). Constitution III: accessible, keyboard support, English text, checked at supported window sizes (min 800×500) |

## Current tokens

### Colors (10 hex values, all in `Themes/Styles.axaml`, + 4 named `Brushes.*` in code)

| Role | Light | Dark | Where |
|---|---|---|---|
| Window background | Fluent default (white) | Fluent default (**#000000**, measured) | — |
| Text | Fluent default (black) | Fluent default (white) | — |
| Muted text (descriptions) | `Brushes.Gray` #808080 | **same, not themed** | `DfaCanvas.Drawing.cs:61` |
| Tape read symbols | `Brush.TapeRead` #8A8A8A | #808080 | `InputTape.cs` |
| Borders | `SystemControlForegroundBaseLowBrush` | same (Fluent) | status bar, δ cells |
| Current state / last edge / tape cursor | `Brush.StateCurrent` = `EdgeLast` = `TapeCursor` #0063B1 | #60CDFF | 3 keys, same value |
| Accepted | `Brush.Accepted` #107C10 | #6CCB5F | |
| Rejected | `Brush.Rejected` #C42B1C | #FF99A4 | |
| Missing δ cell | `Brush.MissingCell` #FDE7E9 | #442726 | |
| Selection / current row / tabs / slider | `SystemControlHighlightListAccentLowBrush` + Fluent accent | OS accent (purple) | `ExecutionTable.axaml:19` |
| Export PNG | `Brushes.Black` on `Brushes.White` (intentional, theme-independent) | | `DfaCanvasImage.cs` |

Near-duplicates: 3 brush keys share one value (StateCurrent / EdgeLast / TapeCursor); 2 grays (#808080, #8A8A8A) do the same "dimmed" job.

### Typography

- Family: Fluent default (Segoe UI on Windows); Inter registered but not selected.
- Sizes: Fluent defaults (14) in XAML; one override `FontSize="32"` (RoPlate plate field). In code: 11 (descriptions), 14 (canvas labels), 16 (tape), 20 (verdict mark).
- Weights: `SemiBold` ×9 (section titles, table headers), `Bold` ×7 (✓/✗, current row). No defined heading scale: section titles are just 14 SemiBold; tab headers are Fluent's large (~24) headers, so the tabs are the biggest text in the window.

### Spacing

Values used: 4, 6, 8, 12, 16 (plus 2 in table cells). 14 distinct Margin/Padding/Spacing strings across 7 views. Most-used: `Margin="0,0,4,4"` ×7 (button wraps), `ColumnSpacing="8"` ×6. Loosely a 4-pt scale, but 6 appears ×3 and nothing is a named token.

### Radius, elevation, borders, opacity

- Radius: Fluent defaults only (≈4 on buttons / text boxes). Canvas shapes are circles.
- Elevation: none (flat; only Fluent's flyout/menu shadows).
- Borders: δ table cells 1 px right/bottom; status bar 1 px top; RoPlate field 3 px colored; canvas strokes 1.5 (normal), 3 (selected / last edge), 4 (highlighted state); tape cells 1, error cell 3.
- Opacity: none set.

### Motion

- No XAML transitions or animations. Only Fluent's built-in control transitions.
- Simulation auto-play: timer 100–2000 ms per step (slider, no label).

## Components inventory

| Component | Variants | Shared? |
|---|---|---|
| Button | 1 (Fluent default) — no primary/secondary distinction; "Start", "Run tests", "Remove" all look the same | Fluent |
| Icon button (↶ ↷) | text glyphs at default size, read as tiny | inline in `MainWindow.axaml` |
| TextBox | default; RoPlate variant with 32 px + `.valid` / `.blocked` 3 px borders | per-view styles |
| CheckBox, Slider, Menu, TabControl | Fluent defaults | Fluent |
| Section title | `TextBlock FontWeight="SemiBold"` repeated inline ×6 | **copy-pasted**, no style class |
| Error text | `TextBlock.error` style | defined only in `EditorPanel.axaml` |
| Verdict line (✓/✗ + text) | **3 copies**: `SimulatorPanel`, `BatchTestPanel` (row), `RoPlateWindow` | copy-pasted |
| "Remove" row pattern | 3 copies (symbols, classes, test rows) | copy-pasted |
| Tables | 3 hand-built grids: δ table (fixed 64 px cells, borders), execution table (shared size groups, no borders), batch tests (grid rows) | each its own |
| Status bar | 1 | `MainWindow.axaml` |
| Custom drawn | `DfaCanvas` (states, edges, highlight, descriptions), `InputTape` | code |
| Empty states | **none** (empty canvas is a black void; empty Tests tab shows only headers) | — |
| Toasts / dialogs | OS file pickers only; messages go to the status bar | — |

### Screens

1. **Main window** — canvas + right tab panel (Edit / Simulate / Tests) + status bar. **The Simulate tab is the most representative screen** (canvas highlight + tape + execution table + verdict).
2. **RoPlate window** — 520 px fixed, plate textbox + verdict + "Show automaton".

Screenshots (dark mode, Windows, 1100×700 @ 125%): `public/design/before/01-main-empty.png`, `02-main-edit-example.png`, `03-main-simulate.png`.

## Problems (ranked)

1. **Two competing accents.** Simulation uses blue (`Brush.StateCurrent`) on the canvas and tape, while the tab underline, slider and current execution row use the OS accent (purple here, any color elsewhere). The same "current step" concept is blue on the canvas and purple in the table. `ExecutionTable.axaml:19`, `App.axaml` (no accent set).
2. **No visual hierarchy / no primary action.** All buttons are identical gray; tab headers (~24 px) dominate the content; section titles are body-size SemiBold. `SimulatorPanel.axaml`, `BatchTestPanel.axaml`, `EditorPanel.axaml`.
3. **Pure black dark background, no surfaces.** Canvas, panel and menu all sit on #000000; the canvas/panel boundary and the `GridSplitter` (4 px) are invisible. `MainWindow.axaml:74-100`.
4. **Light-mode contrast failures.** `Brushes.Gray` descriptions 3.95:1 and `Brush.TapeRead` #8A8A8A 3.45:1 on white — both below 4.5:1 for 11–16 px text. Descriptions are also not themed (`DfaCanvas.Drawing.cs:61`). Missing δ cell fill vs background: 1.18:1 (light) / 1.23:1 (dark) — acceptable only because "—" carries the meaning.
5. **Unlabeled / undersized controls.** Speed slider has no label or value (`SimulatorPanel.axaml:44`); ↶ ↷ glyphs are tiny; ✓ on the canvas and in the verdict line is small for a key result.
6. **Copy-pasted patterns** (verdict line ×3, remove row ×3, section title ×6, error style local to one view) — conflicts with constitution IV.
7. **No empty states.** New document = blank black canvas with no hint how to add a state (double-click / Shift+drag are undiscoverable). Tests tab = bare headers.
8. **Inconsistent tables.** Three hand-rolled grids with different padding, borders and header treatment.
9. **Inter registered but never selected**; typography split between system font (XAML) and `Typeface.Default` (canvas).
10. **Ad-hoc spacing** (4/6/8/12/16 mixed, 14 distinct strings, no tokens).

### Status after the migration (2026-11-12)

| # | Problem | Status |
|---|---|---|
| 1 | Two competing accents | **Resolved.** The Fluent accent is overridden with umber/caramel. Current step, tabs, slider and current row all use `Brush.Accent`. |
| 2 | No hierarchy | **Resolved.** Primary (Start, Run tests), secondary and ghost buttons; 13 px tabs; Fraunces section titles. |
| 3 | Pure black dark mode | **Resolved.** Leather surfaces (`Bg` < `Surface` < `SurfaceRaised`), border lines, and a visible 6 px splitter grip. |
| 4 | Light-mode contrast | **Resolved.** Descriptions and read tape symbols use `Brush.TextMuted` (7.4:1). The export keeps gray on white. |
| 5 | Unlabeled / undersized controls | **Resolved.** Labeled "Step delay" slider with its value, 16 px undo/redo icons, verdict box. **Open:** the ✓ glyph in Inter is still visually light at 20 px. |
| 6 | Copy-pasted patterns | **Resolved.** One `VerdictLine` control, a chip pattern, and shared `heading` / `field-error` / `th` / `mark` styles. |
| 7 | No empty states | **Resolved.** Canvas hint overlay; Tests tab hint line. |
| 8 | Inconsistent tables | **Mostly resolved.** One header and row treatment (table-header / table-row / th). The δ table keeps its fixed grid cells by design. |
| 9 | Typography split | **Resolved.** Inter is the window and canvas font, Fraunces is for titles, IBM Plex Mono is for symbols. |
| 10 | Ad-hoc spacing | **Resolved.** Every view uses the `Gap.*`, `Padding.*` and `Margin.*` tokens. |

After screenshots: `public/design/after/` (dark and light).

### Contrast (computed, WCAG 2.1)

| Pair | Light (on #FFF) | Dark (on #000) |
|---|---|---|
| Body text | 21:1 ✓ | 21:1 ✓ |
| StateCurrent | 6.14 ✓ | 11.67 ✓ |
| Accepted | 5.37 ✓ | 10.35 ✓ |
| Rejected | 5.66 ✓ | 10.34 ✓ |
| Descriptions (#808080) | **3.95 ✗** | 5.32 ✓ |
| Tape read | **3.45 ✗** | 5.32 ✓ |
| Text on missing cell | 17.8 ✓ | 13.4 ✓ |

Note: on a #2B2B2B dark surface (if the redesign adds one) #808080 drops to 3.59 ✗ — the dimmed gray must be retuned with any new background.

## Keep list

User answers, 2026-11-12. The user treats the current UI as a mockup, so anything not listed here may change.

1. **Drawing conventions** (also required by contract 3 §5): state circle radius 28, final double circle 22, start arrow labeled "start", 4 px current-state outline, thick last edge, ✓ / ✗ next to the state, thick blocked tape cell, small muted description under the state, "not color alone" everywhere. PNG export stays black on white.
2. **Compact density**: tight 4–8 px spacing, small tables, Simulate tab content fits without scrolling at 1100×700 (min 800×500).

Fixed by the contracts (not up for change in this redesign, though the user did not pick them): the brush **key names** (`Brush.StateCurrent`, `Brush.Accepted`, `Brush.Rejected`, `Brush.EdgeLast`, `Brush.MissingCell`, `Brush.TapeRead`, `Brush.TapeCursor`) and the main window structure (menu row, one canvas, Edit / Simulate / Tests tabs, status bar, separate RoPlate window). Their **values and look** are free to change.

Not kept: the current status color values, the Fluent default look.

## Change list

Everything the user picked as "most wrong":

1. **Mixed accents.** Pick one app accent and set it explicitly so the OS accent stops leaking in. "Current step" must be the same color on the canvas, tape and execution table.
2. **Flat pure-black dark mode.** Add surfaces: background vs. canvas vs. side panel vs. menu, with a visible splitter. Retune the muted grays for the new surfaces (#808080 fails on #2B2B2B).
3. **No hierarchy.** Primary / secondary / subtle buttons (Start, Run tests = primary; Remove = subtle). Smaller tab headers, a real section title style.
4. **Bare / undiscoverable.** Empty states (canvas: how to add a state / transition; Tests: how to add a word), a labeled speed slider with its value, real undo/redo icons with a usable size, a bigger verdict mark.

Also from the audit (problems 4, 6, 8, 9, 10): fix the light-mode contrast of descriptions and tape (≥ 4.5:1), theme the description brush, extract the copy-pasted verdict line / remove row / section title into shared styles, one table style, spacing tokens.

## Constraints

- **Font: Inter** (already registered with `.WithInterFont()`; select it as the app font and use it on the canvas too).
- Light/dark: the user did not list "both required". The app currently follows the OS theme and has both variants; the themes step should still design both unless the user decides otherwise.
- Fluent: not required. Restyling through resources is the easiest path, but the component library may change.
- WCAG AA (constitution III): 4.5:1 body text, 3:1 large text and UI parts.

## References

- **Aim for: a teaching tool, clear and calm** (Desmos-like; JFLAP done well). Calm, low-saturation colors; the diagram is the hero and readable at a glance; friendly hints over dense chrome. Balanced with the kept compact density: calm does not mean airy.
- Avoid: stock-Fluent "settings page" look, OS-dependent accent colors, pure black backgrounds.

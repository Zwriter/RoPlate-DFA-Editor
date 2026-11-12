# RoPlate Design Rules

Source of truth for all UI in `src/Dfa.App`. Tokens live in `src/Dfa.App/Themes/Tokens.axaml`.
Background: `public/design/audit.md` (what existed), `public/design/chosen-theme.md` (why this theme).
Last updated: 2026-11-12

These rules sit under the constitution (`public/docs/constitution.md`) and contract 3 (`public/docs/plan/contracts/03-app-ui.md`). Where a rule restates a contract requirement, the contract wins.

## 1. Direction

- **Personality:** warm and editorial; a calm teaching tool.
- **Feel in one sentence:** an aged cream notebook page written in sepia ink, with dark-brown annotations, where the automaton drawing is the hero.
- **Signature detail:** the canvas is aged dot-grid paper: `Brush.CanvasDot` dots, 1.2 px, every 18 px (`Size.CanvasDotGap`), on `Brush.Surface`, with a faint edge vignette (`Color.Vignette`). Section titles are set in Fraunces (`Font.Display`). The PNG export stays plain black on white, with no dots and no vignette.
- **Never look like:**
  - A generic developer tool: cold blue-gray neutrals, a hard ruled graph-paper grid, steel-blue accents (rejected direction A).
  - A playful consumer app: pill buttons, drop shadows, floating cards, springy or overshoot motion (rejected C).
  - A stark spec sheet: pure gray or pure black surfaces, black primary buttons, UPPERCASE letter-spaced headings, heavy 2 px dividers (rejected D).
  - Clean white paper: near-white surfaces or blue accents in this paper theme (rejected original Notebook).
  - Stock Fluent: the OS accent color, pure `#000` dark backgrounds, the large Fluent tab headers (audit).

## 2. Tokens

All keys below are resources in `Themes/Tokens.axaml`. Use them with `{DynamicResource Key}` in XAML (theme-aware) or `ThemeBrush.Find(this, "Key", fallback)` in drawn controls.

### Color (semantic)

| Token | Light | Dark | Use for |
|---|---|---|---|
| `Brush.Bg` | #EDE2C8 | #1C1611 | Window background, gaps around panels |
| `Brush.Surface` | #F5ECD6 | #241C15 | Canvas paper, side panel, menu row, status bar |
| `Brush.SurfaceRaised` | #FBF5E6 | #2E241B | Inputs, secondary buttons, tape cells, chips, menus and tooltips |
| `Brush.Border` | #DACAA4 | #3E3125 | Dividers, table row lines, panel edges, disabled outlines |
| `Brush.BorderStrong` | #8A7652 | #847058 | Input and button outlines, table header rule, checkbox box |
| `Brush.Text` | #2E2216 | #F0E4CC | Primary text and every canvas stroke |
| `Brush.TextMuted` | #5B4830 | #C2AF8F | Labels, table headers, state descriptions, read tape symbols, status bar |
| `Brush.TextSubtle` | #76633F | #9A8768 | Placeholders and disabled text only, never information |
| `Brush.Accent` | #7A4B24 | #D9B48A | Primary button, current step (canvas, tape, table), active tab, focus ring, slider |
| `Brush.AccentHover` | #5E3818 | #E6C8A3 | Primary button hover / pressed |
| `Brush.AccentContrast` | #FBF5E6 | #1C1611 | Text and icons on `Brush.Accent` |
| `Brush.AccentSoft` | #E6D6B2 | #3A2C1F | Current table row, ghost button hover, list row hover |
| `Brush.Success` | #3D6A27 | #9CC27E | Accepted / passed only |
| `Brush.SuccessSoft` | #E3DFC4 | #302D20 | Accepted verdict box fill |
| `Brush.Warning` | #875A0A | #E3C35A | Validation warnings only |
| `Brush.Danger` | #9E2B3A | #F09A9A | Rejected / failed / error text and borders only |
| `Brush.DangerSoft` | #ECD9C6 | #382922 | Rejected verdict box fill |
| `Brush.OnDanger` | #FBF5E6 | #1C1611 | Icon on a `Danger` fill (close button hover) |
| `Brush.Info` | = Accent | = Accent | Neutral notices (rare) |
| `Brush.MissingFill` | #F0D6CF | #42262A | Missing δ cell (always shows "—") |
| `Brush.FocusRing` | = Accent | = Accent | Keyboard focus outline |
| `Brush.CanvasDot` | = Border | = Border | Dot grid on the canvas |
| `Color.Vignette` | #1A7A4B24 | #59000000 | Canvas edge vignette (umber at 10% / black at 35%) |
| `Brush.CanvasPaper` | tiled Surface + CanvasDot | same | Canvas background (dot grid) |
| `Brush.CanvasVignette` | radial, clear → Vignette | same | Overlay on top of the canvas, not hit-testable |

**Contract keys (contract 3 §5).** The 7 contract brushes keep their names and their place in `Themes/Styles.axaml`. Their values become aliases of the semantic tokens:

| Contract key | Alias of |
|---|---|
| `Brush.StateCurrent`, `Brush.EdgeLast`, `Brush.TapeCursor` | `Brush.Accent` |
| `Brush.Accepted` | `Brush.Success` |
| `Brush.Rejected` | `Brush.Danger` |
| `Brush.MissingCell` | `Brush.MissingFill` |
| `Brush.TapeRead` | `Brush.TextMuted` |

The **Fluent accent** (`SystemAccentColor`, `…Light1–3`, `…Dark1–3`) is overridden in `Tokens.axaml` with the umber/caramel ramp. The OS accent must never show. The same Fluent bridge section also maps:
- the focus visual brushes;
- the menu, flyout and tooltip backgrounds;
- the list highlight;
- the text selection color;
- the slider thumb and track;
- the theme font family and size (`ContentControlThemeFontFamily`, `ControlContentThemeFontSize`, `TabItemHeaderFontSize`).

Views never reference these Fluent keys.

### Type

| Token | Size / line height | Weight | Family | Use for |
|---|---|---|---|---|
| `FontSize.Plate` | 28 / 1.0 | `Weight.SemiBold` | `Font.Mono` | RoPlate plate field only |
| `FontSize.Verdict` | 20 / 1.0 | `Weight.Bold` | `Font.Body` | ✓ / ✗ marks (verdict box, canvas) |
| `FontSize.Title` | 16 / 1.3 | `Weight.SemiBold` | `Font.Display` | Empty-state titles, window titles inside content |
| `FontSize.Symbol` | 16 / 1.0 | `Weight.Regular` | `Font.Mono` | Input tape characters |
| `FontSize.Heading` | 15 / 1.3 | `Weight.SemiBold` | `Font.Display` | Section titles ("Alphabet (Σ)", "Transition table δ") |
| `FontSize.CanvasLabel` | 14 / 1.0 | `Weight.Regular` (current state `Weight.SemiBold`) | `Font.Body` | State names, edge labels, "start" |
| `FontSize.Body` | 13 / 1.45 | `Weight.Regular`; buttons, tabs, menus `Weight.Medium` | `Font.Body` | Default text and every control |
| `FontSize.Small` | 12 / 1.4 | `Weight.Regular`; table headers `Weight.SemiBold` | `Font.Body` | Tables, labels, field errors, status bar |
| `FontSize.Caption` | 11 / 1.35 | `Weight.Regular` | `Font.Body` | State descriptions on the canvas, kbd hints |
| (mono) | inherits size | inherits | `Font.Mono` | Symbols and words: alphabet chips, δ table cells, execution Read / Remaining, test words |

Families: `Font.Body` = Inter, `Font.Display` = Fraunces, `Font.Mono` = IBM Plex Mono. All are SIL OFL and are bundled in `src/Dfa.App/Assets/Fonts` (licence files beside them), never loaded from the web. Avalonia ignores the weight axis of variable fonts, so Fraunces ships as two static instances, Regular and SemiBold (opsz 16, SOFT 0, WONK 0), cut from the google/fonts variable file. Fraunces has no Reserved Font Name, so the name is kept. The IBM Plex Mono files are unmodified, because "Plex" is a Reserved Font Name. Inter comes from `Avalonia.Fonts.Inter`.

### Spacing

| Primitive | Value | Semantic token | Use for |
|---|---|---|---|
| `Space.1` | 4 | `Gap.Inline` | Icon ↔ label, chip internals, button wrap gap |
| `Space.1_5` | 6 | `Gap.Related` | Items in one group (input + its button, verdict mark + text) |
| `Space.2` | 8 | `Gap.Group`, `Padding.Panel` | Between groups in a panel; panel padding |
| `Space.3` | 12 | `Gap.Section` | Between major sections (Alphabet → Symbol classes → δ table) |
| `Space.4` | 16 | `Padding.Window` | Padding of small windows (RoPlate) |
| `Space.6` | 24 | `Gap.EmptyState` | Inside empty states only |

Thickness tokens:
- **Padding:** `Padding.Panel` 8, `Padding.Window` 16, `Padding.Control` 10,0, `Padding.Input` 8,4, `Padding.Tab` 10,6, `Padding.Cell` 6,3, `Padding.StatusBar` 10,4, `Padding.Chip` 8,2,4,2, `Padding.Verdict` 8, `Padding.Kbd` 5,0.
- **Margin:** `Margin.RowGap` 0,0,0,4, `Margin.WrapGap` 0,0,4,4, `Margin.SectionTop` 0,12,0,0, `Margin.SectionBottom` 0,0,0,12, `Margin.MarkGap` 0,0,6,0, `Margin.Separator` 6,0.
- **BorderThickness:** `Thickness.Hairline` 1, `Thickness.Emphasis` 2, `Thickness.LastEdge` 3, `Thickness.Top` / `.Bottom` / `.Right` (1 px on one side), `Thickness.CellLines` 0,0,1,1, `Thickness.CurrentBar` 3,0,0,0, `Thickness.Kbd` 1,1,1,2.

### Size

| Token | Value | Use for |
|---|---|---|
| `Size.Control` | 28 | Height of buttons, text boxes, icon buttons (also their width) |
| `Size.Icon` | 16 | Icon glyphs inside buttons |
| `Size.TapeCell` | 32 | Input tape cell (drawn) |
| `Size.SidePanel` | 380 (GridLength) | Default width of the right tab panel |
| `Size.Splitter` | 6 | GridSplitter hit area (grip visible) |
| `Size.DeltaCell` | 64 | δ table cell width |
| `Size.ChipButton` | 18 | × remove button in chips and class rows |
| `Size.Separator` / `Size.GripDot` / `Size.GhostState` | 18 / 3 / 56 | Menu-row separator height, splitter grip dots, empty-state ghost circle (2 × radius 28) |
| `Size.TitleBar` / `Size.CaptionButton` / `Size.CaptionIcon` | 36 / 46 / 10 | Menu-row (title bar) minimum height, caption button width, caption glyph size |
| `Size.CanvasDotGap` | 18 | Dot-grid spacing |

### Radius

| Token | Value | Use for |
|---|---|---|
| `Radius.Small` | 4 | Tape cells, checkboxes, kbd hints, δ table outer corners |
| `Radius.Control` | 6 | Buttons, inputs, chips, verdict box, menus, tooltips |
| `Radius.Panel` | 8 | Empty-state card, dialogs, popups |

There is no pill radius. Circles exist only on the canvas (states).

### Stroke

| Token | Value | Use for |
|---|---|---|
| `Stroke.Hairline` | 1 | Borders, dividers, tape cells |
| `Stroke.Edge` | 1.5 | Canvas state circles and edges (contract) |
| `Stroke.Emphasis` | 2 | Input error border, active tab underline, focus ring |
| `Stroke.LastEdge` | 3 | Last edge taken, current row left bar, blocked tape cell |
| `Stroke.Highlight` | 4 | Current / accepted / rejected state outline (contract) |

### Elevation

None. No `BoxShadow` anywhere. Depth comes from the surface steps (§6). The only exception is the platform's own shadow on menus, context menus and tooltips.

### Motion

| Token | Value | Use for |
|---|---|---|
| `Duration.Fast` | 0:0:0.14 | Hover / pressed colors, focus ring |
| `Duration.Base` | 0:0:0.2 | Reserved for future fades. Nothing uses it yet. |
| `Easing.Standard` | spline 0.3, 0, 0.2, 1 | Reserved. Today's button transitions use the default linear easing at 140 ms. |

## 3. Color usage

- **One accent, umber.** It is used for: the primary button, the current step (canvas outline, last edge, tape cursor, current table row bar), the active tab underline, the focus ring, the slider and checked checkboxes. Nothing else is accent-colored: no accent text links, headings or decorations.
- **Current step = accent in every view.** The canvas, tape, execution table and tabs must agree. Never mix in another hue for "current".
- **Status colors only for status.** Success = accepted / passed, Danger = rejected / failed / error, Warning = validation warnings. Never use them for decoration or emphasis.
- **Not color alone** (constitution III, contract 3). Every color signal has a second channel:

  | Signal | Second channel |
  |---|---|
  | current state | 4 px outline |
  | accepted / rejected | ✓ / ✗ mark |
  | last edge | 3 px stroke |
  | current row | bold text + left bar |
  | missing cell | "—" |
  | input error | 2 px border + ✗ + message |
  | blocked tape cell | 3 px outline |

- **Dark mode** is leather, not black. Surfaces get lighter as they rise: `Bg` < `Surface` < `SurfaceRaised`. Never use `#000` or a neutral gray. Text is cream, never white.
- **Light mode** is aged paper, not white. `SurfaceRaised` (#FBF5E6) is the lightest color in the app. Never use `#FFF`.
- **PNG export** is the only place with black on white (`Brushes.Black`, `Brushes.White` in `DfaCanvasImage`). It is deliberate and theme-independent.

## 4. Typography

- `Font.Body` (Inter) is the window default: set `FontFamily` on `Window` / in the app style, not per control. Drawn controls use `Font.Body` / `Font.Mono` through resources, never `Typeface.Default`.
- `Font.Display` (Fraunces) is only for `FontSize.Heading` and `FontSize.Title`. Never use it for body text, controls, tables or the canvas.
- `Font.Mono` is for automaton symbols and words wherever they appear as data (chips, δ cells, tape, execution Read / Remaining, test words, plate). Labels around them stay in `Font.Body`.
- A panel uses at most 3 sizes (typically Heading, Body and Small) and 2 weights besides Bold marks.
- Tabular data: the mono family already gives aligned figures. Step numbers in `Font.Body` are fine.
- **Sentence case** everywhere ("Run to end", "Expected accepted"). Two exceptions: the verdict words **ACCEPTED** / **REJECTED** are uppercase because they are contract messages, and nothing else is uppercase.
- Section titles are plain `TextBlock`s with `Classes="heading"`. Never repeat `FontWeight="SemiBold"` inline for a title.

## 5. Layout and spacing

- **Window sizes to verify** (constitution III): 1100×700 (default) and 800×500 (minimum) for the main window, 520 wide for RoPlate. The Simulate tab content must fit at 1100×700 without vertical scrolling for a word up to 10 symbols and 6 execution rows.
- **Main window structure (contract 3 §2):** menu row on top, canvas left, `Size.SidePanel` tab panel right, status bar at the bottom. There is one canvas. Do not add, remove or reorder these regions.
- **Inside a panel:** padding `Padding.Panel`, `Gap.Group` between groups, `Gap.Section` before each new section title, `Gap.Related` inside an input + button pair.
- **Rows of buttons** use a `WrapPanel` with `Margin.WrapGap` per button so they wrap at 800 px.
- **Controls are `Size.Control` (28) tall.** No taller controls except the RoPlate plate field.
- **Alignment:** labels left-aligned, numbers left-aligned (mono). Table symbol columns are centered only in the δ table.

## 6. Depth and surfaces

| Layer | Token | What sits on it |
|---|---|---|
| 0 | `Brush.Bg` | window; visible only as gaps and around RoPlate content |
| 1 | `Brush.Surface` | canvas, side panel, menu row, status bar |
| 2 | `Brush.SurfaceRaised` | inputs, secondary buttons, chips, tape cells, menus, tooltips |

- Regions are separated by a `Stroke.Hairline` `Brush.Border` line, never by a shadow.
- The canvas / side panel boundary is a 1 px `Brush.Border` line plus a `Size.Splitter` GridSplitter with a visible grip (3 dots in `Brush.BorderStrong`).
- **No cards inside cards.** A panel holds groups directly. The verdict box and the empty-state card are the only boxed elements, and they never contain another box.

## 7. Components

States legend: **D** default · **H** hover · **F** focus-visible · **P** pressed · **X** disabled.

### Button

| Variant | Use | D | H / P | X |
|---|---|---|---|---|
| `primary` | The one main action of a panel: Start, Run tests. **At most one per panel.** | `Accent` fill, `AccentContrast` text, no border | `AccentHover` fill | transparent fill, `Border` outline, `TextSubtle` text |
| (default) secondary | Add, Step forward / back, Run to end, Play / Pause, Show automaton | `SurfaceRaised` fill, `BorderStrong` 1 px, `Text` | border → `Text` | as above |
| `ghost` | Low-emphasis or repeated actions: Remove, Reset (last in its row), Complete with trap state | transparent, no border, `TextMuted` | `AccentSoft` fill, `Text` | `TextSubtle` |
| danger | **Not used.** Every destructive action in RoPlate is undoable (Ctrl+Z), so Remove is ghost. Add a danger variant only with a token change here first. | | | |

- Size: one size, `Size.Control` tall, `Padding.Control`, `Radius.Control`, `FontSize.Body` `Weight.Medium`.
- **F:** `Stroke.Emphasis` `Brush.FocusRing` outline, 2 px offset, on every variant.
- Transitions: background and border over `Duration.Fast`. The value is written into `Themes/Controls.axaml`, because transitions live outside the resource tree.
- There is no loading state: all operations are local and instant.

### Icon button

- Used for undo ↶ and redo ↷ in the menu row. It is a `Size.Control` square with a `Size.Icon` stroke icon (`PathIcon`), styled as ghost.
- Must keep `ToolTip.Tip` with the shortcut ("Undo (Ctrl+Z)") and `AutomationProperties.Name`.
- **X:** icon in `TextSubtle`.

### TextBox

| State | Look |
|---|---|
| D | `SurfaceRaised` fill, `BorderStrong` 1 px, `Radius.Control`, `Size.Control` tall, `FontSize.Body`, placeholder `TextSubtle` |
| H | border `Text` |
| F | border `Accent` `Stroke.Emphasis` (replaces the Fluent bottom bar) |
| Error (`Classes="error"`) | border `Danger` `Stroke.Emphasis`; below it the field error line |
| X | `Bg` fill, `Border` outline, `TextSubtle` text |

- **Field error line:** `TextBlock Classes="field-error"`: ✗ + the OpResult message verbatim, `Danger`, `FontSize.Small`, wraps. Hidden when null. It is one shared style, not a per-view style.
- **Word inputs** (simulator, tests) use `Font.Mono`, and their placeholder is "ε".
- **RoPlate plate field:** `FontSize.Plate`, `Font.Mono`, 0.08em letter spacing. `.valid` = `Success` `Stroke.LastEdge` border + ✓. `.blocked` = `Danger` `Stroke.LastEdge` border + ✗.

### CheckBox

- The box is 16 px, `Radius.Small`, `BorderStrong` outline. When checked: `Accent` fill with an `AccentContrast` tick. Label in `FontSize.Body`.
- Used for: "Guess" and "Expected accepted" in a test row. In table rows the checkbox has `AutomationProperties.Name` = "Expected accepted".

### Slider (speed)

- Always labeled. Layout: "Step delay" label (`FontSize.Small`, `TextMuted`), then the slider, then the value ("600 ms", `Font.Mono`, `FontSize.Small`), in a `Gap.Group` grid. The label is "Step delay", not "Speed", because a higher value is slower.
- The track is `Border`, filled with `Accent`. The thumb is 14 px, `SurfaceRaised` fill, `Accent` 2 px border.

### Menu row and menus

- The menu row is on `Surface` with a `Border` line below it, at least `Size.TitleBar` tall. Order: File, Examples, Edit, Apps | undo, redo | "Complete with trap state" (ghost), then the window title, then the caption buttons. The contract 3 §2 order is kept. Settings lives at the end of the File menu.
- Separators are 1 px `Border`, 18 px tall, `Gap.Related` on each side.
- Menu flyouts and context menus: `SurfaceRaised`, `Radius.Control`, items `FontSize.Body`, hover `AccentSoft`, gesture text `TextMuted`.

### Tabs (Edit / Simulate / Tests)

- Text tabs: `FontSize.Body` `Weight.Medium`, padding 10,6. **D:** `TextMuted`. **H:** `Text`. **Selected:** `Text` with a `Stroke.Emphasis` `Accent` underline. A 1 px `Border` line runs under the whole strip.
- Never the large Fluent headers. Order: Edit, Simulate, Tests (`MainTab`).
- The underline switches instantly (Fluent's `PART_SelectedPipe`, recolored to `Accent`).

### Section title

- `TextBlock Classes="heading"`: `Font.Display`, `FontSize.Heading`, `Weight.SemiBold`, `Text`, `Gap.Section` above (0 for the first).

### Verdict line (shared control)

- One templated control, `Controls/VerdictLine` (properties `Text`, `IsAccepted`, `IsRejected`; look in its ControlTheme in `Themes/Controls.axaml`). It is used in SimulatorPanel and RoPlateWindow, and replaces the copy-pasted ✓ / ✗ stacks. Test rows use `TextBlock Classes="mark ok"` / `"mark no"` instead.
- Layout: mark (`FontSize.Verdict` `Weight.Bold`), then the bold verdict word, then the message (contract text verbatim), with `Gap.Related` between them.

  | State | Border | Fill | Mark |
  |---|---|---|---|
  | Accepted | `Success` | `SuccessSoft` | `Success` ✓ |
  | Rejected | `Danger` | `DangerSoft` | `Danger` ✗ |

- The box: `Radius.Control`, `Stroke.Hairline` border, padding 8.
- With neither flag set (mid-run, "Step 0 of 3."; RoPlate "Incomplete format…") it shows the plain text with no box and no mark. The views hide it when nothing runs.

### Symbol chip (alphabet)

- `Font.Mono` symbol + a × ghost remove button (18 px, `AutomationProperties.Name` = "Remove {symbol}").
- `SurfaceRaised` fill, `BorderStrong` 1 px, `Radius.Control`, `Padding.Chip`. Chips sit in a `WrapPanel` with `Margin.WrapGap`.
- Symbol classes use the same row pattern: name (`Weight.SemiBold`), then characters (`TextMuted`, trimmed, tooltip with the full list), then the × button.

### Tables (δ table, execution table, tests list)

One shared set of styles (`Classes="table"`, `"table-header"`, `"cell"`):

| Part | Look |
|---|---|
| Header row | `FontSize.Small` `Weight.SemiBold` `TextMuted`, `BorderStrong` 1 px rule below |
| Rows | `FontSize.Small`, `Padding.Cell`, `Border` 1 px line between rows |
| Symbol columns | `Font.Mono` |
| Current row (execution) | `AccentSoft` fill + `Weight.Bold` + `Stroke.LastEdge` `Accent` left bar |
| Missing cell (δ) | `MissingFill` + "—" |
| Hover (tests list) | `AccentSoft` |

- The δ table also draws `Border` vertical lines, has fixed 64 px cells and centered text. The row labels keep "→" (start) and "*" (final), with the legend ("→ start · * final · — missing") directly under the heading in `FontSize.Small` `TextMuted`.
- Tests list row: word `TextBox` (mono), then "Expected accepted" checkbox, then the result mark (✓ `Success` / ✗ `Danger`, bold, 22 px column), then a ghost Remove. Double-click opens the word in the simulator. The summary line ("1 of 2 tests passed.") goes below in `FontSize.Small`.

### Input tape (drawn)

- Cells are `Size.TapeCell` squares, `Stroke.Hairline` `BorderStrong`, `SurfaceRaised` fill, characters in `Font.Mono` `FontSize.Symbol`.
- Read symbols in `Brush.TapeRead` (= `TextMuted`). Space is shown as "␣".
- Cursor: an `Accent` triangle under the next cell. A blocked cell gets a `Danger` `Stroke.LastEdge` outline.

### Canvas (drawn; keep list)

- **Geometry stays exactly as contract 3:** radius 28, final 22, start arrow 40 labeled "start", loops and bends from `CanvasGeometry`.
- **Strokes:** `Brush.Text` at `Stroke.Edge`. Selected state: dashed ring at +5. Selected edge: `Stroke.LastEdge`. Last edge: `Brush.EdgeLast` at `Stroke.LastEdge`. Current / accepted / rejected: `Stroke.Highlight` outline in `StateCurrent` / `Accepted` / `Rejected`, plus ✓ / ✗ at `FontSize.Verdict` beside the state.
- **Text:** labels in `Font.Body` `FontSize.CanvasLabel`. Descriptions in `Brush.TextMuted` `FontSize.Caption`, wrapped at 140. The hardcoded `Brushes.Gray` goes away.
- **Background:** set by the window, not the control. The canvas `ScrollViewer` sits on a `Brush.CanvasPaper` border, with a `Brush.CanvasVignette` overlay on top. The control itself stays transparent, and state circles are unfilled; edges end at the circle, so nothing shows through.
- **Export** (`DfaCanvasImage`): black on white, no grid, no vignette.

### Empty states

| Where | Content |
|---|---|
| Canvas, no states | Centered `Radius.Panel` area without a border. Dashed `BorderStrong` ghost circle (radius 28) with "q0" in `TextSubtle` mono. Title "Start with a state" (`FontSize.Title`, `Font.Display`). Hint (`FontSize.Body`, `TextMuted`): "Double-click anywhere to add a state. Shift + drag from one state to another to add a transition." Below it in `FontSize.Small`: "Or open one from the Examples menu." The overlay is not hit-testable, so double-clicks reach the canvas. There is no button, because a flyout cannot reach the window's commands. It shows while `MainViewModel.IsDocumentEmpty`. |
| Tests tab, no rows | One line in `TextMuted`: "Add a word, tick whether it should be accepted, then Run tests." |
| Simulate tab, before Start | The tape and table stay hidden (as now). Show only the input and the hint placeholder "ε". |

- Kbd hint: `Font.Mono` 11, `SurfaceRaised`, `BorderStrong` 1 px with a 2 px bottom, `Radius.Small`.

### Window title bar (app-drawn)

- Both windows use `ExtendClientAreaToDecorationsHint="True"` and `WindowDecorations="BorderOnly"`. There is no system title bar, but the native frame, shadow, resize edges and snapping remain.
- **Main window:** the menu row is the title bar. Its free middle is a `Border` with `chrome:WindowDecorationProperties.ElementRole="TitleBar"` (and a transparent background). It is empty: no title text is drawn in the bar. `Window.Title` still names the window in the taskbar and Alt+Tab. Dragging it moves the window, double-clicking maximizes, and both are native behavior. Interactive elements (menus, buttons) never carry the TitleBar role.
- **RoPlate:** a slim `Border.menubar` with an empty drag area and caption buttons without maximize (`CanMaximize="False"`, because the window has a fixed size).
- **Caption buttons:** one control, `Controls/CaptionButtons` (template in `Themes/Controls.axaml`). It shows minimize, maximize ↔ restore (the `:maximized` pseudo-class swaps the icon and the tooltip) and close.

  | Part | Look |
  |---|---|
  | Button | `Button.caption`, `Size.CaptionButton` wide, full row height, square corners, transparent |
  | Glyph | `Size.CaptionIcon` stroke |
  | Hover | `AccentSoft` fill |
  | Close hover | `Danger` fill + `OnDanger` glyph |

  Each button has a tooltip and an `AutomationProperties.Name`.
- **Maximized:** the root `DockPanel` takes `Margin="{Binding $parent[Window].OffScreenMargin}"`, so nothing is clipped beyond the screen edge.

### Settings menu

- **File ▸ Settings ▸ Theme ▸**, after a separator at the end of the File menu. Its choices "Follow system" / "Light" / "Dark" are radio menu items (`ToggleType="Radio"`, one `GroupName`) bound two-way to `SettingsViewModel.IsSystemTheme` / `IsLightTheme` / `IsDarkTheme`.
- `IThemeService` (`ThemeService`) sets `Application.RequestedThemeVariant`, and every `DynamicResource` follows. The choice is saved through `ISettingsStore` (`JsonSettingsStore`, `%AppData%/RoPlate/settings.json`; a missing or malformed file gives the defaults and never throws) and applied before the main window shows.
- Drawn controls read theme brushes while painting, so they must repaint on `ActualThemeVariantChanged` and on `Foreground` / `FontFamily` changes. `DfaCanvas` and `InputTape` do.
- New settings go into `AppSettings` and get a menu entry here. There is no separate settings window until there are more than a handful of options.

### Status bar

- `Surface` fill, `Border` 1 px top line, `Padding.StatusBar`, `FontSize.Small`, `TextMuted`, single line, trimmed with an ellipsis. It shows OpResult / LoadResult messages verbatim.
- It is the app's only notification channel. There are no toasts.

### Tooltips and dialogs

- Tooltips: `SurfaceRaised`, `Radius.Control`, `FontSize.Small`, `Text`. Every icon-only control has one.
- Dialogs: only OS file pickers and Avalonia message dialogs (`IDialogService`). No custom modal styling beyond `Radius.Panel`, `Surface` and `Padding.Window`.

### GridSplitter

- `Size.Splitter` wide, transparent, with a centered 3-dot grip in `BorderStrong`. On hover the grip turns `Text`. Cursor: resize.

## 8. Motion

- Only button background and border colors animate (`Duration.Fast`). Everything else changes instantly.
- **No animation on the canvas.** Highlights change instantly on each step. The auto-play interval (100–2000 ms, user-set) is the only rhythm.
- Never animate layout, size or position. Only color and opacity.
- Reduced motion: Avalonia has no cross-platform reduced-motion setting, so the budget above (≤ 200 ms, color and opacity only) is the safeguard. If a setting becomes available, all transitions drop to 0.

## 9. Accessibility

- **Contrast:** 4.5:1 for text, 3:1 for large text and UI boundaries. Every token pair in §2 is verified (`public/design/chosen-theme.md` has the ratios). A new color pair needs its ratio computed and added there first.
- `Brush.TextSubtle` is only for placeholders and disabled text. It is ≥ 4.3:1, but it must never carry information.
- **Visible focus:** `Stroke.Emphasis` `Brush.FocusRing` on every interactive control, including canvas focus (dashed ring on the selected state).
- **Keyboard:** every command keeps its shortcut (Ctrl+N/O/S/Shift+S/Z/Y, Enter to submit fields, Delete on the canvas). New commands get a gesture or a menu entry.
- **Names:** icon-only buttons, chip remove buttons and checkboxes in table rows have `AutomationProperties.Name`.
- **Not color alone:** see §3. This is mandatory (constitution III).
- Desktop target sizes: controls ≥ 28×28. The only smaller control is the chip × button (18 px), which is allowed because the symbol can also be removed by keyboard focus + Enter.

## 10. Do / Don't

| Do | Don't |
|---|---|
| `Background="{DynamicResource Brush.Surface}"` | `Background="#F5ECD6"` or `Brushes.Gray` |
| `Classes="primary"` on Start | Make Step forward, Run to end and Start all primary |
| `Classes="heading"` for section titles | `FontWeight="SemiBold"` inline on each title |
| Reuse the verdict line control | Copy the ✓ / ✗ `StackPanel` into a new view |
| `Margin="{StaticResource Margin.WrapGap}"` | `Margin="0,0,4,4"` typed by hand |
| Mono for symbols and words | Mono for labels and buttons |
| Status color + mark / outline / text | Status color alone |
| Contract messages verbatim | Rewording ACCEPTED / REJECTED messages in the view |

## 11. Banned

- Hardcoded colors (`#…`, `Colors.*`, `Brushes.*`), font sizes, font families, radii, shadows or stroke widths in views and controls. Exceptions:
  - `DfaCanvasImage`: the black-on-white export, including its `Brush.TextMuted` override to gray.
  - `Brushes.Transparent` as a hit-test background.
  - `Brushes.Black` / `Brushes.Transparent` as the fallback passed to `ThemeBrush.Find` or used when `Foreground` is null.
  - Drawn controls (`DfaCanvas`, `InputTape`) may keep geometry, stroke and font-size constants equal to their token, each commented with the token name. Their colors and font families still come from resources.
  - Zero values (`Padding="0"`).
- New colors, type sizes or spacing values without adding a token to `Tokens.axaml` and this file first.
- Primitive tokens (`Color.Paper.*`, `Color.Leather.*`, `Space.*`, …) used directly in views. Only semantic tokens are allowed.
- Fluent system resources (`SystemControl*Brush`, `SystemAccentColor`) used directly in views. Map them in `Tokens.axaml` instead.
- `BoxShadow`, pill radii, spring or overshoot easings, uppercase headings, pure white or black surfaces, blue accents.
- Anything in "Never look like" (§1).

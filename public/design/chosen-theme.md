# Chosen theme — Notebook · Old Paper (direction B, revised)

> **Feel:** warm and editorial. It should look like an aged cream page written in sepia ink, with dark-brown annotations.
> The user chose Notebook on 2026-11-12. The same day they asked for a cream / beige base with dark-brown
> accents, "the color of an old piece of paper". This replaced the original ink-blue accent and the near-white
> paper. Everything else from Notebook is unchanged, and it respects every item on the keep list in `public/design/audit.md`.

## Palette

Hex is the source of truth for Avalonia, which has no OKLCH. OKLCH is given for reasoning about lightness.
Every ratio below was computed (WCAG 2.1) and passes AA. No surface is pure white or pure black: old paper is never white.

| Role | Light hex | Light OKLCH | Dark hex | Dark OKLCH |
|---|---|---|---|---|
| `bg` (window, behind panels; aged beige) | #EDE2C8 | oklch(91.5% 0.036 88) | #1C1611 | oklch(20.6% 0.014 62) |
| `surface` (side panel, menu, canvas paper; cream) | #F5ECD6 | oklch(94.4% 0.031 88) | #241C15 | oklch(23.4% 0.018 63) |
| `surface-raised` (inputs, buttons, tape cells) | #FBF5E6 | oklch(97.1% 0.021 89) | #2E241B | oklch(26.9% 0.022 64) |
| `border` (dividers, table rows, canvas dots) | #DACAA4 | oklch(84.3% 0.054 88) | #3E3125 | oklch(32.4% 0.028 64) |
| `border-strong` (input / button outline, table header rule) | #8A7652 | oklch(57.6% 0.057 82) | #847058 | oklch(55.8% 0.043 72) |
| `text` (sepia ink; also canvas strokes) | #2E2216 | oklch(26.3% 0.028 66) | #F0E4CC | oklch(92.2% 0.034 85) |
| `text-muted` (labels, state descriptions, read tape symbols) | #5B4830 | oklch(41.5% 0.045 73) | #C2AF8F | oklch(76.2% 0.049 81) |
| `text-subtle` (placeholders, disabled only, never information) | #76633F | oklch(50.9% 0.057 83) | #9A8768 | oklch(63.3% 0.050 80) |
| `accent` (dark umber brown; caramel in dark mode) | #7A4B24 | oklch(45.9% 0.083 58) | #D9B48A | oklch(79.2% 0.070 70) |
| `accent-hover` | #5E3818 | oklch(38.0% 0.071 58) | #E6C8A3 | oklch(84.9% 0.060 73) |
| `accent-contrast` (text on accent) | #FBF5E6 | oklch(97.1% 0.021 89) | #1C1611 | oklch(20.6% 0.014 62) |
| `accent-soft` (current table row, hover fill) | #E6D6B2 | oklch(88.0% 0.051 87) | #3A2C1F | oklch(30.5% 0.030 64) |
| `success` (olive green) | #3D6A27 | oklch(47.6% 0.109 137) | #9CC27E | oklch(77.0% 0.101 132) |
| `warning` (ochre) | #875A0A | oklch(50.4% 0.103 74) | #E3C35A | oklch(82.5% 0.129 92) |
| `danger` (oxblood crimson) | #9E2B3A | oklch(47.2% 0.150 18) | #F09A9A | oklch(77.3% 0.104 20) |
| `info` | = accent | | = accent | |
| `missing` (empty δ cell; always shows "—") | #F0D6CF | oklch(89.6% 0.031 35) | #42262A | oklch(30.6% 0.043 11) |
| verdict-ok fill (success 10% over surface) | #E3DFC4 | | #302D20 | |
| verdict-bad fill (danger 10% over surface) | #ECD9C6 | | #382922 | |

**Telling the brown accent apart from neighbouring colours.** The accent is brown, so it sits close to the ink and to the danger red.
- **Ink:** the current-step brown is only 1.8:1 (light) / 1.5:1 (dark) against the near-black ink. On the canvas the 4 px outline carries the difference, and the current row has its left bar and bold text. This is the "not color alone" rule doing its job.
- **Danger:** danger was moved from brick red to an oxblood crimson (hue 18 vs the accent's 58, with roughly twice the chroma), so a rejected state never reads as "current". It also always carries ✗.
- **Warning:** in dark mode warning is a yellow ochre, kept away from the caramel accent.

### Contract brush keys (contract 3 §5): the names stay the same, the values change

| Key | Maps to | Light | Dark |
|---|---|---|---|
| `Brush.StateCurrent` | accent | #7A4B24 | #D9B48A |
| `Brush.EdgeLast` | accent | #7A4B24 | #D9B48A |
| `Brush.TapeCursor` | accent | #7A4B24 | #D9B48A |
| `Brush.Accepted` | success | #3D6A27 | #9CC27E |
| `Brush.Rejected` | danger | #9E2B3A | #F09A9A |
| `Brush.MissingCell` | missing | #F0D6CF | #42262A |
| `Brush.TapeRead` | text-muted | #5B4830 | #C2AF8F |
| new: description text (replaces hardcoded `Brushes.Gray`) | text-muted | #5B4830 | #C2AF8F |

The Fluent accent (`SystemAccentColor` and its light/dark variants) is overridden with `accent`. This stops the OS accent from leaking into tabs, sliders and selections. The current step is then dark brown everywhere.

### Contrast (selected; full list in the preview)

| Pair | Light | Dark |
|---|---|---|
| text / bg · surface · raised | 12.0 · 13.2 · 14.2 | 14.2 · 13.3 · 12.0 |
| text-muted / bg · surface · raised | 6.8 · 7.4 · 8.0 | 8.4 · 7.9 · 7.1 |
| text-subtle / bg · surface (≥ 3, non-informational) | 4.5 · 4.9 | 5.2 · 4.8 |
| accent (current) / bg · surface · raised | 5.7 · 6.2 · 6.8 | 9.3 · 8.7 · 7.8 |
| success / surface | 5.4 | 8.3 |
| danger / surface | 6.2 | 7.8 |
| warning / surface | 5.1 | 9.8 |
| border-strong / bg · surface · raised (UI ≥ 3) | 3.4 · 3.7 · 4.0 | 3.8 · 3.6 · 3.2 |
| accent-contrast / accent · accent-hover | 6.8 · 9.4 | 9.3 · 11.2 |
| text / missing | 11.2 | 10.8 |
| text / accent-soft · current / accent-soft | 10.8 · 5.9 | 10.7 · 7.0 |
| verdict mark ✓ / verdict-ok fill · ✗ / verdict-bad fill | 4.8 · 5.4 | 6.9 · 6.5 |

## Typography

| Use | Family | Size / weight / line height |
|---|---|---|
| Panel / empty-state title | Fraunces (serif, opsz 9–144) | 16 / 600 / 1.3 |
| Section title ("Alphabet (Σ)", "Transition table δ") | Fraunces | 15 / 600 / 1.3 |
| Body, buttons, inputs, tabs, menus | **Inter** | 13 / 400 (buttons and tabs 500) / 1.45 |
| Labels, table text, status bar | Inter | 12 / 400–600 / 1.4 |
| Captions, state descriptions on the canvas | Inter | 11 / 400 / 1.35 |
| Canvas state and edge labels | Inter | 14 / 400 (current state 600) |
| Verdict mark ✓ / ✗ | Inter | 20 / 700 |
| Symbols: tape, δ table, words, alphabet chips | IBM Plex Mono | 13 (tape 16) / 400–600 |
| RoPlate plate field | IBM Plex Mono | 28 / 600, letter-spacing 0.08em |

Scale: 11 · 12 · 13 · 14 · 15 · 16 · 20 · 28. Fraunces is for titles only, never body or controls. All three fonts are SIL OFL. Bundle them as `avares://` assets: Inter through `Avalonia.Fonts.Inter` `.WithInterFont()`, Fraunces and IBM Plex Mono as font files. Use the same Inter family on the canvas instead of `Typeface.Default`.

## Spacing, radius, elevation

- **Spacing** (compact, from the keep list): 4 · 6 · 8 · 12 · 16 · 24. Controls are 28 px tall. Gaps inside a group are 4–6, between groups 8, panel padding 8, and 12 between major sections.
- **Radius:** sm 4 (tape cells, checkboxes, kbd), md 6 (buttons, inputs, verdict box, chips), lg 8 (window-level panels, cards in the preview).
- **Elevation:** no shadows. Depth comes from lightness steps: `bg` → `surface` → `surface-raised`, with `border` lines between areas. The canvas sits on `surface`, the side panel on `surface`, and they are split by a 1 px `border` plus a visible splitter grip.

## Motion

| Token | Value | Use |
|---|---|---|
| fast | 140 ms, cubic-bezier(.3, 0, .2, 1) | hover / press colors, focus ring |
| base | 200 ms, cubic-bezier(.3, 0, .2, 1) | tab underline, verdict box appearing, empty-state fade |
| — | none | canvas highlights change instantly on each step; the auto-play speed (100–2000 ms) is the only rhythm |

Honor the OS reduced-motion setting by dropping the transitions.

## Components (as previewed)

- **Buttons:** primary = dark-brown `accent` fill + cream `accent-contrast` text (Start, Run tests; at most one per panel). Secondary = `surface-raised` + `border-strong` outline (Add, Step back). Ghost = no border, `text-muted`, `accent-soft` on hover (Remove, Reset, Complete with trap state). Disabled = `text-subtle` on a transparent fill with a `border` outline.
- **Icon buttons:** 28×28 with a 16 px stroke icon (undo / redo). Each keeps its tooltip and `AutomationProperties.Name`.
- **Inputs:** 28 px, radius 6, `border-strong`. Error state: a 2 px `danger` border, and below it ✗ plus the OpResult message in `danger` 12 px.
- **Tabs:** text tabs, 13 / 500, `text-muted`. The active tab is in `text` with a 2 px `accent` underline. They are no longer the large Fluent headers.
- **Verdict line** (one shared control, used in Simulator, Tests and RoPlate): ✓ / ✗ 20 px + **ACCEPTED** / **REJECTED** bold + message, on the verdict fill with a 1 px status border.
- **Tables** (one shared style for δ, execution and tests): 12 px, header row in `text-muted` 600 with a `border-strong` rule, rows separated by `border`. Symbol columns are mono. The current row gets an `accent-soft` fill, bold text and a 3 px `accent` left bar. Missing cells get the `missing` fill plus "—".
- **Alphabet symbols:** mono chips with a × remove button. They replace the "a [Remove]" text buttons.
- **Speed slider:** labeled "Speed" with its value ("600 ms", mono) beside it.
- **Empty states:** on the canvas, a dashed ghost state, a "Start with a state" title, the hint "Double-click anywhere to add a state. Shift + drag from one state to another to add a transition." and an "Open an example" button. The Tests tab gets a similar one-line hint.
- **Status bar:** `surface`, `text-muted` 12 px, with a `border` line on top.

## Signature

The canvas is drawn on **aged dot-grid paper**: `border`-colored dots, 1.2 px, every 18 px, on cream `surface`. A faint vignette darkens the canvas edges like an old page: an inset umber glow at 10% in light mode, a black glow at 35% in dark mode. In Avalonia that is a radial-gradient overlay, or skip it if it costs too much. Section titles are set in **Fraunces serif**. Together they make RoPlate look like an old lecture notebook without a logo. The dots and vignette are drawn on screen only. The PNG export stays plain black on white.

## Keep list, applied

- Drawing conventions unchanged: radius 28 / final 22, "start" arrow, 4 px current outline (`Brush.StateCurrent`), accepted / rejected 4 px outline with ✓ / ✗ beside the state, 3 px last edge, muted description under the state. Strokes use `text`, so they read on both papers.
- Compact density: 28 px controls, 4–8 px gaps. The Simulate tab still fits at 1100×700.
- Not color alone: ✓ / ✗ marks, thick outlines, the "—" in missing cells, the left bar and bold text on the current row, the 2 px error border plus ✗.

## Rejected directions (feeds "never look like")

| Direction | What it was | Why it lost / what to avoid |
|---|---|---|
| B · Notebook, original ink-blue palette | near-white paper (#FAF8F3), ink-blue accent | The user wanted it to feel older and warmer. Avoid near-white surfaces and blue accents in the paper theme. |
| A · Chalk & Slate | cool slate, graph-paper grid, borders only, steel blue | Competent but impersonal. It reads like a generic developer tool rather than a teaching tool. Avoid cold blue-gray neutrals and a hard ruled grid. |
| C · Lab Bench | mint surfaces, soft shadows, pill buttons, amber accent, springy motion | Too playful and app-like for a study tool. Shadows and floating cards cost density. Avoid pill buttons, drop shadows and spring/overshoot motion. |
| D · Blueprint | true gray, sharp corners, black primary buttons, uppercase labels | Too stark and loud for "clear & calm". Avoid pure-gray or pure-black surfaces, black primary buttons, uppercase letter-spaced headings and heavy 2 px dividers. |

From the audit: never fall back to the OS accent, pure #000 backgrounds, or stock Fluent large tab headers.

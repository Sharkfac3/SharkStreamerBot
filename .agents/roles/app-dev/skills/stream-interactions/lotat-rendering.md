# LotAT Rendering — Visual Layer

## What Was Built

A complete visual layer for Legends of the ASCII Temple sessions.  When
Streamer.bot publishes `lotat.*` broker messages, the overlay renders the
session state on screen using Phaser text and graphics primitives — no image
assets required.

---

## Design Decisions

### Aesthetic
**Retro terminal.**  Dark semi-transparent panels, `Courier New` monospace
font, green (`#00FF88`) primary text, gold (`#FFD700`) accents, cyan (`#00CCFF`)
for chat commands.  Chosen because:
- Zero image assets needed for v1
- Matches "ASCII Temple" naming
- Readable over any stream content
- Easily restyled later (all values in `lotat-constants.ts`)

### Screen Layout
**Lower third** — LotAT UI occupies y ≥ 670.  Stream content is fully visible
above.  Standard broadcast approach.

```
y=0    ┌──────────────────────── 1920 ──────────────────────────┐
       │                                                          │
       │               [stream content visible]                   │
       │                                                          │
y=670  ├──────────────────────────────────────────────────────────┤  base panel
y=678  │ [▸ LOCATION]  [CREW: name]  CHAOS [████░░]  [timer]    │  info strip
y=718  ├──────────────────────────────────────────────────────────┤
       │  narration text / dice display / commander panel         │  content zone
y=862  ├──────────────────────────────────────────────────────────┤
       │  [!cmd1  label  VOTES:N]  [!cmd2  …]  [!cmd3  …]       │  choice cards
y=972  └──────────────────────────────────────────────────────────┘
```

Join panel overlays the lower zone when the join phase is active.
EndingScreen overlays the full centre of the canvas on session end.

---

## File Map

```
Apps/stream-overlay/packages/overlay/src/
  components/lotat/
    lotat-constants.ts     — all layout / colour / font constants
    CountdownTimer.ts      — reusable countdown (used by Join, Choices, Dice, Commander)
    LocationIndicator.ts   — "▸ COMMAND DECK" in the info strip
    CrewFocus.ts           — "CREW: The Water Wizard" in the info strip
    ChaosMeter.ts          — CHAOS label + fill bar + value in info strip
    NarrationBox.ts        — dark panel + word-wrapped read_aloud text
    JoinPanel.ts           — centred join banner with player list + countdown
    ChoiceCards.ts         — dynamic choice cards with live vote tallies
    DiceDisplay.ts         — dice roll window with scrolling roll results
    CommanderMoment.ts     — commander prompt + result panel
    EndingScreen.ts        — full ending display (success / partial / failure)
  systems/
    lotat-renderer.ts      — orchestrator; receives all lotat.* payloads and
                             routes to component show/hide/update calls
  scenes/
    OverlayScene.ts        — wired: routes lotat.* broker messages to lotat-renderer

Actions/LotAT/
  overlay-publish.cs       — reference template; copy PublishLotat* methods
                             into the appropriate engine scripts

Apps/stream-overlay/packages/broker/src/
  lotat-test-session.ts    — standalone test that simulates a full mini session
```

---

## Message → Visual State Map

| Broker Message | Visual Action |
|---|---|
| `lotat.session.start` | Show base lower-third panel |
| `lotat.join.open` | Show JoinPanel with countdown |
| `lotat.join.update` | Update player count and names in JoinPanel |
| `lotat.join.close` | Hide JoinPanel |
| `lotat.node.enter` | Update LocationIndicator, CrewFocus, NarrationBox; hide mechanic panels |
| `lotat.chaos.update` | Show/update ChaosMeter |
| `lotat.commander.open` | Hide NarrationBox; show CommanderMoment with countdown |
| `lotat.commander.close` | Show result in CommanderMoment; auto-hide after 2.5s |
| `lotat.dice.open` | Hide NarrationBox; show DiceDisplay with countdown |
| `lotat.dice.roll` | Append roll result to DiceDisplay |
| `lotat.dice.close` | Show outcome in DiceDisplay; transition to NarrationBox after 3s |
| `lotat.vote.open` | Show ChoiceCards with countdown |
| `lotat.vote.cast` | Update vote tallies on ChoiceCards |
| `lotat.vote.close` | Highlight winner on ChoiceCards; show result_flavor in NarrationBox |
| `lotat.session.end` | Hide all panels; show EndingScreen; auto-hide all after 8s |

---

## Component Status

| Component | Status | Notes |
|---|---|---|
| JoinPanel | ✅ Full | Banner, player count, countdown, last 5 names |
| NarrationBox | ✅ Full | Word-wrapped text, optional colour override |
| LocationIndicator | ✅ Full | Ship section label in strip |
| CrewFocus | ✅ Full | Commander or squad member name in strip |
| ChaosMeter | ✅ Full | Fill bar colour-shifts green→yellow→red at 20 cap |
| CountdownTimer | ✅ Full | M:SS display, turns red under 30s |
| ChoiceCards | ✅ Full | Dynamic (1–4 cards), vote tallies, winner highlight |
| DiceDisplay | ✅ Full | Purpose + threshold + scrolling roll results + outcome |
| CommanderMoment | ✅ Full | Commander name, prompt, result text |
| EndingScreen | ✅ Full | success/partial/failure/zero-join/zero-votes/fault-abort |
| CrewPortrait | ❌ Deferred | No art assets yet; text-only in v1 |
| ShipBackground | ❌ Deferred | No art assets yet; text label only |
| SfxHint audio | ❌ Deferred | Hint is logged to console; no MP3 playback yet |

---

## Depth Conventions

All LotAT elements use depth 20–26 (see `asset-system.md`):

| Depth | Element |
|---|---|
| 20 | Base dark lower-third panel |
| 21 | Info strip (location, crew, chaos, timer) |
| 22 | Narration box, choice cards |
| 24 | Join panel, dice display, commander moment |
| 26 | Ending screen (topmost) |

---

## Chaos Bar

- Visual cap: 20 (constant `CHAOS_MAX_VISUAL` in `lotat-constants.ts`)
- 0–33%: green fill
- 34–66%: yellow fill
- 67–100%: red fill
- Value text colour shifts to match

Adjust `CHAOS_MAX_VISUAL` if story chaos totals exceed 20.

---

## Transitions

- **Show**: immediate `setVisible(true)` — clean, no tween complexity
- **Hide**: immediate `setVisible(false)` — snappy
- **Session end auto-hide**: 8-second delay then full LotAT UI cleared
- **CommanderMoment result**: 2.5-second hold then hide
- **DiceDisplay outcome**: 3-second hold then transition to NarrationBox

All delays use `scene.time.delayedCall()` — Phaser-safe, pauses with the game.

---

## Testing

Run the simulated session:

```bash
# Terminal 1
cd Apps/stream-overlay && pnpm dev:broker

# Terminal 2
pnpm dev:overlay
# Open http://localhost:5173?debug=true

# Terminal 3
cd packages/broker
npx ts-node --esm src/lotat-test-session.ts
```

Expected output: all 11 steps render visually as described in the script's
JSDoc header.

---

## Asset Requirements (Future)

When art is ready, these slots can be filled without structural changes:

| Asset | Path | Used by |
|---|---|---|
| Character portraits | `public/assets/lotat/<name>.png` | CrewFocus (upgrade) |
| Ship section images | `public/assets/lotat/sections/<name>.jpg` | LocationIndicator bg |
| SFX audio files | `public/assets/audio/lotat/<hint>.mp3` | sfxHint playback |

Use `overlay.spawn` to layer portrait/background images under the text panels,
or upgrade the components to load them directly via Phaser's asset loader.

---

## Publishing Integration

`Actions/LotAT/overlay-publish.cs` contains all `PublishLotat*()` methods.

**Copy pattern** — in each engine script:
1. Copy the CONSTANTS BLOCK
2. Copy `PublishBrokerMessage` from `Actions/Overlay/broker-publish.cs`
3. Copy only the `PublishLotat*` methods that script needs

**Integration map** (which engine script calls which method):

| Engine script | Publish method to add |
|---|---|
| `lotat-start-main.cs` | `PublishLotatSessionStart`, `PublishLotatJoinOpen` |
| `lotat-join.cs` | `PublishLotatJoinUpdate` |
| `lotat-join-timeout.cs` | `PublishLotatJoinClose` |
| `lotat-node-enter.cs` | `PublishLotatNodeEnter`, `PublishLotatChaosUpdate`, and optionally `PublishLotatCommanderOpen` / `PublishLotatDiceOpen` |
| `lotat-commander-input.cs` | `PublishLotatCommanderClose` (outcome: success) |
| `lotat-commander-timeout.cs` | `PublishLotatCommanderClose` (outcome: skipped) |
| `lotat-dice-roll.cs` | `PublishLotatDiceRoll`, `PublishLotatDiceClose` (on success) |
| `lotat-dice-timeout.cs` | `PublishLotatDiceClose` (outcome: failure) |
| `lotat-decision-input.cs` | `PublishLotatVoteCast`; also add `PublishLotatVoteOpen` call when window opens |
| `lotat-decision-resolve.cs` | `PublishLotatVoteClose` |
| `lotat-decision-timeout.cs` | `PublishLotatVoteClose` (null winner) |
| `lotat-end-session.cs` | `PublishLotatSessionEnd` |

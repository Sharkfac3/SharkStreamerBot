# Squad Rendering — Visual Layer

## What Was Built

A complete visual rendering system for all four squad mini-games.  When
Streamer.bot publishes `squad.*` broker messages, the overlay renders each
game's UI using Phaser text and graphics primitives — same retro terminal
aesthetic as LotAT.

---

## File Map

```
Apps/stream-overlay/packages/overlay/src/
  components/squad/
    squad-constants.ts    — all layout / colour / font constants
    GameBanner.ts         — game title header (reused by all games)
    GameTimer.ts          — countdown timer (reused by Duck, Pedro)
    ResultDisplay.ts      — win/lose result text with auto-hide
    duck-renderer.ts      — Duck game visual (full)
    pedro-renderer.ts     — Pedro game visual (full)
    clone-renderer.ts     — Clone game visual (full)
    toothless-renderer.ts — Toothless rarity pop-up (full)
  systems/
    squad-renderer.ts     — orchestrator; routes squad.* broker messages
                            to per-game renderer start/update/end calls
  scenes/
    OverlayScene.ts       — wired: routes squad.* broker messages to squad-renderer

Actions/Squad/
  Duck/overlay-publish.cs      — copy PublishDuck* methods into duck scripts
  Pedro/overlay-publish.cs     — copy PublishPedro* methods into pedro scripts
  Clone/overlay-publish.cs     — copy PublishClone* methods into clone scripts
  Toothless/overlay-publish.cs — copy PublishToothless* methods into toothless-main.cs

Apps/stream-overlay/packages/broker/src/
  squad-test-session.ts  — runs all four game scenarios via the broker test client
```

---

## Implementation Status

| Game      | Status | Notes |
|-----------|--------|-------|
| Duck      | ✅ Full | Counter + progress bar + timer + win/fail result |
| Pedro     | ✅ Full | Same structure as Duck, hot pink accent |
| Clone     | ✅ Full | 32×18 grid, player/empire tracking, grid-based collision |
| Toothless | ✅ Full | Upper-right pop-up, rarity per-colour, NEW UNLOCK label |

---

## Screen Layout

Squad games occupy the **top of the screen** (y < 250), entirely above LotAT's
lower-third zone (y ≥ 670).  They can coexist with LotAT visually.

```
y=0    ┌──────────────────────── 1920 ──────────────────────────┐
       │  [SQUAD PANEL — top left]         [TOOTHLESS — top right]│  y=0..~230
y=250  ├──────────────────────────────────────────────────────────┤
       │                                                          │
       │               [stream content visible]                   │
       │                                                          │
y=670  ├──────────────────────────────────────────────────────────┤  LotAT starts here
       │  [LOTAT LOWER THIRD]                                     │
y=1080 └──────────────────────────────────────────────────────────┘
```

Duck, Pedro, and Clone share the top-left panel position (only one runs at a
time per the Streamer.bot mini-game lock).  Toothless uses the upper-right
corner and can technically overlap with them, but the Streamer.bot lock
prevents simultaneous games.

---

## Aesthetic

| Property | Value |
|----------|-------|
| Font | `Courier New` (same as LotAT) |
| Panel | Dark `#000000` semi-transparent (α 0.88–0.93) |
| Depth range | 30–39 (above LotAT 20–29) |
| Duck accent | Cyan `#00CCFF` |
| Pedro accent | Hot pink `#FF69B4` |
| Clone accent | Gold `#FFD700` |
| Toothless accent | Purple `#CC88FF` |

Toothless rarity colours:
| Rarity | Colour |
|--------|--------|
| regular | `#AAAAAA` grey |
| smol | `#44AAFF` blue |
| long | `#44FF88` green |
| flight | `#FFD700` gold |
| party | `#FF44FF` party pink |

---

## Broker Messages Published

### Duck (`squad.duck.*`)

| Topic | Publisher script | Payload |
|-------|-----------------|---------|
| `squad.duck.start` | duck-main.cs | `{ game, triggeredBy }` |
| `squad.duck.update` | duck-call.cs | `{ game, state: { quackCount, uniqueQuackerCount } }` |
| `squad.duck.end` | duck-call.cs (success) / duck-resolve.cs (timeout) | `{ game, result: { success, finalQuackCount, uniqueQuackerCount } }` |

### Pedro (`squad.pedro.*`)

| Topic | Publisher script | Payload |
|-------|-----------------|---------|
| `squad.pedro.start` | pedro-main.cs | `{ game, triggeredBy }` |
| `squad.pedro.update` | pedro-call.cs | `{ game, state: { mentionCount } }` |
| `squad.pedro.end` | pedro-resolve.cs | `{ game, result: { success, finalMentionCount } }` |

### Clone (`squad.clone.*`)

| Topic | Publisher script | Payload |
|-------|-----------------|---------|
| `squad.clone.start` | clone-main.cs | `{ game, triggeredBy, phase: "join", joinWindowSeconds, players, gridCols, gridRows }` |
| `squad.clone.update` | clone-volley.cs | `{ game, state: { event, players, empire, gridCols, gridRows, elapsedSeconds, survivorCount, empireCount, eventDetail? } }` |
| `squad.clone.end` | clone-volley.cs (win/loss paths) | `{ game, result: { outcome: "win"|"loss", survivors: CloneGridPlayer[] } }` |

### Toothless (`squad.toothless.*`)

| Topic | Publisher script | Payload |
|-------|-----------------|---------|
| `squad.toothless.start` | toothless-main.cs | `{ game, triggeredBy }` |
| `squad.toothless.end` | toothless-main.cs | `{ game, result: { rarity, username, isFirstUnlock } }` |

No `squad.toothless.update` is used — the roll is instantaneous.

---

## Streamer.bot Integration

Each `overlay-publish.cs` is a **reference template**, not a deployed action.
Copy the publish methods into the existing game scripts as described in each
file's header.  No existing game logic is modified.

---

## Asset Requirements (v1)

No image assets required.  All visuals use Phaser primitive shapes and text,
identical to the LotAT approach.

Future art slots (when ready):
- `public/assets/squad/duck.png` — duck mascot image
- `public/assets/squad/pedro.gif` — pedro animated
- `public/assets/squad/toothless-{rarity}.png` — per-rarity toothless art
- `public/assets/audio/squad/duck-win.mp3` — victory sounds

---

## Testing

```bash
# Terminal 1 — broker
cd Apps/stream-overlay && pnpm dev:broker

# Terminal 2 — overlay
pnpm dev:overlay
# Open http://localhost:5173?debug=true

# Terminal 3 — test script
cd packages/broker
npx ts-node --esm src/squad-test-session.ts
```

The test runs all four games in sequence with realistic payloads.
See the test script's JSDoc header for the expected visual at each step.

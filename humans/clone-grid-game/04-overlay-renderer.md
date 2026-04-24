# Prompt 04 — Clone Grid Game: Overlay Renderer

## Context

You are working on SharkStreamerBot. Read `.agents/ENTRY.md` before starting. Activate the **app-dev** role (`roles/app-dev/`). Read `roles/app-dev/skills/core.md`.

Check `WORKING.md` before touching any files. Add your row to Active Work.

**Prerequisites:** Prompts 01–03 must be complete.

Read these files before writing any code:
- `Apps/stream-overlay/packages/shared/src/protocol.ts` — new Clone Grid types (added in P01)
- `Apps/stream-overlay/packages/shared/src/topics.ts` — topic strings
- `Apps/stream-overlay/packages/overlay/src/components/squad/squad-constants.ts` — color/font palette
- `Apps/stream-overlay/packages/overlay/src/systems/squad-renderer.ts` — how clone is currently wired
- `Apps/stream-overlay/packages/overlay/src/scenes/OverlayScene.ts` — broker message routing
- `Apps/stream-overlay/packages/overlay/src/components/squad/clone-renderer.ts` — the old renderer you are replacing

---

## What You Are Building

A full **rewrite** of `clone-renderer.ts` to render the Star Wars-themed grid survival game. The old renderer displayed a 5-box musical-chairs UI. The new renderer displays a full-screen grid, player avatars, and empire cells.

You will also update `squad-renderer.ts` to wire the new payload types, and verify `OverlayScene.ts` routes correctly.

---

## Grid Specification

| Property | Value |
|---|---|
| Canvas | 1920 × 1080 px |
| Grid | 32 cols × 18 rows |
| Cell size | 52 × 52 px |
| Grid origin (top-left of cell 1,1) | x = 128, y = 72 |
| Total grid area | 1664 × 936 px |
| Horizontal margin | (1920 − 1664) / 2 = 128 px each side |
| Vertical margin | (1080 − 936) / 2 = 72 px each side |

Cell center for grid position (col, row) [1-indexed]:
```
cellCenterX = 128 + (col - 1) * 52 + 26
cellCenterY = 72  + (row - 1) * 52 + 26
```

---

## Visual Design

### Join Phase (triggered by `squad.clone.start`)

Display a **join panel** in the center of the screen while players are joining:
- Dark semi-transparent background rectangle (full screen: 1920 × 1080, alpha 0.7)
- Title text: `"⚔️ EMPIRE APPROACHES"` — large, gold color
- Subtitle: `"Type !join to join the Rebel defense!"`
- Player list (scrolling if needed): each joined player's userName
- Countdown text: `"Join window closes in Xs"` — tick down using `scene.time` each second
- Depth: use `SQUAD_LAYOUT.depthContent` or higher so it renders above other elements

The join panel counts down from `joinWindowSeconds` (60). Update every second via `scene.time.addEvent`.

### Movement Phase (triggered by `squad.clone.update` with event `"game_start"`)

Hide the join panel. Show the game grid:

**Grid background:**
- Full-screen dark rectangle (1920 × 1080, near-black, alpha 0.92)
- Grid lines: thin horizontal and vertical lines at each cell boundary, very dark (barely visible), hex `0x1a1a2e`

**Empty cells:** no fill beyond the dark background

**Empire cells:** filled rectangle, hex `0x8b0000` (dark red), alpha 0.9, with a small triangle symbol `"▲"` centered in the cell, color `#FF4444`

**Player cells:**
- Filled rectangle, hex `0x003300` (dark green), alpha 0.85
- Username label centered on cell (truncate to 9 chars + "…" if longer), color `#00FF88`, font `Courier New 10px`
- Small rebel indicator (e.g. `"◆"`) above username

**HUD (top strip, y = 8 to 64):**
- Left: survivor count `"REBELS: N"`, color `#00FF88`
- Center: game timer `"0:00 / 5:00"` counting up, color `#FFD700` (gold)
- Right: empire count `"EMPIRE: N"`, color `#FF4444`
- HUD background: dark strip, full width, height 64px, alpha 0.85

**Death/event flash messages:**
- When `event === "player_died"` or `"player_inactivity"`: brief red flash text `"💀 {eventDetail} ELIMINATED"` centered on screen, fades out over 2s
- When `event === "empire_spawned"`: brief gold flash `"⚡ Empire expands!"`, 1.5s fade
- When `event === "empire_killed"`: brief green flash `"✨ Empire cell destroyed!"`, 1.5s fade

### End Phase (triggered by `squad.clone.end`)

- Hide game grid.
- Display result card centered on screen:
  - Win: `"🏆 REBELS HOLD THE LINE!"` in gold, then list of survivors
  - Loss: `"☠️ THE EMPIRE PREVAILS"` in dark red
- Auto-hide after 8 seconds.

---

## Implementation Requirements

### Class: `CloneGridRenderer`

Replace the existing `CloneRenderer` class entirely. Export as `CloneRenderer` so `squad-renderer.ts` import line does not break.

```typescript
export class CloneRenderer {
  // New grid-based implementation
}
```

Public API (same method names, new signatures):
```typescript
onStart(payload: SquadCloneGridStartPayload): void;
onUpdate(state: SquadCloneGridUpdateState): void;
onEnd(result: SquadCloneGridEndResult): void;
destroy(): void;
```

### State the renderer must track

```typescript
private players: CloneGridPlayer[] = [];
private empire:  CloneGridCell[]   = [];
private elapsedSeconds = 0;
private gamePhase: 'idle' | 'join' | 'game' | 'ended' = 'idle';
```

### Cell rendering strategy

Use a `Map<string, Phaser.GameObjects.Rectangle>` keyed by `"col,row"` for empire cells.
Use a `Map<string, { rect: Phaser.GameObjects.Rectangle; label: Phaser.GameObjects.Text }>` keyed by `userId` for players.

On each `onUpdate`:
1. Diff the incoming `players` list against current player rects — add new, move existing, remove gone.
2. Diff the incoming `empire` list against current empire rects — add new, remove gone.
3. Update HUD counters and elapsed timer.

Do not destroy and recreate all cells on every update — mutate positions (`.setPosition()` or `.x`/`.y`) for smooth visuals.

### Timer display

In `onUpdate`, store `elapsedSeconds` and update a Phaser `GameObjects.Text` showing `M:SS / 5:00`.

Update the game timer independently via `scene.time.addEvent` every 1000ms (increment `elapsedSeconds` locally). On each `onUpdate`, sync `elapsedSeconds` from the payload to keep it accurate.

### Depth values

Use values in range 30–39 (squad layer), consistent with `squad-constants.ts`:
- Grid background: depth 30
- Grid lines: depth 31
- Empire/player cells: depth 32
- Labels: depth 33
- HUD: depth 34
- Flash messages: depth 35
- Join panel: depth 36
- End result card: depth 37

---

## Updates to `squad-renderer.ts`

Read the file. The `onCloneStart`, `onCloneUpdate`, `onCloneEnd` methods cast payloads to old types. Update the casts:

```typescript
import type {
  SquadCloneGridStartPayload,
  SquadCloneGridUpdateState,
  SquadCloneGridEndResult,
} from '@stream-overlay/shared';

onCloneStart(payload: SquadGameStartPayload): void {
  this.clone.onStart(payload as unknown as SquadCloneGridStartPayload);
}

onCloneUpdate(payload: SquadGameUpdatePayload): void {
  this.clone.onUpdate(payload.state as unknown as SquadCloneGridUpdateState);
}

onCloneEnd(payload: SquadGameEndPayload): void {
  this.clone.onEnd(payload.result as unknown as SquadCloneGridEndResult);
}
```

---

## Verify `OverlayScene.ts`

Read `OverlayScene.ts`. Confirm that:
- `squad.clone.start` routes to `squadRenderer.onCloneStart(payload)`
- `squad.clone.update` routes to `squadRenderer.onCloneUpdate(payload)`
- `squad.clone.end` routes to `squadRenderer.onCloneEnd(payload)`

No changes needed unless routing is missing or broken.

---

## Add Clone Grid Constants to `squad-constants.ts`

Add a new section at the bottom:

```typescript
// ── Clone Grid Game ────────────────────────────────────────────────────────
export const CLONE_GRID = {
  cols:       32,
  rows:       18,
  cellSize:   52,
  originX:    128,   // pixel x of top-left corner of cell (1,1)
  originY:    72,    // pixel y of top-left corner of cell (1,1)
  hudHeight:  64,
} as const;
```

Use `CLONE_GRID` constants throughout `clone-renderer.ts` instead of magic numbers.

---

## Type Safety Notes

- `SquadCloneGridStartPayload`, `SquadCloneGridUpdateState`, `SquadCloneGridEndResult`, `CloneGridPlayer`, `CloneGridCell` are all defined in `@stream-overlay/shared` (added in Prompt 01).
- Import them directly: `import type { ... } from '@stream-overlay/shared';`
- The `onStart` parameter is `SquadCloneGridStartPayload` (not `SquadGameStartPayload`) because the join panel needs `players`, `gridCols`, `gridRows`, and `joinWindowSeconds`.

---

## After Writing

Run from `Apps/stream-overlay/`:
```
pnpm typecheck
```
Must pass with zero errors. Fix any type errors before completing this prompt.

---

## Wiring Required

No new Streamer.bot actions or timers needed from this prompt. The overlay is a subscriber — it receives broker messages from the scripts built in Prompts 02/03.

**Operator verification after build:**
1. Run `pnpm build` from `Apps/stream-overlay/`.
2. Reload the OBS browser source.
3. In Streamer.bot, run the `test-overlay` action to confirm broker connection is live.
4. Trigger `!clone` in chat to verify join panel appears.

---

## After Completing

1. Remove row from WORKING.md Active Work. Add to Recently Completed.
2. Load `ops/skills/change-summary/_index.md` and produce a paste-target summary.

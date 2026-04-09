# Asset System

## What It Is

The asset system is the rendering engine for the overlay. It lets any broker
client (Streamer.bot, future AI agents, test clients) spawn images and sounds
onto the overlay canvas, animate them, move them, and remove them — all through
broker messages. The LotAT and Squad rendering layers are both built on top of this system.

---

## Source Files

```
packages/overlay/src/systems/
  asset-manager.ts    — registry + lifecycle (spawn, move, animate, remove, clear)
  animation-system.ts — Phaser tween wrappers for all AnimationPresets
  audio-manager.ts    — MP3 load + play via Phaser WebAudio
packages/overlay/src/types/
  phaser3-rex-plugins.d.ts — kept for reference; not imported at runtime
```

---

## Asset Lifecycle

```
overlay.spawn
    │
    ├── src ends in .gif? → hidden <img> + CanvasTexture → Phaser.GameObjects.Image
    └── otherwise        → Phaser.GameObjects.Image
    │
    ├── Asset registered in AssetManager registry by assetId
    ├── enterAnimation tween starts (if not "none")
    └── lifetime timer starts (if lifetime > 0)
                │
                ├── overlay.move    → moveTo() tween (interrupts current tween)
                ├── overlay.animate → apply() preset (interrupts current tween)
                │
                └── Removal trigger (one of):
                        ├── overlay.remove
                        ├── overlay.clear
                        └── lifetime expires (TTL)
                                │
                                ├── exitAnimation tween starts (if not "none")
                                └── onComplete → gameObject.destroy()
```

**Spawn = replace:** Sending `overlay.spawn` with an existing `assetId` destroys
the current asset immediately and spawns fresh. Safe to call repeatedly for
persistent HUD elements — no need to remove first.

---

## Asset ID Convention

| Asset type | Format | Example |
|---|---|---|
| Ephemeral (one-shot alert) | UUID v4 | `"3f2a1b0c-..."` |
| Persistent / named HUD | kebab-case slug | `"chaos-meter"`, `"lotat-hud"` |

Named IDs are the primary pattern for agents building persistent UI: spawn once
with a slug, then move/animate/respawn by slug — never track UUIDs.

---

## Asset Folder Convention

All local assets live under `packages/overlay/public/assets/`. Paths in broker
messages are relative to `public/`:

```
public/assets/
  alerts/   ← stream event assets (subs, raids, follows, bits)
  lotat/    ← LotAT character art, scene images, UI pieces
  squad/    ← squad mini-game images
  audio/    ← all MP3 files
```

**Example `src` values:**
```
"assets/alerts/follow.gif"
"assets/lotat/archivist.png"
"assets/squad/pedro.gif"
"assets/audio/sub-alert.mp3"
```

External URLs also work: `"https://example.com/image.png"` — but local is
preferred for reliability during a live stream.

---

## GIF Handling

GIFs use a hidden `<img>` element approach — `phaser3-rex-plugins` is not used at runtime:

1. `spawnGif()` creates a hidden off-screen `<img src="...">` and waits for its `load` event.
2. A `Phaser.Textures.CanvasTexture` is created with the GIF's natural dimensions.
3. A standard `Phaser.GameObjects.Image` is spawned using that canvas texture key — registered
   in the AssetManager like any other asset.
4. `OverlayScene.update()` (runs every frame) calls `assetManager.updateGifTextures()`, which
   draws the current GIF frame into each canvas texture via `drawImage()` + `refresh()`.
   The browser handles GIF frame timing natively through the `<img>` element.

This means:
- **All Phaser tweens work on GIFs** — move, fade, shake, spin, etc. use the same code path
  as static images.
- The GIF loops automatically as long as the `<img>` is in the DOM.
- No special cases in AssetManager: GIF and static image assets share the same lifecycle.

Agents do not need to treat GIFs differently from PNGs. Send the same
`overlay.spawn` / `overlay.move` / `overlay.remove` messages for both.

---

## Animation Presets

All presets are defined in `AnimationPreset` in `@stream-overlay/shared/protocol.ts`.
Every preset has a real implementation in `AnimationSystem`.

| Preset | Effect | Enter-only? |
|---|---|---|
| `"none"` | No animation | No |
| `"fade-in"` | Alpha 0 → 1 | No (but typical as enter) |
| `"fade-out"` | Alpha 1 → 0 | No (typical as exit) |
| `"slide-in-left"` | Enter from left edge | **Yes** — use as `enterAnimation` only |
| `"slide-in-right"` | Enter from right edge | **Yes** |
| `"slide-in-top"` | Enter from top edge | **Yes** |
| `"slide-in-bottom"` | Enter from bottom edge | **Yes** |
| `"bounce"` | Pop up 30px and back (2 cycles) | No |
| `"shake"` | Rapid left-right oscillation | No |
| `"pulse"` | Scale to 115% and back | No |
| `"spin"` | Full 360° rotation | No |

**Enter-only note:** The four `slide-in-*` presets require an offscreen start
position. They are correctly applied via `enterAnimation` on `overlay.spawn`.
Sending them via `overlay.animate` logs a warning and does nothing — the asset
would teleport offscreen otherwise.

**Mid-animation interruption:** Sending a new `overlay.move` or `overlay.animate`
while a tween is in progress kills the current tween and starts the new one from
the asset's current position. Movement redirects smoothly.

**Easing:** `moveTo` and `slide-in-*` use `Power2.easeOut`. `bounce` uses
Phaser's yoyo tween. All others use `Linear` or `Sine.easeInOut` as appropriate.

---

## Audio Manager

Handles `overlay.audio.play` and `overlay.audio.stop`.

```typescript
// Play a sound
{
  soundId: string;     // UUID or slug — used to stop it later
  src: string;         // path from public/ or full URL
  volume?: number;     // 0–1, default 1
  loop?: boolean;      // default false
}

// Stop a sound
{
  soundId?: string;    // omit to stop ALL sounds
}
```

- Audio is loaded on-demand and cached by Phaser. Same file played twice
  only fetches once.
- Spawning a sound with an existing `soundId` stops and replaces it.
- Non-looping sounds clean themselves out of the registry when they finish.
- Looping sounds persist until explicitly stopped.

**OBS requirement:** In the OBS browser source properties, check
**"Control audio via OBS"**. Without this, Web Audio does not route to the
stream capture output.

---

## Depth / Z-Order

The overlay canvas is 1920×1080. Depth (z-order) is controlled per-asset via
the `depth` field in `overlay.spawn`. Higher values render on top.

Suggested conventions (not enforced by code — use these as guidelines):

| Layer | Suggested depth range | Used by |
|---|---|---|
| Background imagery | 0–9 | Ambient scene art |
| Alert images | 10–19 | Stream event GIFs |
| LotAT HUD | 20–29 | LotAT session UI |
| Squad visuals | 30–39 | Squad game images |
| HUD / foreground | 90–99 | Always-on-top elements |

---

## Message Reference

### overlay.spawn

```json
{
  "topic": "overlay.spawn",
  "payload": {
    "assetId": "pedro-alert",
    "src": "assets/squad/pedro.gif",
    "position": { "x": 960, "y": 540 },
    "width": 300,
    "depth": 30,
    "enterAnimation": "slide-in-bottom",
    "enterDuration": 500,
    "lifetime": 5000,
    "exitAnimation": "fade-out",
    "exitDuration": 300
  }
}
```

### overlay.move

```json
{
  "topic": "overlay.move",
  "payload": {
    "assetId": "pedro-alert",
    "position": { "x": 200, "y": 900 },
    "duration": 1500
  }
}
```

`duration: 0` = instant snap. Omit for default 300ms tween.

### overlay.animate

```json
{
  "topic": "overlay.animate",
  "payload": {
    "assetId": "pedro-alert",
    "animation": "shake",
    "duration": 600
  }
}
```

### overlay.remove

```json
{
  "topic": "overlay.remove",
  "payload": {
    "assetId": "pedro-alert",
    "exitAnimation": "fade-out",
    "exitDuration": 400
  }
}
```

### overlay.clear

```json
{
  "topic": "overlay.clear",
  "payload": {
    "assetIdPrefix": "lotat-",
    "exitAnimation": "fade-out",
    "exitDuration": 300
  }
}
```

Omit `assetIdPrefix` to clear every asset on the canvas.

### overlay.audio.play

```json
{
  "topic": "overlay.audio.play",
  "payload": {
    "soundId": "sub-chime",
    "src": "assets/audio/sub-alert.mp3",
    "volume": 0.8
  }
}
```

### overlay.audio.stop

```json
{
  "topic": "overlay.audio.stop",
  "payload": { "soundId": "sub-chime" }
}
```

```json
{
  "topic": "overlay.audio.stop",
  "payload": {}
}
```

---

## Manual Test Procedure

Uses the broker test client (`packages/broker/src/test-client.ts`). Run this sequence to validate the
full asset system before using it in production.

### Setup

```bash
# Terminal 1 — broker
cd Apps/stream-overlay
pnpm dev:broker

# Terminal 2 — overlay
pnpm dev:overlay

# Browser: open http://localhost:5173?debug=true
# Confirm green dot (broker connected) before running tests
```

Drop test assets into the right folders first:
- `public/assets/alerts/test.jpg` — any JPG image
- `public/assets/alerts/test.gif` — any animated GIF
- `public/assets/audio/test.mp3` — any short MP3

Then send messages via the test client (`pnpm test-client` in the broker package).

---

### Test 1 — Spawn a JPG

```json
{
  "topic": "overlay.spawn",
  "payload": {
    "assetId": "test-jpg",
    "src": "assets/alerts/test.jpg",
    "position": { "x": 200, "y": 200 },
    "width": 200,
    "depth": 10,
    "enterAnimation": "fade-in",
    "enterDuration": 500
  }
}
```

**Expected:** Image fades in at (200, 200). Visible on overlay.

---

### Test 2 — Spawn a GIF

```json
{
  "topic": "overlay.spawn",
  "payload": {
    "assetId": "test-gif",
    "src": "assets/alerts/test.gif",
    "position": { "x": 600, "y": 200 },
    "width": 200,
    "depth": 10,
    "enterAnimation": "slide-in-top",
    "enterDuration": 600
  }
}
```

**Expected:** GIF slides in from top, animates continuously.

---

### Test 3 — Move the JPG

```json
{
  "topic": "overlay.move",
  "payload": {
    "assetId": "test-jpg",
    "position": { "x": 900, "y": 500 },
    "duration": 2000
  }
}
```

**Expected:** JPG smoothly tweens to (900, 500) over 2 seconds.

---

### Test 4 — Move the GIF (verifies GIF movement works identically)

```json
{
  "topic": "overlay.move",
  "payload": {
    "assetId": "test-gif",
    "position": { "x": 900, "y": 700 },
    "duration": 1500
  }
}
```

**Expected:** GIF smoothly tweens to (900, 700) while continuing to animate.

---

### Test 5 — Mid-move redirect

While Test 3 tween is still in progress, immediately send:

```json
{
  "topic": "overlay.move",
  "payload": {
    "assetId": "test-jpg",
    "position": { "x": 100, "y": 800 },
    "duration": 1000
  }
}
```

**Expected:** JPG smoothly redirects from its current in-transit position to (100, 800). No jump or snap.

---

### Test 6 — Animate (shake)

```json
{
  "topic": "overlay.animate",
  "payload": {
    "assetId": "test-gif",
    "animation": "shake",
    "duration": 800
  }
}
```

**Expected:** GIF shakes horizontally for ~800ms then stops.

---

### Test 7 — Fade out the JPG

```json
{
  "topic": "overlay.animate",
  "payload": {
    "assetId": "test-jpg",
    "animation": "fade-out",
    "duration": 600
  }
}
```

**Expected:** JPG fades out. Note: asset still exists in registry — `alpha` is
just 0. A subsequent `overlay.remove` or `overlay.clear` is needed to destroy it.

---

### Test 8 — Play audio

```json
{
  "topic": "overlay.audio.play",
  "payload": {
    "soundId": "test-sound",
    "src": "assets/audio/test.mp3",
    "volume": 0.8
  }
}
```

**Expected:** Sound plays. Console logs `[AudioManager] Playing "test-sound"`.
*(OBS: requires "Control audio via OBS" checked on the browser source.)*

---

### Test 9 — Remove the GIF

```json
{
  "topic": "overlay.remove",
  "payload": {
    "assetId": "test-gif",
    "exitAnimation": "fade-out",
    "exitDuration": 500
  }
}
```

**Expected:** GIF fades out and disappears. Gone from canvas.

---

### Test 10 — TTL auto-remove

```json
{
  "topic": "overlay.spawn",
  "payload": {
    "assetId": "ttl-test",
    "src": "assets/alerts/test.jpg",
    "position": { "x": 1600, "y": 100 },
    "width": 150,
    "depth": 10,
    "lifetime": 3000,
    "exitAnimation": "fade-out",
    "exitDuration": 500
  }
}
```

**Expected:** Image appears at (1600, 100). After 3 seconds it fades out and destroys itself automatically.

---

### Test 11 — Clear all

Spawn two or three assets, then:

```json
{
  "topic": "overlay.clear",
  "payload": { "exitAnimation": "fade-out", "exitDuration": 300 }
}
```

**Expected:** All assets fade out and disappear. Canvas empty.

---

### Test 12 — Invalid assetId (no crash)

```json
{
  "topic": "overlay.move",
  "payload": {
    "assetId": "does-not-exist",
    "position": { "x": 0, "y": 0 }
  }
}
```

**Expected:** Console logs `[AssetManager] overlay.move — unknown assetId "does-not-exist"`. No crash. Overlay continues running.

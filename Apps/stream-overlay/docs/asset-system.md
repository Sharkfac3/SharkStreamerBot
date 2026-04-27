---
id: apps-stream-overlay-asset-system
type: domain-reference
description: Asset registry, public asset paths, lifecycle commands, audio behavior, and depth conventions for Apps/stream-overlay.
status: active
owner: app-dev
---

# Stream Overlay Asset System

## Asset Roots and Paths

Local visual/audio files live under [../packages/overlay/public/assets/](../packages/overlay/public/assets/) and are referenced in payloads relative to the public root, such as "assets/squad/pedro.gif" or "assets/audio/sub-alert.mp3".

Suggested asset folders:

| Folder | Use |
|---|---|
| [../packages/overlay/public/assets/alerts/](../packages/overlay/public/assets/alerts/) | Stream event images and alert art. |
| [../packages/overlay/public/assets/lotat/](../packages/overlay/public/assets/lotat/) | LotAT character art, scene images, and UI pieces. |
| [../packages/overlay/public/assets/squad/](../packages/overlay/public/assets/squad/) | Squad mini-game images. |
| [../packages/overlay/public/assets/audio/](../packages/overlay/public/assets/audio/) | MP3 sound effects and alerts. |

No build step is needed when adding files under the overlay public directory; Vite serves them in dev and copies them to the generated dist directory during build.

## Lifecycle

1. `overlay.spawn` creates a PNG/JPG/GIF object, registers it by `assetId`, applies optional enter animation, and starts an optional lifetime timer.
2. `overlay.move` tweens an existing asset to a new position.
3. `overlay.animate` applies a supported animation preset.
4. `overlay.remove` destroys one asset after an optional exit animation.
5. `overlay.clear` destroys all assets or all assets whose IDs share a prefix.

## Asset IDs

- Use UUIDs for ephemeral one-shot alerts.
- Use kebab-case slugs for persistent/named HUD widgets.
- Spawning with an existing ID replaces the old object.

## GIFs and Audio

GIFs use hidden browser image elements plus Phaser canvas textures. Phaser tweens work on GIFs the same way they work on static images.

Audio messages use `overlay.audio.play` and `overlay.audio.stop`; non-looping sounds clean up after completion, and sounds with the same `soundId` are replaced.

If overlay audio is used, the OBS browser source must have **Control audio via OBS** enabled.

## Depth Conventions

Depth ranges are conventions, not code enforcement:

| Depth | Use |
|---:|---|
| 0-9 | Background imagery. |
| 10-19 | Stream alerts. |
| 20-29 | LotAT UI. |
| 30-39 | Squad visuals. |
| 90-99 | Foreground HUD. |

## Implementation Files

Read these before changing generic overlay asset commands:

- [../packages/overlay/src/systems/asset-manager.ts](../packages/overlay/src/systems/asset-manager.ts)
- [../packages/overlay/src/systems/animation-system.ts](../packages/overlay/src/systems/animation-system.ts)
- [../packages/overlay/src/systems/audio-manager.ts](../packages/overlay/src/systems/audio-manager.ts)

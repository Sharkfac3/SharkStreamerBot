# Drift Sweep 08 — Actions Docs

You are doing a drift sweep for the SharkStreamerBot project. Your job is to read every file listed below, find anything stale or incorrect, and fix it in place. Report each change.

`Actions/` contains Streamer.bot C# scripts. The docs here describe what each script does, its inputs, outputs, and dependencies. Drift here usually means: a script was added/renamed/deleted but its README wasn't updated, or constants referenced in docs don't match `SHARED-CONSTANTS.md`.

Do NOT add features, reorganize, or improve style. Only fix factual drift.

---

## Before you start

1. Read `WORKING.md` at repo root. Check for active file conflicts before editing anything.
2. Edit files in place with minimal diffs — change only what is factually wrong.
3. Do NOT run `git add` or `git commit`. The human operator commits all changes manually.

---

## Ground truth sources (read these first)

- `Actions/SHARED-CONSTANTS.md` — canonical global variable, OBS source, and timer names
- `Actions/HELPER-SNIPPETS.md` — canonical reusable C# patterns
- The actual `.cs` files in each folder — script names, trigger events, CPH calls
- `Apps/stream-overlay/packages/shared/src/topics.ts` — broker topic constants
- `.agents/_shared/mixitup-api.md` — Mix It Up API payload conventions
- `.agents/_shared/info-service-protocol.md` — info-service routes and port

---

## Files to sweep

### `Actions/SHARED-CONSTANTS.md`

This file IS a ground truth source, but it can still have drift. Drift to look for:
- Missing constants: scan the `Actions/` directory for global variable names used in `.cs` files that are NOT listed in SHARED-CONSTANTS.md (especially any new constants added for Intros, overlay, info-service)
- Stale constants: any constants listed that no longer appear in any script
- Missing info-service constants: `INFO_SERVICE_BASE_URL`, `INFO_SERVICE_PORT` (8766), `ASSETS_ROOT`, `SOUND_SUBPATH`, `GIF_SUBPATH` — check if these are listed
- Missing overlay constants: broker URL, broker port (8765)

### `Actions/HELPER-SNIPPETS.md`

Drift to look for:
- Missing patterns: any new reusable patterns established in `.cs` files but not documented here (especially info-service HTTP GET pattern from `first-chat-intro.cs`, Mix It Up wait pattern)
- Stale patterns: any patterns that no longer match what's in actual scripts
- Mix It Up unlock wait: standard 31-second wait — cross-check value against actual script usage

### `Actions/Overlay/README.md`

Drift to look for:
- Script list: cross-check against actual files in `Actions/Overlay/` (broker-connect.cs, broker-disconnect.cs, broker-publish.cs template, test-overlay.cs — verify these exist)
- Broker URL/port: cross-check against `SHARED-CONSTANTS.md`
- Any "placeholder" language about overlay integration that is now complete

### `Actions/Squad/README.md`

Drift to look for:
- Game list: all four games (Duck, Pedro, Clone, Toothless) should be listed
- Mini-game lock contract: cross-check variable names (`minigame_active`, `minigame_name`) against `SHARED-CONSTANTS.md`
- Overlay integration notes: should mention that each game has overlay-publish methods

### `Actions/Squad/Duck/README.md`
### `Actions/Squad/Pedro/README.md`
### `Actions/Squad/Clone/README.md`
### `Actions/Squad/Toothless/README.md`

For each game README, drift to look for:
- Script file list: cross-check against actual `.cs` files in the folder
- Variable names: cross-check against `SHARED-CONSTANTS.md`
- Mix It Up command names or payload fields: cross-check against `mixitup-api.md`
- Overlay topic names: cross-check against `topics.ts`
- Trigger events: still match what's in the actual scripts?

### `Actions/LotAT/README.md`

Drift to look for:
- Script file list: cross-check against actual `.cs` files in `Actions/LotAT/`
- `overlay-publish.cs` should be listed as a reference template
- Variable names: cross-check against `SHARED-CONSTANTS.md`
- Overlay integration: cross-check topic names against `topics.ts`

### `Actions/Commanders/README.md`
### `Actions/Commanders/Captain Stretch/README.md`
### `Actions/Commanders/The Director/README.md`
### `Actions/Commanders/Water Wizard/README.md`

Drift to look for:
- Script file lists: cross-check against actual `.cs` files in each folder
- Variable names: cross-check against `SHARED-CONSTANTS.md`
- Mix It Up integration: cross-check payload shapes against `mixitup-api.md`

### `Actions/Twitch Core Integrations/README.md`

Drift to look for:
- Script list: cross-check against actual `.cs` files in `Actions/Twitch Core Integrations/`
- `stream-start.cs`: should mention mini-game lock reset (`minigame_active = false`)
- Any Intros-related startup logic that references info-service

### `Actions/Twitch Channel Points/README.md`

Drift to look for:
- Script list: cross-check against actual `.cs` files
- Should mention the Custom Intro redemption handler if `redeem-capture.cs` routes through this group; or note that Intros redemptions are handled in `Actions/Intros/`

### `Actions/Twitch Bits Integrations/README.md`
### `Actions/Twitch Hype Train/README.md`
### `Actions/Voice Commands/README.md`
### `Actions/Rest Focus Loop/README.md`

Drift to look for:
- Script lists: cross-check against actual `.cs` files in each folder
- Variable names: cross-check against `SHARED-CONSTANTS.md`

---

## Intros scripts (check if README exists)

Check if `Actions/Intros/` has a README. If not, note it but do NOT create one — that is a separate task.
If a README does exist, sweep it for:
- Script list accuracy (`first-chat-intro.cs`, `redeem-capture.cs`)
- Info-service URL and port (8766)
- Assets root path constant reference

---

## Output format

For each file:
- File path
- What was stale
- What you changed it to

If a file has no drift, say so.

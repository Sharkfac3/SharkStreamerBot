---
id: actions-xj-drivethrough
type: domain-route
description: XJ Drivethrough Streamer.bot overlay action, asset requirements, product-facing handoffs, and validation notes.
owner: streamerbot-dev
secondaryOwners:
  - product-dev
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# XJ Drivethrough — Agent Guide

## Purpose

This folder owns the XJ Drivethrough Streamer.bot action. For non-commanders, the action rolls a 1-100 chance gate and, on rolls above 20, drives a Jeep Cherokee XJ image across the overlay while playing a rev-limiter audio clip through the overlay audio system.

The feature is a Streamer.bot runtime bridge into the stream overlay. Successful non-commander chance rolls publish generic overlay spawn, move, audio-play, audio-stop, and remove messages through the broker. Failed non-commander chance rolls log and exit without touching the re-entry guard or broker.

When the active Water Wizard, Captain Stretch, or The Director types `!xj`, the action bypasses the non-commander chance gate and displays each matching commander role's one-third XJ overlay piece for 10 seconds. If one Twitch user currently holds multiple commander roles, their single `!xj` counts for and displays all of those roles. The first commander `!xj` in an idle window starts a 10-second triforce window; if all three active commander roles are seen before the window closes, the action increments the current-stream triforce count, updates the persisted triforce high score only when the current stream count beats it, and plays the XJ rev-limiter audio.

## When to Activate

Use this guide when editing or reviewing files under [Actions/XJ Drivethrough/](./):

- [xj-drivethrough-main.cs](xj-drivethrough-main.cs) — complete drivethrough sequence and broker publish helper.

Activate `product-dev` if the XJ visual becomes product-facing, is tied to real off-road product messaging, or needs technical/customer-facing documentation.

Activate `brand-steward` before changing public command wording, stream-facing names, humor tone, vehicle framing, alert copy, or any messaging viewers will see or hear.

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot trigger assumptions, global re-entry guard, broker-publish helper, paste readiness, and manual operator setup notes.

## Secondary Owners / Chain To

- `product-dev` — chain if the XJ effect becomes tied to product documentation, technical explanation, sponsorship/product positioning, or customer-facing content.
- `brand-steward` — chain for public copy, feature naming, voice/tone, and stream-visible messaging.
- `app-dev` — chain if overlay broker topics, rendering, audio handling, or public asset paths need app-side changes.
- `ops` — chain for validation, sync, and final handoff workflow.

## Required Reading

Before changing scripts, read:

- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) for XJ Drivethrough and Overlay / Broker constants.
- [Actions/HELPER-SNIPPETS.md](../HELPER-SNIPPETS.md) for reusable C# patterns.
- [Actions/Overlay/AGENTS.md](../Overlay/AGENTS.md) for broker publisher rules and overlay handoffs.
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) for route/ownership, [protocol](../../Apps/stream-overlay/docs/protocol.md) for broker changes, and [asset system](../../Apps/stream-overlay/docs/asset-system.md) for asset or audio behavior changes.
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) if public copy changes.

## Local Workflow

1. Preserve the single-action sequence unless the operator explicitly asks for a multi-action workflow.
2. Preserve the non-commander chance gate unless the operator explicitly asks to change trigger frequency. Current non-commander behavior rolls 1-100 inclusive and only runs on rolls greater than 20 (21-100).
3. Keep the non-commander re-entry guard reliable. `xj_drivethrough_active` must be cleared on every terminal path after the active slot is claimed; failed chance rolls should exit before claiming it.
4. Commander `!xj` handling must identify active commanders from the shared commander slot globals in [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md), not hard-coded usernames.
5. Keep broker constants and topic strings aligned with [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) and the overlay app contract.
6. Maintain cleanup behavior: audio stop and overlay remove should run after the drive finishes.
7. If changing asset ID, source path, dimensions, duration, audio ID, or chance constants, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) when in scope.
8. Keep scripts self-contained and paste-ready for Streamer.bot inline C#.
9. If the effect becomes product/content messaging instead of a pure gag/overlay effect, chain to `product-dev` and `brand-steward` before finalizing wording or documentation.

## Validation

For documentation-only changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

For script changes:

- Manually review C# for Streamer.bot inline compatibility.
- Confirm the overlay broker is running and connected before triggering the action.
- Confirm the XJ image asset exists in the overlay public images folder.
- Confirm the rev-limiter audio asset exists in the overlay public audio folder.
- Confirm the OBS browser source has audio enabled.
- Trigger [xj-drivethrough-main.cs](xj-drivethrough-main.cs) and verify failed rolls log a no-op without sending broker messages.
- Trigger until the chance gate passes, then verify the image drives fully across, audio starts, audio stops, and the asset is removed.
- Trigger the action twice quickly after a passed roll and verify the re-entry guard drops overlapping requests safely.

## Boundaries / Out of Scope

- Do not implement app-side overlay rendering or audio changes in this folder; use [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md), [rendering notes](../../Apps/stream-overlay/docs/rendering-notes.md), and [asset system](../../Apps/stream-overlay/docs/asset-system.md).
- Do not rename asset IDs, paths, or global variables without explicit operator approval.
- Do not introduce product-facing claims, sponsorship framing, or off-road product documentation without `product-dev` and `brand-steward` review.
- Do not move this route into Twitch, Squad, Overlay, or Product docs during this coverage-fill prompt.

## Handoff Notes

Use the terminal workflows after changes:

- [change-summary](../../.agents/workflows/change-summary.md)
- [sync](../../.agents/workflows/sync.md)
- [validation](../../.agents/workflows/validation.md)

For code changes, list [xj-drivethrough-main.cs](xj-drivethrough-main.cs) as the Streamer.bot paste target and note any trigger wiring, asset, audio, OBS, or overlay-broker setup the operator must verify.

Flag ownership ambiguity if the feature is no longer just an overlay gag and should be product/content-owned.

## Action Contracts

The following contract is the source of truth for the script behavior. Update this block before changing runtime behavior, then refresh the script stamp with `python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/XJ Drivethrough/xj-drivethrough-main.cs" --stamp`.

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "xj-drivethrough-main.cs",
      "action": "XJ Drivethrough / Main",
      "purpose": "Handle the !xj command: non-commanders roll a 1-100 chance gate for the existing drivethrough effect, while active commanders display their assigned one-third XJ overlay piece and can complete a 10-second triforce combo.",
      "triggers": [
        "Twitch -> Chat Command -> !xj",
        "Optional operator-wired manual trigger for testing"
      ],
      "globals": [
        "xj_drivethrough_active",
        "broker_connected",
        "current_water_wizard",
        "current_captain_stretch",
        "current_the_director",
        "xj_commander_triforce_active",
        "xj_commander_triforce_seen_json",
        "xj_commander_triforce_deadline_utc",
        "xj_commander_triforce_count",
        "xj_commander_triforce_high_score"
      ],
      "timers": [
        "XJ - Commander Triforce Window"
      ],
      "obsSources": [],
      "obsScenes": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [
        "overlay.spawn",
        "overlay.move",
        "overlay.audio.play",
        "overlay.audio.stop",
        "overlay.remove"
      ],
      "serviceUrls": [],
      "requiredLiterals": [
        "streamerbot",
        "xj-drivethrough",
        "images/xj-drivethrough.png",
        "images/xj-left.png",
        "images/xj-middle.png",
        "images/xj-right.png",
        "xj-left",
        "xj-center",
        "xj-right",
        "xj-rev-limiter",
        "audio/xj-rev-limiter.mp3",
        "XJ_CHANCE_MIN",
        "XJ_CHANCE_MAX_EXCLUSIVE",
        "XJ_TRIGGER_THRESHOLD",
        "WAIT_SPAWN_SETTLE_MS",
        "WAIT_POST_DRIVE_MS",
        "XJ_COMMANDER_DISPLAY_MS",
        "XJ_COMMANDER_TRIFORCE_WINDOW_MS"
      ],
      "runtimeBehavior": [
        "Read the triggering Twitch user from Streamer.bot command args and compare it case-insensitively against current_water_wizard, current_captain_stretch, and current_the_director; collect every commander role that matches the same user.",
        "If a user matches one or more active commander roles, handle all matching commander roles from that single !xj and bypass the non-commander chance gate.",
        "For non-commanders, roll 1-100 inclusive before claiming the re-entry guard and send chat a short message showing the user's roll and threshold.",
        "For non-commanders, exit with no broker messages when the roll is 20 or lower.",
        "For non-commanders, claim non-persisted xj_drivethrough_active only after the chance gate passes.",
        "For non-commanders, drop duplicate requests while xj_drivethrough_active is true.",
        "For non-commanders, spawn the XJ asset off-screen left with no enter animation.",
        "For non-commanders, wait briefly after spawn before publishing overlay.move so first-time asset loading can settle.",
        "For non-commanders, move the XJ asset fully across the 1920x1080 canvas over 10000ms.",
        "For non-commanders, start the rev-limiter audio immediately after the move command.",
        "For non-commanders, after the drive duration plus buffer, stop the audio and remove the XJ asset with no exit animation.",
        "For non-commanders, always release xj_drivethrough_active in a finally block after the active slot is claimed.",
        "For the active Water Wizard, spawn xj-left using images/xj-left.png in the left third of the overlay with lifetime 10000ms.",
        "For the active Captain Stretch, spawn xj-center using images/xj-middle.png in the center third of the overlay with lifetime 10000ms.",
        "For the active The Director, spawn xj-right using images/xj-right.png in the right third of the overlay with lifetime 10000ms.",
        "When the triggering user holds multiple active commander roles, spawn every matching commander piece from that one !xj, then return without waiting.",
        "Commander overlay pieces should use stable per-piece asset IDs so repeated commander commands refresh their own piece without moving the non-commander drivethrough asset.",
        "Commander overlay pieces must rely on the overlay.spawn lifetime field for cleanup rather than CPH.Wait or a delayed overlay.remove publish, so other commanders can issue !xj during the triforce window.",
        "Only commander !xj calls participate in triforce state; non-commander calls must not increment or otherwise affect xj_commander_triforce_count or xj_commander_triforce_high_score.",
        "Treat xj_commander_triforce_count as the non-persisted current-stream count and xj_commander_triforce_high_score as the persisted all-time high score.",
        "When the first unique commander role arrives and no triforce window is active, set non-persisted xj_commander_triforce_active true, store a seen map containing that commander role plus any other matching commander roles from the same !xj in xj_commander_triforce_seen_json, set xj_commander_triforce_deadline_utc to now plus 10000ms, and start the XJ - Commander Triforce Window timer.",
        "While the triforce window is active, count only the first !xj from each commander role and ignore duplicate !xj calls for commander roles already in xj_commander_triforce_seen_json.",
        "If all three commander roles are seen before the 10-second window closes, increment non-persisted xj_commander_triforce_count by one for the current stream, update persisted xj_commander_triforce_high_score only if the new current-stream count is higher than the stored high score, play xj-rev-limiter via overlay.audio.play, and clear xj_commander_triforce_active, xj_commander_triforce_seen_json, and xj_commander_triforce_deadline_utc.",
        "When the XJ - Commander Triforce Window timer fires without all three commanders seen, clear xj_commander_triforce_active, xj_commander_triforce_seen_json, and xj_commander_triforce_deadline_utc without changing xj_commander_triforce_count, xj_commander_triforce_high_score, or playing audio."
      ],
      "failureBehavior": [
        "If broker publish fails during non-commander spawn, log the error, return true, and rely on the finally block to release the guard.",
        "If broker publish fails during commander piece spawn, log the error, keep triforce state consistent, and return true for the handled live-stream failure.",
        "If the WebSocket is disconnected, attempt one reconnect, resend client.hello, and mark broker_connected false if reconnect fails.",
        "Return true for handled no-op or failure paths so Streamer.bot does not treat expected live-stream conditions as script crashes."
      ],
      "pasteTarget": "Streamer.bot action that runs the XJ Drivethrough main Execute C# Code sub-action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

## Runtime Notes

Expected asset locations in the overlay app:

| Asset | Expected overlay public path |
|---|---|
| Non-commander XJ image | images/xj-drivethrough.png |
| Water Wizard XJ piece | images/xj-left.png |
| Captain Stretch XJ piece | images/xj-middle.png |
| The Director XJ piece | images/xj-right.png |
| Rev-limiter audio | audio/xj-rev-limiter.mp3 |

The repo may not contain all binary assets yet. If missing, the C# action can publish successfully while the overlay cannot display or play the intended media.

Movement timing note: XJ is a spawn-then-immediate-move effect. Keep a publisher-side settle delay before `overlay.move` so first-time asset loading does not leave the Jeep off-screen. The overlay renderer has a pending-move safety net, but future moving-asset effects should still follow the same pattern documented in `Apps/stream-overlay/docs/asset-system.md`.

Core runtime values:

| Value | Meaning |
|---|---|
| `xj_drivethrough_active` | Non-persisted guard that prevents overlapping drivethrough sequences. |
| `XJ_CHANCE_MIN = 1` | Inclusive lower bound for chance rolls. |
| `XJ_CHANCE_MAX_EXCLUSIVE = 101` | Exclusive upper bound for chance rolls, producing 1-100. |
| `XJ_TRIGGER_THRESHOLD = 20` | Non-commander rolls must be greater than this value; 21-100 trigger the sequence. |
| `current_water_wizard` | Active Water Wizard username; commander `!xj` maps to the left XJ piece. |
| `current_captain_stretch` | Active Captain Stretch username; commander `!xj` maps to the middle XJ piece. |
| `current_the_director` | Active The Director username; commander `!xj` maps to the right XJ piece. |
| `xj_commander_triforce_active` | Non-persisted bool; true while the 10-second commander combo window is open. |
| `xj_commander_triforce_seen_json` | Non-persisted JSON state of commander roles already counted in the active window. |
| `xj_commander_triforce_deadline_utc` | Non-persisted Unix ms deadline for the active 10-second window. |
| `xj_commander_triforce_count` | Non-persisted current-stream triforce count incremented when all three commanders type `!xj` within the window. |
| `xj_commander_triforce_high_score` | Persisted all-time triforce high score; update only when `xj_commander_triforce_count` exceeds the stored value. |

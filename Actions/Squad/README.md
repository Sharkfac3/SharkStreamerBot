# Squad Script Reference

## Scope
This folder contains Squad mini-game and interaction scripts.

## Action Folder Docs
- `Clone/README.md`
- `Duck/README.md`
- `Pedro/README.md`
- `Toothless/README.md`

## Shared Constants
- Cross-script key/timer/OBS sync reference: `Actions/SHARED-CONSTANTS.md`

## Helper Snippets
- Reusable copy/paste patterns: `Actions/HELPER-SNIPPETS.md`
- Required mini-game contract: see root `README.md` > "Mini-game Contribution Contract (All Features)"

---

## Script: `offering.cs`

### Purpose
Handles offering tokens and applies boost changes, including optional LotAT steal behavior.

### Expected Trigger / Input
- Chat/user input that provides an offering token/member target.

### Required Runtime Variables
- Reads `lotat_active`.
- Reads/writes `lotat_announcement_sent` (one-time announcement latch).
- Reads `lotat_offering_steal_chance` (clamped 0..100).
- Reads `lotat_steal_multiplier` (minimum 1).
- Reads/writes `boost_<member>_<userId>` (clamped final value 0..30).

### Key Outputs / Side Effects
- Adjusts per-user per-member boost values.
- Applies LotAT steal modifier when active.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Wait Behavior
- None.

### Chat / Log Output
- Sends one-time LotAT active announcement.
- Sends unknown-token flavor message when token is invalid.
- Sends result message with delta and resulting boost.

### Operator Notes
- Keep token/member naming aligned with existing chat commands.
- Preserve boost key format (`boost_<member>_<userId>`) for compatibility.

---

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Chat Message (primary trigger for all Squad mini-games)

Squad games are triggered via Twitch → Chat → Message (or a Command trigger for `!` commands).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user who sent the message |
| `userId` | string | Twitch user ID — canonical player identifier in all Squad games |
| `message` | string | Full chat message text |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection (Pedro uses this) |
| `input0` | string | First word after the command trigger (if using a Command trigger) |

> `userId` is the preferred player key — stable even if a user changes their display name. Clone, Duck, Pedro, and Offering all key roster/state entries on `userId`.

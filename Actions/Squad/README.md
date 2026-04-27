# Squad Script Reference

## Scope
This folder contains Squad mini-game and interaction scripts.

## Action Folder Docs
- `Clone/README.md`
- `Duck/README.md`
- `Pedro/README.md`
- `Toothless/README.md`

## Overlay Integration
Duck, Pedro, and Toothless each have an `overlay-publish.cs` reference template in their folder. These are **not standalone deployed actions** — copy the `Publish*` methods into the target game scripts at integration time. See each game's README for the integration map.

## Shared Constants
- Cross-script key/timer/OBS sync reference: `Actions/SHARED-CONSTANTS.md`

## Helper Snippets
- Reusable copy/paste patterns: `Actions/HELPER-SNIPPETS.md` (compatibility index) and `Actions/Helpers/README.md` (concept-specific helper index)
- Required mini-game contract: `Actions/Helpers/mini-game-contract.md`

---

## Script: `squad-game-help.cs`

### Purpose
Handles the `!game` command. `!game` lists all available Squad mini-games in chat; `!game <name>` explains the rules of the named mini-game.

### Expected Trigger / Input
- Chat command wired to `!game`.
- Reads `user`, `input0` (first word after command).

### Required Runtime Variables
- None (read-only; no globals written).

### Key Outputs / Side Effects
- Sends one chat message: either the full game list or the rules for the named game.

### Mix It Up Actions
- None.

### OBS Interactions
- None.

### Operator Notes
- Wire to the `!game` chat command trigger.
- Add new games to the `helpMessages` dictionary inside the script as they are built.

---

## Script: `offering.cs`

### Purpose
Handles offering tokens and applies boost changes, including legacy / experimental LotAT-linked steal behavior.

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
- For current LotAT v1 planning, treat this script as **separate experimental offering work**, not as an approved LotAT runtime mechanic.
- Do not infer LotAT story/runtime contract rules from this script alone.

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

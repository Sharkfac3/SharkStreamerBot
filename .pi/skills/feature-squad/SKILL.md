---
name: feature-squad
description: Squad mini-game and chat interaction feature group. Covers Clone, Duck, Pedro, Toothless, and the shared offering system. Load when working on any script under Actions/Squad/.
---

# Feature: Squad

## Scope

Squad scripts handle chat-triggered mini-games and interaction behavior. Each action folder is a self-contained mini-game or interaction module.

## Action Folders

| Action Folder | Path | Scripts |
|---|---|---|
| Clone | `Actions/Squad/Clone/` | `clone-main.cs`, `clone-position.cs`, `clone-volley.cs` |
| Duck | `Actions/Squad/Duck/` | `duck-main.cs`, `duck-call.cs`, `duck-resolve.cs` |
| Pedro | `Actions/Squad/Pedro/` | `pedro-main.cs` |
| Toothless | `Actions/Squad/Toothless/` | `toothless-main.cs` |
| Offering | `Actions/Squad/` | `offering.cs` |

## Detailed Docs

Each action folder has its own README with full variable/trigger/OBS documentation:

- `Actions/Squad/Clone/README.md`
- `Actions/Squad/Duck/README.md`
- `Actions/Squad/Pedro/README.md`
- `Actions/Squad/Toothless/README.md`
- `Actions/Squad/README.md` (includes `offering.cs` docs)

**Read the relevant action-folder README before making changes.**

## Shared Constants

All Squad scripts share global variable names, OBS sources, and timer names documented in `Actions/SHARED-CONSTANTS.md`. Key groups:

- **Mini-game Lock** — `minigame_active`, `minigame_name`
- **Duck** — `duck_event_active`, `duck_quack_count`, `duck_caller`, `duck_unlocked`
- **Clone** — `clone_unlocked`, `clone_game_active`, `clone_round`, `clone_positions_*`, `clone_winners`
- **Pedro** — `pedro_game_enabled`, `pedro_mention_count`, `pedro_unlocked`
- **Toothless** — `rarity_*`, `last_roll`, `last_rarity`, `last_user`
- **LotAT / Offering** — `lotat_active`, `lotat_announcement_sent`, `lotat_offering_steal_chance`, `boost_*`

## Behavioral Expectations

- Handle spam/rapid triggers gracefully.
- Use deterministic resolution logic where possible.
- Protect against invalid or missing inputs.
- Preserve fairness and existing game feel unless explicitly adjusted.
- All mini-games must follow the mini-game lock contract (see `streamerbot-scripting` skill).

## Stream-Start Reset

`Actions/Twitch Core Integrations/stream-start.cs` resets all Squad state at stream start. Any new Squad global variable must be added to that reset list and to `Actions/SHARED-CONSTANTS.md`.

## Trigger Variables

Access in C# via `CPH.TryGetArg("variableName", out T value)`.

### Chat Message (primary trigger for all Squad mini-games)

Squad games are triggered via Twitch → Chat → Message (or a Command trigger for `!` commands).

| Variable | Type | Notes |
|---|---|---|
| `user` | string | Display name of the user who sent the message |
| `userId` | string | Twitch user ID — used as the canonical player identifier in all Squad games |
| `message` | string | Full chat message text |
| `rawInput` | string | Fallback if `message` is empty |
| `msgId` | string | Unique message ID — use for duplicate detection (Pedro uses this) |
| `input0` | string | First word after the command trigger (if using a Command trigger) |

> `userId` is the preferred player key — it is stable even if a user changes their display name. Clone, Duck, Pedro, and Offering all key roster/state entries on `userId`.

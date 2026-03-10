---
name: feature-squad
description: Squad mini-game and chat interaction feature group. Covers Clone, Duck, Pedro, Toothless, and the shared offering system. Load when working on any script under Actions/Squad/.
---

# Feature: Squad

## Scope

Squad scripts handle chat-triggered mini-games and interaction behavior. Each subfeature is a self-contained mini-game or interaction module.

## Subfeatures

| Subfeature | Path | Scripts |
|---|---|---|
| Clone | `Actions/Squad/Clone/` | `clone-main.cs`, `clone-position.cs`, `clone-volley.cs` |
| Duck | `Actions/Squad/Duck/` | `duck-main.cs`, `duck-call.cs`, `duck-resolve.cs` |
| Pedro | `Actions/Squad/Pedro/` | `pedro-main.cs` |
| Toothless | `Actions/Squad/Toothless/` | `toothless-main.cs` |
| Offering | `Actions/Squad/` | `offering.cs` |

## Detailed Docs

Each subfeature has its own README with full variable/trigger/OBS documentation:

- `Actions/Squad/Clone/README.md`
- `Actions/Squad/Duck/README.md`
- `Actions/Squad/Pedro/README.md`
- `Actions/Squad/Toothless/README.md`
- `Actions/Squad/README.md` (includes `offering.cs` docs)

**Read the relevant subfeature README before making changes.**

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

`Actions/Twitch Integration/stream-start.cs` resets all Squad state at stream start. Any new Squad global variable must be added to that reset list and to `Actions/SHARED-CONSTANTS.md`.

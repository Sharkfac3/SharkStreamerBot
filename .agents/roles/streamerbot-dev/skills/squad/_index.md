# Squad — Feature Overview

## Scope

Chat-triggered mini-games and interaction behavior. Each action folder is a self-contained module.

## Action Folders

| Folder | Path | Scripts |
|---|---|---|
| Clone | `Actions/Squad/Clone/` | `clone-main.cs`, `clone-position.cs`, `clone-volley.cs` |
| Duck | `Actions/Squad/Duck/` | `duck-main.cs`, `duck-call.cs`, `duck-resolve.cs` |
| Pedro | `Actions/Squad/Pedro/` | `pedro-main.cs` |
| Toothless | `Actions/Squad/Toothless/` | `toothless-main.cs` |
| Offering | `Actions/Squad/` | `offering.cs` — also implements the LotAT steal mechanic; reads LotAT state vars reset by `Actions/Twitch Core Integrations/stream-start.cs` |

## Detailed Docs

Read the relevant README before making changes:
- `Actions/Squad/Clone/README.md`
- `Actions/Squad/Duck/README.md`
- `Actions/Squad/Pedro/README.md`
- `Actions/Squad/Toothless/README.md`
- `Actions/Squad/README.md` (offering.cs docs)

## Shared Constants (key groups)

All Squad scripts share global variable names from `Actions/SHARED-CONSTANTS.md`:
- **Mini-game Lock** — `minigame_active`, `minigame_name`
- **Duck** — `duck_event_active`, `duck_quack_count`, `duck_caller`, `duck_unlocked`
- **Clone** — `clone_unlocked`, `clone_game_active`, `clone_round`, `clone_positions_*`, `clone_winners`
- **Pedro** — `pedro_game_enabled`, `pedro_mention_count`, `pedro_unlocked`
- **Toothless** — `rarity_*`, `last_roll`, `last_rarity`, `last_user`
- **LotAT / Offering** — `lotat_active`, `lotat_announcement_sent`, `lotat_offering_steal_chance`, `boost_*`

## Behavioral Expectations

- Handle spam/rapid triggers gracefully
- Use deterministic resolution logic where possible
- Protect against invalid or missing inputs
- Preserve fairness and existing game feel unless explicitly adjusted
- All mini-games must follow the mini-game lock contract (see `streamerbot-dev/skills/core.md`)

## Stream-Start Reset

`Actions/Twitch Core Integrations/stream-start.cs` resets all Squad state at stream start. Any new Squad global variable must be added there and to `Actions/SHARED-CONSTANTS.md`.

## Player Key

`userId` is the preferred player key — stable even if a user changes their display name.

## Overlay Rendering

All four squad games now have overlay visual implementations.  The overlay
renders game state via `squad.*` broker messages.

**Reference:** `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`

### Publish Templates

Each game has a reference template at `Actions/Squad/<Game>/overlay-publish.cs`.
These are NOT deployed actions — copy the methods into the existing game scripts:

| Script | Method(s) to copy in |
|--------|---------------------|
| duck-main.cs | `PublishDuckStart()` |
| duck-call.cs | `PublishDuckUpdate()`, `PublishDuckEndSuccess()` |
| duck-resolve.cs | `PublishDuckEndFailure()` |
| pedro-main.cs | `PublishPedroStart(triggeredBy)` |
| pedro-call.cs | `PublishPedroUpdate(mentionCount)` |
| pedro-resolve.cs | `PublishPedroEndSuccess()`, `PublishPedroEndFailure()` |
| clone-main.cs | `PublishCloneStart(triggeredBy)` |
| clone-volley.cs | `PublishCloneUpdate()`, `PublishCloneEndWin()`, `PublishCloneEndLoss()` |
| toothless-main.cs | `PublishToothlessStart(triggeredBy)`, `PublishToothlessEnd(rarity, username, isFirstUnlock)` |

### Broker Topics Published

```
squad.duck.start / squad.duck.update / squad.duck.end
squad.pedro.start / squad.pedro.update / squad.pedro.end
squad.clone.start / squad.clone.update / squad.clone.end
squad.toothless.start / squad.toothless.end
```

No `squad.toothless.update` is published (Toothless rolls are instant).

## Sub-Skills

Load the specific game file when your task is scoped to that game:
- `clone.md` — Clone mini-game specifics
- `duck.md` — Duck mini-game specifics
- `pedro.md` — Pedro mini-game specifics
- `toothless.md` — Toothless mini-game specifics

# Role: lotat-writer

## What This Role Does

Creates narrative content for Legends of the ASCII Temple (LotAT) — adventure design, lore, worldbuilding, story JSON files, and franchise development. This role operates at the content layer; it does not write C# code.

## Activate When

- Writing a new LotAT adventure (story JSON)
- Designing new missions, story arcs, or branching paths
- Expanding lore, building out the universe, or adding world elements
- Writing character backstory or personality details
- Developing the Starship Shamples franchise or any future franchise
- Planning story arcs tied to real-world build sessions (chain to `brand-steward/content-strategy`)

## Do Not Activate When

- Task is C# engine implementation → use `lotat-tech`
- Task is brand/voice output not related to story content → use `brand-steward`
- Task is Streamer.bot scripting → use `streamerbot-dev`

## Skill Load Order

1. `skills/core.md` — always load first; narrative principles and canon rules
2. `skills/universe/_index.md` — when cast, universe rules, or world elements are involved
3. `skills/universe/cast.md` — when writing any character into a scene or story
4. `skills/universe/rules.md` — when designing mechanics or universe logic
5. `skills/adventures/_index.md` — when building a story or adventure
6. `skills/adventures/mechanics.md` — when designing dice hooks, Chaos Meter, commander moments
7. `skills/adventures/session-format.md` — when structuring a story for live stream pacing
8. `skills/franchises/starship-shamples.md` — for Starship Shamples-specific canon

## Chains To

| Next Role | When |
|---|---|
| `lotat-tech` | After story JSON is complete — hand off for schema validation and engine implementation |
| `brand-steward` | When story content needs canon review or touches brand identity |
| `brand-steward/content-strategy` | When planning a story tied to a specific build session |

## Out of Scope

- C# engine implementation (that is `lotat-tech`)
- Inventing new cast members without operator approval
- Adding commands not in the supported command list (that is `lotat-tech` to add, `lotat-writer` to use)

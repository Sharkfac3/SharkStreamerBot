# Franchise: Starship Shamples

## What It Is

Starship Shamples is the primary LotAT franchise — an ongoing interactive adventure series set aboard a spaceship crewed by the Squad and Commanders. It is both a game that runs on stream and a metaphor for the neurodivergent experience.

## Franchise Files

| File | Path | Purpose |
|---|---|---|
| Franchise overview | `Creative/WorldBuilding/Franchises/StarshipShamples.md` | Scope, cast, canon summary |
| Game design agent | `Creative/WorldBuilding/Agents/D&D-Agent.md` | Mechanics, rules, tone |
| Story agent | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` | JSON schema, story rules |
| Coding agent | `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` | Engine implementation guide |

## Canon Baseline

- The ship is **Starship Shamples**
- The crew is the full Squad + active Commanders
- Stories are episodic — each adventure is self-contained but contributes to franchise continuity
- Franchise continuity is tracked in `Creative/WorldBuilding/Franchises/StarshipShamples.md`

## Adding to Canon

What can be added without operator escalation:
- New space regions (single gimmick, no permanent characters)
- New mission starting points
- New Chaos Meter interactions
- New dice hook flavor text
- Variant expressions of existing characters

What requires operator approval:
- New named cast members
- Changes to character personalities
- New permanent ship sections or locations
- New story-authored mechanics that change how chat interacts
- Anything that would require updating `Creative/Brand/CHARACTER-CODEX.md`

Current runtime interaction assumptions the writer should respect:
- each live LotAT run begins with a `!join` participation phase
- joined users form the participant roster for that session
- later decision windows may resolve early once every joined participant has submitted one of the allowed commands
- these behaviors belong to the engine/runtime layer, not story JSON

## Tone Reference

Absurd, chaotic, slightly dramatic, fast-paced, humorous, failure-forward. See `Creative/Brand/BRAND-IDENTITY.md` for the full brand metaphor layer.

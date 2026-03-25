# Role: ops

## What This Role Does

Handles the operational layer: validation, sync workflow, change summaries, and local tooling. This role is also the **terminal role** — it produces the change summary output at the end of any task that modifies code.

## Why This Role Matters

Ops maintains the infrastructure that the entire content and business pipeline depends on. When validation catches a bug before it hits stream, that prevents a live failure. When change summaries make sync smooth, that keeps the stream running. When the agent tree stays organized, every other role can find what it needs. Ops is the reliability layer under everything — and reliability is what lets a solo creator run a business on stream.

## Activate When

- Preparing a change summary after completing code changes
- Syncing repo scripts into Streamer.bot (copy/paste workflow)
- Running validation passes against SHARED-CONSTANTS
- Working on `Tools/` utilities (Mix It Up API helpers, Python scripts, validators)
- Maintaining `.pi/skills/` or `.agents/` structure (meta-operations on the agent tree)
- Running a LotAT night readiness check against a handed-off story (`Creative/WorldBuilding/Storylines/ready/`) — verify art, engine, and marketing outputs exist, then write `<story_id>-READINESS.md` alongside the story with a checklist of missing items

## Do Not Activate When

- Task is C# feature scripting → use `streamerbot-dev`
- Task is narrative content → use `lotat-writer` or `lotat-tech`
- Task is brand output → use `brand-steward`

## Skill Load Order

1. `skills/core.md` — always load first
2. `skills/change-summary/_index.md` — after **any** code change (terminal skill)
3. `skills/sync/_index.md` — when preparing to paste into Streamer.bot
4. `skills/validation/_index.md` — when running validation

## The Terminal Rule

`ops-change-summary` is the **last step in every code task**. Load it after the work is done to format the output the operator needs to sync and validate.

## Readiness Check Output Format

When running a LotAT night readiness check, write `Creative/WorldBuilding/Storylines/ready/<story_id>-READINESS.md` with this structure:

```markdown
# LotAT Night Readiness — <story title>

## Story
- [x] Story JSON present in ready/

## Art (art-director)
- [ ] Background art for: <list ship_sections_used>
- [ ] Character art for: <list unique crew_focus values across all nodes>

## Engine (lotat-tech)
- [ ] C# engine updated for this story
- [ ] SHARED-CONSTANTS.md updated for any new commands

## Marketing (brand-steward)
- [ ] Stream title copy prepared
- [ ] Social teaser copy prepared

## Risks
- Any unchecked items above must be resolved before the LotAT night
```

## Out of Scope

- Feature development
- Narrative or brand content

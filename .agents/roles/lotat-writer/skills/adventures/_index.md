# Adventures — Overview

## What an Adventure Is

A single LotAT story arc: a JSON file containing nodes, choices, narration, and supported mechanics that the engine executes live on stream. Adventures are self-contained — each has a starting node, branching middle, and multiple endings.

When field-level structure matters, the authoritative contract is `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`. This overview is guidance, not a competing schema.

## Story File Locations

| Type | Path |
|---|---|
| Story agent prompt / authoritative contract | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` |
| Draft story files | `Creative/WorldBuilding/Storylines/drafts/` |
| Ready story files | `Creative/WorldBuilding/Storylines/ready/` |
| Game design / mechanics reference | `Creative/WorldBuilding/Agents/D&D-Agent.md` |

## Minimum Adventure Structure

- ≥ 12 stage nodes
- Exactly 2 choices per stage node
- Multiple distinct ending nodes (not all paths end the same way)
- Chaos Meter escalation across the arc
- Top-level cast usage recorded through the contract's `cast.commanders_used` and `cast.squad_members_used`
- At least one "Pedro makes it worse" moment
- At least one ending that reflects spectacular failure (valid, not shameful)

## Writer Contract Guardrails

- Use only supported top-level and node fields from the authoritative contract
- Do not invent alternate field names, aliases, or one-off metadata blobs
- Do not invent commands; if the story seems to need one, escalate to `lotat-tech`
- Stage nodes normally carry the two chat choices; ending nodes use `choices: []`
- Keep guidance concise here and verify specifics against the contract before finalizing a story file

## Multi-Session Arc Planning

For builds spanning multiple sessions, plan escalating arcs:
- Session 1: Strange anomaly — discovery mode, low chaos
- Mid sessions: Chaos escalates as problems compound
- Final session: Resolution — success, partial survival, or documented spectacular failure

## Sub-Skills

- `mechanics.md` — Chaos Meter, dice hooks, commander moments
- `session-format.md` — pacing for live stream, node length rules

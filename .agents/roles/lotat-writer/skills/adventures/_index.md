# Adventures — Overview

## What an Adventure Is

A single LotAT story arc: a JSON file containing nodes, choices, narration, and mechanics that the C# engine executes live on stream. Adventures are self-contained — each has a starting node, branching middle, and multiple endings.

## Story File Locations

| Type | Path |
|---|---|
| Story agent prompt (schema + rules) | `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` |
| Completed story files | `Creative/WorldBuilding/Storylines/` |
| In-progress / draft adventures | `Creative/WorldBuilding/Experiments/` |
| Game design / mechanics reference | `Creative/WorldBuilding/Agents/D&D-Agent.md` |

## Minimum Adventure Structure

- ≥ 12 stage nodes
- Exactly 2 choices per stage node
- Multiple distinct ending nodes (not all paths end the same way)
- Chaos Meter escalation across the arc
- At least one "Pedro makes it worse" moment
- At least one ending that reflects spectacular failure (valid, not shameful)

## Multi-Session Arc Planning

For builds spanning multiple sessions, plan escalating arcs:
- Session 1: Strange anomaly — discovery mode, low chaos
- Mid sessions: Chaos escalates as problems compound
- Final session: Resolution — success, partial survival, or documented spectacular failure

## Sub-Skills

- `mechanics.md` — Chaos Meter, dice hooks, commander moments
- `session-format.md` — pacing for live stream, node length rules

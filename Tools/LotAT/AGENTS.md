---
id: tools-lotat
type: domain-route
description: LotAT story viewer, schema handoff, validation taxonomy, and local story pipeline tooling guidance.
owner: lotat-tech
secondaryOwners:
  - ops
workflows:
  - change-summary
  - validation
status: active
---

# Tools/LotAT — Agent Guide

## Purpose

[Tools/LotAT/](./) contains local tooling for reviewing and moving Legends of the ASCII Temple stories through the story pipeline.

Current tooling centers on the story viewer in [story_viewer.py](./story_viewer.py), which reads staged story files, renders the branching tree, moves stories between pipeline folders, and copies an approved ready story into the runtime load file used by the Streamer.bot engine.

This folder owns tooling and schema handoff mechanics. It does not own adventure writing or C# runtime behavior.

## When to Activate

Use this guide when working on:

- [Tools/LotAT/story_viewer.py](./story_viewer.py)
- the LotAT story viewer setup, routes, buttons, or file movement behavior
- story pipeline handoff mechanics between drafts, ready, loaded, and finished folders
- schema validation taxonomy or story readiness checks
- tooling that prepares [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json) for the engine
- local validation commands for LotAT story JSON before runtime handoff

If the task changes C# runtime behavior under [Actions/LotAT/](../../Actions/LotAT/), chain to [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md).

## Primary Owner

Primary owner: `lotat-tech`.

`lotat-tech` owns the story schema contract, command-contract changes, validation taxonomy, and the technical handoff between authored JSON and runtime engine consumption.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `ops` | Running local tools, maintaining validation commands, environment setup, or terminal change summaries. |
| `lotat-writer` | Writing or revising story content, adventure branching, lore, pacing, or story files. |
| `streamerbot-dev` | A tooling/schema change requires Streamer.bot engine implementation in [Actions/LotAT/](../../Actions/LotAT/). |
| `app-dev` | A story/tooling change requires stream overlay rendering or protocol changes under [Apps/stream-overlay/](../../Apps/stream-overlay/). |
| `brand-steward` | Canon, cast, metaphor, or public-facing franchise-level changes need review through the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md). |

## Required Reading

Read these first for story pipeline/tooling work:

1. [Tools/LotAT/README.md](./README.md) — viewer install/run instructions and stage actions.
2. [Tools/LotAT/story_viewer.py](./story_viewer.py) — current FastAPI implementation.
3. [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) — shared LotAT contract across tooling, runtime, story, and overlay domains.
4. [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md) — runtime engine handoff expectations.
5. [Actions/LotAT/README.md](../../Actions/LotAT/README.md) — runtime documentation map.
6. [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](../../Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md) — authoritative authored story JSON contract.
7. [Creative/WorldBuilding/Franchises/StarshipShamples.md](../../Creative/WorldBuilding/Franchises/StarshipShamples.md) — franchise/canon reference, not schema authority.
8. [Creative/WorldBuilding/README.md](../../Creative/WorldBuilding/README.md) — worldbuilding folder overview.
9. [validation workflow](../../.agents/workflows/validation.md) and [change-summary workflow](../../.agents/workflows/change-summary.md).

## Local Workflow

1. Confirm whether the task is tooling/schema handoff, story content, or runtime implementation.
2. For tooling changes, work in [Tools/LotAT/](./) and keep behavior aligned with the current folder stages:
   - [Creative/WorldBuilding/Storylines/drafts/](../../Creative/WorldBuilding/Storylines/drafts/)
   - [Creative/WorldBuilding/Storylines/ready/](../../Creative/WorldBuilding/Storylines/ready/)
   - [Creative/WorldBuilding/Storylines/loaded/](../../Creative/WorldBuilding/Storylines/loaded/)
   - [Creative/WorldBuilding/Storylines/finished/](../../Creative/WorldBuilding/Storylines/finished/)
3. Use [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) for shared runtime/tooling/story facts; do not restate or redefine them here.
4. Treat [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json) as the single runtime handoff copy for the engine.
5. Keep the viewer load action as a copy into the loaded runtime file, not a move out of ready.
6. If a ready or finished action invalidates the currently loaded story, clear the loaded runtime copy so Streamer.bot does not consume stale content.
7. Keep comprehensive validation upstream of runtime start. The engine should do minimal-safe load checks, not a second full schema/graph pass.
8. If schema, command contract, or validation taxonomy changes, update the authoritative story contract first, then sync the shared LotAT contract and implementation summaries in the same pass.
8. If canon/cast/metaphor changes, run or request the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md).
9. Finish with the [change-summary workflow](../../.agents/workflows/change-summary.md), including validation output and any runtime handoff notes.

## Validation

For agent-tree/docs validation in this migration prompt, run:

```bash
python3 Tools/AgentTree/validate.py
```

For story viewer tooling sanity, use the commands documented in [Tools/LotAT/README.md](./README.md):

```bash
./run-lotat-story-viewer.sh
```

Manual equivalent:

```bash
python3 -m uvicorn --app-dir . Tools.LotAT.story_viewer:app --reload
```

Validation responsibilities for LotAT story readiness:

- story-writing/review tooling owns comprehensive schema and graph validation before runtime handoff
- the runtime engine owns only minimal-safe start checks and fail-closed behavior
- engine-breaking issues should be hard-fatal before a story can be treated as ready for runtime use
- low-ROI editorial suggestions should remain warnings only when they cannot break engine execution

## Boundaries / Out of Scope

Do not use this folder to:

- write or revise adventures, lore, worldbuilding, or authored story prose; route those tasks to `lotat-writer` in [Creative/WorldBuilding/](../../Creative/WorldBuilding/)
- implement or edit the Streamer.bot C# engine; route that to [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md)
- add overlay visual rendering; route that to `app-dev` under [Apps/stream-overlay/](../../Apps/stream-overlay/)
- treat [Creative/WorldBuilding/Franchises/StarshipShamples.md](../../Creative/WorldBuilding/Franchises/StarshipShamples.md) as a schema authority
- infer `!offering` support from existing Squad experimentation
- create new authored JSON fields for runtime concepts like join rosters, vote maps, timer state, or early-close logic

## Handoff Notes

After changes, report:

- changed files in [Tools/LotAT/](./)
- setup commands or dependency changes
- validation commands run and output
- whether story pipeline behavior changed for drafts, ready, loaded, or finished folders
- whether [current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json) handoff semantics changed
- schema/command changes that require `lotat-writer` or [Actions/LotAT/AGENTS.md](../../Actions/LotAT/AGENTS.md) follow-up
- canon changes that require the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md)

## Runtime Notes

### Story stage locations

| Stage | Path | Tool behavior |
|---|---|---|
| Drafts | [Creative/WorldBuilding/Storylines/drafts/](../../Creative/WorldBuilding/Storylines/drafts/) | Story work before ready review. |
| Ready | [Creative/WorldBuilding/Storylines/ready/](../../Creative/WorldBuilding/Storylines/ready/) | Reviewed candidate stories that can be loaded or finished. |
| Loaded | [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json) | Canonical runtime copy consumed by the engine. |
| Finished | [Creative/WorldBuilding/Storylines/finished/](../../Creative/WorldBuilding/Storylines/finished/) | Completed stories after use/review. |

### Viewer actions

- Draft to ready: move selected story into ready.
- Ready to draft: move selected story back into drafts; clear loaded copy when it was the loaded story.
- Ready to loaded: copy selected story to [current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json).
- Ready to finished: move selected story into finished; clear loaded copy when it was the loaded story.
- Finished to ready: move selected story back into ready.

### Schema authority and synced summaries

Authoritative story schema: [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](../../Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md).

Shared cross-domain summaries live in [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md). Derived or implementation-facing summaries must not compete with the authoritative story contract. If they conflict, update the summary to match the authoritative contract.

## Known Gotchas

- The engine reads only [current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json); it does not choose among ready stories.
- Story viewer module path is `Tools.LotAT.story_viewer`, not a filesystem path passed directly to Uvicorn.
- The load action copies from ready to loaded; it should not consume the ready source file.
- Comprehensive validation belongs before runtime handoff; do not push full schema validation into session start unless v1 contract changes.
- Stage/ending node shape, command boundaries, dice/commander limitations, and offering boundaries are summarized in [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md).
- Runtime concepts such as join windows, roster freeze, vote maps, and early-close are not authored schema fields.

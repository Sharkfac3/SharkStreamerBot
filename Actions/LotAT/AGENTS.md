---
id: actions-lotat
type: domain-route
description: LotAT Streamer.bot runtime engine scripts, session lifecycle, voting, timers, and paste/sync guidance.
owner: lotat-tech
secondaryOwners:
  - streamerbot-dev
  - lotat-writer
  - app-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Actions/LotAT — Agent Guide

## Purpose

[Actions/LotAT/](./) contains the Streamer.bot C# runtime engine for Legends of the ASCII Temple (LotAT) v1.

This folder owns live session execution only:

- starting one LotAT run from the configured trigger
- loading the single runtime story copy at [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json)
- opening and closing the join window
- tracking the session roster and voting state
- entering story nodes and resolving node mechanics
- handling commander and dice pre-vote windows
- collecting authored decision-command votes
- ending and cleaning up safely
- publishing LotAT overlay events through the broker helper template

Story content stays in [Creative/WorldBuilding/](../../Creative/WorldBuilding/). Story review/loading tooling stays in [Tools/LotAT/](../../Tools/LotAT/).

## When to Activate

Use this guide when working on:

- any C# script in [Actions/LotAT/](./)
- LotAT runtime globals, timers, stage transitions, voting, or teardown behavior
- Streamer.bot trigger/action ordering for LotAT scripts
- the runtime side of LotAT overlay publishing from C#
- implementation changes required by story schema or command-contract updates

Also load this guide when a task starts in another domain but changes how the live LotAT engine consumes, starts, or advances stories.

## Primary Owner

Primary owner: `lotat-tech`.

`lotat-tech` owns the runtime contract, schema/command boundaries, session lifecycle, state model, voting behavior, and whether a proposed engine behavior is compatible with LotAT v1.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `streamerbot-dev` | Editing C# actions, Streamer.bot API calls, timers, trigger wiring, global variable resets, or copy/paste deployment notes. |
| `lotat-writer` | A runtime/schema decision requires story JSON changes, adventure updates, or writer-facing guidance. |
| `app-dev` | LotAT broker topics, payload shapes, or overlay rendering behavior changes. |
| `ops` | Running validation, preparing sync/paste notes, or producing the final change summary. |
| `brand-steward` | Runtime changes alter public chat copy, canon, cast meaning, or the Starship Shamples metaphor. Use the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md) for canon-level changes. |

## Required Reading

Read these first for runtime work:

1. [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) — shared LotAT contract across runtime, tooling, story, and overlay domains.
2. [Actions/LotAT/README.md](./README.md) — overview and map to split runtime docs.
3. [Actions/LotAT/runtime-contract.md](./runtime-contract.md) — runtime globals, timers, commands, story-file contract, flow, and v1 boundaries.
4. [Actions/LotAT/operator-setup.md](./operator-setup.md) — Streamer.bot timer/trigger setup and live-test checklist.
5. [Actions/LotAT/implementation-map.md](./implementation-map.md) — script inventory, trigger/input expectations, and paste/sync notes.
6. [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) — canonical global variable and timer names.
7. [Actions/Helpers/README.md](../Helpers/README.md) — reusable Streamer.bot C# helper index.
8. [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) — story pipeline/tooling responsibilities.
9. [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) and [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) before changing overlay payloads or topic names.

## Local Workflow

1. Confirm the task is runtime implementation, not story authoring.
2. Read [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) for shared contract facts; do not restate or redefine them here.
3. Read the relevant split runtime doc: [runtime contract](./runtime-contract.md), [operator setup](./operator-setup.md), or [implementation map](./implementation-map.md).
4. Preserve the runtime/story boundary: runtime session state belongs in C# actions and Streamer.bot globals; authored story data belongs in JSON and the creative/story pipeline.
5. Keep start-time runtime checks minimal-safe. Full schema/graph validation belongs upstream in the story pipeline and reviewer tooling.
6. If adding or renaming globals or timer names, update [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md), [Actions/Twitch Core Integrations/stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs), and the contract docs in the same implementation pass.
7. If changing the story command contract, coordinate with [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md) and update the authoritative story contract plus the shared LotAT contract before treating the engine change as complete.
8. If changing overlay publishing, update the C# publisher helper/template in [overlay-publish.cs](./overlay-publish.cs) and verify the app-side protocol in [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts).
9. Finish with the [change-summary workflow](../../.agents/workflows/change-summary.md), including paste targets and validation output.

## Validation

For documentation/agent-tree changes in this folder, run:

```bash
python3 Tools/AgentTree/validate.py
```

For Streamer.bot runtime changes, also check the relevant workflow docs:

- [validation workflow](../../.agents/workflows/validation.md)
- [sync workflow](../../.agents/workflows/sync.md)
- [change-summary workflow](../../.agents/workflows/change-summary.md)

Runtime validation expectations for LotAT script changes:

- confirm all changed scripts remain copy/paste-ready for Streamer.bot
- confirm changed globals exist in [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)
- confirm reset-sensitive globals and all four LotAT timers are handled in [stream-start.cs](../Twitch%20Core%20Integrations/stream-start.cs)
- smoke test or reason through: start, join, zero-join end, non-zero join, node entry, commander path, dice path, decision vote, early close, ending cleanup, and fault abort

## Boundaries / Out of Scope

Do not use this folder to:

- write adventures, lore, or story content; route that to `lotat-writer` in [Creative/WorldBuilding/](../../Creative/WorldBuilding/)
- modify the story viewer or local pipeline tooling; route that to [Tools/LotAT/AGENTS.md](../../Tools/LotAT/AGENTS.md)
- add visual overlay rendering logic; route that to `app-dev` under [Apps/stream-overlay/](../../Apps/stream-overlay/)
- integrate `!offering` into LotAT v1 without a new explicit contract decision recorded in [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md)
- add operator force-close, manual advance, late join, leave-session, or rich recovery tooling as hidden v1 behavior
- move story content into C# scripts

## Handoff Notes

After changes, report:

- changed files and exact Streamer.bot paste targets
- trigger/timer/action-order setup changes, if any
- new or changed globals and where they reset
- story schema/contract changes that require `lotat-writer` follow-up
- overlay protocol changes that require `app-dev` follow-up
- canon/public-copy changes that require the [canon-guardian workflow](../../.agents/workflows/canon-guardian.md)
- validation commands run and their output

## Runtime Notes

See [.agents/_shared/lotat-contract.md](../../.agents/_shared/lotat-contract.md) for shared LotAT facts such as commands, timers, participation rules, offering boundary, and overlay event contract.

Use the local split docs for runtime-specific details:

- [runtime-contract.md](./runtime-contract.md) — globals, timers, stage flow, and v1 limitations.
- [operator-setup.md](./operator-setup.md) — exact Streamer.bot timer and trigger wiring.
- [implementation-map.md](./implementation-map.md) — script map, trigger/input expectations, and implementation gotchas.

## Paste / Sync Targets

Any edited C# file under [Actions/LotAT/](./) is a Streamer.bot paste target. Include each changed script in the final [sync workflow](../../.agents/workflows/sync.md) output.

Also flag operator setup changes for:

- timer names or durations
- trigger wiring
- action group ordering
- new global variables
- updates to the loaded runtime story prerequisite

## Known Gotchas

- Runtime identity uses lowercase username/login strings, not user IDs, for roster, votes, and commander-target comparison.
- The roster freezes after the join window closes; late join/leave flow is not part of v1.
- Latest valid vote replaces prior valid vote from the same joined user while a decision window remains open.
- If every joined participant has a valid vote, early-close may resolve immediately.
- Zero joiners end the session cleanly before story play.
- Zero valid votes on a node ends the run unresolved; do not invent fallback winners.
- Tie-breaks resolve to the earliest matching choice in the authored `choices` order.
- Commander and dice outcomes are narrative-only in v1; they do not change chaos, branching, vote eligibility, or vote resolution.
- `Actions/Squad/offering.cs` and offering globals are legacy/provisional experimentation, not LotAT v1 mechanics.

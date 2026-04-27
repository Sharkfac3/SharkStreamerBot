# Actions/LotAT

## Purpose

`Actions/LotAT/` contains the current Streamer.bot runtime engine for **Legends of the ASCII Temple (LotAT) v1**.

This folder owns live session execution only:

- start a LotAT run
- open and close the `!join` window
- freeze the participant roster
- enter story nodes from the loaded runtime JSON
- optionally run commander or dice pre-vote windows
- collect and resolve authored decision-command votes
- end and clean up the session safely

For shared facts used across runtime, tooling, story authoring, and overlay presentation, see [Docs/Architecture/lotat-contract.md](../../Docs/Architecture/lotat-contract.md).

## Documentation map

| File | Use |
|---|---|
| [runtime-contract.md](runtime-contract.md) | Runtime globals, timers, commands, story-file contract, session flow, and v1 boundaries. |
| [operator-setup.md](operator-setup.md) | Streamer.bot timer/trigger wiring, prerequisites, and live-test checklist. |
| [implementation-map.md](implementation-map.md) | Script inventory, trigger/input expectations, paste/sync notes, and implementation gotchas. |
| [AGENTS.md](AGENTS.md) | Agent routing, local workflow, validation, boundaries, and handoff expectations for this folder. |
| [Docs/Architecture/lotat-contract.md](../../Docs/Architecture/lotat-contract.md) | Cross-domain LotAT contract index. |

## Current runtime boundary

The checked-in runtime consumes the single loaded story copy at:

- [Creative/WorldBuilding/Storylines/loaded/current-story.json](../../Creative/WorldBuilding/Storylines/loaded/current-story.json)

Story content stays in [Creative/WorldBuilding/](../../Creative/WorldBuilding/). Story review/loading tooling stays in [Tools/LotAT/](../../Tools/LotAT/). Overlay rendering stays in [Apps/stream-overlay/](../../Apps/stream-overlay/).

## Implementation status

The runtime is functional-first and documents current script behavior, not a promise of future mechanics. V1 intentionally excludes offering integration, boost-state integration, late join/leave flow, operator force-close/manual-advance tools, and full runtime schema validation.

See [runtime-contract.md](runtime-contract.md) for the exact current behavior and [operator-setup.md](operator-setup.md) before live testing.

## Streamer.bot paste targets

Any edited C# file under this folder is a Streamer.bot paste target. Include the specific script names and target action/group names in the final change summary.

Documentation-only changes, including this README and the linked split docs, have no Streamer.bot paste target.

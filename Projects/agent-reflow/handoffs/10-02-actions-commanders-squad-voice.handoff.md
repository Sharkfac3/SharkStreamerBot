# Prompt 10-02 handoff — actions-commanders-squad-voice

Date: 2026-04-26
Agent: pi

## State changes

- Created [Actions/Commanders/AGENTS.md](../../../Actions/Commanders/AGENTS.md) for the `actions-commanders` route.
- Created [Actions/Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md) for the `actions-squad` route.
- Created [Actions/Voice Commands/AGENTS.md](../../../Actions/Voice%20Commands/AGENTS.md) for the `actions-voice-commands` route.
- Wrote validator report to [Projects/agent-reflow/findings/10-02-validator.failures.txt](../findings/10-02-validator.failures.txt).

## Migration sources read

- [Projects/agent-reflow/handoffs/10-01-workflows-foundation.handoff.md](10-01-workflows-foundation.handoff.md)
- [Projects/agent-reflow/findings/02-domain-coverage.md](../findings/02-domain-coverage.md)
- [Projects/agent-reflow/findings/05-target-shape.md](../findings/05-target-shape.md)
- [Projects/agent-reflow/findings/06-naming-convention.md](../findings/06-naming-convention.md)
- [Projects/agent-reflow/findings/08-validator.md](../findings/08-validator.md)
- [Actions/Commanders/README.md](../../../Actions/Commanders/README.md)
- [Actions/Squad/README.md](../../../Actions/Squad/README.md)
- [Actions/Voice Commands/README.md](../../../Actions/Voice%20Commands/README.md)
- `.agents/roles/streamerbot-dev/skills/commanders/_index.md`
- `.agents/roles/streamerbot-dev/skills/commanders/captain-stretch.md`
- `.agents/roles/streamerbot-dev/skills/commanders/the-director.md`
- `.agents/roles/streamerbot-dev/skills/commanders/water-wizard.md`
- `.agents/roles/streamerbot-dev/skills/squad/_index.md`
- `.agents/roles/streamerbot-dev/skills/squad/clone.md`
- `.agents/roles/streamerbot-dev/skills/squad/duck.md`
- `.agents/roles/streamerbot-dev/skills/squad/pedro.md`
- `.agents/roles/streamerbot-dev/skills/squad/toothless.md`
- `.agents/roles/streamerbot-dev/skills/voice-commands/_index.md`
- `.agents/roles/streamerbot-dev/context/patterns.md`

## Paste targets

These were documentation-only changes. There are no Streamer.bot script paste targets for this prompt.

If later script changes are made under these folders, the local guides now identify paste targets as the edited `.cs` files under:

- [Actions/Commanders/](../../../Actions/Commanders/)
- [Actions/Squad/](../../../Actions/Squad/)
- [Actions/Voice Commands/](../../../Actions/Voice%20Commands/)

## Validation status

Command run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-02-validator.failures.txt
```

Exit code: `1`

Summary output:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 149 | 49 | FAIL |
| link-integrity | 100 | 331 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 48 | 40 | FAIL |
| orphan | 104 | 18 | FAIL |
| naming | 106 | 0 | PASS |

Total failures: 441
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-02-validator.failures.txt
```

Expected prompt deltas cleared:

- `folder-coverage` missing location/co-location failures are cleared for `actions-commanders`, `actions-squad`, and `actions-voice-commands`.
- `stub-presence` missing entry failures are cleared for `actions-commanders`, `actions-squad`, and `actions-voice-commands`.
- No link-integrity failures are reported for the three new AGENTS.md files.

Remaining failures are expected Phase E baseline items for other domain AGENTS docs, role/root/shared frontmatter, generated routing drift, old source files, Pi meta wrappers, and orphan cleanup.

## Content migrated

### Commanders

Migrated commander slot model, support commands, per-commander script lists, state variables, behavioral expectations, required local README references, Streamer.bot paste expectations, and brand handoff triggers.

Script-name mentions from old central skills were converted to valid local paths such as [Actions/Commanders/Captain Stretch/captain-stretch-redeem.cs](../../../Actions/Commanders/Captain%20Stretch/captain-stretch-redeem.cs).

### Squad

Migrated Squad feature overview, per-game script maps, key global state groups, mini-game lock expectations, stream-start reset expectation, `userId` player-key rule, offering caveat, overlay publishing templates, `squad.*` broker topic notes, Pedro repeated-unlock gotcha, and app/brand/LotAT handoff triggers.

Script-name mentions from old central skills were converted to valid local paths such as [Actions/Squad/Clone/clone-empire-main.cs](../../../Actions/Squad/Clone/clone-empire-main.cs).

### Voice Commands

Migrated mode/scene script map, canonical `stream_mode` values, OBS scene naming table, safe `workspace` fallback, direct `CPH.ObsSetScene(targetScene)` gotcha, paste expectations, and conditional handoff triggers.

Script-name mentions from old central skills were converted to valid local paths such as [Actions/Voice Commands/scene-main.cs](../../../Actions/Voice%20Commands/scene-main.cs).

## Old skill content intentionally not migrated

No substantive commander, Squad, voice-command, or shared pattern content from the listed sources was intentionally left behind.

Not copied verbatim by design:

- Old `Sub-Skills` load instructions pointing to deprecated central skill files. These are replaced by local AGENTS.md route guidance.
- Central source file paths under `.agents/roles/streamerbot-dev/skills/` as navigation targets. They remain migration sources only until cleanup.
- Generic streamerbot-dev behavioral guidance not specific to these folders, except where needed for local paste/sync and validation expectations.

## Open questions / blockers

- None blocking the next prompt.
- Old-source link/orphan failures remain for the migrated commander, Squad, and voice skill files until the cleanup phase retires or stubs those files.

## Next prompt entry point

Proceed with the next Phase E migration prompt. Use [Projects/agent-reflow/findings/10-02-validator.failures.txt](../findings/10-02-validator.failures.txt) as the latest validator baseline.

# Handoff — 100 Manifest Status Normalization

## Summary

Normalized `.agents/manifest.json` status values for the eight audit-identified routes whose co-located `AGENTS.md` files already declare `status: active` and contain substantive migrated guidance.

- `skills` entries were promoted from `planned` to `active`.
- `co_locations` entries were promoted from `planned` to `active`.
- `domains` entries were promoted from `planned` to `covered`, which is the manifest-schema status used for active covered domain routes. The schema does not allow `active` for domain entries.
- No `status: planned` entries remain in `skills`, `domains`, or `co_locations`.

## Per-entry decisions

| Entry | Manifest section(s) | Old status | New status | Rationale |
|---|---|---:|---:|---|
| `actions-destroyer` / `Actions/Destroyer/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide contains full ownership, workflow, validation, paste-target, and runtime guidance. |
| `actions-intros` / `Actions/Intros/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide covers Streamer.bot actions, info-service handoff, Mix It Up playback, validation, and runtime contracts. |
| `actions-rest-focus-loop` / `Actions/Rest Focus Loop/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide contains detailed timer/state-machine ownership, required reading, workflow, validation, and runtime notes. |
| `actions-temporary` / `Actions/Temporary/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide documents standalone temporary timer bridge ownership, workflow, validation, and boundaries. |
| `actions-xj-drivethrough` / `Actions/XJ Drivethrough/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide contains feature purpose, owners, overlay/app handoffs, validation, paste target, and runtime asset notes. |
| `apps-info-service` / `Apps/info-service/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide covers app ownership, schemas, REST contract, workflow, validation commands, and runtime policies. |
| `apps-production-manager` / `Apps/production-manager/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide covers local admin app ownership, info-service integration, workflow, validation, and runtime pages. |
| `tools-lotat` / `Tools/LotAT/AGENTS.md` | `skills`, `domains`, `co_locations` | `planned` | `active` for skill/co-location; `covered` for domain | AGENTS frontmatter is `status: active`; guide contains LotAT story-viewer/tooling ownership, schema handoff, validation, stage workflow, and runtime notes. |

## Planned status audit

After normalization:

```text
skills: 0 planned entries
 domains: 0 planned entries
co_locations: 0 planned entries
```

## Migration ledger decision

Decision: keep `aliases` and `migration.v1_entries` in the active manifest for now.

Rationale:

- `Tools/AgentTree/validate.py` currently reads `manifest.aliases` and validates alias ids and targets during the naming check.
- `aliases` therefore still participates in active validation semantics, even though many entries are deprecated compatibility mappings.
- `migration.v1_entries` appears archival, but no broader tooling audit was requested beyond the current prompt; the prompt default says to keep the ledger if dependency is unclear.
- A later cleanup prompt can split the historical ledger once all active tooling and external consumers are confirmed not to depend on it.

Current ledger counts observed during this prompt:

```text
aliases: 26
migration.v1_entries: 47
```

## Validation

Command run from repo root:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/100-validator.failures.txt
```

Final validator stdout:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 146 | 0 | PASS |
| link-integrity | 48 | 0 | PASS |
| drift | 2 | 0 | PASS |
| stub-presence | 46 | 0 | PASS |
| orphan | 20 | 0 | PASS |
| naming | 105 | 0 | PASS |

Total failures: 0
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/100-validator.failures.txt
```

## Notes

- No `AGENTS.md` docs were downgraded or edited; all audit-listed docs were confirmed substantive and active.
- No git operations were performed.

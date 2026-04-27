# Prompt 10-07 handoff — apps-info-production-doc-folding

Date: 2026-04-26
Agent: pi

## State changes

- Created [Apps/info-service/AGENTS.md](../../../Apps/info-service/AGENTS.md) for route `apps-info-service`.
- Created [Apps/production-manager/AGENTS.md](../../../Apps/production-manager/AGENTS.md) for route `apps-production-manager`.
- Created [Apps/info-service/INFO-SERVICE-PLAN.md](../../../Apps/info-service/INFO-SERVICE-PLAN.md) as the app-local architecture, schema, REST protocol, and operations guide.
- Created [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../../../Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md) as the app-local admin UI/runtime guide.
- Replaced [Docs/INFO-SERVICE-PLAN.md](../../../Docs/INFO-SERVICE-PLAN.md) with a short compatibility pointer to the app-local docs.
- Replaced [.agents/_shared/info-service-protocol.md](../../../.agents/_shared/info-service-protocol.md) with a short compatibility pointer to the app-local protocol guide.
- Updated [Apps/info-service/README.md](../../../Apps/info-service/README.md) to point to the app-local plan/protocol doc.
- Updated [Apps/production-manager/README.md](../../../Apps/production-manager/README.md) to point to the app-local production-manager and info-service docs.
- Updated [.agents/roles/app-dev/context/info-service.md](../../../.agents/roles/app-dev/context/info-service.md) and [.agents/roles/app-dev/skills/core.md](../../../.agents/roles/app-dev/skills/core.md) to point to the new local docs instead of the old shared protocol/Docs plan.
- Updated [.agents/ENTRY.md](../../../.agents/ENTRY.md) and [retired Pi skill mirror/meta-agents-navigate/SKILL.md](../../../retired Pi skill mirror/meta-agents-navigate/SKILL.md) to route info-service protocol reading to the app-local guide.

## Exact app-local doc paths

| Path | Purpose |
|---|---|
| [Apps/info-service/AGENTS.md](../../../Apps/info-service/AGENTS.md) | Local agent guide for route `apps-info-service`. |
| [Apps/info-service/INFO-SERVICE-PLAN.md](../../../Apps/info-service/INFO-SERVICE-PLAN.md) | Migrated architecture plan, protocol reference, schemas, error conventions, data policy, and validation notes. |
| [Apps/production-manager/AGENTS.md](../../../Apps/production-manager/AGENTS.md) | Local agent guide for route `apps-production-manager`. |
| [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../../../Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md) | Admin UI guide, runtime contract, page responsibilities, and pending-intros workflow notes. |

## Old docs status

- [Docs/INFO-SERVICE-PLAN.md](../../../Docs/INFO-SERVICE-PLAN.md) remains as a compatibility pointer only. New implementation details should go to [Apps/info-service/INFO-SERVICE-PLAN.md](../../../Apps/info-service/INFO-SERVICE-PLAN.md) or [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../../../Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md).
- [.agents/_shared/info-service-protocol.md](../../../.agents/_shared/info-service-protocol.md) remains as a compatibility pointer only. New protocol details should go to [Apps/info-service/INFO-SERVICE-PLAN.md](../../../Apps/info-service/INFO-SERVICE-PLAN.md).

## Migration sources read

- [Projects/agent-reflow/handoffs/02-inventory-domains.handoff.md](02-inventory-domains.handoff.md)
- [Projects/agent-reflow/handoffs/04-inventory-cross-refs.handoff.md](04-inventory-cross-refs.handoff.md)
- [Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md](05-design-target-shape.handoff.md)
- [Projects/agent-reflow/handoffs/06-design-naming-convention.handoff.md](06-design-naming-convention.handoff.md)
- [Projects/agent-reflow/handoffs/07-manifest-schema-v2.handoff.md](07-manifest-schema-v2.handoff.md)
- [Projects/agent-reflow/handoffs/08-validator.handoff.md](08-validator.handoff.md)
- [Projects/agent-reflow/findings/02-domain-coverage.md](../findings/02-domain-coverage.md)
- [Projects/agent-reflow/findings/04-cross-refs.md](../findings/04-cross-refs.md)
- [Projects/agent-reflow/findings/05-target-shape.md](../findings/05-target-shape.md)
- [Projects/agent-reflow/findings/06-naming-convention.md](../findings/06-naming-convention.md)
- [Projects/agent-reflow/findings/07-manifest-v2.md](../findings/07-manifest-v2.md)
- [Projects/agent-reflow/findings/08-validator.md](../findings/08-validator.md)
- [.agents/_shared/info-service-protocol.md](../../../.agents/_shared/info-service-protocol.md)
- [.agents/roles/app-dev/context/info-service.md](../../../.agents/roles/app-dev/context/info-service.md)
- [.agents/roles/app-dev/skills/core.md](../../../.agents/roles/app-dev/skills/core.md)
- [Apps/info-service/README.md](../../../Apps/info-service/README.md)
- [Apps/production-manager/README.md](../../../Apps/production-manager/README.md)
- [Docs/INFO-SERVICE-PLAN.md](../../../Docs/INFO-SERVICE-PLAN.md)

## Build/test/validation commands discovered

From [Apps/info-service/package.json](../../../Apps/info-service/package.json):

```bash
cd Apps/info-service
npm install
npm run dev
npm run typecheck
npm run build
```

Info-service smoke check:

```text
GET http://127.0.0.1:8766/health
```

From [Apps/production-manager/package.json](../../../Apps/production-manager/package.json):

```bash
cd Apps/production-manager
npm install
npm run dev
npm run typecheck
npm run build
npm run preview
```

Production-manager runtime checks:

- Dev URL: `http://127.0.0.1:5174`.
- Preview URL: `http://127.0.0.1:4174`.
- Requires info-service running at `http://127.0.0.1:8766` for live health/data pages.

## Manifest update status

No [.agents/manifest.json](../../../.agents/manifest.json) update was needed. The manifest already declared:

- `apps-info-service` at [Apps/info-service/AGENTS.md](../../../Apps/info-service/AGENTS.md)
- `apps-production-manager` at [Apps/production-manager/AGENTS.md](../../../Apps/production-manager/AGENTS.md)

The moved/split plan files were documented by local links rather than new manifest co-location entries.

## Validation status

Command run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-07-validator.failures.txt
```

Exit code: `1`.

Summary output:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 149 | 19 | FAIL |
| link-integrity | 115 | 318 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 48 | 25 | FAIL |
| orphan | 104 | 18 | FAIL |
| naming | 106 | 0 | PASS |

Total failures: 383
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-07-validator.failures.txt
```

Expected prompt deltas cleared:

- `folder-coverage` and `stub-presence` missing entry failures are cleared for `apps-info-service` and `apps-production-manager`.
- No remaining validator failures mention [Apps/info-service/](../../../Apps/info-service/), [Apps/production-manager/](../../../Apps/production-manager/), [Docs/INFO-SERVICE-PLAN.md](../../../Docs/INFO-SERVICE-PLAN.md), [.agents/_shared/info-service-protocol.md](../../../.agents/_shared/info-service-protocol.md), or [.agents/roles/app-dev/context/info-service.md](../../../.agents/roles/app-dev/context/info-service.md) in the final targeted grep.
- Remaining failures are expected Phase E backlog items for other uncreated local docs, old central skill-source links, root/generated drift, role frontmatter, Pi meta wrapper links, and orphan cleanup.

## Paste targets

Documentation-only change. No Streamer.bot script paste targets.

Relevant cross-link only: app docs now link to [Actions/Intros/AGENTS.md](../../../Actions/Intros/AGENTS.md) for Streamer.bot custom-intro script ownership.

## Open questions / blockers

- None blocking the next prompt.
- `pending-intros` fulfillment UI remains planned in [Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md](../../../Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md), matching the existing README pending note.
- The old `.agents/roles/app-dev/context/info-service.md` remains as transitional context until the later role/core/context cleanup phase retires or stubs central app-dev sources.

# Prompt 02 — Inventory Domains

## Agent

Pi (manual copy/paste by operator).

## Purpose

Catalog every domain folder (`Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`) and map each subfolder to its current skill leaf in `.agents/roles/`. Identifies coverage gaps (subfolders without a skill leaf) and per-app/per-feature granularity issues.

## Preconditions

- Prompts 00, 01 done; their findings exist.
- Read both prior handoffs first.

## Scope

Read-only. Writes `Projects/agent-reflow/findings/02-domain-coverage.md`.

## Out-of-scope

- No edits to any domain folder
- No git operations

## Steps

1. For each top-level domain (`Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`), list immediate subfolders.

2. For each subfolder, identify:
   - Current owning role (per `legacy-v1-routing-manifest` quick_routing + role.md activation rules)
   - Current skill leaf path under `.agents/roles/<role>/skills/...` (if any)
   - Whether the subfolder has its own `README.md`
   - Whether the subfolder has its own `CLAUDE.md` or `AGENTS.md`
   - Approximate scope (number of files, lines, or "small / medium / large")

3. Flag coverage gaps:
   - Subfolders with no skill leaf (e.g. `Actions/Destroyer/`, `Actions/XJ Drivethrough/`, `Actions/Intros/`, `Actions/Overlay/`, `Actions/Rest Focus Loop/` per known gap list)
   - Many-to-one collapses (e.g. four `Actions/Twitch *` folders mapping to single `skills/twitch/`)
   - One-to-many gaps (e.g. `app-dev` covering three different runtimes — info-service, production-manager, stream-overlay — under one core.md)

4. For `Docs/` specifically, propose a co-location target per file (e.g. `Docs/INFO-SERVICE-PLAN.md` → `Apps/info-service/`). Mark "uncertain" where the right home is not obvious — Phase B / E will resolve.

5. Write findings file with sections per domain.

## Validator / Acceptance

- Every domain subfolder appears in the findings table
- Coverage gaps explicitly listed
- `Docs/` co-location proposals included (even if marked "uncertain")

## Handoff

Per template. Note: any domain subfolder where ownership is genuinely ambiguous → flag for Phase B operator decision.

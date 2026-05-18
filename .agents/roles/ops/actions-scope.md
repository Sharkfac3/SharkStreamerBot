---
id: ops-actions-scope
type: scope
description: Scoped reading list for ops validating and maintaining Actions scaffolding.
owner: ops
parent: role.md
---

# ops — Actions Scope

ops handles validation, sync workflow, and routing doc maintenance in Actions/. Load in this order for Actions work.

## Validation Run

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry
2. [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) — contract format spec
3. The local contracts file for the folder being validated (for example, [Actions/Intros/contracts.md](../../../Actions/Intros/contracts.md))
4. Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`

## Routing / Scaffolding Maintenance

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — master router
2. [Actions/RULES.md](../../../Actions/RULES.md) — domain rules
3. [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — role matrix
4. The local `AGENTS.md` for the folder being updated

## Sync / Paste Workflow

Follow [.agents/workflows/sync.md](../../../.agents/workflows/sync.md).
After changes: [.agents/workflows/change-summary.md](../../../.agents/workflows/change-summary.md).

## Constants Index

[Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) — index of all constants files.

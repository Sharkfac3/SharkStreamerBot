---
id: docs-architecture
type: domain-route
description: Repo architecture docs and future docs ownership routing.
status: active
owner: ops
secondaryOwners: product-dev
workflows: change-summary, validation
---

# Docs Architecture — Agent Guide

## Purpose

Guide changes to human-facing repository architecture documentation under this folder. Keep these docs about repo shape, documentation routing, and architecture conventions rather than domain implementation details.

## When to Activate

Activate when editing architecture docs under [Docs/Architecture/](./), especially [repo-structure.md](repo-structure.md), or when documenting repo-wide structural decisions that should be readable by humans outside the agent tree.

## Primary Owner

`ops` owns repo architecture routing, manifest alignment, and validation expectations for this folder.

## Secondary Owners / Chain To

- `product-dev` when architecture docs become customer-facing product documentation or technical knowledge articles.
- Domain owners when architecture notes describe a specific implementation area such as [Actions/](../../Actions/), [Apps/](../../Apps/), [Tools/](../../Tools/), or [Creative/](../../Creative/).
- `brand-steward` if public-facing docs need brand voice or claims review.

## Required Reading

- [repo-structure.md](repo-structure.md) for current top-level repository structure.
- [.agents/_shared/project.md](../../.agents/_shared/project.md) for repo-wide priorities and scope.
- [.agents/_shared/conventions.md](../../.agents/_shared/conventions.md) for agent/file routing conventions.
- [.agents/workflows/coordination.md](../../.agents/workflows/coordination.md) before starting.

## Local Workflow

1. Keep architecture docs human-readable and repo-wide.
2. Prefer linking to local domain `AGENTS.md` files rather than duplicating domain procedures.
3. Keep reusable agent procedures in [.agents/workflows/](../../.agents/workflows/), not in Docs.
4. If a doc describes app/tool internals, consider whether it belongs beside that app/tool instead.
5. Update [.agents/manifest.json](../../.agents/manifest.json) only when routing or co-location declarations change.

## Validation

For route/doc changes, run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/<prompt>-validator.failures.txt
```

For general documentation-only edits, at minimum verify links you changed and report any validator failures that are unrelated migration backlog.

## Boundaries / Out of Scope

- Do not store Streamer.bot runtime procedures here; use local [Actions/](../../Actions/) guides.
- Do not store app implementation plans here once an app-local guide exists under [Apps/](../../Apps/).
- Do not store agent workflows here; use [.agents/workflows/](../../.agents/workflows/).
- Do not duplicate product/marketing/canon guidance from [Creative/](../../Creative/) or future product docs.

## Handoff Notes

Documentation-only changes have no Streamer.bot paste targets. Include changed docs, validation output, and any routing/manifest changes in the handoff or change summary.

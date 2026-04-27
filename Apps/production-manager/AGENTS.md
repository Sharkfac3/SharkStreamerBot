---
id: apps-production-manager
type: domain-route
description: Local React/Vite admin UI for managing info-service collections.
owner: app-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - validation
status: active
---

# Production Manager — Agent Guide

## Purpose

This folder owns `production-manager`, the local React/Vite admin app for viewing info-service health and managing stream production data such as custom user intros.

## When to Activate

Use this guide for work under [Apps/production-manager/](./), including:

- React pages, forms, tables, and admin workflows.
- Vite/Tailwind configuration.
- Calls to [Apps/info-service/](../info-service/) REST routes.
- Operator UI behavior for `user-intros` and future `pending-intros` fulfillment.
- App-local docs replacing repo-level info-service/production-manager planning notes.

## Primary Owner

`app-dev` owns the React/Vite app, UI-to-info-service integration, local setup commands, and admin workflow documentation.

## Secondary Owners / Chain To

- `brand-steward` — chain before changing viewer-facing policy language, redemption instructions, or public-facing copy surfaced by the UI.
- `ops` — chain for validation reports, operator setup notes, and change-summary handoff.
- `streamerbot-dev` — chain only when UI/data-contract changes require corresponding C# script behavior changes in [Actions/Intros/AGENTS.md](../../Actions/Intros/AGENTS.md).

## Required Reading

Read these before changing admin behavior:

- [README.md](README.md) for current setup, pages, and pending work.
- [PRODUCTION-MANAGER-GUIDE.md](PRODUCTION-MANAGER-GUIDE.md) for runtime contract, page responsibilities, and pending-intros workflow notes.
- [package.json](package.json) for npm scripts.
- [Apps/info-service/AGENTS.md](../info-service/AGENTS.md) for backend ownership and route-change handoffs.
- [Apps/info-service/INFO-SERVICE-PLAN.md](../info-service/INFO-SERVICE-PLAN.md) for REST routes and schemas.

## Local Workflow

1. Identify whether the change is UI-only, data-contract integration, styling/tooling, or documentation-only.
2. Keep dev and preview servers bound to localhost unless the operator explicitly changes the deployment model.
3. Keep info-service schemas authoritative; do not duplicate incompatible schema rules in UI-only code.
4. Preserve stable keys: use Twitch `userId` for `user-intros`, not login/display name.
5. Store asset filenames in records, not absolute paths.
6. Keep operator-only notes private; never route notes into public stream output.
7. If route/schema assumptions change, update [PRODUCTION-MANAGER-GUIDE.md](PRODUCTION-MANAGER-GUIDE.md), [README.md](README.md), and the info-service guide together when in scope.
8. For public wording or policy text, chain to `brand-steward` before finalizing.

## Validation

App commands:

```bash
cd Apps/production-manager
npm install
npm run typecheck
npm run build
npm run dev
npm run preview
```

Runtime smoke checks:

- Start [Apps/info-service/](../info-service/) first.
- Open `http://127.0.0.1:5174` in dev.
- Confirm the Health page can read `GET /health`.
- Confirm the User Intros page can load, create/edit/delete records against local test data if behavior changed.

Agent-tree validation for doc/routing changes:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-07-validator.failures.txt
```

## Boundaries / Out of Scope

- Do not expose the app to LAN or public internet in v1.
- Do not add auth/session state unless the local-only deployment model changes.
- Do not make Streamer.bot depend on production-manager being open.
- Do not edit Streamer.bot action scripts as part of this folder without chaining to `streamerbot-dev`.
- Do not move backend schema authority out of [Apps/info-service/](../info-service/).

## Handoff Notes

For UI or integration changes, include:

- Pages/components changed.
- Info-service routes or schemas affected.
- Commands run from this folder and their outputs.
- Any operator setup steps, including whether info-service must be running for validation.
- Public-copy or policy-review status if text changed.

## Runtime Notes

Current pages:

| Page | Route | Source file | Backend dependency |
|---|---|---|---|
| Health | root route | [src/pages/HealthPage.tsx](src/pages/HealthPage.tsx) | `GET /health` |
| User Intros | user-intros route | [src/pages/UserIntrosPage.tsx](src/pages/UserIntrosPage.tsx) | `GET/POST/PUT/DELETE /info/user-intros/:key` |

Pending work: add a `pending-intros` fulfillment page after operator workflow decisions are finalized.

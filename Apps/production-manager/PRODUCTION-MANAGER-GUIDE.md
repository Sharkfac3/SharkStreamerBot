# Production Manager — App Guide

`production-manager` is the local React/Vite admin UI for managing data served by [Apps/info-service/](../info-service/). It is the operator-facing half of the custom-intro workflow and should remain local-only.

## Purpose

The app gives the operator a browser UI for viewing service health and editing info-service collections without hand-editing JSON. The implemented intro pages manage `user-intros` records and fulfill or reject captured `pending-intros` redemptions.

## Runtime Contract

| Item | Value |
|---|---|
| App path | `Apps/production-manager/` |
| Framework | React 18 + Vite 5 |
| Language | TypeScript |
| Styling | Tailwind CSS v3 through PostCSS |
| Dev URL | `http://127.0.0.1:5174` |
| Preview URL | `http://127.0.0.1:4174` |
| Backend dependency | `http://127.0.0.1:8766` from [Apps/info-service/](../info-service/) |
| Auth | none in v1; local machine only |

## Key Files

| Path | Purpose |
|---|---|
| [package.json](package.json) | npm scripts and dependencies. |
| [vite.config.ts](vite.config.ts) | Vite config, dev/preview ports and local host binding. |
| [tailwind.config.ts](tailwind.config.ts) | Tailwind v3 content and theme config. |
| [postcss.config.js](postcss.config.js) | PostCSS/Tailwind wiring. |
| [src/App.tsx](src/App.tsx) | App shell and route/page selection. |
| [src/pages/HealthPage.tsx](src/pages/HealthPage.tsx) | Calls `GET /health` on info-service. |
| [src/pages/UserIntrosPage.tsx](src/pages/UserIntrosPage.tsx) | `user-intros` table, create/edit/delete form, and soft-disable toggle. |
| [src/pages/PendingIntrosPage.tsx](src/pages/PendingIntrosPage.tsx) | `pending-intros` review table, fulfill workflow, and reject action. |
| [src/index.css](src/index.css) | Tailwind directives and global styles. |

## Setup and Commands

```bash
cd Apps/production-manager
npm install
npm run dev
npm run typecheck
npm run build
npm run preview
```

Run [Apps/info-service/](../info-service/) at `http://127.0.0.1:8766` before expecting live data on the Health or User Intros pages.

## Pages

| Page | Route | Purpose |
|---|---|---|
| Health | `/` | Fetches info-service health, uptime, and loaded collections. |
| User Intros | `/user-intros` | Lists all `user-intros`; create/edit/delete records; toggle `enabled`. |
| Pending Intros | `/pending-intros` | Lists captured `pending-intros`; fulfill pending records into `user-intros`; reject ineligible records. |

## Info-Service Integration

Use the app-local info-service guide as the source of truth for schemas and route contracts: [Apps/info-service/INFO-SERVICE-PLAN.md](../info-service/INFO-SERVICE-PLAN.md).

Current UI behavior should preserve these policies:

- Write `user-intros` through info-service REST routes rather than touching JSON files directly.
- Store filenames only in records; do not store absolute asset paths in collection data.
- Keep operator-only `notes` out of stream-visible output.
- Treat `userId` as the stable key, not Twitch login or display name.
- Expect optional `soundFile`, `gifFile`, `notes`, and `resolvedUtc` fields to be absent rather than `null`.

## Pending Intros Fulfillment

The Pending Intros page:

1. Lists all `pending-intros` records with `status === 'pending'` sorted and highlighted first.
2. Shows user login, user ID, redemption ID, user input, reward title, status, and redeem time.
3. Lets the operator fulfill a pending record by assigning a `soundFile`, optional `gifFile`, and `enabled` value. Fulfillment creates or updates `/info/user-intros/:userId` and then updates `/info/pending-intros/:redeemId` to `status: 'fulfilled'` with `resolvedUtc`.
4. Lets the operator reject pending records by updating `/info/pending-intros/:redeemId` to `status: 'rejected'` with `resolvedUtc`.
5. Avoids reverse transitions in the UI; edit `user-intros` directly for post-fulfillment rework.

## shadcn/ui Status

shadcn/ui is not initialized yet. If the operator requests it, run from this directory:

```bash
npx shadcn@latest init
```

That will add `components.json` and peer dependencies. Keep any generated component paths documented here if adopted.

## Boundaries

- Do not expose the dev server to the LAN or internet.
- Do not add auth/session state unless the local-only deployment model changes.
- Do not make Streamer.bot or overlay code depend on production-manager being open.
- Do not move schema authority into the UI; zod schemas in info-service remain authoritative.
- Chain to `brand-steward` before changing user-facing policy text or redemption instructions.

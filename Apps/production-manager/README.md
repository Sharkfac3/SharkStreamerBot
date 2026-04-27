# production-manager

React + Vite admin app for managing info-service data (user intros, etc.).

## Tailwind path

Uses **Tailwind CSS v3** via PostCSS (`postcss.config.js` + `tailwind.config.ts`). The `index.css` imports the three Tailwind directives.

## shadcn/ui

shadcn/ui is not yet initialized. To add it, run from this directory:

```sh
npx shadcn@latest init
```

This installs `components.json` and peer deps. Requires Node ≥18 and the dev server dependencies already installed.

## Running locally

```sh
npm install
npm run dev
```

Dev server: `http://127.0.0.1:5174` (127.0.0.1 only — no LAN exposure, no auth needed).

Requires `info-service` running at `http://127.0.0.1:8766` for the Health page to show live data. If info-service is not running the Health page will display an error — expected.

See [PRODUCTION-MANAGER-GUIDE.md](PRODUCTION-MANAGER-GUIDE.md) for the app-local admin workflow guide and [Apps/info-service/INFO-SERVICE-PLAN.md](../info-service/INFO-SERVICE-PLAN.md) for backend schemas and REST protocol.

## Build + preview

```sh
npm run build
npm run preview
```

Preview serves at `http://127.0.0.1:4174`.

## Typecheck

```sh
npm run typecheck
```

## Pages

| Page | Path | Purpose |
|------|------|---------|
| Health | `/` | Fetches `GET /health` from info-service; shows status, uptime, and collections. |
| User Intros | `/user-intros` | Table of all user-intros records; create/edit/delete via modal form; soft-disable toggle. |

## Pending

- **Pending Intros fulfillment page** (C10.5): table of `pending-intros` records + fulfill/reject workflow. Blocked on operator decision for Open Question Q14 in `humans/info-service/COORDINATION.md`.

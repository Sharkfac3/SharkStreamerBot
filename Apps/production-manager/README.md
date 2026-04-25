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

## What's here (C6)

- `src/pages/HealthPage.tsx` — fetches `GET /health` from info-service and displays status, uptime, and collections.

## Coming next

- C7: user-intros management page — table, create/edit form, file picker for `Assets/user-intros/`.

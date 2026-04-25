# Prompt C6 — production-manager Skeleton

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — especially §Folder Layout, §Tech Stack, §Constants to Register
6. `Apps/info-service/package.json` — reference for TypeScript version and naming conventions
7. `Actions/SHARED-CONSTANTS.md` — confirm `PRODUCTION_MANAGER_PORT` (5174) and `INFO_SERVICE_BASE_URL` values

Role: `app-dev`.

## Prereqs

Chunks merged: C1, C2. Verify by reading `Apps/info-service/src/server.ts` — `buildServer()` must exist. C3/C4 do NOT need to be merged; this chunk is parallel to them.

## WORKING.md

Add row at start. Domain: `Apps/production-manager/`, `.agents/`, `.gitignore`. Files: `Apps/production-manager/package.json`, `Apps/production-manager/tsconfig.json`, `Apps/production-manager/vite.config.ts`, `Apps/production-manager/index.html`, `Apps/production-manager/src/main.tsx`, `Apps/production-manager/src/App.tsx`, `Apps/production-manager/src/pages/HealthPage.tsx`, `Apps/production-manager/src/index.css`, `Apps/production-manager/README.md`, `.gitignore`, `.agents/roles/app-dev/role.md`. Remove + log at finish.

## Task

Scaffold `Apps/production-manager/` — the React + Vite admin app. This chunk creates the skeleton only: project scaffold, Tailwind CSS setup, dev server bound to 127.0.0.1:5174, and a minimal Health page that fetches `GET http://127.0.0.1:8766/health` and displays the result. No user-intros UI — that is C7.

### 1. `Apps/production-manager/package.json`

- `name`: `@sharkstreamerbot/production-manager`
- `version`: `0.1.0`
- `private`: `true`
- Scripts:
  - `"dev"`: `vite`
  - `"build"`: `tsc && vite build`
  - `"preview"`: `vite preview`
  - `"typecheck"`: `tsc --noEmit`
- Dependencies: `react`, `react-dom` (React 18)
- Dev dependencies: `typescript`, `@types/react`, `@types/react-dom`, `vite`, `@vitejs/plugin-react`

For Tailwind: use `tailwindcss` v3 with `autoprefixer` and `postcss`, OR `tailwindcss` v4 with `@tailwindcss/vite` if available. Check the Tailwind docs for the correct peer deps for the version you install. Document which path was taken in the README.

shadcn/ui setup: after scaffolding, run `npx shadcn@latest init` in `Apps/production-manager/` to add shadcn's `components.json` and install its peer deps. If that CLI is unavailable in the environment, note it in the README and leave a TODO comment — a future operator step. Do NOT manually enumerate shadcn deps.

Match the TypeScript version used in `Apps/info-service/package.json`.

### 2. `Apps/production-manager/tsconfig.json`

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "moduleResolution": "bundler",
    "jsx": "react-jsx",
    "strict": true,
    "skipLibCheck": true,
    "noEmit": true
  },
  "include": ["src"]
}
```

### 3. `Apps/production-manager/vite.config.ts`

```typescript
import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [react()],
  server: {
    host: '127.0.0.1',
    port: 5174,
    strictPort: true,
  },
  preview: {
    host: '127.0.0.1',
    port: 4174,
    strictPort: true,
  },
});
```

If using Tailwind v4 with `@tailwindcss/vite`, add it to `plugins`. If using the PostCSS path (Tailwind v3), create `postcss.config.js` with `tailwindcss` and `autoprefixer`.

### 4. `Apps/production-manager/index.html`

Standard Vite entry HTML. Title: `Production Manager`. References `src/main.tsx` via `<script type="module">`.

### 5. `Apps/production-manager/src/main.tsx`

Standard React 18 entry using `ReactDOM.createRoot`. Import `./index.css`.

```typescript
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import './index.css';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
);
```

### 6. `Apps/production-manager/src/App.tsx`

Minimal app shell. No router library needed in this chunk — render `<HealthPage />` directly. The structure should be easy to extend with a router in C7.

```typescript
import HealthPage from './pages/HealthPage';

export default function App() {
  return <HealthPage />;
}
```

### 7. `Apps/production-manager/src/pages/HealthPage.tsx` (new file)

A single page that:

- On mount, fetches `GET http://127.0.0.1:8766/health` using plain `fetch` (no library — Decision 18 in COORDINATION.md).
- Manages state: `loading`, `error: string | undefined`, `data: HealthResponse | undefined`.
- `HealthResponse` type: `{ ok: boolean; uptime: number; collections: string[] }`.
- Renders:
  - While loading: a loading indicator ("Checking info-service…").
  - On error: error message in a red-tinted box.
  - On success: shows `ok`, `uptime` (rounded to 1 decimal, labelled "Uptime (s)"), and `collections` (comma-separated or "none").
- Uses Tailwind classes for layout (a centered card or panel).

No shadcn components required in this page — plain Tailwind is sufficient for the skeleton.

### 8. `Apps/production-manager/src/index.css`

Import Tailwind. For Tailwind v3:
```css
@tailwind base;
@tailwind components;
@tailwind utilities;
```

For Tailwind v4 (`@tailwindcss/vite`):
```css
@import "tailwindcss";
```

### 9. Tailwind config (if Tailwind v3)

Create `Apps/production-manager/tailwind.config.ts` (or `.js`):

```typescript
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: { extend: {} },
  plugins: [],
};
```

Not needed for Tailwind v4 (auto-detection handles content).

### 10. `.gitignore` additions

Append to the root `.gitignore`:

```
# Apps/production-manager
Apps/production-manager/dist/
Apps/production-manager/node_modules/
```

### 11. `.agents/roles/app-dev/role.md` update

Add `Apps/production-manager/` to the Active Apps section. One bullet, minimal — e.g.:
```
- `Apps/production-manager/` — React + Vite admin app for managing info-service collections (C6+)
```

### 12. `Apps/production-manager/README.md`

Brief operator README:
- What it is (React admin app for managing info-service data — user intros, etc.)
- How to run: `npm install` then `npm run dev` from `Apps/production-manager/`
- Dev server URL: `http://127.0.0.1:5174` (127.0.0.1 only — no LAN exposure, no auth needed)
- Requires `info-service` running at `http://127.0.0.1:8766` for the Health page to show live data
- How to build for preview: `npm run build` then `npm run preview` (serves at port 4174)
- What's coming: user-intros management page (C7)

## Deliverables

- Files changed:
  - `Apps/production-manager/package.json` (new)
  - `Apps/production-manager/tsconfig.json` (new)
  - `Apps/production-manager/vite.config.ts` (new)
  - `Apps/production-manager/index.html` (new)
  - `Apps/production-manager/src/main.tsx` (new)
  - `Apps/production-manager/src/App.tsx` (new)
  - `Apps/production-manager/src/pages/HealthPage.tsx` (new)
  - `Apps/production-manager/src/index.css` (new)
  - `Apps/production-manager/tailwind.config.ts` or equivalent (new, if Tailwind v3)
  - `Apps/production-manager/postcss.config.js` (new, if Tailwind v3 PostCSS path)
  - `Apps/production-manager/README.md` (new)
  - `.gitignore` (new block appended)
  - `.agents/roles/app-dev/role.md` (updated — add production-manager to Active Apps)
- Scaffolding updates: `.agents/roles/app-dev/role.md`
- Shared constants: none (port values already in `Actions/SHARED-CONSTANTS.md` from C1)
- Tests: N/A

## Forbidden in this chunk

- User-intros table, form, or file picker UI — that is C7
- Calling info-service write routes — no write operations in this chunk
- Any changes to `Apps/info-service/` — separate role concern
- Any collection engine or zod schema work — C3/C4
- Any Streamer.bot `.cs` scripts — separate role
- Auth, login, or session management — not in scope (Decision 26 in COORDINATION.md)
- Changing anything under `Apps/stream-overlay/`

## Finish

1. Run `npm install` and `npm run typecheck` from `Apps/production-manager/`. Zero TypeScript errors required. Fix any before reporting done.
2. Optionally start `npm run dev` — verify the app loads at `http://127.0.0.1:5174`. The Health page will show an error if `info-service` is not running — that is expected and acceptable.
3. Update `humans/info-service/COORDINATION.md` — chunk C6 status → `merged`, append Run Log row (Commit column: `uncommitted`).
4. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
5. Load `ops-change-summary` skill from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
6. **Draft the next chunk's prompt file** (self-propagation):
   - Check `COORDINATION.md` for chunks with status `not-started`, Prompt File `tbd`, and all prereqs `merged` (C6 now counts as merged).
   - C7 (`production-manager: user-intros page`) requires **both** C4 and C6. C4 status is `not-started` / `prompt-ready` at the time C6 runs — C4 is not yet merged.
   - C7 is blocked — do NOT draft its prompt yet.
   - No other chunk is unblocked solely by C6.
   - Report in chat: "C7 is blocked — waiting for C4 (`user-intros collection`, prompt `13-user-intros-collection.md`) to merge. No prompt drafted this run."

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/production-manager/` exists with all files listed in Deliverables
- `npm run typecheck` passes with zero errors
- Vite dev server configured for `127.0.0.1:5174` (dev) and `127.0.0.1:4174` (preview), both `strictPort: true`
- `HealthPage` fetches `GET http://127.0.0.1:8766/health` and renders loading / error / success states
- Tailwind CSS installed and working (classes applied in HealthPage)
- `.gitignore` excludes `dist/` and `node_modules/` under `Apps/production-manager/`
- `.agents/roles/app-dev/role.md` Active Apps section includes `Apps/production-manager/`
- `COORDINATION.md` C6 row: status = `merged`, Prompt File = `15-production-manager-skeleton.md`, run log row appended
- C7 blocker reported in chat (waiting on C4)
- No git commit made by agent

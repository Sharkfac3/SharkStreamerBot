You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — §Schemas §5 (`user-intros` record shape)
6. `.agents/_shared/info-service-protocol.md` — route contracts
7. `Apps/production-manager/src/pages/HealthPage.tsx` — existing page pattern (plain fetch, no library)
8. `Apps/info-service/src/store/schemas/user-intros.ts` — canonical field names

Role: `app-dev`.

## Prereqs

Chunks merged: C4, C6. Verify:
- `Apps/info-service/src/store/schemas/user-intros.ts` exists (C4)
- `Apps/production-manager/src/pages/HealthPage.tsx` exists (C6)

## WORKING.md

Add row at start. Domain: `Apps/production-manager/`. Files: `Apps/production-manager/src/pages/UserIntrosPage.tsx`, `Apps/production-manager/src/App.tsx`. Remove + log at finish.

## Task

Implement the user-intros management page in `Apps/production-manager/`. This adds a table listing all records, an inline create/edit form, and a delete button. No file picker for Assets in this chunk — that is a future enhancement. Write operations call the info-service REST routes directly via plain `fetch`.

### Data-fetching pattern

Use plain `fetch` (Decision 18). No library. Follow the pattern established in `HealthPage.tsx`.

Info-service base URL: `http://127.0.0.1:8766` (hardcoded constant at top of file).

### Routes used

| Operation | Method | Path |
|---|---|---|
| List all records | `GET` | `/info/user-intros` |
| Get single record | `GET` | `/info/user-intros/:userId` |
| Create/replace record | `POST` | `/info/user-intros/:userId` |
| Update record | `PUT` | `/info/user-intros/:userId` |
| Delete record | `DELETE` | `/info/user-intros/:userId` |

Response for `GET /info/user-intros` is the full collection envelope:
```json
{
  "schemaVersion": 1,
  "collection": "user-intros",
  "updatedUtc": 1745500000000,
  "records": {
    "12345": { "userId": "12345", "userLogin": "alice", "soundFile": "alice.mp3", "enabled": true, "updatedUtc": 1745500000000 }
  }
}
```

### `Apps/production-manager/src/pages/UserIntrosPage.tsx`

A single page component that:

**State:**
- `records`: `UserIntroRecord[]` — loaded from GET all
- `loading`: `boolean`
- `error`: `string | undefined`
- `editTarget`: `UserIntroRecord | null` — record being edited; `null` = create mode
- `formOpen`: `boolean`

**Types (define locally in this file — do not import from info-service):**
```typescript
interface UserIntroRecord {
  userId: string;
  userLogin: string;
  soundFile?: string;
  gifFile?: string;
  enabled: boolean;
  notes?: string;
  updatedUtc: number;
}
```

**Table:**
- Columns: `userId`, `userLogin`, `soundFile` (or "—"), `gifFile` (or "—"), `enabled` (Yes/No), `updatedUtc` (human-readable date), Actions (Edit | Delete)
- Empty state: "No records yet. Add the first one."
- Loading state: "Loading…"
- Error state: red-tinted box with message

**Form (modal or inline panel — agent's choice):**
- Fields:
  - `userId` (required, string — disabled when editing an existing record)
  - `userLogin` (required, string)
  - `soundFile` (optional, string — filename only, e.g. `alice.mp3`)
  - `gifFile` (optional, string — filename only)
  - `enabled` (boolean toggle/checkbox)
  - `notes` (optional, textarea)
- Submit: POST (create) or PUT (edit). On success, reload the record list and close the form.
- Cancel: close form, discard changes.
- Validation: `userId` and `userLogin` are required; show inline error if empty on submit.

**Delete:**
- DELETE route call with confirmation (`window.confirm`).
- On success, remove from local state (optimistic) or reload list.

**`updatedUtc` on write:**
- Set to `Date.now()` in the client before sending. The server stores what it receives.

**Tailwind classes** for layout. No shadcn components required.

### `Apps/production-manager/src/App.tsx` update

Add a minimal nav or tab bar so the operator can switch between Health and UserIntros pages. No router library — use local state (`activePage: 'health' | 'user-intros'`).

```typescript
import { useState } from 'react';
import HealthPage from './pages/HealthPage';
import UserIntrosPage from './pages/UserIntrosPage';

type Page = 'health' | 'user-intros';

export default function App() {
  const [page, setPage] = useState<Page>('health');
  return (
    <div>
      <nav className="...">
        <button onClick={() => setPage('health')}>Health</button>
        <button onClick={() => setPage('user-intros')}>User Intros</button>
      </nav>
      {page === 'health' ? <HealthPage /> : <UserIntrosPage />}
    </div>
  );
}
```

Style the nav with Tailwind to match the dark theme in HealthPage (`bg-gray-950` etc.).

## Deliverables

- `Apps/production-manager/src/pages/UserIntrosPage.tsx` (new)
- `Apps/production-manager/src/App.tsx` (updated — add nav + page switch)
- `COORDINATION.md` — C7 status → `merged`, Prompt File → `17-production-manager-user-intros.md`, run log row appended (Commit: `uncommitted`)
- `WORKING.md` — active row removed, completed row added

## Forbidden in this chunk

- File picker for asset upload — future enhancement
- Any changes to `Apps/info-service/`
- Any Streamer.bot `.cs` scripts
- shadcn components (plain Tailwind only)
- Router library (react-router, tanstack-router, etc.)

## Finish

1. Run `npm run typecheck` from `Apps/production-manager/`. Zero TypeScript errors required.
2. Update COORDINATION.md — C7 status → `merged`, run log row appended.
3. Update WORKING.md — active row removed, completed row added.
4. Load `ops-change-summary` from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. Check COORDINATION.md for newly unblocked chunks. C7 has no chunk that depends solely on it — no prompt to draft this run.
6. **Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `UserIntrosPage.tsx` exists with table, form (create + edit), and delete
- `App.tsx` has nav switching between Health and UserIntros pages
- Form validates `userId` and `userLogin` are non-empty before submitting
- All fetch calls use plain `fetch` — no library
- `updatedUtc` set to `Date.now()` on write
- `npm run typecheck` passes with zero errors
- COORDINATION.md C7 row: status = `merged`, Prompt File = `17-production-manager-user-intros.md`
- No git commit made by agent

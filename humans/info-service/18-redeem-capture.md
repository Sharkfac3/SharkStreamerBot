You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md` — for TypeScript info-service work
4. `.agents/roles/streamerbot-dev/role.md` — for C# script conventions
5. `humans/info-service/COORDINATION.md`
6. `Docs/INFO-SERVICE-PLAN.md` — §6 pending-intros schema, §2 key policy, §3 timestamp policy, §7 migration policy
7. `Apps/info-service/src/store/schemas/user-intros.ts` — existing schema pattern
8. `Apps/info-service/src/collections/user-intros.ts` — existing collection pattern
9. `Apps/info-service/src/index.ts` — current wiring
10. `Actions/SHARED-CONSTANTS.md` — §Info Service / Assets section
11. An existing SB script for C# conventions — e.g. `Actions/Intros/first-chat-intro.cs` (if present) or `Actions/Squad/Duck/duck-main.cs`
12. `.agents/_shared/info-service-protocol.md` — existing protocol doc

Role: `app-dev` (TypeScript under `Apps/`) + `streamerbot-dev` (C# under `Actions/`). This chunk spans both domains. Apply each role's conventions to its respective files.

## Prereqs

Chunks merged: C1–C5. Verify by checking that `Apps/info-service/src/routes/write.ts` exists.

## WORKING.md

Add row at start. Domain: `Apps/info-service/`, `Actions/`. Files: `Apps/info-service/src/store/schemas/pending-intros.ts`, `Apps/info-service/src/collections/pending-intros.ts`, `Apps/info-service/src/index.ts`, `Actions/Intros/redeem-capture.cs`, `Actions/SHARED-CONSTANTS.md`, `.agents/_shared/info-service-protocol.md`. Remove + log at finish.

## Task

Add the `pending-intros` collection to `info-service` and implement the Streamer.bot channel-point redeem handler that writes pending records.

### 1. Schema — `Apps/info-service/src/store/schemas/pending-intros.ts`

Implement `PendingIntroRecordSchema` and `SCHEMA_VERSION = 1` exactly as specified in `Docs/INFO-SERVICE-PLAN.md §6`. No deviations. Export `PendingIntroRecord` type.

```typescript
import { z } from 'zod';

export const SCHEMA_VERSION = 1;

export const PendingIntroRecordSchema = z.object({
  userId:      z.string().min(1),
  userLogin:   z.string().min(1),
  redeemId:    z.string().min(1),
  redeemUtc:   z.number().int().positive(),
  rewardTitle: z.string().min(1),
  userInput:   z.string().optional(),
  status:      z.enum(['pending', 'fulfilled', 'rejected']),
  resolvedUtc: z.number().int().positive().optional(),
});

export type PendingIntroRecord = z.infer<typeof PendingIntroRecordSchema>;
```

Record key: `redeemId` (stable per-redemption ID — per §2 key policy).

### 2. Collection instance — `Apps/info-service/src/collections/pending-intros.ts`

Follow the exact same pattern as `Apps/info-service/src/collections/user-intros.ts`. Collection name: `"pending-intros"`. Data file path: `path.resolve(__dirname, '../../data/pending-intros.json')`.

### 3. Wire into `Apps/info-service/src/index.ts`

Import `pendingIntros` from `./collections/pending-intros`. Add `await pendingIntros.load()` before `buildServer`. Pass both collections: `buildServer({ collections: [userIntros, pendingIntros] })`.

### 4. Streamer.bot redeem handler — `Actions/Intros/redeem-capture.cs`

Streamer.bot C# inline script. **Triggered by a channel-point redemption for the "Custom Intro" reward** (operator must wire the trigger in SB).

**Constants in script header:**

```csharp
private const string INFO_SERVICE_URL = "http://127.0.0.1:8766";
private const string COLLECTION_NAME  = "pending-intros";
```

**Behavior:**

1. Read from SB trigger args: `userId` (Twitch numeric userId), `userLogin` (Twitch login, lowercase), `redeemId` (channel-point redemption ID), `rewardTitle` (reward display name), `userInput` (user-supplied message — may be empty/null).
2. **Duplicate check:** GET `{INFO_SERVICE_URL}/info/pending-intros/{redeemId}`.
   - If `200`: record already exists — log "duplicate redeemId: {redeemId}", return `true` (idempotent no-op).
   - If non-200 (expected `404` for new redeems): proceed to create.
3. **Create record:** POST to `{INFO_SERVICE_URL}/info/pending-intros/{redeemId}` with JSON body:
   ```json
   {
     "userId":      "<userId>",
     "userLogin":   "<userLogin>",
     "redeemId":    "<redeemId>",
     "redeemUtc":   <DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()>,
     "rewardTitle": "<rewardTitle>",
     "userInput":   "<userInput>",
     "status":      "pending"
   }
   ```
   Omit `userInput` from the JSON if empty or null (use conditional serialization or omit key). `resolvedUtc` is omitted on creation.
4. If POST returns non-200: log error (status code + body excerpt), return `true` (no crash).
5. If POST returns `200` or `201`: log success with `redeemId` and `userId`.

**Logging:** Log every branch (`CPH.LogInfo`) with enough context for operator to trace in SB action log: userId, redeemId, URL called, response status, outcome.

**Error handling:** Wrap all HTTP calls and JSON operations in try/catch. Always return `true` — never throw or crash.

### 5. `Actions/SHARED-CONSTANTS.md`

In the §Info Service / Assets section, add:
- `COLLECTION_PENDING_INTROS` = `"pending-intros"`

Add `Actions/Intros/redeem-capture.cs` to the "Used in" list.

### 6. `.agents/_shared/info-service-protocol.md`

Add a `pending-intros` collection section parallel to the existing `user-intros` section, documenting:
- Collection name, record key (`redeemId`), schema version
- Data file path
- Field reference table (from plan §6)
- Status lifecycle: `pending → fulfilled | rejected` (per plan §6)
- Note: production-manager PM UI for fulfillment is not yet implemented (pending Q14)

## Deliverables

- Files changed:
  - `Apps/info-service/src/store/schemas/pending-intros.ts` (new)
  - `Apps/info-service/src/collections/pending-intros.ts` (new)
  - `Apps/info-service/src/index.ts` (modified — add pendingIntros load + register)
  - `Actions/Intros/redeem-capture.cs` (new — `Actions/Intros/` folder already exists if C8 ran; create folder if not)
  - `Actions/SHARED-CONSTANTS.md` (modified — COLLECTION_PENDING_INTROS + Used in)
  - `.agents/_shared/info-service-protocol.md` (modified — pending-intros section)
- Scaffolding updates: `.agents/_shared/info-service-protocol.md` pending-intros section
- Shared constants: `COLLECTION_PENDING_INTROS = "pending-intros"` added to SHARED-CONSTANTS.md
- Tests: N/A

## Forbidden in this chunk

- Changes to `Apps/production-manager/` — PM UI for pending-intros fulfillment queue is out of scope (see Open Question Q14 in COORDINATION.md; address after operator resolves)
- Modifying `user-intros` schema, collection, or index wiring beyond adding pendingIntros
- Adding new route plugins — the existing generic `readRoutes` + `writeRoutes` already handle `pending-intros` once the collection is registered
- `Docs/INFO-SERVICE-PLAN.md` modifications
- No `git commit` or `git add`

## Finish

1. Run `tsc --noEmit` in `Apps/info-service/` to verify typecheck passes.
2. Update `humans/info-service/COORDINATION.md` — C10 status → `merged`, append Run Log row (Commit: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` from `.agents/roles/ops/skills/change-summary/_index.md`, show output.
5. **Draft the next chunk's prompt file** (self-propagation):
   - Check COORDINATION.md for next chunk with status `not-started`, Prompt File `tbd`, all prereqs `merged`.
   - C9 (`MixItUp Custom Intro command spec`): prereq C8. If C8 is NOT yet merged → report C9 blocked, skip draft.
   - C11 (`Docs + scaffolding sweep`): prereqs include C9 and C10. C9 must merge first → blocked regardless.
   - If Open Question Q14 (PM pending-intros page) has been resolved and a C10.5 chunk was added to COORDINATION.md with all prereqs met → draft that prompt instead.
   - If no chunk is unblocked: note in chat which chunks are blocked and why.
   - **Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Apps/info-service/src/store/schemas/pending-intros.ts` exists with `PendingIntroRecordSchema` matching plan §6 exactly
- `Apps/info-service/src/collections/pending-intros.ts` exists, wired into `src/index.ts`
- `tsc --noEmit` passes in `Apps/info-service/`
- `Actions/Intros/redeem-capture.cs` exists: reads SB trigger args, GET duplicate check, POST creation, all branches logged, always returns `true`
- `SHARED-CONSTANTS.md` §Info Service updated with `COLLECTION_PENDING_INTROS`
- `.agents/_shared/info-service-protocol.md` has `pending-intros` collection section
- `COORDINATION.md` C10 status = `merged`, run log row appended (Commit: `uncommitted`)
- Next-chunk prompt drafted OR blocker reported in chat
- No git commit made by agent

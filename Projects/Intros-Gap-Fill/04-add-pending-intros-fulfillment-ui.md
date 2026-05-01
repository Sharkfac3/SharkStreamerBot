# Prompt 04 — Add pending-intros fulfillment UI

You are working in the SharkStreamerBot repository as `app-dev`, with awareness of the `streamerbot-dev` Intros route.

## Task

Implement the in-repo `production-manager` workflow for reviewing `pending-intros` and fulfilling/rejecting them through the local `info-service` REST API.

Known gap:

- `Actions/Intros/redeem-capture.cs` can create `pending-intros` records.
- `Apps/production-manager` currently manages `user-intros` only.
- Docs say the pending-intros fulfillment page is planned, so operators do not yet have an in-repo UI to turn pending redemptions into approved user intros.

## Required reading before editing

1. `AGENTS.md`
2. `.agents/ENTRY.md`
3. `WORKING.md`
4. `.agents/roles/app-dev/role.md`
5. `Apps/production-manager/AGENTS.md`
6. `Apps/production-manager/PRODUCTION-MANAGER-GUIDE.md`
7. `Apps/production-manager/src/pages/UserIntrosPage.tsx`
8. `Apps/info-service/AGENTS.md`
9. `Apps/info-service/INFO-SERVICE-PLAN.md` sections `user-intros`, `pending-intros`, and REST Protocol
10. `Actions/Intros/AGENTS.md` for runtime context only

## Constraints

- Keep the workflow simple and explicit.
- Do not change info-service schemas unless the operator approves.
- Do not add authentication, a database, file uploads, asset browsing, or direct audio playback.
- Do not touch Streamer.bot scripts in this task.
- Do not change public reward copy.
- Preserve stable keys: `user-intros` keyed by Twitch `userId`, `pending-intros` keyed by `redeemId`.
- If the desired fulfillment UX is ambiguous, ask before coding rather than inventing a complex workflow.

## Implementation expectation

Add a production-manager page that can:

1. List `pending-intros` records, with emphasis on `status === "pending"`.
2. Show useful fields: user login, user ID, redemption ID, user input, reward title, status, redeem time.
3. Fulfill a pending intro by:
   - creating/updating `/info/user-intros/:userId` with `userId`, `userLogin`, `soundFile`, optional `gifFile`, `enabled`, and `updatedUtc`;
   - updating `/info/pending-intros/:redeemId` to `status: "fulfilled"` and setting `resolvedUtc`.
4. Reject a pending intro by updating `/info/pending-intros/:redeemId` to `status: "rejected"` and setting `resolvedUtc`.
5. Avoid reverse transitions in the UI. If a fulfilled intro needs changes, edit it through the existing user-intros page.

Prefer the existing `UserIntrosPage.tsx` style and API patterns over introducing new abstractions.

## Validation

Run production-manager checks:

```bash
cd Apps/production-manager
npm run typecheck
npm run build
```

If these scripts differ, inspect `package.json` and run the closest targeted equivalents.

## Done criteria

- Operator can fulfill or reject pending-intros without hand-editing JSON.
- Written records validate against existing info-service schemas.
- Docs/navigation are updated only as needed for the new page.
- No unrelated app refactor.
- Handoff includes manual smoke-test steps with a sample pending record.

Ask the operator before coding if fulfillment should require both sound and gif assets, if enabled should default false instead of true, or if pending records should be hidden/deleted after resolution.

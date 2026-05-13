---
id: actions-rules
type: rules
description: Domain rules and universal script rules for all Actions/ scripts.
owner: ops
secondaryOwners: [streamerbot-dev]
parent: AGENTS.md
---

# Actions — Domain Rules

These rules govern all agent and developer work under `Actions/`. They are enforced during validation and code review. When in doubt, these rules take precedence over convenience.

## Domain Rules

- Local action-group `contracts.md` action contracts under Actions are the source of truth for how Streamer.bot action scripts must operate. Update the contract first when behavior changes, then make the `.cs` script conform to it.
- Every edited C# script under Actions must have a matching machine-readable action contract in the nearest local `contracts.md` (or legacy `AGENTS.md` fallback), plus a current `ACTION-CONTRACT` / `ACTION-CONTRACT-SHA256` stamp generated from that contract.
- If an operator requests behavior that conflicts with an existing action contract, treat the contract change as part of the same task; do not silently implement behavior that the contract does not describe.
- Keep `Actions/` focused on Streamer.bot runtime scripts and action-group docs.
- Scripts should remain pasteable into Streamer.bot `Execute C# Code` actions.
- Use Streamer.bot's `CPHInline` style unless a local guide explicitly says otherwise.
- Do not add external NuGet/package dependencies to runtime scripts.
- Preserve existing chat command names, global keys, timer names, OBS source names, and Mix It Up command IDs unless the operator explicitly asks for a migration.
- When chat output directly addresses, thanks, warns, assigns, or lists a specific Twitch user, format the name as `@username` so Twitch mention notifications/highlights work. Use the helper pattern in `Actions/Helpers/chat-input.md` and avoid `@` for role names, character names, or generic labels.
- Check `Actions/SHARED-CONSTANTS.md` before adding or renaming shared values.
- Be explicit about persisted vs. non-persisted globals when using `CPH.SetGlobalVar`.
- Prefer small, local changes over broad refactors; live stream reliability comes first.
- Avoid duplicate helper implementations when a pattern already exists in `Actions/Helpers/`.

## Universal Script Rules

These apply to every .cs script under Actions/ and are not restated in local guides:

- Scripts are self-contained: do not assume shared runtime files can be imported at runtime.
- Read runtime state defensively via `CPH.TryGetArg` or Streamer.bot globals; protect against missing or malformed inputs.

## Boundaries

- App implementation belongs in `Apps/`, not `Actions/`.
- Mix It Up export/import tooling belongs in `Tools/MixItUp/`, not `Actions/`.
- Streamer.bot support tooling belongs in `Tools/StreamerBot/`, not `Actions/`.
- Brand/canon/story/art content belongs in `Creative/` unless it is embedded runtime copy for an action.
- Repo-wide workflow and architecture docs belong in `.agents/`.

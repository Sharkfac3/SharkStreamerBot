---
id: custom-intro-hardening-progress
type: project-progress
description: Progress tracker for the custom intro hardening project.
owner: ops
status: active
---

# Progress — Custom Intro Hardening

| Prompt | Status | Notes |
|---|---|---|
| `first-chat-prompts/01-audit-current-state.md` | done | Audit note added; current flow mostly matches target, with stale-variable hardening and live wrapper/Mix It Up specs still needed. |
| `first-chat-prompts/02-harden-first-chat-resolver.md` | done | Hardened `first-chat-intro.cs` as the authoritative gatekeeper: clears stale globals up front, normalizes filename-only asset values, logs dispatch/no-op reasons, and aligned Intros contract/docs. |
| `first-chat-prompts/03-spec-streamerbot-wrapper.md` | done | Added `Actions/Intros/play-custom-intro-action-spec.md` covering wrapper purpose, input/variable contract, no-op/dispatch rules, stale-global snapshot+clear handling, and Streamer.bot operator build steps. |
| `first-chat-prompts/04-spec-mixitup-command.md` | done | Hardened repo-tracked `Custom Intro` Mix It Up contract/setup docs: documented sound/gif/no-op behavior, expected variables, simple-branching/stale-variable boundaries, and local operator setup steps. |
| `first-chat-prompts/05-final-validation-and-handoff.md` | done | Completed final consistency pass; no contract drift found, added final operator handoff, and ran targeted repo validation across the hardened intro docs/scripts. |

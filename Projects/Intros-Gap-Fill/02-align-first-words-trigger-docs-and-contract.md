# Prompt 02 — Align First Words trigger docs and runtime contract

You are working in the SharkStreamerBot repository as `streamerbot-dev`.

## Task

Make the repo's Intros documentation and action contract language consistently describe the playback trigger as Streamer.bot **Twitch → Chat → First Words**, while preserving the user-facing concept of "first chat of the stream" where helpful.

Known gap:

- `Actions/Intros/first-chat-intro.cs` and docs refer to "First Chat".
- The trigger catalog documents the actual Streamer.bot trigger as `Twitch -> Chat -> First Words`.
- The catalog notes First Words reset timing is configurable and recommends resetting First Words on Stream Online for per-stream behavior.

## Required reading before editing

1. `AGENTS.md`
2. `.agents/ENTRY.md`
3. `WORKING.md`
4. `.agents/roles/streamerbot-dev/role.md`
5. `Actions/AGENTS.md`
6. `Actions/Intros/AGENTS.md`
7. `Actions/Helpers/triggers/twitch/chat.md#first-words`
8. `Actions/Twitch Core Integrations/stream-start.cs` only to inspect whether First Words reset is already present; do not modify it unless the operator explicitly approves scope expansion.

## Constraints

- This is a documentation/contract alignment task, not a runtime behavior change.
- Do not create external Streamer.bot setup.
- Do not change Mix It Up setup.
- Do not edit `stream-start.cs` unless you find a direct contradiction and ask the operator first.
- Keep wording precise: Streamer.bot trigger name is **First Words**; desired behavior is first chat/first words per stream if reset is configured.

## Implementation expectation

- Update `Actions/Intros/AGENTS.md` references from ambiguous "First Chat" to explicit `Twitch -> Chat -> First Words` where trigger wiring is discussed.
- If action contracts already exist when you start, update only the contract fields relevant to trigger naming/reset expectations.
- If action contracts do not exist, do not create the full contract block in this task unless the operator says to merge with the contract task. Instead, note that Prompt 05 handles it.
- Keep `first-chat-intro.cs` filename unchanged unless explicitly asked.

## Validation

For docs-only changes, run:

```bash
python3 Tools/AgentTree/validate.py
```

If action contracts are touched, also run the targeted action contract validator for `first-chat-intro.cs`.

## Done criteria

- Repo docs no longer obscure the Streamer.bot trigger name.
- Handoff clearly tells the operator that per-stream behavior requires First Words reset configuration in Streamer.bot.
- No external setup is implemented or claimed as verified.
- No unrelated edits.

Ask the operator before coding if you discover the current Streamer.bot version uses a different trigger name than the repo catalog.

# Prompt: Thin comments in `Actions/Twitch Core Integrations/subscription-new.cs`

You are working in the SharkStreamerBot repo as `streamerbot-dev`.

Target script:
- `Actions/Twitch Core Integrations/subscription-new.cs`

Source-of-truth / routing docs:
- `.agents/ENTRY.md`
- `WORKING.md`
- `.agents/roles/streamerbot-dev/role.md`
- `Actions/AGENTS.md`
- `Actions/Twitch Core Integrations/AGENTS.md`
- `Actions/SHARED-CONSTANTS.md`
- `Actions/HELPER-SNIPPETS.md` if reusable patterns are relevant

Primary behavior reference for this script:
- `Actions/Twitch Core Integrations/AGENTS.md` (local action contract/source-of-truth)

Task:
Thin the comments in `Actions/Twitch Core Integrations/subscription-new.cs` so the script does not duplicate behavior already described in `Actions/Twitch Core Integrations/AGENTS.md`. The behavior docs are the source of truth; the script should reference them instead of restating long narrative explanations.

Requirements:
1. Read the source-of-truth / routing docs listed above before editing.
2. Check `WORKING.md` for conflicts, then register this task while editing.
3. Edit only `Actions/Twitch Core Integrations/subscription-new.cs` unless you discover the behavior reference is missing or materially wrong.
4. If the behavior reference is wrong, update `Actions/Twitch Core Integrations/AGENTS.md` first. If the script uses a machine-readable action contract in an `AGENTS.md`, refresh the action-contract stamp after contract changes.
5. Preserve runtime behavior exactly. Do not change constants, globals, trigger assumptions, broker topics, OBS names, timer names, Mix It Up IDs, chat output, or control flow.
6. Keep any existing `ACTION-CONTRACT` and `ACTION-CONTRACT-SHA256` header intact unless the contract changes and you intentionally re-stamp it.
7. Replace large explanatory header/section comments with brief references such as:
   - `// Runtime source of truth: Actions/Twitch Core Integrations/AGENTS.md`
   - `// Shared names/constants reference: Actions/SHARED-CONSTANTS.md`
8. Keep only comments that help a maintainer navigate the code or avoid a non-obvious implementation mistake. Prefer short section labels over duplicated behavioral explanations.
9. Do not remove comments that explain genuinely non-obvious Streamer.bot compatibility constraints unless that explanation already exists in the local behavior docs or shared helper docs.
10. Validate after editing with:
   - If this script has or gains an `ACTION-CONTRACT` header, run the matching `Tools/StreamerBot/Validation/action_contracts.py --script ...` validation. If it has no contract header, do a careful manual paste-readiness review instead.
11. Remove your `WORKING.md` active row and add a Recently Completed entry when done.

Final response:
- Summarize the comment-thinning changes.
- State that runtime behavior was unchanged.
- Include validation/review output.
- List paste target: `Actions/Twitch Core Integrations/subscription-new.cs`.

# Prompt 05 — Add Intros action contracts and stamps

You are working in the SharkStreamerBot repository as `streamerbot-dev` with an `ops` validation mindset.

## Task

Add machine-readable action contracts for the two Intros C# scripts and stamp the scripts so the repo's action-contract validator passes for `Actions/Intros/`.

Known gap:

- `Actions/Intros/AGENTS.md` has no `<!-- ACTION-CONTRACTS:START -->` block.
- `Actions/Intros/first-chat-intro.cs` and `Actions/Intros/redeem-capture.cs` have no `ACTION-CONTRACT` / `ACTION-CONTRACT-SHA256` stamps.
- `python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/first-chat-intro.cs"` currently fails for that reason.

## Required reading before editing

1. `AGENTS.md`
2. `.agents/ENTRY.md`
3. `WORKING.md`
4. `.agents/roles/streamerbot-dev/role.md`
5. `Actions/AGENTS.md` section `Action Contracts`
6. `Actions/Intros/AGENTS.md`
7. `Actions/Intros/first-chat-intro.cs`
8. `Actions/Intros/redeem-capture.cs`
9. `Actions/Helpers/triggers/twitch/chat.md#first-words`
10. `Actions/Helpers/triggers/twitch/channel-reward.md#reward-redemption`
11. `Actions/SHARED-CONSTANTS.md` section `Info Service / Assets`
12. `Tools/StreamerBot/Validation/action_contracts.py` only if you need validator behavior details

## Constraints

- This task is contract/validation only unless the operator explicitly combines it with a runtime fix.
- Do not change runtime behavior in the C# scripts except inserting/updating contract stamp comments.
- Do not invent external setup verification. Contracts may name paste targets and dependencies, but do not claim they exist.
- Contract contents must match current code at the time you edit. If another prompt has already fixed runtime gaps, reflect the fixed behavior. If not, ask the operator whether to encode current behavior or wait until runtime fixes land.
- Keep contracts small, precise, and useful. No essay inside JSON.

## Implementation expectation

1. Add an `ACTION-CONTRACTS` JSON block to `Actions/Intros/AGENTS.md` for:
   - `first-chat-intro.cs`
   - `redeem-capture.cs`
2. Include accurate fields for:
   - `script`
   - `action`
   - `purpose`
   - `triggers`
   - `globals`
   - `timers`
   - `obsSources`
   - `obsScenes`
   - `mixItUpCommandIds`
   - `overlayTopics`
   - `serviceUrls`
   - `requiredLiterals`
   - `runtimeBehavior`
   - `failureBehavior`
   - `pasteTarget`
3. Run the validator with `--stamp` for both scripts.
4. Re-run validation without `--stamp`.

Use trigger names from the repo catalog:

- `Twitch -> Chat -> First Words`
- `Twitch -> Channel Reward -> Reward Redemption`

## Validation

Run:

```bash
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/first-chat-intro.cs" --stamp
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/redeem-capture.cs" --stamp
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/first-chat-intro.cs"
python3 Tools/StreamerBot/Validation/action_contracts.py --script "Actions/Intros/redeem-capture.cs"
python3 Tools/AgentTree/validate.py
```

## Done criteria

- `Actions/Intros/AGENTS.md` contains valid contracts for both scripts.
- Both scripts contain current action-contract stamps.
- Targeted action-contract validation passes.
- No runtime logic changed.
- Handoff lists paste targets and notes that external Streamer.bot/Mix It Up setup remains unverified.

Ask the operator before coding if current runtime behavior conflicts with the desired behavior from Prompts 01–03.

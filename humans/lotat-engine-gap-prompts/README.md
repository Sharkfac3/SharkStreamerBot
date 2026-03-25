# LotAT Engine Gap Prompts

This folder contains copy/paste prompts for resolving one LotAT engine ambiguity at a time.

## Goal

Use these prompts in separate chats so each agent focuses on a single bounded question before making any project changes.

## Critical operator rule

Every prompt in this folder tells the agent to:
- load the required project context first
- understand the existing LotAT contract before proposing changes
- ask clarifying questions **before editing any files**
- avoid blind schema drift or runtime-contract drift

If an agent starts changing files before asking questions, stop that chat and restart with the prompt.

## Suggested workflow

1. Pick one ambiguity prompt.
2. Paste it into a fresh chat.
3. Let the agent read the listed files.
4. Answer its clarifying questions.
5. Only after alignment, ask it to propose or implement the change.

## Included prompts

1. `01-chaos-outcome-contract.md`
2. `02-dice-hook-runtime-contract.md`
3. `03-commander-moment-runtime-contract.md`
4. `04-timer-contract.md`
5. `05-runtime-state-variable-contract.md`
6. `06-runtime-storage-shape.md`
7. `07-story-loading-and-validation-flow.md`
8. `08-hard-vs-soft-schema-rules.md`
9. `09-supported-mechanics-semantics.md`
10. `10-operator-recovery-controls.md`
11. `11-validation-failure-handling.md`
12. `12-offering-integration-boundary.md`

## Notes

- These prompts are for the `lotat-tech` role unless the chat explicitly needs follow-on implementation work from `streamerbot-dev`.
- Most of these questions are runtime-contract or schema-boundary questions, not story-writing tasks.
- If a prompt would cause a breaking schema change, the agent should call that out and ask before proceeding.

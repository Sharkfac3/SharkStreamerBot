# Intros Gap Fill Prompt Pack

Purpose: focused prompts for agents to close known Intros gaps without touching unverifiable external setup.

Use one prompt per agent/task. Each prompt is intentionally narrow. If an agent discovers an unforeseen conflict, schema mismatch, Streamer.bot API ambiguity, or behavior decision not covered by the prompt, it should stop and ask the operator before coding.

## Prompts

1. [`01-fix-redeem-capture-trigger-args.md`](01-fix-redeem-capture-trigger-args.md) — fix runtime arg names for Custom Intro redemption capture.
2. [`02-align-first-words-trigger-docs-and-contract.md`](02-align-first-words-trigger-docs-and-contract.md) — make first-chat/First Words wiring expectations explicit in repo docs/contracts.
3. [`03-add-first-chat-sound-file-guard.md`](03-add-first-chat-sound-file-guard.md) — no-op safely when approved intro audio file is missing.
4. [`04-add-pending-intros-fulfillment-ui.md`](04-add-pending-intros-fulfillment-ui.md) — implement the in-repo operator UI workflow for pending intros.
5. [`05-add-intros-action-contracts.md`](05-add-intros-action-contracts.md) — add machine-readable action contracts and stamps for Intros scripts.

## Explicitly out of scope for this pack

- Creating or validating the Streamer.bot action `Intros - Play Custom Intro`.
- Creating or validating the Mix It Up command `Custom Intro`.
- Verifying local audio playback outside the repository.
- Changing public reward copy or policy unless the operator explicitly asks.

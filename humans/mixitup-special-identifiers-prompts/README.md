# Mix It Up Special Identifier Backfill Prompts

This folder contains human-ready prompts for asking an agent to backfill Mix It Up `SpecialIdentifiers` in action scripts that currently send `{ }` or need richer structured payloads.

Use one prompt at a time. Paste the prompt into the coding agent from the repo root.

## Prompt Index

### Highest-priority stubs
- `follower-new.md`
- `subscription-counter-rollover.md`
- `hype-train-start.md`
- `hype-train-level-up.md`
- `hype-train-end.md`
- `on-screen-celebration.md`

### Empty-special-identifier scripts with existing behavior
- `bits-tier-1.md`
- `bits-tier-2.md`
- `bits-tier-3.md`
- `bits-tier-4.md`
- `message-effects.md`
- `explain-current-task.md`

## Naming convention requested

Use prefixes that make the Mix It Up side obvious:

- Follow: `follow*`
- Sub counter rollover: `sub*`
- Hype train: `hypetrain*`
- Bits readouts: `bits*`
- Message effects: `messageeffects*`
- On-screen celebration: `celebration*`
- Explain current task: `explaintask*`

Avoid dots in keys even when Streamer.bot args contain dots, e.g. map `top.bits.user` to `hypetraintopbitsuser`.

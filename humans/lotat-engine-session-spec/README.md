# LotAT Engine Session Spec Prompt Pack

This folder contains **manual copy/paste prompts** for creating or refining the LotAT runtime-session scaffolding in fresh AI chats.

Use this pack when you want an agent to work on the **LotAT engine operating contract** without writing C# or changing story JSON.

## Goal

Document and maintain the recommended LotAT runtime spec for:
- join-phase behavior
- `!join` participation
- participant roster rules
- decision-window voting behavior
- early-close when all joined users have voted
- runtime stages, edge cases, and recovery expectations

## Important boundary

These prompts are for **agent scaffolding and documentation only**.

They are **not** for:
- writing C# scripts
- editing `Actions/`
- changing authored story JSON structure
- inventing new story fields

## Suggested order

1. `01-lotat-tech-session-lifecycle.md`
2. `02-lotat-tech-state-and-voting.md`
3. `03-lotat-tech-navigation-alignment.md`
4. `04-scaffolding-review.md`

## Expected outcome

After running this prompt sequence, the project should have:
- a clear LotAT session-lifecycle spec in `.agents/roles/lotat-tech/skills/engine/`
- a clear participant/voting spec in `.agents/roles/lotat-tech/skills/engine/`
- navigation hooks so future agents can find those docs quickly
- no accidental drift into story-schema changes or C# implementation work

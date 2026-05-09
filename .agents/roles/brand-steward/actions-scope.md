---
id: brand-steward-actions-scope
type: scope
description: Scoped reading list for brand-steward reviewing Actions scripts.
owner: brand-steward
parent: role.md
---

# brand-steward — Actions Scope

brand-steward reviews Actions scripts for public-facing copy, tone, and naming — not for C# correctness. Load only what you need for review.

## For Any Actions Review

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry
2. [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — understand what brand-steward reviews vs. what streamerbot-dev owns
3. The local `AGENTS.md` for the folder being reviewed — check the "Secondary Owners" section for brand-steward responsibilities

## What brand-steward Reviews in Actions

- Public chat output text (CPH.SendMessage calls, formatted strings)
- TTS content
- Reward/command names (channel point names, !command names)
- Commander character copy (Captain Stretch, The Director, Water Wizard)

## What brand-steward Does NOT Review

- C# logic, globals, timer names, OBS source names
- Contract JSON structure or SHA256 stamps
- Trigger configuration

## Domain Rules for Review Work

See [Actions/RULES.md](../../../Actions/RULES.md) — specifically the rules about public chat formatting and @mention conventions.

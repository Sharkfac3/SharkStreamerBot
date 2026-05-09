---
id: actions-ownership
type: ownership
description: Role ownership model for all Actions/ scripts and scaffolding.
owner: ops
secondaryOwners: [streamerbot-dev, brand-steward]
parent: AGENTS.md
---

# Actions — Ownership Model

This file defines who owns what under `Actions/`. Per-folder ownership is defined in each folder's AGENTS.md. This file covers domain-level shared ownership rules.

## Shared Ownership Rules

`streamerbot-dev` owns all C# runtime behavior under `Actions/` by default.

`brand-steward` review is required before any change to: public chat output, TTS/spoken text,
overlay copy, command names visible to chat, or event announcement wording — across all folders.
Local AGENTS.md files only list secondary owners when the folder adds exceptions to this rule.

`ops` handles validation, paste/sync workflow, and agent-tree maintenance across all folders.

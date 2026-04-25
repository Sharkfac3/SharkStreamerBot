---
name: meta-agents-navigate
description: How to navigate and use the .agents/ project knowledge tree. Load when you need to find context for a task or explore what's available in the agent tree.
---

# Navigating .agents/

## Entry Point

Always start at `.agents/ENTRY.md`. It contains:
- The project description
- The full roles table (which role handles which tasks)
- Pointers to `_shared/` and each role folder

## Navigation Pattern

1. Read `ENTRY.md` → identify the role for your task
2. Read `roles/<role>/role.md` → confirm scope, check trigger/anti-trigger conditions
3. Read `roles/<role>/skills/core.md` → always required for that role
4. Navigate into sub-skill folders only if the task demands it:
   - Read `_index.md` files to understand what a folder contains before going deeper
   - Load only the specific skill files the task requires — don't load everything
5. Check `roles/<role>/context/` for living notes left by previous agents

## The `_index.md` Convention

Every skill subfolder has an `_index.md` that acts as a navigation header:
- Read `_index.md` to understand what's in the folder
- Decide if you need to go deeper into specific files
- This keeps token cost low — you only load what you need

## Shared Context

`_shared/` contains cross-role knowledge. Read these when relevant:
- `project.md` — project identity, domains, priorities
- `conventions.md` — git, naming, routing rules
- `coordination.md` — WORKING.md protocol, conflict rules
- `mixitup-api.md` — Mix It Up API payload spec
- `info-service-protocol.md` — info-service REST routes, collection schemas, error conventions

## Role → Skill Quick Map

| You're working on | Start here |
|---|---|
| Any `.cs` script | `roles/streamerbot-dev/` |
| LotAT C# engine | `roles/lotat-tech/` |
| LotAT story content | `roles/lotat-writer/` |
| Art / diffusion prompts | `roles/art-director/` |
| Chat text, titles, canon | `roles/brand-steward/` |
| Stream interaction apps | `roles/app-dev/` |
| Validation, sync, change summary | `roles/ops/` |

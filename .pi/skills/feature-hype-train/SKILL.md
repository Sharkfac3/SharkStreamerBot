---
name: feature-hype-train
description: Twitch Hype Train event scripts (start, level-up, end). Load when working on any script under Actions/Twitch Hype Train/.
---

# Feature: Hype Train

## Scope

| Script | Path | Trigger |
|---|---|---|
| Hype Train Start | `Actions/Twitch Hype Train/hype-train-start.cs` | Hype Train → Start |
| Hype Train Level Up | `Actions/Twitch Hype Train/hype-train-level-up.cs` | Hype Train → Level Up |
| Hype Train End | `Actions/Twitch Hype Train/hype-train-end.cs` | Hype Train → End |

## Detailed Docs

- `Actions/Twitch Hype Train/README.md`

**Read the README before making changes.**

## Current State

All three scripts are **stubs** — they call a Mix It Up command but pass empty `Arguments` and `SpecialIdentifiers`. The `BuildArguments()` and `BuildSpecialIdentifiers()` methods are placeholders with a comment to expand once the final event field contract is decided.

When expanding a script to use trigger variables, add them to `BuildArguments()` or `BuildSpecialIdentifiers()` as described in the operator notes inside each script.

## Behavioral Expectations

- Keep hype train scripts lightweight — they are notification bridges, not stateful mini-games.
- All three scripts must skip the Mix It Up call gracefully when the command ID is still a placeholder.
- Log warnings instead of throwing exceptions so the action queue stays stable.
- Do not introduce OBS interactions unless explicitly requested.

## Trigger Variables

Full trigger variable reference for all three events: `Actions/Twitch Hype Train/README.md`

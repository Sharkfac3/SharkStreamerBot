# Twitch Hype Train

## Scripts

| Script | Path | Trigger |
|---|---|---|
| `hype-train-start.cs` | `Actions/Twitch Hype Train/` | Hype Train → Start |
| `hype-train-level-up.cs` | `Actions/Twitch Hype Train/` | Hype Train → Level Up |
| `hype-train-end.cs` | `Actions/Twitch Hype Train/` | Hype Train → End |

## Current State — Stubs

All three scripts are stubs. They call a Mix It Up command but pass empty `Arguments` and `SpecialIdentifiers`. `BuildArguments()` and `BuildSpecialIdentifiers()` are placeholders pending final event field contracts.

When expanding to use trigger variables, add them to `BuildArguments()` or `BuildSpecialIdentifiers()`.

## Behavioral Expectations

- Keep scripts lightweight — they are notification bridges, not stateful mini-games
- All three must skip the Mix It Up call gracefully when command ID is still a placeholder
- Log warnings instead of throwing exceptions (keeps action queue stable)
- Do not introduce OBS interactions unless explicitly requested

## Trigger Variables

Full reference for all three events: `Actions/Twitch Hype Train/README.md`

---
id: actions-helper-mini-game-contract
type: reference
description: Checklist for Streamer.bot mini-game lock usage, terminal paths, and documentation updates.
owner: streamerbot-dev
secondaryOwners:
  - ops
status: active
---

## Required mini-game contract checklist
1. Add lock constants + acquire/release methods.
2. Acquire lock at the mini-game start action.
3. Release lock on every terminal path (win/loss/timeout/cancel/manual stop/guard exits).
4. Add chat feedback when blocked by another active mini-game.
5. For single-action mini-games, use `finally` to guarantee lock release.
6. Add/update docs in feature README and `Actions/SHARED-CONSTANTS.md` when needed.

## Related references

- [Mini-game Lock Helper](mini-game-lock.md)
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md)


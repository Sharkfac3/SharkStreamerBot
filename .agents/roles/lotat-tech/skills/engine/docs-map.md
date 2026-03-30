# LotAT Engine — Docs Map

## Purpose

This is the high-level navigation map for LotAT engine documentation.

Use this file first when you know the task is about the LotAT runtime engine, but you are not yet sure which engine doc contains the answer.

The canonical **runtime session spec** currently lives in:
- `session-lifecycle.md` — session stages, transitions, teardown, and unattended fail-closed behavior
- `state-and-voting.md` — participant roster, vote tracking, early-close rules, and timeout/abort edge cases

## Recommended Reading Order

1. `../core.md`
   - role-wide technical principles
   - schema/engine separation rules
   - shared state-management expectations

2. `_index.md`
   - quick overview of what the engine does
   - current status of engine docs in this folder

3. `commands.md`
   - authored decision commands vs runtime session commands
   - `!join` contract
   - command-category boundary

4. `session-lifecycle.md`
   - **canonical runtime session contract** for LotAT live play
   - runtime stages and allowed transitions
   - join open/close behavior
   - node-entry and decision-window flow
   - zero-join, unresolved, fault-abort, and end-of-session behavior
   - unattended fail-closed rules for v1

5. `state-and-voting.md`
   - participant identity rules
   - roster rules
   - per-node vote storage
   - early-close behavior
   - tie-break behavior
   - timeout/abort edge cases

## Which Doc to Read for Which Question

### “How does a LotAT session start and progress on stream?”
Read:
- `session-lifecycle.md`
- `commands.md`

### “What are the canonical runtime stages?”
Read:
- `session-lifecycle.md`

### “How does `!join` work?”
Read:
- `commands.md`
- `session-lifecycle.md`
- `state-and-voting.md`

### “Who counts as a participant?”
Read:
- `state-and-voting.md`
- `session-lifecycle.md`

### “What happens if nobody joins?”
Read:
- `session-lifecycle.md`

### “When should a decision window auto-close?”
Read:
- `state-and-voting.md`
- `session-lifecycle.md`

### “How does LotAT recover safely without operator intervention in v1?”
Read:
- `session-lifecycle.md`
- `state-and-voting.md`

### “Is this a schema change or only runtime behavior?”
Read:
- `../core.md`
- `../story-pipeline/json-schema.md`
- `commands.md`
- `session-lifecycle.md`
- `state-and-voting.md`

### “Can this go into story JSON?”
Usually no for runtime/session concerns.
Read:
- `../story-pipeline/json-schema.md`
- `session-lifecycle.md`
- `state-and-voting.md`

### “I need to implement this in Streamer.bot later. What should I read first?”
Read in this order:
- `docs-map.md`
- `commands.md`
- `session-lifecycle.md`
- `state-and-voting.md`
- then `.agents/roles/streamerbot-dev/skills/lotat/_index.md`

## Runtime vs Story Boundary

Keep this boundary explicit:

### Runtime engine concerns
Belong in engine docs like this folder:
- session stages
- join windows
- participant rosters
- vote tracking
- timer behavior
- auto-close logic
- unattended fail-closed session behavior
- zero-join teardown

### Story/schema concerns
Belong in story-pipeline docs:
- top-level JSON fields
- node structure
- authored commands in `choices[].command`
- story content contract

## Current Runtime Assumptions Captured Here

The current documented LotAT runtime contract assumes:
- every session begins with a join phase
- the engine loads exactly one runtime story file in v1: `Creative/WorldBuilding/Storylines/loaded/current-story.json`
- the engine does not scan `ready/` directly
- a single start trigger is the only planned operator control in v1
- viewers join with `!join`
- roster creation is session-scoped and runtime-owned
- the participant roster freezes when the join phase closes if at least one user joined
- zero joins ends the session as a normal non-error outcome instead of entering story play
- only joined users count for the "everyone has voted" rule
- decision windows may close early when all joined users have voted
- a decision timer with zero valid votes ends the run unresolved in v1
- true runtime/code faults fail closed and return the engine to `idle`
- runtime story load must succeed before `join_open` begins
- runtime stages progress through `idle` / `join_open` / `node_intro` / `decision_open` / `decision_resolving` / `ended`
- these are runtime rules, not authored story fields


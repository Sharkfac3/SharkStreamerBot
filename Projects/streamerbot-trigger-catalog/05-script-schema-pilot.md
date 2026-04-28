---
id: trigger-catalog-phase-05-script-schema-pilot
type: project-phase
description: Pilot the new ## Args Consumed schema by refactoring Actions/Twitch Core Integrations/README.md from canonical Trigger Variables tables to per-script consumption tables.
status: active
owner: streamerbot-dev
phase: 5
depends_on:
  - 03-twitch-content.md
---

# Phase 5 — Script-Side Schema Pilot

## Goal

Refactor `Actions/Twitch Core Integrations/README.md` from "Trigger Variables" canonical-arg blocks to per-script `## Args Consumed` tables that describe **what each `.cs` actually reads and why**. Lock the schema before fanning out to the rest of the repo (Phase 14).

This README owns 7 of 8 Twitch Subscription scripts plus Follow + Watch Streak + Sub Counter Rollover — densest possible stress test of the new pattern.

## Why

Today the README's `## Trigger Variables` section duplicates upstream args and implies it's canonical. After Phase 3, the canonical args live in `Actions/Helpers/triggers/twitch/subscriptions.md` (and `chat.md`, `channel.md`). The README should describe **consumption**, not enumerate the upstream surface.

## Prerequisites

1. Read `Projects/streamerbot-trigger-catalog/README.md` — especially the script-side `Args Consumed` schema.
2. Phase 3 complete (Twitch catalog seeded). Verify by checking `Actions/Helpers/triggers/twitch/subscriptions.md#sub-counter-rollover` has args.
3. Coordination check via [WORKING.md](../../WORKING.md).
4. Read [feedback_one_script_per_action.md](../../../.claude/projects/...) reminder if accessible: do not consolidate scripts even when they look similar. Preserve one-`.cs`-per-action structure.

## Procedure

For each `## Script:` section in `Actions/Twitch Core Integrations/README.md`:

### Step 1. Identify which args the script actually reads

Open the `.cs` file. Grep for:

```bash
rg -n "TryGetArg|args\." "Actions/Twitch Core Integrations/<script>.cs"
```

Note every arg key consumed plus what it's used for downstream (Mix It Up special identifier, branching condition, log line, etc.).

### Step 2. Replace existing per-script content

Inside each `## Script: <name>.cs` block:

- Remove the script-specific arg duplication from any inline lists if present.
- Add or rewrite the `### Trigger` subsection:

  ```markdown
  ### Trigger
  - Source: Twitch -> <Subcategory> -> <Trigger Name>
  - Catalog: [Helpers/triggers/twitch/<subcategory>.md#<trigger-slug>](../Helpers/triggers/twitch/<subcategory>.md#<trigger-slug>)
  ```

- Add or rewrite the `### Args Consumed` subsection per the schema:

  ```markdown
  ### Args Consumed

  | Arg | Used as | Purpose |
  |---|---|---|
  | user | `subuser` SI | Mix It Up alert recipient name. |
  | userId | `subuserid` SI | Mix It Up identifier for subscriber. |
  | tier | `subtier` SI | Mix It Up branch by tier. |
  | isMultiMonth | `subismultimonth` SI | Mix It Up branch on multi-month flag. |
  ```

  Only list args the `.cs` actually reads. Do not include args that exist upstream but the script ignores.

### Step 3. Strip the bottom-of-file `## Trigger Variables` section

That block currently re-documents upstream args for Cheer, Follow, all Subscriptions, etc. Remove it entirely. Replace with:

```markdown
## Trigger Variables

The full upstream args list for each trigger now lives in the canonical catalog. See:

- [triggers/twitch/subscriptions.md](../Helpers/triggers/twitch/subscriptions.md) — Sub, Resub, Gift, Gift Bomb, Upgrades, Pay It Forward, Sub Counter Rollover.
- [triggers/twitch/channel.md#follow](../Helpers/triggers/twitch/channel.md#follow) — Follow.
- [triggers/twitch/chat.md#watch-streak](../Helpers/triggers/twitch/chat.md#watch-streak) — Watch Streak.

Each script section above has its own `### Args Consumed` table describing the subset that script actually uses.
```

### Step 4. Validate per-script consumption matches code

For each refactored script section, run:

```bash
rg -n "TryGetArg" "Actions/Twitch Core Integrations/<script>.cs"
```

Cross-check every returned arg key has a row in the script's `### Args Consumed` table. Any missing row = bug. Any extra row (in README but not in code) = also a bug; remove it.

## Schema decisions to lock during this phase

After completing the refactor, write a `Projects/streamerbot-trigger-catalog/schema-decisions.md` short note capturing:

- Final shape of the `### Trigger` / `### Args Consumed` blocks.
- Any unexpected ergonomics (e.g. how to handle scripts wired to two triggers — `subscription-gift.cs` is one of these).
- Anything Phase 14 fanout should know.

This note feeds Phase 14.

## Validation

1. Every `## Script:` section in `Actions/Twitch Core Integrations/README.md` has both `### Trigger` and `### Args Consumed`.
2. No script's `### Args Consumed` table contains an arg the `.cs` doesn't actually read (grep cross-check).
3. The bottom-of-file `## Trigger Variables` block is replaced with the catalog-pointer stub.
4. All catalog links resolve (open them once each).
5. `schema-decisions.md` exists in the project folder.

## Exit

- Dirty tree. Do not commit.
- Change summary lists: scripts refactored, catalog links added, `schema-decisions.md` highlights.

## Next phase

After operator review of the pilot, Phase 14 (`14-script-schema-fanout.md`) applies the locked schema to all remaining feature READMEs.

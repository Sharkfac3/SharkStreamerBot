---
id: trigger-catalog-phase-14-script-schema-fanout
type: project-phase
description: Apply the Phase 5 ## Args Consumed schema to every remaining feature README and .cs header so all script docs align with the catalog.
status: active
owner: streamerbot-dev
phase: 14
depends_on:
  - 05-script-schema-pilot.md
---

# Phase 14 — Script Schema Fanout

## Goal

Apply the locked-in `## Args Consumed` schema (validated in Phase 5 on Twitch Core Integrations) to every remaining feature README and `.cs` header across the repo. End state: no feature doc duplicates the canonical args list — every script doc describes only what it consumes and points to the catalog for the full set.

## Prerequisites

1. Phase 5 complete and operator-approved.
2. Read `Projects/streamerbot-trigger-catalog/schema-decisions.md` (produced in Phase 5) — captures any unexpected ergonomics from the pilot.
3. Phases 3–4 (and ideally 6–13) complete so every script's catalog target exists.
4. Coordination check via [WORKING.md](../../WORKING.md).
5. Memory check: do not consolidate scripts even when they look similar. One `.cs` per Streamer.bot action stays the rule.

## Feature READMEs to refactor

Each currently carries some flavor of `## Trigger Variables` block, or per-script arg duplication, that should be replaced with `### Trigger` + `### Args Consumed` per script and a catalog-pointer stub at the bottom of the file:

- [Actions/Twitch Bits Integrations/README.md](../../Actions/Twitch%20Bits%20Integrations/README.md) — bits-tier-1..4, gigantify-emote, message-effects, on-screen-celebration.
- [Actions/Twitch Channel Points/README.md](../../Actions/Twitch%20Channel%20Points/README.md) — explain-current-task, disco-party, etc.
- [Actions/Twitch Hype Train/README.md](../../Actions/Twitch%20Hype%20Train/README.md) — hype-train-start, hype-train-level-up, hype-train-end.
- [Actions/Squad/README.md](../../Actions/Squad/README.md) — Squad triggers (mostly Commands).
- [Actions/Squad/Toothless/README.md](../../Actions/Squad/Toothless/README.md), [Squad/Pedro/README.md](../../Actions/Squad/Pedro/README.md), [Squad/Duck/README.md](../../Actions/Squad/Duck/README.md), [Squad/Clone/README.md](../../Actions/Squad/Clone/README.md).
- [Actions/Commanders/README.md](../../Actions/Commanders/README.md) and per-commander subfolder READMEs (Captain Stretch, The Director, Water Wizard).
- [Actions/Voice Commands/README.md](../../Actions/Voice Commands/README.md).
- [Actions/Rest Focus Loop/README.md](../../Actions/Rest%20Focus%20Loop/README.md).
- [Actions/Twitch Core Integrations/README.md](../../Actions/Twitch%20Core%20Integrations/README.md) — already done in Phase 5; verify nothing regressed.
- [Actions/Overlay/README.md](../../Actions/Overlay/README.md) — broker-* scripts (likely WebSocket triggers, possibly Custom).
- [Actions/LotAT/README.md](../../Actions/LotAT/README.md) and the LotAT operator/runtime/implementation docs.
- [Actions/Intros/README.md](../../Actions/Intros/README.md) if present (`first-chat-intro.cs`, `redeem-capture.cs`).
- [Actions/XJ Drivethrough/README.md](../../Actions/XJ%20Drivethrough/README.md) if present.
- [Actions/Destroyer/README.md](../../Actions/Destroyer/README.md) if present.
- [Actions/Twitch Bits Integrations/README.md](../../Actions/Twitch%20Bits%20Integrations/README.md).

(Confirm the list by `find Actions -name README.md` and walking each one — do not skip any feature folder with a README.)

## Procedure

For each README in the list:

### Step 1. Inventory scripts

For each `## Script: <name>.cs` (or equivalent section) in the README, locate the matching `.cs` file.

### Step 2. Identify consumed args

```bash
rg -n "TryGetArg|args\." "Actions/<folder>/<script>.cs"
```

Note every arg key consumed and its downstream use (Mix It Up SI, branching, log).

### Step 3. Identify the trigger source

Read the script's `Expected Trigger / Input` header section. Cross-reference with the catalog to find the canonical entry. If the entry is missing in the catalog, that's a catalog bug — fix the catalog first, then return.

### Step 4. Rewrite the README script section

Replace any inline arg duplication with:

```markdown
### Trigger
- Source: <Platform> -> <Subcategory> -> <Trigger Name>
- Catalog: [Helpers/triggers/<platform>/<subcategory>.md#<trigger-slug>](...)

### Args Consumed
| Arg | Used as | Purpose |
|---|---|---|
| <key> | <how mapped> | <why> |
```

### Step 5. Replace bottom-of-file `## Trigger Variables` block

If the README has a bottom section enumerating canonical args, replace with a catalog-pointer stub listing the catalog files relevant to that feature folder's scripts. Same shape as Phase 5 substitution.

### Step 6. Validate

Per script, grep cross-check: every `TryGetArg` in the `.cs` matches a row in the README's Args Consumed table.

### Step 7. `.cs` headers (optional polish)

If the `.cs` file's top comment block currently lists args inline, leave it — but consider adding a one-line `// Catalog: <link>` reference for the trigger entry. Do not remove existing comments.

## Validation

1. Every feature README in the list has been refactored.
2. No README contains a redundant `## Trigger Variables` block listing upstream-canonical args (only catalog-pointer stubs allowed at bottom).
3. Every `### Args Consumed` table reconciles with `rg TryGetArg` output for the matching `.cs`.
4. All catalog links resolve.
5. `Actions/Helpers/cph-api-signatures.md` "Trigger Arguments" section still points at the catalog.
6. `WORKING.md` released after each batch.

## Exit

- Dirty tree. Do not commit.
- Final change summary lists every README touched and confirms the catalog-vs-script-doc separation is complete repo-wide.

## After this phase

Project complete. Future trigger work:

- New `.cs` script wiring → reads catalog, adds `### Args Consumed` to its README.
- Streamer.bot upstream releases new triggers → re-run a slim version of Phases 0+3 against the affected platform to refresh.

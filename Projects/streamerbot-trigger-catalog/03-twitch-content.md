---
id: trigger-catalog-phase-03-twitch-content
type: project-phase
description: Fill every Twitch subcategory catalog file with args, version notes, caveats, and used-in-repo backlinks. Defines the content-fill procedure later platform phases reference.
status: active
owner: streamerbot-dev
phase: 3
depends_on:
  - 02-wiring.md
---

# Phase 3 — Twitch Content Fill

## Goal

For every Twitch trigger in `manifest.json`, fetch its upstream page, transcribe its args + version + caveats into the matching catalog file, backfill the `Used in repo` section, and flip `coverage` to `seeded`.

This phase also defines the **content-fill procedure** that later platform phases (04, 06–13) reuse. Those prompts point back to this one for the procedure; only their inputs differ.

## Prerequisites

1. Read `Projects/streamerbot-trigger-catalog/README.md` (especially per-trigger entry schema).
2. Confirm Phases 0–2 complete: `manifest.json` exists, catalog skeleton present, wiring landed.
3. Coordination check via [WORKING.md](../../WORKING.md).

## Inputs

- `Projects/streamerbot-trigger-catalog/manifest.json` — Twitch entries.
- Catalog files at `Actions/Helpers/triggers/twitch/*.md` (skeleton stage).
- Repo `.cs` files for `Used in repo` backfill, especially under `Actions/Twitch Core Integrations/`, `Actions/Twitch Bits Integrations/`, `Actions/Twitch Channel Points/`, `Actions/Twitch Hype Train/`, `Actions/Commanders/`, `Actions/Squad/`, `Actions/Intros/`.

## Procedure (the canonical content-fill loop — referenced by later phases)

For each subcategory file:

### Step 1. Fetch each trigger's upstream page

Use WebFetch with a prompt that asks for:

- The full args/variables table (Variable, Type, Notes / Description).
- Any "Min version" / "Required version" callouts.
- Any caveats, special-id notes, or "fires when" preconditions.
- Any examples that imply args (sometimes upstream lists examples without a formal table).

Sample prompt for WebFetch:

> Extract the full Trigger Variables / Args table for this Streamer.bot trigger. List every variable name (exact casing), its type, and its description. Also note any minimum Streamer.bot version requirement and any caveats (e.g. "fires only when X", "available since v0.2.5", "shared variable groups not available"). Format as a markdown table for the args. Separately list caveats as bullet points. Do not summarize.

Caching: WebFetch caches 15 min. Safe to re-issue if the first parse looks thin.

### Step 2. Transcribe into catalog file

Replace the per-trigger placeholder section in the matching subcategory file using the schema from project README.md:

```markdown
## <Trigger Name>

- Path: Twitch -> <Subcategory> -> <Trigger Name>
- Upstream: <full URL from manifest>
- Min SB version: <e.g. `any`, `v0.2.5+`>
- Coverage: seeded

### Args

| Variable | Type | Notes |
|---|---|---|
| user | string | Display name. |
| ... | ... | ... |

### Caveats

- ...

### Used in repo

- _Backfilled in Step 3._
```

If upstream lists no trigger-specific args, write under Args:

```markdown
_No trigger-specific args. Shared variable groups apply (see upstream)._
```

If upstream documents shared variable groups available (e.g. "Twitch User", "Twitch Chat"), include a one-line `Shared groups:` field after `Min SB version:` listing them.

### Step 3. Backfill `Used in repo`

For each trigger, grep the repo for evidence of consumption:

```bash
# Match comments and arg reads referring to the trigger
rg -n "Twitch.*<subcategory>.*<trigger>" Actions/
rg -n "<distinctive arg name>" Actions/ --type cs
```

Cross-reference with each script's `Expected Trigger / Input` header in the existing `.cs` file. List every `.cs` that wires this trigger:

```markdown
### Used in repo

- [Actions/Twitch Core Integrations/subscription-new.cs](../../Twitch%20Core%20Integrations/subscription-new.cs)
- [Actions/Twitch Core Integrations/README.md#subscription-new](../../Twitch%20Core%20Integrations/README.md#script-subscription-newcs)
```

If no script wires it: write `_Not yet wired._` (preserve the skeleton placeholder).

### Step 4. Update coverage flags

- Each subcategory file's frontmatter: `coverage: seeded` (when all triggers in that file are filled). If any trigger inside is incomplete (e.g. upstream 404), use `partial` and add a note.
- Platform README (`Actions/Helpers/triggers/twitch/README.md`): bump coverage column for that subcategory row from `stub` to `seeded` / `partial`.
- Top-level catalog README: bump Twitch row when all 19 subcategories reach `seeded`.

## Twitch-specific notes

- Twitch has a "Shared Chat" subcategory that mirrors several Subscription / Raid / Ban triggers. Many have parallel args to the canonical version. Document them fully — do not write "see Subscriptions"; transcribe each fresh in `shared-chat.md`. The args may look the same but the trigger source differs (shared chat session) and the caveats may differ.
- Twitch Moderation has 27 triggers. Pace yourself — could be split into a sub-pass if the session is getting long. Each Moderation trigger generally has small arg sets (often just `user`, `userId`, `moderator`), so it's volume not depth.
- Twitch Subscriptions:
  - `Sub Counter Rollover` is a counter event — note that Twitch Chat / Twitch User shared variable groups are NOT available (only General + Twitch Broadcaster). Already documented in `Actions/Twitch Core Integrations/README.md`; cross-reference.
  - `Gift Bomb` and `Gift Subscription` interact (one bomb fires N gift events with `fromGiftBomb=true`). Caveat in both entries.
- Twitch Hype Train: `Update` fires frequently during a train — call out rate and de-dupe expectations.

## Validation

1. Every Twitch subcategory file has frontmatter `coverage: seeded` (or `partial` with a documented reason).
2. Every trigger heading has Path, Upstream, Min SB version, Args, Caveats, Used in repo sections present (even if `_Not yet wired._`).
3. `grep -L "_Pending —" Actions/Helpers/triggers/twitch/` returns all 19 files (i.e. no placeholder text remains).
4. `Used in repo` entries resolve — every linked `.cs` exists.
5. Spot-check 3 entries against upstream by re-fetching — args match.

## Exit

- Dirty tree. Do not commit.
- Change summary: trigger count documented, breakdown per subcategory, list of any `partial` subcategories with their reason.

## Next phase options (parallelizable)

- `04-core-content.md` — Streamer.bot Core subcategories.
- `05-script-schema-pilot.md` — pilot script-side `Args Consumed` refactor (depends on this phase).
- `06-kick-content.md` through `13-custom-content.md` — other platforms.

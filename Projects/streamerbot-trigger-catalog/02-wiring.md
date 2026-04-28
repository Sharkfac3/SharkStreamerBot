---
id: trigger-catalog-phase-02-wiring
type: project-phase
description: Wire the new catalog into existing scaffolding so agents discover it from Helpers index, cph-api-signatures, the streamerbot-dev role, and Actions/AGENTS.md. Add the lookup-order rule.
status: active
owner: streamerbot-dev
phase: 2
depends_on:
  - 01-skeleton.md
---

# Phase 2 — Wiring

## Goal

Make the catalog discoverable from the existing entry points an agent already reads when working on Streamer.bot scripts. Add the lookup-order rule so the catalog becomes the first stop, not a hidden corner.

## Prerequisites

1. Read `Projects/streamerbot-trigger-catalog/README.md`.
2. Confirm Phase 1 completed: `Actions/Helpers/triggers/` tree exists.
3. Coordination check via [WORKING.md](../../WORKING.md).

## Files to edit

### 1. `Actions/Helpers/README.md`

Add a new row to the Helper files table:

```markdown
| [triggers/](triggers/README.md) | Canonical Streamer.bot trigger catalog — args, version, caveats — mirrored from upstream nav 1:1. |
```

If a "Compatibility note" section discusses migration from old Trigger Variables tables, append a sentence: "Per-feature `Trigger Variables` blocks are being migrated to per-script `Args Consumed` tables; canonical args live in `triggers/`."

### 2. `Actions/Helpers/cph-api-signatures.md`

Locate the `### Trigger Arguments` subsection. Replace its closing line ("See each feature README's 'Trigger Variables' section for the full list per event type.") with:

```markdown
For the canonical args list per trigger, see the catalog at [triggers/](triggers/README.md). Feature READMEs and `.cs` headers describe `## Args Consumed` — the subset each script reads and how — not the full upstream args set.
```

### 3. `.agents/roles/streamerbot-dev/role.md`

Update the `## Living Context` section. Replace the existing single paragraph with:

```markdown
## Living Context

Lookup order when wiring or editing a Streamer.bot script:

1. **Catalog** — [Actions/Helpers/triggers/](../../../Actions/Helpers/triggers/README.md). Canonical upstream args per trigger.
2. **Script docs** — local `AGENTS.md` and the feature README's `## Args Consumed` table for the relevant `.cs` file.
3. **Upstream** — https://docs.streamer.bot/api/triggers (last resort; if the catalog is wrong, fix the catalog first).

Then check [Actions/HELPER-SNIPPETS.md](../../../Actions/HELPER-SNIPPETS.md) and the concept-specific files under [Actions/Helpers/](../../../Actions/Helpers/) for reusable C# patterns.
```

### 4. `Actions/AGENTS.md`

If this file exists, locate the section that routes agents to subfolder guides. Add the lookup-order rule near the top of that section, with the same three-step ordering as the role file. If `Actions/AGENTS.md` does not exist, skip — do not create it just for this rule; the role-file edit covers it.

## Validation

1. All four edits applied (or three, if `Actions/AGENTS.md` absent).
2. `grep -n "triggers/" Actions/Helpers/README.md` finds the new row.
3. `grep -n "Args Consumed" Actions/Helpers/cph-api-signatures.md` finds the new pointer.
4. `grep -n "Lookup order" .agents/roles/streamerbot-dev/role.md` finds the new section.
5. No broken links — open each new link in the modified files and confirm the target exists.

## Exit

- Dirty tree. Do not commit.
- Change summary lists the four files edited (or three) and quotes the new lookup-order rule for the operator's reference.

## Next phase

`03-twitch-content.md` — fill all Twitch subcategory files with args. Independent from `04`/`06`–`13` (content phases all only depend on Phase 2).

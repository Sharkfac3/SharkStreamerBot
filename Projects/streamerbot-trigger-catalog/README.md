---
id: project-streamerbot-trigger-catalog
type: project
description: Multi-phase project to build a canonical, repo-local catalog of every Streamer.bot trigger (args, version, caveats) so agents stop fetching upstream nav before writing scripts.
status: active
owner: streamerbot-dev
secondaryOwners:
  - ops
---

# Streamer.bot Trigger Catalog Project

## Why this exists

Streamer.bot exposes ~250 triggers across 11 platforms. Today, agents either grep partial info from feature READMEs (which conflate "trigger emits these args" with "this script consumes these args") or leave the repo and read upstream docs every time. Both waste effort and drift.

Goal: a single repo-local catalog at `Actions/Helpers/triggers/` that mirrors the upstream nav 1:1 and documents every trigger's args. Feature READMEs and `.cs` script headers stop trying to be canonical — they describe **which args the script actually consumes and how**, with a pointer to the catalog entry for the full set.

## Phase sequence

Run in order. Each phase finishes with a dirty tree — operator commits manually. Each prompt file is self-contained for cold Claude sessions; read this README first, then the phase prompt, then start.

| # | File | Scope | Depends on |
|---|---|---|---|
| 0 | `00-pre-seed.md` | Fetch upstream subcategory index pages, build `manifest.json` of every (platform, subcategory, trigger, upstream URL). No catalog files yet. | — |
| 1 | `01-skeleton.md` | Scaffold catalog folder tree + every README + every subcategory file with frontmatter, trigger headings, upstream URL stubs, `coverage: stub`. No args. | 00 |
| 2 | `02-wiring.md` | Wire pointers (Helpers index, cph-api-signatures, streamerbot-dev role, Actions/AGENTS.md). Add lookup-order rule. | 01 |
| 3 | `03-twitch-content.md` | Fetch + transcribe args for every Twitch trigger. ~100 fetches, 19 subcategory files filled. | 02 |
| 4 | `04-core-content.md` | Same for Streamer.bot Core (Commands, System, Voice Control, MIDI, etc.). | 02 |
| 5 | `05-script-schema-pilot.md` | Pilot `## Args Consumed` refactor on `Actions/Twitch Core Integrations/README.md`. Locks script-side schema. | 03 |
| 6 | `06-kick-content.md` | Kick — 8 subcategories. | 02 |
| 7 | `07-youtube-content.md` | YouTube — 5 subcategories. | 02 |
| 8 | `08-obs-studio-content.md` | OBS Studio. | 02 |
| 9 | `09-integrations-content.md` | All 19 third-party integrations. May self-split into sub-passes at runtime. | 02 |
| 10 | `10-elgato-content.md` | Camera Hub, Stream Deck, Wave Link. | 02 |
| 11 | `11-meld-studio-content.md` | Meld Studio. | 02 |
| 12 | `12-streamlabs-desktop-content.md` | Streamlabs Desktop. | 02 |
| 13 | `13-custom-content.md` | Custom triggers. | 02 |
| 14 | `14-script-schema-fanout.md` | Apply pilot pattern to remaining feature READMEs. | 05 |

Phases 03+ are independent of each other (only depend on 02). Can run in parallel sessions if desired.

## Conventions (read before any phase)

### Catalog file layout

```
Actions/Helpers/triggers/
  README.md                       # top-level platform index + lookup-order rule
  <platform-slug>/
    README.md                     # subcategory index + per-subcategory coverage flag
    <subcategory-slug>.md         # one file per upstream subcategory
```

### Slug rules

- Platform slug: lowercase, kebab-case, derived from upstream nav name (e.g. `Twitch` → `twitch`, `OBS Studio` → `obs-studio`, `Streamlabs Desktop` → `streamlabs-desktop`).
- Subcategory slug: lowercase, kebab-case (e.g. `Channel Reward` → `channel-reward`, `Hype Train` → `hype-train`).
- Trigger anchor (within subcategory file): lowercase, kebab-case from the trigger name as worded upstream (e.g. `Gift Bomb` → `#gift-bomb`, `Sub Counter Rollover` → `#sub-counter-rollover`).

Slugs are deterministic so script docs can link without lookup.

### Catalog file frontmatter

```yaml
---
id: triggers-<platform-slug>-<subcategory-slug>
type: reference
description: Streamer.bot <Platform> <Subcategory> trigger reference — args, versions, caveats.
owner: streamerbot-dev
status: active
coverage: seeded | partial | stub
upstream: https://docs.streamer.bot/api/triggers/<platform-slug>/<subcategory-slug>
---
```

`coverage: stub` = file exists, trigger names listed, no args. `partial` = some triggers seeded. `seeded` = all triggers in subcategory have args + caveats.

### Per-trigger entry schema (inside subcategory file)

```markdown
## <Trigger Name>

- Path: <Platform> -> <Subcategory> -> <Trigger Name>
- Upstream: <full URL>
- Min SB version: <version or `any`>
- Coverage: seeded | partial | stub

### Args

| Variable | Type | Notes |
|---|---|---|
| user | string | Display name. |
| ... | ... | ... |

### Caveats

- ...

### Used in repo

- [Actions/.../foo.cs](...)
- _Not yet wired._  ← when no scripts consume this trigger
```

If upstream lists no args beyond shared groups, write `_No trigger-specific args. Shared groups apply (see upstream)._` under Args.

### Script-side ("Args Consumed") schema — used in feature READMEs and `.cs` headers

```markdown
### Trigger
- Source: <Platform> -> <Subcategory> -> <Trigger Name>
- Catalog: [triggers/<platform-slug>/<subcategory-slug>.md#<trigger-slug>](...)

### Args Consumed
| Arg | Used as | Purpose |
|---|---|---|
| bits | tier branching | Route to tier 1–4 script. |
| user | `$bitsuser` SI | MIU alert display. |
| message | SI forward | MIU display, `CheerXXX` stripped first. |
```

Only list args the `.cs` actually reads. Do not duplicate the full upstream args list — that's the catalog's job.

### Lookup-order rule (for any agent writing or editing a Streamer.bot script)

1. **Catalog** (`Actions/Helpers/triggers/<platform>/<subcategory>.md#<trigger>`) — canonical upstream args.
2. **Feature README + `.cs` header `## Args Consumed`** — what this script actually reads and how.
3. **Upstream docs** — source of truth for catalog content only. Do not bypass the catalog when wiring scripts; if the catalog is wrong, fix it first.

### Manifest schema

`Projects/streamerbot-trigger-catalog/manifest.json`. Built in Phase 0, consumed by every later phase.

```json
{
  "fetched_at": "YYYY-MM-DD",
  "platforms": [
    {
      "name": "Twitch",
      "slug": "twitch",
      "upstream_url": "https://docs.streamer.bot/api/triggers/twitch",
      "subcategories": [
        {
          "name": "Subscriptions",
          "slug": "subscriptions",
          "upstream_url": "https://docs.streamer.bot/api/triggers/twitch/subscriptions",
          "triggers": [
            {
              "name": "Sub Counter Rollover",
              "slug": "sub-counter-rollover",
              "upstream_url": "https://docs.streamer.bot/api/triggers/twitch/subscriptions/sub-counter-rollover"
            }
          ]
        }
      ]
    }
  ]
}
```

## Memory and exit rules (every phase)

- **No commits.** Operator commits manually. Finish each phase with a dirty working tree. Never run `git add` or `git commit`.
- **One script per action.** If a Phase 5/14 refactor tempts script consolidation, do not consolidate — preserve the one-`.cs`-per-action structure even when scripts look similar.
- **Coordination.** Read [WORKING.md](../../WORKING.md) before starting any phase. Follow [.agents/workflows/coordination.md](../../.agents/workflows/coordination.md).
- **Change summary.** End every phase with the [.agents/workflows/change-summary.md](../../.agents/workflows/change-summary.md) format: paste targets, setup steps, validation output. No `.cs` paste targets in catalog phases (docs only); skip that section.
- **Validation.** Each phase prompt names its own success criteria. Do not move to the next phase until they pass.

## How to start a phase

1. Read this README in full.
2. Read [.agents/ENTRY.md](../../.agents/ENTRY.md) and [.agents/roles/streamerbot-dev/role.md](../../.agents/roles/streamerbot-dev/role.md) for repo conventions.
3. Read [WORKING.md](../../WORKING.md) and follow coordination protocol.
4. Read the specific phase prompt file (`NN-*.md`).
5. If the phase consumes the manifest, read `manifest.json`.
6. Execute. Validate. Hand off via change summary.

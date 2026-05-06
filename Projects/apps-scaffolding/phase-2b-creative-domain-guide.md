# Phase 2b: Create Creative/AGENTS.md Domain Guide

## Context

You are working in the SharkStreamerBot repository. This repo uses structured agent scaffolding where every major domain folder has an `AGENTS.md` entrypoint for coding agents (Claude and Pi).

`Creative/` has 4 subdirectories each with their own `AGENTS.md`, but no domain-level entrypoint. An agent landing at `Creative/` has no routing surface.

## Required Reading

Read these files **before writing anything**:

1. `Actions/AGENTS.md` — gold-standard domain guide template. Match its structure.
2. `Apps/AGENTS.md` — Phase 1 output. Second example of the pattern (if it exists; if not, rely on Actions/AGENTS.md).
3. `.agents/roles/brand-steward/role.md` — primary owner of Creative/.
4. `.agents/roles/art-director/role.md` — owns Creative/Art/.
5. `.agents/_shared/conventions.md` — repo-wide conventions.
6. `.agents/_shared/project.md` — repo-wide context.
7. Each subdirectory's AGENTS.md (listed below).

## Pre-Digested Content Inventory

Use this information directly — do not re-discover it:

| Area | Path | Owner | Purpose | Has AGENTS.md | Has README |
|---|---|---|---|---|---|
| Brand | `Creative/Brand/` | `brand-steward` | Brand identity, voice, values, character codex, neurodivergent metaphor, canon source docs | Yes | No |
| Art | `Creative/Art/` | `art-director` | Visual canon, character art agents, stream style, approved assets, art references, art projects | Yes | Yes |
| Marketing | `Creative/Marketing/` | `brand-steward` / `content-repurposer` | Campaign scaffolding, social content strategy, community growth, clip strategy, platform packaging | Yes | Yes |
| WorldBuilding | `Creative/WorldBuilding/` | `lotat-writer` / `lotat-tech` / `brand-steward` | LotAT adventures, Starship Shamples lore, franchises, storylines, settings, universe rules | Yes | Yes |

### Key Characteristics

- Creative/ is the non-code creative source for the stream
- Multiple owners: `brand-steward` owns brand/marketing, `art-director` owns art, `lotat-writer` owns worldbuilding
- The `canon-guardian` workflow applies across all Creative/ subdirectories for canon-sensitive changes
- Brand docs (BRAND-IDENTITY.md, BRAND-VOICE.md, CHARACTER-CODEX.md) are cross-cutting references used by many domains
- Creative/ does not contain runtime code, tooling, or scripts — those belong in Actions/, Apps/, or Tools/
- Art production tooling lives in `Tools/ArtPipeline/`, not in `Creative/Art/`
- Content pipeline tooling lives in `Tools/ContentPipeline/`, not in `Creative/Marketing/`

### Key Cross-Cutting Brand Docs

| Doc | Path | Used by |
|---|---|---|
| Brand Identity | `Creative/Brand/BRAND-IDENTITY.md` | All roles producing public-facing content |
| Brand Voice | `Creative/Brand/BRAND-VOICE.md` | All roles writing copy, narration, chat text |
| Character Codex | `Creative/Brand/CHARACTER-CODEX.md` | All roles using character names, identities, or metaphors |

## Expected Output Format

Use this exact frontmatter:

```yaml
---
id: creative-agent-guide
type: domain-guide
description: Brand, art, marketing, worldbuilding, and creative source docs for public-facing work.
status: active
owner: brand-steward
---
```

**Frontmatter rules (validator-enforced):**
- `id` must be `creative-agent-guide`
- `type` must be `domain-guide`
- `id` must be kebab-case
- All of `id`, `type`, `description`, `status` are required

## Required Sections (in this order)

1. **Purpose** — 2-3 sentences. What Creative/ contains — brand identity, visual canon, marketing strategy, and worldbuilding. Non-code creative source for the stream.
2. **Start Here** — Numbered steps for an agent arriving at Creative/. Read this file → identify the area → read that area's AGENTS.md → check if canon-guardian workflow applies.
3. **Content Inventory** — Table with: area name, path (as markdown link to local AGENTS.md), primary owner, purpose summary.
4. **Folder Routing** — Map each subdirectory to its local AGENTS.md.
5. **Cross-Cutting Brand References** — Table of the 3 brand docs that are used across the repo, with paths and when to read them.
6. **Canon Guardian** — Brief note that the `canon-guardian` workflow applies to canon-sensitive changes across all Creative/ areas. Link to `.agents/workflows/canon-guardian.md`.
7. **Shared References** — Table linking other key cross-cutting docs beyond brand.
8. **Validation** — The agent-tree validator command.
9. **Boundaries** — What does NOT belong in Creative/ (code → Actions/Apps/, tooling → Tools/, runtime scripts → Actions/).

**Do NOT add sections beyond this list.**

## Length Target

Aim for roughly 60-90 lines of markdown (excluding frontmatter). Creative/ is a routing surface — each area's AGENTS.md has full detail. Keep this concise.

## Constraints

- **Only create `Creative/AGENTS.md`.** Do not modify any existing file.
- Use relative markdown links from `Creative/` (e.g., `[Brand/AGENTS.md](Brand/AGENTS.md)`).
- Do not invent information beyond what is in the required reading files and this prompt.
- Do not add commentary, explanations, or meta-notes inside the file.

## Validation

After creating the file, run:

```bash
python Tools/AgentTree/validate.py
```

**What the validator checks:**
- **frontmatter**: required fields `id`, `type`, `description`, `status` present
- **folder-coverage**: every first-level subdirectory under Creative/ has AGENTS.md coverage
- **link-integrity**: all markdown links and backtick paths resolve to real files
- **naming**: `id` is kebab-case and matches domain pattern (`creative-*` for files under `Creative/`)
- **id-uniqueness**: no duplicate frontmatter IDs across all AGENTS.md files

Report the full validator output. If there are failures related to your new file, fix them before finishing.

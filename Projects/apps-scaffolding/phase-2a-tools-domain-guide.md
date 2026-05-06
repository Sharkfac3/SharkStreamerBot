# Phase 2a: Create Tools/AGENTS.md Domain Guide

## Context

You are working in the SharkStreamerBot repository. This repo uses structured agent scaffolding where every major domain folder has an `AGENTS.md` entrypoint for coding agents (Claude and Pi).

`Tools/` has 6 subdirectories each with their own `AGENTS.md`, but no domain-level entrypoint. An agent landing at `Tools/` has no routing surface.

## Required Reading

Read these files **before writing anything**:

1. `Actions/AGENTS.md` — gold-standard domain guide template. Match its structure.
2. `Apps/AGENTS.md` — Phase 1 output. Second example of the pattern (if it exists; if not, rely on Actions/AGENTS.md).
3. `.agents/roles/ops/role.md` — primary owner of Tools/.
4. `.agents/_shared/conventions.md` — repo-wide conventions.
5. `.agents/_shared/project.md` — repo-wide context.
6. Each tool's AGENTS.md (listed below) — to understand what each tool does.

## Pre-Digested Tool Inventory

Use this information directly — do not re-discover it:

| Tool | Path | Owner | Purpose | Has AGENTS.md | Has README |
|---|---|---|---|---|---|
| AgentTree | `Tools/AgentTree/` | `ops` | Agent-tree validation: frontmatter, folder coverage, link integrity, naming, ID uniqueness | Yes | No |
| StreamerBot | `Tools/StreamerBot/` | `ops` / `streamerbot-dev` | Sync helpers, validation scripts (action contracts), operator templates for Streamer.bot | Yes | Yes |
| MixItUp | `Tools/MixItUp/` | `ops` / `streamerbot-dev` / `app-dev` | Mix It Up API helpers, command support, overlay sources, payload conventions | Yes | Yes |
| LotAT | `Tools/LotAT/` | `lotat-tech` / `ops` | Story viewer (FastAPI), pipeline stage management, schema handoff tooling | Yes | Yes |
| ContentPipeline | `Tools/ContentPipeline/` | `content-repurposer` / `ops` | Short-form content: transcription, highlight detection, clip extraction, review, publishing, feedback | Yes | Yes |
| ArtPipeline | `Tools/ArtPipeline/` | `art-director` / `ops` | Art production: spec → prompt → generate → review pipeline for Stable Diffusion assets | Yes | Yes |

### Key Characteristics

- All tools are local-only Python or Node utilities
- Tools support other domains (Actions/, Apps/, Creative/) but don't own runtime behavior
- `ops` role has oversight across all tools; individual tools have domain-specific primary owners
- Tools/AgentTree/validate.py is the repo-wide agent doc validator used by all domains
- No shared package manager — tools are standalone scripts, some with pip requirements

## Expected Output Format

Use this exact frontmatter:

```yaml
---
id: tools-agent-guide
type: domain-guide
description: Local utilities, validators, sync helpers, and external integration scripts.
status: active
owner: ops
---
```

**Frontmatter rules (validator-enforced):**
- `id` must be `tools-agent-guide`
- `type` must be `domain-guide`
- `id` must be kebab-case
- All of `id`, `type`, `description`, `status` are required

## Required Sections (in this order)

1. **Purpose** — 2-3 sentences. What Tools/ contains and its relationship to other domains.
2. **Start Here** — Numbered steps for an agent arriving at Tools/. Read this file → identify the tool → read that tool's AGENTS.md → follow workflows.
3. **Tool Inventory** — Table with: tool name, path (as markdown link to local AGENTS.md), primary owner, purpose summary.
4. **Folder Routing** — Same table format as Actions/AGENTS.md routing. Map each subdirectory to its local AGENTS.md.
5. **Shared Conventions** — Bullet list: all tools local-only, Python environment detection (from `.agents/_shared/conventions.md`), tools support but don't own runtime behavior.
6. **Shared References** — Table linking key cross-cutting docs.
7. **Validation** — The agent-tree validator command. Note that individual tools have their own validation in their local guides.
8. **Boundaries** — What does NOT belong in Tools/ (runtime C# scripts → Actions/, app code → Apps/, creative content → Creative/).

**Do NOT add sections beyond this list.**

## Length Target

Aim for roughly 60-90 lines of markdown (excluding frontmatter). Tools/ is a routing surface — each tool's AGENTS.md already has full detail. Keep this concise.

## Constraints

- **Only create `Tools/AGENTS.md`.** Do not modify any existing file.
- Use relative markdown links from `Tools/` (e.g., `[AgentTree/AGENTS.md](AgentTree/AGENTS.md)`).
- Do not invent information beyond what is in the required reading files and this prompt.
- Do not add commentary, explanations, or meta-notes inside the file.

## Validation

After creating the file, run:

```bash
python Tools/AgentTree/validate.py
```

**What the validator checks:**
- **frontmatter**: required fields `id`, `type`, `description`, `status` present
- **folder-coverage**: every first-level subdirectory under Tools/ has AGENTS.md coverage
- **link-integrity**: all markdown links and backtick paths resolve to real files
- **naming**: `id` is kebab-case and matches domain pattern (`tools-*` for files under `Tools/`)
- **id-uniqueness**: no duplicate frontmatter IDs across all AGENTS.md files

Report the full validator output. If there are failures related to your new file, fix them before finishing.

# Drift Sweeps

Paste-ready prompts for a full project audit. Each prompt covers one domain, reads the relevant files, and fixes all stale/incorrect content in place.

## What "drift" means in this project

- File paths in docs that no longer match the actual directory structure
- Feature descriptions that describe planned/placeholder state when the feature is now built
- Missing entries for features, domains, or files added since the doc was written
- Cross-references between docs that point to the wrong file or section
- Outdated port numbers, collection names, constant names, or schema field names
- Startup prompts or skill wrappers that don't mention skill files that exist

## How to run a sweep

1. Open a fresh agent chat (Claude Code or Pi)
2. Paste the entire contents of the prompt file
3. Let the agent read and fix — it will report what changed
4. Human operator commits the changes manually after reviewing the report
5. Run the next sweep in a fresh chat

## Recommended order

Run in sequence. Later sweeps assume earlier ones are clean.

| # | File | Covers |
|---|------|--------|
| 01 | `01-root-and-arch-docs.md` | README.md, CLAUDE.md, Docs/Architecture/ |
| 02 | `02-agent-scaffolding-core.md` | .agents/ENTRY.md, _shared/*, all role.md + core.md |
| 03 | `03-agent-app-dev-skills.md` | .agents/roles/app-dev/ full skill tree |
| 04 | `04-agent-streamerbot-dev-skills.md` | .agents/roles/streamerbot-dev/ full skill tree |
| 05 | `05-agent-other-role-skills.md` | lotat-tech, lotat-writer, ops, art-director, brand-steward, content-repurposer, product-dev |
| 06 | `06-pi-skills-layer.md` | .pi/skills/ full tree |
| 07 | `07-human-startup-prompts.md` | humans/agent-startup-prompts/ all role prompts |
| 08 | `08-actions-docs.md` | Actions/**/*.md, SHARED-CONSTANTS.md, HELPER-SNIPPETS.md |
| 09 | `09-human-setup-docs.md` | humans/setup-info/, humans/art-pipeline/ |

## Ground truth hierarchy

When docs conflict, trust in this order:

1. **Source code** — actual files in `Apps/`, `Actions/`, `Tools/`
2. **`.agents/_shared/`** — canonical cross-role contracts
3. **Role skill files** — `.agents/roles/<role>/skills/`
4. **Everything else** — README files, startup prompts, Pi wrappers

## After all sweeps complete

Every markdown file in the repo should accurately describe what exists, not what was planned. Run the ops validation skill to confirm no broken cross-references remain.

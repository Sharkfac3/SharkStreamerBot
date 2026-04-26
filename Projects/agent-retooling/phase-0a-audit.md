# Phase 0a — Audit current scaffolding

## Before you start

1. Read `Projects/agent-retooling/PLAN.md` for locked design decisions and scope.
2. Read `Projects/agent-retooling/STATUS.md` and confirm `phase-0a-audit.md` is not already checked off.

## Mission

Inventory every file under `.agents/` and `.pi/skills/`. Output one row per file in `Projects/agent-retooling/audit.md`. Read-only against the production tree — do not edit anything outside `Projects/agent-retooling/`.

## Steps

1. List every file under `.agents/` (recursive) and `.pi/skills/` (recursive). Use Glob.
2. For each file, read it (or skim the first 30 lines for long files) to understand its purpose.
3. For each file, run a Grep across the full repo (excluding `Projects/agent-retooling/` and `.git/`) to find references to that file's path or basename. Record file paths that link to it.
4. Write the audit table in the format below.

## Output format — `audit.md`

```markdown
# Audit — Current Agent Scaffolding

Snapshot of every file under `.agents/` and `.pi/skills/` at the start of the retool. Read-only inventory. Verdicts come in `salvage-list.md`.

## .agents/ root

| path | purpose | content type | referenced by |
|---|---|---|---|
| .agents/ENTRY.md | ... | index | CLAUDE.md, README.md |
| .agents/routing-manifest.json | ... | manifest | (none) |

## .agents/_shared/

| path | purpose | content type | referenced by |
|---|---|---|---|
| ... | ... | ... | ... |

## .agents/roles/<role-name>/

(one H2 per role — _template, app-dev, art-director, brand-steward, content-repurposer, lotat-tech, lotat-writer, ops, product-dev, streamerbot-dev)

## .pi/skills/

(one H2 per top-level skill folder)

## Open questions

- (anything you couldn't resolve — bullet list)
```

## Content-type values (use exactly these)

`role-definition`, `skill`, `context-note`, `manifest`, `stub-redirect`, `reference-doc`, `index`, `template`.

## When done

1. Append to `STATUS.md` under `## Notes`: `- 0a complete: <count> files inventoried, <count> open questions logged.`
2. Tick `[x] phase-0a-audit.md` in STATUS.md.
3. Phase 0a is independent of 0b, 0c. Phase 0d cannot start until 0a and 0b are both checked off.

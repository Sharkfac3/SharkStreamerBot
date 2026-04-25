# Drift Sweep 07 — Human Startup Prompts

You are doing a drift sweep for the SharkStreamerBot project. Your job is to read every file listed below, find anything stale or incorrect, and fix it in place. Report each change.

The startup prompts in `humans/agent-startup-prompts/` are copy/paste instructions that operators use to spin up agent sessions. Drift here means: a prompt tells the agent to load files that have moved, skip files that now exist, or adopt a stance ("this is a placeholder") that no longer matches reality.

Do NOT add features, reorganize, or improve style. Only fix factual drift.

---

## Before you start

1. Read `WORKING.md` at repo root. Check for active file conflicts before editing anything.
2. Edit files in place with minimal diffs — change only what is factually wrong.
3. Do NOT run `git add` or `git commit`. The human operator commits all changes manually.

---

## Ground truth sources (read these first)

- `.agents/ENTRY.md` — canonical roles and shared context list
- `.agents/roles/<role>/role.md` — each role's current scope and skill load order
- `.agents/roles/<role>/skills/core.md` — each role's core skill file
- `.agents/_shared/` — all shared context files that now exist

---

## Files to sweep

### `humans/agent-startup-prompts/README.md`

Drift to look for:
- Included roles list: should match the current role list in `.agents/ENTRY.md`
- Any instructions referencing files or conventions that have changed

### `humans/agent-startup-prompts/app-dev.md`

This is the most likely to be stale. Drift to look for:
- "Current status note" section: says role is a placeholder — this is no longer true. The Apps/ domain is fully established with three active apps. Remove or rewrite this note.
- Context loading list: should include `.agents/_shared/info-service-protocol.md` when info-service work is involved
- Operating rules: "If you are defining first-generation app patterns, document assumptions clearly" — no longer applicable; patterns are established
- "any existing app docs or source files under `Apps/` if that domain exists for the task" — Apps/ definitely exists; rewrite to name the actual apps
- Business context section: should mention the three active apps and their roles
- Sub-skill navigation: should direct agent to `stream-interactions/_index.md` and `context/info-service.md` for relevant work

### `humans/agent-startup-prompts/streamerbot-dev.md`

Drift to look for:
- Context loading list: should include overlay integration skill, lotat skill, squad skill, info-service-protocol.md (for Intros scripts)
- Operating rules: any rules referencing "if overlay integration exists" hedging that is no longer needed — overlay IS integrated
- Any references to features as planned/upcoming that are now complete

### `humans/agent-startup-prompts/lotat-tech.md`

Drift to look for:
- Context loading list: all engine skill files should be named
- Any references to LotAT overlay integration as upcoming — it's complete

### `humans/agent-startup-prompts/lotat-writer.md`

Drift to look for:
- Context loading list: all writer skill files should be named
- Any placeholder language about franchise or adventure content

### `humans/agent-startup-prompts/art-director.md`

Drift to look for:
- Context loading list: all art-director skill files should be named
- Asset path references: should match actual `Assets/` structure

### `humans/agent-startup-prompts/brand-steward.md`

Drift to look for:
- Context loading list: all brand-steward skill files should be named
- Brand file references: cross-check against `Creative/Brand/` directory

### `humans/agent-startup-prompts/content-repurposer.md`

Drift to look for:
- Context loading list: all content-repurposer skill files should be named
- Pipeline tool references: cross-check against `Tools/ContentPipeline/` if it exists

### `humans/agent-startup-prompts/ops.md`

Drift to look for:
- Context loading list: all ops skill files should be named
- Validation script references: cross-check against `Tools/StreamerBot/Validation/`
- Change summary instructions: should mention Apps/ as a deployment target alongside Actions/
- Any sync workflow steps that don't account for Apps/ (stream-overlay, info-service, production-manager are separate processes, not copy/pasted like Streamer.bot scripts)

### `humans/agent-startup-prompts/product-dev.md`

Drift to look for:
- Context loading list: all product-dev skill files should be named
- Any scope references that conflict with current product-dev role definition

---

## Output format

For each file:
- File path
- What was stale
- What you changed it to

If a file has no drift, say so.

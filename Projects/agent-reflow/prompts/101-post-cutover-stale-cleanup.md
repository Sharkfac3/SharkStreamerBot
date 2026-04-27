# Prompt 101 — Post-Cutover Stale Cleanup

## Agent

Pi (manual copy/paste by operator).

## Purpose

Remove migration-era references and procedural duplication from active docs now that cutover is complete. Audit findings 6.1, 6.2, 7.1, 8.1, 8.2, 2.2: active docs still mention Phase E expectations, retired Pi mirror, and duplicated validation procedure that belongs in workflows.

Bundles audit findings: 6.1, 6.2, 7.1, 8.1, 8.2, 2.2.

## Preconditions

- Prompt 100 complete; manifest statuses aligned
- Validator green
- Read `findings/99-audit.md` findings 6.1, 6.2, 7.1, 8.1, 8.2, 2.2 first
- Read `handoffs/100-manifest-status-normalization.handoff.md`

## Scope

Edits:

**Stale migration references (finding 6.1):**
- `.agents/workflows/validation.md` lines 43, 59, 81, 93–95 (remove Phase E baseline + migration prompt refs)
- `Tools/AgentTree/AGENTS.md` lines 78, 94, 129, 131
- Any `Actions/*/AGENTS.md` containing "do not delete old central skill files during migration" — remove that language; cutover is done

**Retired-mirror dead link (finding 6.2 / 7.1):**
- `.agents/_shared/conventions.md` line 25 — remove or replace link to `../../retired Pi skill mirror/` with historical note

**Validation workflow scope cleanup (finding 8.1):**
- `.agents/workflows/validation.md` — keep only reusable workflow logic (choose checks, run validator, report exit codes, escalate failures); migration-era expected failures removed (move to `Projects/agent-reflow/findings/08-validator.md` or archive)

**Procedural duplication in domain docs (finding 8.2 + 2.2):**
- `Actions/Commanders/AGENTS.md:97`
- `Actions/Squad/AGENTS.md:111`
- `Actions/Twitch Bits Integrations/AGENTS.md:102`
- `Actions/Twitch Channel Points/AGENTS.md:88`
- `Actions/Twitch Core Integrations/AGENTS.md:106`
- `Actions/Twitch Hype Train/AGENTS.md:101`
- `Actions/Voice Commands/AGENTS.md:105`
- `Tools/ArtPipeline/AGENTS.md:105`
- `Tools/ContentPipeline/AGENTS.md:94`
- `Tools/MixItUp/AGENTS.md:90`
- `Tools/StreamerBot/AGENTS.md:78`

For each: replace duplicated migration validation language with a stable pointer to `.agents/workflows/validation.md` plus only domain-specific commands or paste targets.

## Out-of-scope

- No new files; no concept-boundary splits (those are 102, 104)
- No manifest schema changes (100 owned that)
- No rewriting domain facts — only removing migration-era language and procedural duplication
- No git operations

## Steps

1. Walk the listed files. For each:
   - Identify migration-era language (refs to `Projects/agent-reflow/`, Phase E, retired mirror, "during migration", "do not delete old central skill files")
   - Identify duplicated procedural validation block
   - Replace or remove per audit recommendations

2. Update `.agents/workflows/validation.md` to be reusable-only. Move historical Phase E baseline to `Projects/agent-reflow/findings/08-validator.md` appendix or new `Projects/agent-reflow/findings/101-archived-validation-baseline.md`.

3. Fix `.agents/_shared/conventions.md` retired-mirror link. Replace with brief historical note ("Pi mirror retired 2026-04 at Phase F cutover") or remove the row entirely.

4. For each procedural-duplication site, replace the duplicated block with a one-line pointer (e.g. "See [`.agents/workflows/validation.md`](../../.agents/workflows/validation.md)") plus any genuinely domain-specific command.

5. Run `python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/101-validator.failures.txt`. Expect 0 failures.

6. Run a broader broken-link scan over `.agents/`, `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`, root markdown. Document any remaining stale paths in handoff.

7. Write handoff.

## Validator / Acceptance

- `python3 Tools/AgentTree/validate.py` exits 0
- Grep `.agents/`, `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`, root markdown for: `retired Pi skill mirror`, `Projects/agent-reflow/findings/08`, `Phase E`, `Phase F`, "during migration", "do not delete old central skill" — only allowed in `Projects/agent-reflow/` itself or explicit historical-archive doc
- All listed duplication sites now point at `.agents/workflows/validation.md` instead of containing duplicated procedure

## Handoff

Per template. Include:
- Files edited + summary of cuts per file
- Broader broken-link scan result
- Final validator output
- Any leftover stale references that operator needs to triage manually

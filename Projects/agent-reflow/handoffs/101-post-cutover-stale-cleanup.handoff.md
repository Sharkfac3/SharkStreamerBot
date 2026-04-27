# Handoff — 101 Post-Cutover Stale Cleanup

## Summary

Removed migration-era validation language from active agent/workflow docs after cutover. Active docs now point to the reusable validation workflow and retain only domain-specific validation commands or paste/setup notes.

## Files edited

### Shared workflows and conventions

- `.agents/workflows/validation.md` — removed Phase E baseline references, prompt-specific migration acceptance language, and expected-failure guidance. Reframed as reusable validation procedure: choose checks, run validators, record exit codes, categorize failures, and escalate blockers.
- `.agents/_shared/conventions.md` — removed the dead link to `../../retired Pi skill mirror/`; replaced it with a non-linked historical note that the Pi skill mirror was retired at cutover. Removed manifest-transition wording around wrapper skills.
- `.agents/roles/app-dev/role.md` — removed stale “old central skill tree” transition wording from living context.

### Agent-tree tooling guide

- `Tools/AgentTree/AGENTS.md` — removed prompt-specific validator paths, migration-baseline expectations, legacy routing wording, and migration prompt references. Kept stable validator compile/run commands and linked to `.agents/workflows/validation.md` for shared procedure.

### Domain agent docs with duplicated validation blocks

- `Actions/Commanders/AGENTS.md` — replaced migration validator block with pointer to `.agents/workflows/validation.md`; removed old central skill deletion warning; updated validation handoff wording.
- `Actions/Squad/AGENTS.md` — same cleanup; preserved mini-game lock, overlay, and shared-constants checks.
- `Actions/Twitch Bits Integrations/AGENTS.md` — same cleanup; replaced migration-specific boundary with stable non-Bits ownership boundary.
- `Actions/Twitch Channel Points/AGENTS.md` — same cleanup; preserved OBS scene and Mix It Up payload checks.
- `Actions/Twitch Core Integrations/AGENTS.md` — same cleanup; preserved stream-start reset and Mix It Up payload checks.
- `Actions/Twitch Hype Train/AGENTS.md` — same cleanup; preserved hype-train payload checks.
- `Actions/Voice Commands/AGENTS.md` — same cleanup; preserved OBS scene/mode validation and paste-target testing notes.
- `Tools/ArtPipeline/AGENTS.md` — replaced migration-phase agent-tree command with stable pointer to `.agents/workflows/validation.md`; preserved art-pipeline dry-run and setup commands.
- `Tools/ContentPipeline/AGENTS.md` — replaced migration-phase agent-tree command with stable pointer; preserved setup, py_compile, and phase-command guidance.
- `Tools/MixItUp/AGENTS.md` — replaced migration-phase agent-tree command with stable pointer; preserved Mix It Up API discovery and py_compile guidance.
- `Tools/StreamerBot/AGENTS.md` — replaced migration-phase agent-tree command with stable pointer; preserved Streamer.bot tooling compile guidance.

### Migration archive

- `Projects/agent-reflow/findings/08-validator.md` — marked the old validator baseline as archived historical Phase E baseline and renamed the baseline section accordingly.
- `Projects/agent-reflow/findings/101-validator.failures.txt` — final validator report path from this prompt; validator completed with zero failures.

## Stale-reference grep

Command:

```bash
rg -n "retired Pi skill mirror|Projects/agent-reflow/findings/08|Phase E|Phase F|during migration|do not delete old central skill" .agents Actions Apps Tools Creative Docs ./*.md --glob '*.md' --glob '!Apps/**/node_modules/**'
```

Result: no matches in active docs.

## Broader broken-link scan

Custom Markdown link scan over `.agents/`, `Actions/`, `Apps/`, `Tools/`, `Creative/`, `Docs/`, and root Markdown files, excluding dependency/build folders:

```text
files scanned: 106
checked markdown links: 1042
broken markdown links: 0
```

No remaining stale deleted-path references were found by this scan.

## Final validator output

Command:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/101-validator.failures.txt
```

Final stdout:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 146 | 0 | PASS |
| link-integrity | 48 | 0 | PASS |
| drift | 2 | 0 | PASS |
| stub-presence | 46 | 0 | PASS |
| orphan | 20 | 0 | PASS |
| naming | 105 | 0 | PASS |

Total failures: 0
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/101-validator.failures.txt
```

## Leftover stale references / operator triage

None found in active docs for the requested stale-reference patterns. Historical references remain only under `Projects/agent-reflow/`, where they are archival migration records.

## Coordination

- `WORKING.md` was checked before edits.
- No conflicting active work was present.
- Active Work row should be cleared after this handoff is complete.

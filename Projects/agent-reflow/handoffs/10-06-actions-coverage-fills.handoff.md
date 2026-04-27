# Prompt 10-06 handoff — actions-coverage-fills

Date: 2026-04-26
Agent: pi

## State changes

Created five local action-folder agent guides:

- `Actions/Destroyer/AGENTS.md`
- `Actions/Intros/AGENTS.md`
- `Actions/Rest Focus Loop/AGENTS.md`
- `Actions/Temporary/AGENTS.md`
- `Actions/XJ Drivethrough/AGENTS.md`

Wrote this handoff:

- `Projects/agent-reflow/handoffs/10-06-actions-coverage-fills.handoff.md`

Updated validation report:

- `Projects/agent-reflow/findings/10-06-validator.failures.txt`

## Inputs read

Prompt-required inputs:

- `.agents/ENTRY.md`
- `WORKING.md`
- `Projects/agent-reflow/findings/02-domain-coverage.md`
- `Projects/agent-reflow/handoffs/02-inventory-domains.handoff.md`
- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/handoffs/05-design-target-shape.handoff.md`
- `Projects/agent-reflow/findings/06-naming-convention.md`
- `Projects/agent-reflow/handoffs/06-design-naming-convention.handoff.md`
- `Projects/agent-reflow/findings/08-validator.md`
- `Projects/agent-reflow/handoffs/08-validator.handoff.md`
- `.agents/roles/streamerbot-dev/skills/core.md`
- `.agents/_shared/mixitup-api.md`
- `.agents/_shared/info-service-protocol.md`
- `Actions/SHARED-CONSTANTS.md`
- `Actions/HELPER-SNIPPETS.md`

Target-folder scouting:

- `Actions/Destroyer/destroyer-spawn.cs`
- `Actions/Destroyer/destroyer-move.cs`
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/redeem-capture.cs`
- `Actions/Rest Focus Loop/README.md`
- `Actions/Temporary/temp-focus-timer-start.cs`
- `Actions/Temporary/temp-focus-timer-end.cs`
- `Actions/XJ Drivethrough/xj-drivethrough-main.cs`

Additional route examples read for style/frontmatter/link conventions:

- `Actions/Commanders/AGENTS.md`
- `Actions/Overlay/AGENTS.md`
- `Actions/Twitch Core Integrations/AGENTS.md`
- `.agents/manifest.json`

## Route docs created

| Route ID | Local guide | Notes |
|---|---|---|
| `actions-destroyer` | `Actions/Destroyer/AGENTS.md` | Documents spawn/move split, overlay broker dependency, asset requirements, and brand/app-dev handoffs. |
| `actions-intros` | `Actions/Intros/AGENTS.md` | Documents info-service collections, First Chat playback, redemption capture, Mix It Up sub-action chain, and chain rules to `app-dev`, `brand-steward`, and `ops`. |
| `actions-rest-focus-loop` | `Actions/Rest Focus Loop/AGENTS.md` | Documents rest/focus timer state machine, phase guards, fail-closed behavior, commander links, and timer validation. |
| `actions-temporary` | `Actions/Temporary/AGENTS.md` | Documents the temporary focus timer bridge as standalone manifest coverage, operationally adjacent to Rest Focus Loop but not covered by it. |
| `actions-xj-drivethrough` | `Actions/XJ Drivethrough/AGENTS.md` | Documents XJ overlay/audio action, product/brand ambiguity triggers, app-dev handoff, asset requirements, and re-entry guard validation. |

## Scope notes

- No app-side info-service docs were created.
- No Mix It Up tool docs were created.
- No Twitch, Squad, Commanders, Voice, LotAT, or Overlay docs were modified by this prompt.
- No old source files were deleted.
- No git operations were performed.

## Ambiguous ownership / operator review flags

- `Actions/Temporary/`: left as a standalone `actions-temporary` route. It is operationally related to focus/rest behavior but is not the durable Rest Focus Loop state machine. If the temporary timer is retained permanently, operator should decide whether to migrate it into `Actions/Rest Focus Loop/` or keep it as a separate temporary/ops bridge.
- `Actions/XJ Drivethrough/`: primary ownership remains `streamerbot-dev` for runtime behavior. If the XJ effect becomes product-facing, technical content, sponsorship positioning, or customer-facing off-road messaging, chain to `product-dev` and `brand-steward`.
- `Actions/Intros/`: primary ownership remains `streamerbot-dev`, but practical changes may require `app-dev` for info-service schemas/routes, `ops` for Mix It Up command/API setup, and `brand-steward` for reward copy or intro policy. The guide notes dependency on later info-service and Mix It Up local AGENTS guides without linking to missing files.

## Validator status

Command run from repo root:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-06-validator.failures.txt
```

Exit code: `1`

Stdout summary:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 149 | 23 | FAIL |
| link-integrity | 113 | 331 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 48 | 27 | FAIL |
| orphan | 104 | 18 | FAIL |
| naming | 106 | 0 | PASS |

Total failures: 402
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-06-validator.failures.txt
```

Expected cleared deltas confirmed:

- No remaining `folder-coverage` or `stub-presence` failures for:
  - `actions-destroyer`
  - `actions-intros`
  - `actions-rest-focus-loop`
  - `actions-temporary`
  - `actions-xj-drivethrough`
- No new broken-link failures remain for the five newly created guides.
- Remaining failures are pre-existing migration backlog: old central role/shared/Pi link failures, remaining missing domain docs outside this prompt, stale generated tables, role frontmatter gaps, and existing orphan reports.

## Next prompt considerations

- Later `Apps/info-service/AGENTS.md` and `Tools/MixItUp/AGENTS.md` prompts should link back to `Actions/Intros/AGENTS.md` and become the preferred setup/protocol references for intro-related app/tool changes.
- If operator decides `Actions/Temporary/` should be folded into Rest Focus Loop, do that in a scoped prompt that updates manifest coverage intentionally rather than silently changing coverage during docs fill.

# Info Service Prompt Series

Prompts that build `info-service` (general-purpose REST data layer) and `production-manager` (React admin web app) across many fresh-chat sessions with a coding agent.

## How to use

1. Open `COORDINATION.md` — live decisions, open questions, chunk status, run log.
2. Run the next prompt in order (see status table in `COORDINATION.md`).
3. For each prompt: open the file, copy its contents into a new coding-agent chat window, let it run.
4. When agent finishes, review commits + update `COORDINATION.md` status row if agent did not.
5. If prompt left `Open Questions`, answer them in `COORDINATION.md` before running the next prompt.

## Run order

| # | File | Purpose | Writes to |
|---|------|---------|-----------|
| P0 | `00-seed-plan.md` | Create initial architecture plan doc | `Docs/INFO-SERVICE-PLAN.md` |
| P1 | `01-harden-questions.md` | Identify architecture gaps, list for operator | `COORDINATION.md` Open Questions |
| P2 | `02-schemas.md` | Propose envelope + first-collection schemas | `Docs/INFO-SERVICE-PLAN.md` Schemas section |
| P3 | `03-draft-exec-prompts.md` | Draft first two execution prompts | `humans/info-service/10-*.md`, `11-*.md` |
| P4 | `04-draft-next-exec-prompts.md` | Reusable drip-meta — draft next two unblocked execution prompts. Run any time the chain breaks. | `humans/info-service/1N-*.md` files |
| C1+ | `10-*.md`, `11-*.md`, ... | Execution chunks (self-propagating from 12+) | Code, scaffolding, docs |

Execution prompts drip one or two at a time — each completed chunk generates the next prompt to keep it synced with current state.

## Goal (one-line)

Add a file-backed JSON REST service (`info-service`) + React admin app (`production-manager`) to support per-user custom intros, structured to scale to additional collections later.

## Non-goals

- No SQLite, no DB migration tooling, no auth yet
- No new characters or cast additions
- No changes to overlay or broker behavior

## Prompt authoring rules

**No git commits.** Every prompt in this series — already written or drafted later — MUST NOT instruct the agent to run `git commit` or `git add`. Agents finish with files changed and skill output shown; operator reviews and commits manually. When drafting new execution prompts, omit commit steps and mark the Run Log `Commit` column as `uncommitted` for the agent's own row.

**Self-propagation.** Every execution prompt (`12-*.md` onward) MUST end with a Finish step that drafts the NEXT chunk's prompt file, following the same template. This keeps the chain moving without an operator meta run between every chunk. If the next chunk's prereqs are not all merged (e.g. a parallel chunk is pending), the agent reports the blocker in chat instead of drafting.

**Drip-meta fallback.** `04-draft-next-exec-prompts.md` is a reusable catch-up tool. Run it any time the self-propagation chain breaks (missed Finish step, merge conflict, prereq chain unblocked after being stuck). It drafts the next two unblocked chunks without modifying code.

**Scope discipline.** Drafted prompts pull scope from `Docs/INFO-SERVICE-PLAN.md` and `COORDINATION.md`, never from invention. Ambiguity = Open Question in COORDINATION, not a guess.

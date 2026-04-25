# Prompt P1 — Harden Plan (Identify Gaps, Ask Questions)

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations. All context is in committed files.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `humans/info-service/README.md`
4. `humans/info-service/COORDINATION.md` — pay attention to Decisions (do not re-open these) + current Open Questions
5. `Docs/INFO-SERVICE-PLAN.md` — the architecture plan seeded by P0
6. `.agents/roles/app-dev/role.md` — your role for this task
7. `.agents/_shared/mixitup-api.md` — style reference
8. `Actions/SHARED-CONSTANTS.md` — convention reference

Role for this task: `app-dev`.

## WORKING.md

Add row at start, remove + log at end. Domain: `humans/`. Files: `humans/info-service/COORDINATION.md`.

## Task

Read `Docs/INFO-SERVICE-PLAN.md` critically. Identify gaps, ambiguities, or decisions needed before any execution chunk can be written cleanly. Write them into `humans/info-service/COORDINATION.md` under the `## Open Questions` section. The operator will answer them between runs. P2 picks up after.

You are not authorized to resolve any question yourself. Your job is to surface them with options + a recommendation.

## Required topic coverage

At minimum, produce questions under each of these topics. Add more if you see gaps.

### HTTP framework for info-service

Options to present: Fastify, Express, Node native `http`. Recommend one with a one-line reason (bundle size, types, middleware, familiarity with overlay stack — check `Apps/stream-overlay/packages/broker/package.json` first and note what broker uses).

### Validator

zod already tentative in plan. Confirm or propose alternative (valibot, typia, hand-written). Recommend zod unless strong reason.

### production-manager data-fetching layer

Options: plain `fetch`, React Query (TanStack Query), SWR. Recommend one.

### production-manager component library

Options: none (plain CSS / Tailwind), shadcn/ui, Mantine, Chakra, MUI. Recommend one that fits a small operator-only admin app. Default bias: minimal.

### Ports

Broker port is documented somewhere — find it (search overlay broker source or env). Propose ports for `info-service` + `production-manager` dev server + (if applicable) `production-manager` prod-served port. Avoid collisions.

### Assets root path

Relative to repo root? Absolute path from operator machine? MixItUp needs a fixed path. Propose: repo-relative by default, operator can override via a constant. Confirm.

### Data folder location for info-service JSON files

Inside `Apps/info-service/data/`? Sibling `data/` at repo root? Committed to git or gitignored? Back up to `Assets/` folder? Surface tradeoffs.

### Schema version strategy

On boot, envelope `schemaVersion` mismatch detected — error out, auto-migrate, log and continue? Propose policy.

### First-chat behavior when no intro registered

SB queries info-service, gets 404. Silent no-op? Small log? Queue into `pending-intros` automatically? Propose.

### Channel point redeem capture flow

Today: operator watches chat, manually enters intro after redeem. Future: redeem handler writes row to `pending-intros` collection, operator sees pending list in production-manager, fulfills it. Confirm future target so P2 schemas can include fields we will need.

### production-manager auth posture

Bind `127.0.0.1` alone is decided. Any additional soft-auth needed (e.g. a static token in the URL) to protect against accidental browser tab left open? Recommend: none for v1. Confirm.

### Scaffolding changes: new role or extend app-dev?

Extend `app-dev`. Confirm there are no reasons to split a `data-services` role. Recommend: no split.

### Commit discipline during execution chunks

Direct to `main` per `CLAUDE.md` — confirm chunk size stays small enough to honor this, or pre-approve worktree branch for which chunks.

## Format for each question in COORDINATION.md

```
### Q<n>. <short title>
- **Context:** one or two sentences
- **Options:**
  - A) ...
  - B) ...
  - C) ...
- **Recommendation:** <letter> — <one-line reason>
- **Operator answer:** _(blank — operator fills)_
```

Number questions `Q1`, `Q2`, ... Numbering continues across prompt runs.

## Do NOT in this prompt

- Do not modify `Docs/INFO-SERVICE-PLAN.md` content itself.
- Do not resolve any question.
- Do not add code, folders, or packages.
- Do not draft execution prompts.

## Finish

1. Save updated `COORDINATION.md` with new Open Questions.
2. Update chunk status: P1 → `merged`. Append Run Log row (leave Commit column as `uncommitted` — operator commits manually).
3. `WORKING.md` update.
4. Load `ops-change-summary` skill and show output.

**Do NOT run `git commit` or `git add`. Leave all changes staged-or-unstaged as you leave them. Operator handles commits.**

## Definition of done

- Every topic listed above has at least one question in `COORDINATION.md`.
- Each question follows the required format.
- Each question has a recommendation from you.
- `Docs/INFO-SERVICE-PLAN.md` untouched.
- Chunk status + run log updated.
- No git commit made by you.

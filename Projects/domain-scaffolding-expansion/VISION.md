---
id: domain-scaffolding-expansion-vision
type: vision
description: North-star vision for extending Actions-folder agent scaffolding principles to Apps, Creative, and Tools.
owner: ops
status: draft
---

# VISION — Domain Scaffolding Expansion

## Why This Exists

`Actions/` has matured into the repo's strongest agent-collaboration surface. Future agents working in `Apps/`, `Creative/`, and `Tools/` should encounter the same quality of guidance, the same drift defenses, and the same reuse coordination that `Actions/` provides today.

This document is a **north star, not a plan**. It states the end-state principles and the per-domain shape that end-state should take. Concrete phase plans, chunk prompts, and validator designs live in follow-up project work, not here.

## The Three Strengths (Source Pattern from `Actions/`)

The pattern being extended is the one already proven under `Actions/`:

### 1. Abundance of `AGENTS.md` files with progressive discovery

A domain root `AGENTS.md` routes to per-subfolder `AGENTS.md` files. Each guide is narrow, owns one slice of behavior, and lists its own required reading. Agents read top-down, stop at the smallest guide that covers their edit, and never load more context than the change warrants.

### 2. Behavior-over-implementation framing

Guides describe **contracts**: purpose, inputs, outputs, side effects, paste/run targets, failure behavior. They do not duplicate code or implementation detail. The script or app can be rewritten freely; the contract stays stable. Drift between intent and code is caught at validation, not at code review.

### 3. Coordination of shared names and reusable patterns

A single canonical-names file (`SHARED-CONSTANTS.md` in `Actions/`) and a topic-indexed helpers folder (`Helpers/`) prevent per-script duplication and per-script drift. Cross-cutting concerns have one home; consumers link instead of copying.

These three reinforce each other: many small guides only stay coherent because shared names and helpers exist; contracts only stay honest because validators check them; reuse only stays clean because guides point at it instead of inlining.

## End-State Goal for Each Domain

Each of `Apps/`, `Creative/`, and `Tools/` should reach a state where:

- A domain root `AGENTS.md` exists and routes to every subfolder.
- Every subfolder with editable work product has a local `AGENTS.md` that follows the same progressive-discovery pattern.
- Guides describe **what the work product must do**, not how it currently does it.
- A domain-appropriate analogue to `ACTION-CONTRACT` + SHA stamp pins each work product to its declared contract, with a mechanical validator that an agent can run before handoff.
- Shared names, shared configuration, and shared patterns live in one canonical place per domain, linked rather than duplicated.
- Cross-domain links (e.g. an Apps route consumed by an Actions script) are explicit on both sides and survive renames.

The exact mechanism for each strength will differ per domain. The principles do not.

## Per-Domain Shape

Each domain has its own native unit of work. Scaffolding must match the grain of that unit, not impose `Actions/`'s grain on it. The notes below describe shape only — they do not prescribe mechanism.

### Apps/

Native unit: **npm/pnpm-managed TypeScript apps** (`info-service`, `production-manager`, `stream-overlay`). Each app already has a local `AGENTS.md` and a plan/guide doc.

End-state shape:

- A top-level `Apps/AGENTS.md` exists as domain router, sibling to `Actions/AGENTS.md`.
- Each app's `AGENTS.md` declares a **service / package contract**: ports, base URLs, exported routes or modules, schema sources, write/read clients, and consumer apps.
- A canonical cross-app names file (the Apps analogue to `SHARED-CONSTANTS.md`) holds ports, base URLs, collection names, schema-version policies, and broker topics that more than one app or script depends on.
- A shared-patterns surface (the Apps analogue to `Helpers/`) holds reusable TypeScript patterns — zod schema conventions, atomic-write helpers, Fastify route conventions, build script conventions — that today are reinvented per app.
- Drift defense: a per-app contract block declares what the app exposes; a validator confirms package scripts, entry points, ports, and route inventories still match the contract.
- Cross-domain edges (info-service ↔ Actions/Intros, production-manager ↔ info-service, stream-overlay ↔ Overlay broker) are listed in both directions and validated as part of the same check.

### Creative/

Native unit: **content scaffolding** — prompt systems, character/canon references, story scaffolds, marketing copy systems. Output is creative artifacts, not runtime code.

End-state shape:

- A top-level `Creative/AGENTS.md` exists as domain router, sibling to `Actions/AGENTS.md`.
- Each creative sub-domain (`Art/`, `WorldBuilding/`, `Marketing/`) keeps its existing local guide and adds, where appropriate, a guide per scaffolded **agent / prompt system** so that prompt evolution is treated as first-class edit work.
- Guides describe **pipeline contract**: input source, prompt structure, expected output shape, canon-guardian touchpoints, approval gate, downstream consumers (e.g. which Streamer.bot actions or apps use the artifact).
- A canonical creative-names file holds character names, recurring locations, franchise tags, persona handles, asset path conventions, and approved-vs-experimental status — names that show up in multiple prompts and risk silent drift.
- A shared-patterns surface holds reusable prompt scaffolds, evaluation rubrics, and canon-check patterns currently inlined into individual agents.
- Drift defense: a per-agent / per-prompt-system contract block declares the canon dependencies, approval state, and downstream consumers; a validator confirms named entities still match the canonical names file and that consumers still resolve.
- The validator must be light enough that creative iteration is not slowed; canon-guardian remains the human gate for meaning-level changes.

### Tools/

Native unit: **local support utilities** — Python scripts, validators, sync helpers, pipeline runners. Output is automation, not runtime stream code.

End-state shape:

- A top-level `Tools/AGENTS.md` exists as domain router, sibling to `Actions/AGENTS.md`.
- Each tool subfolder (`StreamerBot/`, `MixItUp/`, `AgentTree/`, `ArtPipeline/`, `ContentPipeline/`, `LotAT/`) keeps its existing local guide and, where the subfolder hosts multiple distinct utilities, gains finer-grained guides per utility.
- Guides describe **tool contract**: invocation surface (CLI args / entry script), inputs read, outputs written, side effects, exit-code semantics, and the domain it supports (`Actions/`, an app, a pipeline).
- A canonical tools-names file holds shared paths, env var names, exit-code conventions, validator names, and the set of consumers each tool serves.
- A shared-patterns surface holds reusable Python patterns — argparse conventions, repo-relative path resolution, change-detection helpers, stamp/checksum patterns — that today are duplicated across pipelines.
- Drift defense: a per-tool contract block declares the CLI surface, inputs, outputs, and downstream consumers; a validator confirms the script still exposes the declared surface and that documented consumers still reference it.
- Tools that themselves validate other domains (e.g. `Tools/StreamerBot/Validation/action_contracts.py`) are treated as load-bearing and held to the strictest version of the contract+stamp discipline they enforce on others.

## Cross-Domain Concerns

Three concerns cut across all four domains (`Actions/` included) and must be preserved through the expansion:

- **Single source of truth for cross-domain names.** Ports, collection names, broker topics, character names, exit codes, and similar names should live in exactly one canonical file per domain. Cross-domain consumers link to the canonical entry; they do not duplicate the value.
- **Bidirectional edges.** When `Actions/Intros/` consumes an `info-service` route, both sides name the dependency. When `production-manager` writes a `user-intros` record, both sides name the contract. Validators should catch one-sided edges.
- **Validator UX.** Every drift defense added must produce a single command an agent can run before handoff. Multi-step validation, optional flags, or "remember to also run X" is the failure mode being avoided.

## Non-Goals

This vision deliberately does **not**:

- Pick the contract mechanism for any domain. (Apps may want a JSON block + SHA stamp like `Actions/`; Creative may want something lighter; Tools may want a CLI-introspection check. Each follow-up plan chooses.)
- Schedule the work. (Phase ordering, dependencies between domains, and chunk granularity belong to follow-up plans.)
- Mandate file names beyond the top-level routers. (`SHARED-CONSTANTS.md`, `Helpers/`, etc. are `Actions/`-native names; each domain may keep them, adapt them, or pick names that fit its grain better.)
- Touch domains outside `Apps/`, `Creative/`, `Tools/`. `Actions/` is the source pattern; other top-level folders (`Assets/`, `humans/`, run scripts) are out of scope.
- Migrate or rewrite existing work product. The goal is scaffolding around the existing work, not refactoring the work itself.

## Success Signals

The expansion is working when, in a fresh chat session, an agent asked to edit anywhere under `Apps/`, `Creative/`, or `Tools/`:

1. Lands on a clear domain router within one read.
2. Reaches the narrowest relevant local guide within two more reads.
3. Knows from the guide what the work product is contracted to do, without reading its source.
4. Knows which shared names and helpers exist and where to find them.
5. Has one command to run before handoff that catches drift between guide and work product.
6. Surfaces cross-domain impacts by reading the guide, not by grepping the repo.

## Follow-Up Work

This document is the first artifact of `Projects/domain-scaffolding-expansion/`. Subsequent artifacts in this project folder will:

- Audit per-domain baseline against the end-state shape above.
- Choose a contract mechanism per domain.
- Define the validator surface per domain.
- Break the rollout into phases and chunk prompts following the `Projects/` execution model.

No execution work begins until the follow-up plans exist.

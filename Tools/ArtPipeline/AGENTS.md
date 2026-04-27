---
id: tools-art-pipeline
type: domain-route
description: Art production tooling that turns visual canon into Stable Diffusion prompt, candidate, review, and publish manifests.
owner: art-director
secondaryOwners:
  - ops
workflows:
  - change-summary
  - validation
status: active
---

# Tools/ArtPipeline — Agent Guide

## Purpose

[Tools/ArtPipeline/](./) contains local Python tooling for repeatable art production runs. It converts stream visual canon into specs, deterministic prompts, generated candidates, review decisions, and published assets.

Use this folder for production mechanics around commander redemption overlays, future emotes, thumbnails, banners, character sheets, and LotAT scene or environment batches. The pipeline automates manifests, prompt assembly, backend calls, review state, and publish copies; `art-director` still owns taste, canon quality, and final approval standards.

## When to Activate

Use this guide when working on:

- [specify.py](./specify.py), [prompt.py](./prompt.py), [generate.py](./generate.py), or [review.py](./review.py)
- [asset_types.json](./asset_types.json) and supported art asset type defaults
- [config.py](./config.py), environment handling, backend setup, or data directory behavior
- [lib/canon_parser.py](./lib/canon_parser.py), [lib/prompt_builder.py](./lib/prompt_builder.py), [lib/backends.py](./lib/backends.py), or other pipeline helpers
- operator/developer documentation for art production tooling
- validation of whether art-pipeline docs still match code

Do not activate this guide for loose art brainstorming with no tooling, canon writing before art-agent files exist, or one-off prompt ideation that does not need pipeline manifests.

## Primary Owner

Primary owner: `art-director`.

`art-director` owns visual canon interpretation, prompt quality, asset approval standards, and whether generated candidates match the intended character and brand identity.

## Secondary Owners / Chain To

| Role | Chain when |
|---|---|
| `ops` | Environment setup, local validation, dependency checks, or terminal handoff formatting. |
| `brand-steward` | A visual decision affects public brand identity, character identity, or franchise-level canon. |
| `lotat-writer` | Asset generation depends on LotAT story/lore details or reusable adventure visuals. |
| `streamerbot-dev` | Published assets must be wired into Streamer.bot runtime actions. |

## Required Reading

Read these first for art-pipeline work:

1. [Tools/ArtPipeline/README.md](./README.md) — main operator/developer workflow.
2. [Tools/ArtPipeline/SETUP.md](./SETUP.md) — dependency and environment setup.
3. [Tools/ArtPipeline/FULL-RUN.md](./FULL-RUN.md) — real end-to-end run walkthrough.
4. [Tools/ArtPipeline/config.py](./config.py) — defaults, paths, environment variables, and directory rules.
5. [Tools/ArtPipeline/asset_types.json](./asset_types.json) — supported asset type registry.
6. [Tools/ArtPipeline/lib/canon_parser.py](./lib/canon_parser.py) — markdown parsing contract for style and character agents.
7. [Tools/ArtPipeline/lib/prompt_builder.py](./lib/prompt_builder.py) — deterministic prompt assembly rules.
8. [Creative/Art/Agents/stream-style-art-agent.md](../../Creative/Art/Agents/stream-style-art-agent.md) — shared stream style canon used by the parser.

## Local Workflow

The current four-phase flow is:

1. Specify asset needs from character/style canon.
2. Build deterministic prompt manifests from written specs.
3. Generate candidates through the configured Stable Diffusion backend.
4. Review candidates, approve one per spec, and publish the approved asset.

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id commanders-redemption-v1
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Important behavior:

- [specify.py](./specify.py) writes one spec manifest per batch.
- [prompt.py](./prompt.py) requires the spec manifest to exist, even in dry-run mode.
- [generate.py](./generate.py) requires the prompt manifest to exist, even in dry-run mode.
- [review.py](./review.py) copies approved images into [Creative/Art/Assets/](../../Creative/Art/Assets/) and writes prompt records into [Creative/Art/Projects/](../../Creative/Art/Projects/).
- Current backend support is ComfyUI only.
- There is no one-command orchestrator, overwrite mode, resume mode, automatic regeneration runner, Automatic1111 backend, ControlNet flow, inpainting flow, animation flow, or frontend build system.

## Validation

For a docs/tooling smoke test that avoids real image generation, use a written spec followed by dry-run downstream phases:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id docs-test
python3 Tools/ArtPipeline/prompt.py docs-test --dry-run
python3 Tools/ArtPipeline/prompt.py docs-test
python3 Tools/ArtPipeline/generate.py docs-test --dry-run
```

For dependency setup, install requirements before running the review UI or backend code:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

For agent-doc changes, follow [.agents/workflows/validation.md](../../.agents/workflows/validation.md) and run the agent-tree validator with the task-requested report path.

## Boundaries / Out of Scope

Do not use this folder to:

- invent or revise character canon without updating the relevant creative source first
- store generated production data as committed project state
- replace art-director approval with automatic publish decisions
- implement Streamer.bot runtime logic
- treat prompt generation as LLM-authored prose; current prompt assembly is deterministic and code-driven
- add broad image-generation features without documenting the manifest and operator workflow changes

## Handoff Notes

After changes, report:

- changed files in [Tools/ArtPipeline/](./)
- setup or dependency changes
- commands run and exact validation output
- whether manifest contracts changed between phases
- whether new asset types or character parser assumptions were introduced
- whether published asset behavior changed for [Creative/Art/Assets/](../../Creative/Art/Assets/) or [Creative/Art/Projects/](../../Creative/Art/Projects/)

## Runtime Notes

### Phase contracts

| Phase | Script | Consumes | Produces |
|---|---|---|---|
| Specify | [specify.py](./specify.py) | [asset_types.json](./asset_types.json), style agent, character agents | spec manifest |
| Prompt | [prompt.py](./prompt.py) | spec manifest, style/character canon, asset registry | prompt manifest |
| Generate | [generate.py](./generate.py) | prompt manifest, ComfyUI backend | candidate images and candidate manifest |
| Review | [review.py](./review.py) | candidate and prompt manifests | published asset and prompt record |

### Adding asset types

When adding an asset type:

1. Update [asset_types.json](./asset_types.json).
2. Define description, default requirements, and Stable Diffusion defaults.
3. Confirm the stream style rules support the target background/framing.
4. Extend [lib/prompt_builder.py](./lib/prompt_builder.py) if the type needs special negative exclusions.
5. Test specify, prompt, and generate dry-run behavior.
6. Update [README.md](./README.md), and this guide if the workflow changes.

### Adding characters

When adding a character to the pipeline:

1. Create or update the relevant file under [Creative/Art/Agents/](../../Creative/Art/Agents/).
2. Keep headings and bullet structure compatible with [lib/canon_parser.py](./lib/canon_parser.py).
3. Include enough canon for body, clothing, colors, eyes, expression, accessories, and avoid rules.
4. Run a one-character specify and prompt dry-run before real generation.

## Known Gotchas

- A dry-run specify pass does not write the manifest needed by prompt dry-run.
- Output collisions are rejected; use a new batch id or intentionally clean local state.
- Relative paths in environment files are resolved from [Tools/ArtPipeline/](./).
- Windows drive paths are converted for WSL-style environments by [config.py](./config.py).
- Published asset type behavior is partly inferred from prompt/spec context.

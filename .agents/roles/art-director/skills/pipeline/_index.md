# Pipeline Tooling — art-director

## Purpose

Covers the local Python tooling in `Tools/ArtPipeline/` that converts visual canon into repeatable art production runs:
- specify asset needs
- assemble deterministic prompts
- generate candidate images
- review one candidate down to one published asset

Load this skill area when the task is about how the art pipeline works, how to operate it, or how to maintain its docs/code.

## When to Load

- Running the art pipeline for real asset production
- Verifying how `Tools/ArtPipeline/` behaves before documenting or modifying it
- Adding or changing supported asset types in `asset_types.json`
- Adding new character canon files that must work with pipeline parsing
- Troubleshooting spec creation, prompt assembly, generation, review, or publishing
- Updating operator docs for the art pipeline
- Checking whether art-director documentation still matches implemented code

## What the Pipeline Is For

The pipeline is the repeatable production path for art-director work when the task is:
- commander redemption overlays
- future emotes
- future thumbnails/banners
- future character sheets
- future LotAT scene or environment asset batches

Use it when you want structured, reproducible, batchable generation with manifest files between phases.

Do **not** use it when the task is only:
- brainstorming loose visual directions
- writing new canon from scratch before character files exist
- doing one-off prompt ideation without pipeline manifests

## Current Pipeline Shape

```text
style agent + character agents + asset_types.json
  -> specify.py
  -> data/specs/

spec manifest
  -> prompt.py
  -> data/prompts/

prompt manifest
  -> generate.py
  -> data/candidates/ + candidate manifest

candidate manifest
  -> review.py
  -> Creative/Art/Assets/ + Creative/Art/Projects/
```

This is a diamond flow:
- one spec per requested character asset
- multiple generated candidates per spec
- one approved published asset per spec

## The Four Phases

### Phase 1 — Specify

Purpose:
- define what assets are needed for a batch
- lock character, asset type, and base requirements into a manifest

Command:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id commanders-redemption-v1
```

Important current behavior:
- validates the asset type key from `Tools/ArtPipeline/asset_types.json`
- requires `Creative/Art/Agents/stream-style-art-agent.md`
- requires `Creative/Art/Agents/<character>-art-agent.md`
- parses default expression from character canon when possible
- writes `Tools/ArtPipeline/data/specs/<batch_id>.json`
- `--dry-run` prints only and does not write the manifest

### Phase 2 — Prompt

Purpose:
- turn canon plus requirements into deterministic SD-ready prompt manifests

Command:

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
```

Important current behavior:
- reads the written spec manifest
- parses the shared style agent once
- parses each referenced character agent
- assembles positive/negative prompts in code via `lib/prompt_builder.py`
- copies generation defaults from the selected asset type
- sets `batch_count` from config unless overridden
- writes `Tools/ArtPipeline/data/prompts/<batch_id>.json`

### Phase 3 — Generate

Purpose:
- ask the configured Stable Diffusion backend for candidate images

Command:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
```

Important current behavior:
- reads the written prompt manifest
- supports `--spec` to generate one spec only
- supports backend and URL overrides
- health-checks the backend before real generation
- currently supports **ComfyUI only**
- writes candidate images under `Tools/ArtPipeline/data/candidates/<batch_id>/<spec_id>/`
- writes `Tools/ArtPipeline/data/candidates/<batch_id>.json`

### Phase 4 — Review

Purpose:
- reduce multiple candidates down to one approved asset and publish it

Command:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Important current behavior:
- serves a FastAPI review UI
- can approve/reject individual candidates
- can mark a spec for regeneration
- can publish one approved image into `Creative/Art/Assets/`
- writes a prompt record markdown file into `Creative/Art/Projects/`
- has a CLI shortcut for immediate approve+publish with `--approve <filename>`

## Real Verification Sequence

Because downstream dry-runs still require written manifests, the current valid smoke-test sequence is:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id docs-test
python3 Tools/ArtPipeline/prompt.py docs-test --dry-run
python3 Tools/ArtPipeline/prompt.py docs-test
python3 Tools/ArtPipeline/generate.py docs-test --dry-run
```

Do not assume this works:

```bash
python3 Tools/ArtPipeline/specify.py ... --dry-run
python3 Tools/ArtPipeline/prompt.py docs-test --dry-run
```

That fails today because `specify.py --dry-run` does not write the source manifest.

## How This Integrates With art-director Duties

The pipeline handles:
- manifest creation
- canon parsing
- deterministic prompt assembly
- backend calls
- candidate storage
- review UI state
- publishing file copies and prompt record creation

The art-director role still owns:
- visual canon quality
- whether character files are accurate enough to parse safely
- whether prompts actually reflect the intended identity
- whether an approved candidate matches brand and character canon
- whether a new asset type needs different framing, exclusions, or style rules
- deciding when regeneration is required instead of forcing a weak publish

Short version:
- the pipeline automates production mechanics
- the art-director owns taste, canon, and approval standards

## Source of Truth

Use these in order when validating claims:

1. `Tools/ArtPipeline/config.py` — defaults, paths, env vars, directory rules
2. the phase script itself — CLI behavior and runtime flow
3. `Tools/ArtPipeline/lib/` helpers — manifest schema, naming, parser, prompt assembly, backend behavior
4. `Tools/ArtPipeline/README.md` — operator/developer workflow documentation
5. `Tools/ArtPipeline/SETUP.md` and `Tools/ArtPipeline/FULL-RUN.md` — setup and walkthrough docs

If docs disagree with code, fix the docs to match the code unless the task explicitly includes changing behavior.

## Adding New Asset Types

Primary file:
- `Tools/ArtPipeline/asset_types.json`

Checklist:
1. add a new top-level asset type key
2. define `description`
3. define `default_requirements`
4. define `sd_defaults`
5. make sure the style agent has a compatible background rule if needed
6. extend `lib/prompt_builder.py` if the type needs special negative exclusions
7. test one full specify → prompt → generate dry run path
8. update docs if the operator workflow changed

Important nuance:
- prompt background behavior is partly inferred from asset type slug fragments like `overlay`, `panel`, `thumbnail`, `banner`, `character-sheet`, and `emote`
- if you invent a new slug with no overlap, you may also need to expand the style background table or prompt normalization logic

## Adding New Characters

Primary file location:
- `Creative/Art/Agents/<character-slug>-art-agent.md`

Checklist:
1. create the new character art agent file
2. keep the headings/parser structure compatible with `lib/canon_parser.py`
3. include enough canon for body, clothing, colors, eyes, expression, accessories, and avoid rules
4. run `specify.py` for that character
5. run `prompt.py --dry-run` on the written batch
6. inspect the prompt output before generating images

Important nuance:
- the parser reads markdown structure, not freeform intent
- this is not an LLM reading arbitrary prose; formatting quality matters

## Maintenance Expectations

- Keep each phase independently runnable from the CLI.
- Keep manifests as the explicit contract between phases.
- Keep `README.md` operator/developer focused.
- Keep this skill focused on operational truth and maintenance expectations.
- Keep `Tools/ArtPipeline/data/` treated as local state.
- Keep review UI behavior dependency-light: FastAPI plus inline HTML/CSS/JS.

## What This Pipeline Does Not Do

Current non-goals / not-implemented items:
- no LLM-written prompt generation
- no ControlNet or pose-control integration
- no inpainting/outpainting workflow
- no animation or video generation
- no frontend build system
- no one-command orchestrator
- no overwrite/resume mode
- no automatic rerun after regeneration is requested
- no Automatic1111 backend yet

## Related Files

| Path | Why it matters |
|---|---|
| `Tools/ArtPipeline/README.md` | Main pipeline reference |
| `Tools/ArtPipeline/SETUP.md` | Dependency and environment setup |
| `Tools/ArtPipeline/FULL-RUN.md` | Full real run walkthrough |
| `Tools/ArtPipeline/config.py` | Settings and path resolution |
| `Tools/ArtPipeline/lib/canon_parser.py` | Character/style markdown parsing contract |
| `Tools/ArtPipeline/lib/prompt_builder.py` | Deterministic prompt assembly rules |
| `../core.md` | Base art-director expectations and prompt structure |
| `../characters/_index.md` | Character canon routing |
| `../stream-style/_index.md` | Shared stream style routing |

## After Code Changes

Chain to ops and load `ops-change-summary` before handing work back to the operator.

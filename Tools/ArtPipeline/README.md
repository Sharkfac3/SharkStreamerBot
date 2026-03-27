# Art Pipeline

Local Python tooling for turning Starship Shamples visual canon into reviewable image candidates and one published asset.

This README is the developer/agent reference for the tooling in `Tools/ArtPipeline/`.

## Pipeline Overview

The art pipeline is split into four independently runnable phases:

```text
character agents + style agent + asset type registry
  ↓
specify.py
  ↓  data/specs/<batch_id>.json

prompt.py
  ↓  data/prompts/<batch_id>.json

generate.py
  ↓  data/candidates/<batch_id>/<spec_id>/*.png
     data/candidates/<batch_id>.json

review.py
  ↓  Creative/Art/Assets/<final-asset>.png
     Creative/Art/Projects/<batch_id>-<spec_id>.md
```

The workflow is intentionally diamond-shaped:
- one spec per character asset request
- multiple generated candidates per spec
- one approved and published asset per spec

Current V1 scope is commander redemption overlays for:
- Captain Stretch
- The Director
- Water Wizard

The implementation is already structured to extend into other asset types through `asset_types.json`.

## Repository Layout

- `config.py` — shared settings loader with `.env` support
- `asset_types.json` — asset type registry and per-type defaults
- `specify.py` — character + asset type → spec manifest
- `prompt.py` — spec manifest → deterministic prompt manifest
- `generate.py` — prompt manifest → candidate images + candidate manifest
- `review.py` — review UI and publish flow
- `lib/manifest_io.py` — manifest read/write helpers and batch IDs
- `lib/naming.py` — spec IDs and published asset filenames
- `lib/canon_parser.py` — markdown parser for style/character canon files
- `lib/prompt_builder.py` — deterministic positive/negative prompt assembly
- `lib/backends.py` — Stable Diffusion backend abstraction
- `data/` — local working outputs; gitignored

## Prerequisites

### Python

Use `python3` for the current environment.

Recommended minimum: Python 3.11+.

### Python dependencies

Install the ArtPipeline requirements:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

Current package requirements:
- `fastapi`
- `uvicorn`
- `httpx`
- `Pillow`

### Local Stable Diffusion backend

The current implementation supports **ComfyUI only**.

Expected defaults:
- backend: `comfyui`
- URL: `http://127.0.0.1:8188`

`generate.py` expects a reachable ComfyUI instance that supports:
- `GET /system_stats` for health checks
- `POST /prompt` for workflow submission
- `GET /history/<prompt_id>` for polling
- `GET /view?...` for image fetches

Checkpoint selection is controlled by environment variables read in `lib/backends.py`:
- `ART_PIPELINE_SD_MODEL`
- fallback: `COMFYUI_CHECKPOINT`
- default: `v1-5-pruned-emaonly.safetensors`

## Additional Docs

- `Tools/ArtPipeline/SETUP.md` — environment and dependency setup
- `Tools/ArtPipeline/FULL-RUN.md` — full end-to-end walkthrough

## Quick Start

For a real full pass with all three commanders, run the phases in order:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id commanders-redemption-v1
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Notes:
- `specify.py --dry-run` prints JSON but does **not** write `data/specs/<batch>.json`.
- `prompt.py --dry-run` still requires the source spec manifest to exist.
- `generate.py --dry-run` still requires the source prompt manifest to exist.
- there is no single orchestrator script yet
- there is no `--force` or resume support yet

## Phase Reference

### Phase 1 — `specify.py`

Responsibilities:
- load the asset type registry from `asset_types.json`
- normalize and dedupe character slugs
- require the shared style agent and each character agent file
- parse default expression from each character art agent when not overridden
- build one spec entry per character
- write one spec manifest for the batch

CLI:

```bash
python3 Tools/ArtPipeline/specify.py \
  --asset-type redemption-overlay \
  --characters captain-stretch the-director water-wizard \
  --batch-id commanders-redemption-v1
```

Arguments:
- `--asset-type` — required asset type key from `Tools/ArtPipeline/asset_types.json`
- `--characters` — required list of one or more character slugs
- `--batch-id` — optional explicit batch ID; auto-generated if omitted
- `--expression` — optional override applied to every generated spec
- `--pose` — optional override applied to every generated spec
- `--notes` — optional note stored on each spec
- `--dry-run` — print manifest JSON without writing the file

Consumes:
- `Tools/ArtPipeline/asset_types.json`
- `Creative/Art/Agents/stream-style-art-agent.md`
- `Creative/Art/Agents/<character>-art-agent.md`

Produces:
- `Tools/ArtPipeline/data/specs/<batch_id>.json`

Spec manifest shape includes:
- `schema_version`
- `batch_id`
- `created_at_utc`
- `asset_type`
- `specs[]`

Each spec entry includes:
- `spec_id`
- `character`
- `character_agent`
- `style_agent`
- `asset_type`
- `requirements`
- `status`
- `notes`

Important current behavior:
- if `--batch-id` is omitted, `generate_batch_id()` builds a stable slug from characters + asset type + `v1`
- duplicate character inputs are deduped in order
- default pose is `standing`
- default expression falls back to `neutral` if parsing fails
- existing output files are rejected; use a different batch ID

### Phase 2 — `prompt.py`

Responsibilities:
- read a spec manifest from `data/specs/`
- load and parse the shared style agent once
- load and parse each referenced character agent
- build deterministic positive and negative prompts
- merge generation defaults from the asset type registry
- assign batch count and default seed behavior
- write one prompt manifest for the batch

CLI:

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
```

Arguments:
- `batch_id` — required source spec batch ID
- `--batch-count` — optional override for candidate count per prompt
- `--dry-run` — print manifest JSON without writing the file

Consumes:
- `Tools/ArtPipeline/data/specs/<batch_id>.json`
- style and character agent paths stored in the spec manifest
- `Tools/ArtPipeline/asset_types.json`

Produces:
- `Tools/ArtPipeline/data/prompts/<batch_id>.json`

Prompt manifest shape includes:
- `schema_version`
- `batch_id`
- `created_at_utc`
- `source_spec`
- `prompts[]`

Each prompt entry includes:
- `spec_id`
- `character`
- `positive_prompt`
- `negative_prompt`
- `generation_params`
- `canon_sources`
- `status`

Important current behavior:
- status is written as `ready`
- `batch_count` defaults to `ART_PIPELINE_DEFAULT_BATCH_COUNT` or `4`
- `seed` defaults to `-1` unless overridden later
- prompt assembly is deterministic and code-driven, not LLM-generated
- prompt output does **not** currently include a first-class `asset_type` field in each prompt entry; downstream publish logic infers asset type from `spec_id` when needed

### Phase 3 — `generate.py`

Responsibilities:
- read a prompt manifest from `data/prompts/`
- optionally filter to one `spec_id`
- create the configured SD backend
- run a backend health check
- submit generation requests and save candidate images
- write one candidate manifest summarizing images and any failures

CLI:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
```

Arguments:
- `batch_id` — required source prompt batch ID
- `--spec` — optional single-spec filter
- `--dry-run` — print planned generation requests without creating images
- `--backend` — optional backend override
- `--url` — optional backend URL override

Consumes:
- `Tools/ArtPipeline/data/prompts/<batch_id>.json`
- configured SD backend

Produces:
- `Tools/ArtPipeline/data/candidates/<batch_id>/<spec_id>/<spec_id>_<nnn>.png`
- `Tools/ArtPipeline/data/candidates/<batch_id>.json`

Candidate manifest shape includes:
- `schema_version`
- `batch_id`
- `created_at_utc`
- `source_prompts`
- `backend`
- `backend_url`
- `candidates[]`
- optional `errors[]`

Important current behavior:
- dry run still performs backend creation and a health check printout
- real generation stops if the backend health check fails
- current backend support is `comfyui` only
- each prompt is generated `batch_count` times as separate one-image requests
- output collisions are rejected; there is no resume/overwrite mode
- failed specs are recorded as `generation_failed` in the candidate manifest

### Phase 4 — `review.py`

Responsibilities:
- load the candidate manifest and linked prompt manifest
- infer review state per spec from candidate image statuses
- serve a local FastAPI review UI
- approve one candidate per spec
- reject candidates
- mark a spec for regeneration
- publish an approved candidate to `Creative/Art/Assets/`
- write a prompt record markdown file to `Creative/Art/Projects/`

CLI:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Arguments:
- `batch_id` — required candidate batch ID
- `--host` — bind host, default `127.0.0.1`
- `--port` — bind port, default from config or `8766`
- `--approve <filename>` — approve and publish a candidate immediately, then exit
- `--reload` — enable uvicorn reload for local development

Consumes:
- `Tools/ArtPipeline/data/candidates/<batch_id>.json`
- `Tools/ArtPipeline/data/prompts/<batch_id>.json`
- candidate image files under `data/candidates/<batch_id>/...`

Produces:
- updated candidate manifest status fields
- `Creative/Art/Assets/<published-filename>.<ext>`
- `Creative/Art/Projects/<batch_id>-<spec_id>.md`

Current review statuses:
- image statuses: `pending_review`, `approved`, `rejected`
- spec statuses: `pending_review`, `approved`, `published`, `needs_regeneration`, `generation_failed`

Important current behavior:
- publish requires one approved candidate
- published assets are copied, not moved, from candidate storage
- publish writes a markdown prompt record with prompts, params, and source files
- asset naming is deterministic through `lib/naming.py`
- published filename collisions are rejected
- regeneration currently marks state and stores the suggested command; it does not auto-run generation

## Configuration

`config.py` is the source of truth for ArtPipeline settings. Optional overrides live in `Tools/ArtPipeline/.env`.

Supported `ART_PIPELINE_*` variables:

- `ART_PIPELINE_DATA_DIR`
  - default: `Tools/ArtPipeline/data`
- `ART_PIPELINE_SPECS_DIR`
  - default: `Tools/ArtPipeline/data/specs`
- `ART_PIPELINE_PROMPTS_DIR`
  - default: `Tools/ArtPipeline/data/prompts`
- `ART_PIPELINE_CANDIDATES_DIR`
  - default: `Tools/ArtPipeline/data/candidates`
- `ART_PIPELINE_ASSETS_DIR`
  - default: `Creative/Art/Assets`
- `ART_PIPELINE_PROJECTS_DIR`
  - default: `Creative/Art/Projects`
- `ART_PIPELINE_ASSET_TYPES_PATH`
  - default: `Tools/ArtPipeline/asset_types.json`
- `ART_PIPELINE_SD_BACKEND`
  - default: `comfyui`
- `ART_PIPELINE_SD_URL`
  - default: `http://127.0.0.1:8188`
- `ART_PIPELINE_DEFAULT_BATCH_COUNT`
  - default: `4`
- `ART_PIPELINE_REVIEW_PORT`
  - default: `8766`
- `ART_PIPELINE_SD_MODEL`
  - used by `lib/backends.py` for ComfyUI checkpoint selection
  - default: unset, then falls back to `COMFYUI_CHECKPOINT`, then `v1-5-pruned-emaonly.safetensors`

Path handling notes:
- relative paths in `.env` are resolved from `Tools/ArtPipeline/`
- Windows drive paths are converted for WSL-style Linux environments
- environment variables override `.env` file values

## Asset Type Registry

`Tools/ArtPipeline/asset_types.json` is the registry for supported asset types.

Current implemented entry:
- `redemption-overlay`

Current shape:

```json
{
  "redemption-overlay": {
    "description": "...",
    "default_requirements": {
      "width": 512,
      "height": 768,
      "format": "png",
      "transparent_background": true,
      "framing": "upper-body"
    },
    "sd_defaults": {
      "steps": 30,
      "cfg_scale": 7.0,
      "sampler": "euler_a",
      "denoising_strength": 1.0
    }
  }
}
```

How it is used:
- `specify.py` copies `default_requirements` into each spec and adds expression/pose
- `prompt.py` validates that the asset type exists
- `lib/prompt_builder.py` reads the asset type settings into `generation_params`
- published asset type naming is inferred later from prompt/spec context

How to add a new asset type:
1. add a new top-level key in `asset_types.json`
2. define `description`
3. define `default_requirements`
4. define `sd_defaults`
5. choose values that match the actual target output shape and SD defaults
6. run a spec + prompt pass to confirm the new fields flow correctly
7. update pipeline docs and art-director skill docs if the workflow expectations changed

Current implementation caveat:
- style background rules are matched by slug fragments like `overlay`, `panel`, `thumbnail`, `banner`, `character-sheet`, and `emote`
- new asset types work best when their slug shares a recognizable token with an existing style background rule, or when the style agent's background table is expanded accordingly

## Extending the Pipeline

### Add a new character

1. create `Creative/Art/Agents/<character-slug>-art-agent.md`
2. make sure its markdown structure is parsable by `lib/canon_parser.py`
3. include enough canon for body, clothing, colors, eyes, expression, and avoid rules
4. run:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters <character-slug> --batch-id test-<character-slug>
python3 Tools/ArtPipeline/prompt.py test-<character-slug> --dry-run
```

Parser-sensitive headings currently include things like:
- `Core Rules`
- `Body Structure`
- `Clothing`
- `Eyes`
- `Expression`
- `What to Avoid`

The parser is tolerant, but not freeform. This is code-parsed canon, not an LLM reading arbitrary prose.

### Add a new asset type

1. update `Tools/ArtPipeline/asset_types.json`
2. confirm the shared style agent has an appropriate background rule if needed
3. if the new type needs special exclusions, extend `ASSET_TYPE_NEGATIVES` in `lib/prompt_builder.py`
4. if the publish filename should behave differently, update `lib/naming.py`
5. test with a one-character batch through specify/prompt/generate/review

### Add a new backend

1. implement a new backend class in `lib/backends.py`
2. make it satisfy the `GenerationBackend` protocol:
   - `health_check()`
   - `generate(request)`
3. update `create_backend()` routing
4. document any backend-specific env vars or constraints
5. verify dry-run and real generation flows

## Troubleshooting

### `python: command not found`

Use `python3` in this environment.

### `prompt.py` says the spec manifest is missing

`prompt.py` requires `Tools/ArtPipeline/data/specs/<batch_id>.json` to exist.

If you ran:

```bash
python3 Tools/ArtPipeline/specify.py ... --dry-run
```

that printed JSON only. Re-run `specify.py` without `--dry-run`.

### `generate.py` says the prompt manifest is missing

`generate.py` requires `Tools/ArtPipeline/data/prompts/<batch_id>.json` to exist.

Run `prompt.py` without `--dry-run` first.

### Generate health check is unavailable

The current backend health check calls ComfyUI at `ART_PIPELINE_SD_URL` using `/system_stats`.

Verify:
- ComfyUI is running
- the URL is correct
- the repo environment can reach the service
- the selected checkpoint exists in ComfyUI

### `review.py` says FastAPI or uvicorn is missing

Install requirements:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

### Existing manifest or candidate directory blocks a rerun

Current implementation rejects overwriting:
- spec manifest collisions
- prompt manifest collisions
- candidate manifest collisions
- candidate output directory collisions
- published asset filename collisions

Use a new `--batch-id`, or manually clean local `data/` state if appropriate.

### Character expression parsing falls back to `neutral`

`specify.py` parses expressions from the character art agent markdown. If the parser cannot find a usable value, it warns and falls back to `neutral`.

Check the character agent headings and bullet structure.

### Published asset type looks inferred rather than explicit

That is current behavior. `review.py` publishes using asset type inferred from prompt/spec context because prompt entries do not currently store `asset_type` explicitly.

If that becomes fragile for new types, update the prompt manifest contract first, then document the change.

## Agent Notes

- Keep each phase independently runnable from the CLI.
- Treat `config.py` as the canonical settings contract.
- Keep manifest schemas stable, or update the agent skill docs when changing them.
- Keep `data/` local-only.
- The review UI is plain FastAPI + inline HTML/CSS/JS; there is no frontend build step.
- The prompt builder is deterministic and canon-driven; there is no LLM prompt generation stage in the current implementation.

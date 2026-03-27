# Art Pipeline Setup

Developer/agent setup reference for `Tools/ArtPipeline/`.

Use this file for technical environment preparation. Human operator setup instructions live in `humans/art-pipeline/SETUP.md`. Use `README.md` for pipeline behavior and `FULL-RUN.md` for the end-to-end walkthrough.

## 1. Python

Use `python3` in the current project environment.

Check it:

```bash
python3 --version
```

Recommended minimum: Python 3.11+.

## 2. Install Python dependencies

From the repo root:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

Current requirements file includes:
- `fastapi`
- `uvicorn`
- `httpx`
- `Pillow`

## 3. Optional `.env` configuration

Copy the example file if you want local overrides:

```bash
cp Tools/ArtPipeline/.env.example Tools/ArtPipeline/.env
```

Supported ArtPipeline variables:
- `ART_PIPELINE_DATA_DIR`
- `ART_PIPELINE_SPECS_DIR`
- `ART_PIPELINE_PROMPTS_DIR`
- `ART_PIPELINE_CANDIDATES_DIR`
- `ART_PIPELINE_ASSETS_DIR`
- `ART_PIPELINE_PROJECTS_DIR`
- `ART_PIPELINE_ASSET_TYPES_PATH`
- `ART_PIPELINE_SD_BACKEND`
- `ART_PIPELINE_SD_URL`
- `ART_PIPELINE_DEFAULT_BATCH_COUNT`
- `ART_PIPELINE_REVIEW_PORT`
- `ART_PIPELINE_SD_MODEL`

Backend compatibility variable also supported by `lib/backends.py`:
- `COMFYUI_CHECKPOINT`

Notes:
- environment variables override `.env`
- relative paths are resolved from `Tools/ArtPipeline/`
- Windows drive paths are converted for WSL-style environments

## 4. Start local Stable Diffusion

The current implementation supports **ComfyUI only**.

Default expected endpoint:

```text
http://127.0.0.1:8188
```

ComfyUI must support:
- `GET /system_stats`
- `POST /prompt`
- `GET /history/<prompt_id>`
- `GET /view?...`

Checkpoint resolution order:
1. `ART_PIPELINE_SD_MODEL`
2. `COMFYUI_CHECKPOINT`
3. `v1-5-pruned-emaonly.safetensors`

## 5. Verify dry-run prerequisites

The safest initial smoke test is phase-by-phase.

### Specify dry run

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id setup-check --dry-run
```

This confirms:
- Python runs
- the asset registry loads
- style and character agent files are found
- character canon parsing works enough to build specs

### Prompt dry run requires a written spec

Because downstream dry-runs still read real manifests, write the spec first:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id setup-check
python3 Tools/ArtPipeline/prompt.py setup-check --dry-run
```

### Generate dry run requires a written prompt manifest

```bash
python3 Tools/ArtPipeline/prompt.py setup-check
python3 Tools/ArtPipeline/generate.py setup-check --dry-run
```

That confirms:
- prompt manifest loading works
- backend creation works
- health check path is reachable or at least reports clearly
- request planning matches expected width/height/count values

## 6. Review UI dependencies

To launch the review UI later:

```bash
python3 Tools/ArtPipeline/review.py <batch-id>
```

Default review port is `8766` unless overridden with `ART_PIPELINE_REVIEW_PORT` or `--port`.

## 7. Local output locations

Default working outputs:
- `Tools/ArtPipeline/data/specs/`
- `Tools/ArtPipeline/data/prompts/`
- `Tools/ArtPipeline/data/candidates/`

Published outputs:
- `Creative/Art/Assets/`
- `Creative/Art/Projects/`

`Tools/ArtPipeline/data/` is local working state and should stay uncommitted.

## Common setup problems

### `python3` works but `python` does not

That is expected in the current shell. Use `python3`.

### `FastAPI is not installed`

Install requirements:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

### Generate health check says unavailable

ComfyUI is not reachable at the configured URL. Check:
- ComfyUI is running
- the port is correct
- firewall/network rules allow access
- the environment can reach `127.0.0.1:8188` if using defaults

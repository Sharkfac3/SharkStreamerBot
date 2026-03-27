# Art Pipeline Setup

Human setup checklist for `Tools/ArtPipeline/`.

Use this before trying a real run.

## Goal

By the end of this setup, you should be able to:
- run `python3`
- install the ArtPipeline Python requirements
- point the pipeline at a working local ComfyUI instance
- complete a safe phase-by-phase smoke test

## 1. Confirm Python works

From the repo root:

```bash
python3 --version
```

You want a real version response, ideally Python 3.11+.

If `python3` does not work, stop here and fix Python first.

## 2. Install the required Python packages

From the repo root:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

This installs the dependencies used by the review UI and image pipeline scripts.

## 3. Optional: create a local `.env`

If you want to override defaults, copy the example file:

```bash
cp Tools/ArtPipeline/.env.example Tools/ArtPipeline/.env
```

You do **not** need to change anything if the defaults already match your setup.

The most likely values a human operator might care about are:
- `ART_PIPELINE_SD_URL`
- `ART_PIPELINE_REVIEW_PORT`
- `ART_PIPELINE_DEFAULT_BATCH_COUNT`
- output directory paths, if you intentionally want custom locations

## 4. Start ComfyUI

The current implementation supports **ComfyUI only**.

Default expected URL:

```text
http://127.0.0.1:8188
```

Before moving on, make sure ComfyUI is actually running.

If you use a different port or host, set `ART_PIPELINE_SD_URL` in `Tools/ArtPipeline/.env`.

Optional model selection variables:
- `ART_PIPELINE_SD_MODEL`
- `COMFYUI_CHECKPOINT`

## 5. Safe smoke test: specify dry run

Run this first:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id setup-check --dry-run
```

What success looks like:
- JSON prints to the terminal
- no missing-file error for the style or character agents
- no asset-type validation error

What this proves:
- Python runs
- the pipeline scripts run
- the art agent files are being found
- spec creation logic is working

## 6. Safe smoke test: prompt dry run

Important: `prompt.py --dry-run` still needs a real spec file.

So first write the spec file:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id setup-check
```

Then dry-run prompt generation:

```bash
python3 Tools/ArtPipeline/prompt.py setup-check --dry-run
```

What success looks like:
- prompt JSON prints to the terminal
- you see prompt entries for all three commanders
- each prompt has generation settings like width, height, steps, cfg, sampler, and batch count

## 7. Safe smoke test: generate dry run

Important: `generate.py --dry-run` still needs a real prompt manifest.

So write it first:

```bash
python3 Tools/ArtPipeline/prompt.py setup-check
```

Then run the dry generation plan:

```bash
python3 Tools/ArtPipeline/generate.py setup-check --dry-run
```

What success looks like:
- the script prints a backend health line
- the script prints one planned generation block per spec
- each spec shows the expected candidate count and image size

If the health line says `unavailable`, your ComfyUI server is not reachable yet.

## 8. Know where files go

Temporary pipeline outputs:
- `Tools/ArtPipeline/data/specs/`
- `Tools/ArtPipeline/data/prompts/`
- `Tools/ArtPipeline/data/candidates/`

Published outputs later:
- `Creative/Art/Assets/`
- `Creative/Art/Projects/`

## Common setup problems

### `python: command not found`

Use `python3`.

### `FastAPI is not installed` or `uvicorn is not installed`

Run:

```bash
pip install -r Tools/ArtPipeline/requirements.txt
```

### Generate says the backend is unavailable

Check these in order:
1. ComfyUI is open and running
2. the URL is correct
3. `ART_PIPELINE_SD_URL` matches the real server address
4. your shell/environment can reach that server

### Prompt dry run says the spec file is missing

That means you only ran `specify.py --dry-run`.

Run the real specify command once so the spec JSON is written.

### Generate dry run says the prompt file is missing

That means you only ran `prompt.py --dry-run`.

Run the real prompt command once so the prompt JSON is written.

## Next step

Once setup checks pass, continue with:
- `humans/art-pipeline/FULL-RUN.md`

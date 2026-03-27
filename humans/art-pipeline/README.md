# Art Pipeline

Human operator guide for `Tools/ArtPipeline/`.

This folder exists to keep human runbooks separate from agent-facing implementation notes. If you are trying to actually set up, test, and run the pipeline by hand, start here.

## What this pipeline does

The Art Pipeline helps you turn character canon into finished image assets in four phases:

1. **Specify** — define which assets you want for which characters
2. **Prompt** — build Stable Diffusion-ready prompts from the canon files
3. **Generate** — ask your local SD backend for multiple image candidates
4. **Review** — choose the winner and publish it into the real art folders

The flow is diamond-shaped:
- one request per character asset
- multiple generated image options
- one approved final asset

## Current tested scope

Right now the pipeline is set up and documented around one real use case:
- **commander redemption overlays** for:
  - Captain Stretch
  - The Director
  - Water Wizard

The code is built to grow later, but this is the safest starting point for a human test run.

## Where to go next

- If you need to install or verify dependencies: `humans/art-pipeline/SETUP.md`
- If you want a real end-to-end test run: `humans/art-pipeline/FULL-RUN.md`

## What gets created

Working files during the run:
- `Tools/ArtPipeline/data/specs/`
- `Tools/ArtPipeline/data/prompts/`
- `Tools/ArtPipeline/data/candidates/`

Published outputs after approval:
- `Creative/Art/Assets/`
- `Creative/Art/Projects/`

## Important operator expectations

- Use `python3`, not `python`, in the current environment.
- The current backend is **ComfyUI only**.
- There is no one-click orchestrator yet; you run each phase manually.
- There is no safe overwrite/resume mode yet; use a fresh batch ID when testing.
- `--dry-run` is phase-local only. Downstream dry-runs still need the previous manifest file to exist.

## Recommended first goal

If you are touching this pipeline for the first time, do this in order:

1. follow `SETUP.md`
2. run the dry checks in `SETUP.md`
3. do the real walkthrough in `FULL-RUN.md`

## Where the more technical notes live

- `Tools/ArtPipeline/README.md`
- `Tools/ArtPipeline/SETUP.md`
- `Tools/ArtPipeline/FULL-RUN.md`
- `.agents/roles/art-director/skills/pipeline/_index.md`

Those files are better for maintenance and implementation details. This `humans/` copy is the simpler operator-facing path.

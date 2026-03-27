# Art Pipeline Full Run

Developer/agent end-to-end walkthrough for the current four-phase ArtPipeline.

Use this file when you want to trace the implemented pipeline behavior phase by phase. Human operator run instructions live in `humans/art-pipeline/FULL-RUN.md`. Use `SETUP.md` first if dependencies are not ready yet.

## Example: all three commander redemption overlays

This walkthrough uses the current V1 asset type:
- `redemption-overlay`

and the current three commanders:
- `captain-stretch`
- `the-director`
- `water-wizard`

Batch ID used below:
- `commanders-redemption-v1`

## Phase 1 — write the spec manifest

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id commanders-redemption-v1
```

Expected output:
- `Tools/ArtPipeline/data/specs/commanders-redemption-v1.json`

What this phase does:
- validates the asset type exists
- loads the shared stream style agent
- loads each character art agent
- creates one spec per character
- fills expression from character canon unless you override it

## Phase 2 — write the prompt manifest

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
```

Expected output:
- `Tools/ArtPipeline/data/prompts/commanders-redemption-v1.json`

What this phase does:
- parses the style and character canon files
- assembles deterministic positive and negative prompts
- merges asset type SD defaults
- sets candidate batch count

Optional dry inspection before writing:

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1 --dry-run
```

## Phase 3 — generate candidates

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
```

Expected outputs:
- candidate images under `Tools/ArtPipeline/data/candidates/commanders-redemption-v1/`
- candidate manifest at `Tools/ArtPipeline/data/candidates/commanders-redemption-v1.json`

What this phase does:
- health-checks the configured backend
- sends one request per candidate image
- writes image files under each spec folder
- records seeds and generation times

Useful variations:

Generate one spec only:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --spec captain-stretch-redemption-overlay
```

Preview generation plan only:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --dry-run
```

Override backend URL:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --url http://127.0.0.1:8188
```

## Phase 4 — review and publish

Launch the review UI:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Then open:

```text
http://127.0.0.1:8766
```

What to do in review:
1. open a spec
2. select one candidate
3. approve it, or reject candidates, or mark the spec for regeneration
4. publish the approved candidate

Expected publish outputs per approved spec:
- `Creative/Art/Assets/<final-file>.png`
- `Creative/Art/Projects/commanders-redemption-v1-<spec_id>.md`

## CLI shortcut: approve and publish one known candidate

If you already know the exact candidate filename:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1 --approve captain-stretch-redemption-overlay_001.png
```

That command:
- finds the owning spec
- approves that candidate
- publishes it immediately
- exits

## Dry-run sequence that matches the current implementation

Because downstream dry-runs still require written manifests, this is the valid sequence for a doc/test pass:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id docs-test
python3 Tools/ArtPipeline/prompt.py docs-test --dry-run
python3 Tools/ArtPipeline/prompt.py docs-test
python3 Tools/ArtPipeline/generate.py docs-test --dry-run
```

Why this matters:
- `specify.py --dry-run` prints only; it does not write the spec file
- `prompt.py --dry-run` still reads the real spec file
- `generate.py --dry-run` still reads the real prompt file

## Cleanup between runs

Current implementation does not support overwrite/resume.

If you rerun the same batch ID, these may block the run:
- existing spec manifest
- existing prompt manifest
- existing candidate manifest
- existing candidate output directory
- existing published asset filename

Simplest fix:
- use a new batch ID for each pass

## Current limitations

- no orchestrator script yet
- no auto-regeneration; review stores the command for you to run manually
- no overwrite or resume mode
- no Automatic1111 backend
- no ControlNet integration
- no animation/video generation
- no LLM-based prompt-writing phase

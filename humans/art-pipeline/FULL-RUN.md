# Art Pipeline Full Run

Human operator walkthrough for a real end-to-end ArtPipeline test.

This guide assumes you already completed:
- `humans/art-pipeline/SETUP.md`

## Test goal

Run one full batch for all three commander redemption overlays:
- Captain Stretch
- The Director
- Water Wizard

Asset type:
- `redemption-overlay`

Suggested test batch ID:
- `commanders-redemption-v1`

If you already used that batch ID before, pick a new one like:
- `commanders-redemption-v2`
- `commanders-redemption-test-01`

## Before you start

Make sure:
- `python3` works
- dependencies are installed
- ComfyUI is running
- you are in the repo root

## Phase 1 — create the spec manifest

Run:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id commanders-redemption-v1
```

Expected result:
- the script says it wrote a spec manifest
- file created at:
  - `Tools/ArtPipeline/data/specs/commanders-redemption-v1.json`

If this fails because the file already exists:
- use a new batch ID

## Phase 2 — create the prompt manifest

Run:

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1
```

Expected result:
- the script says it wrote a prompt manifest
- file created at:
  - `Tools/ArtPipeline/data/prompts/commanders-redemption-v1.json`

Optional check before writing, if you want to inspect prompt content first:

```bash
python3 Tools/ArtPipeline/prompt.py commanders-redemption-v1 --dry-run
```

## Phase 3 — generate candidate images

Run:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1
```

Expected result:
- backend health check reports `ok`
- generation starts for each spec
- candidate PNG files are written under:
  - `Tools/ArtPipeline/data/candidates/commanders-redemption-v1/`
- candidate manifest created at:
  - `Tools/ArtPipeline/data/candidates/commanders-redemption-v1.json`

If you want to preview the request plan without creating images:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --dry-run
```

If you only want to generate one character's spec:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --spec captain-stretch-redemption-overlay
```

## Phase 4 — review and publish

Run:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1
```

Then open this in your browser:

```text
http://127.0.0.1:8766
```

If you changed the review port in `.env` or with `--port`, use that value instead.

## What to do in the review UI

For each spec:
1. open the spec from the dashboard
2. click a candidate image to select it
3. decide whether to:
   - approve the selected candidate
   - reject a candidate
   - mark the whole spec for regeneration
4. once a spec has an approved candidate, click **Publish**

## What publishing does

Publishing copies the approved image into the real asset folder and writes a prompt record file.

Expected publish outputs per approved spec:
- `Creative/Art/Assets/<final-file>.png`
- `Creative/Art/Projects/commanders-redemption-v1-<spec_id>.md`

## CLI shortcut: approve and publish one known candidate

If you already know the exact candidate filename you want:

```bash
python3 Tools/ArtPipeline/review.py commanders-redemption-v1 --approve captain-stretch-redemption-overlay_001.png
```

That shortcut:
- finds the spec
- approves the candidate
- publishes it
- exits

## If you need regeneration

The current implementation does **not** auto-regenerate from the UI.

If you mark a spec for regeneration, the UI stores the command you should run manually.

Typical pattern:

```bash
python3 Tools/ArtPipeline/generate.py commanders-redemption-v1 --spec captain-stretch-redemption-overlay
```

Then refresh the review page.

## Safe doc/testing sequence

If your goal is only to validate the docs and pipeline flow without generating images, this sequence matches the current implementation:

```bash
python3 Tools/ArtPipeline/specify.py --asset-type redemption-overlay --characters captain-stretch the-director water-wizard --batch-id docs-test
python3 Tools/ArtPipeline/prompt.py docs-test --dry-run
python3 Tools/ArtPipeline/prompt.py docs-test
python3 Tools/ArtPipeline/generate.py docs-test --dry-run
```

Why this is the right sequence:
- `specify.py --dry-run` does not write a spec file
- `prompt.py --dry-run` still needs a real spec file
- `generate.py --dry-run` still needs a real prompt file

## Cleanup / rerun guidance

There is no overwrite mode yet.

If you reuse a batch ID, you may hit errors because these already exist:
- spec manifest
- prompt manifest
- candidate manifest
- candidate output folders
- published asset filename

The safest rerun plan is simple:
- use a brand new batch ID every time

## Success checklist

A successful full run means:
- spec manifest written
- prompt manifest written
- candidate manifest written
- candidate images created
- review UI opens
- at least one candidate approved
- at least one final asset published into `Creative/Art/Assets/`
- prompt record written into `Creative/Art/Projects/`

# LotAT Story Viewer

A small local FastAPI tool for reviewing Legends of the ASCII Temple stories as a branching tree while they move through the local pipeline.

## Install dependencies

```bash
pip install fastapi uvicorn
```

## Run

Simplest option from the repo root:

```bash
./run-lotat-story-viewer.sh
```

This launcher script changes into the repo root automatically and starts Uvicorn with the correct import base.

Manual equivalent:

```bash
python3 -m uvicorn --app-dir . Tools.LotAT.story_viewer:app --reload
```

Why this form is safest:
- `Tools.LotAT.story_viewer` is a Python module path, not a file path.
- `--app-dir .` makes the repo root importable even if your shell environment is inconsistent.
- `python3 -m uvicorn` ensures you use the same Python environment where you installed FastAPI/Uvicorn.

## Story stage locations

The viewer reads stories from these folders:

```text
Creative/WorldBuilding/Storylines/drafts/
Creative/WorldBuilding/Storylines/ready/
Creative/WorldBuilding/Storylines/finished/
```

It also writes a canonical runtime copy to:

```text
Creative/WorldBuilding/Storylines/loaded/current-story.json
```

It renders the selected story tree starting at `starting_node_id`.

## Stage actions

The viewer now supports forward progression, one-step reversal, and loading a ready story into a canonical runtime file for future Streamer.bot consumers.

### Draft → Ready
The **HAND OFF STORY** button moves the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/drafts/
```

to:

```text
Creative/WorldBuilding/Storylines/ready/
```

If the story is already in `ready/`, the handoff endpoint returns success without failing.

### Ready → Draft
The **MOVE TO DRAFTS** button moves the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/ready/
```

to:

```text
Creative/WorldBuilding/Storylines/drafts/
```

If that ready story was the currently loaded runtime story, the viewer clears the runtime copy so Streamer.bot does not keep consuming a stale file.

### Ready → Loaded
The **LOAD** button copies the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/ready/<story-name>.json
```

into the canonical runtime path:

```text
Creative/WorldBuilding/Storylines/loaded/current-story.json
```

This is intentionally a copy, not a move. The original story remains in `ready/`.
Future Streamer.bot scripts should read `loaded/current-story.json` so they do not need to know which ready filename was chosen.

### Ready → Finished
The **FINISHED** button moves the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/ready/
```

to:

```text
Creative/WorldBuilding/Storylines/finished/
```

If that ready story was the currently loaded runtime story, the viewer clears the runtime copy so Streamer.bot does not keep consuming a stale file.

### Finished → Ready
The **MOVE TO READY** button moves the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/finished/
```

to:

```text
Creative/WorldBuilding/Storylines/ready/
```

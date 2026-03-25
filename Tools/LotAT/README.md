# LotAT Story Viewer

A small local FastAPI tool for reviewing Legends of the ASCII Temple story drafts as a branching tree before handing them off to the next stage of the pipeline.

## Install dependencies

```bash
pip install fastapi uvicorn
```

## Run

From the repo root:

```bash
uvicorn Tools/LotAT/story_viewer:app --reload
```

## Draft story location

Put draft story JSON files in:

```text
Creative/WorldBuilding/Storylines/drafts/
```

The viewer reads draft stories from that folder and renders the tree starting at `starting_node_id`.

## Handoff button

The **HAND OFF STORY** button moves the selected `.json` file from:

```text
Creative/WorldBuilding/Storylines/drafts/
```

to:

```text
Creative/WorldBuilding/Storylines/ready/
```

If the story is already in `ready/`, the handoff endpoint returns success without failing.

# LotAT Story Viewer Setup Guide

This guide explains how to set up and run the **LotAT Story Viewer** on a **Windows 11 machine using WSL**.

After following these steps, you will be able to:
- start the local story viewer web app
- open it in your Windows browser
- review draft and ready LotAT stories
- hand off a reviewed story from `drafts/` to `ready/`
- move a completed ready story from `ready/` to `finished/`

---

## What this tool is

The LotAT Story Viewer is a small local web app for reviewing story JSON files before they are handed off to the next stage of the pipeline.

It reads draft stories from:

```text
Creative/WorldBuilding/Storylines/drafts/
```

When you click **HAND OFF STORY** in the viewer, it moves the selected draft story file to:

```text
Creative/WorldBuilding/Storylines/ready/
```

When you click **FINISHED** on a ready story, it moves that file to:

```text
Creative/WorldBuilding/Storylines/finished/
```

That handoff matters because the technical pipeline is expected to consume stories from `ready/`, not directly from `drafts/`.

---

## What you need before you start

You should have:
- Windows 11
- WSL installed
- this repo cloned inside WSL or accessible from WSL
- Python 3 installed in WSL
- terminal access inside the repo root

---

## 1. Open WSL and go to the repo root

Open your WSL terminal and change into the project folder.

Example:

```bash
cd /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot
```

Make sure you are at the repo root by checking that you can see folders like:
- `Actions`
- `Creative`
- `Tools`
- `humans`

If you want, you can verify with:

```bash
pwd
ls
```

---

## 2. Confirm Python is available in WSL

Run:

```bash
python3 --version
```

If that does not work, try:

```bash
python --version
```

If neither command works, install Python in WSL first.

For Ubuntu-based WSL:

```bash
sudo apt update
sudo apt install python3 python3-pip python3-venv
```

---

## 3. Create a virtual environment (recommended)

This is optional, but recommended so the viewer dependencies stay isolated.

From the repo root, run:

```bash
python3 -m venv .venv
source .venv/bin/activate
```

After activation, your shell usually shows something like:

```text
(.venv)
```

If you do not want to use a virtual environment, you can skip this step.

---

## 4. Install the viewer dependencies

Install FastAPI and Uvicorn:

```bash
pip install fastapi uvicorn
```

If `pip` does not work, try:

```bash
pip3 install fastapi uvicorn
```

---

## 5. Start the story viewer

From the repo root, run:

```bash
./run-lotat-story-viewer.sh
```

This is the recommended launch command for operators.

The launcher script safely:
- changes into the repo root first
- uses the correct Python module path for Uvicorn
- avoids the working-directory import mistake that caused `ModuleNotFoundError: No module named 'Tools'`

Manual equivalent:

```bash
python3 -m uvicorn --app-dir . Tools.LotAT.story_viewer:app --reload
```

If it starts correctly, you should see output similar to:

```text
Uvicorn running on http://127.0.0.1:8000
```

Leave this terminal window open while you use the viewer.

---

## 6. Open the viewer in your Windows browser

On Windows 11, open your normal browser and go to:

```text
http://localhost:8000
```

Because you are using WSL, Windows can usually access the web server running inside WSL through `localhost`.

If `localhost:8000` does not work, try:

```text
http://127.0.0.1:8000
```

---

## 7. Add or confirm draft stories exist

The viewer shows stories from:

```text
Creative/WorldBuilding/Storylines/drafts/
```

There is currently a test draft story in this repo:

```text
Creative/WorldBuilding/Storylines/drafts/test-prototype-soda-comet.json
```

If the viewer is working, you should see that file listed in the Draft Stories section of the sidebar.

If no stories appear:
- confirm the file exists in `Creative/WorldBuilding/Storylines/drafts/`
- confirm it has a `.json` extension
- confirm the JSON is valid
- refresh the browser page

---

## 8. Review a story

Inside the viewer:

1. Click a story filename in the left sidebar.
2. Review the story overview.
3. Review the branching nodes and choices.
4. Confirm the story looks correct before handoff.

For the current test story, select:

```text
test-prototype-soda-comet.json
```

---

## 9. Hand off a reviewed story

When you are satisfied with a story, click:

```text
HAND OFF STORY
```

That moves the file from:

```text
Creative/WorldBuilding/Storylines/drafts/
```

to:

```text
Creative/WorldBuilding/Storylines/ready/
```

This marks it as ready for downstream technical/runtime work.

Important:
- after handoff, downstream agents should treat `ready/` as the source of truth
- do not keep editing the draft after handoff unless you intentionally move back into a draft workflow

---

## Optional: run the viewer directly with Python

There is also a direct script entry point:

```bash
python Tools/LotAT/story_viewer.py --reload
```

If you launch it this way, the script's default port is:

```text
8876
```

So you would open:

```text
http://127.0.0.1:8876
```

However, the repo's standard operator workflow is still:

```bash
./run-lotat-story-viewer.sh
```

and then:

```text
http://localhost:8000
```

---

## Troubleshooting

### Problem: `uvicorn: command not found`

Try:

```bash
python3 -m pip install fastapi uvicorn
./run-lotat-story-viewer.sh
```

### Problem: `pip: command not found`

Install pip in WSL:

```bash
sudo apt update
sudo apt install python3-pip
```

### Problem: browser says the page cannot be reached

Check:
- the terminal is still running the viewer
- you launched it from the repo root
- you are opening `http://localhost:8000`
- if needed, try `http://127.0.0.1:8000`

### Problem: no stories appear in the sidebar

Check that story files are in:

```text
Creative/WorldBuilding/Storylines/drafts/
```

The viewer only lists `.json` files from that folder.

### Problem: the story file exists but will not load

Possible causes:
- invalid JSON
- malformed schema
- incomplete file

If needed, validate the JSON file separately or open it in an editor to inspect for syntax errors.

---

## Quick start summary

If you already have Python working in WSL, the shortest path is:

```bash
cd /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot
python3 -m venv .venv
source .venv/bin/activate
pip install fastapi uvicorn
./run-lotat-story-viewer.sh
```

Then open in Windows:

```text
http://localhost:8000
```

Then review:

```text
Creative/WorldBuilding/Storylines/drafts/test-prototype-soda-comet.json
```

---

## Expected result

If everything is working, an operator starting from scratch should be able to:
- open the viewer in a browser
- see available draft stories
- select and review a story
- hand it off into `Creative/WorldBuilding/Storylines/ready/`

That means the LotAT story review tooling is set up correctly.

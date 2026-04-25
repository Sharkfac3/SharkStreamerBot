# Content Pipeline Setup and Operator Guide

This guide explains how to set up and run the **Content Pipeline** on a **Windows 11 machine using WSL**.

After following these steps, you will be able to:
- validate the local environment
- run the full pipeline for one recording
- review clips in your Windows browser
- sync review results back into the feedback loop

---

## What this tool is

The Content Pipeline turns a stream recording into review-ready short-form clips.

The normal operator flow is:

1. transcribe the recording
2. detect highlights with Ollama
3. extract clips with FFmpeg
4. format clips for review
5. review clips in the browser
6. sync approved results back into the feedback loop

For operators, the repo root includes these launcher scripts:

- `./run-content-pipeline-check-setup.sh`
- `./run-content-pipeline.sh <recording_name>`
- `./run-content-pipeline-review-server.sh`
- `./run-content-pipeline-feedback.sh <command>`

These launchers:
- resolve paths from the repo root
- work from anywhere you call them
- prefer the repo's `.venv` Python if it exists
- are intended for Windows 11 + WSL operator use

---

## What you need before you start

You should have:
- Windows 11
- WSL installed
- this repo cloned inside WSL or accessible from WSL
- Python 3.10+ installed in WSL
- an NVIDIA GPU available to WSL
- FFmpeg and ffprobe installed in WSL or configured in `Tools/ContentPipeline/.env`
- Ollama installed on native Windows 11 and able to run locally there
- WSL able to reach the Windows Ollama API at `http://localhost:11434`
- terminal access inside the repo root

Important:
- the pipeline requires an NVIDIA GPU for Phase 3 clip encoding through `h264_nvenc`
- there is no CPU fallback for clip extraction
- do not continue until `nvidia-smi` works in WSL

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

## 3. Install FFmpeg and ffprobe

The pipeline needs both `ffmpeg` and `ffprobe`.

If they are already installed in WSL, confirm with:

```bash
ffmpeg -version
ffprobe -version
```

If not, install them in WSL first.

For Ubuntu-based WSL:

```bash
sudo apt update
sudo apt install ffmpeg
```

If you do not want to use PATH-based detection, you can set these in `Tools/ContentPipeline/.env`:

- `CONTENT_PIPELINE_FFMPEG_PATH`
- `CONTENT_PIPELINE_FFPROBE_PATH`

---

## 4. Confirm NVIDIA GPU access in WSL

Run:

```bash
nvidia-smi
```

If this fails, stop here.

The pipeline uses FFmpeg's `h264_nvenc` encoder during clip extraction. That requires an NVIDIA GPU that is visible inside WSL.

There is no CPU fallback for this pipeline.

---

## 5. Install and prepare Ollama on Windows

Install Ollama on native Windows 11 if it is not already installed.

Use the Windows setup guide in:

```text
humans/setup-info/ollama-windows-11-setup.md
```

Then, from Windows, pull the required model:

```powershell
ollama pull llama3.1:8b
```

Before you run Phase 2 or the full pipeline from WSL, Ollama must be running on Windows.

If needed, start it from Windows PowerShell:

```powershell
ollama serve
```

Then verify from WSL that the Windows-hosted API is reachable:

```bash
curl http://localhost:11434/api/tags
```

---

## 6. Create a virtual environment and install dependencies

From the repo root, run:

```bash
python3 -m venv .venv
source .venv/bin/activate
pip install -r Tools/ContentPipeline/requirements.txt
```

Important note:
- `faster-whisper` downloads the `large-v3` model the first time you run transcription
- that first model download is about 3 GB
- this is expected

You can leave the virtual environment activated while you work.

The root launcher scripts also prefer `.venv` automatically if it exists.

---

## 7. Configure the recordings folder

Copy the example env file:

```bash
cp Tools/ContentPipeline/.env.example Tools/ContentPipeline/.env
```

Then edit `Tools/ContentPipeline/.env` and set:

```text
CONTENT_PIPELINE_RECORDINGS_DIR=
```

This must point to the folder that contains your source stream recordings.

Example Windows-style path from the default config:

```text
C:\Users\sharkfac3\Workspace\streamStuff\recordings
```

---

## 8. Validate the environment

Run:

```bash
./run-content-pipeline-check-setup.sh
```

This checks:
- Python version
- Python packages
- FFmpeg
- ffprobe
- NVIDIA GPU access
- WSL connectivity to the Windows-hosted Ollama API
- Ollama model availability
- recordings directory access
- data directory readiness

Do not continue until the setup check passes.

---

## 9. Run the pipeline for one recording

Run:

```bash
./run-content-pipeline.sh <recording_name>
```

Example:

```bash
./run-content-pipeline.sh "2026-02-23 21-11-57.mp4"
```

You can also use:
- the full file name
- a recording stem without `.mp4`
- an absolute path to the recording

The orchestrator runs:
1. transcription
2. highlight detection
3. clip extraction
4. Instagram formatting
5. review UI launch

If it succeeds, it launches the review UI at:

```text
http://localhost:8000
```

Leave that terminal open while you review clips.

---

## 10. Review clips in your Windows browser

Open your normal Windows browser and go to:

```text
http://localhost:8000
```

Because you are using WSL, Windows can usually reach the review server through `localhost`.

If needed, try:

```text
http://127.0.0.1:8000
```

Current review controls:
- `A` approves the current clip
- `S` skips the current clip
- `T` sends the current clip to trash
- `E` focuses the caption field
- `Ctrl+Enter` saves caption and hashtag edits

Approved items move from:

```text
Tools/ContentPipeline/data/review_queue/
```

to:

```text
Tools/ContentPipeline/data/published/
```

---

## 11. Sync the feedback loop

After you finish reviewing clips, run:

```bash
./run-content-pipeline-feedback.sh sync
```

This indexes published metadata and rebuilds the feedback outputs used by Phase 2.

This step is optional, but it improves highlight detection over time.

---

## Optional: reopen the review UI later

If the pipeline already generated review-ready clips and you just want to review them again, run:

```bash
./run-content-pipeline-review-server.sh
```

Then open:

```text
http://localhost:8000
```

---

## Optional: other feedback commands

The feedback launcher forwards commands directly to `feedback.py`.

Examples:

```bash
./run-content-pipeline-feedback.sh report
./run-content-pipeline-feedback.sh report --json
./run-content-pipeline-feedback.sh template
./run-content-pipeline-feedback.sh import-csv Tools/ContentPipeline/data/feedback/metrics_template.csv
```

---

## Troubleshooting

### Problem: the setup check fails on GPU or NVENC

Check:
- `nvidia-smi` works in WSL
- your FFmpeg build includes `h264_nvenc`
- your NVIDIA drivers and WSL GPU support are working

Run the setup check again:

```bash
./run-content-pipeline-check-setup.sh
```

### Problem: Ollama is not reachable

Check:
- Ollama is installed on Windows
- `ollama serve` is running on Windows, or the Ollama background app is active
- the model is pulled locally on Windows
- WSL can reach `http://localhost:11434/api/tags`
- `CONTENT_PIPELINE_OLLAMA_URL` is correct in `Tools/ContentPipeline/.env`

### Problem: the review UI does not open

Check:
- the pipeline terminal is still running
- you are opening `http://localhost:8000`
- if needed, try `http://127.0.0.1:8000`
- if you only need the UI, start it again with `./run-content-pipeline-review-server.sh`

### Problem: no clips show up in the review UI

Check:
- Phase 4 completed successfully
- files exist in `Tools/ContentPipeline/data/review_queue/`
- you are reviewing the same repo and environment that generated the files

### Problem: the recordings folder is not found

Check:
- `Tools/ContentPipeline/.env` exists
- `CONTENT_PIPELINE_RECORDINGS_DIR` points to the correct folder
- the path is readable from WSL

---

## Quick start summary

If you already have WSL, Python, FFmpeg, NVIDIA GPU access, and Windows-hosted Ollama working, the shortest path is:

First, on Windows if needed:

```powershell
ollama serve
ollama pull llama3.1:8b
```

Then in WSL:

```bash
cd /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot
curl http://localhost:11434/api/tags
python3 -m venv .venv
source .venv/bin/activate
pip install -r Tools/ContentPipeline/requirements.txt
cp Tools/ContentPipeline/.env.example Tools/ContentPipeline/.env
./run-content-pipeline-check-setup.sh
./run-content-pipeline.sh "2026-02-23 21-11-57.mp4"
```

Then open in Windows:

```text
http://localhost:8000
```

After review, run:

```bash
./run-content-pipeline-feedback.sh sync
```

---

## Expected result

If everything is working, an operator starting from scratch should be able to:
- validate the environment
- run the full pipeline for one recording
- review clips in a browser
- approve at least one clip into `data/published/`
- sync published results back into the feedback loop

That means the Content Pipeline operator workflow is set up correctly.

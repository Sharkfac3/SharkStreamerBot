# Pipeline Dev Notes

## 2026-03-24 — Phase 1 bootstrap

- `Tools/ContentPipeline/config.py` translates the default Windows recordings path into `/mnt/c/...` automatically when run from WSL/Linux.
- `transcribe.py` intentionally skips `-vertical.mp4` files during batch discovery so the same stream audio is not transcribed twice.
- In the current repo shell, `python3` is available but `faster-whisper` is not installed yet, so Phase 1 was validated with syntax checks and real-file dry runs instead of a full transcript generation run.

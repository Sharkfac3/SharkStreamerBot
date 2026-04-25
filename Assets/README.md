# Assets/

Binary media assets for stream features. Not tracked in git (see §Gitignore).

---

## Purpose

Stores audio (`.mp3`, `.wav`) and image (`.gif`, `.png`, `.jpg`) files used at runtime by stream features. The `info-service` data store holds **filenames only** — no paths. Consumers (Streamer.bot scripts, MixItUp commands) join the filename with `ASSETS_ROOT` at runtime to construct the full absolute path.

---

## Folder Convention

```
Assets/
└── <collection>/       ← one folder per feature/collection (e.g., user-intros/)
    ├── sound/          ← audio assets for this collection
    └── gif/            ← image/gif assets for this collection
```

- One subfolder per collection (e.g., `user-intros/`).
- Subfolders by asset type (`sound/`, `gif/`).
- Filenames match the `soundFile` / `gifFile` fields stored in the corresponding `info-service` collection record.

---

## Why Gitignored

Binary media files are machine-specific, large, and operator-placed. They are not tracked in the repo. The operator places files here manually after cloning or pulling. Only `.gitkeep` placeholder files and this `README.md` are tracked.

The `.gitignore` block that covers this folder:

```
Assets/**/*.mp3
Assets/**/*.wav
Assets/**/*.gif
Assets/**/*.png
Assets/**/*.jpg
Assets/**/*.jpeg
Assets/**/*.webm
```

---

## MixItUp Path Expectation

MixItUp constructs asset paths at runtime using the `ASSETS_ROOT` constant:

- Sound: `ASSETS_ROOT + "/user-intros/sound/" + soundFile`
- Gif:   `ASSETS_ROOT + "/user-intros/gif/"   + gifFile`

`ASSETS_ROOT` is an absolute path set per operator machine in `Actions/SHARED-CONSTANTS.md`. **No trailing slash.**

Example: `C:\Users\operator\Workspace\coding\SharkStreamerBot\Assets`

---

## How to Add New Asset Types for a Collection

1. Add a subfolder under `Assets/<collection>/` (e.g., `Assets/user-intros/video/`).
2. Add a `.gitkeep` file inside it so git tracks the directory.
3. Add a matching `.gitignore` pattern for the new file extensions (e.g., `Assets/**/*.mp4`).
4. Register a subpath constant in `Actions/SHARED-CONSTANTS.md` under the `## Info Service / Assets (shared)` section (e.g., `ASSETS_USER_INTROS_VIDEO_SUBPATH = "user-intros/video/"`).

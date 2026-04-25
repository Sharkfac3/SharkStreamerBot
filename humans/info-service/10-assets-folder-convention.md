# Prompt C1 — Assets Folder Convention + SHARED-CONSTANTS

Paste everything below this line into a fresh coding-agent chat window.

---

You are working in the `SharkStreamerBot` repo. Fresh session — no memory of prior conversations.

## Read first

1. `CLAUDE.md`
2. `.agents/ENTRY.md`
3. `.agents/roles/app-dev/role.md`
4. `humans/info-service/COORDINATION.md`
5. `Docs/INFO-SERVICE-PLAN.md` — especially §Constants to Register and §Folder Layout
6. `Actions/SHARED-CONSTANTS.md` — formatting reference for the new constants section

Role: `app-dev`.

## Prereqs

Chunks merged: P3. Verify with `git log` before starting — `humans/info-service/10-assets-folder-convention.md` must exist.

## WORKING.md

Add row at start. Domain: `Assets/`, `Actions/`. Files: `Assets/user-intros/sound/.gitkeep`, `Assets/user-intros/gif/.gitkeep`, `Assets/README.md`, `.gitignore`, `Actions/SHARED-CONSTANTS.md`. Remove + log at finish.

## Task

No code. Documentation and folder scaffolding only. Five deliverables:

### 1. Create `Assets/` folder structure at repo root

Create two empty `.gitkeep` placeholder files:
- `Assets/user-intros/sound/.gitkeep`
- `Assets/user-intros/gif/.gitkeep`

Both files must be empty (zero bytes). Their purpose is to keep the directory tree in git without tracking binary content.

### 2. Create `Assets/README.md`

Document:
- **Purpose**: binary media assets (audio, gif) for stream features; info-service records store filenames only; consumers join filename with `ASSETS_ROOT` at runtime to get full paths
- **Folder convention**: `Assets/<collection>/<type>/` — one subfolder per collection (e.g., `user-intros/`), subfolders by asset type (`sound/`, `gif/`)
- **Why gitignored**: machine-specific binaries; operator places files here manually; not tracked in repo
- **MixItUp path expectation**: constructed as `ASSETS_ROOT + "/user-intros/sound/" + soundFile` (or `.../gif/`); `ASSETS_ROOT` is an absolute path set per operator machine in `Actions/SHARED-CONSTANTS.md`
- **How to add new asset types for a collection**: add a subfolder under `Assets/<collection>/`, add a matching `.gitignore` pattern for its extensions, and register the subpath constant in `Actions/SHARED-CONSTANTS.md`

### 3. Add to `.gitignore`

Append a block that ignores binary media extensions under `Assets/` while **preserving** `.gitkeep` and `README.md` files. Do NOT add a blanket `Assets/` ignore.

```
# Assets/ — binary media files; machine-specific; operator places manually
Assets/**/*.mp3
Assets/**/*.wav
Assets/**/*.gif
Assets/**/*.png
Assets/**/*.jpg
Assets/**/*.jpeg
Assets/**/*.webm
```

Verify after adding: `git status` should show `Assets/user-intros/sound/.gitkeep` and `Assets/user-intros/gif/.gitkeep` as untracked (tracked), not ignored.

### 4. Add `## Info Service / Assets (shared)` section to `Actions/SHARED-CONSTANTS.md`

Match the formatting style of existing sections (section header, bullet list of `constant = value` pairs, operator notes, "Used in:" list).

Constants to register (values from `Docs/INFO-SERVICE-PLAN.md` §Constants to Register and §Tech Stack):

| Constant | Value | Notes |
|---|---|---|
| `ASSETS_ROOT` | *(operator must set — absolute path to repo-root `Assets/` folder on this machine)* | **TBD** — set before running first intro. Example: `C:\Users\operator\Workspace\coding\SharkStreamerBot\Assets` |
| `INFO_SERVICE_URL` | `http://127.0.0.1:8766` | Base URL for all info-service HTTP requests from Streamer.bot |
| `INFO_SERVICE_PORT` | `8766` | info-service listen port |
| `PRODUCTION_MANAGER_PORT` | `5174` | production-manager dev server port |
| `ASSETS_USER_INTROS_SOUND_SUBPATH` | `user-intros/sound/` | Relative path from `ASSETS_ROOT` to user-intro sound files |
| `ASSETS_USER_INTROS_GIF_SUBPATH` | `user-intros/gif/` | Relative path from `ASSETS_ROOT` to user-intro gif files |

Operator note to include:
> `ASSETS_ROOT` is machine-specific. Set it to the absolute path of the `Assets/` folder on this machine before running any intro lookup scripts. Full path example: `C:\Users\<you>\Workspace\coding\SharkStreamerBot\Assets`. No trailing slash.

"Used in:" list: `(none yet — C8 will add first-chat-intro.cs which references INFO_SERVICE_URL and ASSETS_ROOT)`

## Deliverables

- Files changed:
  - `Assets/user-intros/sound/.gitkeep` (new, empty)
  - `Assets/user-intros/gif/.gitkeep` (new, empty)
  - `Assets/README.md` (new)
  - `.gitignore` (new block appended)
  - `Actions/SHARED-CONSTANTS.md` (new section appended)
- Scaffolding updates: none in this chunk
- Shared constants: see Task §4 above — six constants under new `## Info Service / Assets (shared)` section
- Tests: N/A

## Forbidden in this chunk

- Creating `Apps/info-service/` or `Apps/production-manager/` — those are C2 and C6
- Writing any TypeScript or C# code
- Modifying any existing Streamer.bot scripts
- Adding `Assets/` as a blanket gitignore entry (would hide `.gitkeep` and `README.md`)

## Finish

1. No build/typecheck commands needed (no code changed).
2. Update `humans/info-service/COORDINATION.md` — chunk C1 status → `merged`, append Run Log row (Commit column: `uncommitted`).
3. `WORKING.md` — remove Active Work row, add Recently Completed row (trim to 10).
4. Load `ops-change-summary` skill, show output.

**Do NOT run `git commit` or `git add`. Operator commits manually.**

## Definition of done

- `Assets/user-intros/sound/.gitkeep` exists (empty file, tracked by git)
- `Assets/user-intros/gif/.gitkeep` exists (empty file, tracked by git)
- `Assets/README.md` covers purpose, folder convention, gitignore rule, MixItUp path expectation, and how to extend
- `.gitignore` ignores binary extensions under `Assets/` but does NOT ignore `.gitkeep` or `README.md`
- `Actions/SHARED-CONSTANTS.md` has `## Info Service / Assets (shared)` section with all six constants; `ASSETS_ROOT` clearly marked TBD with example
- `COORDINATION.md` C1 row: status = `merged`, Prompt File = `10-assets-folder-convention.md`
- No git commit made by agent.

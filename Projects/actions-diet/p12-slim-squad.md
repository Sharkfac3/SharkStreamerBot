Edit `Actions/Squad/AGENTS.md` (~236 lines, target ~90 lines). No ACTION-CONTRACTS block.

Squad hosts 4 mini-games as subfolders (Clone, Duck, Pedro, Toothless), plus 2 shared scripts (`offering.cs`, `squad-game-help.cs`). Each subfolder has its own `README.md` documenting its specific game mechanics — do NOT edit those files.

READ `Actions/Squad/AGENTS.md` in full before editing.

REMOVE or REPLACE:
1. Ownership section → 1–2 lines + pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. Keep Squad-specific refs alongside it: `Actions/Helpers/mini-game-lock.md` and `Actions/Helpers/mini-game-contract.md` are directly relevant and should remain explicit.
3. Local Workflow: remove universal script rules. Keep only Squad-specific rules.
4. Large Script Reference tables → replace with compact summary (see below).
5. Validation / Boundaries / Handoff → pointer to `Actions/AGENTS.md`.

CONTENT TO KEEP (this is load-bearing for Squad work):
- Mini-game lock usage: Squad games must acquire/release the global mini-game lock. Reference `Actions/Helpers/mini-game-lock.md`.
- The call/main/resolve/overlay-publish script pattern that each mini-game subfolder follows.
- `offering.cs` manages shared Squad state; `squad-game-help.cs` provides shared helpers — note these are shared across all 4 games.
- Folder routing table pointing to each subfolder's README.md for game-specific mechanics.

COMPACT SUMMARY to replace large tables:

Subfolder routing:
| Subfolder | Game | README |
|---|---|---|
| Clone/ | Clone Empire (grid survival) | Clone/README.md |
| Duck/ | Duck Call (quack race) | Duck/README.md |
| Pedro/ | Pedro event | Pedro/README.md |
| Toothless/ | Toothless event | Toothless/README.md |

Shared scripts:
| Script | Purpose |
|---|---|
| `offering.cs` | Shared Squad state management |
| `squad-game-help.cs` | Shared helpers for all Squad games |

After editing, confirm: reading this file tells an agent (a) what Squad is, (b) how to find game-specific docs, (c) the mini-game lock requirement, and (d) which scripts are shared.

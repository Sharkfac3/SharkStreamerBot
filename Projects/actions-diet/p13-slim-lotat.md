Edit `Actions/LotAT/AGENTS.md` (~214 lines, target ~100 lines). No ACTION-CONTRACTS block.

LotAT has 13 scripts implementing a multi-phase turn-based game. It has separate reference docs in the same folder (`implementation-map.md`, `operator-setup.md`, `runtime-contract.md`) — do NOT edit those files. The AGENTS.md should point to them, not duplicate them.

READ `Actions/LotAT/AGENTS.md` in full before editing.

REMOVE or REPLACE:
1. Ownership section → 1–2 lines + pointer to `Actions/AGENTS.md`.
2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required. Keep LotAT-specific refs alongside it: `implementation-map.md`, `operator-setup.md`, `runtime-contract.md` (same folder, primary LotAT references).
3. Local Workflow: remove universal script rules. Keep only LotAT-specific rules.
4. Large Script Reference tables → replace with compact phase-map table (see below).
5. Validation / Boundaries / Handoff → pointer to `Actions/AGENTS.md`.

CONTENT TO KEEP (this is load-bearing for LotAT work):
- The phase sequence — this is the most important non-obvious fact:
  join → dice roll → decision → node entry → (timeout handlers at each phase) → end session
- `overlay-publish.cs` serves a specific role (sends LotAT state to the overlay) — keep this noted.
- `lotat-commander-input.cs` contains the hand-rolled JSON parser reference implementation (see `Actions/Helpers/json-no-external-libraries.md`) — worth noting as the canonical example.
- Pointer to `implementation-map.md` for the full script dependency map.
- Pointer to `runtime-contract.md` for state variable definitions.

COMPACT PHASE-MAP TABLE to replace large script tables:

| Phase | Script | Notes |
|---|---|---|
| Start | `lotat-start-main.cs` | Game entry point |
| Join | `lotat-join.cs` | Player registration |
| Join timeout | `lotat-join-timeout.cs` | Closes join window |
| Dice roll | `lotat-dice-roll.cs` | Rolls for active players |
| Dice timeout | `lotat-dice-timeout.cs` | Resolves on timer |
| Decision input | `lotat-decision-input.cs` | Collects player decisions |
| Decision resolve | `lotat-decision-resolve.cs` | Applies decisions |
| Decision timeout | `lotat-decision-timeout.cs` | Resolves on timer |
| Commander input | `lotat-commander-input.cs` | Commander action input (JSON parser) |
| Commander timeout | `lotat-commander-timeout.cs` | Resolves on timer |
| Node enter | `lotat-node-enter.cs` | Applies node effects |
| End session | `lotat-end-session.cs` | Tears down game state |
| Overlay | `overlay-publish.cs` | Publishes state to overlay |

After editing, confirm: reading this file tells an agent (a) what LotAT is, (b) the phase sequence, (c) where to find full docs, and (d) which scripts handle which phase.

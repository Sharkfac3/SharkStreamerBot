# Prompt 03 — Clone Grid Game: Movement + Game Logic Scripts

## Context

You are working on SharkStreamerBot. Read `.agents/ENTRY.md` before starting. Activate the **streamerbot-dev** role (`roles/streamerbot-dev/`). Read `roles/streamerbot-dev/skills/core.md`.

Check `WORKING.md` before touching any files. Add your row to Active Work.

**Prerequisite:** Prompts 01 and 02 must be complete.

Read these files before writing any code:
- `Actions/HELPER-SNIPPETS.md` — mini-game lock, JSON helpers, CPH API signatures
- `Actions/SHARED-CONSTANTS.md` — canonical var/timer/topic names
- `Actions/Squad/Clone/clone-empire-main.cs` — see how lock and state vars are set
- `Actions/Squad/Clone/clone-empire-start.cs` — see how players/empire JSON is structured

---

## What You Are Building

Two Streamer.bot C# scripts for the **movement and game-logic phase**:

| File | Trigger | Purpose |
|------|---------|---------|
| `Actions/Squad/Clone/clone-empire-move.cs` | `!up` / `!down` / `!left` / `!right` | Move a player, apply all grid rules |
| `Actions/Squad/Clone/clone-empire-tick.cs` | Timer `Clone - Game Tick` (5s repeating) | Kill idle players, check win condition |

Do NOT edit existing files. New files only (except as noted in wiring section at the end).

---

## Full Rule Set (implement exactly)

### Grid
- 32 cols × 18 rows, 1-indexed. Col 1 = left, col 32 = right. Row 1 = top, row 18 = bottom.
- Boundary: if a move would take a player off the grid, it is silently ignored (player stays in place, no death, no cooldown consumed).

### Movement cooldown
- Each player has a 1-second per-move cooldown tracked via `lastMoveUtc` in their JSON entry.
- If `now - lastMoveUtc < 1000ms` → silently ignore this command (no chat message).

### Rule 1 — Death by empire
- If the player's **new** position overlaps any empire cell → player dies instantly.
- On death: remove player from player list. Do NOT add empire at the new position (player died entering empire territory, not leaving safe ground).
- Check if this was the last player → if yes, trigger loss end.

### Rule 2 — Empire growth on departure
- Checked against the player's **old** (pre-move) position, after the player has successfully moved (i.e., player is alive and at the new position).
- Condition: does oldPos have any orthogonal neighbor that is an empire cell **AND** does that empire cell itself have at least one OTHER orthogonal empire neighbor (not oldPos itself)?
- If condition is true → oldPos becomes a new empire cell.
- After adding the new empire cell, immediately run a Rule 3 check on that cell and its neighbors.

### Rule 3 — Empire death by encirclement
- After any empire cell is added (from Rule 2 or from inactivity kill in the tick script):
  - For each newly added empire cell C: count how many of its 4 orthogonal neighbors are "occupied."
  - A neighbor is "occupied" if it is: another empire cell, a player-occupied cell (any living player is there), OR a grid edge (out of bounds).
  - If all 4 neighbors are occupied → empire cell C is destroyed (removed from list).
  - After removing C, re-check all 4 of C's former neighbors (they may now trigger Rule 3 themselves). Repeat until stable. Limit iterations to 100 to prevent runaway loops.

> Rule 3 operates on newly added empire cells only. Do not re-scan the entire empire list on every move (too expensive).

### Inactivity death (tick script only)
- If `now - player.lastMoveUtc > 30000ms` → player dies from inactivity.
- On inactivity death: remove player from list, add empire cell at player's last position.
- Run Rule 3 on the new empire cell.
- Inactivity check only runs when `empire_game_start_utc > 0` (movement phase is open).

### Win condition (tick script only)
- If `now - empire_game_start_utc >= 300000ms` (5 minutes) AND `players.Count > 0` → win!
- Trigger win path: Mix It Up unlock + overlay end event + chat message + lock release.

### Loss condition
- All players are dead (player list empty at any point).
- Trigger loss path: overlay end event + chat message + lock release.

---

## Data Classes (copy into both scripts)

```csharp
[DataContract]
private class EmpirePlayer
{
    [DataMember(Name = "userId")]      public string UserId      { get; set; }
    [DataMember(Name = "userName")]    public string UserName    { get; set; }
    [DataMember(Name = "col")]         public int    Col         { get; set; }
    [DataMember(Name = "row")]         public int    Row         { get; set; }
    [DataMember(Name = "lastMoveUtc")] public long   LastMoveUtc { get; set; }
}

[DataContract]
private class EmpireCell
{
    [DataMember(Name = "col")] public int Col { get; set; }
    [DataMember(Name = "row")] public int Row { get; set; }
}
```

---

## Script 1: `clone-empire-move.cs`

**Trigger:** `!up`, `!down`, `!left`, `!right` chat commands (all four point to this same action)  
**Required args:** `user` (string), `userId` (string), `rawInput` (string) or read the command itself

> This script is triggered by the same commands as `destroyer-move.cs`. The mini-game lock check at step 1 ensures only one game processes the command at a time.

### How to determine direction

Streamer.bot passes the matched command text. Use `CPH.TryGetArg("rawInput", out string rawInput)` — will contain `"!up"`, `"!down"`, etc. Strip the `!` prefix and lowercase to get direction string.

Alternatively: create four separate Streamer.bot actions (one per direction), each passing a `direction` argument. Either approach is fine — document your choice clearly.

Recommended approach: **one action for all four commands** — read `rawInput`, strip `!`, lowercase, validate against `{"up","down","left","right"}`.

### Logic

```
1. Check minigame_name == "clone_empire". If not → return true (another game or no game).
2. Check empire_game_active == true AND empire_join_active == false (movement phase).
   If join phase still active → send chat "@{user} join phase is still open — movement starts soon!"
   If game not active → return true silently.
3. Parse rawInput → direction string. Validate in {up, down, left, right}. If invalid → return.
4. Parse empire_players_json → List<EmpirePlayer>.
5. Find player by userId. If not found → send "@{user} you're not in this game!" and return.
6. Check cooldown: now - player.LastMoveUtc < 1000ms → silently return (no message).
7. Compute new col/row from direction:
   up    → row - 1
   down  → row + 1
   left  → col - 1
   right → col + 1
8. Boundary check: if newCol < 1 || newCol > 32 || newRow < 1 || newRow > 18 → silently return.
9. Parse empire_cells_json → List<EmpireCell>.
10. RULE 1: Check if (newCol, newRow) is in empire list.
    If yes:
      - Remove player from players list.
      - Save players_json.
      - Send chat: "💀 {user} flew into Empire territory and was destroyed!"
      - Publish squad.clone.update (event: "player_died", eventDetail: userName)
      - If players.Count == 0 → call TriggerLoss(); return.
      - Else: save state, publish, return.
11. Player moves safely:
    - Store oldCol, oldRow.
    - Update player: col = newCol, row = newRow, lastMoveUtc = now.
    - Save players_json.
12. RULE 2: Check empire growth at (oldCol, oldRow):
    For each of the 4 orthogonal directions from oldPos:
      If that neighbor (nCol, nRow) is an empire cell:
        For each of 4 orthogonal directions from (nCol, nRow):
          If that second neighbor is ALSO an empire cell AND != (oldCol, oldRow):
            → oldPos becomes empire. Add EmpireCell{oldCol, oldRow} to empire list. Break all.
    (One pass — stop at first match found.)
13. If new empire cell was added at step 12:
    - Run Rule3Check(empire, players, newEmpireCols=[oldCol], newEmpireRows=[oldRow])
    - Save empire_cells_json.
    - Publish squad.clone.update (event: "empire_spawned", eventDetail: $"{oldCol},{oldRow}")
    else:
    - Publish squad.clone.update (event: "player_moved", eventDetail: userName)
14. If players.Count == 0 after all checks → call TriggerLoss().
```

### Rule3Check helper method

```csharp
// Checks newly added empire cells for encirclement. Modifies empireList in-place.
// Returns list of cells removed.
private List<EmpireCell> Rule3Check(
    List<EmpireCell> empire,
    List<EmpirePlayer> players,
    List<int> checkCols,
    List<int> checkRows)
{
    var removed = new List<EmpireCell>();
    var toCheck = new Queue<(int col, int row)>();
    for (int i = 0; i < checkCols.Count; i++)
        toCheck.Enqueue((checkCols[i], checkRows[i]));

    int iterations = 0;
    while (toCheck.Count > 0 && iterations < 100)
    {
        iterations++;
        var (col, row) = toCheck.Dequeue();

        // Verify cell is still in empire list (may have been removed this pass)
        if (!empire.Exists(e => e.Col == col && e.Row == row))
            continue;

        int[] dCol = { 0, 0, -1, 1 };
        int[] dRow = { -1, 1, 0, 0 };
        bool allOccupied = true;

        for (int d = 0; d < 4; d++)
        {
            int nc = col + dCol[d];
            int nr = row + dRow[d];

            bool isEdge   = nc < 1 || nc > 32 || nr < 1 || nr > 18;
            bool isEmpire = empire.Exists(e => e.Col == nc && e.Row == nr);
            bool isPlayer = players.Exists(p => p.Col == nc && p.Row == nr);

            if (!isEdge && !isEmpire && !isPlayer)
            {
                allOccupied = false;
                break;
            }
        }

        if (allOccupied)
        {
            empire.RemoveAll(e => e.Col == col && e.Row == row);
            removed.Add(new EmpireCell { Col = col, Row = row });

            // Enqueue neighbors to re-check (they may now be surrounded too)
            for (int d = 0; d < 4; d++)
            {
                int nc = col + dCol[d];
                int nr = row + dRow[d];
                if (nc >= 1 && nc <= 32 && nr >= 1 && nr <= 18)
                    if (empire.Exists(e => e.Col == nc && e.Row == nr))
                        toCheck.Enqueue((nc, nr));
            }
        }
    }
    return removed;
}
```

### TriggerLoss helper

```csharp
private void TriggerLoss()
{
    CPH.DisableTimer(TIMER_GAME_TICK);
    CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, false, false);
    CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, false, false);
    ReleaseMiniGameLockIfOwned();

    // Publish end event
    PublishCloneEnd("loss", new List<EmpirePlayer>());

    CPH.SendMessage("☠️ All Rebel fighters have been eliminated. The Empire controls this sector.");
}
```

### Broker publish — `squad.clone.update`

```json
{
  "id": "<uuid>",
  "topic": "squad.clone.update",
  "sender": "streamerbot",
  "timestamp": <unix-ms>,
  "payload": {
    "game": "clone",
    "state": {
      "event": "<event-string>",
      "players": [...],
      "empire": [...],
      "gridCols": 32,
      "gridRows": 18,
      "elapsedSeconds": <(now - empire_game_start_utc) / 1000>,
      "survivorCount": <count>,
      "empireCount": <count>,
      "eventDetail": "<optional>"
    }
  }
}
```

### Broker publish — `squad.clone.end`

```json
{
  "id": "<uuid>",
  "topic": "squad.clone.end",
  "sender": "streamerbot",
  "timestamp": <unix-ms>,
  "payload": {
    "game": "clone",
    "result": {
      "outcome": "win" | "loss",
      "survivors": [...players...]
    }
  }
}
```

### Constants

```csharp
private const string VAR_MINIGAME_ACTIVE     = "minigame_active";
private const string VAR_MINIGAME_NAME       = "minigame_name";
private const string MINIGAME_NAME           = "clone_empire";
private const string VAR_EMPIRE_GAME_ACTIVE  = "empire_game_active";
private const string VAR_EMPIRE_JOIN_ACTIVE  = "empire_join_active";
private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
private const string VAR_EMPIRE_CELLS_JSON   = "empire_cells_json";
private const string TIMER_GAME_TICK         = "Clone - Game Tick";
private const int    BROKER_WS_INDEX         = 0;
private const int    EMPIRE_GRID_COLS        = 32;
private const int    EMPIRE_GRID_ROWS        = 18;
private const long   EMPIRE_MOVE_COOLDOWN_MS = 1000L;
private const string MIXITUP_CLONE_UNLOCK_COMMAND_ID = "REPLACE_WITH_CLONE_UNLOCK_COMMAND_ID";
private const int    WAIT_MIXITUP_UNLOCK_STARTUP_MS  = 3000;
```

---

## Script 2: `clone-empire-tick.cs`

**Trigger:** Timer `"Clone - Game Tick"` (fires every 5 seconds while enabled)  
**Required args:** None

### Logic

```
1. Read empire_game_active. If false → disable timer "Clone - Game Tick" and return.
2. Read empire_game_start_utc. If 0 → game hasn't opened movement yet (join phase). Return.
3. long now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
4. Parse empire_players_json. Parse empire_cells_json.

5. WIN CHECK:
   If now - empire_game_start_utc >= 300000 AND players.Count > 0:
     → call TriggerWin(players, empire); return.

6. INACTIVITY CHECK:
   bool stateChanged = false;
   var newEmpireCols = new List<int>();
   var newEmpireRows = new List<int>();
   var killedPlayers = new List<EmpirePlayer>();

   For each player in players:
     if now - player.LastMoveUtc > 30000:
       killedPlayers.Add(player)
       empire.Add(new EmpireCell { Col = player.Col, Row = player.Row })
       newEmpireCols.Add(player.Col)
       newEmpireRows.Add(player.Row)
       stateChanged = true
       CPH.SendMessage($"⏰ {player.UserName} was consumed by the Empire (inactivity)!")

   // Remove killed players
   foreach (var dead in killedPlayers)
     players.RemoveAll(p => p.UserId == dead.UserId)

   // Run Rule 3 on all new empire cells
   if (newEmpireCols.Count > 0)
     Rule3Check(empire, players, newEmpireCols, newEmpireRows)

7. Save updated vars if stateChanged.

8. If players.Count == 0 → TriggerLoss(); return.

9. If stateChanged → publish squad.clone.update (event: "player_inactivity").

10. Optional: every 60-second milestone, send a status chat message:
    long elapsed = (now - empire_game_start_utc) / 1000;
    if (elapsed > 0 && elapsed % 60 == 0):
      long remaining = (300000 - (now - empire_game_start_utc)) / 1000;
      CPH.SendMessage($"⚔️ {players.Count} rebels holding. {remaining}s until victory!")
    (Use a simple modulo check; may fire slightly off due to 5s resolution — acceptable.)
```

### TriggerWin helper

```csharp
private void TriggerWin(List<EmpirePlayer> survivors, List<EmpireCell> empire)
{
    CPH.DisableTimer(TIMER_GAME_TICK);
    CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, false, false);
    CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, false, false);

    // Persist clone unlocked for Disco Party integration
    CPH.SetGlobalVar("clone_unlocked", true, true);

    // Trigger Mix It Up unlock sequence
    bool unlockTriggered = TriggerMixItUpCommand(
        MIXITUP_CLONE_UNLOCK_COMMAND_ID,
        "Clone Empire Win"
    );
    if (unlockTriggered)
        CPH.Wait(WAIT_MIXITUP_UNLOCK_STARTUP_MS);

    // Publish win end event
    PublishCloneEnd("win", survivors);

    string names = string.Join(", ", survivors.ConvertAll(p => p.UserName));
    CPH.SendMessage($"🏆 THE REBELS HAVE HELD THE LINE! Clone squad member unlocked! Survivors: {names}");

    ReleaseMiniGameLockIfOwned();
}
```

Include:
- `TriggerMixItUpCommand` from HELPER-SNIPPETS.md §2
- `TriggerLoss` helper (same as in move script — copy it)
- `Rule3Check` helper (same as in move script — copy it)
- `ReleaseMiniGameLockIfOwned` from HELPER-SNIPPETS.md §1
- JSON helpers from HELPER-SNIPPETS.md §7
- Broker publish helpers

### Constants

Same constants block as move script, plus:
```csharp
private const long EMPIRE_WIN_DURATION_MS    = 300000L;
private const long EMPIRE_INACTIVITY_KILL_MS = 30000L;
```

---

## Shared private helpers (both scripts)

Both scripts need:
1. `PublishBrokerMessage(string topic, string payloadJson)` — wraps `CPH.WebsocketSend`
2. `PublishCloneUpdate(string event, List<EmpirePlayer> players, List<EmpireCell> empire, string detail = "")` — builds and sends `squad.clone.update`
3. `PublishCloneEnd(string outcome, List<EmpirePlayer> survivors)` — builds and sends `squad.clone.end`
4. `GetElapsedSeconds()` — reads `empire_game_start_utc`, computes seconds since then
5. All JSON helpers from HELPER-SNIPPETS.md §7
6. Mini-game lock helpers from §1
7. `TriggerMixItUpCommand` from §2 (tick script only needs this)

Since Streamer.bot scripts are self-contained, copy all helpers into each script.

---

## Commenting Standard

Full header comment block per `roles/streamerbot-dev/skills/core.md`. Plain-language inline comments.

---

## Wiring Required in Streamer.bot UI

### Movement Commands

`!up`, `!down`, `!left`, `!right` already exist as Streamer.bot command triggers (used by Destroyer mini-game). Add `Clone - Empire - Move` as an **additional action** on each of those four triggers. Do not remove the Destroyer action.

| Action Name | Script | Trigger |
|---|---|---|
| `Clone - Empire - Move` | `clone-empire-move.cs` | `!up`, `!down`, `!left`, `!right` (add to all four) |
| `Clone - Empire - Tick` | `clone-empire-tick.cs` | Timer `Clone - Game Tick` |

### Timer

`Clone - Game Tick` was created as disabled in Prompt 02. Set it to 5-second repeating interval. The `clone-empire-start.cs` script enables it when the movement phase opens.

### Move command argument passing

If you implement direction detection via `rawInput` (recommended), no Streamer.bot UI argument setup is needed — the script reads it directly from the argument list.

If you prefer to pass direction as an explicit argument, configure each command's action to pass `%command%` or a hardcoded string. Document your choice in the script header.

---

## After Completing

1. Remove row from WORKING.md Active Work. Add to Recently Completed.
2. Load `ops/skills/change-summary/_index.md` and produce a paste-target summary.

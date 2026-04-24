# Prompt 02 — Clone Grid Game: Game-Start Streamer.bot Scripts

## Context

You are working on SharkStreamerBot. Read `.agents/ENTRY.md` before starting. Activate the **streamerbot-dev** role (`roles/streamerbot-dev/`). Read `roles/streamerbot-dev/skills/core.md`.

Check `WORKING.md` before touching any files. Add your row to Active Work.

**Prerequisite:** Prompt 01 must be complete (protocol types and SHARED-CONSTANTS entries already written).

Read these files before writing any code:
- `Actions/HELPER-SNIPPETS.md` — copy/paste patterns for mini-game lock, Mix It Up API, JSON helpers, broker publish
- `Actions/SHARED-CONSTANTS.md` — canonical var/timer/topic names for this feature

---

## What You Are Building

Three Streamer.bot C# scripts for the **join phase** of the Clone Grid mini-game:

| File | Trigger | Purpose |
|------|---------|---------|
| `Actions/Squad/Clone/clone-empire-main.cs` | `!clone` chat command | Acquires lock, starts join window, adds initiator as first player |
| `Actions/Squad/Clone/clone-empire-join.cs` | `!join` chat command | Adds viewer to player list during join window |
| `Actions/Squad/Clone/clone-empire-start.cs` | Timer `Clone - Join Window` fires | Closes join window, spawns empire, opens movement phase |

Do NOT edit or delete any existing clone files in this prompt. Do NOT touch stream-start.cs. Do NOT touch overlay TypeScript. New files only.

---

## Game Design Reference (full — read before coding)

**Grid:** 32 columns × 18 rows, 1-indexed. Col 1 = left edge, col 32 = right edge. Row 1 = top edge, row 18 = bottom edge.

**Spawn:** All players spawn at col 16, row 9 (center). Multiple players may share a cell.

**Join window:** 60 seconds from `!clone`. Any viewer may type `!join`. The user who typed `!clone` is automatically the first player (no need to also type `!join`).

**Empire spawn:** After join window closes, 5 empire ships are placed at random grid positions. Empire must not spawn at the center (16, 9). No two empire on same cell.

**Movement phase opens** once empire is placed. Inactivity timer (30s per player) begins. Win timer (5 min) begins.

**`empire_players_json` structure:**
```json
[{"userId":"12345","userName":"alice","col":16,"row":9,"lastMoveUtc":1714000000000}]
```
`lastMoveUtc` is set to `empire_game_start_utc` when movement opens (tick script handles inactivity from that point).

**`empire_cells_json` structure:**
```json
[{"col":5,"row":3},{"col":12,"row":7}]
```

**Broker topics (copy from SHARED-CONSTANTS.md):**
- `squad.clone.start` — sent by clone-empire-main.cs when join window opens
- `squad.clone.update` — sent by clone-empire-start.cs when game phase begins

---

## JSON Helper Requirement

All three scripts need the hand-rolled JSON parser/serializer from `Actions/HELPER-SNIPPETS.md` §7. Copy the full internals verbatim into each script that parses or writes JSON (every script here does). Do not import external libraries.

Define these data classes in each script that uses them:

```csharp
[DataContract]
private class EmpirePlayer
{
    [DataMember(Name = "userId")]   public string UserId   { get; set; }
    [DataMember(Name = "userName")] public string UserName { get; set; }
    [DataMember(Name = "col")]      public int    Col      { get; set; }
    [DataMember(Name = "row")]      public int    Row      { get; set; }
    [DataMember(Name = "lastMoveUtc")] public long LastMoveUtc { get; set; }
}

[DataContract]
private class EmpireCell
{
    [DataMember(Name = "col")] public int Col { get; set; }
    [DataMember(Name = "row")] public int Row { get; set; }
}
```

---

## Script 1: `clone-empire-main.cs`

**Trigger:** `!clone` chat command  
**Required args:** `user` (string), `userId` (string)

### Logic

1. Read mini-game lock (`minigame_active`, `minigame_name`). If another game owns lock → send chat message and return.
2. Read `empire_game_active`. If already true → send chat "⚔️ The Empire is already advancing! Type !join to join the resistance." and return.
3. Acquire mini-game lock with name `"clone_empire"`.
4. Set `empire_game_active = true` (non-persisted).
5. Set `empire_join_active = true` (non-persisted).
6. Set `empire_game_start_utc = 0` (non-persisted; movement not open yet).
7. Build initial player list with just the initiator at center (col 16, row 9, lastMoveUtc 0).
8. Set `empire_players_json` = serialized player list (non-persisted).
9. Set `empire_cells_json = "[]"` (non-persisted).
10. Enable timer `"Clone - Join Window"`.
11. Publish broker message `squad.clone.start` (see payload below).
12. Send chat: `"⚔️ EMPIRE APPROACHES! Type !join in the next 60 seconds to join the Rebel defense! {user} leads the resistance! (1 rebel)"`

### Broker publish — `squad.clone.start`

Publish via WebSocket to the broker using WebSocket index 0 (constant `BROKER_WS_INDEX = 0`).

Payload shape (serialize to JSON string):
```json
{
  "id": "<uuid-v4>",
  "topic": "squad.clone.start",
  "sender": "streamerbot",
  "timestamp": <unix-ms>,
  "payload": {
    "game": "clone",
    "triggeredBy": "<userName>",
    "phase": "join",
    "joinWindowSeconds": 60,
    "players": [{"userId":"...","userName":"...","col":16,"row":9}],
    "gridCols": 32,
    "gridRows": 18
  }
}
```

For UUID generation, use `Guid.NewGuid().ToString()`. For timestamp, use `DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()`.

Use `CPH.WebsocketSend(jsonString, BROKER_WS_INDEX)` to publish. Wrap in try/catch and log on failure.

### Constants block

```csharp
private const string VAR_MINIGAME_ACTIVE    = "minigame_active";
private const string VAR_MINIGAME_NAME      = "minigame_name";
private const string MINIGAME_NAME          = "clone_empire";
private const string VAR_EMPIRE_GAME_ACTIVE = "empire_game_active";
private const string VAR_EMPIRE_JOIN_ACTIVE = "empire_join_active";
private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
private const string VAR_EMPIRE_CELLS_JSON  = "empire_cells_json";
private const string TIMER_JOIN_WINDOW      = "Clone - Join Window";
private const int    BROKER_WS_INDEX        = 0;
private const int    EMPIRE_SPAWN_COL       = 16;
private const int    EMPIRE_SPAWN_ROW       = 9;
```

Include mini-game lock helpers from HELPER-SNIPPETS.md §1. Include JSON helpers from §7. Include broker publish helper (build from §5 WebSocket pattern).

---

## Script 2: `clone-empire-join.cs`

**Trigger:** `!join` chat command  
**Required args:** `user` (string), `userId` (string)

> Note: `!join` is also used by LotAT. This script guards itself with `empire_join_active` — it will silently return if the join window is not open. LotAT has its own guard on `lotat_active`. No conflict.

### Logic

1. Read `empire_join_active` (non-persisted). If not true → return immediately (no chat message — LotAT may own this command).
2. Read `empire_players_json` and deserialize.
3. Check if `userId` already exists in the list. If yes → send chat "@{user} you're already in the resistance!" and return.
4. Add new player at center (col 16, row 9, lastMoveUtc 0).
5. Serialize and save updated `empire_players_json`.
6. Count total players.
7. Publish broker message `squad.clone.update` with event `"player_joined"` (see below).
8. Send chat: `"🚀 {user} joins the Rebel forces! ({count} rebels standing by)"`

### Broker publish — `squad.clone.update` (player_joined)

```json
{
  "id": "<uuid>",
  "topic": "squad.clone.update",
  "sender": "streamerbot",
  "timestamp": <unix-ms>,
  "payload": {
    "game": "clone",
    "state": {
      "event": "player_joined",
      "players": [...],
      "empire": [],
      "gridCols": 32,
      "gridRows": 18,
      "elapsedSeconds": 0,
      "survivorCount": <count>,
      "empireCount": 0,
      "eventDetail": "<userName>"
    }
  }
}
```

### Constants block

```csharp
private const string VAR_EMPIRE_JOIN_ACTIVE  = "empire_join_active";
private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
private const string VAR_EMPIRE_CELLS_JSON   = "empire_cells_json";
private const int    BROKER_WS_INDEX         = 0;
private const int    EMPIRE_SPAWN_COL        = 16;
private const int    EMPIRE_SPAWN_ROW        = 9;
```

Include JSON helpers. Include broker publish helper.

---

## Script 3: `clone-empire-start.cs`

**Trigger:** Timer `"Clone - Join Window"` fires (fires once, 60 seconds after it was enabled)  
**Required args:** None (timer-triggered)

### Logic

1. Disable timer `"Clone - Join Window"` immediately (prevent double-fire).
2. Set `empire_join_active = false`.
3. Read `empire_players_json` and deserialize.
4. If player list is empty → cancel game:
   - Release mini-game lock (set `minigame_active = false`, `minigame_name = ""`).
   - Set `empire_game_active = false`.
   - Send chat: `"⚔️ No rebels showed up. The Empire claims this sector unopposed."`
   - Return.
5. Spawn 5 empire ships at random positions:
   - Use `Random` to pick col 1–32 and row 1–18.
   - **Skip** position (16, 9) — the player spawn.
   - **Skip** any position already chosen (no duplicate empire cells).
   - If random lands on a blocked position, retry until a valid spot is found (max 1000 attempts total to prevent infinite loop; if exhausted, use fewer empire ships and log a warning).
6. Set `empire_game_start_utc` = current Unix ms.
7. Update all players' `lastMoveUtc` = `empire_game_start_utc` (reset inactivity clock to now).
8. Save updated `empire_players_json`.
9. Save `empire_cells_json`.
10. Enable timer `"Clone - Game Tick"` (repeating 5-second tick).
11. Publish broker message `squad.clone.update` with event `"game_start"` (full game state, see below).
12. Send chat: `"⚔️ {N} Rebels on the grid! The Empire has deployed {5} ships. MOVE with !up !down !left !right — survive 5 minutes to unlock Clone! ☠️ Danger: standing still for 30s = death!"`

### Broker publish — `squad.clone.update` (game_start)

```json
{
  "id": "<uuid>",
  "topic": "squad.clone.update",
  "sender": "streamerbot",
  "timestamp": <unix-ms>,
  "payload": {
    "game": "clone",
    "state": {
      "event": "game_start",
      "players": [...all players...],
      "empire": [...all 5 empire cells...],
      "gridCols": 32,
      "gridRows": 18,
      "elapsedSeconds": 0,
      "survivorCount": <count>,
      "empireCount": 5,
      "eventDetail": "Movement phase open"
    }
  }
}
```

### Constants block

```csharp
private const string VAR_MINIGAME_ACTIVE     = "minigame_active";
private const string VAR_MINIGAME_NAME       = "minigame_name";
private const string MINIGAME_NAME           = "clone_empire";
private const string VAR_EMPIRE_GAME_ACTIVE  = "empire_game_active";
private const string VAR_EMPIRE_JOIN_ACTIVE  = "empire_join_active";
private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
private const string VAR_EMPIRE_CELLS_JSON   = "empire_cells_json";
private const string TIMER_JOIN_WINDOW       = "Clone - Join Window";
private const string TIMER_GAME_TICK         = "Clone - Game Tick";
private const int    BROKER_WS_INDEX         = 0;
private const int    EMPIRE_GRID_COLS        = 32;
private const int    EMPIRE_GRID_ROWS        = 18;
private const int    EMPIRE_SPAWN_COL        = 16;
private const int    EMPIRE_SPAWN_ROW        = 9;
private const int    EMPIRE_INITIAL_COUNT    = 5;
```

Include JSON helpers. Include release-lock helper. Include broker publish helper.

---

## Commenting Standard

Follow `roles/streamerbot-dev/skills/core.md` — every script needs a header comment block:
- Purpose
- Expected trigger/input
- Required runtime variables
- Key outputs/side effects
- Operator notes

Inline comments should explain intent in plain language for a beginner developer.

---

## Wiring Required in Streamer.bot UI

After pasting the scripts into Streamer.bot, the operator must set up:

### New Actions

| Action Name | Script | Trigger |
|---|---|---|
| `Clone - Empire - Start Game` | `clone-empire-main.cs` | Chat command `!clone` |
| `Clone - Empire - Join` | `clone-empire-join.cs` | Chat command `!join` |
| `Clone - Empire - Open Movement` | `clone-empire-start.cs` | Timer `Clone - Join Window` |

### New Timers

| Timer Name | Interval | Repeat | Fires Action |
|---|---|---|---|
| `Clone - Join Window` | 60 seconds | No (one-shot) | `Clone - Empire - Open Movement` |
| `Clone - Game Tick` | 5 seconds | Yes (repeating) | `Clone - Empire - Tick` *(created in Prompt 03)* |

> Create the `Clone - Game Tick` timer now but leave it **disabled**. Prompt 03 creates the action it fires.

### Existing Action Updates

The existing `!join` command in Streamer.bot may already point to an action. Add `Clone - Empire - Join` as an **additional action** on the same `!join` trigger (not a replacement). The LotAT join script guards itself with `lotat_active`; this script guards itself with `empire_join_active`. They coexist safely.

---

## After Completing

1. Remove row from WORKING.md Active Work. Add to Recently Completed.
2. Load `ops/skills/change-summary/_index.md` and produce a paste-target summary.

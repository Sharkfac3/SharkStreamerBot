// ACTION-CONTRACT: Actions/Twitch Core Integrations/AGENTS.md#stream-start.cs
// ACTION-CONTRACT-SHA256: 94e70f7c6375a9282390571fba3b839341006b9486e2c0babbadb53371b78fbb

using System;
using System.Collections.Generic;

public class CPHInline
{
    // Runtime source of truth: Actions/Twitch Core Integrations/AGENTS.md
    // Shared names/constants reference: Actions/SHARED-CONSTANTS.md
    private const string OBS_SCENE_DISCO_WORKSPACE = "Disco Party: Workspace";

    // Duck
    private const string VAR_DUCK_EVENT_ACTIVE = "duck_event_active";
    private const string VAR_DUCK_QUACK_COUNT = "duck_quack_count";
    private const string VAR_DUCK_CALLER = "duck_caller";
    private const string VAR_DUCK_UNLOCKED = "duck_unlocked";
    private const string VAR_DUCK_TARGET_QUACKS = "duck_target_quacks";
    private const string VAR_DUCK_UNIQUE_QUACKERS = "duck_unique_quackers";
    private const string VAR_DUCK_UNIQUE_QUACKER_COUNT = "duck_unique_quacker_count";
    private const string OBS_SOURCE_DUCK_DANCING = "Duck - Dancing";
    private const string TIMER_DUCK_CALL_WINDOW = "Duck - Call Window";

    // Clone
    private const string VAR_EMPIRE_GAME_ACTIVE = "empire_game_active";
    private const string VAR_EMPIRE_JOIN_ACTIVE = "empire_join_active";
    private const string VAR_EMPIRE_GAME_START_UTC = "empire_game_start_utc";
    private const string VAR_EMPIRE_PLAYERS_JSON = "empire_players_json";
    private const string VAR_EMPIRE_CELLS_JSON = "empire_cells_json";
    private const string OBS_SOURCE_CLONE_DANCING = "Clone - Dancing";
    private const string TIMER_CLONE_JOIN_WINDOW = "Clone - Join Window";
    private const string TIMER_CLONE_GAME_TICK = "Clone - Game Tick";

    // Pedro
    private const string VAR_PEDRO_GAME_ENABLED = "pedro_game_enabled";
    private const string VAR_PEDRO_MENTION_COUNT = "pedro_mention_count";
    private const string VAR_PEDRO_UNLOCKED = "pedro_unlocked";
    private const string VAR_PEDRO_NEXT_ALLOWED_UTC = "pedro_next_allowed_utc";
    private const string VAR_PEDRO_SECRET_UNLOCK_ACTIVE = "pedro_secret_unlock_active";
    private const string VAR_PEDRO_LAST_MESSAGE_ID = "pedro_last_message_id";
    private const string OBS_SOURCE_PEDRO_DANCING = "Pedro - Dancing";
    private const string TIMER_PEDRO_CALL_WINDOW = "Pedro - Call Window";

    // Toothless
    private const string PREFIX_RARITY = "rarity_";
    private const string VAR_LAST_ROLL = "last_roll";
    private const string VAR_LAST_RARITY = "last_rarity";
    private const string VAR_LAST_USER = "last_user";

    // LotAT
    private const string VAR_LOTAT_ACTIVE = "lotat_active";
    private const string VAR_LOTAT_SESSION_ID = "lotat_session_id";
    private const string VAR_LOTAT_SESSION_STAGE = "lotat_session_stage";
    private const string VAR_LOTAT_SESSION_STORY_ID = "lotat_session_story_id";
    private const string VAR_LOTAT_SESSION_CURRENT_NODE_ID = "lotat_session_current_node_id";
    private const string VAR_LOTAT_SESSION_CHAOS_TOTAL = "lotat_session_chaos_total";
    private const string VAR_LOTAT_SESSION_ROSTER_FROZEN = "lotat_session_roster_frozen";
    private const string VAR_LOTAT_SESSION_JOINED_ROSTER_JSON = "lotat_session_joined_roster_json";
    private const string VAR_LOTAT_SESSION_JOINED_COUNT = "lotat_session_joined_count";
    private const string VAR_LOTAT_NODE_ACTIVE_WINDOW = "lotat_node_active_window";
    private const string VAR_LOTAT_NODE_WINDOW_RESOLVED = "lotat_node_window_resolved";
    private const string VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON = "lotat_node_allowed_commands_json";
    private const string VAR_LOTAT_VOTE_MAP_JSON = "lotat_vote_map_json";
    private const string VAR_LOTAT_VOTE_VALID_COUNT = "lotat_vote_valid_count";
    private const string VAR_LOTAT_NODE_COMMANDER_NAME = "lotat_node_commander_name";
    private const string VAR_LOTAT_NODE_COMMANDER_TARGET_USER = "lotat_node_commander_target_user";
    private const string VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON = "lotat_node_commander_allowed_commands_json";
    private const string VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD = "lotat_node_dice_success_threshold";
    private const string VAR_LOTAT_SESSION_LAST_CHOICE_ID = "lotat_session_last_choice_id";
    private const string VAR_LOTAT_SESSION_LAST_END_STATE = "lotat_session_last_end_state";
    private const string VAR_LOTAT_ANNOUNCEMENT_SENT = "lotat_announcement_sent";
    private const string VAR_LOTAT_OFFERING_STEAL_CHANCE = "lotat_offering_steal_chance";
    private const string VAR_LOTAT_STEAL_MULTIPLIER = "lotat_steal_multiplier";
    private const string TIMER_LOTAT_JOIN_WINDOW = "LotAT - Join Window";
    private const string TIMER_LOTAT_DECISION_WINDOW = "LotAT - Decision Window";
    private const string TIMER_LOTAT_COMMANDER_WINDOW = "LotAT - Commander Window";
    private const string TIMER_LOTAT_DICE_WINDOW = "LotAT - Dice Window";

    // Shared mini-game lock (cross-feature)
    private const string VAR_MINIGAME_ACTIVE = "minigame_active";
    private const string VAR_MINIGAME_NAME = "minigame_name";

    // Stream mode (shared across Twitch integration and mode actions)
    private const string VAR_STREAM_MODE = "stream_mode";
    private const string MODE_WORKSPACE = "workspace";

    // Disco Party channel point redeem
    private const string VAR_DISCO_PARTY_ACTIVE     = "disco_party_active";
    private const string VAR_DISCO_PARTY_PREV_SCENE = "disco_party_prev_scene";

    // Rest / Focus loop
    private const string VAR_REST_FOCUS_LOOP_ACTIVE = "rest_focus_loop_active";
    private const string VAR_REST_FOCUS_LOOP_PHASE = "rest_focus_loop_phase";
    private const string PHASE_IDLE = "idle";
    private const string TIMER_REST_FOCUS_PRE_REST = "Rest Focus - Pre Rest";
    private const string TIMER_REST_FOCUS_REST = "Rest Focus - Rest";
    private const string TIMER_REST_FOCUS_PRE_FOCUS = "Rest Focus - Pre Focus";
    private const string TIMER_REST_FOCUS_FOCUS = "Rest Focus - Focus";

    // Temporary timers
    private const string TIMER_TEMP_FOCUS = "Temp Focus Timer";

    // XJ Drivethrough
    private const string VAR_XJ_ACTIVE = "xj_drivethrough_active";

    // Overlay broker
    private const int    BROKER_WS_INDEX      = 0;
    private const string VAR_BROKER_CONNECTED = "broker_connected";
    private const string BROKER_CLIENT_NAME   = "streamerbot";
    private const int    WAIT_BROKER_CONNECT_MS = 600;

    // Stream-start reset. Behavior contract lives in the local AGENTS.md.
    public bool Execute()
    {
        // Broker failure is non-fatal; continue startup reset.
        EnsureOverlayBrokerConnected();

        // Shared mini-game lock.
        CPH.SetGlobalVar(VAR_MINIGAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_MINIGAME_NAME, "", false);

        // Disco Party reset.
        CPH.SetGlobalVar(VAR_DISCO_PARTY_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_DISCO_PARTY_PREV_SCENE, "", false);

        // Default stream mode.
        CPH.SetGlobalVar(VAR_STREAM_MODE, MODE_WORKSPACE, false);

        // Toothless rarity list maps to source names and unlock flag keys.
        var toothlessRarities = new List<string>
        {
            "regular",
            "smol",
            "long",
            "flight",
            "party"
        };

        // Toothless reset.
        foreach (string rarity in toothlessRarities)
        {
            string dancingSource = $"Toothless - Dancing - {rarity}";

            // Hide/show/hide clears stale OBS visibility cache.
            CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);
            CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);
            CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, dancingSource);

            // Per-stream unlock flags.
            CPH.SetGlobalVar($"{PREFIX_RARITY}{rarity}", false, false);
        }

        // Latest-roll breadcrumbs.
        CPH.SetGlobalVar(VAR_LAST_ROLL, 0, false);
        CPH.SetGlobalVar(VAR_LAST_RARITY, "", false);
        CPH.SetGlobalVar(VAR_LAST_USER, "", false);

        // Note: per-user boosts are stored as boost_<member>_<userId>.
        // We intentionally do not iterate/clear unknown user IDs here.

        // LotAT + offerings reset; disable timers before state reset.
        CPH.DisableTimer(TIMER_LOTAT_JOIN_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DECISION_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_COMMANDER_WINDOW);
        CPH.DisableTimer(TIMER_LOTAT_DICE_WINDOW);

        // LotAT v1 runtime defaults.
        CPH.SetGlobalVar(VAR_LOTAT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STAGE, PHASE_IDLE, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_STORY_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CURRENT_NODE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_CHAOS_TOTAL, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_ROSTER_FROZEN, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_ROSTER_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_JOINED_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ACTIVE_WINDOW, "none", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_WINDOW_RESOLVED, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_MAP_JSON, "{}", false);
        CPH.SetGlobalVar(VAR_LOTAT_VOTE_VALID_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_NAME, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_TARGET_USER, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_CHOICE_ID, "", false);
        CPH.SetGlobalVar(VAR_LOTAT_SESSION_LAST_END_STATE, "", false);

        // Legacy offering state; not active LotAT engine state.
        CPH.SetGlobalVar(VAR_LOTAT_ANNOUNCEMENT_SENT, false, false);
        CPH.SetGlobalVar(VAR_LOTAT_OFFERING_STEAL_CHANCE, 0, false);
        CPH.SetGlobalVar(VAR_LOTAT_STEAL_MULTIPLIER, 1, false);

        // Duck reset.
        CPH.SetGlobalVar(VAR_DUCK_EVENT_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_DUCK_QUACK_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_DUCK_CALLER, "", false);
        CPH.SetGlobalVar(VAR_DUCK_UNLOCKED, false, false);
        CPH.SetGlobalVar(VAR_DUCK_TARGET_QUACKS, 0, false);
        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKERS, "|", false);
        CPH.SetGlobalVar(VAR_DUCK_UNIQUE_QUACKER_COUNT, 0, false);

        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_DUCK_DANCING);
        CPH.DisableTimer(TIMER_DUCK_CALL_WINDOW);

        // Clone reset.
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_CLONE_DANCING);

        // Clone Grid Game non-persisted state; leave persisted clone_unlocked untouched.
        CPH.SetGlobalVar(VAR_EMPIRE_GAME_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_EMPIRE_JOIN_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_EMPIRE_GAME_START_UTC, 0L, false);
        CPH.SetGlobalVar(VAR_EMPIRE_PLAYERS_JSON, "[]", false);
        CPH.SetGlobalVar(VAR_EMPIRE_CELLS_JSON, "[]", false);

        // Clone timers.
        CPH.DisableTimer(TIMER_CLONE_JOIN_WINDOW);
        CPH.DisableTimer(TIMER_CLONE_GAME_TICK);

        // Pedro reset.
        CPH.SetGlobalVar(VAR_PEDRO_GAME_ENABLED, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_MENTION_COUNT, 0, false);
        CPH.SetGlobalVar(VAR_PEDRO_UNLOCKED, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_NEXT_ALLOWED_UTC, "", false);
        CPH.SetGlobalVar(VAR_PEDRO_SECRET_UNLOCK_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_PEDRO_LAST_MESSAGE_ID, "", false);

        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.ObsShowSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.ObsHideSource(OBS_SCENE_DISCO_WORKSPACE, OBS_SOURCE_PEDRO_DANCING);
        CPH.DisableTimer(TIMER_PEDRO_CALL_WINDOW);

        // Rest / Focus loop reset.
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_ACTIVE, false, false);
        CPH.SetGlobalVar(VAR_REST_FOCUS_LOOP_PHASE, PHASE_IDLE, false);
        CPH.DisableTimer(TIMER_REST_FOCUS_PRE_REST);
        CPH.DisableTimer(TIMER_REST_FOCUS_REST);
        CPH.DisableTimer(TIMER_REST_FOCUS_PRE_FOCUS);
        CPH.DisableTimer(TIMER_REST_FOCUS_FOCUS);

        // Temporary timer reset.
        CPH.DisableTimer(TIMER_TEMP_FOCUS);

        // XJ Drivethrough reset.
        CPH.SetGlobalVar(VAR_XJ_ACTIVE, false, false);

        return true;
    }

    private bool EnsureOverlayBrokerConnected()
    {
        const string LOG_PREFIX = "[StreamStartBroker]";

        // WebsocketIsConnected() checks TCP state only — not whether ClientHello
        // was sent. Only skip when both the socket is open and our non-persisted
        // registration flag says the broker hello already succeeded.
        bool alreadyConnected = CPH.WebsocketIsConnected(BROKER_WS_INDEX);
        bool helloSent = (CPH.GetGlobalVar<bool?>(VAR_BROKER_CONNECTED, false) ?? false);
        if (alreadyConnected && helloSent)
        {
            CPH.LogWarn($"{LOG_PREFIX} Overlay broker already connected and registered.");
            return true;
        }

        // Clear stale registration state before connect/register.
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, false, false);

        if (!alreadyConnected)
        {
            CPH.LogWarn($"{LOG_PREFIX} Connecting to overlay broker (WS client index {BROKER_WS_INDEX})...");
            CPH.WebsocketConnect(BROKER_WS_INDEX);
            CPH.Wait(WAIT_BROKER_CONNECT_MS);
        }
        else
        {
            CPH.LogWarn($"{LOG_PREFIX} TCP socket already open. Sending ClientHello to register with broker...");
        }

        if (!CPH.WebsocketIsConnected(BROKER_WS_INDEX))
        {
            CPH.LogError(
                $"{LOG_PREFIX} Connection failed after {WAIT_BROKER_CONNECT_MS}ms. " +
                "Is the stream-overlay broker running at ws://localhost:8765? " +
                "Stream start will continue, but overlay broker publishes may fail until reconnected."
            );
            return false;
        }

        string hello =
            "{\"type\":\"client.hello\"," +
            "\"name\":\"" + BROKER_CLIENT_NAME + "\"," +
            "\"subscriptions\":[]}";

        CPH.WebsocketSend(hello, BROKER_WS_INDEX);
        CPH.SetGlobalVar(VAR_BROKER_CONNECTED, true, false);
        CPH.LogWarn($"{LOG_PREFIX} Overlay broker connected and ClientHello sent.");
        return true;
    }
}

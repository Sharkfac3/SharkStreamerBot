---
id: constants-lotat
type: constants
description: LotAT session state, roster, dice, and offering globals.
owner: lotat-tech
secondaryOwners: [streamerbot-dev]
parent: ../SHARED-CONSTANTS.md
---

# LotAT Constants

Canonical globals for the Legends of the ASCII Temple runtime. These names are the source of truth for all LotAT C# scripts.

## LotAT / Offering (shared)
- `VAR_LOTAT_ACTIVE` = `lotat_active` *(active LotAT session flag; do not treat as an offering toggle in LotAT v1 docs/implementation)*
- `VAR_LOTAT_SESSION_ID` = `lotat_session_id`
- `VAR_LOTAT_SESSION_STAGE` = `lotat_session_stage`
- `VAR_LOTAT_SESSION_STORY_ID` = `lotat_session_story_id`
- `VAR_LOTAT_SESSION_CURRENT_NODE_ID` = `lotat_session_current_node_id`
- `VAR_LOTAT_SESSION_CHAOS_TOTAL` = `lotat_session_chaos_total`
- `VAR_LOTAT_SESSION_ROSTER_FROZEN` = `lotat_session_roster_frozen`
- `VAR_LOTAT_SESSION_JOINED_ROSTER_JSON` = `lotat_session_joined_roster_json`
- `VAR_LOTAT_SESSION_JOINED_COUNT` = `lotat_session_joined_count`
- `VAR_LOTAT_NODE_ACTIVE_WINDOW` = `lotat_node_active_window`
- `VAR_LOTAT_NODE_WINDOW_RESOLVED` = `lotat_node_window_resolved`
- `VAR_LOTAT_NODE_ALLOWED_COMMANDS_JSON` = `lotat_node_allowed_commands_json`
- `VAR_LOTAT_VOTE_MAP_JSON` = `lotat_vote_map_json`
- `VAR_LOTAT_VOTE_VALID_COUNT` = `lotat_vote_valid_count`
- `VAR_LOTAT_NODE_COMMANDER_NAME` = `lotat_node_commander_name`
- `VAR_LOTAT_NODE_COMMANDER_TARGET_USER` = `lotat_node_commander_target_user`
- `VAR_LOTAT_NODE_COMMANDER_ALLOWED_COMMANDS_JSON` = `lotat_node_commander_allowed_commands_json`
- `VAR_LOTAT_NODE_DICE_SUCCESS_THRESHOLD` = `lotat_node_dice_success_threshold`
- `VAR_LOTAT_SESSION_LAST_CHOICE_ID` = `lotat_session_last_choice_id`
- `VAR_LOTAT_SESSION_LAST_END_STATE` = `lotat_session_last_end_state` *(recommended v1 history breadcrumb; safe idle default is empty string)*
- `VAR_LOTAT_ANNOUNCEMENT_SENT` = `lotat_announcement_sent` *(legacy / provisional offering-system latch)*
- `VAR_LOTAT_OFFERING_STEAL_CHANCE` = `lotat_offering_steal_chance` *(legacy / provisional offering variable; not active LotAT v1 engine contract)*
- `VAR_LOTAT_STEAL_MULTIPLIER` = `lotat_steal_multiplier` *(legacy / provisional offering variable; not active LotAT v1 engine contract)*
- `TIMER_LOTAT_JOIN_WINDOW` = `LotAT - Join Window`
- `TIMER_LOTAT_DECISION_WINDOW` = `LotAT - Decision Window`
- `TIMER_LOTAT_COMMANDER_WINDOW` = `LotAT - Commander Window`
- `TIMER_LOTAT_DICE_WINDOW` = `LotAT - Dice Window`
- `PREFIX_BOOST` = `boost_` *(external boost-system prefix; not LotAT v1 engine state by itself)*

Used in:
- `Actions/Squad/offering.cs`
- `Actions/Twitch Core Integrations/stream-start.cs`
- `Actions/LotAT/` *(planned engine implementation)*

Operator note:
- These are timer **names only**. V1 timer durations stay in the runtime contract / implementation layer.
- Runtime defaults to preserve when implementation begins: join = `120s`, decision = `120s`.
- Stream start should disable all four LotAT timers to clear stale state before returning LotAT to `idle`.
- Current LotAT v1 contract boundary: `!offering` remains out of scope until a future explicit integration decision is documented.

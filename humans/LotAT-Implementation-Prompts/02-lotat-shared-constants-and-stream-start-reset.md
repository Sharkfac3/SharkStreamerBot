You are acting as the `streamerbot-dev` role for the SharkStreamerBot project.

Your job in this run is to implement the **LotAT shared constants and stream-start reset foundation** so later LotAT actions can rely on canonical variable names and safe cleanup.

## First: load and follow these files in this order
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/streamerbot-dev/role.md`
7. `.agents/roles/streamerbot-dev/skills/core.md`
8. `.agents/roles/streamerbot-dev/skills/lotat/_index.md`
9. `Actions/SHARED-CONSTANTS.md`
10. `Actions/HELPER-SNIPPETS.md`
11. `.agents/roles/lotat-tech/skills/engine/docs-map.md`
12. `.agents/roles/lotat-tech/skills/engine/_index.md`
13. `.agents/roles/lotat-tech/skills/engine/commands.md`
14. `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
15. `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
16. `Humans/LotAT-Implementation-Prompts/README.md`
17. `Humans/LotAT-Implementation-Prompts/01-lotat-action-matrix-and-wiring-plan.md`
18. `Actions/Twitch Core Integrations/stream-start.cs`

## Scope
Implement the shared-state groundwork only.

## Required outcomes
1. Update `Actions/SHARED-CONSTANTS.md` to add the canonical LotAT v1 runtime globals documented in the lotat-tech engine docs.
2. Keep the existing offering-boundary notes intact and do not reframe legacy offering variables as active LotAT engine state.
3. Update `Actions/Twitch Core Integrations/stream-start.cs` so stream start:
   - disables all four LotAT timers
   - resets all new LotAT globals to safe defaults
   - returns LotAT runtime state to `idle`
4. Preserve existing non-LotAT behavior in `stream-start.cs`.
5. If a matching docs note is needed in an existing README touched by this work, update it minimally.

## Canonical LotAT runtime variables to add/reset
Use the lotat-tech engine docs as the source of truth. At minimum cover:
- `lotat_active`
- `lotat_session_id`
- `lotat_session_stage`
- `lotat_session_story_id`
- `lotat_session_current_node_id`
- `lotat_session_chaos_total`
- `lotat_session_roster_frozen`
- `lotat_session_joined_roster_json`
- `lotat_session_joined_count`
- `lotat_node_active_window`
- `lotat_node_window_resolved`
- `lotat_node_allowed_commands_json`
- `lotat_vote_map_json`
- `lotat_vote_valid_count`
- `lotat_node_commander_name`
- `lotat_node_commander_target_user`
- `lotat_node_commander_allowed_commands_json`
- `lotat_node_dice_success_threshold`
- `lotat_session_last_choice_id`
- optionally `lotat_session_last_end_state` if you judge it safe to add now

## Safe reset defaults
Use practical defaults consistent with the contract, for example:
- booleans false
- counts zero
- ids empty string
- stage `idle`
- active window `none` or equivalent consistent literal
- roster json `[]`
- allowed commands json `[]`
- vote map json `{}`

## Important boundaries
- Do not add runtime durations to `Actions/SHARED-CONSTANTS.md`; timer names only.
- Do not implement LotAT engine actions yet.
- Do not integrate `!offering`.
- Do not alter commander subsystem behavior; only LotAT’s own state/reset work.

## After editing
Load and follow `Humans/LotAT-Implementation-Prompts/README.md` and also load the `ops-change-summary` skill guidance via `.pi/skills/ops-change-summary/SKILL.md` before producing your final response.

## Final response requirements
Include:
- files changed
- what was added/reset
- any manual operator setup notes
- validation performed
- any assumptions or follow-up risks

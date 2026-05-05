This is the final step. Verify the diet work, commit, then delete the project folder.

---

STEP 1 — Run the validator

  python3 Tools/StreamerBot/Validation/action_contracts.py --all

Must pass with 0 errors. If there are failures:
- If the failure name matches a script that was edited in this project, fix it before continuing.
- If the failure is for a script that was not touched (pre-existing), note it and continue.

---

STEP 2 — Measure final line counts

Run:
  wc -l Actions/AGENTS.md Actions/*/AGENTS.md Actions/*/*/AGENTS.md

Report the new total. Note which files changed most.

---

STEP 3 — Full routing smoke check

Read every file in the table. Verify it still contains all listed non-negotiable content. Fix any gap before continuing — do not defer.

| File | Must contain |
|---|---|
| Actions/AGENTS.md | "Shared Ownership Rules", "Universal Script Rules", "Shared Required Reading" |
| Actions/Commanders/AGENTS.md | commander slot model, "SHARED-STATE", "Actions/AGENTS.md" in Required Reading |
| Actions/Commanders/Captain Stretch/AGENTS.md | "captain-stretch", "current_captain_stretch", "Actions/Commanders/AGENTS.md" in Required Reading |
| Actions/Commanders/The Director/AGENTS.md | "the-director-toad.cs", "current_the_director", "Actions/Commanders/AGENTS.md" in Required Reading |
| Actions/Commanders/Water Wizard/AGENTS.md | "water-wizard", "current_water_wizard", "Actions/Commanders/AGENTS.md" in Required Reading |
| Actions/Twitch Core Integrations/AGENTS.md | "stream-start.cs", "Mix It Up", "stream_mode" |
| Actions/Twitch Bits Integrations/AGENTS.md | "bits-tier-1.cs", "Mix It Up", "brand-steward" |
| Actions/Twitch Hype Train/AGENTS.md | "hype-train-level-up.cs", "brand-steward" |
| Actions/Twitch Channel Points/AGENTS.md | "disco-party.cs", "explain-current-task.cs" |
| Actions/Voice Commands/AGENTS.md | "mode-" vs "scene-" distinction, "stream_mode" |
| Actions/Overlay/AGENTS.md | "broker-connect.cs", "broker-publish.cs", "app-dev" |
| Actions/Squad/AGENTS.md | "mini-game lock", "Duck/README.md", "call/main/resolve" pattern |
| Actions/LotAT/AGENTS.md | "lotat-join.cs", phase sequence list, "implementation-map.md" |
| Actions/Intros/AGENTS.md | "first-chat-intro.cs", "info-service", "app-dev" |
| Actions/Rest Focus Loop/AGENTS.md | "rest-focus-loop-start.cs", "pre-rest"/"rest"/"pre-focus"/"focus" phases, "brand-steward" |
| Actions/XJ Drivethrough/AGENTS.md | "xj-drivethrough-main.cs", "triforce", "product-dev" |
| Actions/Destroyer/AGENTS.md | "destroyer-spawn.cs", "destroyer-move.cs", "broker" |
| Actions/Temporary/AGENTS.md | "temp-focus-timer-start.cs", "Temp Focus Timer", "Rest Focus Loop" |

---

STEP 4 — Commit all changes

Stage and commit everything under Actions/ that was modified:
  git add Actions/
  git commit -m "diet: compress Actions AGENTS.md files

Consolidate boilerplate to Actions/AGENTS.md parent. Compress ACTION-CONTRACTS
runtimeBehavior to tight one-liners. Replace verbose Script Reference tables
with compact per-script summaries. No contract literal values changed.

Validator: python3 Tools/StreamerBot/Validation/action_contracts.py --all passes."

---

STEP 5 — Delete this project folder

  rm -rf Projects/actions-diet

This project is complete. No artifacts should remain.

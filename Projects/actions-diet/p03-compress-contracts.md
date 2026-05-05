Edit the ACTION-CONTRACTS JSON blocks in the following 6 files:
- Actions/Commanders/AGENTS.md
- Actions/Commanders/Captain Stretch/AGENTS.md
- Actions/Commanders/The Director/AGENTS.md
- Actions/Commanders/Water Wizard/AGENTS.md
- Actions/Intros/AGENTS.md
- Actions/XJ Drivethrough/AGENTS.md

Do not touch any prose outside the `<!-- ACTION-CONTRACTS:START -->` / `<!-- ACTION-CONTRACTS:END -->` markers. Only the JSON inside those markers changes in this step.

BEFORE making any changes, run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Record the result. If there are failures, note them — they are pre-existing and not caused here.

---

FOR EACH contract entry in each file, apply these rules:

RULE 1 — Compress `runtimeBehavior` array items to tight one-liners (≤15 words each). The .cs file is the implementation source of truth; AGENTS.md is the contract summary. Keep at least 1 item — the validator rejects empty arrays.

Before example:
  "Reads the current_the_director global to determine the outgoing director before assigning the new one, then finalizes the outgoing director's award count."
After example:
  "Reads current_the_director, finalizes outgoing director award count, updates high-score if beaten."

RULE 2 — Remove `failureBehavior` arrays that only restate generic patterns like "log and return true on missing integration" or "exit safely if global missing." These are universal and no longer need to be per-contract. If a failureBehavior item documents a non-obvious, script-specific failure mode, keep it but compress it to one line.

RULE 3 — Compress `purpose` field values to one sentence (≤25 words) if currently longer.

DO NOT CHANGE:
- `script` field values (must match actual filenames)
- `action` field values (must match Streamer.bot action names)
- `pasteTarget` field values
- Any literal field values: `globals`, `timers`, `obsSources`, `obsScenes`, `mixItUpCommandIds`, `overlayTopics`, `serviceUrls`, `requiredLiterals`
- The markers: `<!-- ACTION-CONTRACTS:START -->`, `<!-- ACTION-CONTRACTS:END -->`
- The `"version": 1` and `"contracts": [...]` wrapper

AFTER editing all 6 files, run:
  python Tools/StreamerBot/Validation/action_contracts.py --stamp
  python Tools/StreamerBot/Validation/action_contracts.py --all

Must pass with 0 errors. If failures, fix before finishing this step.

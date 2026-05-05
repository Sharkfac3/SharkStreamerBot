Edit `Actions/Commanders/The Director/AGENTS.md`. This is a child file under the Commanders tree. The parent (`Actions/Commanders/AGENTS.md`) already defines the shared commander slot model. This file should only contain what is unique to The Director slot.

READ the file in full before editing.

REMOVE or REPLACE:

1. Ownership section → replace entire section with:
   "Owner: `streamerbot-dev`. Chain to `brand-steward` for The Director character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules."

2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required to pull the shared rules chain. Keep `Actions/Commanders/AGENTS.md` (slot model parent — same reason). Also keep CHARACTER-CODEX.md and BRAND-VOICE.md since The Director has a specific character identity.

3. Local Workflow steps that restate commander slot model rules → remove. Keep only The Director-specific steps. In particular, keep the note about `SCENE_SOURCE_MAP` sync requirement between `the-director-primary.cs` and `the-director-secondary.cs` — this is non-obvious and unique to this slot.

4. Validation / Boundaries / Out of Scope / Handoff sections → replace each with:
   "See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules."

5. Purpose section → trim to 2–3 lines covering: role assignment via channel-point redeem, !award support command, and The Director-only commands (!checkchat, !toad, !primary, !secondary).

KEEP:
- When to Activate file list
- The Director's unique state variable names (current_the_director, the_director_award_count, high-score keys)
- The SCENE_SOURCE_MAP sync rule (unique to this slot, non-obvious)
- The ACTION-CONTRACTS block exactly as-is (already compressed in p03 — do not touch)
- Script Summary table (compress to one-line-per-script if currently verbose)

After editing, confirm: reading `Actions/Commanders/AGENTS.md` plus this file gives complete guidance for The Director work. Run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Must pass with 0 errors.

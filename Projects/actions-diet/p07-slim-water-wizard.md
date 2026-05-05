Edit `Actions/Commanders/Water Wizard/AGENTS.md`. This is a child file under the Commanders tree. The parent (`Actions/Commanders/AGENTS.md`) already defines the shared commander slot model. This file should only contain what is unique to the Water Wizard slot.

READ the file in full before editing.

REMOVE or REPLACE:

1. Ownership section → replace entire section with:
   "Owner: `streamerbot-dev`. Chain to `brand-steward` for Water Wizard character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules."

2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required to pull the shared rules chain. Keep `Actions/Commanders/AGENTS.md` (slot model parent — same reason). Keep any Water Wizard-specific references alongside these.

3. Local Workflow steps that restate commander slot model rules → remove. Keep only steps unique to Water Wizard behavior (hydration commands, cast/orb/hail mechanics, or any non-obvious state the Water Wizard manages that isn't in the parent's SHARED-STATE).

4. Validation / Boundaries / Out of Scope / Handoff sections → replace each with:
   "See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules."

5. Purpose section → trim to 2–3 lines describing what Water Wizard uniquely does.

KEEP:
- When to Activate file list
- State variable names unique to Water Wizard
- Any Water Wizard-specific rules not covered by the parent
- The ACTION-CONTRACTS block exactly as-is (already compressed in p03 — do not touch)
- Script Summary table (compress to one-line-per-script if currently verbose)

After editing, confirm: reading `Actions/Commanders/AGENTS.md` plus this file gives complete guidance for Water Wizard work. Run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Must pass with 0 errors.

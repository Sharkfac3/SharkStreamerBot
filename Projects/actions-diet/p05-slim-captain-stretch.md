Edit `Actions/Commanders/Captain Stretch/AGENTS.md`. This is a child file under the Commanders tree. The parent (`Actions/Commanders/AGENTS.md`) already defines the shared commander slot model. This file should only contain what is unique to the Captain Stretch slot.

READ the file in full before editing.

REMOVE or REPLACE:

1. Ownership section → replace entire section with:
   "Owner: `streamerbot-dev`. Chain to `brand-steward` for Captain Stretch character voice, reward copy, or public command names. See `Actions/AGENTS.md` for full ownership rules."

2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required to pull the shared rules chain. Keep `Actions/Commanders/AGENTS.md` (the slot model parent — same reason: must be explicit). Keep any Captain Stretch-specific references (BRAND-VOICE, CHARACTER-CODEX if applicable) alongside these.

3. Local Workflow steps that restate commander slot model rules already in the parent AGENTS.md → remove. Keep only steps unique to Captain Stretch behavior.

4. Validation / Boundaries / Out of Scope / Handoff sections → replace each with:
   "See `Actions/Commanders/AGENTS.md` and `Actions/AGENTS.md` for shared rules."

5. Any Purpose text that restates the generic commander slot pattern → trim to 2–3 lines describing what Captain Stretch uniquely does.

KEEP:
- When to Activate file list (specific to this folder)
- State variable names unique to Captain Stretch (not already in Commanders parent SHARED-STATE)
- Any Captain Stretch-specific rules not covered by the parent
- The ACTION-CONTRACTS block exactly as-is (already compressed in p03 — do not touch)
- Script Summary table (keep, compress to one-line-per-script if currently verbose)

After editing, confirm: reading `Actions/Commanders/AGENTS.md` plus this file gives complete guidance for Captain Stretch work. Run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Must pass with 0 errors.

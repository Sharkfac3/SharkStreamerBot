Edit `Actions/Commanders/AGENTS.md`. This is the parent file that defines the shared commander slot model for Captain Stretch, The Director, and Water Wizard. Preserve the load-bearing content; remove boilerplate that now lives in Actions/AGENTS.md (added in p02).

READ the file in full before editing.

REMOVE or REPLACE:

1. Generic brand-steward escalation reminder → replace with one line:
   "Chain to `brand-steward` for any change to commander character name, role description, or reward copy."

2. Generic ops reminder → remove entirely (lives in Actions/AGENTS.md now).

3. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry. The streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first — this explicit pointer is the only guaranteed way to pull in the shared rules and reading chain. Keep any Commanders-specific references alongside it.

4. Any Local Workflow steps that restate universal script rules (self-contained, paste-ready, defensive read, SetGlobalVar persisted vs. non-persisted) → remove. Keep only commander-slot-specific steps.

5. Validation / Boundaries / Out of Scope / Handoff sections → replace each with a single line:
   "See Actions/AGENTS.md for universal validation, boundary, and handoff rules."

KEEP:
- The commander slot model definition (one active slot per role, backward-compat redeem behavior)
- The shared support command contract (!help, !award pattern)
- The SHARED-STATE section listing current_* globals and high-score keys
- The ACTION-CONTRACTS block exactly as-is (already compressed in p03 — do not touch)
- The Script Summary / Script Reference table (compress to compact one-line-per-script rows if it's a large table, but keep the table)

After editing, confirm: reading this file alone gives a complete understanding of the commander slot model. Run:
  python Tools/StreamerBot/Validation/action_contracts.py --all
Must pass with 0 errors.

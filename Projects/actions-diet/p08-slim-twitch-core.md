Edit `Actions/Twitch Core Integrations/AGENTS.md` (currently ~688 lines). This file has no ACTION-CONTRACTS block — no validator constraint applies. Target: ~100 lines.

READ the file in full before editing.

REMOVE or REPLACE:

1. Ownership / Secondary Owners section → replace with:
   "Owner: `streamerbot-dev`. Chain to `brand-steward` for follow/sub/watch-streak announcement wording. See `Actions/AGENTS.md` for full ownership rules."

2. Required Reading: replace `Actions/SHARED-CONSTANTS.md` and `Actions/Helpers/AGENTS.md` with a single `Actions/AGENTS.md` entry — the streamerbot-dev role routes directly to local AGENTS.md files without reading the Actions/ parent first, so this pointer is required to pull the shared rules chain. Keep conditional entries: Squad/AGENTS.md (when changing stream-start Squad resets) and LotAT/AGENTS.md (when changing LotAT state resets).

3. Local Workflow: remove steps that restate universal script rules. Keep only these folder-specific rules:
   - `stream-start.cs` is the central stream-session reset point; it must run early in Streamer.bot startup.
   - Any new global variable added by any feature must be added to `stream-start.cs` reset logic and documented in `Actions/SHARED-CONSTANTS.md`.
   - Mix It Up payload compatibility: `Platform = "Twitch"`, empty `Arguments`, lowercase no-space keys in `SpecialIdentifiers`.
   - The five dedicated subscription scripts each map to a distinct Twitch subscription trigger — preserve that mapping.
   - `subscription-gift.cs` is the dual-trigger handler for Gift Subscription and Gift Bomb; preserve suppression of per-recipient events during gift bombs.

4. Large Script Reference section (likely 200–350 lines of per-script variable tables) → replace with a compact summary table with these columns: Script | Trigger category | Key globals reset/used | Purpose.

Before removing the existing Script Reference section, read it carefully and extract the actual global variable names used by each script. Populate the "Key globals reset/used" column from that data — do not leave entries blank or use "—" unless a script genuinely uses no globals. This column is the only information worth preserving from the large tables, so get it right.

One row per script: `stream-start.cs`, `follower-new.cs`, `subscription-new.cs`, `subscription-renewed.cs`, `subscription-prime-paid-upgrade.cs`, `subscription-gift-paid-upgrade.cs`, `subscription-pay-it-forward.cs`, `subscription-gift.cs`, `subscription-counter-rollover.cs`, `watch-streak.cs`.

5. Validation / Boundaries / Handoff sections → replace with:
   "See `Actions/AGENTS.md` for universal validation, boundary, and handoff rules."

KEEP:
- Purpose section (trim to 3–4 lines)
- When to Activate file list
- The folder-specific rules listed in step 3 above
- The compact Script Summary table

After editing: confirm the file is under 120 lines and still answers "what does this folder do and what are the rules."

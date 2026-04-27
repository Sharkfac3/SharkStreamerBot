# Handoff — 102b Actions Helper Snippets Split Execute

Date: 2026-04-27  
Agent: pi

## Summary

Split `Actions/HELPER-SNIPPETS.md` into concept-specific helper references under `Actions/Helpers/`, retained the original helper file as a thin compatibility index, updated targeted cross-references, and registered the new helper docs in `.agents/manifest.json`.

## File creation summary

New files under `Actions/Helpers/`:

| File | Lines | Purpose |
|---|---:|---|
| `Actions/Helpers/README.md` | 36 | Folder index for reusable Streamer.bot C# helpers. |
| `Actions/Helpers/mini-game-lock.md` | 85 | Mini-game lock constants and acquire/release snippets. |
| `Actions/Helpers/mini-game-contract.md` | 23 | Required mini-game contract checklist. |
| `Actions/Helpers/mixitup-command-api.md` | 152 | Mix It Up command API helper and unlock pacing guidance. |
| `Actions/Helpers/chat-input.md` | 65 | Chat message/rawInput, duplicate guard, and sender helpers. |
| `Actions/Helpers/obs-scenes.md` | 54 | OBS scene switching/direct-call guidance. |
| `Actions/Helpers/cph-api-signatures.md` | 80 | Verified CPH API method signatures and unverified-call policy. |
| `Actions/Helpers/timers.md` | 98 | Timer enable/disable/reset/interval-update patterns. |
| `Actions/Helpers/json-no-external-libraries.md` | 586 | No-external-library JSON parser/serializer helper. |

Additional validator-required routing shim:

| File | Lines | Purpose |
|---|---:|---|
| `Actions/Helpers/AGENTS.md` | 22 | Thin local route guide pointing to `README.md`; added because the agent-tree validator requires every first-level `Actions/` folder to have a manifest domain route whose agent doc is named `AGENTS.md`. |

## `HELPER-SNIPPETS.md` line count

| File | Before | After |
|---|---:|---:|
| `Actions/HELPER-SNIPPETS.md` | 1043 | 34 |

The rewritten file is now a compatibility index with a mapping table from each former H2 section to its new helper file.

## Cross-reference updates applied

| File | Update |
|---|---|
| `Actions/Overlay/README.md` | Replaced `Actions/HELPER-SNIPPETS.md § 7` JSON helper reference with `Actions/Helpers/json-no-external-libraries.md`. |
| `Actions/Overlay/AGENTS.md` | Replaced complex JSON serialization references with `Actions/Helpers/json-no-external-libraries.md`. |
| `Actions/Rest Focus Loop/AGENTS.md` | Replaced verified timer/helper-pattern reference with `Actions/Helpers/timers.md` and `Actions/Helpers/cph-api-signatures.md`. |
| `Actions/Temporary/AGENTS.md` | Replaced Mix It Up API helper-pattern reference with `Actions/Helpers/mixitup-command-api.md`. |
| `Actions/Voice Commands/AGENTS.md` | Replaced OBS helper reference with `Actions/Helpers/obs-scenes.md` and linked the local direct-call guidance. |
| `Actions/Twitch Channel Points/AGENTS.md` | Replaced direct OBS/no-reflection helper reference with `Actions/Helpers/obs-scenes.md`. |
| `Actions/Squad/README.md` | Replaced stale “mini-game contract is in root README” statement with `Actions/Helpers/mini-game-contract.md`. |
| `Actions/Squad/AGENTS.md` | Added direct links to `Actions/Helpers/mini-game-lock.md` and `Actions/Helpers/mini-game-contract.md` in required reading and local mini-game guidance. |

Broad references in `.agents/_shared/`, `Docs/`, workflows, role docs, and tool guides were intentionally left pointing at the thin `Actions/HELPER-SNIPPETS.md` compatibility index.

## Comment-only `.cs` edit summary

File: `Actions/Overlay/broker-publish.cs`

Only one comment line changed; no executable code, using directives, method bodies, or signatures changed.

Before:

```csharp
//                 Build this with string concatenation or the SerializeJson
//                 helper from Actions/HELPER-SNIPPETS.md § 7.
```

After:

```csharp
//                 Build this with string concatenation or the SerializeJson
//                 helper from Actions/Helpers/json-no-external-libraries.md.
```

Diff confirmation:

```text
git diff --numstat -- Actions/Overlay/broker-publish.cs
1	1	Actions/Overlay/broker-publish.cs
```

## Manifest entries added

Added 9 `co_locations` entries for the new helper docs:

```json
{
  "path": "Actions/Helpers/README.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/chat-input.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/cph-api-signatures.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/json-no-external-libraries.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/mini-game-contract.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/mini-game-lock.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/mixitup-command-api.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/obs-scenes.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
{
  "path": "Actions/Helpers/timers.md",
  "owner": "streamerbot-dev",
  "status": "active",
  "kind": "shared-reference",
  "route": "streamerbot-dev"
}
```

Validator/schema note: the manifest schema requires `kind` and `route` on `co_locations`, so the entries include those required fields in addition to the requested `path`, `owner`, and `status`.

Additional validator-required manifest entry:

- Added a single `domains` route for `Actions/Helpers/` pointing at `Actions/Helpers/AGENTS.md`. This was necessary because `Tools/AgentTree/validate.py` fails folder coverage for any first-level `Actions/` directory without a manifest domain route. No new `skills` entries were added.

## Final validator output

Command:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/102b-validator.failures.txt
```

Output:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 157 | 0 | PASS |
| link-integrity | 57 | 0 | PASS |
| drift | 2 | 0 | PASS |
| stub-presence | 46 | 0 | PASS |
| orphan | 20 | 0 | PASS |
| naming | 106 | 0 | PASS |

Total failures: 0
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/102b-validator.failures.txt
```

## Orphan content check

All former H2 sections from `Actions/HELPER-SNIPPETS.md` have a destination:

- `## 1) Mini-game Lock Helper (Global, cross-feature)` → `Actions/Helpers/mini-game-lock.md`
- `## 2) Mix It Up Command API Helper` → `Actions/Helpers/mixitup-command-api.md`
- `## 3) Chat Message Input Helper (message/rawInput)` → `Actions/Helpers/chat-input.md`
- `## 4) OBS Scene Switching` → `Actions/Helpers/obs-scenes.md`
- `## 5) Verified CPH API Method Signatures` → `Actions/Helpers/cph-api-signatures.md`
- `## 6) Timer Management` → `Actions/Helpers/timers.md`
- `## 7) JSON Parse / Serialize Helper (No External Libraries)` → `Actions/Helpers/json-no-external-libraries.md`
- `## Required mini-game contract checklist` → `Actions/Helpers/mini-game-contract.md`

No orphan helper-section content was discovered.

## Streamer.bot paste targets

N/A — the only `.cs` edit was a comment-only documentation reference update in `Actions/Overlay/broker-publish.cs`; no Streamer.bot resync is required.

## Open notes for operator

- `Tools/AgentTree/validate.py` currently requires a manifest `domains` route for every first-level `Actions/` folder and requires domain agent docs to be named `AGENTS.md`. This forced the small `Actions/Helpers/AGENTS.md` routing shim and one `domains` entry even though the prompt requested co-location-only manifest changes.
- Existing broader references to `Actions/HELPER-SNIPPETS.md` remain by design and now land on the compatibility index.

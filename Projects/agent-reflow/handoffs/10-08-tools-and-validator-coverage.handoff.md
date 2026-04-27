# Prompt 10-08 handoff — tools-and-validator-coverage

Date: 2026-04-26
Agent: pi

## State changes

- Created local tool agent guides:
  - `Tools/ArtPipeline/AGENTS.md`
  - `Tools/ContentPipeline/AGENTS.md`
  - `Tools/MixItUp/AGENTS.md`
  - `Tools/StreamerBot/AGENTS.md`
  - `Tools/AgentTree/AGENTS.md`
- Updated `.agents/manifest.json` to:
  - mark `tools-mix-it-up` as active/covered now that the local guide exists
  - add explicit `tools-agent-tree` coverage in `skills`, `domains`, and `co_locations`
- Replaced `.agents/_shared/mixitup-api.md` with a compatibility pointer to `Tools/MixItUp/AGENTS.md` after folding the API payload convention into the local Mix It Up guide.
- Wrote validator output to `Projects/agent-reflow/findings/10-08-validator.failures.txt`.
- Wrote this handoff.

## Sources migrated/read

- `.agents/roles/art-director/skills/pipeline/_index.md`
- `.agents/roles/content-repurposer/skills/pipeline/_index.md`
- `.agents/roles/content-repurposer/skills/pipeline/phase-map.md`
- `.agents/_shared/mixitup-api.md`
- `Tools/ArtPipeline/README.md`
- `Tools/ContentPipeline/README.md`
- `Tools/MixItUp/README.md`
- `Tools/StreamerBot/README.md`
- `Tools/README.md`
- Existing `Tools/LotAT/AGENTS.md` was used as the local guide style reference.

## Final disposition for `Tools/AgentTree/`

`Tools/AgentTree/` is now explicitly covered by a new manifest domain route, `tools-agent-tree`, with `ops` as primary owner.

It is **not** declared as covered by `Tools/StreamerBot/AGENTS.md`. The folder owns manifest v2 and agent-tree validation semantics, while `Tools/StreamerBot/` owns Streamer.bot sync/support utilities. Keeping a separate route cleared the Phase 08 baseline gap and keeps future validator work discoverable at the point of use.

## Manifest/schema notes

- `.agents/manifest.json` was updated.
- `.agents/manifest.schema.json` did not require changes; the existing schema already supports the new `tools-agent-tree` route.
- `quick_routing` was not changed because `Tools/AgentTree/` is a specialized ops tool route, not a new high-level role routing row.

## Validator status

Schema validation check run as part of manifest update:

```bash
python3 - <<'PY'
import json, jsonschema
schema=json.load(open('.agents/manifest.schema.json'))
data=json.load(open('.agents/manifest.json'))
jsonschema.Draft202012Validator.check_schema(schema)
errors=sorted(jsonschema.Draft202012Validator(schema).iter_errors(data), key=lambda e:list(e.path))
print('schema ok' if not errors else errors[0].message)
raise SystemExit(1 if errors else 0)
PY
```

Exit code: 0

Output:

```text
schema ok
```

Acceptance validator run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-08-validator.failures.txt
```

Exit code: 1

Output summary:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 152 | 10 | FAIL |
| link-integrity | 120 | 316 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 49 | 21 | FAIL |
| orphan | 104 | 18 | FAIL |
| naming | 108 | 0 | PASS |

Total failures: 368
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-08-validator.failures.txt
```

## Prompt-specific acceptance deltas

Cleared for this prompt:

- `folder-coverage` and `stub-presence` for `tools-art-pipeline`
- `folder-coverage` and `stub-presence` for `tools-content-pipeline`
- `folder-coverage` and `stub-presence` for `tools-mix-it-up`
- `folder-coverage` and `stub-presence` for `tools-streamer-bot`
- `Tools/AgentTree/: first-level domain folder has no manifest domain route`
- `folder-coverage`, `stub-presence`, and naming coverage for the new `tools-agent-tree` route

Remaining `folder-coverage` failures are outside prompt 10-08 scope and are for missing Creative/Docs local guides:

- `Creative/Art/AGENTS.md`
- `Creative/Brand/AGENTS.md`
- `Creative/Marketing/AGENTS.md`
- `Creative/WorldBuilding/AGENTS.md`
- `Docs/Architecture/AGENTS.md`

## Open questions / blockers

- Full validator pass still fails because unrelated baseline work remains for Creative/Docs local guides, role/root frontmatter, generated routing drift, old central skill link normalization, and orphan cleanup.
- `Tools/AgentTree/` has no separate `README.md`; its local `AGENTS.md` now documents validator usage and disposition.
- `.agents/_shared/mixitup-api.md` remains as a compatibility pointer only and should be deleted or fully rerouted in a later cutover prompt if old references are removed.

## Next recommended entry point

- Continue with the next migration prompt after reviewing `Projects/agent-reflow/findings/10-08-validator.failures.txt`.
- Later prompts should address the remaining Creative/Docs folder coverage and frontmatter/drift failures.

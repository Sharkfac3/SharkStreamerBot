# Prompt 07 handoff — manifest-schema-v2

Date: 2026-04-26
Agent: pi

## State changes

- Created `.agents/manifest.schema.json` as the Draft 2020-12 JSON Schema for manifest v2.
- Created `.agents/manifest.json` as the new manifest v2 routing source.
- Wrote `Projects/agent-reflow/findings/07-manifest-v2.md` documenting schema/manifest locations, v1-v2 differences, validation, and migration mapping.
- Left `legacy v1 routing manifest (retired)` in place as the legacy v1 reference for Phase F cutover.
- Updated `WORKING.md` to record completion.

## Findings appended

- `findings/07-manifest-v2.md`: manifest v2 summary, including:
  - final JSON Schema location
  - final manifest v2 location
  - key differences from v1 manifest
  - manifest v2 section purposes
  - validation commands/results
  - migration mapping table for every v1 manifest entry
  - open drift-protection note for prompt 08

## Inputs read

- `.agents/ENTRY.md`
- `WORKING.md`
- `retired Pi skill mirror/meta/SKILL.md`
- `retired Pi skill mirror/meta-agents-navigate/SKILL.md`
- `retired Pi skill mirror/meta-agents-update/SKILL.md`
- `retired Pi skill mirror/ops-change-summary/SKILL.md`
- `.agents/_shared/project.md`
- `.agents/roles/ops/skills/change-summary/_index.md`
- `Projects/agent-reflow/findings/05-target-shape.md`
- `Projects/agent-reflow/findings/06-naming-convention.md`
- `Projects/agent-reflow/findings/02-domain-coverage.md`
- `legacy v1 routing manifest (retired)`

## Manifest v2 contents

- `skills`: 48 entries covering roles, domain routes, workflows, shared references, and transitional Pi meta helpers.
- `workflows`: 5 entries:
  - `canon-guardian`
  - `change-summary`
  - `sync`
  - `validation`
  - `coordination` — planned
- `domains`: 27 entries covering first-level domain folders under `Actions/`, `Apps/`, `Tools/`, `Creative/`, plus `Docs/Architecture/`.
- `co_locations`: declares root docs, role overview docs, workflow docs, local domain `AGENTS.md` paths, and transitional Pi wrappers.
- `aliases`: maps legacy v1/Pi names directly to final role/domain/workflow IDs.
- `migration.v1_entries`: accounts for every v1 role, canonical subskill, helper, quick-routing row, and alias.

## Decisions / implementation notes

- Used `.agents/manifest.json` rather than overwriting `legacy v1 routing manifest (retired)` so v2 can land without deleting the old v1 reference.
- Marked some local `agentDoc` and workflow paths as `planned` because prompt 07 is schema/manifest creation only; later prompts still need to create/move those docs.
- Mapped broad `streamerbot-dev-twitch` compatibility to `actions-twitch-core-integrations` while specific Twitch aliases/routes point to their specific folders.
- Kept `meta`, `meta-agents-navigate`, and `meta-agents-update` as transitional `meta` skills/compatibility co-locations until Pi mirror cutover.
- No `.pi/`, `.agents/roles/`, or domain-folder content was intentionally edited for this prompt.

## Validator status

Last run:

```bash
python3 - <<'PY'
import json, jsonschema, collections
schema=json.load(open('.agents/manifest.schema.json'))
data=json.load(open('.agents/manifest.json'))
jsonschema.Draft202012Validator.check_schema(schema)
errors=sorted(jsonschema.Draft202012Validator(schema).iter_errors(data), key=lambda e:e.path)
print('schema ok' if not errors else errors[0].message)
for name in ['skills','workflows','domains','aliases']:
 c=collections.Counter(x['id'] for x in data[name])
 d=[k for k,v in c.items() if v>1]
 print(f'{name} duplicate ids:', d)
v1=json.load(open('legacy v1 routing manifest (retired)'))
mig={(e['v1_type'],e['v1_id']) for e in data['migration']['v1_entries']}
missing=[]
for typ,key,mt in [('roles','name','role'),('canonical_subskills','name','canonical_subskill'),('helpers','name','helper'),('quick_routing','work','quick_routing'),('aliases','name','alias')]:
 for x in v1[typ]:
  if (mt,x[key]) not in mig: missing.append((mt,x[key]))
print('v1 migration missing:', missing)
if errors or any(collections.Counter(x['id'] for x in data[name]).most_common(1)[0][1]>1 for name in ['skills','workflows','domains','aliases']) or missing:
 raise SystemExit(1)
PY
```

Exit code: 0

Key output:

```text
schema ok
skills duplicate ids: []
workflows duplicate ids: []
domains duplicate ids: []
aliases duplicate ids: []
v1 migration missing: []
```

## Open questions / blockers

- Until prompt 08 ships the validator tool, manifest v2 has no automated drift protection beyond ad hoc schema validation.
- Several `agentDoc` and workflow paths are declared but not created yet; later migration prompts should create/move those docs.
- `legacy v1 routing manifest (retired)` remains as legacy reference until Phase F cutover decides whether to delete, replace, or generate it from v2.

## Next prompt entry point

Proceed per `Projects/agent-reflow/prompts/08-validator.md`.

Use these inputs:

1. `.agents/manifest.schema.json`
2. `.agents/manifest.json`
3. `Projects/agent-reflow/findings/07-manifest-v2.md`
4. `Projects/agent-reflow/handoffs/07-manifest-schema-v2.handoff.md`
5. `legacy v1 routing manifest (retired)` as the legacy v1 reference

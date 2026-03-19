---
name: lotat-tech
description: LotAT technical pipeline — JSON story schema, engine architecture, C# engine implementation. Load when working on the story pipeline or engine layer.
---

# lotat-tech

Full context: `.agents/roles/lotat-tech/role.md`

## Always Load

`.agents/roles/lotat-tech/skills/core.md`

## Then Navigate

| Task | Load |
|---|---|
| Story pipeline, schema validation | `.agents/roles/lotat-tech/skills/story-pipeline/_index.md` |
| JSON schema field reference | `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md` |
| C# engine architecture | `.agents/roles/lotat-tech/skills/engine/_index.md` |
| Supported commands | `.agents/roles/lotat-tech/skills/engine/commands.md` |

## Terminal

After any code change: load `ops/change-summary/SKILL.md`

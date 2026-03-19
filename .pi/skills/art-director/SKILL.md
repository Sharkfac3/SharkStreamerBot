---
name: art-director
description: Diffusion model prompts, character art, stream visuals. Load when generating any art asset for the stream.
---

# art-director

Full context: `.agents/roles/art-director/role.md`

## Always Load

`.agents/roles/art-director/skills/core.md`

## Then Navigate

| Task | Load |
|---|---|
| Any character art | `.agents/roles/art-director/skills/characters/_index.md` + relevant character file |
| Captain Stretch art | `.agents/roles/art-director/skills/characters/captain-stretch.md` |
| The Director art | `.agents/roles/art-director/skills/characters/the-director.md` |
| Water Wizard art | `.agents/roles/art-director/skills/characters/water-wizard.md` |
| Non-character stream assets | `.agents/roles/art-director/skills/stream-style/_index.md` |

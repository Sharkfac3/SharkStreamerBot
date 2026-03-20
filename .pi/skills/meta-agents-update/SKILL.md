---
name: meta-agents-update
description: How to add or update content in the .agents/ project knowledge tree. Load when creating a new role, adding a skill file, or leaving context notes for future agents.
---

# Updating .agents/

## What Lives Where

| What you want to add | Where it goes |
|---|---|
| New role | `roles/<new-role>/` (copy from `roles/_template/`) |
| New skill file for an existing role | `roles/<role>/skills/<subfolder>/<topic>.md` |
| Navigation header for a new subfolder | `roles/<role>/skills/<subfolder>/_index.md` |
| Living context note (discovered pattern, gotcha) | `roles/<role>/context/<topic>.md` |
| Cross-role knowledge (affects all agents) | `_shared/<topic>.md` + update `ENTRY.md` if needed |

## Adding a New Role

1. Copy `roles/_template/` to `roles/<new-role>/`
2. Edit `roles/<new-role>/role.md` — fill in all sections
3. Edit `roles/<new-role>/skills/core.md` — base knowledge for this role
4. Add sub-skill folders as needed
5. Update `ENTRY.md` Roles table
6. Add the new role to `.pi/skills/README.md` routing table
7. Create `.pi/skills/<new-role>/SKILL.md` — thin wrapper pointing to `.agents/roles/<new-role>/`

## Adding a Skill File

1. Create the file at the appropriate path
2. If it's the first file in a new subfolder, also create `_index.md` for that subfolder
3. Update the parent `_index.md` or `role.md` skill load order if the new file should be explicitly referenced

## Updating Pi Wrappers

When `.agents/` gains a new Pi-facing role or sub-skill:

- Keep the detailed knowledge in `.agents/`
- Add or update the thin wrapper in `.pi/skills/`
- Use flat Pi-safe names only
- Role wrappers use `<role>`
- Canonical sub-skill wrappers use `<role>-<subskill>`
- Update `.agents/routing-manifest.json` first — it is the primary routing contract
- Re-run `python3 Tools/StreamerBot/Validation/sync-routing-docs.py` after routing-contract changes
- Keep `AGENTS.md` and `.pi/skills/README.md`'s generated tables aligned with `.agents/routing-manifest.json`
- Avoid duplicate route rows, wrapper-name collisions, or alias-to-alias targets — validator now rejects them
- If an older flat wrapper name is already in circulation, preserve it as a migrated compatibility alias when helpful

Never mirror `.agents/` sub-skill hierarchy literally in `.pi/skills/` with slash-style names.

## Writing Context Notes

Context notes in `roles/<role>/context/` are living knowledge. Agents write here; other agents read here.

Format:
```markdown
## <Topic Title>

<One paragraph: what you discovered, when it matters, what to do about it>
```

Rules:
- Additive only — do not delete other agents' notes; append new ones
- Date and agent name optional but helpful for traceability
- Keep notes focused — one insight per entry

## Updating Existing Skills

- Edit the file directly — no versioning needed (git handles history)
- If a skill's scope changes significantly, update the `description` frontmatter in the corresponding `.pi/skills/` SKILL.md
- If a role's trigger conditions change, update `role.md` and the `ENTRY.md` Roles table

## Do Not

- Do not add runtime code (C#, Python) to `.agents/` — that belongs in `Actions/` or `Tools/`
- Do not add story content to `.agents/` — that belongs in `Creative/WorldBuilding/`
- Do not duplicate content that already exists in `Actions/SHARED-CONSTANTS.md` or `Actions/HELPER-SNIPPETS.md` — reference them instead

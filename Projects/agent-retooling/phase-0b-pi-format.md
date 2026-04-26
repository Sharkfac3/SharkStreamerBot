# Phase 0b — Reverse-engineer Pi skill format

## Before you start

1. Read `Projects/agent-retooling/PLAN.md`.
2. Read `Projects/agent-retooling/STATUS.md` and confirm `phase-0b-pi-format.md` is not already checked off.

## Mission

The new tree lives at `.agents/skills/<skill>/SKILL.md` and must be discoverable by both Pi coding agent and Claude. Confirm Pi's skill format and discovery rules. Output `Projects/agent-retooling/pi-skill-format.md`.

## Sources

- https://github.com/badlogic/pi-mono/tree/main/packages/coding-agent#skills (primary doc)
- https://github.com/badlogic/pi-mono/tree/main/packages/coding-agent (source if doc is thin)
- Existing `.pi/skills/*/SKILL.md` files in this repo — sample at least 3 different ones.

Use WebFetch for the GitHub URLs. Use Read for local files.

## Required findings

Document:

1. **Frontmatter** — required fields, expected shape (e.g. `name`, `description`).
2. **Filename / directory** — must be `SKILL.md` inside `<skill-name>/`? Other shapes accepted?
3. **Discovery order** — which directories Pi searches, in what order. List them all.
4. **`.agents/skills/` discovery** — confirm Pi searches `.agents/skills/`. If it does, no `.pi/` mirror is needed.
5. **Invocation syntax** — `/skill:name`? Anything else?
6. **Move impact** — what breaks if we move all skills from `.pi/skills/` to `.agents/skills/` and delete `.pi/` entirely?

## Output format — `pi-skill-format.md`

```markdown
# Pi Coding Agent — Skill Format Reference

Reverse-engineered from Pi docs and existing `.pi/skills/` files. Used by Phase 3 (new tree design) to confirm `.agents/skills/<skill>/SKILL.md` is correct shape.

## Frontmatter

(YAML block, required fields, descriptions)

## File and directory shape

(naming rules)

## Discovery order

1. ...
2. ...

## Invocation

...

## Verdict — can `.pi/` be deleted?

**Yes / No** — and brief reason.

## Sample existing skill files reviewed

- `.pi/skills/<x>/SKILL.md`
- `.pi/skills/<y>/SKILL.md`
- `.pi/skills/<z>/SKILL.md`

## Open questions

- (anything not resolvable from sources)
```

## When done

1. Append to `STATUS.md` under `## Notes`: `- 0b complete: <yes/no> verdict on .pi/ deletion.`
2. Tick `[x] phase-0b-pi-format.md` in STATUS.md.
3. Phase 0b is independent of 0a, 0c. Phase 0d cannot start until 0a and 0b are both checked off.

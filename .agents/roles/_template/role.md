# Role: <role-name>

## What This Role Does

<One paragraph describing the job this role performs in the project.>

## Activate When

- <trigger condition>
- <trigger condition>

## Do Not Activate When

- <anti-trigger — tasks that belong to a different role>

## Skill Load Order

1. `skills/core.md` — always load first
2. `skills/<subfolder>/_index.md` — load when task involves that feature area
3. `skills/<subfolder>/<specific>.md` — load only when task is specifically about that topic

Use `skills/example-skill/_index.md` as the pattern when creating the first real sub-skill folder for a new role.

## Chains To

| Next Role | When |
|---|---|
| `ops` | After any code change — load `ops/skills/change-summary/_index.md` |
| `<other-role>` | <when to chain> |

## Out of Scope

- <things this role does not do>

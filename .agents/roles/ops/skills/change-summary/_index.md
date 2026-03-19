# Change Summary — Terminal Skill

## When to Use

After completing **any code change** in this repo. This is the last step.

## Required Sections

1. **Changed files** — clear paths to every modified file
2. **Behavioral summary** — what changed, in operator-friendly language
3. **Streamer.bot paste targets** — where each file goes, or `N/A` for `Tools/`/`Creative/`-only changes
4. **Manual setup steps** — any Streamer.bot UI changes needed (variables, trigger wiring, action ordering); omit if none
5. **Validation checklist** — which checks were run/confirmed

## Template

```
### Changed files
- `<path>`

### Behavioral summary
<what changed, operator-friendly>

### Streamer.bot paste targets
| File | Action/Group |
|---|---|
| `<path>` | <Group > Action> |

### Manual setup steps
<steps or "None">

### Validation checklist
- [ ] Syntax check
- [ ] Happy path
- [ ] Edge case
```

## For Tools/ Changes

Provide **run instructions** (command + expected output file path) instead of paste targets. Validation checklist: see `ops/skills/sync/_index.md`.

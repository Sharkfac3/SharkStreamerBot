---
name: change-summary
description: Standard response format for code changes. Produces paste targets, manual setup steps, and validation checklists. Load after completing any code change to format the summary.
---

# Change Summary Format

## When to Use

After completing any code change in this repo, format the response using this template.

## Required Sections

1. **Changed files** — Clear paths to every modified file.
2. **Behavioral summary** — What changed, in operator-friendly language.
3. **Streamer.bot paste targets** — Where each file should be pasted, or `N/A` for `Tools/` / `Creative/` only changes.
4. **Manual setup steps** — Any Streamer.bot UI changes needed (variables, trigger wiring, action ordering). Omit if none.
5. **Validation checklist executed** — Which checks were run/confirmed.

## Change Control Rules

- Prefer small, targeted edits.
- Preserve existing external behavior unless requested.
- Do not rename files/actions casually when operators rely on them.
- Highlight any breaking change **before** implementation.
- If requirements are ambiguous for live behavior, ask before proceeding.

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

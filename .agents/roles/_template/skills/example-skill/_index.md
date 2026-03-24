# Example Skill Area — Overview

<!--
Rename this folder and heading to the real feature area name.
This file is the navigation entry point for a sub-skill folder.
Use it to explain what the area covers, when an agent should load deeper docs,
and which files belong to the area.
-->

## Purpose

<!--
Briefly describe the job this skill area performs inside the role.
Answer: what kind of tasks belong here, and why this sub-skill exists separately
from core.md.
-->

<Describe the feature area, responsibility, or problem space this sub-skill covers.>

## When to Load

<!--
List trigger conditions that tell an agent to load this folder's docs.
These should be concrete task cues, not broad role descriptions.
-->

- <Load when the task involves this feature area>
- <Load when editing files in these paths>
- <Load when reviewing behavior, docs, or decisions tied to this area>

## Files in This Skill Area

<!--
List the files inside this subfolder and what each one is for.
Include _index.md itself if helpful, then add deeper topic files below it.
-->

| File | Purpose |
|---|---|
| `_index.md` | Navigation overview for this skill area |
| `<topic>.md` | Detailed rules, implementation notes, or workflow for one topic |
| `<another-topic>.md` | Another focused document inside this skill area |

## Related Project Files

<!--
Point to source files, docs, schemas, or folders in the main repo that this
sub-skill commonly touches.
-->

| Path | Why it matters |
|---|---|
| `<path>` | Primary implementation or reference file |
| `<path>` | Secondary dependency, shared constants, or supporting docs |

## Cross-References

<!--
Link to adjacent skills or roles when tasks often chain across boundaries.
Use this section to tell future agents what to load next.
-->

- `../core.md` — always load before using this sub-skill
- `<sibling-skill>/_index.md` — load when the task overlaps that feature area
- `../../<other-role>/role.md` — switch roles if the task actually belongs elsewhere

## Notes for Future Authors

<!--
Capture structural expectations:
- Keep this file high-level and navigational.
- Put detailed procedures in separate topic files.
- Update role.md if this becomes a commonly referenced sub-skill.
- If Pi needs to route directly to this area, add/update the matching flat wrapper
  in .pi/skills/ and the routing manifest.
-->

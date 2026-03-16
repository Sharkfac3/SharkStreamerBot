---
name: creative-art
description: Art generation agents for diffusion model prompting — character canon references, prompt conventions, asset naming, and output placement for stream visuals.
---

# Creative Art

## When to Load

Load this skill for any task involving:
- Character visualization or concept art
- Diffusion model prompt generation (overlays, thumbnails, emotes, panels, stream assets)
- Multi-character scene composition
- Extending or updating a character's visual canon

## Agent Loading Order

**Always load agents in this order — no exceptions:**

1. `Creative/Art/Agents/StreamStyle-art-agent.md` — shared style foundation (rendering, line art, shading, readability rules)
2. The relevant character agent(s) for the task

The style agent defines *how everything looks*. Character agents define *who the character is*. Character agents do not override the style agent — they layer on top of it.

## Character Agent Files

| Character | Agent File |
|---|---|
| Captain Stretch | `Creative/Art/Agents/CaptainStretch-art-Agent.md` |
| The Director | `Creative/Art/Agents/TheDirector-art-Agent.md` |
| Water Wizard | `Creative/Art/Agents/WaterWizard-art-agent.md` |

## Prompt Structure

Each generated prompt output should include these labeled sections:

```
**Character:** <name>
**Style Agent:** Creative/Art/Agents/StreamStyle-art-agent.md
**Character Agent:** <agent file path>
**Model:** <model used or "unspecified">
**Asset Type:** <overlay | emote | thumbnail | banner | character sheet | other>
**Positive Prompt:** <full prompt text — include "anime style, cel shading, clean linework" as base>
**Negative Prompt:** <always start with: "photorealistic, 3d render, western cartoon, gritty, painterly, blurry, low contrast, missing eye highlights" — then add asset-specific exclusions>
**Notes:** <any operator flags, deviations, or iteration notes>
```

The **Style Agent** field is mandatory in every output — it makes the style dependency explicit and reviewable.

## Multi-Character Scenes

- Load StreamStyle first, then each character's agent file.
- List all characters in the **Character** field, comma-separated.
- All characters in a scene must follow the same StreamStyle rules — this is what makes them look like they belong together.
- Flag any canonical conflicts (e.g., scale mismatch between species) in **Notes**.

## Asset Naming Convention

```
CharacterName-AssetType-VersionNote
```

Examples:
- `CaptainStretch-overlay-transparent-v1`
- `TheDirector-emote-wave-anime`
- `WaterWizard-thumbnail-background-v2`
- `Squad-banner-all-v1`

Use lowercase-hyphen format. `VersionNote` is optional but recommended when iterating.

## Output Placement

| Content | Directory |
|---|---|
| Finished prompts (ready to run) | `Creative/Art/Projects/` |
| Generated image assets | `Creative/Art/Assets/` |
| Reference materials (mood boards, pose refs) | `Creative/Art/References/` |
| Character canon files | `Creative/Art/Agents/` (do not overwrite — append or create new) |

## Model-Agnostic Policy

This skill does not prescribe a specific diffusion model. Always note the target model in the **Model** field of the prompt output. Prompt syntax (e.g., weighting, negative prompt format) should match the stated model's conventions.

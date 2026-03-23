# Core Skills — art-director

## Business Context

Art assets support a live R&D stream about building off-road racing products. Visuals you create serve both the live stream experience AND short-form content across YouTube, TikTok, and Instagram. Design with both contexts in mind: immersive at stream resolution, recognizable at thumbnail size. The brand's visual identity is a key part of the content pipeline. Read `.agents/_shared/project.md` for the full business context and content pipeline.

## Agent Loading Order

**Always load agents in this order — no exceptions:**

1. `Creative/Art/Agents/StreamStyle-art-agent.md` — shared style foundation (rendering, line art, shading, readability rules)
2. The relevant character agent(s) for the task

The style agent defines *how everything looks*. Character agents define *who the character is*. Character agents do not override the style agent — they layer on top of it.

## Prompt Structure

Every generated prompt output must include these labeled sections:

```
**Character:** <name or "N/A">
**Style Agent:** Creative/Art/Agents/StreamStyle-art-agent.md
**Character Agent:** <agent file path or "N/A">
**Model:** <model used or "unspecified">
**Asset Type:** <overlay | emote | thumbnail | banner | character sheet | other>
**Positive Prompt:** <full prompt — include "anime style, cel shading, clean linework" as base>
**Negative Prompt:** <always start with: "photorealistic, 3d render, western cartoon, gritty, painterly, blurry, low contrast, missing eye highlights" — then add asset-specific exclusions>
**Notes:** <operator flags, deviations, iteration notes>
```

The **Style Agent** field is mandatory in every output — it makes the style dependency explicit and reviewable.

## Multi-Character Scenes

- Load StreamStyle first, then each character's agent file
- List all characters in the **Character** field, comma-separated
- All characters in a scene must follow the same StreamStyle rules
- Flag canonical conflicts (e.g., scale mismatch between species) in **Notes**

## Asset Naming Convention

```
CharacterName-AssetType-VersionNote
```

Examples:
- `CaptainStretch-overlay-transparent-v1`
- `TheDirector-emote-wave-anime`
- `WaterWizard-thumbnail-background-v2`
- `Squad-banner-all-v1`

Lowercase-hyphen format. `VersionNote` is optional but recommended when iterating.

## Output Placement

| Content | Directory |
|---|---|
| Finished prompts (ready to run) | `Creative/Art/Projects/` |
| Generated image assets | `Creative/Art/Assets/` |
| Reference materials (mood boards, pose refs) | `Creative/Art/References/` |
| Character canon files | `Creative/Art/Agents/` (do not overwrite — append or create new) |

## Model-Agnostic Policy

This skill does not prescribe a specific diffusion model. Always note the target model in the **Model** field. Prompt syntax (weighting, negative prompt format) should match the stated model's conventions.

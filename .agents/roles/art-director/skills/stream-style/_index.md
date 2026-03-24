# Stream Style — Overview

## The Style Agent

`Creative/Art/Agents/stream-style-art-agent.md` is the foundational style document for all stream visuals. It defines:
- Rendering style (anime, cel shading, clean linework)
- Line art and shading conventions
- Readability rules for stream assets (contrast, silhouette, legibility at small sizes)
- Color palette guidance

**Always load this file before generating any stream asset, regardless of whether characters are involved.**

## Asset Types

| Type | Notes |
|---|---|
| Overlay | Transparent background; must read clearly over varied stream content |
| Emote | Very small at display size — silhouette and expression must read at 28px |
| Thumbnail | High contrast; must attract attention in a feed |
| Banner | Wide format; composition needs to work at different crop points |
| Character sheet | Full reference; multiple poses/expressions acceptable |
| Panel | Static overlay element; must match existing panel style |

## When No Character Is Involved

For environmental assets, backgrounds, UI elements, or abstract stream graphics:
- Load StreamStyle agent only
- Describe the asset type and purpose clearly
- Follow the same positive/negative prompt structure from `art-director/skills/core.md`

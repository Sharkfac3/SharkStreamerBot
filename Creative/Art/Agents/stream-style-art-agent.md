# Starship Shamples — Shared Stream Art Style Agent

This document defines the **canonical visual style** for all Starship Shamples stream assets.

**Load this file before any character agent.** Character agents define *who* a character is. This document defines *how everything looks*.

---

## Core Style

All stream assets use an **anime illustration** style.

Key qualities:
- Clean, expressive anime linework
- Bold character silhouettes that read instantly
- Large, expressive eyes with emotional range
- Simplified but vivid facial features
- Characters should feel like they belong in the same anime series

---

## Line Art

- **Present in all renders** — no lineless style
- Line weight: **thick outer contour, thinner internal lines** (standard anime inking)
- Lines may taper and vary in weight for expressiveness
- Line color: black or a deep dark version of the character's primary color

---

## Shading

- **Anime cel shading** — hard shadow shapes, clean edges, minimal gradients
- 1–2 shadow tones per color region; no painterly blending
- Specular highlight on eyes is required — this is a core anime readability signal
- Simple rim light or highlight on head and shoulders
- Avoid: soft airbrush shading, painted textures, complex gradient maps

---

## Eyes

Eyes are the emotional anchor of anime style and must be treated carefully:

- Large and expressive relative to the face
- Include a specular highlight (white dot or shape)
- Iris should have depth — at minimum a base color plus a darker upper tone
- Character-specific eye shapes and pupil designs are defined in each character agent and take priority over these defaults

---

## Color Philosophy

- Saturated, clean anime colors — avoid muted, desaturated, or earthy palettes
- Each character retains their own personal color palette (defined in their agent file)
- No character should share their primary palette with another unless narratively intentional
- Glowing effects (magic, technology, special eyes) use bright high-contrast treatment with a soft bloom glow

---

## Emote and Phone Readability

Assets are frequently used as emotes or viewed on mobile. Design for this from the start:

- **Emotes**: face fills most of the frame; expression must read at 28×28px — test mentally at thumbnail scale
- **Phone overlays**: bold outlines, high contrast between character and background, no fine detail that disappears below 300px
- **Silhouette test**: the character's silhouette alone must be identifiable — no two characters should have the same outline shape
- Avoid small accessories or text that become noise at small sizes
- When in doubt: bigger eyes, thicker lines, higher contrast

---

## Backgrounds

| Asset Type | Background |
|---|---|
| Emotes | Transparent (PNG) |
| Overlays / panels | Transparent (PNG) |
| Thumbnails / banners | Scene or stylized anime gradient background |
| Character sheets | Clean white or soft neutral — no distracting background |

---

## What to Avoid

- Photorealism or 3D rendering style
- Western cartoon or comic book style (flat shading, thick uniform lines without taper)
- Gritty, painted, or textured realism
- Horror or unsettling styling
- Hyper-detailed skin or material texture
- Excessive visual noise (too many elements competing for attention)
- Desaturated, washed-out, or muddy palettes
- Missing eye highlights — eyes without a specular dot look lifeless

---

## Goal

Every asset should feel like it belongs in the same anime series — consistent studio style, consistent rendering language, consistent emotional energy.

Captain Stretch, The Director, and The Water Wizard side by side should look like they share a world.

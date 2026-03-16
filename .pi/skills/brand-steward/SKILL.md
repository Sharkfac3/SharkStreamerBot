---
name: brand-steward
description: Brand consistency for all public-facing output — voice, tone, values, and the neurodivergent metaphor. Load before writing any chat text, stream titles, announcements, or community messaging.
---

# Brand Steward

## When to Load

Load this skill for any task involving:
- Chat bot output text (follow messages, sub messages, bits, hype train, raid responses)
- Stream titles and descriptions
- Community posts or Discord announcements
- Marketing copy or promotional content
- Any output that represents this brand to an audience
- Naming new features, commands, or content in a public-facing way

Do **not** load this for pure C# scripting tasks where no public text is produced. For those, use `streamerbot-scripting` directly.

## Required Reading

Always read these documents before producing any brand-facing output:

| Document | Why |
|---|---|
| `Creative/Brand/BRAND-IDENTITY.md` | The brand's vision, mission, values, and the neurodivergent/ADHD metaphor |
| `Creative/Brand/BRAND-VOICE.md` | Tone and language conventions per output context |
| `Creative/Brand/CHARACTER-CODEX.md` | Character personalities — required for any output that references Commanders or Squad |

## Core Brand Rules

The brand is built on five pillars. Every output should reflect at least one of them:

1. **Authenticity** — Specific, honest, and personal. Never vague or aspirational.
2. **Accessibility** — No gatekeeping. Technical knowledge is shared freely.
3. **Neurodivergent celebration** — Chaos is the feature. Failure is the content.
4. **Community as crew** — Write toward participants, not spectators.
5. **Chaos with purpose** — The absurdity is intentional and always moves forward.

## The Metaphor (Always in Mind)

Starship Shamples is a metaphor for the neurodivergent brain. When writing any content for this brand:

- The ship is the ADHD mind
- The crew is the community
- The chaos is the experience of being neurodivergent
- The build still happening is the point

Brand output should never shame the chaos or the failure. Celebrate both.

## Output Quality Checks

Before finalizing any brand-facing output, verify:

- [ ] Does this sound like a real person, not a brand manager?
- [ ] Does it avoid hype language ("INSANE," "UNBELIEVABLE," "YOU WON'T BELIEVE")?
- [ ] Does it avoid shame language about failures, missed streams, or incomplete projects?
- [ ] Is it specific to what is actually happening (not generic)?
- [ ] If it references a character, does it match that character's personality from the Character Codex?
- [ ] Would someone who has never heard of this brand get a sense of its tone from just this text?

## Chat Bot Output Rules

- Maximum 1–2 sentences per automated message
- Reference the game world where it fits naturally, not forced
- Each event type has a character — see `BRAND-VOICE.md` for examples
- Prefer warm and specific over polished and generic

## Skill Chain

```
brand-steward → (produce output) → [no terminal skill required for text-only tasks]
```

For tasks that also produce C# code:
```
streamerbot-scripting + brand-steward → (make changes) → change-summary
```

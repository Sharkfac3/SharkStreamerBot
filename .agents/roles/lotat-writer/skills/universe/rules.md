# Universe Rules

## The Ship

Starship Shamples is the setting for all LotAT adventures. The ship sections are the physical spaces where scenes take place.

## Ship Sections

Ship sections are referenced in each story node's `ship_section` field, and the story also tracks them at the top level in `ship_sections_used`. Use established sections — do not invent new ones without operator approval.

For the current list of ship sections, see: `Creative/WorldBuilding/Agents/D&D-Agent.md`

## Space Regions

Space regions provide the backdrop and tone for missions. Established regions:
- **Screaming Asteroid Belt** — chaotic, mechanical, high Pedro energy
- **Infinite Space Mall** — sourcing parts, strange discoveries
- (additional regions in `Creative/WorldBuilding/Settings/`)

New single-gimmick space regions are approvable without operator escalation. New permanent locations require operator approval.

## Chaos Meter

- Starts low at the beginning of every story
- Escalates over the arc — never resets mid-story
- Ending nodes must reflect the final chaos level in tone
- Per-node chaos is tracked with the contract's `chaos` object using a single non-negative `delta` field — not a flat `chaos_change` field
- If you are unsure how to represent a chaos adjustment, check `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`

## The Dice System

- Dice hooks add tension to specific moments — use sparingly
- Defined in `Creative/WorldBuilding/Agents/D&D-Agent.md`
- In v1, a dice hook is a pre-vote **narrative-only** mechanic; it does not change branching, chaos, vote eligibility, or vote resolution
- Use only the supported `dice_hook` structure from the authoritative story contract; do not improvise alternate roll fields

## Tone Constraints

- Absurd, chaotic, slightly dramatic, fast-paced, humorous, failure-forward
- Failure is always valid content — do not write toward guaranteed success
- "We made it worse trying to fix it" is the most relatable and encouraged outcome
- Do not introduce darkness or seriousness that does not serve the humor

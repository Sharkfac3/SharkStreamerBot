# Adventure Mechanics

## Chaos Meter

- Represents escalating ship crisis level
- Starts low, ends high — this is non-negotiable
- `chaos_change` field on each node adjusts the meter
- Do not reset mid-story
- Final nodes (endings) must match the chaos level in tone — high chaos = dramatic/spectacular outcome, low chaos = calmer resolution

## Dice Hooks

- Optional mechanic for individual nodes
- Adds audience tension — a roll determines which path executes
- Use sparingly (1–3 per story max) — not a default mechanic
- Defined format: see `Creative/WorldBuilding/Agents/D&D-Agent.md` for dice hook field spec

## Commander Moments

- Optional — 1–2 per story maximum
- Must be personality-specific to the commander featured
- Not generic heroics — a Captain Stretch moment feels different from a Water Wizard moment
- Use `commander_moment` field in the node
- Only include if it serves the story; they are rare and impactful

## Choices

- Every stage node must have **exactly 2 choices**
- Choice labels should be action-oriented and thematically appropriate
- Choices should lead to meaningfully different outcomes — not just cosmetically different narration
- At least one path in each branch should lead toward increased chaos

## Endings

- At least 2 distinct endings per story
- Endings should reflect the chaos level at the time of arrival
- At least one ending should be "we made it worse" (most relatable, best content)
- Success endings are valid but should feel earned — partial success is often more interesting than total victory

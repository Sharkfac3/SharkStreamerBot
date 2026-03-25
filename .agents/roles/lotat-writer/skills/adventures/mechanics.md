# Adventure Mechanics

## Chaos Meter

- Represents escalating ship crisis level
- Starts low, ends high — this is non-negotiable
- Use the node-level `chaos` object from the contract: `on_enter`, `on_success`, `on_failure`
- Do not replace that object with a flat `chaos_change` field or any other alias
- Do not reset mid-story
- Ending nodes must match the final chaos level in tone — high chaos = dramatic/spectacular outcome, low chaos = calmer resolution

## Dice Hooks

- Optional mechanic for individual nodes
- Adds audience tension — a roll influences how the moment resolves
- Use sparingly (1–3 per story max) — not a default mechanic
- Use the supported `dice_hook` structure from the authoritative contract
- For field-level details, check `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`

## Commander Moments

- Optional — 1–2 per story maximum
- Must be personality-specific to the commander featured
- Not generic heroics — a Captain Stretch moment feels different from a Water Wizard moment
- Use the contract's `commander_moment` object; do not invent alternate commander trigger fields
- Only include if it serves the story; they are rare and impactful

## Choices

- Every **stage** node must have **exactly 2 choices**
- Choice labels should be action-oriented and thematically appropriate
- Choices should lead to meaningfully different outcomes — not just cosmetically different narration
- At least one path in each branch should lead toward increased chaos
- Each choice uses the contract shape, including a supported authored decision `command` value
- Runtime participation commands like `!join` are not story choices and never belong in `choices`
- Ending nodes do not get faux choices; they use `choices: []`

## Cast Usage

- Record featured characters in the top-level `cast` object, using `commanders_used` and `squad_members_used`
- Use `crew_focus` at the node level when a specific commander or squad member is the scene focus
- Do not turn cast tracking into a freeform list, custom tag, or invented metadata field

## Endings

- At least 2 distinct endings per story
- Endings should reflect the chaos level at the time of arrival
- At least one ending should be "we made it worse" (most relatable, best content)
- Success endings are valid but should feel earned — partial success is often more interesting than total victory

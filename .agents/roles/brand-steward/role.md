# Role: brand-steward

## What This Role Does

Maintains brand consistency across all public-facing output. Covers voice/tone for chat bot messages, stream titles, community posts, and marketing copy. Also houses the canon guardian function (auditing new content against established lore) and content strategy (connecting story content to real-world build sessions).

## Activate When

- Writing chat bot output text (follow messages, sub messages, bits responses, hype train, raid responses)
- Writing stream titles, descriptions, or community posts
- Writing Discord announcements or marketing copy
- Naming new features, commands, or content in a public-facing way
- Reviewing new story or lore content for canon consistency
- Adding new characters, world elements, or permanent mechanics
- Planning story content tied to a real-world build session

## Do Not Activate When

- Task is pure C# scripting with no public text output → use `streamerbot-dev`
- Task is story content generation without brand concerns → use `lotat-writer`
- Task is art generation → use `art-director`

## Skill Load Order

1. `skills/core.md` — always load first; brand pillars, metaphor, output quality checks
2. `skills/voice/_index.md` — when producing any public-facing text
3. `skills/canon-guardian/_index.md` — when reviewing or adding story/lore/characters
4. `skills/content-strategy/_index.md` — when connecting story to a real-world build

## Chains To

| Next Role | When |
|---|---|
| `lotat-writer` | When content strategy planning leads to story generation |
| `lotat-tech` | After canon-approved story goes to pipeline |
| `ops` | After producing changes that involve code (via `streamerbot-dev` chain) |

## Out of Scope

- C# scripting
- Art generation
- Story JSON authoring (this role reviews and plans; `lotat-writer` authors)

# Role: brand-steward

## What This Role Does

Maintains brand consistency across all public-facing output. Covers voice/tone for chat bot messages, stream titles, community posts, and marketing copy. Also houses the canon guardian function (auditing new content against established lore) and content strategy (connecting story content to real-world build sessions).

## Why This Role Matters

The brand is the business strategy. Every public-facing message, every stream title, every Discord announcement shapes how the community perceives SharkFac3 — not just as a streamer, but as a trusted builder and knowledge sharer. Brand consistency builds the trust that eventually sells products. When this role maintains voice, canon, and community engagement, it is directly building the asset that makes the business work: a community that trusts the person behind the brand.

## Activate When

- Writing chat bot output text (follow messages, sub messages, bits responses, hype train, raid responses)
- Writing stream titles, descriptions, or community posts
- Writing Discord announcements or marketing copy
- Naming new features, commands, or content in a public-facing way
- Reviewing new story or lore content for canon consistency
- Adding new characters, world elements, or permanent mechanics
- Planning story content tied to a real-world build session
- Planning community growth strategy (Discord, Twitch, social media)
- Designing community-to-customer engagement patterns
- Writing content that serves the community → authority → products pipeline
- Preparing LotAT night marketing copy from a handed-off story (`Creative/WorldBuilding/Storylines/ready/`) — read `title`, `summary`, `cast`, and `tone` to produce stream schedule copy and social teaser copy

## Do Not Activate When

- Task is pure C# scripting with no public text output → use `streamerbot-dev`
- Task is story content generation without brand concerns → use `lotat-writer`
- Task is art generation → use `art-director`

## Skill Load Order

1. `skills/core.md` — always load first; brand pillars, metaphor, output quality checks
2. `skills/voice/_index.md` — when producing any public-facing text
3. `skills/canon-guardian/_index.md` — when reviewing or adding story/lore/characters
4. `skills/content-strategy/_index.md` — when connecting story to a real-world build
5. `skills/community-growth/_index.md` — when planning community engagement, audience development, or growth strategy

## Chains To

| Next Role | When |
|---|---|
| `lotat-writer` | When content strategy planning leads to story generation |
| `lotat-tech` | After canon-approved story goes to pipeline |
| `ops` | After producing changes that involve code (via `streamerbot-dev` chain) |
| `content-repurposer` | When community strategy identifies content repurposing needs |

## Out of Scope

- C# scripting
- Art generation
- Story JSON authoring (this role reviews and plans; `lotat-writer` authors)

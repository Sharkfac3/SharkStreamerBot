# Onboarding — SharkStreamerBot

> **For agents:** Read this if you are new to this project. It tells you what this project is, what it is for, and what to read next based on the task you have been given.

---

## What Is This Project?

SharkStreamerBot is the technical and creative infrastructure for a live stream where an R&D company is being built in public. Two things happen simultaneously:

1. **Live R&D for off-road racing products** — real research and development, novel approaches, knowledge shared freely, real mistakes on stream. The stream is both the workspace and the marketing engine for the business.
2. **Interactive entertainment** — Legends of the ASCII Temple (LotAT), mini-games, commander roles, and other engagement features that keep viewers watching through slow stretches and create clip-worthy moments for the content pipeline.

These are not two separate things. The entertainment is the engagement layer that keeps the community growing while the real work happens. The content pipeline (live stream → highlights → short-form → community → products) is the business strategy.

**Read `Creative/Brand/BRAND-IDENTITY.md` for the full picture, including the ADHD metaphor that runs through everything and the business model that ties it all together.**

---

## What This Project Is Not

- Not a traditional gaming channel — the stream game is a layer on top of real automotive content
- Not a polished, production-quality brand — authenticity over polish, always
- Not passive watch content — the audience participates in outcomes
- Not a product-first company — the community comes first; products are the long-term play built on trust and authority earned through free knowledge sharing

---

## The Content Pipeline

Every agent should understand how their work feeds into the broader pipeline:

```
Live Stream → Highlights → Short-Form Content → Discovery → Community → Products
     ↑                                              ↓
     └──────── Entertainment keeps them watching ────┘
```

| Stage | What Happens | Key Roles |
|---|---|---|
| Live stream | Real R&D work + entertainment layers | `streamerbot-dev`, `lotat-tech`, `lotat-writer` |
| Highlights | Clip-worthy moments identified from recordings | `content-repurposer` |
| Short-form | Clips formatted for YouTube Shorts, TikTok, Instagram | `content-repurposer`, `art-director` |
| Discovery | New people find the content and visit the live stream | `brand-steward` (community growth) |
| Community | Viewers become regulars, join Discord, participate | `brand-steward` |
| Products | Community buys products they watched get developed | `product-dev` |

---

## How This Project Is Structured

```
/
├── Actions/           Streamer.bot C# runtime scripts (what runs on stream)
├── Tools/             External utilities — Mix It Up API, validators, helpers
├── Creative/          Art, world-building, brand documents, marketing
│   ├── Brand/         BRAND-IDENTITY, BRAND-VOICE, CHARACTER-CODEX (start here for brand work)
│   ├── Art/           Visual style guide and character art agents
│   ├── WorldBuilding/ LotAT game design, story content, franchise docs
│   └── Marketing/     Promotional copy, clip strategy, content planning
├── Docs/              Architecture docs and this onboarding guide
└── .agents/           Agent knowledge tree — roles, skills, living context (start here)
```

Scripts in `Actions/` are **not auto-deployed**. Each changed script must be manually copy/pasted into Streamer.bot.

---

## Start Here — All Agents

Read `.agents/ENTRY.md` first. It identifies your role, explains the navigation pattern, and points you to the right skill tree for your task.

---

## Read This Next (Based on Your Task)

### If you are working on LotAT stories or lore
Role: `lotat-writer`
1. `.agents/ENTRY.md` → `roles/lotat-writer/role.md`
2. `.agents/roles/lotat-writer/skills/core.md`
3. `Creative/Brand/BRAND-IDENTITY.md` — Brand foundation and the metaphor
4. `Creative/Brand/CHARACTER-CODEX.md` — Canonical character reference
5. `Creative/WorldBuilding/Agents/D&D-Agent.md` — Game mechanics

### If you are working on the LotAT technical pipeline
Role: `lotat-tech`
1. `.agents/ENTRY.md` → `roles/lotat-tech/role.md`
2. `.agents/roles/lotat-tech/skills/core.md`
3. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — JSON schema
4. `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md` — Engine guide

### If you are writing chat bot output or stream text
Role: `brand-steward`
1. `.agents/ENTRY.md` → `roles/brand-steward/role.md`
2. `.agents/roles/brand-steward/skills/core.md`
3. `Creative/Brand/BRAND-IDENTITY.md` — Brand values and voice
4. `Creative/Brand/BRAND-VOICE.md` — Tone and language per context

### If you are working on C# Streamer.bot scripts
Role: `streamerbot-dev`
1. `.agents/ENTRY.md` → `roles/streamerbot-dev/role.md`
2. `.agents/roles/streamerbot-dev/skills/core.md`
3. `Actions/SHARED-CONSTANTS.md` — Canonical variable and OBS names
4. `Actions/HELPER-SNIPPETS.md` — Reusable copy/paste patterns

### If you are working on art generation
Role: `art-director`
1. `.agents/ENTRY.md` → `roles/art-director/role.md`
2. `.agents/roles/art-director/skills/core.md`
3. `Creative/Art/Agents/stream-style-art-agent.md` — Visual style (always load first)
4. Relevant character agent file

### If you are planning content around a build
Role: `brand-steward` (content-strategy sub-skill)
1. `.agents/roles/brand-steward/skills/content-strategy/_index.md`
2. `Creative/Brand/BRAND-IDENTITY.md` — The build/game relationship
3. `Creative/Brand/BRAND-VOICE.md` — Stream title and description guidance

### If you are adding a new feature or character
Roles: `brand-steward` (canon review) + relevant dev role
1. `.agents/roles/brand-steward/skills/canon-guardian/_index.md` — review before committing
2. `Creative/Brand/BRAND-IDENTITY.md` — Brand values; does this fit?
3. `Creative/Brand/CHARACTER-CODEX.md` — Does this conflict with existing cast?

### If you are repurposing stream content for short-form
Role: `content-repurposer`
1. `.agents/ENTRY.md` → `roles/content-repurposer/role.md`
2. `.agents/roles/content-repurposer/skills/core.md`
3. `Creative/Brand/BRAND-IDENTITY.md` — Brand voice and business context
4. `Creative/Brand/BRAND-VOICE.md` — Tone for captions and descriptions

### If you are working on product documentation
Role: `product-dev`
1. `.agents/ENTRY.md` → `roles/product-dev/role.md`
2. `.agents/roles/product-dev/skills/core.md` — Note: this role is a placeholder; check Next Steps in `role.md`
3. `Creative/Brand/BRAND-IDENTITY.md` — Brand alignment for product content

### If you are doing ops work (sync, validation, change summary)
Role: `ops`
1. `.agents/ENTRY.md` → `roles/ops/role.md`
2. `.agents/roles/ops/skills/core.md`

---

## Key Conventions

- **No auto-deploy:** All `Actions/` script changes require manual copy/paste to Streamer.bot
- **Cast is fixed:** No new named LotAT/Starship Shamples characters without operator approval
- **Story schema is a contract:** The technical engine reads story JSON directly — schema changes break the engine
- **Brand first for any public text:** Load `brand-steward` before writing anything that reaches the audience
- **Canon guardian before new lore:** Load `brand-steward-canon-guardian` before adding anything to the game world
- **ops-change-summary is always terminal:** After any code change, produce the change summary

---

## Key People and Context

- Solo streamer with ADHD — the brand is built around neurodivergent authenticity
- Building an R&D company for off-road racing products — the stream is the workspace; knowledge sharing builds authority; the community is the future customer base
- Small Discord community of regulars actively providing feedback
- Multiple coding agents collaborate on this project — check `WORKING.md` before starting any task

This is an incubator-style project: open knowledge sharing, community co-creation, no gatekeeping.

---

## See Also

- `WORKING.md` — Active agent work and task queue (check before starting anything)
- `Docs/AGENT-WORKFLOW.md` — How to contribute: direct vs. branch, merge review template
- `AGENTS.md` — Quick role routing table
- `README.md` — Repo structure overview
- `Creative/Brand/BRAND-IDENTITY.md` — Why this all exists

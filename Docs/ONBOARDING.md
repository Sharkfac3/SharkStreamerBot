# Onboarding — SharkStreamerBot

> **For agents:** Read this if you are new to this project. It tells you what this project is, what it is for, and what to read next based on the task you have been given.

---

## What Is This Project?

SharkStreamerBot is the technical and creative infrastructure for a Twitch streaming channel built around two things happening simultaneously:

1. **Live jeep and automotive builds** — real hands-on projects, novel approaches, shared knowledge, real mistakes on stream
2. **Starship Shamples** — a Twitch chat-controlled spaceship adventure game that runs during the stream; chaotic, failure-forward, and participatory

These are not two separate things. Starship Shamples is a live metaphor for the build experience itself. The chaos of the game mirrors the chaos of building. The crew (chat) is the community. The ship is the neurodivergent mind doing the work.

**Read `Creative/Brand/BRAND-IDENTITY.md` for the full picture, including the ADHD metaphor that runs through everything in this project.**

---

## What This Project Is Not

- Not a traditional gaming channel — the stream game is a layer on top of real automotive content
- Not a polished, production-quality brand — authenticity over polish, always
- Not passive watch content — the audience participates in outcomes

---

## How This Project Is Structured

```
/
├── Actions/           Streamer.bot C# runtime scripts (what runs on stream)
├── Tools/             External utilities — Mix It Up API, validators, helpers
├── Creative/          Art, world-building, brand documents, marketing
│   ├── Brand/         BRAND-IDENTITY, BRAND-VOICE, CHARACTER-CODEX (start here for brand work)
│   ├── Art/           Visual style guide and character art agents
│   ├── WorldBuilding/ Starship Shamples game design, story content, franchise docs
│   └── Marketing/     Promotional copy, clip strategy, content planning
├── Docs/              Architecture docs and this onboarding guide
└── .pi/skills/        Skill files — focused agent instructions loaded per task
```

Scripts in `Actions/` are **not auto-deployed**. Each changed script must be manually copy/pasted into Streamer.bot. See `AGENTS.md` for the sync workflow.

---

## Read This First (Based on Your Task)

### If you are working on Starship Shamples stories or lore
1. `Creative/Brand/BRAND-IDENTITY.md` — Brand foundation and the metaphor
2. `Creative/Brand/CHARACTER-CODEX.md` — Canonical character reference
3. `Creative/WorldBuilding/Agents/D&D-Agent.md` — Game mechanics
4. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md` — Story schema
5. Skills: `creative-worldbuilding`, `brand-canon-guardian`

### If you are writing chat bot output or stream text
1. `Creative/Brand/BRAND-IDENTITY.md` — The brand's values and voice
2. `Creative/Brand/BRAND-VOICE.md` — Tone and language per context
3. `Creative/Brand/CHARACTER-CODEX.md` — If referencing characters
4. Skill: `brand-steward`

### If you are working on C# Streamer.bot scripts
1. `AGENTS.md` — Routing table; tells you which skills to load
2. `Actions/SHARED-CONSTANTS.md` — Canonical variable and OBS names
3. `Actions/HELPER-SNIPPETS.md` — Reusable copy/paste patterns
4. Skills: `streamerbot-scripting` + the relevant `feature-*` skill

### If you are working on art generation
1. `Creative/Art/Agents/StreamStyle-art-agent.md` — Visual style (load first, always)
2. Relevant character agent (CaptainStretch, TheDirector, WaterWizard)
3. `Creative/Brand/CHARACTER-CODEX.md` — Character personality context
4. Skill: `creative-art`

### If you are planning content around a build
1. `Creative/Brand/BRAND-IDENTITY.md` — The build/game relationship
2. `Creative/Brand/BRAND-VOICE.md` — Stream title and description guidance
3. `Creative/WorldBuilding/Franchises/StarshipShamples.md` — Franchise scope
4. Skills: `content-strategy` → `creative-worldbuilding`

### If you are adding a new feature or character
1. `Creative/Brand/BRAND-IDENTITY.md` — Brand values; does this fit?
2. `Creative/Brand/CHARACTER-CODEX.md` — Does this conflict with existing cast?
3. `AGENTS.md` — Routing for the technical implementation
4. Skill: `brand-canon-guardian` (review before committing to anything)

---

## The Skill System

This project uses [pi skills](https://agentskills.io/specification). Instead of loading all project rules at once, specific skill files are loaded per task.

- `AGENTS.md` is the routing table — it maps tasks to skills
- `.pi/skills/` contains the skill files
- `.pi/skills/README.md` lists all available skills

Always check `AGENTS.md` before starting work to confirm you have the right skills loaded.

---

## Key Conventions

- **No auto-deploy:** All `Actions/` script changes require manual copy/paste to Streamer.bot
- **Cast is fixed:** No new named Starship Shamples characters without operator approval
- **Story schema is a contract:** The technical engine reads story JSON directly — schema changes break the engine
- **Brand first for any public text:** Load `brand-steward` before writing anything that reaches the audience
- **Canon guardian before new lore:** Load `brand-canon-guardian` before adding anything to the game world

---

## Key People and Context

- Solo streamer with ADHD — the brand is built around neurodivergent authenticity
- Small Discord community of regulars actively providing feedback
- A couple of friends who also stream — potential collaborative growth

This is an incubator-style project: open knowledge sharing, community co-creation, no gatekeeping.

---

## See Also

- `AGENTS.md` — Skill routing and scope boundaries
- `README.md` — Repo structure overview
- `Creative/Brand/BRAND-IDENTITY.md` — Why this all exists

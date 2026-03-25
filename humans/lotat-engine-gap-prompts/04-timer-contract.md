You are acting as the `lotat-tech` role for the SharkStreamerBot project.

Your job: help refine the technical pipeline for Legends of the ASCII Temple (LotAT) — especially the JSON schema contract, story pipeline architecture, and the C# engine/runtime contract that will eventually consume story content.

Before doing anything else, load and follow these files in this order:
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/lotat-tech/role.md`
7. `.agents/roles/lotat-tech/skills/core.md`

Then load these LotAT-specific references:
- `Actions/SHARED-CONSTANTS.md`
- `Actions/HELPER-SNIPPETS.md`
- `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`
- `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`
- `.agents/roles/lotat-tech/skills/story-pipeline/_index.md`
- `.agents/roles/lotat-tech/skills/story-pipeline/json-schema.md`
- `.agents/roles/lotat-tech/skills/engine/docs-map.md`
- `.agents/roles/lotat-tech/skills/engine/_index.md`
- `.agents/roles/lotat-tech/skills/engine/commands.md`
- `.agents/roles/lotat-tech/skills/engine/session-lifecycle.md`
- `.agents/roles/lotat-tech/skills/engine/state-and-voting.md`
- any current story JSON the engine is expected to consume, especially `Creative/WorldBuilding/Storylines/finished/test-prototype-soda-comet.json`

Operating rules:
- Maintain strict separation between story content and engine implementation.
- The story role produces JSON content; you produce the technical contract and implementation guidance.
- Do not mix narrative authoring and engine code in the same output.
- Treat the JSON schema as a contract shared with `lotat-writer`.
- Do not add or rename schema fields casually.
- Ask before making breaking schema changes.
- Any new LotAT state variable must also be added to `Actions/Twitch Core Integrations/stream-start.cs` and `Actions/SHARED-CONSTANTS.md` when implementation happens.
- Reuse helper patterns from `Actions/HELPER-SNIPPETS.md` when appropriate.

Very important process rule:
- Do **not** change any project files yet.
- First, study the current contract and identify the exact ambiguity this prompt is about.
- Then ask me the clarifying questions you need answered **before** proposing edits.
- If you think no changes are needed, explain why and still confirm your assumptions with questions first.

When responding in this chat:
1. Start with a short summary of your understanding of the current contract.
2. List the ambiguity or gap you see.
3. Ask your clarifying questions.
4. Wait for my answers before proposing file changes.

Do not edit files, draft patches, or rewrite docs until after I answer your questions.

Your focused task:
Resolve the ambiguity around LotAT timers.

Specifically investigate:
- what timer types LotAT needs in v1 (join timer, decision timer, anything else)
- whether timer durations are global defaults, operator-configurable, or story-authored
- whether the timer contract belongs in runtime docs only or also needs constants/state entries
- whether LotAT should use Streamer.bot named timers, another deterministic mechanism, or a hybrid
- what timer names and reset expectations would be needed once implementation begins

Questions you should be ready to ask me may include:
- What should the default join duration be?
- What should the default decision duration be?
- Do we want per-story or per-node timing in v1?
- How much operator override is acceptable live on stream?

Do not add timer names, state vars, or doc changes until I answer your questions.

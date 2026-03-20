# LotAT Tech Chat Starter

Use this prompt to start a new chat when you want the agent to work in the `lotat-tech` role.

---

Please act as the `lotat-tech` agent for this repository.

Before doing anything else:
1. Check `WORKING.md` for active conflicts.
2. Read `.agents/ENTRY.md`.
3. Read `.agents/roles/lotat-tech/role.md`.
4. Read `.agents/roles/lotat-tech/skills/core.md`.
5. If the task is schema or pipeline related, load the relevant `lotat-tech` story-pipeline skills.
6. If the task is engine related, load the relevant `lotat-tech` engine skills.
7. If you make code changes, finish by loading the `ops-change-summary` skill and give me the validation checklist, paste targets, and any manual sync/setup steps.

Working style:
- Treat LotAT as a two-layer system: story content and technical engine are separate concerns.
- Keep story content out of the engine unless I explicitly ask for mixed planning.
- Preserve schema compatibility unless I explicitly approve a schema change.
- Call out any effects on Streamer.bot globals, story JSON compatibility, command support, or runtime flow.
- If you add a new global variable, also update `Actions/Twitch Core Integrations/stream-start.cs` and `Actions/SHARED-CONSTANTS.md`.
- If the task crosses into general `Actions/` script work, chain to `streamerbot-dev` guidance as needed.

When you respond after your initial repo/role read, give me:
- a short restatement of the task
- any files you expect to inspect or edit
- any conflicts or missing context you notice
- a brief plan

Task:

<replace this block with your actual LotAT technical task>

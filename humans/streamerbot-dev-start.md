# Streamerbot Dev Chat Starter

Use this prompt to start a new chat when you want the agent to work in the `streamerbot-dev` role.

---

Please act as the `streamerbot-dev` agent for this repository.

Before doing anything else:
1. Check `WORKING.md` for active conflicts.
2. Read `.agents/ENTRY.md`.
3. Read `.agents/roles/streamerbot-dev/role.md`.
4. Read `.agents/roles/streamerbot-dev/skills/core.md`.
5. If the task is inside a specific feature area, load any additional `streamerbot-dev` sub-skills that apply.
6. If you make code changes, finish by loading the `ops-change-summary` skill and give me the validation checklist, paste targets, and any manual Streamer.bot sync steps.

Working style:
- Treat `Actions/` C# files as Streamer.bot scripts that must stay copy/paste ready.
- Prefer minimal-risk changes.
- Preserve existing behavior unless I explicitly ask for a behavior change.
- Call out any required Streamer.bot UI setup, variable setup, trigger wiring, or action ordering.
- If you add a new global variable, also update `Actions/Twitch Core Integrations/stream-start.cs` and `Actions/SHARED-CONSTANTS.md`.
- If chat-facing text changes, also load the `brand-steward` role guidance as needed.

When you respond after your initial repo/role read, give me:
- a short restatement of the task
- any files you expect to inspect or edit
- any conflicts or missing context you notice
- a brief plan

Task:

<replace this block with your actual Streamer.bot scripting task>

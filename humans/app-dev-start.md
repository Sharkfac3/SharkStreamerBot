# App Dev Chat Starter

Use this prompt to start a new chat when you want the agent to work in the `app-dev` role.

---

Please act as the `app-dev` agent for this repository.

Before doing anything else:
1. Check `WORKING.md` for active conflicts.
2. Read `.agents/ENTRY.md`.
3. Read `.agents/roles/app-dev/role.md`.
4. Read `.agents/roles/app-dev/skills/core.md`.
5. If the task is stream-interaction app work, load the relevant `app-dev` stream-interactions skills.
6. If the task overlaps with Streamer.bot scripts, tooling, or brand/public copy, call out where another role should be chained in.
7. If you make code changes, finish by loading the `ops-change-summary` skill and give me the validation checklist, run/setup steps, and any operator-facing notes.

Working style:
- Treat this role as standalone app work, not Streamer.bot script work.
- Prefer pragmatic architecture and minimal dependencies unless I ask otherwise.
- Call out assumptions early, especially because this role is still lightly defined in the repo.
- If you need to create a new app domain, place it under `Apps/<AppName>/` and explain the structure.
- Make integration points with Streamer.bot, Mix It Up, Twitch, or OBS explicit.

When you respond after your initial repo/role read, give me:
- a short restatement of the task
- any files you expect to inspect or edit
- any conflicts or missing context you notice
- a brief plan

Task:

<replace this block with your actual app development task>

# Ops Chat Starter

Use this prompt to start a new chat when you want the agent to work in the `ops` role.

---

Please act as the `ops` agent for this repository.

Before doing anything else:
1. Check `WORKING.md` for active conflicts.
2. Read `.agents/ENTRY.md`.
3. Read `.agents/roles/ops/role.md`.
4. Read `.agents/roles/ops/skills/core.md`.
5. If the task is validation related, load the relevant `ops` validation skills.
6. If the task is sync related, load the relevant `ops` sync skills.
7. If the task ends in a code change, finish by loading the `ops-change-summary` skill and give me the operator-ready summary.

Working style:
- Prefer small, targeted operational changes.
- Preserve existing workflow and operator expectations unless I explicitly request a process change.
- Call out any routing, validation, sync, or tooling drift you find.
- When changing agent infrastructure, keep `.agents/`, `.pi/skills/`, validation, and generated routing docs aligned.
- If a task affects Streamer.bot manual paste workflow, make the operator steps explicit.

When you respond after your initial repo/role read, give me:
- a short restatement of the task
- any files you expect to inspect or edit
- any conflicts or missing context you notice
- a brief plan

Task:

<replace this block with your actual ops/tooling/validation task>

You are acting as the `lotat-tech` role for the SharkStreamerBot project.

Your job: document the runtime session lifecycle for Legends of the ASCII Temple (LotAT) as agent scaffolding only.

Before doing anything else, load and follow these files in this order:
1. `WORKING.md`
2. `.agents/ENTRY.md`
3. `.agents/_shared/project.md`
4. `.agents/_shared/conventions.md`
5. `.agents/_shared/coordination.md`
6. `.agents/roles/lotat-tech/role.md`
7. `.agents/roles/lotat-tech/skills/core.md`
8. `.agents/roles/lotat-tech/skills/engine/_index.md`
9. `.agents/roles/lotat-tech/skills/engine/commands.md`
10. `Creative/WorldBuilding/Experiments/StarshipShamples-coding-agent.md`
11. `Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md`

Task:
Create or refine a lotat-tech engine skill doc that explains the LotAT session lifecycle as a runtime contract.

The doc should cover:
- runtime stages like idle / join_open / node_intro / decision_open / decision_resolving / ended
- join-phase behavior at session start
- `!join` as a runtime session command
- participant roster creation and roster-freeze behavior
- zero-join behavior
- node-entry flow
- decision-window open/close flow
- end-of-session behavior
- operator recovery controls

Rules:
- Do not write or modify any C# scripts.
- Do not modify story JSON files.
- Do not add new story-schema fields.
- Keep this as engine/runtime scaffolding for future agents.
- Be explicit that this is a runtime contract, not authored story content.
- Update lotat-tech navigation docs if needed so future agents can find the new skill easily.

When responding:
- summarize what doc(s) you created or updated
- call out the runtime assumptions you documented
- explicitly say whether any schema changes were made

You are acting as the `lotat-tech` role for the SharkStreamerBot project.

Your job: document the LotAT runtime state model and voting contract as agent scaffolding only.

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
Create or refine a lotat-tech engine skill doc that explains the recommended runtime state model and voting behavior for LotAT sessions.

The doc should cover:
- participant identity rules (`userId` preferred, lowercased `user` fallback)
- joined-participant roster rules
- vote eligibility rules
- valid-vote storage rules
- latest-vote-replaces-prior-vote behavior
- early-close when all joined participants have voted
- tie-break behavior
- suggested runtime state categories
- edge cases like duplicate joins, non-joined votes, late votes, zero joiners
- recovery expectations

Rules:
- Do not write or modify any C# scripts.
- Do not change authored story JSON.
- Do not invent new top-level or node-level schema fields.
- Treat `!join` as a runtime/session command, not a story-choice command.
- Keep the doc implementation-aware but not implementation-specific.

When responding:
- summarize what doc(s) you created or updated
- call out the vote-handling assumptions you documented
- explicitly say whether any schema changes were made

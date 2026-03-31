# LotAT Implementation Prompt Pack

Purpose: numbered self-contained prompts for building the LotAT v1 Streamer.bot engine in discrete agent runs.

Run in order:
1. `01-lotat-action-matrix-and-wiring-plan.md`
2. `02-lotat-shared-constants-and-stream-start-reset.md`
3. `03-lotat-session-bootstrap-and-story-load.md`
4. `04-lotat-join-flow.md`
5. `05-lotat-node-entry-core.md`
6. `06-lotat-decision-voting-and-resolution.md`
7. `07-lotat-commander-window.md`
8. `08-lotat-dice-window.md`
9. `09-lotat-session-end-and-abort.md`
10. `10-lotat-readme-and-operator-notes.md`
11. `11-lotat-smoke-test-and-drift-audit.md`

Global rules for every prompt:
- Re-read the listed docs before acting.
- Respect the current LotAT runtime contract and story contract.
- Do not integrate `!offering` or boost-state behavior into LotAT v1.
- Treat `Creative/WorldBuilding/Storylines/loaded/current-story.json` as the only runtime story file.
- Keep prompts implementation-focused and safe for Streamer.bot manual copy/paste workflow.

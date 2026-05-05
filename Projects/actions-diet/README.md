# Actions Diet — Prompt Runner

Run these prompts in order against a coding agent. Each is self-contained.
The final prompt (p15) deletes this folder. No artifacts remain after completion.

| # | File | Changes |
|---|---|---|
| 1 | [p01-baseline.md](p01-baseline.md) | None — measurement only |
| 2 | [p02-enrich-parent.md](p02-enrich-parent.md) | `Actions/AGENTS.md` |
| 3 | [p03-compress-contracts.md](p03-compress-contracts.md) | 6 contract-bearing AGENTS.md files |
| 4 | [p04-slim-commanders-parent.md](p04-slim-commanders-parent.md) | `Actions/Commanders/AGENTS.md` |
| 5 | [p05-slim-captain-stretch.md](p05-slim-captain-stretch.md) | `Actions/Commanders/Captain Stretch/AGENTS.md` |
| 6 | [p06-slim-the-director.md](p06-slim-the-director.md) | `Actions/Commanders/The Director/AGENTS.md` |
| 7 | [p07-slim-water-wizard.md](p07-slim-water-wizard.md) | `Actions/Commanders/Water Wizard/AGENTS.md` |
| 8 | [p08-slim-twitch-core.md](p08-slim-twitch-core.md) | `Actions/Twitch Core Integrations/AGENTS.md` |
| 9 | [p09-slim-twitch-bits-hype-points.md](p09-slim-twitch-bits-hype-points.md) | Bits + Hype Train + Channel Points AGENTS.md |
| 10 | [p10-slim-voice-overlay.md](p10-slim-voice-overlay.md) | Voice Commands + Overlay AGENTS.md |
| 11 | [p11-slim-focus-xj-intros.md](p11-slim-focus-xj-intros.md) | Rest Focus Loop + XJ Drivethrough + Intros AGENTS.md |
| 12 | [p12-slim-squad.md](p12-slim-squad.md) | `Actions/Squad/AGENTS.md` |
| 13 | [p13-slim-lotat.md](p13-slim-lotat.md) | `Actions/LotAT/AGENTS.md` |
| 14 | [p14-slim-small.md](p14-slim-small.md) | Destroyer + Temporary AGENTS.md |
| 15 | [p15-verify-and-cleanup.md](p15-verify-and-cleanup.md) | Verify all, then delete `Projects/actions-diet/` |

**Do not skip p02.** Later prompts drop content that p02 moves to the parent.
**Do not skip p03 before p04–p07.** Commanders prose slim happens after contract compression.

---
name: streamerbot-dev
description: Streamer.bot C# actions development. Base role for all .cs script work. Routes to .agents/roles/streamerbot-dev/ for full context.
---

# streamerbot-dev

Load `.agents/roles/streamerbot-dev/role.md` to confirm scope and skill load order.

## Always Load

`.agents/roles/streamerbot-dev/skills/core.md`

## Then Navigate

| Task | Load |
|---|---|
| Any Squad mini-game work | `skills/squad/` sub-skill |
| Commander role/command work | `skills/commanders/` sub-skill |
| Twitch events, bits, channel points, hype train | `skills/twitch/` sub-skill |
| Voice command mode/scene work | `skills/voice-commands/` sub-skill |
| LotAT C# engine work | `skills/lotat/` sub-skill |

## Sub-Skills

- `streamerbot-dev-squad/SKILL.md`
- `streamerbot-dev-commanders/SKILL.md`
- `streamerbot-dev-twitch/SKILL.md`
- `streamerbot-dev-voice-commands/SKILL.md`
- `streamerbot-dev-lotat/SKILL.md`

## Terminal

After any code change: load `ops-change-summary/SKILL.md`

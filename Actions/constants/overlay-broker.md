---
id: constants-overlay-broker
type: constants
description: WebSocket broker connection state and overlay topic string constants.
owner: streamerbot-dev
secondaryOwners: [app-dev]
parent: ../SHARED-CONSTANTS.md
---

# Overlay / Broker Constants

Canonical broker connection state and topic strings. These are used by overlay publisher scripts and any script that sends events to the overlay. Topic strings here are the source of truth — do not hardcode them in scripts.

Note: for C# usage patterns (how to publish to the broker), see Actions/Helpers/overlay-broker.md.

## Overlay / Broker (shared)

- `BROKER_WS_INDEX` = `0` *(integer — position of the broker entry in Streamer.bot → Servers/Clients → WebSocket Clients; first entry = 0)*
- `VAR_BROKER_CONNECTED` = `broker_connected` *(non-persisted boolean; true when WebSocket to broker is live)*
- `BROKER_CLIENT_NAME` = `"streamerbot"` *(sent in ClientHello; must match CLIENT_NAMES.STREAMERBOT in @stream-overlay/shared/topics.ts)*

Topic string constants used in C# scripts (mirror TOPICS in @stream-overlay/shared/topics.ts).
Each script defines these as private constants locally — copy them from here to stay in sync.

**Overlay — generic asset commands** (used in `Actions/Overlay/`):
- `TOPIC_OVERLAY_SPAWN`          = `"overlay.spawn"`
- `TOPIC_OVERLAY_MOVE`           = `"overlay.move"`
- `TOPIC_OVERLAY_ANIMATE`        = `"overlay.animate"`
- `TOPIC_OVERLAY_REMOVE`         = `"overlay.remove"`
- `TOPIC_OVERLAY_CLEAR`          = `"overlay.clear"`

**Overlay — audio** (used in any action that plays/stops sounds):
- `TOPIC_OVERLAY_AUDIO_PLAY`     = `"overlay.audio.play"`
- `TOPIC_OVERLAY_AUDIO_STOP`     = `"overlay.audio.stop"`

**LotAT session lifecycle** (used in `Actions/LotAT/overlay-publish.cs`):
- `TOPIC_LOTAT_SESSION_START`    = `"lotat.session.start"`
- `TOPIC_LOTAT_SESSION_END`      = `"lotat.session.end"`
- `TOPIC_LOTAT_JOIN_OPEN`        = `"lotat.join.open"`
- `TOPIC_LOTAT_JOIN_UPDATE`      = `"lotat.join.update"`
- `TOPIC_LOTAT_JOIN_CLOSE`       = `"lotat.join.close"`
- `TOPIC_LOTAT_NODE_ENTER`       = `"lotat.node.enter"`
- `TOPIC_LOTAT_VOTE_OPEN`        = `"lotat.vote.open"`
- `TOPIC_LOTAT_VOTE_CAST`        = `"lotat.vote.cast"`
- `TOPIC_LOTAT_VOTE_CLOSE`       = `"lotat.vote.close"`
- `TOPIC_LOTAT_DICE_OPEN`        = `"lotat.dice.open"`
- `TOPIC_LOTAT_DICE_ROLL`        = `"lotat.dice.roll"`
- `TOPIC_LOTAT_DICE_CLOSE`       = `"lotat.dice.close"`
- `TOPIC_LOTAT_COMMANDER_OPEN`   = `"lotat.commander.open"`
- `TOPIC_LOTAT_COMMANDER_CLOSE`  = `"lotat.commander.close"`
- `TOPIC_LOTAT_CHAOS_UPDATE`     = `"lotat.chaos.update"`

**Squad mini-games** (used in `Actions/Squad/*/overlay-publish.cs`):
- `TOPIC_SQUAD_DUCK_START`       = `"squad.duck.start"`
- `TOPIC_SQUAD_DUCK_UPDATE`      = `"squad.duck.update"`
- `TOPIC_SQUAD_DUCK_END`         = `"squad.duck.end"`
- `TOPIC_SQUAD_CLONE_START`      = `"squad.clone.start"`
- `TOPIC_SQUAD_CLONE_UPDATE`     = `"squad.clone.update"`
- `TOPIC_SQUAD_CLONE_END`        = `"squad.clone.end"`
- `TOPIC_SQUAD_PEDRO_START`      = `"squad.pedro.start"`
- `TOPIC_SQUAD_PEDRO_UPDATE`     = `"squad.pedro.update"`
- `TOPIC_SQUAD_PEDRO_END`        = `"squad.pedro.end"`
- `TOPIC_SQUAD_TOOTHLESS_START`  = `"squad.toothless.start"`
- `TOPIC_SQUAD_TOOTHLESS_UPDATE` = `"squad.toothless.update"`
- `TOPIC_SQUAD_TOOTHLESS_END`    = `"squad.toothless.end"`

Used in:
- `Actions/Overlay/broker-connect.cs`
- `Actions/Overlay/broker-publish.cs`
- `Actions/Overlay/broker-disconnect.cs`
- `Actions/Overlay/test-overlay.cs`
- `Actions/LotAT/overlay-publish.cs`
- `Actions/Squad/Duck/overlay-publish.cs`
- `Actions/Squad/Pedro/overlay-publish.cs`
- `Actions/Squad/Toothless/overlay-publish.cs`
- `Actions/Twitch Core Integrations/stream-start.cs` *(ensures broker connection/ClientHello at startup)*

Operator notes:
- `broker_connected` is non-persisted. It is set to `false` before a Streamer.bot-side connect/register attempt and to `true`
  when the broker ClientHello handshake is sent successfully.
- `stream-start.cs` now includes the broker connect/register logic directly, so a separate broker-connect sub-action is optional for manual reconnects rather than required for normal stream start.
- `BROKER_WS_INDEX` must match the position of the "Overlay Broker" entry in the Streamer.bot
  WebSocket Clients list (Servers/Clients → WebSocket Clients). Default is 0 (first entry).

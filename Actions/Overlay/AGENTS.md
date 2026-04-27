---
id: actions-overlay
type: domain-route
description: Streamer.bot bridge scripts, paste targets, and publisher templates for the stream overlay broker.
owner: streamerbot-dev
secondaryOwners:
  - app-dev
workflows:
  - change-summary
  - sync
  - validation
status: active
path: Actions/Overlay/
---

# Actions/Overlay — Agent Guide

## Purpose

This folder owns the Streamer.bot-side bridge to the stream overlay ecosystem. These C# scripts connect Streamer.bot to the local overlay broker, publish broker-message envelopes, disconnect cleanly, and provide an end-to-end overlay test action.

Streamer.bot never talks directly to the Phaser overlay. It publishes to the broker; the broker routes to the overlay and any future clients.

```text
Streamer.bot C# action -> local broker WebSocket -> Phaser overlay in OBS
```

## When to Activate

Use this guide for changes under [Actions/Overlay/](./), especially:

- [broker-connect.cs](broker-connect.cs) — opens the broker WebSocket and sends the Streamer.bot hello frame.
- [broker-publish.cs](broker-publish.cs) — reference template for the `PublishBrokerMessage` helper.
- [broker-disconnect.cs](broker-disconnect.cs) — clean stream-end disconnect.
- [test-overlay.cs](test-overlay.cs) — manual end-to-end overlay smoke test.
- Any Streamer.bot setup, paste target, WebSocket client index, or broker-publish helper changes.

Also activate when app-side protocol changes in [Apps/stream-overlay/](../../Apps/stream-overlay/) require C# publisher changes.

## Primary Owner

`streamerbot-dev` owns this folder: C# action behavior, Streamer.bot UI setup assumptions, paste targets, global variables, and helper snippets copied into publishing actions.

## Secondary Owners / Chain To

- `app-dev` for broker protocol changes, topic taxonomy, TypeScript shared types, overlay rendering behavior, and app build/test commands.
- `lotat-tech` when `lotat.*` payloads or stage ordering are affected.
- `brand-steward` if chat-facing test messages or public overlay copy changes.
- `ops` for validation, sync, and final change-summary handoff.

Related guides:

- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) — route, ownership, local workflow, and app validation.
- [Apps/stream-overlay/docs/protocol.md](../../Apps/stream-overlay/docs/protocol.md) — broker protocol, topics, subscriptions, and topic-change workflow.
- [Apps/stream-overlay/docs/asset-system.md](../../Apps/stream-overlay/docs/asset-system.md) — asset paths, asset IDs, lifecycle commands, and audio behavior.
- [Apps/stream-overlay/docs/rendering-notes.md](../../Apps/stream-overlay/docs/rendering-notes.md) — overlay renderer behavior, feature renderers, OBS caveats, and gotchas.
- [Actions/Squad/AGENTS.md](../Squad/AGENTS.md) — Squad publish-template integration.
- [Actions/LotAT/AGENTS.md](../LotAT/AGENTS.md) — LotAT publish-template integration.

## Required Reading

Before editing, read:

- [Actions/Overlay/README.md](README.md) for operator setup and script behavior.
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) for route/ownership and [Apps/stream-overlay/docs/protocol.md](../../Apps/stream-overlay/docs/protocol.md) for app-side broker and protocol rules.
- [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) for the authoritative TypeScript contract.
- [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) before adding or renaming topic constants.
- [Actions/Helpers/json-no-external-libraries.md](../Helpers/json-no-external-libraries.md) when complex JSON serialization is needed.

## Local Workflow

1. Keep scripts paste-ready for Streamer.bot inline C# actions. Each deployed action needs a complete `CPHInline` class.
2. For any publishing action, copy the constants block and `PublishBrokerMessage` helper from [broker-publish.cs](broker-publish.cs) into the target action's class.
3. Build payloads as JSON strings or with the shared serialization helper from [Actions/Helpers/json-no-external-libraries.md](../Helpers/json-no-external-libraries.md).
4. Publish to topic names that match [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
5. Test with the broker and overlay running from [Apps/stream-overlay/](../../Apps/stream-overlay/), then run [test-overlay.cs](test-overlay.cs) via a mod/operator-only chat command.

## Validation

For documentation-only changes, run the agent-tree validator from the repository root:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-04-validator.failures.txt
```

For Streamer.bot script changes:

- Manually inspect C# syntax for Streamer.bot-compatible inline script style.
- Confirm the broker is running from [Apps/stream-overlay/](../../Apps/stream-overlay/) with `pnpm dev:broker`.
- Confirm the overlay is loaded with `pnpm dev:overlay` and debug mode visible in a browser or OBS.
- Trigger [broker-connect.cs](broker-connect.cs), then [test-overlay.cs](test-overlay.cs), and verify the broker health endpoint shows Streamer.bot connected.

## Boundaries / Out of Scope

- Do not add broker, protocol, or rendering logic here. That belongs in [Apps/stream-overlay/](../../Apps/stream-overlay/).
- Do not add feature-specific Squad or LotAT business logic here. Those publishers live beside their feature actions under [Actions/Squad/](../Squad/) and [Actions/LotAT/](../LotAT/).
- Do not deploy [broker-publish.cs](broker-publish.cs) as its own Streamer.bot action. It is a reference template.
- Do not hardcode new topic strings without updating the shared TypeScript source and [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md).
- Info-service and production-manager migration are out of scope for this route.

## Handoff Notes

Every code change in this folder needs a final handoff that includes:

- Streamer.bot paste target list for each edited `.cs` file.
- Whether the operator must update the WebSocket client index or broker host/port in Streamer.bot UI.
- Whether [Actions/SHARED-CONSTANTS.md](../SHARED-CONSTANTS.md) needs matching updates.
- App-side validation output if the change affects [Apps/stream-overlay/](../../Apps/stream-overlay/).
- Manual smoke-test result for [test-overlay.cs](test-overlay.cs) when runtime behavior changes.

## Runtime Notes

### Streamer.bot WebSocket setup

Streamer.bot uses its built-in WebSocket client UI. Configure one client for the local broker:

| Setting | Value |
|---|---|
| Name | Overlay Broker |
| Host | localhost |
| Port | 8765 |
| Endpoint | / |
| Scheme | ws |

The scripts assume this client is connection index 0. If the operator changes the order in Streamer.bot, update the broker WebSocket index constant in all relevant publisher helpers.

### Connection lifecycle

1. Stream start triggers [broker-connect.cs](broker-connect.cs).
2. The script connects WebSocket client index 0 and sends a hello frame with client name `streamerbot` and no subscriptions.
3. Publisher actions call `PublishBrokerMessage(topic, payloadJson)`.
4. Stream end triggers [broker-disconnect.cs](broker-disconnect.cs), which closes the socket and clears `broker_connected`.

The C# helper uses Streamer.bot WebSocket APIs. Do not add raw .NET WebSocket implementations.

### Runtime variable

| Variable | Scope | Meaning |
|---|---|---|
| `broker_connected` | Non-persisted global | Best-effort flag set by connect/disconnect scripts and checked by publishers. The helper still checks the actual WebSocket state. |

Stream start should reset `broker_connected` to false before connecting. If the Twitch stream-start script owns that reset, keep the behavior documented in that folder's guide when it is created.

## Paste / Sync Targets

Each edited C# file in this folder is a Streamer.bot paste target:

| Repo file | Streamer.bot target |
|---|---|
| [broker-connect.cs](broker-connect.cs) | Stream-start sub-action, plus optional manual reconnect action. |
| [broker-disconnect.cs](broker-disconnect.cs) | Stream-end sub-action. |
| [test-overlay.cs](test-overlay.cs) | Moderator/broadcaster-only `!testoverlay` command action. |
| [broker-publish.cs](broker-publish.cs) | Not standalone; copy constants and helper into each action that publishes overlay messages. |

If you edit [broker-publish.cs](broker-publish.cs), find and update every copied helper in feature publisher templates or runtime actions as needed. Current known reference-template locations include [Actions/LotAT/overlay-publish.cs](../LotAT/overlay-publish.cs) and the per-game overlay-publish files under [Actions/Squad/](../Squad/).

## Protocol / Publisher Contract

`PublishBrokerMessage` accepts a topic string and a pre-serialized JSON payload string. It wraps them in a broker-message envelope with:

- generated ID,
- topic,
- sender `streamerbot`,
- current timestamp,
- payload object.

It then sends the message over WebSocket client index 0. If the socket is down, it attempts one reconnect and resends the hello frame before publishing. Failure is logged and returns false; publishing scripts should fail gracefully and avoid throwing during a live stream.

Common Streamer.bot-published namespaces:

| Namespace | Use |
|---|---|
| `overlay.*` | Generic canvas asset commands such as spawn, move, animate, remove, and clear. |
| `overlay.audio.*` | Sound playback control. |
| `lotat.*` | LotAT session visual lifecycle. |
| `squad.*` | Squad mini-game visual lifecycle. |
| `stream.*` | Platform event visual alerts. |

## Build / Test Commands

The C# actions do not have a local compiler in this repo. Use app-side commands for end-to-end testing:

```bash
cd Apps/stream-overlay
pnpm dev:broker
pnpm dev:overlay
pnpm typecheck
pnpm build
```

Manual runtime smoke test:

1. Start the broker and overlay.
2. Trigger [broker-connect.cs](broker-connect.cs) from Streamer.bot.
3. Run [test-overlay.cs](test-overlay.cs) through a restricted `!testoverlay` command.
4. Verify a test image appears and disappears, and Streamer.bot logs show successful `overlay.spawn` and `overlay.remove` sends.

## Known Gotchas

- The broker must be running before [broker-connect.cs](broker-connect.cs) succeeds.
- The overlay can reconnect automatically, but Streamer.bot publishers rely on the configured WebSocket client and helper reconnect logic.
- [test-overlay.cs](test-overlay.cs) requires a test image at the path documented in [Actions/Overlay/README.md](README.md).
- A true `broker_connected` flag does not guarantee the socket is still open; the helper checks the actual WebSocket state on publish.
- Payload JSON must be valid. Use the serialization helper for nested or user-provided strings to avoid quote escaping bugs.

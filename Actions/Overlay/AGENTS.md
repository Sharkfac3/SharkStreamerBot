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

This folder owns the Streamer.bot-side bridge to the stream overlay broker: connect, publish, disconnect, and smoke-test actions.

Streamer.bot never talks directly to the Phaser overlay; it publishes WebSocket messages to the broker, which routes them to the overlay and future clients.

## When to Activate

Use this guide when editing or reviewing:

- [broker-connect.cs](broker-connect.cs)
- [broker-publish.cs](broker-publish.cs)
- [broker-disconnect.cs](broker-disconnect.cs)
- [test-overlay.cs](test-overlay.cs)
- Streamer.bot setup, paste targets, WebSocket client index, or broker-publish helper docs in this folder

## Ownership

`streamerbot-dev` owns these C# bridge scripts; chain to `app-dev` for broker protocol, topic taxonomy, shared TypeScript types, or overlay rendering changes. For shared ownership, validation, paste/sync, boundaries, and handoff rules, start with [Actions/AGENTS.md](../AGENTS.md).

## Required Reading

- [Actions/AGENTS.md](../AGENTS.md)
- [Actions/Helpers/overlay-broker.md](../Helpers/overlay-broker.md) for the `EnsureOverlayBrokerConnected` connect/register pattern
- [Apps/stream-overlay/AGENTS.md](../../Apps/stream-overlay/AGENTS.md) and [Apps/stream-overlay/docs/protocol.md](../../Apps/stream-overlay/docs/protocol.md) when app-side protocol behavior is involved
- [Apps/stream-overlay/packages/shared/src/topics.ts](../../Apps/stream-overlay/packages/shared/src/topics.ts) and [Apps/stream-overlay/packages/shared/src/protocol.ts](../../Apps/stream-overlay/packages/shared/src/protocol.ts) when topic or payload contracts change

## Folder-Specific Rules

- `broker-connect.cs` and `broker-disconnect.cs` manage the local broker WebSocket connection lifecycle.
- `broker-publish.cs` is a reference template; do not deploy it as its own Streamer.bot action.
- Publishing actions copy the constants block and helper from `broker-publish.cs`, or follow the `EnsureOverlayBrokerConnected` pattern in [Actions/Helpers/overlay-broker.md](../Helpers/overlay-broker.md).
- `test-overlay.cs` is a mod/operator-only dev/debug smoke test for the broker-to-overlay path.
- The scripts assume the Streamer.bot WebSocket client for `ws://localhost:8765/` is index `0` unless updated everywhere.
- Keep broker protocol/rendering logic in [Apps/stream-overlay/](../../Apps/stream-overlay/), not in this folder.

## Script Summary

| Script | Summary |
|---|---|
| [broker-connect.cs](broker-connect.cs) | Connects WebSocket client index `0` to the overlay broker and sends the Streamer.bot hello frame. |
| [broker-disconnect.cs](broker-disconnect.cs) | Closes the broker WebSocket connection and clears `broker_connected`. |
| [broker-publish.cs](broker-publish.cs) | Reference template for wrapping topic/payload JSON in a broker-message envelope and sending it. |
| [test-overlay.cs](test-overlay.cs) | Dev/debug smoke test that publishes overlay spawn/remove messages and confirms the pipeline. |

## Action Contracts

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "broker-connect.cs",
      "action": "Broker Connect",
      "purpose": "Contracts expected runtime behavior for broker-connect.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented broker-connect.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-connect.cs"
    },
    {
      "script": "broker-disconnect.cs",
      "action": "Broker Disconnect",
      "purpose": "Contracts expected runtime behavior for broker-disconnect.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented broker-disconnect.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-disconnect.cs"
    },
    {
      "script": "broker-publish.cs",
      "action": "Broker Publish",
      "purpose": "Contracts expected runtime behavior for broker-publish.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented broker-publish.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for broker-publish.cs"
    },
    {
      "script": "test-overlay.cs",
      "action": "Test Overlay",
      "purpose": "Contracts expected runtime behavior for test-overlay.cs.",
      "triggers": [],
      "globals": [],
      "obsSources": [],
      "obsScenes": [],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": [
        "Runs the documented test-overlay.cs Streamer.bot action behavior."
      ],
      "failureBehavior": [],
      "pasteTarget": "Streamer.bot Execute C# Code action for test-overlay.cs"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

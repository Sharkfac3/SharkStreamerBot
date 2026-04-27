# Prompt 10-04 handoff — apps-stream-overlay-actions-overlay

Date: 2026-04-26
Agent: pi

## State changes

- Created [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md) for the `apps-stream-overlay` route.
- Created [Actions/Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md) for the `actions-overlay` route.
- Wrote validator report to [Projects/agent-reflow/findings/10-04-validator.failures.txt](../findings/10-04-validator.failures.txt).

## Migration sources read

- [Projects/agent-reflow/handoffs/10-02-actions-commanders-squad-voice.handoff.md](10-02-actions-commanders-squad-voice.handoff.md)
- [Projects/agent-reflow/handoffs/10-03-actions-lotat-tools-lotat.handoff.md](10-03-actions-lotat-tools-lotat.handoff.md)
- [Projects/agent-reflow/findings/02-domain-coverage.md](../findings/02-domain-coverage.md)
- [Projects/agent-reflow/findings/05-target-shape.md](../findings/05-target-shape.md)
- [Projects/agent-reflow/findings/06-naming-convention.md](../findings/06-naming-convention.md)
- [Projects/agent-reflow/findings/08-validator.md](../findings/08-validator.md)
- [Apps/stream-overlay/README.md](../../../Apps/stream-overlay/README.md)
- [Actions/Overlay/README.md](../../../Actions/Overlay/README.md)
- [Apps/stream-overlay/package.json](../../../Apps/stream-overlay/package.json)
- `.agents/roles/app-dev/skills/stream-interactions/_index.md`
- `.agents/roles/app-dev/skills/stream-interactions/asset-system.md`
- `.agents/roles/app-dev/skills/stream-interactions/broker.md`
- `.agents/roles/app-dev/skills/stream-interactions/lotat-rendering.md`
- `.agents/roles/app-dev/skills/stream-interactions/overlay.md`
- `.agents/roles/app-dev/skills/stream-interactions/protocol.md`
- `.agents/roles/app-dev/skills/stream-interactions/squad-rendering.md`
- `.agents/roles/streamerbot-dev/skills/overlay-integration.md`

## Paste targets

These were documentation-only changes. There are no Streamer.bot script paste targets for this prompt.

If later runtime changes are made under [Actions/Overlay/](../../../Actions/Overlay/), the local guide now identifies each edited C# file as a Streamer.bot paste target:

| Repo file | Streamer.bot target |
|---|---|
| [Actions/Overlay/broker-connect.cs](../../../Actions/Overlay/broker-connect.cs) | Stream-start sub-action and optional manual reconnect action. |
| [Actions/Overlay/broker-disconnect.cs](../../../Actions/Overlay/broker-disconnect.cs) | Stream-end sub-action. |
| [Actions/Overlay/test-overlay.cs](../../../Actions/Overlay/test-overlay.cs) | Moderator/broadcaster-only `!testoverlay` command action. |
| [Actions/Overlay/broker-publish.cs](../../../Actions/Overlay/broker-publish.cs) | Reference helper only; copy into publishing actions, not standalone. |

## App build/test commands found

From [Apps/stream-overlay/package.json](../../../Apps/stream-overlay/package.json):

```bash
cd Apps/stream-overlay
pnpm install
pnpm dev:broker
pnpm dev:overlay
pnpm typecheck
pnpm build
```

Broker/debug commands documented in the new app guide:

```bash
cd Apps/stream-overlay/packages/broker
pnpm test-client
npx tsx src/test-client.ts
npx tsx src/lotat-test-session.ts
npx tsx src/squad-test-session.ts
```

Operational endpoints/URLs documented:

- Broker WebSocket: ws://localhost:8765
- Broker health check: http://localhost:8765/health
- Overlay dev URL: http://localhost:5173
- Overlay debug URL: http://localhost:5173?debug=true

## Protocol details moved

Moved/normalized the stream-interactions protocol guidance into [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md):

- Shared TypeScript source of truth: [Apps/stream-overlay/packages/shared/src/protocol.ts](../../../Apps/stream-overlay/packages/shared/src/protocol.ts) and [Apps/stream-overlay/packages/shared/src/topics.ts](../../../Apps/stream-overlay/packages/shared/src/topics.ts).
- Broker message envelope fields: generated ID, topic, sender, timestamp, payload.
- Client hello/welcome lifecycle and stable client names.
- Topic namespaces: `system.*`, `overlay.*`, `overlay.audio.*`, `lotat.*`, `squad.*`, per-game Squad topics, and `stream.*`.
- Single-level wildcard subscription caveat.
- v1 error policy: silent drop plus log for malformed messages.
- Protocol evolution rules: optional fields/new topics are safe; removal, renaming, and type changes are breaking.
- Streamer.bot publisher contract: C# scripts send pre-serialized payloads through `PublishBrokerMessage`, and the helper builds the broker envelope.

## Rendering and asset details moved

Moved/normalized app runtime guidance into [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md):

- Monorepo package roles for `packages/shared`, `packages/broker`, and `packages/overlay`.
- Broker responsibilities and explicit non-responsibilities: no business logic, no persistence, no state authority, no public exposure.
- Overlay OBS model: transparent 1920 by 1080 browser source, dev URL, generated production build, debug HUD, and Control audio via OBS requirement.
- Asset lifecycle for `overlay.spawn`, `overlay.move`, `overlay.animate`, `overlay.remove`, `overlay.clear`, `overlay.audio.play`, and `overlay.audio.stop`.
- Asset folder convention under [Apps/stream-overlay/packages/overlay/public/assets/](../../../Apps/stream-overlay/packages/overlay/public/assets/).
- GIF handling, named-vs-ephemeral asset IDs, and depth conventions.
- LotAT lower-third rendering map and component locations.
- Squad top-screen rendering map and component locations.

## Actions/Overlay details moved

Moved/normalized Streamer.bot bridge guidance into [Actions/Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md):

- WebSocket client UI setup: localhost, port 8765, endpoint `/`, scheme `ws`, connection index 0.
- Stream start/connect, publish, stream end/disconnect lifecycle.
- `broker_connected` non-persisted global behavior and caveat that actual WebSocket state must still be checked.
- Paste/sync target guidance for all files in [Actions/Overlay/](../../../Actions/Overlay/).
- Rule that [Actions/Overlay/broker-publish.cs](../../../Actions/Overlay/broker-publish.cs) is a reference helper only.
- Known helper-copy targets in [Actions/LotAT/overlay-publish.cs](../../../Actions/LotAT/overlay-publish.cs) and [Actions/Squad/](../../../Actions/Squad/).

## Cross-links created

Prompt 10-02 and 10-03 had already created both requested docs, so this prompt linked them directly:

- [Actions/Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md)
- [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md)

No deferred Squad/LotAT link was needed.

## Validation status

Command run:

```bash
python3 Tools/AgentTree/validate.py --report Projects/agent-reflow/findings/10-04-validator.failures.txt
```

Exit code: `1`

Summary output:

```text
Agent tree validation summary
| Check | Checked | Failures | Status |
|---|---:|---:|---|
| schema | 1 | 0 | PASS |
| folder-coverage | 149 | 41 | FAIL |
| link-integrity | 104 | 331 | FAIL |
| drift | 3 | 3 | FAIL |
| stub-presence | 48 | 36 | FAIL |
| orphan | 104 | 18 | FAIL |
| naming | 106 | 0 | PASS |

Total failures: 429
Failure report: /mnt/c/Users/sharkfac3/Workspace/coding/SharkStreamerBot/Projects/agent-reflow/findings/10-04-validator.failures.txt
```

Expected prompt deltas cleared:

- `folder-coverage` missing location/co-location failures are cleared for `apps-stream-overlay` and `actions-overlay`.
- `stub-presence` missing entry failures are cleared for `apps-stream-overlay` and `actions-overlay`.
- No link-integrity failures are reported for [Apps/stream-overlay/AGENTS.md](../../../Apps/stream-overlay/AGENTS.md) or [Actions/Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md) after fixing two local link/path issues found in the first run.

Remaining failures are expected Phase E baseline items for other uncreated local docs, role/root/shared frontmatter, generated routing drift, old central skill-source links, Pi meta wrappers, and orphan cleanup. Existing old app-dev stream-interactions link failures remain by design until cleanup.

## Old skill content intentionally not migrated

No substantive broker, protocol, overlay, asset-system, LotAT rendering, Squad rendering, or Streamer.bot overlay-integration guidance from the listed sources was intentionally left behind.

Not copied verbatim by design:

- Old central skill-navigation instructions pointing to deprecated `_index.md`/skill files as active entrypoints.
- Broken source-file path references from `.agents/roles/app-dev/skills/stream-interactions/` and `.agents/roles/streamerbot-dev/skills/overlay-integration.md`.
- Long JSON payload examples for every asset test case; the local guide now summarizes the commands and points to source files for authoritative types.
- Info-service and production-manager REST/API content, which is explicitly out of scope for this prompt.

## Open questions / blockers

- None blocking the next prompt.
- The production OBS file target under `packages/overlay/dist/` is generated by `pnpm build`; it may not exist in a clean checkout until the build is run, so the new app guide avoids linking directly to the generated file.
- Old source link failures under `.agents/roles/app-dev/skills/stream-interactions/` remain until the cleanup phase retires or stubs those files.

## Next prompt entry point

Proceed with the next Phase E migration prompt. Use [Projects/agent-reflow/findings/10-04-validator.failures.txt](../findings/10-04-validator.failures.txt) as the latest validator baseline.

# Stream Interactions — Overview

## Status

Placeholder — populated as specific stream interaction apps are designed.

## What Stream Interaction Apps Do

Stream interaction apps extend Streamer.bot's capabilities with richer UI, persistent state, or complex logic that is impractical in C# scripts. Examples of what might be built:

- **Live overlay dashboard** — operator-facing view of current stream state (active commander, Squad status, Chaos Meter)
- **LotAT session manager** — UI for loading story files, tracking session state, advancing nodes
- **Chat analytics** — tracking engagement patterns for Squad games and commander interactions
- **Asset manager** — browsing and applying art assets during stream

## Design Principles (When Building)

- Apps should complement Streamer.bot, not duplicate it
- Prefer reading state from Streamer.bot globals over maintaining parallel state
- Keep operator UX simple — the stream is already complex
- Document the integration boundary clearly (what data flows where)

## Update This File When

An app is approved and scoped. Replace this placeholder with the app's purpose, architecture decisions, and integration points.

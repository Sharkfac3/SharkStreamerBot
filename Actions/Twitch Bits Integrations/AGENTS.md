---
id: actions-twitch-bits-integrations
type: domain-route
description: Twitch bits and automatic reward Streamer.bot event bridges, Mix It Up payloads, pacing, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Twitch Bits Integrations — Agent Guide

## Purpose

This folder owns Streamer.bot C# scripts for Twitch cheer/bits tiers and related automatic reward bridges. These scripts sanitize viewer text, preserve Mix It Up payload compatibility, and keep readout/effect pacing safe during live events.

## When to Activate

Use this guide when editing or reviewing:

- [bits-tier-1.cs](bits-tier-1.cs)
- [bits-tier-2.cs](bits-tier-2.cs)
- [bits-tier-3.cs](bits-tier-3.cs)
- [bits-tier-4.cs](bits-tier-4.cs)
- [gigantify-emote.cs](gigantify-emote.cs)
- [message-effects.cs](message-effects.cs)
- [on-screen-celebration.cs](on-screen-celebration.cs)
- Script/operator documentation in this folder

## Ownership

`streamerbot-dev` owns runtime behavior; chain to `brand-steward` for public cheer/readout text, reward prompts, overlay copy, TTS, or Mix It Up viewer-facing wording.

Start with [Actions/AGENTS.md](../AGENTS.md) for shared Actions rules, validation, sync, and handoff expectations. Also read [Twitch Core Integrations/AGENTS.md](../Twitch%20Core%20Integrations/AGENTS.md) when changes affect stream-start reset or shared Twitch event conventions.

## Folder-Specific Rules

- Cheer tiers route by bits amount: Tier 1 = `1–99`, Tier 2 = `100–999`, Tier 3 = `1000–9999`, Tier 4 = `10000+`.
- Cheer-tier scripts use `messageStripped` first, then `message`, then `rawInput`, so Cheer tokens stay sanitized.
- Preserve word caps: Tier 1 none, Tier 2 250 words, Tier 3 100 words, Tier 4 10 words.
- Preserve readout pacing where used: `3000ms + 400ms per word + 500ms buffer`.
- Automatic reward scripts use `Twitch -> Channel Reward -> Automatic Reward Redemption` and must be filtered to the intended reward.
- Keep Mix It Up `Arguments` compatibility and lowercase/no-space `SpecialIdentifiers` keys.

## Script Summary

| Script | Bit tier / threshold | Key effect |
|---|---:|---|
| [bits-tier-1.cs](bits-tier-1.cs) | Tier 1 / 1–99 bits | Sends uncapped sanitized cheer text to Mix It Up for paced readout. |
| [bits-tier-2.cs](bits-tier-2.cs) | Tier 2 / 100–999 bits | Sends sanitized cheer text capped at 250 words to Mix It Up for paced readout. |
| [bits-tier-3.cs](bits-tier-3.cs) | Tier 3 / 1000–9999 bits | Sends sanitized cheer text capped at 100 words to Mix It Up for paced readout. |
| [bits-tier-4.cs](bits-tier-4.cs) | Tier 4 / 10000+ bits | Sends sanitized cheer text capped at 10 words to Mix It Up for paced readout. |
| [gigantify-emote.cs](gigantify-emote.cs) | Automatic reward | Forwards gigantified emote metadata to Mix It Up; command ID placeholder must be replaced. |
| [message-effects.cs](message-effects.cs) | Automatic reward | Forwards viewer-entered message effect text to Mix It Up with paced readout. |
| [on-screen-celebration.cs](on-screen-celebration.cs) | Automatic reward | Forwards celebration reward metadata to Mix It Up with empty `Arguments`. |

## Action Contracts

Contracts for all 7 Twitch Bits Integration scripts live in [contracts.md](contracts.md).

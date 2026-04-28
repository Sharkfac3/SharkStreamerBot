---
id: trigger-catalog-phase-00-pre-seed
type: project-phase
description: Build manifest.json — full enumeration of every Streamer.bot trigger, its slug, and its upstream URL. No catalog files written yet.
status: active
owner: streamerbot-dev
phase: 0
---

# Phase 0 — Pre-seed Manifest

## Goal

Produce a single `Projects/streamerbot-trigger-catalog/manifest.json` that lists every Streamer.bot trigger across every platform with deterministic slugs and upstream URLs. Subsequent phases (skeleton, content fills) consume this manifest and never re-fetch nav.

**Out of scope this phase:** writing any catalog file under `Actions/Helpers/triggers/`, transcribing any args, editing any wiring file. Manifest only.

## Prerequisites

1. Read `Projects/streamerbot-trigger-catalog/README.md` in full — conventions, slug rules, manifest schema, exit rules.
2. Read [.agents/ENTRY.md](../../.agents/ENTRY.md) and [.agents/roles/streamerbot-dev/role.md](../../.agents/roles/streamerbot-dev/role.md).
3. Read [WORKING.md](../../WORKING.md) and follow coordination protocol before starting.

## Inputs (already known — bake directly into manifest, do not re-fetch)

### Top-level platforms (from https://docs.streamer.bot/api/triggers)

- Core (slug `core`)
- Custom (slug `custom`)
- Elgato (slug `elgato`)
- Integrations (slug `integrations`)
- Kick (slug `kick`)
- Meld Studio (slug `meld-studio`)
- OBS Studio (slug `obs-studio`)
- Streamlabs Desktop (slug `streamlabs-desktop`)
- Twitch (slug `twitch`)
- YouTube (slug `youtube`)

### Twitch subcategories (from https://docs.streamer.bot/api/triggers/twitch)

Ads, Channel, Channel Goal, Channel Reward, Charity, Chat, Community Goal, Connections, Emotes, General, Guest Star, Hype Train, Moderation, Polls, Predictions, Pyramid, Raid, Shared Chat, Subscriptions.

### Twitch Subscriptions trigger list (from https://docs.streamer.bot/api/triggers/twitch/subscriptions)

Gift Bomb, Gift Paid Upgrade, Gift Subscription, Pay It Forward, Prime Paid Upgrade, Resubscription, Sub Counter Rollover, Subscription.

(Use this as a reference example of the expected per-subcategory shape — bake the per-trigger upstream URL slugs by fetching the actual subscription pages or by inferring from the upstream nav. Confirm via WebFetch before locking.)

### Other platform subcategory lists (from https://docs.streamer.bot/api/triggers)

- **Core:** Uncategorized, Commands, File Folder Watcher, File I/O, Global Variables, Groups, Inputs, MIDI, Processes, Quotes, System, Voice Control, WebSocket.
- **Custom:** Uncategorized.
- **Elgato:** Camera Hub, Stream Deck, Wave Link.
- **Integrations:** Crowdcontrol, Donordrive, Fourthwall, HypeRate.io, Ko-Fi, Pally.gg, Patreon, Pulsoid, Shopify, Speaker.bot, StreamElements, Streamer.bot, Streamerbot Remote, Streamlabs, Tipeeestream, T.I.T.S., Treatstream, Voicemod, Vtube Studio.
- **Kick:** Channel, Channel Reward, Chat, Emotes, General, Kicks, Moderation, Subscriptions.
- **Meld Studio:** Uncategorized.
- **OBS Studio:** Uncategorized.
- **Streamlabs Desktop:** Uncategorized.
- **YouTube:** Broadcast, Chat, General, Membership, Polls.

## Steps

1. Read README conventions for slug rules and manifest schema.
2. For every (platform, subcategory) pair above, fetch the subcategory index page at `https://docs.streamer.bot/api/triggers/<platform-slug>/<subcategory-slug>`.
   - Adjust slug case if upstream uses a different scheme — verify by fetching one canary page per platform first. Lock the slug-derivation rule before bulk fetches.
3. From each fetched page, extract the list of trigger names (exactly as worded upstream) and their per-trigger URL slugs.
4. Build manifest entries per the schema in README.md.
5. Write `Projects/streamerbot-trigger-catalog/manifest.json`.
6. Set the top-level `fetched_at` field to today's ISO date.

## WebFetch guidance

- Use prompts that ask only for trigger names and link targets. Do not ask the model to summarize or include args — args are out of scope for this phase.
- Cache is 15 min; safe to re-fetch a page if your first parse looks thin.
- If a page redirects, follow the redirect once and use the canonical URL.
- If a subcategory page is missing or returns no trigger list, mark its `triggers` array as empty and add a `notes` field explaining (e.g. `"upstream page not found 404"`).

## Validation

Before exiting:

1. Manifest validates as JSON.
2. `platforms` length = 10 (per the inputs section above).
3. Every subcategory has `name`, `slug`, `upstream_url`, `triggers` array.
4. Every trigger has `name`, `slug`, `upstream_url`.
5. Slugs are kebab-case, lowercase, no spaces.
6. Spot-check 3 random triggers — open `upstream_url` in a fresh WebFetch and confirm the page loads and matches the trigger name.

## Exit

- Single file written: `Projects/streamerbot-trigger-catalog/manifest.json`.
- Dirty tree. Do not commit.
- Hand off via change summary noting: manifest path, platform count, total trigger count, any subcategories that returned no triggers (with the reason).

## Next phase

`01-skeleton.md` — scaffolds the catalog file tree from this manifest.

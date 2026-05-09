# Shared Constants

Purpose: keep cross-script names synchronized for Streamer.bot copy/paste workflows.

When you rename any value below, update **all listed scripts** before syncing to Streamer.bot.

---

## Index

| Domain | File | Contents |
|---|---|---|
| Stream Core | [constants/stream-core.md](constants/stream-core.md) | OBS scenes, stream mode, Disco Party |
| Mini-Games | [constants/mini-games.md](constants/mini-games.md) | Lock state, Duck, Clone Empire, Pedro, Toothless |
| Commanders | [constants/commanders.md](constants/commanders.md) | Commander slots, tenure, high-scores |
| Overlay / Broker | [constants/overlay-broker.md](constants/overlay-broker.md) | Connection state, topic strings |
| LotAT / Offering | [constants/lotat.md](constants/lotat.md) | Session state, roster, dice, offering |
| Effects | [constants/effects.md](constants/effects.md) | Rest/Focus loop, Bits, XJ Drivethrough, Destroyer, Info Service |

---

## Stream Core

OBS scene names, stream mode globals, and Disco Party state → [constants/stream-core.md](constants/stream-core.md)

---

## Mini-Games

Mini-game lock state and per-game globals (Duck, Clone Empire, Pedro, Toothless) → [constants/mini-games.md](constants/mini-games.md)

---

## Commanders

Commander slot globals, tenure counters, and high-score tracking → [constants/commanders.md](constants/commanders.md)

---

## Overlay / Broker

WebSocket connection state and overlay topic strings → [constants/overlay-broker.md](constants/overlay-broker.md)

---

## LotAT / Offering

Session state, roster, dice result, and offering globals → [constants/lotat.md](constants/lotat.md)

---

## Effects

Rest/Focus loop, Bits/unlock pacing, XJ Drivethrough, Destroyer, Info Service, Temporary → [constants/effects.md](constants/effects.md)

---

## Operator Sync Notes
1. Update constants in source files in this repo.
2. Paste updated scripts into Streamer.bot actions.
3. Run smoke tests:
   - stream-start reset,
   - stream mode switching (garage/workspace/gamer),
   - Duck start/call/resolve,
   - Clone start/position/volley,
   - Pedro start/mentions/unlock,
   - Toothless roll + first-time unlock,
   - Offering happy path + LotAT steal path,
   - Commander redeems + !hail/!thank/!award scoring for all three slots,
   - Bits tiers 1-4 cheer forwarding + wait behavior.

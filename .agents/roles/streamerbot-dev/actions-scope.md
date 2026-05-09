---
id: streamerbot-dev-actions-scope
type: scope
description: Task-ordered reading guide for streamerbot-dev working in Actions/.
owner: streamerbot-dev
parent: role.md
---

# streamerbot-dev — Actions Scope

Load files in the order that matches your task. You do not need to load everything upfront.

## Starting Any Actions Task

Always load first:
1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry and folder routing
2. The local `AGENTS.md` for the specific folder you are editing

## Writing or Editing a Script

After the above:
3. The relevant constants file(s) from the [Actions/SHARED-CONSTANTS.md](../../../Actions/SHARED-CONSTANTS.md) index — load only the domain you need
4. [Actions/Helpers/AGENTS.md](../../../Actions/Helpers/AGENTS.md) — check for reusable C# patterns
5. [Actions/Helpers/triggers/README.md](../../../Actions/Helpers/triggers/README.md) — canonical trigger args

## Validating or Updating a Contract

After the script step:
6. The local `contracts.md` for the folder you are in
7. [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) — contract format spec
8. Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`

## Domain Rules or Ownership Questions

- [Actions/RULES.md](../../../Actions/RULES.md) — all domain rules
- [Actions/OWNERSHIP.md](../../../Actions/OWNERSHIP.md) — role matrix and chain-to rules

## Folder Map

Load the local `AGENTS.md` for the folder you need. Load `contracts.md` when validating or changing runtime behavior.

| Folder | AGENTS.md | contracts.md |
|---|---|---|
| Twitch Core Integrations | [Twitch Core Integrations/AGENTS.md](../../../Actions/Twitch%20Core%20Integrations/AGENTS.md) | [Twitch Core Integrations/contracts.md](../../../Actions/Twitch%20Core%20Integrations/contracts.md) |
| Twitch Channel Points | [Twitch Channel Points/AGENTS.md](../../../Actions/Twitch%20Channel%20Points/AGENTS.md) | [Twitch Channel Points/contracts.md](../../../Actions/Twitch%20Channel%20Points/contracts.md) |
| Twitch Bits Integrations | [Twitch Bits Integrations/AGENTS.md](../../../Actions/Twitch%20Bits%20Integrations/AGENTS.md) | [Twitch Bits Integrations/contracts.md](../../../Actions/Twitch%20Bits%20Integrations/contracts.md) |
| Twitch Hype Train | [Twitch Hype Train/AGENTS.md](../../../Actions/Twitch%20Hype%20Train/AGENTS.md) | [Twitch Hype Train/contracts.md](../../../Actions/Twitch%20Hype%20Train/contracts.md) |
| Voice Commands | [Voice Commands/AGENTS.md](../../../Actions/Voice%20Commands/AGENTS.md) | [Voice Commands/contracts.md](../../../Actions/Voice%20Commands/contracts.md) |
| Commanders | [Commanders/AGENTS.md](../../../Actions/Commanders/AGENTS.md) | [Commanders/contracts.md](../../../Actions/Commanders/contracts.md) |
| Squad | [Squad/AGENTS.md](../../../Actions/Squad/AGENTS.md) | [Squad/contracts.md](../../../Actions/Squad/contracts.md) |
| LotAT | [LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) | [LotAT/contracts.md](../../../Actions/LotAT/contracts.md) |
| Overlay | [Overlay/AGENTS.md](../../../Actions/Overlay/AGENTS.md) | [Overlay/contracts.md](../../../Actions/Overlay/contracts.md) |
| Intros | [Intros/AGENTS.md](../../../Actions/Intros/AGENTS.md) | [Intros/contracts.md](../../../Actions/Intros/contracts.md) |
| Rest Focus Loop | [Rest Focus Loop/AGENTS.md](../../../Actions/Rest%20Focus%20Loop/AGENTS.md) | [Rest Focus Loop/contracts.md](../../../Actions/Rest%20Focus%20Loop/contracts.md) |
| Destroyer | [Destroyer/AGENTS.md](../../../Actions/Destroyer/AGENTS.md) | [Destroyer/contracts.md](../../../Actions/Destroyer/contracts.md) |
| XJ Drivethrough | [XJ Drivethrough/AGENTS.md](../../../Actions/XJ%20Drivethrough/AGENTS.md) | [XJ Drivethrough/contracts.md](../../../Actions/XJ%20Drivethrough/contracts.md) |
| Temporary | [Temporary/AGENTS.md](../../../Actions/Temporary/AGENTS.md) | [Temporary/contracts.md](../../../Actions/Temporary/contracts.md) |
| Helpers | [Helpers/AGENTS.md](../../../Actions/Helpers/AGENTS.md) | — |

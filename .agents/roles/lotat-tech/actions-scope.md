---
id: lotat-tech-actions-scope
type: scope
description: Scoped reading list for lotat-tech working in Actions/.
owner: lotat-tech
parent: role.md
---

# lotat-tech — Actions Scope

When working in Actions/, load in this order. Stop loading when you have what you need for the task.

## Required for All LotAT Work

1. [Actions/AGENTS.md](../../../Actions/AGENTS.md) — domain entry, folder routing
2. [Actions/LotAT/AGENTS.md](../../../Actions/LotAT/AGENTS.md) — LotAT rules, ownership, workflow, phase map
3. [Actions/LotAT/contracts.md](../../../Actions/LotAT/contracts.md) — all 12 LotAT script contracts (load when validating or updating a contract)

## Constants Relevant to LotAT

- [Actions/constants/lotat.md](../../../Actions/constants/lotat.md) — session state, roster, dice, offering globals
- [Actions/constants/overlay-broker.md](../../../Actions/constants/overlay-broker.md) — broker topics used by LotAT overlay publish scripts

## Do Not Load for LotAT Work

- Squad contracts, mini-game constants — not relevant to LotAT
- Commander constants — not used by LotAT runtime
- XJ Drivethrough, Destroyer, Rest/Focus constants — not relevant

## If You Need to Validate

See [Actions/CONTRACT-SCHEMA.md](../../../Actions/CONTRACT-SCHEMA.md) for contract format spec.
Run: `python3 Tools/StreamerBot/Validation/action_contracts.py --changed`

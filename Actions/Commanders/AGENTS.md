---
id: actions-commanders
type: domain-route
description: Streamer.bot commander role actions, support commands, state variables, paste targets, and brand handoffs.
owner: streamerbot-dev
secondaryOwners:
  - brand-steward
workflows:
  - change-summary
  - sync
  - validation
status: active
---

# Commanders — Agent Guide

## Purpose

This folder owns the Streamer.bot C# actions for the commander role system: Captain Stretch, The Director, Water Wizard, their channel-point redeems, their commander-only commands, their support commands, and shared commander help behavior.

Commander work prioritizes live-stream reliability, stable chat commands, and backward-compatible role assignment. The three commander slots can all be active at the same time.

## When to Activate

Use this guide when editing or reviewing files under [Actions/Commanders/](./), including:

- [Actions/Commanders/commander-help.cs](commander-help.cs)
- [Actions/Commanders/commanders.cs](commanders.cs)
- [Actions/Commanders/Captain Stretch/](Captain%20Stretch/) scripts
- [Actions/Commanders/The Director/](The%20Director/) scripts
- [Actions/Commanders/Water Wizard/](Water%20Wizard/) scripts
- Script Reference or operator documentation in this folder

## Primary Owner

`streamerbot-dev` owns the C# runtime behavior, Streamer.bot paste readiness, global variable use, command wiring expectations, and compatibility with existing channel-point and chat-command triggers.

## Secondary Owners / Chain To

Chain to `brand-steward` for any change to commander character name, role description, or reward copy.

## Required Reading

Read [Actions/AGENTS.md](../AGENTS.md) plus the relevant commander README before editing scripts:

- [Actions/constants/commanders.md](../constants/commanders.md) — canonical commander slot globals, support counters, cooldown vars, and high-score keys.
- [Actions/Commanders/Captain Stretch/AGENTS.md](Captain%20Stretch/AGENTS.md)
- [Actions/Commanders/The Director/AGENTS.md](The%20Director/AGENTS.md)
- [Actions/Commanders/Water Wizard/AGENTS.md](Water%20Wizard/AGENTS.md)
- [Creative/Brand/BRAND-VOICE.md](../../Creative/Brand/BRAND-VOICE.md) when public copy changes
- [Creative/Brand/CHARACTER-CODEX.md](../../Creative/Brand/CHARACTER-CODEX.md) when commander characterization changes

## Local Workflow

1. Identify the commander and trigger type: channel-point redeem, commander-only command, support command, or shared help.
2. Preserve the commander slot model: Captain Stretch, The Director, and Water Wizard are independent slots and may all be active simultaneously.
3. Preserve chat-facing command names unless the operator explicitly requests a rename.
4. Update [script-reference.md](script-reference.md) when trigger variables, command behavior, state variables, or operator wiring changes.

Commander script map:

| Commander | Scripts |
|---|---|
| Captain Stretch | [captain-stretch-redeem.cs](Captain%20Stretch/captain-stretch-redeem.cs), [captain-stretch-stretch.cs](Captain%20Stretch/captain-stretch-stretch.cs), [captain-stretch-shrimp.cs](Captain%20Stretch/captain-stretch-shrimp.cs), [captain-stretch-thank.cs](Captain%20Stretch/captain-stretch-thank.cs), [captain-stretch-generalfocus.cs](Captain%20Stretch/captain-stretch-generalfocus.cs) |
| The Director | [the-director-redeem.cs](The%20Director/the-director-redeem.cs), [the-director-checkchat.cs](The%20Director/the-director-checkchat.cs), [the-director-toad.cs](The%20Director/the-director-toad.cs), [the-director-award.cs](The%20Director/the-director-award.cs), [the-director-primary.cs](The%20Director/the-director-primary.cs), [the-director-secondary.cs](The%20Director/the-director-secondary.cs) |
| Water Wizard | [water-wizard-redeem.cs](Water%20Wizard/water-wizard-redeem.cs), [water-wizard-hail.cs](Water%20Wizard/water-wizard-hail.cs), [wizard-hydrate.cs](Water%20Wizard/wizard-hydrate.cs), [water-wizard-orb.cs](Water%20Wizard/water-wizard-orb.cs), [water-wizard-castrest.cs](Water%20Wizard/water-wizard-castrest.cs) |
| Shared helpers | [commander-help.cs](commander-help.cs), [commanders.cs](commanders.cs) |

Support command rules:

| Command | Commander | Counter variable |
|---|---|---|
| `!thank` | Captain Stretch | `captain_stretch_thank_count` |
| `!award` | The Director | `the_director_award_count` |
| `!hail` | Water Wizard | `water_wizard_hail_count` |

Active commanders cannot support themselves with their own support command. On commander redeem, compare the outgoing tenure counter to the persistent high score for that role.

## SHARED-STATE

Canonical commander names for the globals below live in [Actions/constants/commanders.md](../constants/commanders.md).

Current commander slot globals:

| Role | Active slot global |
|---|---|
| Captain Stretch | `current_captain_stretch` |
| The Director | `current_the_director` |
| Water Wizard | `current_water_wizard` |

Support tenure counters and persistent high-score keys:

| Role | Tenure counter | High score | High-score holder |
|---|---|---|---|
| Captain Stretch | `captain_stretch_thank_count` | `captain_stretch_thank_high_score` | `captain_stretch_thank_high_score_user` |
| The Director | `the_director_award_count` | `the_director_award_high_score` | `the_director_award_high_score_user` |
| Water Wizard | `water_wizard_hail_count` | `water_wizard_hail_high_score` | `water_wizard_hail_high_score_user` |

## Validation

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

## Boundaries / Out of Scope

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

## Handoff Notes

See Actions/AGENTS.md for universal validation, boundary, and handoff rules.

## Action Contracts

Contracts for all Commanders scripts live in [contracts.md](contracts.md). Load it when validating or updating a script contract.

## Script Reference

Detailed per-script documentation lives in [script-reference.md](script-reference.md). Load it when implementing or reviewing a specific commander script.

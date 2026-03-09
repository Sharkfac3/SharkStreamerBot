# Commanders Script Reference

## Scope
This folder contains commander slot assignment scripts and commander support command scripts.

## Model Rules
- Three commander slots exist:
  - Captain Stretch
  - The Director
  - Water Wizard
- All three commander slots can be active simultaneously.
- Redeem behavior should remain backward-compatible unless intentionally changed.

## Support Command Rules
- Chat can support active commanders with:
  - `!hail` (Water Wizard)
  - `!thank` (Captain Stretch)
  - `!award` (The Director)
- Active commander cannot support themselves with their own support command.
- Each support command increments a per-tenure counter.
- On commander redeem, outgoing tenure counter is compared to persistent high score for that role.

## Commander Docs
- `Captain Stretch/README.md`
- `The Director/README.md`
- `Water Wizard/README.md`

## Shared Constants
- Cross-script key sync reference: `Actions/SHARED-CONSTANTS.md`

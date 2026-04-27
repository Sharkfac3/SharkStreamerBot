---
id: canon-guardian
type: workflow
description: Shared canon review procedure for brand-level and LotAT-level continuity.
status: active
owner: brand-steward
appliesTo:
  - canon-review
  - lore-change
  - character-change
skills:
  - brand-steward
  - lotat-writer
terminal: false
path: .agents/workflows/canon-guardian.md
sourceOfTruth: true
---
# Workflow: canon-guardian

## Purpose

Use this workflow to review new or changed Starship Shamples canon before it becomes reusable project knowledge. It merges the former brand-steward and LotAT-specific canon guardian procedures into one shared path with role-specific notes.

The workflow protects three layers:

- Brand-level metaphor, tone, cast identity, and visual canon.
- LotAT adventure continuity, live-session pacing, world logic, and Chaos Meter behavior.
- Escalation boundaries for changes that require operator approval or updates to source canon documents.

## When to Run

Run this workflow when work includes any of the following:

- Adding a new character, NPC, recurring faction, ship section, space region, hazard, or permanent world element.
- Extending or modifying an existing character personality, role, relationship, metaphor mapping, or visual identity.
- Reviewing a generated or authored LotAT adventure for continuity before finalization.
- Proposing mechanics that affect how chat interacts with existing characters or story systems.
- Writing content that could make temporary adventure flavor look like franchise-level canon.

Do not use this workflow as a substitute for structural story validation, Streamer.bot runtime validation, or schema checks. Canon review is a content and continuity gate.

## Inputs

Required source references:

| Reference | Use |
|---|---|
| [Creative/Brand/BRAND-IDENTITY.md](../../Creative/Brand/BRAND-IDENTITY.md) | Brand metaphor, values, tone, and neurodivergent/ADHD guardrails. |
| [Creative/Brand/CHARACTER-CODEX.md](../../Creative/Brand/CHARACTER-CODEX.md) | Canonical cast identities, personalities, appearances, and behavioral constraints. |
| [Creative/WorldBuilding/Agents/D&D-Agent.md](../../Creative/WorldBuilding/Agents/D&D-Agent.md) | Established game mechanics, Chaos Meter framing, dice hooks, and adventure tone. |
| [Creative/WorldBuilding/Franchises/StarshipShamples.md](../../Creative/WorldBuilding/Franchises/StarshipShamples.md) | Franchise-level setting, tone, and recurring world elements. |
| [Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md](../../Creative/WorldBuilding/Experiments/StarshipShamples-story-agent.md) | LotAT story contract, story structure, and adventure generation rules. |

Task inputs should include the draft content, the intended publication/runtime location, and a short statement of whether the change is local flavor, reusable LotAT lore, or franchise-level canon.

## Procedure

1. Classify the canon layer.
   - Local adventure flavor: usable for a single story without updating franchise sources.
   - Reusable LotAT element: may recur across adventures and must fit established LotAT world logic.
   - Franchise-level canon: affects characters, metaphor, setting rules, or permanent world elements.
2. Check metaphor fit.
   - Identify what the element represents in the neurodivergent/ADHD metaphor.
   - Confirm it adds something the current cast or setting does not already represent.
   - Reject or revise anything that contradicts the existing metaphor layer.
3. Check tone and live-stream fit.
   - Keep the feel absurd, chaotic, slightly dramatic, fast-paced, humorous, and failure-forward.
   - Avoid darkness unless it clearly serves humor and stream energy.
   - Confirm the content is readable at live stream pace.
4. Check character ownership.
   - Water Wizard owns calm wisdom, hydration solutions, and festival-brain support.
   - Captain Stretch owns executive function, authority, and shrimp denial.
   - The Director owns committee consensus, executive process, and four-eyed calm.
   - Pedro owns well-meaning escalation and engineering chaos.
   - Duck owns hyperfocus rabbit holes and bad ideas that seem fine.
   - Clone owns random intrusive thoughts and unexplained presence.
   - Toothless owns threat response, security, and disproportionate reaction.
   - New recurring characters must occupy a distinct metaphor and story space.
5. Check visual and cast-size impact when relevant.
   - Confirm the silhouette, palette, and style do not collide with existing characters.
   - Prefer using an existing character when the current cast can carry the narrative function.
   - Treat new named cast members as operator-escalated changes.
6. Check LotAT story mechanics when reviewing adventure content.
   - Confirm pacing, stage beats, command expectations, and story fields match the current contract.
   - Keep commander moments rare, distinctive, and personality-specific.
   - Ensure the Chaos Meter escalates honestly and endings reflect final chaos state.
   - Avoid stale aliases or unsupported schema inventions.
7. Decide outcome.
   - Approve as written.
   - Approve with local-only framing.
   - Request revision.
   - Escalate to operator before merge/finalization.

## Validation / Done Criteria

Canon review is complete when all applicable checks below are satisfied:

- [ ] Required source references were consulted.
- [ ] The canon layer was classified as local flavor, reusable LotAT lore, or franchise-level canon.
- [ ] Existing characters remain consistent with the Character Codex.
- [ ] New or changed elements do not duplicate an existing character metaphor space.
- [ ] Tone remains absurd, chaotic, humorous, and stream-playable.
- [ ] LotAT story content respects pacing, current story contract fields, and Chaos Meter behavior.
- [ ] Permanent lore implications are explicitly approved or removed.
- [ ] Required escalations are listed in the handoff or change summary.

## Output / Handoff

Provide a short canon review note with:

- Reviewed files or draft sections.
- Canon layer classification.
- Decision: approved, approved local-only, revision requested, or operator escalation required.
- Specific edits or risks, especially Character Codex, franchise, or story-contract impacts.

If this workflow runs as part of a code or doc change, include the canon decision in the final [change-summary workflow](change-summary.md) output.

## Related Routes

- Brand Steward role: [brand-steward role overview](../roles/brand-steward/role.md)
- LotAT Writer role: [lotat-writer role overview](../roles/lotat-writer/role.md)
- Creative Brand domain target: creative-brand route.
- Creative WorldBuilding domain target: creative-worldbuilding route.
- Creative Art domain target: creative-art route.
- Validation workflow: [validation.md](validation.md)

## Role-Specific Notes

### Brand-level review

Brand-level canon review is required for new named cast members, character personality changes, metaphor remapping, permanent ship sections or locations, mechanics that change chat interaction, or anything requiring updates to the Character Codex.

Brand review may approve smaller additions without escalation when they are single-use regions, mission starting points, Chaos Meter flavor, dice-hook flavor text, or variant expressions of existing characters that do not alter canon.

### LotAT-specific review

LotAT review focuses on adventure continuity and live-session fit. It should confirm that new lore is temporary/local unless explicitly approved for reuse, that top-level cast usage matches the current contract, and that writer-side engine-safety validation has already passed.

Escalate from LotAT review to brand-level review when content adds or materially changes a recurring character, introduces permanent setting rules, changes metaphor mapping, or requires source canon updates.

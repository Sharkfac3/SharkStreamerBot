---
id: custom-intro-hardening-readme
type: project
description: Hardening pass plan for custom viewer intro runtime and operator flow.
owner: ops
status: active
---

# Custom Intro Hardening

## Goal

Harden the custom viewer intro flow around the agreed first-chat runtime design:

1. `first-chat-intro.cs` resolves enabled local assets and decides whether anything should run.
2. Streamer.bot action `Intros - Play Custom Intro` acts as a thin playback wrapper.
3. Mix It Up command `Custom Intro` performs sound and/or gif playback using passed-through variables.

## Scope

This project is focused on the first-chat intro playback path only.

In scope:
- `Actions/Intros/first-chat-intro.cs`
- `Actions/Intros/contracts.md`
- `Actions/Intros/AGENTS.md`
- `Tools/MixItUp/AGENTS.md`
- new project notes/prompts in this project folder

Out of scope:
- reward intake redesign
- app-side schema changes unless required by the hardening pass
- direct editing of live Mix It Up command configuration outside repo-tracked docs/checklists
- unrelated intro feature expansion

## Deliverables

- A current-state audit of what already matches the desired flow
- Repo changes that harden the Streamer.bot-side resolver contract
- Repo-tracked spec/checklist for the `Intros - Play Custom Intro` wrapper action
- Repo-tracked operator setup for the Mix It Up `Custom Intro` command
- Final validation + handoff notes

## Execution Model

Run the prompt files in `first-chat-prompts/` in numeric order. Each prompt is self-contained, but later prompts assume earlier prompts have already completed unless they explicitly say otherwise.

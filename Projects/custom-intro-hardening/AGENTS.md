---
id: custom-intro-hardening-agents
type: project-guide
description: Execution guide for the custom intro hardening project.
owner: ops
status: active
---

# Agent Guide — Custom Intro Hardening

## Purpose

This project breaks the first-chat custom intro hardening pass into small execution prompts that can be run in separate coding-agent sessions.

## Primary roles

- `streamerbot-dev` for runtime and contract work under [Actions/Intros/](../../Actions/Intros/)
- `ops` for Mix It Up setup docs, project tracking, and repo-facing operator checklists

## Hardening pass context

The target runtime flow is:

1. [Actions/Intros/first-chat-intro.cs](../../Actions/Intros/first-chat-intro.cs) resolves local intro assets and gates execution.
2. `Intros - Play Custom Intro` acts as a thin Streamer.bot wrapper.
3. Mix It Up command `Custom Intro` plays sound and/or gif based on passed-through variables.

Every prompt in this project must:
- first inspect what already exists
- identify the gap between current state and target flow
- then make only the changes required for that prompt's deliverable
- keep the broader hardening pass in mind and avoid duplicating work meant for later prompts

## Required reading for every prompt

1. `Projects/AGENTS.md`
2. `Projects/custom-intro-hardening/README.md`
3. [Projects/custom-intro-hardening/progress.md](./progress.md)
4. This file
5. Then the prompt file being executed

Also read any domain guides named by the prompt before editing.

## Progress tracking

Update [Projects/custom-intro-hardening/progress.md](./progress.md) as the last step of every prompt.

## Prompt output expectations

Each prompt should leave behind:
- the requested repo edits
- brief validation output in the final handoff
- any unresolved gaps clearly called out for the next prompt or operator

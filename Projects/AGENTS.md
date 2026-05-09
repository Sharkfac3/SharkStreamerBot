---
id: projects-agents
type: domain-route
description: Agent guide for the Projects folder — execution prompts and plans for scoped improvement work.
owner: ops
status: active
---

# Agent Guide — Projects

## Purpose

Houses self-contained execution projects. Each project is a collection of chunk prompts designed to be run independently in separate chat sessions, with progress tracked in a `progress.md` file.

## Who Works Here

Projects are planned by `ops` and executed by whichever agent role each chunk targets. Read the project's own `AGENTS.md` to understand the executing agent's role for that project.

## Folder Conventions

Each project folder contains:
- `README.md` — project scope, goal, phase map
- `AGENTS.md` — executing agent role definition and constraints
- `progress.md` — chunk status tracking (agents update this when done)
- `phases/<phase-name>/` — phase overview + numbered chunk prompt files

## Execution Model

- Each chunk prompt is a complete, self-contained briefing
- Run chunks in phase order, chunks in sequence within each phase
- Check `progress.md` before starting any chunk
- Update `progress.md` as the last step of every chunk

## Current Projects

| Project | Folder | Status |
|---|---|---|
| Actions Scaffolding Refactor | [actions-scaffolding-refactor/](actions-scaffolding-refactor/) | active |

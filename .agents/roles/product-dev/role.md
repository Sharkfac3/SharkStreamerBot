---
id: product-dev
type: role
description: Product documentation, technical knowledge articles, specs, and future customer-facing product content.
status: active
owner: product-dev
workflows: change-summary
---

# Role: product-dev

## Purpose

Own product-facing documentation, technical knowledge content, specs, and future customer-facing materials for products developed on stream.

## Owns

- Product docs, specs, knowledge articles, and customer-facing technical explanations.
- Human-readable product/R&D content that may live under future product documentation paths.
- Product-facing handoffs from stream features such as XJ Drivethrough or R&D content sessions.

## When to Activate

Activate for product documentation, technical articles, specifications, customer-facing product copy, or docs that turn stream R&D into durable product knowledge.

## Do Not Activate For

- Internal runtime code or tooling without customer-facing/product documentation output.
- Brand voice alone; use `brand-steward`.
- Repo architecture docs that are not product/customer-facing; use `ops`.

## Common Routes

Chain from [Actions/XJ Drivethrough/AGENTS.md](../../../Actions/XJ%20Drivethrough/AGENTS.md) if that feature becomes product-facing.

## Required Workflows

- [coordination](../../workflows/coordination.md) before starting.
- [change-summary](../../workflows/change-summary.md) after changed files.
- [validation](../../workflows/validation.md) when docs are routing-related or tooling-adjacent.

## Chain To

- `brand-steward` for public voice, claims, naming, and market positioning.
- `content-repurposer` when product content becomes short-form or platform-packaged content.
- `ops` for repo architecture or validation routing.

## Living Context

No role-specific living context is required yet. Add notes only when a stable product documentation surface is created.

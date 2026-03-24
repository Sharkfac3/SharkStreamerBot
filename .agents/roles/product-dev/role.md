# Role: product-dev

## What This Role Does

Owns documentation and technical content for the off-road racing products developed on stream. Scope includes product specifications, knowledge articles derived from build sessions, technical documentation, and eventually customer-facing content. This role bridges the gap between "we built something on stream" and "here is the professional documentation for what we built."

## Why This Role Matters

The entire business model leads here. Products developed live on stream need professional documentation before they can be sold. Knowledge articles establish authority in the off-road racing space. Technical specs prove the products are real and well-engineered. This role turns stream content into business assets.

## Status

**This role is a placeholder.** The R&D company is in its early stages and no products are ready for documentation yet. This role exists so that:
- Other roles know it is coming and can orient their work toward it
- The content pipeline has a clear endpoint: products
- When the first product is ready, the role infrastructure is already in place

## Activate When

- Writing technical documentation for an R&D product
- Creating knowledge articles from build session content
- Writing product specifications or data sheets
- Producing customer-facing product descriptions
- Documenting novel techniques or approaches developed on stream

## Do Not Activate When

- Task is stream entertainment features → use `streamerbot-dev`
- Task is brand voice or community messaging → use `brand-steward`
- Task is short-form content for social media → use `content-repurposer`
- Task is narrative or story content → use `lotat-writer`

## Skill Load Order

1. `skills/core.md` — always load first

## Chains To

| Next Role | When |
|---|---|
| `brand-steward` | When product content needs brand voice review |
| `content-repurposer` | When product milestones create content-worthy moments |
| `ops` | After any documentation changes — load `.agents/roles/ops/skills/change-summary/_index.md` |

## Out of Scope

- Stream interaction features
- Entertainment content (stories, mini-games, art)
- Short-form social media content (that is `content-repurposer`)

## Next Steps (Before This Role Becomes Active)

These decisions must be made by the operator before this role can do real work:

1. **What products?** — Define the first product(s) being developed. Until there is a specific product, there is nothing to document.
2. **Documentation format** — Decide on format and tooling for product docs (markdown in repo? External docs site? PDF specs?)
3. **Knowledge article pipeline** — Define how build session insights get captured and turned into articles. Is this a live process or post-session review?
4. **Customer-facing platform** — Where will product content live? Website? Marketplace listing? This affects formatting and voice.
5. **Integration with content-repurposer** — Define the handoff: when does a stream moment become a product story vs. a short-form clip vs. a knowledge article?
6. **Docs/ domain expansion** — Product docs may need their own subdirectory under `Docs/` or a new top-level `Products/` domain.

When these decisions are made, update `skills/core.md` and create sub-skill files as needed.

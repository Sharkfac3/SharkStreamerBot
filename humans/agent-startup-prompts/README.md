# Agent Startup Prompts

This folder contains copy/paste startup prompts for each scaffolded project role.

## How to use

1. Open the chat with your chosen AI agent.
2. Paste the contents of the role file that matches the work you want done.
3. After the agent loads context, give it the specific task.
4. If the task crosses role boundaries, either:
   - start with the primary role and let it chain to the next role in its output, or
   - start a fresh chat with the next role prompt.

## Included roles

- `streamerbot-dev.md`
- `lotat-tech.md`
- `lotat-writer.md`
- `art-director.md`
- `brand-steward.md`
- `content-repurposer.md`
- `app-dev.md`
- `product-dev.md`
- `ops.md`

## Notes

- These prompts are grounded in `.agents/ENTRY.md`, `.agents/_shared/*`, and each role's `role.md` + `skills/core.md`.
- They are written for humans to paste into general-purpose AI chats.
- They tell the agent what to read, what to avoid, and what success looks like inside this repo.
- For multi-chat migration or hardening work, see sibling prompt packs under `humans/` such as `humans/lotat-engine-session-spec/`.

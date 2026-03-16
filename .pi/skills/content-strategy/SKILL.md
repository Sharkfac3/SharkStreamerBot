---
name: content-strategy
description: Connects Starship Shamples story content to the real jeep/automotive build happening on stream. Load when planning stories around a specific build, designing stream content, or bridging the game world to real-world projects.
---

# Content Strategy

## When to Load

Load this skill for any task involving:
- Planning a Starship Shamples story tied to a specific real-world build or project
- Designing stream content that bridges the game and the actual build
- Writing stream titles or descriptions for a specific build session
- Planning what story arc to run during a multi-session project
- Thinking about how to introduce a new project to the community through the game

Do not load this for pure story generation without a real-world context. For that, use `creative-worldbuilding` directly.

## Required Reading

| Document | Why |
|---|---|
| `Creative/Brand/BRAND-IDENTITY.md` | The full brand context — especially the section on the relationship between the build and the game |
| `Creative/Brand/BRAND-VOICE.md` | How to talk about builds in stream titles, descriptions, and community posts |
| `Creative/Brand/CHARACTER-CODEX.md` | Character metaphor roles — for mirroring real build challenges in game events |

## The Core Principle: The Build IS the Mission

Starship Shamples is not a separate thing that happens on stream alongside the build. It is a live metaphor for the build itself.

| What is happening on stream | How it can appear in Starship Shamples |
|---|---|
| A new build is starting (first attempt at an untested approach) | Mission starting point: "suspicious anomaly," "strange new region of space" |
| A build step is going wrong | Pedro making things worse; Chaos Meter rising |
| A build step that looked easy turns complicated | Duck's drinks producing terrible ideas that seemed fine |
| A completely unexpected problem | Clone appearing with no explanation |
| Trying to contain a spiraling problem | Toothless responding to the Security Deck |
| Trying to get the build back on track | Commander moment for Captain Stretch |
| The build finishing (or partially finishing) | Mission ending — success, partial survival, or spectacular failure |

**This is not required.** Stories do not have to mirror the build directly. But the best stories will feel like they were written for the specific chaos of that stream session.

## Planning a Story for a Specific Build

When given context about an active or upcoming build, follow this process:

### Step 1: Understand the build
- What is being built or modified?
- What is the novel/untested aspect of this build?
- What is most likely to go wrong?
- What would success look like?
- What would partial success or spectacular failure look like?

### Step 2: Map the build to the game
- Which ship section(s) fit the build context? (Engineering for technical work, Cargo Bay for strange discoveries, Command Deck for major decisions)
- Which character's personality mirrors the build's main challenge? (Pedro for "good plan gone wrong," Captain Stretch for "maintaining authority while things slip")
- What space region fits the build's tone? (Screaming Asteroid Belt for chaotic mechanical work, Infinite Space Mall for sourcing parts, new region if needed)

### Step 3: Mirror failure modes
- What are the 2-3 most likely failure modes for this build?
- How do those map to story branching outcomes?
- At least one ending should reflect "we made it worse trying to fix it" — this is the most relatable outcome

### Step 4: Plan the story arc across sessions
For multi-session builds, plan story arcs that escalate:
- Session 1: Strange anomaly — something unexpected about the build (discovery mode)
- Mid sessions: Chaos escalates as problems compound (classic Pedro)
- Final session: The build either comes together or spectacular failure is documented — either is valid content

## Content Planning for Community Posts and Clips

### Before a build session
Communicate specifically what is being attempted. Frame it as honest uncertainty, not manufactured hype:
> "Trying [specific approach] on [part]. Haven't seen this done before. Brings us back on stream tonight."

### After a build session (success)
Keep it specific and real. The success is the unusual thing — treat it that way:
> "The [thing] worked. Documenting how in case it helps someone."

### After a build session (failure or setback)
The failure is content. Document it honestly:
> "[Thing] didn't work. Here's specifically why, and what we'll try instead."

Never apologize for failure. Never frame failure as a problem. It is the expected outcome and the most interesting content.

### Clips strategy
The best clips from a build stream are:
1. The moment something goes wrong (Pedro energy)
2. The moment a genuinely novel solution works (the crew succeeds)
3. Any moment where Captain Stretch / Water Wizard / The Director appears in chat or commander interactions that mirror the build energy

## Stream Title Formula

Good titles are specific + slightly absurd + honest:
- "[Specific thing being done] that [honest assessment of likelihood]"
- "The [part/system] issue gets [worse/fixed/complicated] | Starship Shamples"
- "[Build update after a setback]: [specific next step]"

Avoid:
- Vague titles ("Working on the Jeep | Starship Shamples")
- Clickbait ("YOU WON'T BELIEVE THIS JEEP BUILD")
- Over-apologetic titles ("Hopefully fixing the thing I broke last time")

## Skill Chain

```
content-strategy → creative-worldbuilding → (story JSON) → streamerbot-scripting + change-summary
```

For stream content planning without code:
```
content-strategy → brand-steward → (produce titles, descriptions, community posts)
```

# Session Format — Live Stream Pacing

## The Core Constraint

This content plays live on stream. Pacing must respect the stream environment:
- Narration competes with the streamer's own commentary
- Chat is reading and voting simultaneously
- High chaos = shorter, punchier text; lower chaos = slightly more room to breathe

## Node Narration Rules

- `read_aloud` field: **1–4 sentences maximum**
- Prefer 2–3 sentences for most stage nodes
- Lead with the action/event — do not bury the hook
- End stage narration on a decision point that makes both choices feel viable
- Ending narration can run slightly longer (up to 4 sentences) since no choice follows

## Choice Label Rules

- Keep choice labels short — 4–8 words ideal
- Action-oriented: start with a verb when possible
- Both choices should sound plausible (avoid obvious "good choice / bad choice" framing)
- The worse-sounding choice should sometimes be the better outcome — subvert expectations

## Pacing Across the Full Story

- Early stages (low chaos): slightly more exposition is okay
- Mid stages (rising chaos): narration gets tighter, choices get more urgent
- Late stages (high chaos): short, punchy, no time to breathe
- Endings: match the chaos level — high chaos endings can be slightly longer for payoff

## Chat Command Timing

- Live sessions now begin with an engine-run join phase where viewers opt in via `!join`
- `!join` is a runtime participation command, not a story-authored choice command
- Commands should feel organic to the narration — not bolted on
- A `!scan` command fits an investigation moment; `!deploy` fits a crew action
- Don't front-load the story with commands — distribute them across the arc
- Assume the engine may close a decision window early once every joined participant has submitted one of the currently allowed commands
- Invalid commands or runtime-only commands placed into authored choices are hard-fatal before handoff because they can break live engine behavior

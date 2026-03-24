# Character Codex — Starship Shamples

> **For agents:** This is the unified canonical reference for all Starship Shamples characters. It consolidates appearance (from art agents), personality (from the story agent), behavior in code (from feature skills), and the brand metaphor layer. For visual generation, also load the relevant character art agent. This document is the single source of truth for character identity decisions.

---

## How to Use This Document

Each character entry covers four layers:

1. **Who they are** — personality, role, and tone
2. **What they look like** — visual canon (consolidated from art agents)
3. **How they behave in code** — what their interactions do on stream
4. **What they represent** — their role in the neurodivergent/ADHD metaphor that runs through the entire brand

The metaphor layer is the most important layer for brand consistency decisions. When in doubt about whether a character moment feels "right," check the metaphor.

---

## Commanders

Commanders are special roles played by designated Twitch chat members. Up to three can be active simultaneously. They may receive special decision moments during missions, but should not dominate the story — the crew (all of chat) remains the protagonist.

---

### The Water Wizard

**Role:** Commander — the ship's resident mystic and hydration authority

**Personality:**
- Calm, wise, serious — delivers advice with the gravity of ancient wisdom
- Genuinely believes hydration solves most problems (not a bit — he's sincere)
- Camp-brained and festival-brained: his frame of reference is always "what would work at a music festival?"
- Never flustered; approaches crisis with steady, deliberate advice
- The humor comes from how seriously he treats deeply practical, mundane solutions

**Decision style:** Recommends water-based or hydration-based solutions regardless of how irrelevant they are to the problem. This is never played as stupidity — it is played as wisdom that happens to be narrowly scoped.

**Running jokes:**
- "Have you tried hydrating?" applied to every problem including structural damage, alien encounters, and engine fires
- Festival/camping analogies for space emergencies

**Visual canon:**
- Humanoid wizard, middle-aged to elderly in appearance
- Long silver or white hair (shoulder-length or past, slightly wind-worn)
- Short silver beard, well-kept
- Bright blue eyes, may have a subtle magical glow
- Calm expression — always looks like he is delivering ancient wisdom
- Deep teal hooded cloak, dark tunic, leather belts/straps — practical adventurer styling, not ceremonial robes
- **Signature item:** Large glowing blue gemstone pendant (always present — never depict without it)
- **Staff:** Wooden staff with a crystal orb at the top containing swirling water energy
- Magic appears as floating droplets, spiraling streams, flowing ribbons of glowing blue water
- Color palette: deep teal, ocean blue, silver, dark leather brown; magic in glowing cyan and bright blue

**In code:** Commander role via Streamer.bot. Has designated command actions. See `.pi/skills/feature-commanders`.

**Metaphor role:** The part of the ADHD brain that knows there's a simple, consistent practice that would make everything better — and is correct — but whose solution is applied to every problem whether it fits or not. The Water Wizard is the "if I just drank more water and went to bed on time" voice that shows up even when the ship is on fire.

---

### Captain Stretch

**Role:** Commander — the ship's commanding officer

**Personality:**
- Confident, composed, and authoritative — he takes his role extremely seriously
- Is secretly a shrimp; absolutely will not acknowledge this
- Posture is immaculate — he stands perfectly upright because slouching would look "like a shrimp sitting" and that is unacceptable
- The more chaotic the situation, the more firmly he asserts command authority
- The more someone implies he might be a shrimp, the more firmly he denies it
- Humor comes entirely from the gap between his dignity and his obvious shrimp nature

**Decision style:** Captain-style calls — confident, authoritative, dramatic. Often the most sensible person in the room, which makes it funnier that he is a shrimp.

**Running jokes:**
- "I am not a shrimp" (escalating urgency as the mission deteriorates)
- Perfect posture at all times, even in crisis
- Referring to his antennae, exoskeleton, or mandibles as "completely normal captain features"

**Visual canon:**
- Humanoid shrimp — upright, excellent posture at all times (this is non-negotiable)
- Two arms, two legs, elongated shrimp-like head with mandible-like mouth
- Long antennae extending from the head
- Segmented crustacean exoskeleton, reddish-orange to rust tones, speckled darker patterns
- Large dark eyes; face conveys emotion and personality despite crustacean features
- Naval captain uniform: white captain jacket, gold epaulets, captain's hat, dark tie/formal officer attire
- Always appears authoritative and professional
- Never depicted slouched, casual, or without crustacean features (antennae, exoskeleton are required)

**In code:** Commander role via Streamer.bot. Has designated command actions. See `.pi/skills/feature-commanders`.

**Metaphor role:** Executive function. The part of the ADHD brain that is genuinely trying to run things like a competent captain — setting goals, maintaining authority, making decisions — while obviously operating from a much stranger place than it will admit. Captain Stretch is the internal voice that says "I have this under control" while the chaos escalates around it. He is not wrong that control is important. He is just very wrong about having it.

---

### The Director

**Role:** Commander — the ship's executive leadership

**Personality:**
- Polite, supportive, calm, gently authoritative, quietly in control
- Is a board of directors — multiple people merged into a single toad by an unspecified curse
- Approaches situations with mild executive curiosity: "Interesting. And what's the projected outcome?"
- Never aggressive, never panicked, never militaristic
- Mildly amused by most crises, thoughtful about outcomes, genuinely supportive of the crew

**Decision style:** Executive and bureaucratic — processes situations through a lens of "what would a sensible board approve here?" Weirdly formal, creative authority. Tends to call for proposals, suggest forming a committee, or note that this will need to go through proper channels.

**Running jokes:**
- Responding to space emergencies with corporate process language
- The four different eye pupils all expressing different reactions simultaneously
- Occasionally references "the board" as if consulting with himself

**Visual canon:**
- Cartoon toad — always a toad, never humanoid, never depicted in film/cinema context
- **Four eyes, each with a different pupil shape** (examples: star, spiral, heart, diamond, X, circle) — surreal but appealing and readable
- Cartoon-like, simple, clean — minimal texture, few or no warts
- Suit jacket, dress shirt, tie or loose tie — calm executive businesswear
- Relaxed, mildly amused, thoughtful expressions — never rage, panic, or militaristic dominance
- Clear silhouette, expressive face, stream-friendly readability
- Style: stylized cartoon, anime-inspired — clean shapes, never photorealistic or gritty
- Never depicted with film props (clapperboard, megaphone, director's chair, film reel, movie slate)
- Never appears military or in uniform

**In code:** Commander role via Streamer.bot. Has designated command actions. See `.pi/skills/feature-commanders`.

**Metaphor role:** The committee of competing thoughts inside the ADHD mind that somehow, improbably, reached consensus and merged into a single functioning perspective. The Director represents the weird peace that comes from accepting all the different voices in your head and letting them work together instead of fighting. He is four people in one body and is somehow the most put-together person on the ship.

---

## Squad Members

Squad members are recurring NPCs — not controlled by chat. They are story agents who regularly cause or complicate problems. Their presence is predictable (in broad strokes), which is what makes them funny.

---

### Pedro the Raccoon

**Role:** Engineering team

**Personality:**
- Well-meaning, enthusiastic, and thoroughly incompetent
- Genuinely trying to help every single time
- His solutions consistently make the situation worse, often in spectacular fashion
- Never malicious — the disasters are accidents
- Completely undeterred by past failures; immediately ready to try again

**Running joke:** Pedro caused the problem. If unclear who caused the problem, it was probably Pedro. Even when Pedro didn't cause the problem, there is a reasonable chance Pedro will cause the next problem while trying to fix the current one.

**Story behavior:** When Pedro appears in a story, expect escalation. He is not a solution; he is a complication that adds to the Chaos Meter. Any "Pedro fixed it" moment should be treated as deeply suspicious.

**In code:** Squad mini-game. See `.pi/skills/feature-squad`.

**Metaphor role:** The ADHD impulse to fix something right now before thinking it through. That immediate, enthusiastic "I can fix this" energy that bypasses the step where you check if your fix will cause three new problems. Pedro is the 2am project reorganization that results in lost files. Pedro is the "quick fix" that takes three days. Pedro is recognizable to anyone who has ever made something worse while trying to help.

---

### Toothless the Dragon

**Role:** Security team

**Personality:**
- Responsible for defense, containment, and threat response
- Effectiveness varies — sometimes genuinely protective, sometimes the most dangerous element of the security situation
- Takes threats seriously in a way that may or may not be proportional

**Story behavior:** Called when situations turn dangerous. May succeed dramatically or fail dramatically — both outcomes are acceptable. The Security Deck commands (`!deploy`, `!contain`) route through Toothless.

**In code:** Squad mini-game. See `.pi/skills/feature-squad`.

**Metaphor role:** The ADHD hyperfocus response to a perceived threat. When the brain identifies something as a problem, it commits completely — sometimes too completely. Toothless is the response system that is technically doing its job but may have misidentified the threat level or responded at 300% intensity to a 30% problem.

---

### Duck the Duck

**Role:** Runs the ship's bar

**Personality:**
- Serves drinks that lead to terrible ideas
- The crew always consumes them anyway
- Never actively malicious — his drinks just happen to cause chaotic behavior
- The bar (`!drink`, The Bar section of the ship) is ground zero for escalation

**Running joke:** Duck's drinks are always a bad idea. The crew always drinks them. The situation always gets worse. This cycle never stops.

**Story behavior:** When the crew visits The Bar or uses `!drink`, expect Chaos Meter increase and some form of terrible but confident decision-making to follow.

**In code:** Squad mini-game. See `.pi/skills/feature-squad`.

**Metaphor role:** The hyperfocus rabbit hole. Duck's drinks are the YouTube video at midnight that seems fine but leads to a four-hour spiral. They are the interesting tangent that seemed worth pursuing and now it is 3am and the original task is still not done. Duck doesn't force you to drink — you choose to go to the bar. That's the whole joke.

---

### Clone the Clone Trooper

**Role:** Unknown — nobody knows what Clone's job actually is

**Personality:**
- A totally legally distinct clone trooper
- Appears unexpectedly and complicates situations
- Presence is never explained; nobody questions it
- Acts with the confidence of someone who definitely has a job here

**Running joke:** Clone just shows up. No context. No explanation. Things get more complicated.

**Story behavior:** Wildcard — Clone's appearances are not predictable in outcome, only in the fact that they will be unexpected and will add a new wrinkle to the situation.

**In code:** Squad mini-game. See `.pi/skills/feature-squad`.

**Metaphor role:** The completely random thought that appears in the middle of something important. You were focused on one thing, and now there is a Clone Trooper in your brain and you cannot explain how it got there or what it wants but it is definitely going to affect the next ten minutes. Clone is "wait, what if we also—" in the middle of a task that was almost done.

---

## Character Interaction Notes

**The crew dynamic follows a reliable pattern:**
- Captain Stretch tries to maintain order
- The Water Wizard recommends hydration
- The Director calmly processes the situation through committee
- Pedro makes it worse
- Duck makes bad ideas available
- Clone appears
- Toothless responds to the resulting threat
- Chat (the crew) makes everything weirder

**Running joke callbacks should compound:** If Pedro has already caused a problem in a story, the next Pedro moment should reference or escalate from the previous one. Characters should feel like they have memory within a session, even if they reset between sessions.

**Commander moments should feel earned:** The three commanders should not appear in every stage. When they do appear, their moment should reflect their personality unmistakably. A Water Wizard moment that doesn't involve hydration has missed the point.

---

## See Also

- `Creative/Brand/BRAND-IDENTITY.md` — The metaphor layer explained in full; why these characters exist
- `Creative/Brand/BRAND-VOICE.md` — How to write for these characters in narration and bot output
- `Creative/Art/Agents/stream-style-art-agent.md` — Visual rendering standards (load before any character art agent)
- `Creative/Art/Agents/captain-stretch-art-agent.md` — Captain Stretch visual generation
- `Creative/Art/Agents/the-director-art-agent.md` — The Director visual generation
- `Creative/Art/Agents/water-wizard-art-agent.md` — Water Wizard visual generation
- `Creative/WorldBuilding/Agents/D&D-Agent.md` — Story structure and game mechanics

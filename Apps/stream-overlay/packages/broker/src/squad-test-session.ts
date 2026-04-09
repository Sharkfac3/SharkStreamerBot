/**
 * squad-test-session.ts — Simulated squad game sessions for overlay testing
 *
 * Publishes complete test sessions for all four squad games through the broker
 * so you can verify each game's overlay renders correctly without needing a
 * live Streamer.bot instance.
 *
 * USAGE:
 *   # Terminal 1 — broker
 *   cd Apps/stream-overlay
 *   pnpm dev:broker
 *
 *   # Terminal 2 — overlay
 *   pnpm dev:overlay
 *   # Open http://localhost:5173?debug=true in browser
 *
 *   # Terminal 3 — this script
 *   cd Apps/stream-overlay/packages/broker
 *   npx ts-node --esm src/squad-test-session.ts
 *   # or: pnpm exec ts-node --esm src/squad-test-session.ts
 *
 * ─────────────────────────────────────────────────────────────────────────────
 * WHAT TO LOOK FOR AT EACH STEP:
 *
 * ── DUCK ──────────────────────────────────────────────────────────────────────
 *   Step D1  squad.duck.start
 *            ✓ Top-left panel appears (dark, semi-transparent)
 *            ✓ "DUCK GAME!" header in cyan
 *            ✓ Countdown timer starts at 2:00 (cyan, top-right of panel)
 *            ✓ "QUACKS: 0" and "QUACKERS: 0" labels visible
 *            ✓ Empty progress bar below counters
 *
 *   Step D2  squad.duck.update × 5
 *            ✓ QUACKS counter increments each update (0 → 10 → 25 → 50 → 80 → 110)
 *            ✓ QUACKERS counter increments (0 → 3 → 6 → 9 → 12 → 15)
 *            ✓ Progress bar fills proportionally (bar cap = 150 quacks)
 *
 *   Step D3  squad.duck.end (success)
 *            ✓ Timer stops
 *            ✓ "★ THRESHOLD REACHED! DUCK UNLOCKED! ★" in cyan
 *            ✓ Panel auto-hides after ~7 seconds
 *
 * ── PEDRO ─────────────────────────────────────────────────────────────────────
 *   Step P1  squad.pedro.start
 *            ✓ Top-left panel appears (replaces Duck panel if still visible)
 *            ✓ "PEDRO GAME!" header in hot pink
 *            ✓ Countdown timer starts at 2:00 (pink, top-right)
 *            ✓ "PEDROS: 0" and "(goal hidden)" labels visible
 *            ✓ Empty progress bar
 *
 *   Step P2  squad.pedro.update × 4
 *            ✓ PEDROS counter climbs (0 → 20 → 55 → 85 → 95)
 *            ✓ Progress bar fills (bar cap = 120 mentions)
 *
 *   Step P3  squad.pedro.end (failure)
 *            ✓ Timer stops
 *            ✓ "✗ ONLY 95 PEDROS. NOT ENOUGH." in red
 *            ✓ Panel auto-hides after ~7 seconds
 *
 * ── CLONE ─────────────────────────────────────────────────────────────────────
 *   Step C1  squad.clone.start
 *            ✓ Top-left panel appears (wider than Duck/Pedro)
 *            ✓ "CLONE GAME!" header in gold
 *            ✓ "ROUND 1" label beside header
 *            ✓ Five numbered boxes (POS 1–5) all showing "OPEN" in green
 *
 *   Step C2  squad.clone.update × 3 (volleys)
 *            After volley 1: POS 3 turns red, shows "× POS 3 ×" / "ELIMINATED"
 *                            ROUND 1 updates to ROUND 2
 *            After volley 2: POS 1 eliminated, ROUND 3
 *            After volley 3: POS 5 eliminated, ROUND 4
 *
 *   Step C3  squad.clone.end (win)
 *            ✓ POS 4 is eliminated (final volley)
 *            ✓ "★ CLONES WIN! SURVIVORS: sharkfac3, hydro99 ★" in gold
 *            ✓ Panel auto-hides after ~8 seconds
 *
 * ── TOOTHLESS ─────────────────────────────────────────────────────────────────
 *   Step T1  squad.toothless.start
 *            ✓ Small pop-up appears in upper-right corner
 *            ✓ "TOOTHLESS" label in purple
 *            ✓ "@sharkfac3" username in dim colour
 *            ✓ "rolling…" in grey while result is pending
 *
 *   Step T2  squad.toothless.end (flight — first unlock)
 *            ✓ "★ FLIGHT ★" appears in gold
 *            ✓ "✦ NEW UNLOCK! ✦" visible in party pink
 *            ✓ Pop-up auto-hides after ~8 seconds
 *
 *   Step T3  squad.toothless.start + end (regular — not first unlock)
 *            ✓ Pop-up shows "@wrenchmonkey"
 *            ✓ "★ REGULAR ★" in grey
 *            ✓ No "NEW UNLOCK!" text
 *            ✓ Auto-hides after ~8 seconds
 * ─────────────────────────────────────────────────────────────────────────────
 */

import { WebSocket } from 'ws';
import { randomUUID } from 'crypto';
import { BROKER_URL, TOPICS } from '@stream-overlay/shared';
import type { ClientHello, BrokerMessage } from '@stream-overlay/shared';

// ---------------------------------------------------------------------------
// Connection
// ---------------------------------------------------------------------------

const CLIENT_NAME = 'squad-test-session';

console.log('\n🎮 Squad Test Session — connecting to broker...\n');

const ws = new WebSocket(BROKER_URL);

ws.on('open', () => {
  const hello: ClientHello = {
    type: 'client.hello',
    name: CLIENT_NAME,
    subscriptions: [],
  };
  ws.send(JSON.stringify(hello));
});

ws.on('message', (data: WebSocket.RawData) => {
  const parsed = JSON.parse(data.toString()) as Record<string, unknown>;
  if (parsed['type'] === 'client.welcome') {
    console.log('✓ Connected to broker\n');
    runAllSessions().catch(console.error);
  }
});

ws.on('error', (err: Error) => {
  console.error('✗ Broker connection error:', err.message);
  console.error('  Is the broker running?  pnpm dev:broker');
  process.exit(1);
});

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function send(topic: string, payload: unknown): void {
  const msg: BrokerMessage = {
    id: randomUUID(),
    topic,
    sender: CLIENT_NAME,
    timestamp: Date.now(),
    payload,
  };
  ws.send(JSON.stringify(msg));
  console.log(`  → ${topic}`);
}

function wait(ms: number): Promise<void> {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

// ---------------------------------------------------------------------------
// Duck session
// ---------------------------------------------------------------------------

async function runDuckSession(): Promise<void> {
  console.log('═══ DUCK GAME ═══════════════════════════════════════════════════');

  console.log('\n── Step D1: duck.start ────────────────────────────────────────');
  send(TOPICS.SQUAD_DUCK_START, { game: 'duck', triggeredBy: 'sharkfac3' });
  await wait(1500);

  console.log('\n── Step D2: duck.update × 5 ───────────────────────────────────');
  const updates: Array<[number, number]> = [
    [10, 3], [25, 6], [50, 9], [80, 12], [110, 15],
  ];
  for (const [quackCount, uniqueQuackerCount] of updates) {
    send(TOPICS.SQUAD_DUCK_UPDATE, {
      game: 'duck',
      state: { quackCount, uniqueQuackerCount },
    });
    await wait(600);
  }

  await wait(800);

  console.log('\n── Step D3: duck.end (success) ────────────────────────────────');
  send(TOPICS.SQUAD_DUCK_END, {
    game: 'duck',
    result: { success: true, finalQuackCount: 110, uniqueQuackerCount: 15 },
  });

  // Wait for panel auto-hide (7s) + buffer before next game
  await wait(8000);
}

// ---------------------------------------------------------------------------
// Pedro session
// ---------------------------------------------------------------------------

async function runPedroSession(): Promise<void> {
  console.log('\n═══ PEDRO GAME ══════════════════════════════════════════════════');

  console.log('\n── Step P1: pedro.start ───────────────────────────────────────');
  send(TOPICS.SQUAD_PEDRO_START, { game: 'pedro', triggeredBy: 'hydro99' });
  await wait(1500);

  console.log('\n── Step P2: pedro.update × 4 ──────────────────────────────────');
  for (const mentionCount of [20, 55, 85, 95]) {
    send(TOPICS.SQUAD_PEDRO_UPDATE, {
      game: 'pedro',
      state: { mentionCount },
    });
    await wait(700);
  }

  await wait(800);

  console.log('\n── Step P3: pedro.end (failure) ───────────────────────────────');
  send(TOPICS.SQUAD_PEDRO_END, {
    game: 'pedro',
    result: { success: false, finalMentionCount: 95 },
  });

  await wait(8000);
}

// ---------------------------------------------------------------------------
// Clone session
// ---------------------------------------------------------------------------

async function runCloneSession(): Promise<void> {
  console.log('\n═══ CLONE GAME ══════════════════════════════════════════════════');

  console.log('\n── Step C1: clone.start ───────────────────────────────────────');
  send(TOPICS.SQUAD_CLONE_START, { game: 'clone', triggeredBy: 'wrenchmonkey' });
  await wait(2000);

  console.log('\n── Step C2: clone.update × 3 (volleys) ───────────────────────');
  // Volley 1 — eliminate position 3, move to round 2
  send(TOPICS.SQUAD_CLONE_UPDATE, {
    game: 'clone',
    state: { round: 2, positionsOpen: '1,2,4,5', eliminatedPosition: 3 },
  });
  await wait(2000);

  // Volley 2 — eliminate position 1, round 3
  send(TOPICS.SQUAD_CLONE_UPDATE, {
    game: 'clone',
    state: { round: 3, positionsOpen: '2,4,5', eliminatedPosition: 1 },
  });
  await wait(2000);

  // Volley 3 — eliminate position 5, round 4
  send(TOPICS.SQUAD_CLONE_UPDATE, {
    game: 'clone',
    state: { round: 4, positionsOpen: '2,4', eliminatedPosition: 5 },
  });
  await wait(2000);

  console.log('\n── Step C3: clone.end (win) ───────────────────────────────────');
  send(TOPICS.SQUAD_CLONE_END, {
    game: 'clone',
    result: {
      outcome: 'win',
      eliminatedPosition: 4,
      winners: 'sharkfac3,hydro99',
    },
  });

  await wait(9000);
}

// ---------------------------------------------------------------------------
// Toothless session
// ---------------------------------------------------------------------------

async function runToothlessSession(): Promise<void> {
  console.log('\n═══ TOOTHLESS GAME ══════════════════════════════════════════════');

  console.log('\n── Step T1+T2: toothless start → end (flight, first unlock) ───');
  send(TOPICS.SQUAD_TOOTHLESS_START, { game: 'toothless', triggeredBy: 'sharkfac3' });
  await wait(1000);  // brief "rolling…" state
  send(TOPICS.SQUAD_TOOTHLESS_END, {
    game: 'toothless',
    result: { rarity: 'flight', username: 'sharkfac3', isFirstUnlock: true },
  });

  await wait(9000);

  console.log('\n── Step T3: toothless start → end (regular, already unlocked) ─');
  send(TOPICS.SQUAD_TOOTHLESS_START, { game: 'toothless', triggeredBy: 'wrenchmonkey' });
  await wait(1000);
  send(TOPICS.SQUAD_TOOTHLESS_END, {
    game: 'toothless',
    result: { rarity: 'regular', username: 'wrenchmonkey', isFirstUnlock: false },
  });

  await wait(9000);
}

// ---------------------------------------------------------------------------
// Run all sessions in sequence
// ---------------------------------------------------------------------------

async function runAllSessions(): Promise<void> {
  await runDuckSession();
  await runPedroSession();
  await runCloneSession();
  await runToothlessSession();

  console.log('\n✓ All squad test sessions complete. Closing connection.\n');
  ws.close();
}

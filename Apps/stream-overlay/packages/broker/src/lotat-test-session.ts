/**
 * lotat-test-session.ts — Simulated LotAT session for overlay testing
 *
 * Publishes a complete mini LotAT session through the broker so you can
 * verify the overlay renders each stage correctly without needing a live
 * Streamer.bot instance.
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
 *   npx ts-node --esm src/lotat-test-session.ts
 *   # or: pnpm exec ts-node --esm src/lotat-test-session.ts
 *
 * WHAT TO LOOK FOR AT EACH STEP:
 *
 *   Step 1  lotat.session.start
 *           ✓ Dark lower-third panel appears at y=670
 *
 *   Step 2  lotat.join.open
 *           ✓ Join panel appears (centred, dark green border)
 *           ✓ "★ LEGENDS OF THE ASCII TEMPLE" title visible
 *           ✓ "Type !join to enlist" in cyan
 *           ✓ Countdown ticking from 2:00
 *
 *   Step 3  lotat.join.update × 3
 *           ✓ "CREW ENLISTED: N operators" updates each time
 *           ✓ Player names appear in the panel
 *
 *   Step 4  lotat.join.close
 *           ✓ Join panel disappears
 *
 *   Step 5  lotat.node.enter (stage node)
 *           ✓ "▸ COMMAND DECK" appears in info strip
 *           ✓ "CREW: The Water Wizard" appears in info strip (gold)
 *           ✓ Narration text appears in dark panel below strip
 *
 *   Step 6  lotat.chaos.update
 *           ✓ "CHAOS" label + bar appears centre-right of strip
 *           ✓ Bar fills to ~20% (4/20)
 *
 *   Step 7  lotat.vote.open
 *           ✓ Three choice cards appear in the choices zone
 *           ✓ Each card shows cyan command, green label, "VOTES: 0"
 *           ✓ Countdown timer appears in strip (top right)
 *
 *   Step 8  lotat.vote.cast × 3
 *           ✓ Vote counts update per card
 *
 *   Step 9  lotat.vote.close
 *           ✓ Winning card brightens / gold command text
 *           ✓ Losing cards dim
 *           ✓ Result flavour text replaces narration
 *
 *   Step 10 lotat.node.enter (ending node)
 *           ✓ Narration updates to ending text
 *           ✓ Choices zone cleared
 *
 *   Step 11 lotat.session.end
 *           ✓ "★ MISSION COMPLETE ★" ending screen appears (centred)
 *           ✓ Ending narration text below header
 *           ✓ All other LotAT panels hidden
 *           ✓ After 8 seconds the entire LotAT UI fades out
 */

import { WebSocket } from 'ws';
import { randomUUID } from 'crypto';
import { BROKER_URL, TOPICS } from '@stream-overlay/shared';
import type { ClientHello, BrokerMessage } from '@stream-overlay/shared';

// ---------------------------------------------------------------------------
// Connection
// ---------------------------------------------------------------------------

const SESSION_ID = randomUUID();
const CLIENT_NAME = 'lotat-test-session';

console.log('\n🎮 LotAT Test Session — connecting to broker...');
console.log(`   sessionId: ${SESSION_ID}\n`);

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
    runSession().catch(console.error);
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
// Test session
// ---------------------------------------------------------------------------

async function runSession(): Promise<void> {
  console.log('── Step 1: session.start ──────────────────────────────────────');
  send(TOPICS.LOTAT_SESSION_START, {
    sessionId: SESSION_ID,
    storyId:   'test-story-01',
    title:     'The Malfunctioning Warp Core',
  });

  await wait(1000);

  // ── Join phase ────────────────────────────────────────────────────────────

  console.log('\n── Step 2: join.open ──────────────────────────────────────────');
  send(TOPICS.LOTAT_JOIN_OPEN, {
    sessionId:     SESSION_ID,
    windowSeconds: 120,
  });

  await wait(1500);

  console.log('\n── Step 3: join.update × 3 ───────────────────────────────────');
  for (const [idx, username] of ['sharkfac3', 'hydro99', 'wrenchmonkey'].entries()) {
    send(TOPICS.LOTAT_JOIN_UPDATE, {
      sessionId:        SESSION_ID,
      username,
      participantCount: idx + 1,
    });
    await wait(700);
  }

  await wait(1000);

  console.log('\n── Step 4: join.close ─────────────────────────────────────────');
  send(TOPICS.LOTAT_JOIN_CLOSE, {
    sessionId:        SESSION_ID,
    participantCount: 3,
  });

  await wait(1000);

  // ── First story node ──────────────────────────────────────────────────────

  console.log('\n── Step 5: node.enter (stage) ─────────────────────────────────');
  send(TOPICS.LOTAT_NODE_ENTER, {
    sessionId:   SESSION_ID,
    nodeId:      'node_001',
    nodeType:    'stage',
    shipSection: 'Command Deck',
    title:       'Crisis at the Console',
    readAloud:
      'The emergency klaxon blares across the ship. ' +
      'Sparks rain from a shattered conduit above the main console. ' +
      'The warp core indicator flashes a deep, alarming red.',
    sfxHint:  'klaxon',
    crewFocus: { commander: 'The Water Wizard', squadMember: null },
    chaosDelta: 4,
    diceHook:   null,
    commanderMoment: null,
    choices: [
      { choiceId: 'c1', label: 'Scan for the fault source',  command: '!scan'    },
      { choiceId: 'c2', label: 'Reroute auxiliary power',    command: '!reroute' },
      { choiceId: 'c3', label: 'Hit the emergency stop',     command: '!stop'    },
    ],
    endState: null,
  });

  await wait(800);

  // ── Chaos update ──────────────────────────────────────────────────────────

  console.log('\n── Step 6: chaos.update ───────────────────────────────────────');
  send(TOPICS.LOTAT_CHAOS_UPDATE, {
    sessionId:  SESSION_ID,
    chaosTotal: 4,
    delta:      4,
  });

  await wait(1500);

  // ── Decision window ───────────────────────────────────────────────────────

  console.log('\n── Step 7: vote.open ──────────────────────────────────────────');
  send(TOPICS.LOTAT_VOTE_OPEN, {
    sessionId:        SESSION_ID,
    nodeId:           'node_001',
    choices: [
      { choiceId: 'c1', label: 'Scan for the fault source', command: '!scan'    },
      { choiceId: 'c2', label: 'Reroute auxiliary power',   command: '!reroute' },
      { choiceId: 'c3', label: 'Hit the emergency stop',    command: '!stop'    },
    ],
    windowSeconds:    120,
    participantCount: 3,
  });

  await wait(1500);

  console.log('\n── Step 8: vote.cast × 3 ─────────────────────────────────────');
  const votes = [
    { username: 'sharkfac3',   command: '!scan',    totals: { '!scan': 1, '!reroute': 0, '!stop': 0 }, count: 1 },
    { username: 'hydro99',     command: '!scan',    totals: { '!scan': 2, '!reroute': 0, '!stop': 0 }, count: 2 },
    { username: 'wrenchmonkey',command: '!reroute', totals: { '!scan': 2, '!reroute': 1, '!stop': 0 }, count: 3 },
  ];

  for (const v of votes) {
    send(TOPICS.LOTAT_VOTE_CAST, {
      sessionId:        SESSION_ID,
      nodeId:           'node_001',
      username:         v.username,
      command:          v.command,
      voteTotals:       v.totals,
      votedCount:       v.count,
      participantCount: 3,
    });
    await wait(800);
  }

  await wait(500);

  console.log('\n── Step 9: vote.close ─────────────────────────────────────────');
  send(TOPICS.LOTAT_VOTE_CLOSE, {
    sessionId:       SESSION_ID,
    nodeId:          'node_001',
    winningCommand:  '!scan',
    winningChoiceId: 'c1',
    resultFlavor:    'The crew initiates a deep scan. The warp core diagnostic reveals a cracked plasma conduit in sector seven.',
    voteTotals:      { '!scan': 2, '!reroute': 1, '!stop': 0 },
    nextNodeId:      'node_002',
  });

  await wait(3000);

  // ── Ending node ───────────────────────────────────────────────────────────

  console.log('\n── Step 10: node.enter (ending) ───────────────────────────────');
  send(TOPICS.LOTAT_NODE_ENTER, {
    sessionId:   SESSION_ID,
    nodeId:      'node_002',
    nodeType:    'ending',
    shipSection: 'Engineering Bay',
    title:       'The Conduit Holds',
    readAloud:
      'Engineering seals the fractured conduit in record time. ' +
      'The warp core stabilises and the klaxon falls silent. ' +
      'The crew breathes a collective sigh of relief as the stars blur back into streaks.',
    sfxHint:  'success-chime',
    crewFocus: { commander: 'The Water Wizard', squadMember: null },
    chaosDelta: 0,
    diceHook:   null,
    commanderMoment: null,
    choices:    [],
    endState:   'success',
  });

  await wait(2000);

  // ── Session end ───────────────────────────────────────────────────────────

  console.log('\n── Step 11: session.end ───────────────────────────────────────');
  send(TOPICS.LOTAT_SESSION_END, {
    sessionId: SESSION_ID,
    reason:    'ending-reached',
    endState:  'success',
  });

  console.log('\n✓ All messages sent.');
  console.log('  Watch the overlay — ending screen should display for ~8 seconds then fade.\n');

  // Keep connection open until overlay auto-hides
  await wait(10000);
  ws.close();
  process.exit(0);
}

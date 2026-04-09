/**
 * @stream-overlay/shared — Topic Constants
 *
 * The canonical source for all topic strings in the broker protocol.
 * Import from here everywhere — never hardcode topic strings inline.
 *
 * Naming convention:
 *   TOPICS.<NAMESPACE>_<SUBTOPIC>_<ACTION>
 *   e.g. TOPICS.LOTAT_VOTE_OPEN, TOPICS.OVERLAY_AUDIO_PLAY
 *
 * Wildcard subscriptions (for ClientHello.subscriptions):
 *   TOPICS.WILDCARD_<NAMESPACE>  →  "<namespace>.*"
 *   Single-level only: "lotat.*" matches "lotat.session.start"
 *   but NOT "lotat.visual.spawn" — subscribe to the nested namespace separately.
 */

export const TOPICS = {
  // ----------------------------------------------------------
  // SYSTEM
  // ----------------------------------------------------------
  /** Broker announces a new client completed handshake */
  SYSTEM_CLIENT_CONNECTED: "system.client.connected",
  /** Broker announces a client disconnected */
  SYSTEM_CLIENT_DISCONNECTED: "system.client.disconnected",
  /** Keepalive ping sent by any client */
  SYSTEM_PING: "system.ping",
  /** Keepalive pong in response to ping */
  SYSTEM_PONG: "system.pong",

  // ----------------------------------------------------------
  // OVERLAY — generic asset management
  // ----------------------------------------------------------
  /** Spawn a PNG/JPG/GIF asset at a canvas position */
  OVERLAY_SPAWN: "overlay.spawn",
  /** Move an existing asset to a new canvas position */
  OVERLAY_MOVE: "overlay.move",
  /** Apply or change an animation on an existing asset */
  OVERLAY_ANIMATE: "overlay.animate",
  /** Remove a specific asset from the canvas */
  OVERLAY_REMOVE: "overlay.remove",
  /** Remove all assets (or all matching a prefix filter) */
  OVERLAY_CLEAR: "overlay.clear",

  // ----------------------------------------------------------
  // OVERLAY — audio
  // ----------------------------------------------------------
  /** Play an MP3 through the Phaser audio system */
  OVERLAY_AUDIO_PLAY: "overlay.audio.play",
  /** Stop a playing sound by instance ID */
  OVERLAY_AUDIO_STOP: "overlay.audio.stop",

  // ----------------------------------------------------------
  // LOTAT — session lifecycle
  // Stage machine: idle → join_open → node_intro → [commander_open?]
  //   → [dice_open?] → decision_open → decision_resolving → ... → ended → idle
  // ----------------------------------------------------------
  /** Session initialized; join phase is about to open (idle → join_open) */
  LOTAT_SESSION_START: "lotat.session.start",
  /** Session fully torn down; overlay should clear all LotAT UI (ended → idle) */
  LOTAT_SESSION_END: "lotat.session.end",

  /** Join window opened; chat may type !join */
  LOTAT_JOIN_OPEN: "lotat.join.open",
  /** A viewer joined; participant count increased */
  LOTAT_JOIN_UPDATE: "lotat.join.update",
  /** Join phase closed; roster is now frozen */
  LOTAT_JOIN_CLOSE: "lotat.join.close",

  /** Engine entered a story node (node_intro stage) */
  LOTAT_NODE_ENTER: "lotat.node.enter",

  /** Decision window opened; joined participants may vote (decision_open) */
  LOTAT_VOTE_OPEN: "lotat.vote.open",
  /** A valid vote was cast (decision_open) */
  LOTAT_VOTE_CAST: "lotat.vote.cast",
  /** Voting closed; winner resolved (decision_resolving or ended) */
  LOTAT_VOTE_CLOSE: "lotat.vote.close",

  /** Dice roll window opened; any viewer may !roll (dice_open) */
  LOTAT_DICE_OPEN: "lotat.dice.open",
  /** A viewer used !roll (dice_open) */
  LOTAT_DICE_ROLL: "lotat.dice.roll",
  /** Dice window resolved (success or failure) */
  LOTAT_DICE_CLOSE: "lotat.dice.close",

  /** Commander moment window opened (commander_open) */
  LOTAT_COMMANDER_OPEN: "lotat.commander.open",
  /** Commander moment resolved (success or skipped) */
  LOTAT_COMMANDER_CLOSE: "lotat.commander.close",

  /** Chaos meter total changed (emitted on node entry when chaosDelta > 0) */
  LOTAT_CHAOS_UPDATE: "lotat.chaos.update",

  // ----------------------------------------------------------
  // SQUAD — mini-game events
  // Pattern: squad.<game>.(start|update|end)
  // ----------------------------------------------------------

  // Duck
  SQUAD_DUCK_START: "squad.duck.start",
  SQUAD_DUCK_UPDATE: "squad.duck.update",
  SQUAD_DUCK_END: "squad.duck.end",

  // Clone
  SQUAD_CLONE_START: "squad.clone.start",
  SQUAD_CLONE_UPDATE: "squad.clone.update",
  SQUAD_CLONE_END: "squad.clone.end",

  // Pedro
  SQUAD_PEDRO_START: "squad.pedro.start",
  SQUAD_PEDRO_UPDATE: "squad.pedro.update",
  SQUAD_PEDRO_END: "squad.pedro.end",

  // Toothless
  SQUAD_TOOTHLESS_START: "squad.toothless.start",
  SQUAD_TOOTHLESS_UPDATE: "squad.toothless.update",
  SQUAD_TOOTHLESS_END: "squad.toothless.end",

  // ----------------------------------------------------------
  // STREAM — raw Twitch platform events
  // ----------------------------------------------------------
  /** Viewer subscribed or resubscribed */
  STREAM_SUB: "stream.sub",
  /** Incoming raid */
  STREAM_RAID: "stream.raid",
  /** New follower */
  STREAM_FOLLOW: "stream.follow",
  /** Bits cheer */
  STREAM_BITS: "stream.bits",

  // ----------------------------------------------------------
  // WILDCARD SUBSCRIPTIONS
  // Use these in ClientHello.subscriptions for namespace-level subscriptions.
  // Single-level only: "lotat.*" catches "lotat.session.start" but NOT
  // "lotat.visual.spawn" — subscribe to nested namespaces separately.
  // ----------------------------------------------------------
  WILDCARD_SYSTEM: "system.*",
  WILDCARD_OVERLAY: "overlay.*",
  WILDCARD_OVERLAY_AUDIO: "overlay.audio.*",
  WILDCARD_LOTAT: "lotat.*",
  WILDCARD_SQUAD: "squad.*",
  WILDCARD_SQUAD_DUCK: "squad.duck.*",
  WILDCARD_SQUAD_CLONE: "squad.clone.*",
  WILDCARD_SQUAD_PEDRO: "squad.pedro.*",
  WILDCARD_SQUAD_TOOTHLESS: "squad.toothless.*",
  WILDCARD_STREAM: "stream.*",
} as const;

/** Union type of all valid topic strings — useful for runtime topic validation */
export type Topic = (typeof TOPICS)[keyof typeof TOPICS];

// ----------------------------------------------------------
// CONNECTION CONSTANTS
// ----------------------------------------------------------

/** WebSocket port the broker listens on */
export const BROKER_PORT = 8765 as const;

/** WebSocket URL for local broker connections */
export const BROKER_URL = `ws://localhost:${BROKER_PORT}` as const;

// ----------------------------------------------------------
// KNOWN CLIENT NAMES
// Use these in BrokerMessage.sender and ClientHello.name.
// ----------------------------------------------------------

export const CLIENT_NAMES = {
  /** Streamer.bot C# WebSocket client */
  STREAMERBOT: "streamerbot",
  /** Phaser OBS browser source overlay */
  OVERLAY: "overlay",
} as const;

export type ClientName = (typeof CLIENT_NAMES)[keyof typeof CLIENT_NAMES];

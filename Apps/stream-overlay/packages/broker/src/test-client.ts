/**
 * @stream-overlay/broker — Test Client
 *
 * Development/debugging tool only. Not part of the production broker.
 *
 * Usage:
 *   pnpm test-client
 *
 * What it does:
 *   - Connects to the broker at BROKER_URL
 *   - Sends ClientHello and subscribes to all topics
 *   - Prints every message it receives
 *   - Accepts typed input to publish test messages
 *
 * Input format (type and press Enter):
 *   <topic> <json-payload>
 *
 * Examples:
 *   overlay.spawn {"assetId":"test","src":"test.png","position":{"x":100,"y":100}}
 *   lotat.session.start {"sessionId":"abc","storyId":"story-01","title":"Test Session"}
 *   system.ping {}
 *
 * Press Ctrl+C to exit.
 */

import { WebSocket } from "ws";
import { createInterface } from "readline";
import { randomUUID } from "crypto";
import { BROKER_URL, TOPICS } from "@stream-overlay/shared";
import type { ClientHello, BrokerMessage } from "@stream-overlay/shared";

const CLIENT_NAME = "test-client";

// All wildcard subscriptions — receives everything flowing through the broker
const ALL_SUBSCRIPTIONS = [
  TOPICS.WILDCARD_SYSTEM,
  TOPICS.WILDCARD_OVERLAY,
  TOPICS.WILDCARD_OVERLAY_AUDIO,
  TOPICS.WILDCARD_LOTAT,
  TOPICS.WILDCARD_SQUAD,
  TOPICS.WILDCARD_STREAM,
];

console.log(`\nStream Overlay — Broker Test Client`);
console.log(`Connecting to ${BROKER_URL} as "${CLIENT_NAME}"...`);
console.log(`Subscribing to: ${ALL_SUBSCRIPTIONS.join(", ")}\n`);

const ws = new WebSocket(BROKER_URL);

ws.on("open", () => {
  const hello: ClientHello = {
    type: "client.hello",
    name: CLIENT_NAME,
    subscriptions: ALL_SUBSCRIPTIONS,
  };
  ws.send(JSON.stringify(hello));
  console.log("[connected] Sent ClientHello. Waiting for welcome...\n");
});

ws.on("message", (data: WebSocket.RawData) => {
  let parsed: unknown;
  try {
    parsed = JSON.parse(data.toString());
  } catch {
    console.log("[raw]", data.toString());
    return;
  }

  if (isObject(parsed) && parsed["type"] === "client.welcome") {
    console.log(
      `[welcome] clientId=${parsed["clientId"]}  connected peers: [${(parsed["connectedClients"] as string[]).join(", ") || "none"}]`,
    );
    console.log(
      `\nListening. Type messages to publish (format: <topic> <json>):\n`,
    );
    startReadline();
    return;
  }

  // Pretty-print all received BrokerMessages
  const msg = parsed as BrokerMessage;
  const ts = new Date(msg.timestamp ?? Date.now()).toISOString().slice(11, 23);
  console.log(`[${ts}] ${msg.topic ?? "?"} from "${msg.sender ?? "?"}"`);
  if (msg.payload && Object.keys(msg.payload as object).length > 0) {
    console.log("         ", JSON.stringify(msg.payload));
  }
});

ws.on("close", () => {
  console.log("\n[disconnected] Connection closed.");
  process.exit(0);
});

ws.on("error", (err: Error) => {
  console.error("[error]", err.message);
  process.exit(1);
});

// ---------------------------------------------------------------------------
// Readline input — publish test messages
// ---------------------------------------------------------------------------

function startReadline(): void {
  const rl = createInterface({
    input: process.stdin,
    output: process.stdout,
    prompt: "> ",
  });

  rl.prompt();

  rl.on("line", (line: string) => {
    const trimmed = line.trim();
    if (!trimmed) {
      rl.prompt();
      return;
    }

    // Split on first space: "<topic> <payload-json>"
    const spaceIdx = trimmed.indexOf(" ");
    const topic = spaceIdx === -1 ? trimmed : trimmed.slice(0, spaceIdx);
    const payloadRaw = spaceIdx === -1 ? "{}" : trimmed.slice(spaceIdx + 1);

    let payload: unknown;
    try {
      payload = JSON.parse(payloadRaw);
    } catch {
      console.log(`  [error] Invalid JSON payload: ${payloadRaw}`);
      rl.prompt();
      return;
    }

    const message: BrokerMessage = {
      id: randomUUID(),
      topic,
      sender: CLIENT_NAME,
      timestamp: Date.now(),
      payload,
    };

    ws.send(JSON.stringify(message));
    console.log(`  [sent] ${topic}`);
    rl.prompt();
  });

  rl.on("close", () => {
    console.log("\nExiting.");
    ws.close();
    process.exit(0);
  });
}

// ---------------------------------------------------------------------------

function isObject(val: unknown): val is Record<string, unknown> {
  return typeof val === "object" && val !== null && !Array.isArray(val);
}

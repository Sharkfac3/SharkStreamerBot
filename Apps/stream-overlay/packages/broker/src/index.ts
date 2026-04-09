// @stream-overlay/broker — WebSocket message broker entry point.
//
// The broker is the central hub of the stream overlay ecosystem:
//   - Accepts WebSocket connections from all clients (Streamer.bot, overlay, future apps)
//   - Routes messages by topic to subscribed clients
//   - Publishes system events (client.connected, client.disconnected, pong)
//   - Exposes a health check endpoint at GET /health
//
// Contains NO business logic. It does not interpret messages — only routes them.
// Business logic lives in Streamer.bot scripts (Actions/) or in the clients themselves.

import { ClientRegistry } from "./client-registry.js";
import { createBrokerServer } from "./server.js";

const registry = new ClientRegistry();
const broker = createBrokerServer(registry);

broker.listen();

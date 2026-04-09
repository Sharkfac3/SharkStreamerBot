import { createServer } from "http";
import { WebSocketServer, WebSocket } from "ws";
import { randomUUID } from "crypto";
import { TOPICS, type BrokerMessage, type ClientHello, type ClientWelcome } from "@stream-overlay/shared";
import type { ClientRegistry } from "./client-registry.js";
import { routeMessage } from "./message-router.js";
import { logger } from "./logger.js";
import { config } from "./config.js";

export function createBrokerServer(registry: ClientRegistry) {
  const startTime = Date.now();

  // One HTTP server handles both WebSocket upgrades and the health check endpoint.
  const httpServer = createServer((req, res) => {
    if (req.method === "GET" && req.url === "/health") {
      const body = JSON.stringify(
        {
          status: "ok",
          uptime: Math.floor((Date.now() - startTime) / 1000),
          clients: registry.all().map((c) => ({
            name: c.name,
            subscriptions: c.subscriptions,
          })),
        },
        null,
        2,
      );
      res.writeHead(200, { "Content-Type": "application/json" });
      res.end(body);
      return;
    }
    res.writeHead(404);
    res.end("Not found");
  });

  const wss = new WebSocketServer({ server: httpServer });

  wss.on("connection", (socket: WebSocket) => {
    // Temporary ID used in logs before the client sends ClientHello
    const tempId = randomUUID();
    logger.info(`New connection (awaiting hello) [${tempId}]`);

    // Populated once the client completes the handshake
    let clientId: string | null = null;

    socket.on("message", (data) => {
      // --- Parse raw bytes to string ---
      let raw: string;
      try {
        raw = data.toString();
      } catch {
        logger.error(`Could not decode message data [${clientId ?? tempId}]`);
        return;
      }

      // --- Parse JSON ---
      let parsed: unknown;
      try {
        parsed = JSON.parse(raw);
      } catch {
        logger.error(
          `Malformed JSON from [${clientId ?? tempId}]:`,
          raw.slice(0, 200),
        );
        return;
      }

      if (!isObject(parsed)) {
        logger.error(`Non-object message from [${clientId ?? tempId}]`);
        return;
      }

      // --- ClientHello (handshake frame — not a BrokerMessage) ---
      if (parsed["type"] === "client.hello") {
        if (!isClientHello(parsed)) {
          logger.error(
            `Invalid ClientHello from [${tempId}]:`,
            raw.slice(0, 200),
          );
          return;
        }
        clientId = handleHello(parsed as unknown as ClientHello, socket, registry);
        return;
      }

      // --- All other messages must be BrokerMessages ---
      if (!isBrokerMessage(parsed)) {
        logger.error(
          `Invalid BrokerMessage from [${clientId ?? tempId}]:`,
          raw.slice(0, 200),
        );
        return;
      }

      const message = parsed as unknown as BrokerMessage;

      // --- system.ping → broker publishes system.pong ---
      if (message.topic === TOPICS.SYSTEM_PING) {
        const pong: BrokerMessage = {
          id: randomUUID(),
          topic: TOPICS.SYSTEM_PONG,
          sender: "broker",
          timestamp: Date.now(),
          payload: { echo: (message.payload as Record<string, unknown>)?.["echo"] },
        };
        routeMessage(pong, registry);
        return;
      }

      // --- Route all other messages to subscribers ---
      routeMessage(message, registry);
    });

    socket.on("close", () => {
      if (clientId !== null) {
        const client = registry.remove(clientId);
        if (client) {
          logger.info(`Client disconnected: "${client.name}" [${clientId}]`);
          const msg: BrokerMessage = {
            id: randomUUID(),
            topic: TOPICS.SYSTEM_CLIENT_DISCONNECTED,
            sender: "broker",
            timestamp: Date.now(),
            payload: { clientId, clientName: client.name },
          };
          routeMessage(msg, registry);
        }
      } else {
        logger.info(`Connection closed before hello [${tempId}]`);
      }
    });

    socket.on("error", (err: Error) => {
      logger.error(`Socket error [${clientId ?? tempId}]:`, err.message);
    });
  });

  return {
    listen(): void {
      httpServer.listen(config.port, () => {
        logger.info(`Broker listening   ws://localhost:${config.port}`);
        logger.info(`Health check       http://localhost:${config.port}/health`);
        logger.info(`Log level          ${config.logLevel}`);
      });
    },
  };
}

// ---------------------------------------------------------------------------
// Handshake handler
// ---------------------------------------------------------------------------

function handleHello(
  hello: ClientHello,
  socket: WebSocket,
  registry: ClientRegistry,
): string {
  const clientId = randomUUID();
  const existingNames = registry.names(); // snapshot before adding

  registry.add({
    id: clientId,
    name: hello.name,
    socket,
    subscriptions: hello.subscriptions,
    connectedAt: Date.now(),
  });

  logger.info(
    `Client registered: "${hello.name}" [${clientId}] subs=[${hello.subscriptions.join(", ")}]`,
  );

  // Send ClientWelcome back to the connecting client
  const welcome: ClientWelcome = {
    type: "client.welcome",
    clientId,
    connectedClients: existingNames,
  };
  socket.send(JSON.stringify(welcome));

  // Notify all OTHER subscribers that a new client connected
  const connectMsg: BrokerMessage = {
    id: randomUUID(),
    topic: TOPICS.SYSTEM_CLIENT_CONNECTED,
    sender: "broker",
    timestamp: Date.now(),
    payload: { clientId, clientName: hello.name },
  };
  routeMessage(connectMsg, registry, clientId); // exclude the new client itself

  return clientId;
}

// ---------------------------------------------------------------------------
// Type guards
// ---------------------------------------------------------------------------

function isObject(val: unknown): val is Record<string, unknown> {
  return typeof val === "object" && val !== null && !Array.isArray(val);
}

function isClientHello(val: Record<string, unknown>): boolean {
  return (
    val["type"] === "client.hello" &&
    typeof val["name"] === "string" &&
    Array.isArray(val["subscriptions"]) &&
    (val["subscriptions"] as unknown[]).every((s) => typeof s === "string")
  );
}

function isBrokerMessage(val: Record<string, unknown>): boolean {
  return (
    typeof val["id"] === "string" &&
    typeof val["topic"] === "string" &&
    typeof val["sender"] === "string" &&
    typeof val["timestamp"] === "number" &&
    "payload" in val
  );
}

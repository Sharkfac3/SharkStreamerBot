import { WebSocket } from "ws";
import type { BrokerMessage } from "@stream-overlay/shared";
import type { ClientRegistry } from "./client-registry.js";
import { topicMatches } from "./topic-matcher.js";
import { logger } from "./logger.js";

/**
 * Deliver a message to all clients subscribed to its topic.
 *
 * @param message        The message to route.
 * @param registry       The connected client registry.
 * @param excludeClientId  If provided, skip delivery to this client ID.
 *                         Used when the broker publishes system.client.connected
 *                         so the newly connected client doesn't receive its own
 *                         announcement (per protocol spec).
 */
export function routeMessage(
  message: BrokerMessage,
  registry: ClientRegistry,
  excludeClientId?: string,
): void {
  const raw = JSON.stringify(message);
  let delivered = 0;

  for (const client of registry.all()) {
    if (client.id === excludeClientId) continue;

    const isSubscribed = client.subscriptions.some((sub) =>
      topicMatches(sub, message.topic),
    );

    if (isSubscribed && client.socket.readyState === WebSocket.OPEN) {
      client.socket.send(raw);
      delivered++;
    }
  }

  logger.debug(
    `Routed [${message.topic}] from "${message.sender}" → ${delivered} client(s)`,
  );
}

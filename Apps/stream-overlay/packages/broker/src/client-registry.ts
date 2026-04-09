import type { WebSocket } from "ws";

export interface ConnectedClient {
  /** Broker-assigned UUID for this connection session */
  id: string;
  /** Stable human-readable name from ClientHello (e.g. "streamerbot", "overlay") */
  name: string;
  socket: WebSocket;
  subscriptions: string[];
  connectedAt: number;
}

export class ClientRegistry {
  private readonly clients = new Map<string, ConnectedClient>();

  add(client: ConnectedClient): void {
    this.clients.set(client.id, client);
  }

  remove(clientId: string): ConnectedClient | undefined {
    const client = this.clients.get(clientId);
    this.clients.delete(clientId);
    return client;
  }

  get(clientId: string): ConnectedClient | undefined {
    return this.clients.get(clientId);
  }

  all(): ConnectedClient[] {
    return Array.from(this.clients.values());
  }

  /** Returns the human-readable names of all connected clients */
  names(): string[] {
    return this.all().map((c) => c.name);
  }

  size(): number {
    return this.clients.size;
  }
}

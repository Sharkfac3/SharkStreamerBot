/**
 * BrokerClient — WebSocket connection to the message broker.
 *
 * Responsibilities:
 *   - Connect to the broker at BROKER_URL on construction
 *   - Send ClientHello and handle ClientWelcome
 *   - Reconnect automatically with exponential backoff when the connection drops
 *   - Send system.ping keepalives every 15 seconds while connected
 *   - Expose on(topic, handler) for exact-topic subscriptions
 *   - Expose onAny(handler) for catching all incoming messages
 *   - Expose onStatus(handler) for tracking connection state
 *
 * The overlay subscribes to all rendering-relevant topic namespaces in its
 * ClientHello. By the time messages arrive here, the broker has already matched
 * wildcard subscriptions — all incoming messages carry concrete topic strings.
 * Topic routing in this module is therefore exact-match only.
 *
 * This module exports a singleton `brokerClient`. Phaser scenes import it
 * directly — no wiring through Phaser's registry is needed.
 */

import {
  BROKER_URL,
  CLIENT_NAMES,
  TOPICS,
  type BrokerMessage,
  type ClientWelcome,
} from '@stream-overlay/shared';

export type BrokerStatus = 'connecting' | 'connected' | 'disconnected';

type MessageHandler = (message: BrokerMessage) => void;
type StatusHandler = (status: BrokerStatus) => void;

const RECONNECT_BASE_MS = 1_000;
const RECONNECT_MAX_MS = 30_000;
const PING_INTERVAL_MS = 15_000;

// Wildcard key used internally by onAny() — not a real topic string
const ANY_TOPIC_KEY = '__any__';

export class BrokerClient {
  private ws: WebSocket | null = null;
  private reconnectAttempt = 0;
  private reconnectTimer: ReturnType<typeof setTimeout> | null = null;
  private pingTimer: ReturnType<typeof setInterval> | null = null;
  private destroyed = false;

  private readonly topicHandlers = new Map<string, Set<MessageHandler>>();
  private readonly statusHandlers = new Set<StatusHandler>();
  private _status: BrokerStatus = 'disconnected';

  constructor() {
    this.connect();
  }

  get status(): BrokerStatus {
    return this._status;
  }

  /**
   * Subscribe to messages with an exact topic string.
   * Returns an unsubscribe function — call it to remove the handler.
   *
   * @example
   * const unsub = brokerClient.on(TOPICS.OVERLAY_SPAWN, (msg) => { ... });
   * // later:
   * unsub();
   */
  on(topic: string, handler: MessageHandler): () => void {
    if (!this.topicHandlers.has(topic)) {
      this.topicHandlers.set(topic, new Set());
    }
    this.topicHandlers.get(topic)!.add(handler);
    return () => this.topicHandlers.get(topic)?.delete(handler);
  }

  /**
   * Subscribe to ALL incoming BrokerMessages, regardless of topic.
   * Useful for debug logging. Returns an unsubscribe function.
   */
  onAny(handler: MessageHandler): () => void {
    return this.on(ANY_TOPIC_KEY, handler);
  }

  /**
   * Subscribe to broker connection status changes.
   * Called immediately on the next status change — does not replay current status.
   * Returns an unsubscribe function.
   */
  onStatus(handler: StatusHandler): () => void {
    this.statusHandlers.add(handler);
    return () => this.statusHandlers.delete(handler);
  }

  /** Tear down this client — closes the socket, cancels all timers. */
  destroy(): void {
    this.destroyed = true;
    this.clearTimers();
    this.ws?.close();
    this.ws = null;
  }

  // --------------------------------------------------------------------------
  // Private — connection lifecycle
  // --------------------------------------------------------------------------

  private connect(): void {
    if (this.destroyed) return;
    this.setStatus('connecting');

    try {
      this.ws = new WebSocket(BROKER_URL);
    } catch (err) {
      console.error('[BrokerClient] WebSocket construction failed:', err);
      this.scheduleReconnect();
      return;
    }

    this.ws.addEventListener('open', () => {
      this.reconnectAttempt = 0;
      this.sendHello();
    });

    this.ws.addEventListener('message', (event: MessageEvent<string>) => {
      this.handleMessage(event.data);
    });

    this.ws.addEventListener('close', () => {
      this.clearPingTimer();
      if (!this.destroyed) {
        this.setStatus('disconnected');
        this.scheduleReconnect();
      }
    });

    // 'error' always fires before 'close' — reconnect is handled in close handler
    this.ws.addEventListener('error', () => undefined);
  }

  private sendHello(): void {
    const hello = {
      type: 'client.hello' as const,
      name: CLIENT_NAMES.OVERLAY,
      subscriptions: [
        TOPICS.WILDCARD_OVERLAY,
        TOPICS.WILDCARD_OVERLAY_AUDIO,
        TOPICS.WILDCARD_LOTAT,
        TOPICS.WILDCARD_SQUAD,
        TOPICS.WILDCARD_STREAM,
        TOPICS.WILDCARD_SYSTEM,
      ],
    };
    this.ws?.send(JSON.stringify(hello));
  }

  private handleMessage(data: string): void {
    let parsed: unknown;
    try {
      parsed = JSON.parse(data);
    } catch {
      console.warn('[BrokerClient] Non-JSON message received:', data.slice(0, 100));
      return;
    }

    if (!isObject(parsed)) return;

    // ClientWelcome — handshake complete, subscriptions are now active
    if (parsed['type'] === 'client.welcome') {
      const welcome = parsed as unknown as ClientWelcome;
      console.log(
        `[BrokerClient] Connected. clientId=${welcome.clientId}`,
        'peers:', welcome.connectedClients,
      );
      this.setStatus('connected');
      this.startPingTimer();
      return;
    }

    // BrokerMessage — dispatch to topic and onAny subscribers
    if (typeof parsed['topic'] === 'string') {
      const message = parsed as unknown as BrokerMessage;
      this.dispatch(message.topic, message);
      this.dispatch(ANY_TOPIC_KEY, message);
    }
  }

  private dispatch(key: string, message: BrokerMessage): void {
    this.topicHandlers.get(key)?.forEach((h) => h(message));
  }

  // --------------------------------------------------------------------------
  // Private — keepalive
  // --------------------------------------------------------------------------

  private startPingTimer(): void {
    this.clearPingTimer();
    this.pingTimer = setInterval(() => {
      if (this.ws?.readyState === WebSocket.OPEN) {
        const ping: BrokerMessage = {
          id: crypto.randomUUID(),
          topic: TOPICS.SYSTEM_PING,
          sender: CLIENT_NAMES.OVERLAY,
          timestamp: Date.now(),
          payload: {},
        };
        this.ws.send(JSON.stringify(ping));
      }
    }, PING_INTERVAL_MS);
  }

  // --------------------------------------------------------------------------
  // Private — reconnect backoff
  // --------------------------------------------------------------------------

  private scheduleReconnect(): void {
    const delay = Math.min(
      RECONNECT_BASE_MS * 2 ** this.reconnectAttempt,
      RECONNECT_MAX_MS,
    );
    this.reconnectAttempt++;
    console.log(
      `[BrokerClient] Reconnecting in ${delay}ms (attempt ${this.reconnectAttempt})`,
    );
    this.reconnectTimer = setTimeout(() => this.connect(), delay);
  }

  // --------------------------------------------------------------------------
  // Private — helpers
  // --------------------------------------------------------------------------

  private setStatus(status: BrokerStatus): void {
    if (this._status === status) return;
    this._status = status;
    this.statusHandlers.forEach((h) => h(status));
  }

  private clearPingTimer(): void {
    if (this.pingTimer !== null) {
      clearInterval(this.pingTimer);
      this.pingTimer = null;
    }
  }

  private clearTimers(): void {
    this.clearPingTimer();
    if (this.reconnectTimer !== null) {
      clearTimeout(this.reconnectTimer);
      this.reconnectTimer = null;
    }
  }
}

function isObject(val: unknown): val is Record<string, unknown> {
  return typeof val === 'object' && val !== null && !Array.isArray(val);
}

/** Singleton broker client — shared across all Phaser scenes. */
export const brokerClient = new BrokerClient();

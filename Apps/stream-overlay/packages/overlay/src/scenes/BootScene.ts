import Phaser from 'phaser';

/**
 * BootScene — first scene in the Phaser scene stack.
 *
 * Responsibilities:
 *   - Preload any shared static assets needed before rendering starts
 *     (none yet — asset system built in Prompt 04)
 *   - Hand off to OverlayScene once ready
 *
 * The broker connection is started by importing broker-client.ts (singleton).
 * BootScene does not wait for the broker to connect before starting OverlayScene —
 * the overlay renders an empty transparent canvas until messages arrive, and the
 * debug indicator (if ?debug=true) shows connection status in real time.
 */
export class BootScene extends Phaser.Scene {
  constructor() {
    super({ key: 'BootScene' });
  }

  preload(): void {
    // Assets are loaded on-demand by AssetManager when overlay.spawn messages
    // arrive. Nothing needs preloading at boot — Phaser's built-in cache
    // ensures each asset is only fetched once regardless of how many times
    // it is spawned. Add shared UI textures or fonts here if needed in future.
  }

  create(): void {
    console.log('[BootScene] Boot complete — starting OverlayScene');
    this.scene.start('OverlayScene');
  }
}

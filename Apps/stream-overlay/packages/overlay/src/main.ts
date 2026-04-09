/**
 * @stream-overlay/overlay — Phaser OBS browser source entry point.
 *
 * Initializes Phaser at 1920×1080 with a transparent WebGL canvas and starts
 * the broker connection. The canvas sits in the #game div defined in index.html
 * and is composited by OBS as a browser source above the stream scene.
 *
 * Debug mode: append ?debug=true to the URL to show a connection status HUD
 * in the top-right corner. Not visible in normal operation.
 *
 * Scene flow:
 *   BootScene → OverlayScene
 *   BootScene preloads shared assets (none in Prompt 03) then immediately
 *   starts OverlayScene, which runs for the lifetime of the overlay.
 */

import Phaser from 'phaser';
import { BootScene } from './scenes/BootScene';
import { OverlayScene } from './scenes/OverlayScene';
import { brokerClient, type BrokerStatus } from './broker-client';

// --------------------------------------------------------------------------
// Phaser game
// --------------------------------------------------------------------------

const gameConfig: Phaser.Types.Core.GameConfig = {
  type: Phaser.WEBGL,    // WebGL renderer — OBS supports WebGL. Switch to Phaser.AUTO if needed.
  width: 1920,
  height: 1080,
  transparent: true,     // Required for OBS compositing — no background fills the canvas
  parent: 'game',        // Mount into <div id="game"> in index.html
  scene: [BootScene, OverlayScene],
  // Disable the default Phaser banner in the browser console
  banner: false,
};

new Phaser.Game(gameConfig);

// --------------------------------------------------------------------------
// Debug status indicator
// Only mounted when ?debug=true is in the URL.
// Shows broker connection state as a color-coded dot + label in the corner.
// --------------------------------------------------------------------------

const DEBUG = new URLSearchParams(window.location.search).has('debug');

if (DEBUG) {
  mountDebugOverlay();
}

function mountDebugOverlay(): void {
  const wrapper = document.createElement('div');
  wrapper.style.cssText = `
    position: fixed;
    top: 8px;
    right: 8px;
    display: flex;
    align-items: center;
    gap: 6px;
    background: rgba(0, 0, 0, 0.55);
    border-radius: 4px;
    padding: 4px 8px;
    font-family: monospace;
    font-size: 11px;
    color: #fff;
    z-index: 9999;
    pointer-events: none;
    user-select: none;
  `;

  const dot = document.createElement('span');
  dot.style.cssText = `
    width: 8px;
    height: 8px;
    border-radius: 50%;
    display: inline-block;
    flex-shrink: 0;
  `;

  const label = document.createElement('span');

  wrapper.appendChild(dot);
  wrapper.appendChild(label);
  document.body.appendChild(wrapper);

  const statusColors: Record<BrokerStatus, string> = {
    connecting: '#ffaa00',
    connected: '#44ff44',
    disconnected: '#ff4444',
  };

  function applyStatus(status: BrokerStatus): void {
    dot.style.background = statusColors[status];
    label.textContent = `broker: ${status}`;
  }

  // Reflect current status immediately (broker starts connecting on import)
  applyStatus(brokerClient.status);

  // Update on future status changes
  brokerClient.onStatus(applyStatus);
}

/**
 * CountdownTimer — reusable countdown display component.
 *
 * Renders a "M:SS" timer at a fixed canvas position and ticks down once per
 * second using Phaser's time system.  The timer is local to the overlay —
 * it receives the window length at open time and counts down independently
 * (no server tick messages).
 *
 * Usage:
 *   const timer = new CountdownTimer(scene, x, y, depth);
 *   timer.start(120);    // start a 120-second countdown
 *   timer.stop();        // cancel and hide
 *   timer.destroy();     // clean up on scene shutdown
 */

import Phaser from 'phaser';
import { LOTAT_FONT, LOTAT_COLOR } from './lotat-constants';

export class CountdownTimer {
  private readonly label: Phaser.GameObjects.Text;
  private timerEvent: Phaser.Time.TimerEvent | null = null;
  private remaining = 0;

  constructor(
    private readonly scene: Phaser.Scene,
    x: number,
    y: number,
    depth: number,
  ) {
    this.label = scene.add
      .text(x, y, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.accent,
      })
      .setDepth(depth)
      .setVisible(false);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  /** Start the countdown from `windowSeconds`.  Cancels any running timer. */
  start(windowSeconds: number): void {
    this.stop();
    this.remaining = windowSeconds;
    this.render();
    this.label.setVisible(true);

    this.timerEvent = this.scene.time.addEvent({
      delay: 1000,
      loop: true,
      callback: this.tick,
      callbackScope: this,
    });
  }

  /** Stop the countdown and hide the display. */
  stop(): void {
    if (this.timerEvent) {
      this.timerEvent.remove(false);
      this.timerEvent = null;
    }
    this.label.setVisible(false);
  }

  /** Move the timer display to a new canvas position. */
  setPosition(x: number, y: number): void {
    this.label.setPosition(x, y);
  }

  /** Destroy the underlying game object.  Call on scene shutdown. */
  destroy(): void {
    this.stop();
    this.label.destroy();
  }

  // --------------------------------------------------------------------------
  // Private
  // --------------------------------------------------------------------------

  private tick(): void {
    if (this.remaining > 0) this.remaining--;
    this.render();
  }

  private render(): void {
    const m = Math.floor(this.remaining / 60);
    const s = this.remaining % 60;
    const sStr = s.toString().padStart(2, '0');
    this.label.setText(`${m}:${sStr}`);

    // Colour shifts to danger red when under 30 seconds
    this.label.setColor(this.remaining <= 30 ? LOTAT_COLOR.danger : LOTAT_COLOR.accent);
  }
}

/**
 * GameTimer — squad countdown display.
 *
 * Identical behaviour to CountdownTimer (lotat) but uses squad layout
 * constants and is positioned at the top-right of the squad panel.
 *
 * Usage:
 *   const timer = new GameTimer(scene, x, y, depth, color);
 *   timer.start(60);   // 60-second countdown
 *   timer.stop();
 *   timer.destroy();
 */

import Phaser from 'phaser';
import { SQUAD_FONT, SQUAD_COLOR } from './squad-constants';

export class GameTimer {
  private readonly label: Phaser.GameObjects.Text;
  private timerEvent: Phaser.Time.TimerEvent | null = null;
  private remaining = 0;
  /** Accent colour shown while time remains; switches to danger under 30s. */
  private readonly accentColor: string;

  constructor(
    private readonly scene: Phaser.Scene,
    x: number,
    y: number,
    depth: number,
    accentColor: string = SQUAD_COLOR.primary,
  ) {
    this.accentColor = accentColor;
    this.label = scene.add
      .text(x, y, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: accentColor,
      })
      .setDepth(depth)
      .setVisible(false);
  }

  /** Start countdown from windowSeconds. Cancels any running timer first. */
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

  /** Stop and hide the timer. */
  stop(): void {
    if (this.timerEvent) {
      this.timerEvent.remove(false);
      this.timerEvent = null;
    }
    this.label.setVisible(false);
  }

  destroy(): void {
    this.stop();
    this.label.destroy();
  }

  // --------------------------------------------------------------------------

  private tick(): void {
    if (this.remaining > 0) this.remaining--;
    this.render();
  }

  private render(): void {
    const m = Math.floor(this.remaining / 60);
    const s = this.remaining % 60;
    this.label.setText(`${m}:${s.toString().padStart(2, '0')}`);
    this.label.setColor(this.remaining <= 30 ? SQUAD_COLOR.danger : this.accentColor);
  }
}

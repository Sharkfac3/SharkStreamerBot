/**
 * ResultDisplay — win/loss/outcome announcement for any squad game.
 *
 * Shows a single line of result text inside the squad panel.
 * Auto-hides after a configurable delay (default 5 seconds).
 *
 * Usage:
 *   const result = new ResultDisplay(scene, x, y, depth);
 *   result.show('★ QUACK THRESHOLD REACHED! DUCK UNLOCKED! ★', '#00CCFF');
 *   result.show('✗ NOT ENOUGH QUACKS.', '#FF4444', 4000); // custom delay
 *   result.hide();
 *   result.destroy();
 */

import Phaser from 'phaser';
import { SQUAD_FONT } from './squad-constants';

const DEFAULT_AUTO_HIDE_MS = 5000;

export class ResultDisplay {
  private readonly text: Phaser.GameObjects.Text;
  private autoHideEvent: Phaser.Time.TimerEvent | null = null;

  constructor(
    private readonly scene: Phaser.Scene,
    x: number,
    y: number,
    depth: number,
    maxWidth: number,
  ) {
    this.text = scene.add
      .text(x, y, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: '#FFFFFF',
        wordWrap: { width: maxWidth },
      })
      .setDepth(depth)
      .setVisible(false);
  }

  /** Show result text. Auto-hides after `autoHideMs` (pass 0 to disable auto-hide). */
  show(content: string, color: string, autoHideMs = DEFAULT_AUTO_HIDE_MS): void {
    if (this.autoHideEvent) {
      this.autoHideEvent.remove(false);
      this.autoHideEvent = null;
    }

    this.text.setText(content).setColor(color).setVisible(true);

    if (autoHideMs > 0) {
      this.autoHideEvent = this.scene.time.delayedCall(autoHideMs, () => {
        this.hide();
        this.autoHideEvent = null;
      });
    }
  }

  hide(): void {
    this.text.setVisible(false);
  }

  destroy(): void {
    if (this.autoHideEvent) {
      this.autoHideEvent.remove(false);
    }
    this.text.destroy();
  }
}

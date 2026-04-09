/**
 * GameBanner — squad game title header.
 *
 * Renders a game name (e.g. "DUCK GAME!" or "CLONE GAME!") at the top of
 * the squad panel with a game-specific accent colour. Used by every
 * per-game renderer.
 *
 * Usage:
 *   const banner = new GameBanner(scene, panelX, panelY, depth);
 *   banner.show('DUCK GAME!', SQUAD_COLOR.duck);
 *   banner.hide();
 *   banner.destroy();
 */

import Phaser from 'phaser';
import { SQUAD_FONT, SQUAD_LAYOUT } from './squad-constants';

export class GameBanner {
  private readonly label: Phaser.GameObjects.Text;

  constructor(
    scene: Phaser.Scene,
    /** Absolute canvas x (typically SQUAD_LAYOUT.panelX + marginX) */
    x: number,
    /** Absolute canvas y (typically SQUAD_LAYOUT.panelY + bannerY) */
    y: number,
    depth: number,
  ) {
    this.label = scene.add
      .text(x, y, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: '#FFFFFF',
      })
      .setDepth(depth)
      .setVisible(false);
  }

  /** Show the banner with the given text and accent colour. */
  show(text: string, color: string): void {
    this.label.setText(text).setColor(color).setVisible(true);
  }

  hide(): void {
    this.label.setVisible(false);
  }

  /** Move the banner to a new canvas position (e.g. to make room for a timer). */
  setPosition(x: number, y: number): void {
    this.label.setPosition(x, y);
  }

  destroy(): void {
    this.label.destroy();
  }
}

// Export a convenience factory so callers don't need to calculate the position.
export function makeGameBanner(scene: Phaser.Scene): GameBanner {
  return new GameBanner(
    scene,
    SQUAD_LAYOUT.panelX + SQUAD_LAYOUT.marginX,
    SQUAD_LAYOUT.panelY + SQUAD_LAYOUT.bannerY,
    SQUAD_LAYOUT.depthContent,
  );
}

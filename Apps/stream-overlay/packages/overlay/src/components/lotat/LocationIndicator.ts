/**
 * LocationIndicator — displays the current ship section in the info strip.
 *
 * Renders: "▸ COMMAND DECK"
 *
 * Positioned at the left end of the strip row.
 * Shown for the duration of a node; hidden between nodes and when session ends.
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
} from './lotat-constants';

const X = LOTAT_LAYOUT.marginX;
const Y = LOTAT_LAYOUT.stripY;

export class LocationIndicator {
  private readonly text: Phaser.GameObjects.Text;

  constructor(scene: Phaser.Scene) {
    this.text = scene.add
      .text(X, Y, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
      })
      .setDepth(LOTAT_LAYOUT.depthStrip)
      .setVisible(false);
  }

  show(shipSection: string): void {
    this.text.setText(`▸ ${shipSection.toUpperCase()}`).setVisible(true);
  }

  hide(): void {
    this.text.setVisible(false);
  }

  destroy(): void {
    this.text.destroy();
  }
}

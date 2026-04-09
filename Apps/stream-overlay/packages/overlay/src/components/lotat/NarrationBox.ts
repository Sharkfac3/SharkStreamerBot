/**
 * NarrationBox — displays the node's `read_aloud` narration text.
 *
 * A dark panel spanning nearly the full canvas width sits behind
 * word-wrapped narration text.  Also used to show the `result_flavor`
 * text when a vote resolves.
 *
 * Rendered in the narration zone (y=718, h=136) of the lower third.
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
  LOTAT_HEX,
  LOTAT_ALPHA,
} from './lotat-constants';

const X    = LOTAT_LAYOUT.marginX;
const Y    = LOTAT_LAYOUT.narrationY;
const W    = LOTAT_LAYOUT.canvasW - LOTAT_LAYOUT.marginX * 2;
const H    = LOTAT_LAYOUT.narrationH;
const PAD  = 18;   // inner text padding from panel edge

export class NarrationBox {
  private readonly bg:   Phaser.GameObjects.Rectangle;
  private readonly text: Phaser.GameObjects.Text;

  constructor(scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthContent;

    this.bg = scene.add
      .rectangle(X, Y, W, H, LOTAT_HEX.panelDark, LOTAT_ALPHA.panel)
      .setOrigin(0, 0)
      .setDepth(d)
      .setVisible(false);

    this.text = scene.add
      .text(X + PAD, Y + PAD, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
        wordWrap: { width: W - PAD * 2 },
        lineSpacing: 6,
      })
      .setDepth(d + 0.1)
      .setVisible(false);
  }

  /**
   * Show narration text.
   * @param content  The read_aloud string from the story node.
   * @param color    Optional override colour — used for result_flavor text.
   */
  show(content: string, color?: string): void {
    this.text.setText(content).setColor(color ?? LOTAT_COLOR.primary);
    this.bg.setVisible(true);
    this.text.setVisible(true);
  }

  hide(): void {
    this.bg.setVisible(false);
    this.text.setVisible(false);
  }

  destroy(): void {
    this.bg.destroy();
    this.text.destroy();
  }
}

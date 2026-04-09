/**
 * ChaosMeter — displays the cumulative chaos level in the info strip.
 *
 * Renders a text label + a filled progress bar + a numeric value:
 *   CHAOS  [████████░░]  12
 *
 * The bar colour shifts:
 *   0–33%  → green   (calm)
 *   34–66% → yellow  (tension)
 *   67–100% → red    (critical)
 *
 * Positioned centre-right of the info strip.
 * Updated every time a `lotat.chaos.update` message arrives.
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
  LOTAT_HEX,
  LOTAT_ALPHA,
  CHAOS_MAX_VISUAL,
} from './lotat-constants';

// Bar geometry
const BAR_W  = 160;
const BAR_H  = 14;
const LABEL_X = 870;   // "CHAOS" label x
const BAR_X   = 940;   // bar left edge
const VALUE_X = BAR_X + BAR_W + 12;
const Y_LABEL = LOTAT_LAYOUT.stripY + 2;
const Y_BAR   = LOTAT_LAYOUT.stripY + 10;

export class ChaosMeter {
  private readonly labelText:  Phaser.GameObjects.Text;
  private readonly valueText:  Phaser.GameObjects.Text;
  private readonly barTrack:   Phaser.GameObjects.Graphics;
  private readonly barFill:    Phaser.GameObjects.Graphics;

  private chaosTotal = 0;
  private shown = false;

  constructor(scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthStrip;

    this.labelText = scene.add
      .text(LABEL_X, Y_LABEL, 'CHAOS', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.dim,
      })
      .setDepth(d)
      .setVisible(false);

    // Bar track (background)
    this.barTrack = scene.add.graphics().setDepth(d).setVisible(false);
    this.barTrack.fillStyle(LOTAT_HEX.barBg, LOTAT_ALPHA.panel);
    this.barTrack.fillRect(BAR_X, Y_BAR, BAR_W, BAR_H);

    // Bar fill (redrawn on update)
    this.barFill = scene.add.graphics().setDepth(d + 0.1).setVisible(false);

    this.valueText = scene.add
      .text(VALUE_X, Y_LABEL, '0', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.primary,
      })
      .setDepth(d)
      .setVisible(false);
  }

  get isShown(): boolean {
    return this.shown;
  }

  show(chaosTotal: number): void {
    this.chaosTotal = chaosTotal;
    this.shown = true;
    this.render();
    this.labelText.setVisible(true);
    this.barTrack.setVisible(true);
    this.barFill.setVisible(true);
    this.valueText.setVisible(true);
  }

  update(chaosTotal: number): void {
    this.chaosTotal = chaosTotal;
    if (this.shown) this.render();
  }

  hide(): void {
    this.shown = false;
    this.labelText.setVisible(false);
    this.barTrack.setVisible(false);
    this.barFill.setVisible(false);
    this.valueText.setVisible(false);
  }

  destroy(): void {
    this.labelText.destroy();
    this.barTrack.destroy();
    this.barFill.destroy();
    this.valueText.destroy();
  }

  // --------------------------------------------------------------------------

  private render(): void {
    const ratio     = Math.min(this.chaosTotal / CHAOS_MAX_VISUAL, 1);
    const fillW     = Math.round(BAR_W * ratio);
    const fillColor = ratio < 0.34
      ? LOTAT_HEX.barFill
      : ratio < 0.67
        ? LOTAT_HEX.barMid
        : LOTAT_HEX.barHigh;

    this.barFill.clear();
    if (fillW > 0) {
      this.barFill.fillStyle(fillColor, 1);
      this.barFill.fillRect(BAR_X, Y_BAR, fillW, BAR_H);
    }

    this.valueText.setText(String(this.chaosTotal));
    const textColor = ratio < 0.34
      ? LOTAT_COLOR.primary
      : ratio < 0.67
        ? LOTAT_COLOR.partial
        : LOTAT_COLOR.danger;
    this.valueText.setColor(textColor);
  }
}

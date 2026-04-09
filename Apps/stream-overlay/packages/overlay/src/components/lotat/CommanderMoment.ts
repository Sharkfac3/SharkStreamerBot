/**
 * CommanderMoment — shown during commander_open and commander resolution.
 *
 * Occupies the narration zone and renders:
 *
 *   ╔════════════════════════════════════════════╗
 *   ║  ⚡ COMMANDER MOMENT — The Water Wizard    ║
 *   ║  The klaxon blares. Your move, Wizard.    ║
 *   ║  [Awaiting commander input...]             ║
 *   ╚════════════════════════════════════════════╝
 *
 * On `showResult()`:
 *   SUCCESS — shows authored success_text in green.
 *   SKIPPED — shows a neutral skip message in dim colour.
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
  LOTAT_HEX,
  LOTAT_ALPHA,
} from './lotat-constants';
import { CountdownTimer } from './CountdownTimer';

const X   = LOTAT_LAYOUT.marginX;
const Y   = LOTAT_LAYOUT.narrationY;
const W   = LOTAT_LAYOUT.canvasW - LOTAT_LAYOUT.marginX * 2;
const H   = LOTAT_LAYOUT.narrationH;
const PAD = 16;

const TIMER_X = LOTAT_LAYOUT.canvasW - LOTAT_LAYOUT.marginX - 80;
const TIMER_Y = LOTAT_LAYOUT.stripY;

export class CommanderMoment {
  private readonly bg:          Phaser.GameObjects.Rectangle;
  private readonly headerText:  Phaser.GameObjects.Text;
  private readonly promptText:  Phaser.GameObjects.Text;
  private readonly statusText:  Phaser.GameObjects.Text;
  private readonly countdown:   CountdownTimer;

  constructor(private readonly scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthOverlay;

    this.bg = scene.add
      .rectangle(X, Y, W, H, LOTAT_HEX.panelDark, LOTAT_ALPHA.overlay)
      .setOrigin(0, 0)
      .setStrokeStyle(1, 0x887700, 0.9)
      .setDepth(d)
      .setVisible(false);

    this.headerText = scene.add
      .text(X + PAD, Y + PAD, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.accent,
      })
      .setDepth(d + 0.1)
      .setVisible(false);

    this.promptText = scene.add
      .text(X + PAD, Y + PAD + 30, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
        wordWrap: { width: W - PAD * 2 },
      })
      .setDepth(d + 0.1)
      .setVisible(false);

    this.statusText = scene.add
      .text(X + PAD, Y + PAD + 72, '[Awaiting commander input...]', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.dim,
      })
      .setDepth(d + 0.1)
      .setVisible(false);

    this.countdown = new CountdownTimer(scene, TIMER_X, TIMER_Y, LOTAT_LAYOUT.depthStrip);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  show(commander: string, prompt: string, windowSeconds: number): void {
    this.headerText.setText(`⚡ COMMANDER MOMENT — ${commander}`);
    this.promptText.setText(prompt);
    this.statusText
      .setText('[Awaiting commander input...]')
      .setColor(LOTAT_COLOR.dim);

    this.bg.setVisible(true);
    this.headerText.setVisible(true);
    this.promptText.setVisible(true);
    this.statusText.setVisible(true);

    this.countdown.start(windowSeconds);
  }

  showResult(outcome: 'success' | 'skipped', successText?: string): void {
    this.countdown.stop();

    if (outcome === 'success' && successText) {
      this.statusText.setText(`✓ ${successText}`).setColor(LOTAT_COLOR.success);
    } else {
      this.statusText.setText('[Commander did not respond — continuing...]')
        .setColor(LOTAT_COLOR.dim);
    }
  }

  hide(): void {
    this.countdown.stop();
    this.bg.setVisible(false);
    this.headerText.setVisible(false);
    this.promptText.setVisible(false);
    this.statusText.setVisible(false);
  }

  destroy(): void {
    this.countdown.destroy();
    this.bg.destroy();
    this.headerText.destroy();
    this.promptText.destroy();
    this.statusText.destroy();
  }
}

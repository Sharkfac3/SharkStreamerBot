/**
 * DiceDisplay — shown during the dice_open and dice resolution phases.
 *
 * Occupies the narration zone and renders:
 *
 *   ╔════════════════════════════════════════╗
 *   ║  🎲 DICE ROLL — Navigation Check       ║
 *   ║  Target: 70+   Type !roll in chat      ║
 *   ║  ─────────────────────────────────     ║
 *   ║  HydroStar_99  rolled  45  ✗           ║
 *   ║  WrenchMonkey  rolled  82  ✓ SUCCESS   ║
 *   ╚════════════════════════════════════════╝
 *
 * Roll results scroll in as they arrive (last 4 kept).
 * After dice.close, the outcome text replaces the roll list.
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

const MAX_ROLLS = 4;

interface RollLine {
  text: Phaser.GameObjects.Text;
}

export class DiceDisplay {
  private readonly bg:          Phaser.GameObjects.Rectangle;
  private readonly headerText:  Phaser.GameObjects.Text;
  private readonly targetText:  Phaser.GameObjects.Text;
  private readonly divider:     Phaser.GameObjects.Graphics;
  private readonly rollLines:   RollLine[] = [];
  private readonly outcomeText: Phaser.GameObjects.Text;
  private readonly countdown:   CountdownTimer;

  private successThreshold = 0;

  constructor(private readonly scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthOverlay;

    this.bg = scene.add
      .rectangle(X, Y, W, H, LOTAT_HEX.panelDark, LOTAT_ALPHA.overlay)
      .setOrigin(0, 0)
      .setStrokeStyle(1, 0x005522, 0.9)
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

    this.targetText = scene.add
      .text(X + PAD, Y + PAD + 28, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.primary,
      })
      .setDepth(d + 0.1)
      .setVisible(false);

    // Divider line
    this.divider = scene.add.graphics().setDepth(d + 0.1).setVisible(false);
    this.divider.lineStyle(1, 0x004422, 0.8);
    this.divider.lineBetween(X + PAD, Y + 60, X + W - PAD, Y + 60);

    // Roll result lines (pre-create, set text on demand)
    for (let i = 0; i < MAX_ROLLS; i++) {
      const txt = scene.add
        .text(X + PAD, Y + 68 + i * 16, '', {
          fontFamily: LOTAT_FONT.family,
          fontSize: LOTAT_FONT.sizeXS,
          color: LOTAT_COLOR.dim,
        })
        .setDepth(d + 0.1)
        .setVisible(false);
      this.rollLines.push({ text: txt });
    }

    this.outcomeText = scene.add
      .text(X + PAD, Y + 68, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
        wordWrap: { width: W - PAD * 2 },
      })
      .setDepth(d + 0.1)
      .setVisible(false);

    this.countdown = new CountdownTimer(scene, TIMER_X, TIMER_Y, LOTAT_LAYOUT.depthStrip);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  show(purpose: string, successThreshold: number, windowSeconds: number): void {
    this.successThreshold = successThreshold;
    this.clearRolls();

    this.headerText.setText(`🎲 DICE ROLL — ${purpose}`);
    this.targetText.setText(`Target: ${successThreshold}+   |   Type !roll in chat`);

    this.outcomeText.setVisible(false);
    this.showRollLines(true);

    this.bg.setVisible(true);
    this.headerText.setVisible(true);
    this.targetText.setVisible(true);
    this.divider.setVisible(true);

    this.countdown.start(windowSeconds);
  }

  addRoll(username: string, rollValue: number, isSuccess: boolean): void {
    // Shift lines down — oldest falls off the bottom, newest appears at top
    for (let i = this.rollLines.length - 1; i > 0; i--) {
      const cur  = this.rollLines[i];
      const prev = this.rollLines[i - 1];
      if (!cur || !prev) continue;
      cur.text.setText(prev.text.text);
      cur.text.setColor(prev.text.style.color as string);
    }

    const mark  = isSuccess ? '✓ SUCCESS' : '✗';
    const color = isSuccess ? LOTAT_COLOR.success : LOTAT_COLOR.dim;
    const roll  = rollValue.toString().padStart(3, ' ');
    const first = this.rollLines[0];
    if (first) {
      first.text
        .setText(`${username.padEnd(20)}  rolled  ${roll}  ${mark}`)
        .setColor(color);
    }
  }

  showOutcome(outcome: 'success' | 'failure', outcomeText: string): void {
    this.countdown.stop();
    this.showRollLines(false);

    const color = outcome === 'success' ? LOTAT_COLOR.success : LOTAT_COLOR.danger;
    const prefix = outcome === 'success' ? '✓ SUCCESS — ' : '✗ FAILURE — ';
    this.outcomeText.setText(`${prefix}${outcomeText}`).setColor(color).setVisible(true);
  }

  hide(): void {
    this.countdown.stop();
    this.bg.setVisible(false);
    this.headerText.setVisible(false);
    this.targetText.setVisible(false);
    this.divider.setVisible(false);
    this.outcomeText.setVisible(false);
    this.showRollLines(false);
  }

  destroy(): void {
    this.countdown.destroy();
    this.bg.destroy();
    this.headerText.destroy();
    this.targetText.destroy();
    this.divider.destroy();
    for (const line of this.rollLines) line.text.destroy();
    this.outcomeText.destroy();
  }

  // --------------------------------------------------------------------------

  private clearRolls(): void {
    for (const line of this.rollLines) line.text.setText('');
  }

  private showRollLines(visible: boolean): void {
    for (const line of this.rollLines) line.text.setVisible(visible);
  }
}

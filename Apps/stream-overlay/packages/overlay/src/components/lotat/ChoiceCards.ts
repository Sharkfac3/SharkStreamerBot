/**
 * ChoiceCards — displays the available vote choices during decision_open.
 *
 * Renders N cards side-by-side in the choices zone (y=862).
 * Each card shows:
 *   Line 1: "!scan"        (chat command — cyan)
 *   Line 2: "Scan for threats"  (human label — green)
 *   Line 3: "VOTES: 0"    (live tally — white)
 *
 * Cards are created dynamically in `show()` so the width adapts to the
 * number of choices (1–4).  Existing cards are destroyed before each
 * new set is built.
 *
 * When `highlightWinner(command)` is called, the winning card brightens
 * and all other cards dim.  This is called by the renderer on vote.close.
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

interface ChoiceCardObjects {
  bg:        Phaser.GameObjects.Rectangle;
  cmdText:   Phaser.GameObjects.Text;
  labelText: Phaser.GameObjects.Text;
  voteText:  Phaser.GameObjects.Text;
  command:   string;
}

const Y   = LOTAT_LAYOUT.choicesY;
const H   = LOTAT_LAYOUT.choicesH;
const LX  = LOTAT_LAYOUT.marginX;
const TOT_W = LOTAT_LAYOUT.canvasW - LOTAT_LAYOUT.marginX * 2;
const GAP = LOTAT_LAYOUT.choiceGap;
const PAD = 14;

// Timer position — right corner of the strip row, above choices
const TIMER_X = LOTAT_LAYOUT.canvasW - LOTAT_LAYOUT.marginX - 80;
const TIMER_Y = LOTAT_LAYOUT.stripY;

export class ChoiceCards {
  private cards: ChoiceCardObjects[] = [];
  private readonly countdown: CountdownTimer;

  constructor(private readonly scene: Phaser.Scene) {
    this.countdown = new CountdownTimer(scene, TIMER_X, TIMER_Y, LOTAT_LAYOUT.depthStrip);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  show(
    choices: Array<{ choiceId: string; label: string; command: string }>,
    windowSeconds: number,
  ): void {
    this.destroyCards();

    const n        = Math.max(1, Math.min(choices.length, 4));
    const cardW    = Math.floor((TOT_W - GAP * (n - 1)) / n);
    const d        = LOTAT_LAYOUT.depthContent;

    choices.slice(0, 4).forEach((choice, i) => {
      const cx = LX + i * (cardW + GAP);

      const bg = this.scene.add
        .rectangle(cx, Y, cardW, H, LOTAT_HEX.choiceBg, LOTAT_ALPHA.choice)
        .setOrigin(0, 0)
        .setStrokeStyle(1, 0x004400, 0.8)
        .setDepth(d)
        .setVisible(true);

      const cmdText = this.scene.add
        .text(cx + PAD, Y + PAD, choice.command, {
          fontFamily: LOTAT_FONT.family,
          fontSize: LOTAT_FONT.sizeMD,
          color: LOTAT_COLOR.cmd,
        })
        .setDepth(d + 0.1)
        .setVisible(true);

      const labelText = this.scene.add
        .text(cx + PAD, Y + PAD + 28, choice.label, {
          fontFamily: LOTAT_FONT.family,
          fontSize: LOTAT_FONT.sizeSM,
          color: LOTAT_COLOR.primary,
          wordWrap: { width: cardW - PAD * 2 },
        })
        .setDepth(d + 0.1)
        .setVisible(true);

      const voteText = this.scene.add
        .text(cx + PAD, Y + H - PAD - 20, 'VOTES: 0', {
          fontFamily: LOTAT_FONT.family,
          fontSize: LOTAT_FONT.sizeSM,
          color: LOTAT_COLOR.white,
        })
        .setDepth(d + 0.1)
        .setVisible(true);

      this.cards.push({ bg, cmdText, labelText, voteText, command: choice.command });
    });

    this.countdown.start(windowSeconds);
  }

  /** Update live vote totals.  `voteTotals` is a command→count map. */
  updateTally(voteTotals: Record<string, number>): void {
    for (const card of this.cards) {
      const count = voteTotals[card.command] ?? 0;
      card.voteText.setText(`VOTES: ${count}`);
    }
  }

  /** Highlight the winning card; dim all others. */
  highlightWinner(winningCommand: string | null): void {
    for (const card of this.cards) {
      const isWinner = card.command === winningCommand;
      card.bg.setFillStyle(
        isWinner ? LOTAT_HEX.choiceHover : LOTAT_HEX.choiceBg,
        isWinner ? 1 : 0.4,
      );
      card.cmdText.setAlpha(isWinner ? 1 : 0.4);
      card.labelText.setAlpha(isWinner ? 1 : 0.4);
      card.voteText.setAlpha(isWinner ? 1 : 0.4);
      if (isWinner) {
        card.cmdText.setColor(LOTAT_COLOR.winner);
      }
    }
    this.countdown.stop();
  }

  hide(): void {
    this.countdown.stop();
    this.destroyCards();
  }

  destroy(): void {
    this.countdown.destroy();
    this.destroyCards();
  }

  // --------------------------------------------------------------------------

  private destroyCards(): void {
    for (const card of this.cards) {
      card.bg.destroy();
      card.cmdText.destroy();
      card.labelText.destroy();
      card.voteText.destroy();
    }
    this.cards = [];
  }
}

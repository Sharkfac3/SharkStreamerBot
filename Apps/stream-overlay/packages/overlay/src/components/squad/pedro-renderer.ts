/**
 * pedro-renderer.ts — Visual layer for the Pedro mini-game.
 *
 * ## Game summary
 * Chat mentions "pedro" during a call window.  If total mentions exceed 100
 * before the timer expires, Pedro is unlocked.  There's also a secret phrase
 * path (`!pedro x500livepedro`) that bypasses this mechanic entirely.
 *
 * ## Visual layout (top-left panel, identical structure to Duck)
 *
 *   ┌──────────────────────────────────────────────────────────────┐
 *   │  PEDRO GAME!                                        1:30    │  banner + timer
 *   │  ─────────────────────────────────────────────────────────  │
 *   │  PEDROS: 47              (goal hidden)                       │  counter
 *   │  [████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░]   │  progress bar
 *   │                                                              │
 *   │  result text (win/lose)                                      │
 *   └──────────────────────────────────────────────────────────────┘
 *
 * The 100-mention threshold is NOT shown to viewers.
 *
 * ## Broker messages handled
 *   squad.pedro.start  → show panel, start timer
 *   squad.pedro.update → update mention count + progress bar
 *   squad.pedro.end    → show result, stop timer, auto-hide
 */

import Phaser from 'phaser';
import type { SquadPedroUpdateState, SquadPedroEndResult } from '@stream-overlay/shared';
import {
  SQUAD_LAYOUT,
  SQUAD_HEX,
  SQUAD_ALPHA,
  SQUAD_COLOR,
  SQUAD_FONT,
} from './squad-constants';
import { GameBanner }    from './GameBanner';
import { GameTimer }     from './GameTimer';
import { ResultDisplay } from './ResultDisplay';

// Visual bar fills to 100% at this mention count (slightly above the real 100 threshold)
const PEDRO_BAR_CAP = 120;

const PX = SQUAD_LAYOUT.panelX;
const PY = SQUAD_LAYOUT.panelY;
const PW = SQUAD_LAYOUT.panelW;
const PH = SQUAD_LAYOUT.panelH;
const MX = SQUAD_LAYOUT.marginX;
const MY = SQUAD_LAYOUT.marginY;
const D_BASE    = SQUAD_LAYOUT.depthBase;
const D_CONTENT = SQUAD_LAYOUT.depthContent;
const D_BAR     = SQUAD_LAYOUT.depthBar;

export class PedroRenderer {
  private readonly bg:           Phaser.GameObjects.Rectangle;
  private readonly banner:       GameBanner;
  private readonly timer:        GameTimer;
  private readonly divider:      Phaser.GameObjects.Rectangle;
  private readonly mentionLabel: Phaser.GameObjects.Text;
  private readonly hintLabel:    Phaser.GameObjects.Text;
  private readonly barBg:        Phaser.GameObjects.Rectangle;
  private readonly barFill:      Phaser.GameObjects.Rectangle;
  private readonly result:       ResultDisplay;

  constructor(private readonly scene: Phaser.Scene) {
    // Background panel
    this.bg = scene.add
      .rectangle(PX, PY, PW, PH, SQUAD_HEX.panelDark, SQUAD_ALPHA.panel)
      .setOrigin(0, 0)
      .setDepth(D_BASE)
      .setVisible(false);

    // Banner — "PEDRO GAME!"
    this.banner = new GameBanner(
      scene,
      PX + MX,
      PY + MY,
      D_CONTENT,
    );

    // Timer — top-right corner
    this.timer = new GameTimer(
      scene,
      PX + PW - MX - 80,
      PY + MY,
      D_CONTENT,
      SQUAD_COLOR.pedro,
    );

    // Thin divider below banner row
    this.divider = scene.add
      .rectangle(PX + MX, PY + 56, PW - MX * 2, 1, 0x553344, 0.6)
      .setOrigin(0, 0)
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Mention count label
    this.mentionLabel = scene.add
      .text(PX + MX, PY + 68, 'PEDROS: 0', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.pedro,
      })
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Subtle hint that goal is hidden
    this.hintLabel = scene.add
      .text(PX + MX + 220, PY + 72, '(goal hidden)', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeXS,
        color: SQUAD_COLOR.dim,
      })
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Progress bar track
    const barX = PX + MX;
    const barY = PY + SQUAD_LAYOUT.barY;
    const barW = PW - MX * 2;
    this.barBg = scene.add
      .rectangle(barX, barY, barW, SQUAD_LAYOUT.barH, SQUAD_HEX.barBg, 0.9)
      .setOrigin(0, 0)
      .setDepth(D_BAR)
      .setVisible(false);

    // Progress bar fill
    this.barFill = scene.add
      .rectangle(barX, barY, 1, SQUAD_LAYOUT.barH, 0xcc3388, 1)
      .setOrigin(0, 0)
      .setDepth(D_BAR + 0.1)
      .setVisible(false);

    // Result display
    this.result = new ResultDisplay(
      scene,
      PX + MX,
      PY + 144,
      D_CONTENT,
      PW - MX * 2,
    );
  }

  // --------------------------------------------------------------------------
  // Public API — called by SquadRenderer
  // --------------------------------------------------------------------------

  onStart(triggeredBy: string): void {
    console.log(`[PedroRenderer] start  triggeredBy=${triggeredBy}`);
    this.resetCounters();
    this.showAll();
    this.banner.show('PEDRO GAME!', SQUAD_COLOR.pedro);
    this.timer.start(120);
  }

  onUpdate(state: SquadPedroUpdateState): void {
    this.mentionLabel.setText(`PEDROS: ${state.mentionCount}`);
    this.updateBar(state.mentionCount);
  }

  onEnd(result: SquadPedroEndResult): void {
    console.log(`[PedroRenderer] end  success=${result.success}  mentions=${result.finalMentionCount}`);
    this.timer.stop();
    this.mentionLabel.setText(`PEDROS: ${result.finalMentionCount}`);
    this.updateBar(result.finalMentionCount);

    if (result.success) {
      this.result.show('★ PEDRO IS HERE! PEDRO UNLOCKED! ★', SQUAD_COLOR.pedro);
    } else {
      this.result.show(`✗ ONLY ${result.finalMentionCount} PEDROS. NOT ENOUGH.`, SQUAD_COLOR.danger);
    }

    this.scene.time.delayedCall(7000, () => this.hideAll());
  }

  destroy(): void {
    this.bg.destroy();
    this.banner.destroy();
    this.timer.destroy();
    this.divider.destroy();
    this.mentionLabel.destroy();
    this.hintLabel.destroy();
    this.barBg.destroy();
    this.barFill.destroy();
    this.result.destroy();
  }

  // --------------------------------------------------------------------------
  // Helpers
  // --------------------------------------------------------------------------

  private showAll(): void {
    this.bg.setVisible(true);
    this.divider.setVisible(true);
    this.mentionLabel.setVisible(true);
    this.hintLabel.setVisible(true);
    this.barBg.setVisible(true);
    this.barFill.setVisible(true);
  }

  private hideAll(): void {
    this.bg.setVisible(false);
    this.banner.hide();
    this.timer.stop();
    this.divider.setVisible(false);
    this.mentionLabel.setVisible(false);
    this.hintLabel.setVisible(false);
    this.barBg.setVisible(false);
    this.barFill.setVisible(false);
    this.result.hide();
  }

  private resetCounters(): void {
    this.mentionLabel.setText('PEDROS: 0');
    this.barFill.setSize(1, SQUAD_LAYOUT.barH);
  }

  private updateBar(count: number): void {
    const maxW = SQUAD_LAYOUT.panelW - SQUAD_LAYOUT.marginX * 2;
    const ratio = Math.min(count / PEDRO_BAR_CAP, 1);
    const fillW = Math.max(1, Math.floor(maxW * ratio));
    this.barFill.setSize(fillW, SQUAD_LAYOUT.barH);
  }
}

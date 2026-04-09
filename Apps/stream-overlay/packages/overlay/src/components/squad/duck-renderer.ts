/**
 * duck-renderer.ts — Visual layer for the Duck mini-game.
 *
 * ## Game summary
 * Chat collectively types "quack" to hit a hidden threshold before a timer
 * expires.  Unique participants scale the threshold.  Success = Duck unlocked.
 *
 * ## Visual layout (top-left panel, y < 250)
 *
 *   ┌──────────────────────────────────────────────────────────────┐
 *   │  DUCK GAME!                                         2:00    │  banner + timer
 *   │  ─────────────────────────────────────────────────────────  │
 *   │  QUACKS: 47              QUACKERS: 12                        │  counters
 *   │  [████████░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░]      │  progress bar
 *   │                                                              │
 *   │  result text (win/lose)                                      │
 *   └──────────────────────────────────────────────────────────────┘
 *
 * ## Broker messages handled
 *   squad.duck.start  → show panel, start timer
 *   squad.duck.update → update quack count + quacker count + progress bar
 *   squad.duck.end    → show result, stop timer, auto-hide panel after 6s
 *
 * The exact quack threshold is NEVER shown to viewers (matches the game design
 * where the goal is hidden).  The progress bar is visual-only and grows
 * proportionally based on a fixed visual cap.
 */

import Phaser from 'phaser';
import type { SquadDuckUpdateState, SquadDuckEndResult } from '@stream-overlay/shared';
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

// Visual-only quack cap for the progress bar.
// The bar fills to 100% at this value regardless of actual threshold.
const QUACK_BAR_CAP = 150;

const PX = SQUAD_LAYOUT.panelX;
const PY = SQUAD_LAYOUT.panelY;
const PW = SQUAD_LAYOUT.panelW;
const PH = SQUAD_LAYOUT.panelH;
const MX = SQUAD_LAYOUT.marginX;
const MY = SQUAD_LAYOUT.marginY;
const D_BASE    = SQUAD_LAYOUT.depthBase;
const D_CONTENT = SQUAD_LAYOUT.depthContent;
const D_BAR     = SQUAD_LAYOUT.depthBar;

export class DuckRenderer {
  private readonly bg:          Phaser.GameObjects.Rectangle;
  private readonly banner:      GameBanner;
  private readonly timer:       GameTimer;
  private readonly divider:     Phaser.GameObjects.Rectangle;
  private readonly quackLabel:  Phaser.GameObjects.Text;
  private readonly quackerLabel: Phaser.GameObjects.Text;
  private readonly barBg:       Phaser.GameObjects.Rectangle;
  private readonly barFill:     Phaser.GameObjects.Rectangle;
  private readonly result:      ResultDisplay;

  // Track current quack count to animate the bar smoothly
  private quackCount = 0;

  constructor(private readonly scene: Phaser.Scene) {
    // Background panel
    this.bg = scene.add
      .rectangle(PX, PY, PW, PH, SQUAD_HEX.panelDark, SQUAD_ALPHA.panel)
      .setOrigin(0, 0)
      .setDepth(D_BASE)
      .setVisible(false);

    // Banner — "DUCK GAME!"
    this.banner = new GameBanner(
      scene,
      PX + MX,
      PY + MY,
      D_CONTENT,
    );

    // Timer — top-right corner of panel
    this.timer = new GameTimer(
      scene,
      PX + PW - MX - 80,
      PY + MY,
      D_CONTENT,
      SQUAD_COLOR.duck,
    );

    // Thin divider below banner row
    this.divider = scene.add
      .rectangle(PX + MX, PY + 56, PW - MX * 2, 1, 0x224444, 0.6)
      .setOrigin(0, 0)
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Quack count label
    this.quackLabel = scene.add
      .text(PX + MX, PY + 68, 'QUACKS: 0', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.duck,
      })
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Unique quacker count label
    this.quackerLabel = scene.add
      .text(PX + MX + 280, PY + 68, 'QUACKERS: 0', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.primary,
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

    // Progress bar fill — starts at 0 width
    this.barFill = scene.add
      .rectangle(barX, barY, 1, SQUAD_LAYOUT.barH, 0x0099cc, 1)
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
    console.log(`[DuckRenderer] start  triggeredBy=${triggeredBy}`);
    this.quackCount = 0;
    this.resetCounters();
    this.showAll();
    this.banner.show('DUCK GAME!', SQUAD_COLOR.duck);
    this.timer.start(120); // default window; will be overridden if payload includes windowSeconds
  }

  onUpdate(state: SquadDuckUpdateState): void {
    this.quackCount = state.quackCount;
    this.quackLabel.setText(`QUACKS: ${state.quackCount}`);
    this.quackerLabel.setText(`QUACKERS: ${state.uniqueQuackerCount}`);
    this.updateBar(state.quackCount);
  }

  onEnd(result: SquadDuckEndResult): void {
    console.log(`[DuckRenderer] end  success=${result.success}  quacks=${result.finalQuackCount}`);
    this.timer.stop();
    this.quackLabel.setText(`QUACKS: ${result.finalQuackCount}`);
    this.quackerLabel.setText(`QUACKERS: ${result.uniqueQuackerCount}`);
    this.updateBar(result.finalQuackCount);

    if (result.success) {
      this.result.show('★ THRESHOLD REACHED! DUCK UNLOCKED! ★', SQUAD_COLOR.duck);
    } else {
      this.result.show('✗ NOT ENOUGH QUACKS. DUCK STAYS HIDDEN.', SQUAD_COLOR.danger);
    }

    // Auto-hide the whole panel after 7 seconds (result display auto-hides after 5s)
    this.scene.time.delayedCall(7000, () => this.hideAll());
  }

  destroy(): void {
    this.bg.destroy();
    this.banner.destroy();
    this.timer.destroy();
    this.divider.destroy();
    this.quackLabel.destroy();
    this.quackerLabel.destroy();
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
    this.quackLabel.setVisible(true);
    this.quackerLabel.setVisible(true);
    this.barBg.setVisible(true);
    this.barFill.setVisible(true);
  }

  private hideAll(): void {
    this.bg.setVisible(false);
    this.banner.hide();
    this.timer.stop();
    this.divider.setVisible(false);
    this.quackLabel.setVisible(false);
    this.quackerLabel.setVisible(false);
    this.barBg.setVisible(false);
    this.barFill.setVisible(false);
    this.result.hide();
  }

  private resetCounters(): void {
    this.quackLabel.setText('QUACKS: 0');
    this.quackerLabel.setText('QUACKERS: 0');
    this.barFill.setSize(1, SQUAD_LAYOUT.barH);
  }

  private updateBar(count: number): void {
    const maxW = SQUAD_LAYOUT.panelW - SQUAD_LAYOUT.marginX * 2;
    const ratio = Math.min(count / QUACK_BAR_CAP, 1);
    const fillW = Math.max(1, Math.floor(maxW * ratio));
    this.barFill.setSize(fillW, SQUAD_LAYOUT.barH);
  }
}

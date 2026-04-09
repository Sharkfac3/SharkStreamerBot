/**
 * clone-renderer.ts — Visual layer for the Clone mini-game.
 *
 * ## Game summary
 * Musical chairs with 5 numbered positions.  Viewers pick a position with
 * `!rebel N`.  Each volley (timer tick in Streamer.bot) eliminates one
 * position.  Players in the eliminated position are out.  Survivors who
 * held their position from round 1 without losing win.
 *
 * ## Visual layout (wider top-left panel)
 *
 *   ┌─────────────────────────────────────────────────────────────────────────┐
 *   │  CLONE GAME!   ROUND 2                                        0:30     │
 *   │  ─────────────────────────────────────────────────────────────────────  │
 *   │  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐│
 *   │  │  POS  1  │  │  POS  2  │  │ ×POS  3× │  │  POS  4  │  │  POS  5  ││
 *   │  │  OPEN    │  │  OPEN    │  │ ELIMINATED│  │  OPEN    │  │  OPEN    ││
 *   │  └──────────┘  └──────────┘  └──────────┘  └──────────┘  └──────────┘│
 *   │  result text on end                                                    │
 *   └─────────────────────────────────────────────────────────────────────────┘
 *
 * ## Broker messages handled
 *   squad.clone.start  → show panel, init 5 open boxes
 *   squad.clone.update → mark eliminated box, increment round counter
 *   squad.clone.end    → show win/loss result, auto-hide
 */

import Phaser from 'phaser';
import type { SquadCloneUpdateState, SquadCloneEndResult } from '@stream-overlay/shared';
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

const POSITIONS = 5;

const PX = SQUAD_LAYOUT.panelX;
const PY = SQUAD_LAYOUT.panelY;
const PW = SQUAD_LAYOUT.clonePanelW;
const PH = SQUAD_LAYOUT.clonePanelH;
const MX = SQUAD_LAYOUT.marginX;
const MY = SQUAD_LAYOUT.marginY;
const D_BASE    = SQUAD_LAYOUT.depthBase;
const D_CONTENT = SQUAD_LAYOUT.depthContent;

const BOX_W   = SQUAD_LAYOUT.cloneBoxW;
const BOX_H   = SQUAD_LAYOUT.cloneBoxH;
const BOX_GAP = SQUAD_LAYOUT.cloneBoxGap;
const BOX_Y   = PY + SQUAD_LAYOUT.cloneBoxY;   // absolute y for boxes

// Absolute x for each position box (1-indexed, index 0 unused)
function boxX(position: number): number {
  return PX + MX + (position - 1) * (BOX_W + BOX_GAP);
}

export class CloneRenderer {
  private readonly bg:         Phaser.GameObjects.Rectangle;
  private readonly banner:     GameBanner;
  private readonly timer:      GameTimer;
  private readonly roundLabel: Phaser.GameObjects.Text;
  private readonly divider:    Phaser.GameObjects.Rectangle;

  // One bg rect + label per position box (indices 1-5, index 0 is null)
  private readonly boxBgs:    Array<Phaser.GameObjects.Rectangle | null>;
  private readonly boxLabels: Array<Phaser.GameObjects.Text | null>;
  private readonly boxStatus: Array<Phaser.GameObjects.Text | null>;

  private readonly result: ResultDisplay;

  // Track which positions are currently eliminated
  private readonly eliminated = new Set<number>();

  constructor(private readonly scene: Phaser.Scene) {
    // Background panel
    this.bg = scene.add
      .rectangle(PX, PY, PW, PH, SQUAD_HEX.panelDark, SQUAD_ALPHA.panel)
      .setOrigin(0, 0)
      .setDepth(D_BASE)
      .setVisible(false);

    // Banner
    this.banner = new GameBanner(
      scene,
      PX + MX,
      PY + MY,
      D_CONTENT,
    );

    // Round label (right of banner)
    this.roundLabel = scene.add
      .text(PX + MX + 260, PY + MY + 4, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeSM,
        color: SQUAD_COLOR.dim,
      })
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Timer — top-right corner
    this.timer = new GameTimer(
      scene,
      PX + PW - MX - 80,
      PY + MY,
      D_CONTENT,
      SQUAD_COLOR.clone,
    );

    // Divider
    this.divider = scene.add
      .rectangle(PX + MX, PY + 56, PW - MX * 2, 1, 0x444422, 0.6)
      .setOrigin(0, 0)
      .setDepth(D_CONTENT)
      .setVisible(false);

    // Position boxes — allocate 6 slots, index 1-5 used
    this.boxBgs    = new Array(POSITIONS + 1).fill(null);
    this.boxLabels = new Array(POSITIONS + 1).fill(null);
    this.boxStatus = new Array(POSITIONS + 1).fill(null);

    for (let p = 1; p <= POSITIONS; p++) {
      const bx = boxX(p);

      this.boxBgs[p] = scene.add
        .rectangle(bx, BOX_Y, BOX_W, BOX_H, SQUAD_HEX.boxBg, 0.9)
        .setOrigin(0, 0)
        .setDepth(D_CONTENT)
        .setStrokeStyle(1, SQUAD_HEX.boxBorder, 0.8)
        .setVisible(false);

      this.boxLabels[p] = scene.add
        .text(bx + BOX_W / 2, BOX_Y + 18, `POS ${p}`, {
          fontFamily: SQUAD_FONT.family,
          fontSize: SQUAD_FONT.sizeSM,
          color: SQUAD_COLOR.clone,
        })
        .setOrigin(0.5, 0)
        .setDepth(D_CONTENT + 0.1)
        .setVisible(false);

      this.boxStatus[p] = scene.add
        .text(bx + BOX_W / 2, BOX_Y + 46, 'OPEN', {
          fontFamily: SQUAD_FONT.family,
          fontSize: SQUAD_FONT.sizeXS,
          color: SQUAD_COLOR.primary,
        })
        .setOrigin(0.5, 0)
        .setDepth(D_CONTENT + 0.1)
        .setVisible(false);
    }

    // Result display below boxes
    this.result = new ResultDisplay(
      scene,
      PX + MX,
      BOX_Y + BOX_H + 14,
      D_CONTENT,
      PW - MX * 2,
    );
  }

  // --------------------------------------------------------------------------
  // Public API — called by SquadRenderer
  // --------------------------------------------------------------------------

  onStart(triggeredBy: string): void {
    console.log(`[CloneRenderer] start  triggeredBy=${triggeredBy}`);
    this.eliminated.clear();
    this.resetBoxes();
    this.roundLabel.setText('ROUND 1');
    this.showAll();
    this.banner.show('CLONE GAME!', SQUAD_COLOR.clone);
    // Clone uses a volley timer in Streamer.bot; no fixed window sent on start.
    // Show the timer display stopped (will update on first volley update).
  }

  onUpdate(state: SquadCloneUpdateState): void {
    console.log(`[CloneRenderer] update  round=${state.round}  elim=${state.eliminatedPosition}`);

    // Mark the newly eliminated position
    this.markEliminated(state.eliminatedPosition);

    // Update round label
    this.roundLabel.setText(`ROUND ${state.round}`);
  }

  onEnd(result: SquadCloneEndResult): void {
    console.log(`[CloneRenderer] end  outcome=${result.outcome}`);
    this.timer.stop();
    this.markEliminated(result.eliminatedPosition);

    if (result.outcome === 'win') {
      const names = result.winners ? result.winners.replace(/,/g, ', ') : 'the survivors';
      this.result.show(`★ CLONES WIN! SURVIVORS: ${names} ★`, SQUAD_COLOR.clone);
    } else {
      this.result.show('✗ ALL POSITIONS ELIMINATED. NO SURVIVORS.', SQUAD_COLOR.danger);
    }

    this.scene.time.delayedCall(8000, () => this.hideAll());
  }

  destroy(): void {
    this.bg.destroy();
    this.banner.destroy();
    this.timer.destroy();
    this.roundLabel.destroy();
    this.divider.destroy();
    this.result.destroy();
    for (let p = 1; p <= POSITIONS; p++) {
      this.boxBgs[p]?.destroy();
      this.boxLabels[p]?.destroy();
      this.boxStatus[p]?.destroy();
    }
  }

  // --------------------------------------------------------------------------
  // Helpers
  // --------------------------------------------------------------------------

  private resetBoxes(): void {
    for (let p = 1; p <= POSITIONS; p++) {
      this.boxBgs[p]?.setFillStyle(SQUAD_HEX.boxBg, 0.9).setStrokeStyle(1, SQUAD_HEX.boxBorder, 0.8);
      this.boxLabels[p]?.setColor(SQUAD_COLOR.clone).setText(`POS ${p}`);
      this.boxStatus[p]?.setColor(SQUAD_COLOR.primary).setText('OPEN');
    }
  }

  private markEliminated(position: number): void {
    if (position < 1 || position > POSITIONS) return;
    this.eliminated.add(position);

    this.boxBgs[position]?.setFillStyle(SQUAD_HEX.boxElim, 0.9).setStrokeStyle(1, SQUAD_HEX.boxElimBorder, 0.8);
    this.boxLabels[position]?.setColor(SQUAD_COLOR.danger).setText(`× POS ${position} ×`);
    this.boxStatus[position]?.setColor(SQUAD_COLOR.danger).setText('ELIMINATED');
  }

  private showAll(): void {
    this.bg.setVisible(true);
    this.divider.setVisible(true);
    this.roundLabel.setVisible(true);
    for (let p = 1; p <= POSITIONS; p++) {
      this.boxBgs[p]?.setVisible(true);
      this.boxLabels[p]?.setVisible(true);
      this.boxStatus[p]?.setVisible(true);
    }
  }

  private hideAll(): void {
    this.bg.setVisible(false);
    this.banner.hide();
    this.timer.stop();
    this.divider.setVisible(false);
    this.roundLabel.setVisible(false);
    this.result.hide();
    for (let p = 1; p <= POSITIONS; p++) {
      this.boxBgs[p]?.setVisible(false);
      this.boxLabels[p]?.setVisible(false);
      this.boxStatus[p]?.setVisible(false);
    }
  }
}

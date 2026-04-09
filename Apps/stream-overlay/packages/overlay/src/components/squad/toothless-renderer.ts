/**
 * toothless-renderer.ts — Visual layer for the Toothless mini-game.
 *
 * ## Game summary
 * Any viewer can trigger a rarity roll.  Rolls are instant — Streamer.bot
 * acquires the mini-game lock, determines a rarity, then releases the lock
 * in the same script.  First-time unlocks per rarity get a 19-second
 * celebration.  Non-unlock rolls just show the result.
 *
 * ## Visual layout (upper-right pop-up, NOT a persistent panel)
 *
 *   ┌──────────────────────────────────────┐
 *   │  TOOTHLESS                           │  game label
 *   │  @username                           │  roller
 *   │  ★ FLIGHT ★                          │  rarity (accent color per tier)
 *   │  ✦ NEW UNLOCK! ✦                     │  only on first unlock
 *   └──────────────────────────────────────┘
 *
 * The pop-up slides in from the right and auto-hides.
 * There is no countdown — the roll is instant.
 *
 * ## Broker messages handled
 *   squad.toothless.start  → show "rolling…" state briefly
 *   squad.toothless.end    → show rarity result, auto-hide
 *
 * No squad.toothless.update messages are sent (roll is instantaneous).
 */

import Phaser from 'phaser';
import type { SquadToothlessEndResult, ToothlessRarity } from '@stream-overlay/shared';
import {
  SQUAD_LAYOUT,
  SQUAD_HEX,
  SQUAD_ALPHA,
  SQUAD_COLOR,
  SQUAD_FONT,
} from './squad-constants';

const PX = SQUAD_LAYOUT.toothlessX;
const PY = SQUAD_LAYOUT.toothlessY;
const PW = SQUAD_LAYOUT.toothlessW;
const PH = SQUAD_LAYOUT.toothlessH;
const MX = SQUAD_LAYOUT.marginX;
const MY = SQUAD_LAYOUT.marginY;
const D = SQUAD_LAYOUT.depthPopup;

// Display duration — long enough for viewers to read, then auto-hides
const AUTO_HIDE_MS = 8000;

/** Maps rarity → CSS colour string */
const RARITY_COLOR: Record<ToothlessRarity, string> = {
  regular: SQUAD_COLOR.rarityRegular,
  smol:    SQUAD_COLOR.raritySmol,
  long:    SQUAD_COLOR.rarityLong,
  flight:  SQUAD_COLOR.rarityFlight,
  party:   SQUAD_COLOR.rarityParty,
};

export class ToothlessRenderer {
  private readonly bg:          Phaser.GameObjects.Rectangle;
  private readonly gameLabel:   Phaser.GameObjects.Text;
  private readonly userLabel:   Phaser.GameObjects.Text;
  private readonly rarityLabel: Phaser.GameObjects.Text;
  private readonly unlockLabel: Phaser.GameObjects.Text;

  private autoHideEvent: Phaser.Time.TimerEvent | null = null;

  constructor(private readonly scene: Phaser.Scene) {
    // Background panel
    this.bg = scene.add
      .rectangle(PX, PY, PW, PH, SQUAD_HEX.panelDark, SQUAD_ALPHA.popup)
      .setOrigin(0, 0)
      .setDepth(D)
      .setVisible(false);

    // "TOOTHLESS" label
    this.gameLabel = scene.add
      .text(PX + MX, PY + MY, 'TOOTHLESS', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeSM,
        color: SQUAD_COLOR.toothless,
      })
      .setDepth(D + 0.1)
      .setVisible(false);

    // Username
    this.userLabel = scene.add
      .text(PX + MX, PY + MY + 26, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeSM,
        color: SQUAD_COLOR.dim,
      })
      .setDepth(D + 0.1)
      .setVisible(false);

    // Rarity name (large, coloured)
    this.rarityLabel = scene.add
      .text(PX + MX, PY + MY + 58, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.toothless,
      })
      .setDepth(D + 0.1)
      .setVisible(false);

    // "NEW UNLOCK!" — only shown on first unlock
    this.unlockLabel = scene.add
      .text(PX + MX, PY + MY + 100, '✦ NEW UNLOCK! ✦', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.rarityParty,
      })
      .setDepth(D + 0.1)
      .setVisible(false);
  }

  // --------------------------------------------------------------------------
  // Public API — called by SquadRenderer
  // --------------------------------------------------------------------------

  onStart(triggeredBy: string): void {
    console.log(`[ToothlessRenderer] start  triggeredBy=${triggeredBy}`);
    // Show a brief "rolling…" state while Streamer.bot resolves the roll.
    this.cancelAutoHide();
    this.userLabel.setText(`@${triggeredBy}`);
    this.rarityLabel.setText('rolling…').setColor(SQUAD_COLOR.dim);
    this.unlockLabel.setVisible(false);
    this.showAll();

    // If the end message doesn't arrive within 5s, hide (safety valve)
    this.autoHideEvent = this.scene.time.delayedCall(5000, () => this.hideAll());
  }

  onEnd(result: SquadToothlessEndResult): void {
    console.log(`[ToothlessRenderer] end  rarity=${result.rarity}  user=${result.username}  firstUnlock=${result.isFirstUnlock}`);
    this.cancelAutoHide();

    const color = RARITY_COLOR[result.rarity] ?? SQUAD_COLOR.toothless;
    const rarityDisplay = result.rarity.toUpperCase();

    this.userLabel.setText(`@${result.username}`);
    this.rarityLabel.setText(`★ ${rarityDisplay} ★`).setColor(color);
    this.unlockLabel.setVisible(result.isFirstUnlock);
    this.showAll();

    this.autoHideEvent = this.scene.time.delayedCall(AUTO_HIDE_MS, () => this.hideAll());
  }

  destroy(): void {
    this.cancelAutoHide();
    this.bg.destroy();
    this.gameLabel.destroy();
    this.userLabel.destroy();
    this.rarityLabel.destroy();
    this.unlockLabel.destroy();
  }

  // --------------------------------------------------------------------------
  // Helpers
  // --------------------------------------------------------------------------

  private showAll(): void {
    this.bg.setVisible(true);
    this.gameLabel.setVisible(true);
    this.userLabel.setVisible(true);
    this.rarityLabel.setVisible(true);
    // unlockLabel visibility is set explicitly in onEnd
  }

  private hideAll(): void {
    this.bg.setVisible(false);
    this.gameLabel.setVisible(false);
    this.userLabel.setVisible(false);
    this.rarityLabel.setVisible(false);
    this.unlockLabel.setVisible(false);
  }

  private cancelAutoHide(): void {
    if (this.autoHideEvent) {
      this.autoHideEvent.remove(false);
      this.autoHideEvent = null;
    }
  }
}

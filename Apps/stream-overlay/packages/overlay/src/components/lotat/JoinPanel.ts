/**
 * JoinPanel — shown during the join_open phase.
 *
 * Displays a prominent centred banner inviting viewers to type !join,
 * a live participant count, a countdown timer, and the last few player
 * names as they join.
 *
 * Renders (centred in the lower zone):
 *
 *   ╔══════════════════════════════════╗
 *   ║  ★ LEGENDS OF THE ASCII TEMPLE  ║
 *   ║  Type  !join  to enlist          ║
 *   ║  CREW ENLISTED: 0                ║
 *   ║  ⏱ 2:00                          ║
 *   ║  — sharkfac3 —  — hydro99 —     ║
 *   ╚══════════════════════════════════╝
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

// Panel geometry — centred horizontally in the lower zone
const W   = LOTAT_LAYOUT.joinW;
const H   = LOTAT_LAYOUT.joinH;
const X   = (LOTAT_LAYOUT.canvasW - W) / 2;
const Y   = LOTAT_LAYOUT.zoneY + 20;
const PAD = 24;

/** How many recent player names to display in the panel */
const MAX_NAMES = 5;

export class JoinPanel {
  private readonly bg:          Phaser.GameObjects.Rectangle;
  private readonly titleText:   Phaser.GameObjects.Text;
  private readonly subtitleText:Phaser.GameObjects.Text;
  private readonly countText:   Phaser.GameObjects.Text;
  private readonly timerLabel:  Phaser.GameObjects.Text;
  private readonly namesText:   Phaser.GameObjects.Text;
  private readonly countdown:   CountdownTimer;

  private recentNames: string[] = [];
  private participantCount = 0;

  constructor(scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthOverlay;

    this.bg = scene.add
      .rectangle(X, Y, W, H, LOTAT_HEX.panelAccent, LOTAT_ALPHA.overlay)
      .setOrigin(0, 0)
      .setDepth(d)
      .setStrokeStyle(2, 0x00aa44, 0.8)
      .setVisible(false);

    // "★ LEGENDS OF THE ASCII TEMPLE"
    this.titleText = scene.add
      .text(X + W / 2, Y + PAD, '★ LEGENDS OF THE ASCII TEMPLE', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeLG,
        color: LOTAT_COLOR.accent,
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    // "Type  !join  to enlist"
    this.subtitleText = scene.add
      .text(X + W / 2, Y + PAD + 44, 'Type  !join  to enlist', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.cmd,
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    // "CREW ENLISTED: 0"
    this.countText = scene.add
      .text(X + W / 2, Y + PAD + 84, 'CREW ENLISTED: 0', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    // Timer label sits beside the countdown
    this.timerLabel = scene.add
      .text(X + W / 2 - 60, Y + PAD + 122, '⏱', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.accent,
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    this.countdown = new CountdownTimer(
      scene,
      X + W / 2 - 28,
      Y + PAD + 122,
      d + 0.1,
    );

    // Recent player names
    this.namesText = scene.add
      .text(X + W / 2, Y + PAD + 196, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.dim,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  show(windowSeconds: number): void {
    this.recentNames = [];
    this.participantCount = 0;
    this.updateCountText();
    this.namesText.setText('').setVisible(true);

    this.bg.setVisible(true);
    this.titleText.setVisible(true);
    this.subtitleText.setVisible(true);
    this.countText.setVisible(true);
    this.timerLabel.setVisible(true);
    this.countdown.start(windowSeconds);
  }

  addPlayer(username: string, count: number): void {
    this.participantCount = count;
    this.updateCountText();

    this.recentNames.push(username);
    if (this.recentNames.length > MAX_NAMES) {
      this.recentNames.shift();
    }
    this.namesText.setText(
      this.recentNames.map((n) => `— ${n} —`).join('  '),
    );
  }

  hide(): void {
    this.countdown.stop();
    this.bg.setVisible(false);
    this.titleText.setVisible(false);
    this.subtitleText.setVisible(false);
    this.countText.setVisible(false);
    this.timerLabel.setVisible(false);
    this.namesText.setVisible(false);
  }

  destroy(): void {
    this.countdown.destroy();
    this.bg.destroy();
    this.titleText.destroy();
    this.subtitleText.destroy();
    this.countText.destroy();
    this.timerLabel.destroy();
    this.namesText.destroy();
  }

  // --------------------------------------------------------------------------

  private updateCountText(): void {
    const word = this.participantCount === 1 ? 'operator' : 'operators';
    this.countText.setText(`CREW ENLISTED: ${this.participantCount} ${word}`);
  }
}

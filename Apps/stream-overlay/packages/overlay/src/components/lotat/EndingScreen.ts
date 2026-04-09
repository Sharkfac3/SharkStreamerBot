/**
 * EndingScreen — shown when a LotAT session reaches an authored ending.
 *
 * Renders a large centred panel that covers most of the screen:
 *
 *   ┌──────────────────────────────────────────────────┐
 *   │                                                  │
 *   │          ★ MISSION COMPLETE ★                   │  ← success
 *   │          ⚠ MISSION PARTIAL ⚠                   │  ← partial
 *   │          ✗ MISSION FAILED ✗                    │  ← failure
 *   │                                                  │
 *   │  "The crew secured the reactor and..."          │
 *   │                                                  │
 *   │  [session ended normally]                       │
 *   └──────────────────────────────────────────────────┘
 *
 * Also handles non-story endings:
 *   zero-join   → "No crew enlisted.  Mission aborted."
 *   zero-votes  → "No votes recorded.  Mission unresolved."
 *   fault-abort → "A runtime error ended the mission."
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
  LOTAT_HEX,
  LOTAT_ALPHA,
} from './lotat-constants';

// Ending screen geometry — centred, taller than the narration box
const W   = 1400;
const H   = 360;
const X   = (LOTAT_LAYOUT.canvasW - W) / 2;
const Y   = (LOTAT_LAYOUT.canvasH - H) / 2;
const PAD = 40;

export class EndingScreen {
  private readonly bg:         Phaser.GameObjects.Rectangle;
  private readonly headerText: Phaser.GameObjects.Text;
  private readonly bodyText:   Phaser.GameObjects.Text;
  private readonly footerText: Phaser.GameObjects.Text;

  constructor(scene: Phaser.Scene) {
    const d = LOTAT_LAYOUT.depthEnding;

    this.bg = scene.add
      .rectangle(X, Y, W, H, LOTAT_HEX.panelDark, LOTAT_ALPHA.ending)
      .setOrigin(0, 0)
      .setDepth(d)
      .setVisible(false);

    this.headerText = scene.add
      .text(X + W / 2, Y + PAD, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeXL,
        color: LOTAT_COLOR.accent,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    this.bodyText = scene.add
      .text(X + PAD, Y + PAD + 80, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.primary,
        wordWrap: { width: W - PAD * 2 },
        align: 'center',
      })
      .setOrigin(0, 0)
      .setDepth(d + 0.1)
      .setVisible(false);

    this.footerText = scene.add
      .text(X + W / 2, Y + H - PAD - 20, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeSM,
        color: LOTAT_COLOR.dim,
        align: 'center',
      })
      .setOrigin(0.5, 1)
      .setDepth(d + 0.1)
      .setVisible(false);
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  /**
   * Show the ending screen based on session end payload.
   *
   * @param reason    From lotat.session.end payload
   * @param endState  Only present when reason === 'ending-reached'
   * @param readAloud The ending node's narration text (passed from node.enter)
   */
  show(
    reason: 'ending-reached' | 'zero-join' | 'zero-votes' | 'fault-abort',
    endState?: 'success' | 'partial' | 'failure' | null,
    readAloud?: string,
  ): void {
    const { header, headerColor, body, bgColor } = this.buildContent(
      reason, endState, readAloud,
    );

    this.bg.setFillStyle(bgColor, LOTAT_ALPHA.ending);
    this.headerText.setText(header).setColor(headerColor);
    this.bodyText.setText(body ?? '');
    this.footerText.setText('LEGENDS OF THE ASCII TEMPLE');

    this.bg.setVisible(true);
    this.headerText.setVisible(true);
    this.bodyText.setVisible(body ? true : false);
    this.footerText.setVisible(true);
  }

  hide(): void {
    this.bg.setVisible(false);
    this.headerText.setVisible(false);
    this.bodyText.setVisible(false);
    this.footerText.setVisible(false);
  }

  destroy(): void {
    this.bg.destroy();
    this.headerText.destroy();
    this.bodyText.destroy();
    this.footerText.destroy();
  }

  // --------------------------------------------------------------------------

  private buildContent(
    reason: string,
    endState?: 'success' | 'partial' | 'failure' | null,
    readAloud?: string,
  ): { header: string; headerColor: string; body: string | null; bgColor: number } {
    if (reason === 'ending-reached') {
      switch (endState) {
        case 'success':
          return {
            header:      '★ MISSION COMPLETE ★',
            headerColor: LOTAT_COLOR.success,
            body:        readAloud ?? null,
            bgColor:     LOTAT_HEX.success,
          };
        case 'partial':
          return {
            header:      '⚠ MISSION PARTIAL ⚠',
            headerColor: LOTAT_COLOR.partial,
            body:        readAloud ?? null,
            bgColor:     LOTAT_HEX.partial,
          };
        case 'failure':
        default:
          return {
            header:      '✗ MISSION FAILED ✗',
            headerColor: LOTAT_COLOR.failure,
            body:        readAloud ?? null,
            bgColor:     LOTAT_HEX.failure,
          };
      }
    }

    if (reason === 'zero-join') {
      return {
        header:      '— NO CREW ENLISTED —',
        headerColor: LOTAT_COLOR.dim,
        body:        'No operators joined.  Mission aborted.',
        bgColor:     LOTAT_HEX.panelDark,
      };
    }

    if (reason === 'zero-votes') {
      return {
        header:      '— MISSION UNRESOLVED —',
        headerColor: LOTAT_COLOR.partial,
        body:        'No votes were recorded.  The crew was lost to indecision.',
        bgColor:     LOTAT_HEX.partial,
      };
    }

    // fault-abort
    return {
      header:      '⚠ RUNTIME FAULT ⚠',
      headerColor: LOTAT_COLOR.danger,
      body:        'A runtime error ended the mission safely.',
      bgColor:     LOTAT_HEX.failure,
    };
  }
}

/**
 * CrewFocus — displays the spotlighted crew member in the info strip.
 *
 * Shows whichever of commander or squadMember is non-null.
 * If both are null, the component hides itself.
 *
 * Renders: "CREW: The Water Wizard"
 *
 * Positioned in the info strip, right of the location indicator.
 * v1 is text-only — portrait images deferred until art assets exist.
 */

import Phaser from 'phaser';
import {
  LOTAT_LAYOUT,
  LOTAT_FONT,
  LOTAT_COLOR,
} from './lotat-constants';

/** Horizontal offset — clears the LocationIndicator on the left */
const X = LOTAT_LAYOUT.marginX + 420;
const Y = LOTAT_LAYOUT.stripY;

export class CrewFocus {
  private readonly text: Phaser.GameObjects.Text;

  constructor(scene: Phaser.Scene) {
    this.text = scene.add
      .text(X, Y, '', {
        fontFamily: LOTAT_FONT.family,
        fontSize: LOTAT_FONT.sizeMD,
        color: LOTAT_COLOR.accent,
      })
      .setDepth(LOTAT_LAYOUT.depthStrip)
      .setVisible(false);
  }

  show(crewFocus: { commander: string | null; squadMember: string | null }): void {
    const name = crewFocus.commander ?? crewFocus.squadMember;
    if (!name) {
      this.hide();
      return;
    }
    this.text.setText(`CREW: ${name}`).setVisible(true);
  }

  hide(): void {
    this.text.setVisible(false);
  }

  destroy(): void {
    this.text.destroy();
  }
}

/**
 * lotat-renderer.ts — LotAT visual layer orchestrator.
 *
 * A system (not a separate Phaser scene) that manages all LotAT visual
 * elements within the OverlayScene's lotatLayer.
 *
 * ## Responsibility
 * - Instantiates all LotAT visual components
 * - Receives typed payload objects from OverlayScene broker handlers
 * - Routes each payload to the appropriate component show/hide/update call
 * - Shows and hides the overall LotAT UI based on session state
 *
 * ## Design contract
 * - This renderer is a dumb visual layer — it renders what the engine tells it
 * - No business logic here: no vote resolution, no branching decisions
 * - All payload types come directly from @stream-overlay/shared protocol.ts
 *
 * ## Layout (lower third — y ≥ 670)
 *
 *   ┌─────────────────────────────────────────────────────────────┐
 *   │                  [stream content visible]                   │
 *   │                                                             │
 * y=670 ────────────────────────────────────────────────────────────
 *   │  [▸ LOCATION]  [CREW: name]  [CHAOS ████░]  [timer]        │ strip
 * y=718 ────────────────────────────────────────────────────────────
 *   │  narration text / dice display / commander panel            │
 * y=862 ────────────────────────────────────────────────────────────
 *   │  [!cmd1  label  VOTES:N]  [!cmd2  label  VOTES:N]          │ choices
 * y=972 ────────────────────────────────────────────────────────────
 *
 * Join panel, EndingScreen overlay their respective zones when active.
 */

import Phaser from 'phaser';
import type {
  LotatSessionStartPayload,
  LotatSessionEndPayload,
  LotatJoinOpenPayload,
  LotatJoinUpdatePayload,
  LotatJoinClosePayload,
  LotatNodeEnterPayload,
  LotatVoteOpenPayload,
  LotatVoteCastPayload,
  LotatVoteClosePayload,
  LotatDiceOpenPayload,
  LotatDiceRollPayload,
  LotatDiceClosePayload,
  LotatCommanderOpenPayload,
  LotatCommanderClosePayload,
  LotatChaosUpdatePayload,
} from '@stream-overlay/shared';

import {
  LOTAT_LAYOUT,
  LOTAT_HEX,
  LOTAT_ALPHA,
} from '../components/lotat/lotat-constants';
import { LocationIndicator } from '../components/lotat/LocationIndicator';
import { CrewFocus }         from '../components/lotat/CrewFocus';
import { ChaosMeter }        from '../components/lotat/ChaosMeter';
import { NarrationBox }      from '../components/lotat/NarrationBox';
import { JoinPanel }         from '../components/lotat/JoinPanel';
import { ChoiceCards }       from '../components/lotat/ChoiceCards';
import { DiceDisplay }       from '../components/lotat/DiceDisplay';
import { CommanderMoment }   from '../components/lotat/CommanderMoment';
import { EndingScreen }      from '../components/lotat/EndingScreen';

export class LotATRenderer {
  // Base dark panel that frames the lower-third zone
  private readonly basePanelBg: Phaser.GameObjects.Rectangle;

  // Always-visible strip elements (shown for entire session duration)
  private readonly location:  LocationIndicator;
  private readonly crewFocus: CrewFocus;
  private readonly chaos:     ChaosMeter;

  // Content area — one visible at a time
  private readonly narration:  NarrationBox;
  private readonly dice:       DiceDisplay;
  private readonly commander:  CommanderMoment;

  // Modal / overlay panels
  private readonly joinPanel:  JoinPanel;
  private readonly choices:    ChoiceCards;
  private readonly ending:     EndingScreen;

  // State kept for session.end rendering
  private lastEndingReadAloud: string | null = null;
  private sessionActive = false;

  constructor(private readonly scene: Phaser.Scene) {
    // Base panel covers the full lower-third zone
    this.basePanelBg = scene.add
      .rectangle(
        0,
        LOTAT_LAYOUT.zoneY,
        LOTAT_LAYOUT.canvasW,
        LOTAT_LAYOUT.zoneH,
        LOTAT_HEX.panelDark,
        LOTAT_ALPHA.panel,
      )
      .setOrigin(0, 0)
      .setDepth(LOTAT_LAYOUT.depthBase)
      .setVisible(false);

    this.location   = new LocationIndicator(scene);
    this.crewFocus  = new CrewFocus(scene);
    this.chaos      = new ChaosMeter(scene);
    this.narration  = new NarrationBox(scene);
    this.dice       = new DiceDisplay(scene);
    this.commander  = new CommanderMoment(scene);
    this.joinPanel  = new JoinPanel(scene);
    this.choices    = new ChoiceCards(scene);
    this.ending     = new EndingScreen(scene);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Session lifecycle
  // ──────────────────────────────────────────────────────────────────────────

  onSessionStart(payload: LotatSessionStartPayload): void {
    console.log(`[LotATRenderer] session.start  sessionId=${payload.sessionId}  story="${payload.title}"`);
    this.sessionActive = true;
    this.lastEndingReadAloud = null;
    this.showBaseLower();
    // Join panel will appear separately on join.open
  }

  onSessionEnd(payload: LotatSessionEndPayload): void {
    console.log(`[LotATRenderer] session.end  reason=${payload.reason}  endState=${payload.endState ?? 'n/a'}`);

    // Hide active content panels
    this.joinPanel.hide();
    this.narration.hide();
    this.dice.hide();
    this.commander.hide();
    this.choices.hide();
    this.location.hide();
    this.crewFocus.hide();
    this.chaos.hide();

    // Show the ending screen
    this.ending.show(payload.reason, payload.endState, this.lastEndingReadAloud ?? undefined);

    // Auto-hide everything after 8 seconds
    this.scene.time.delayedCall(8000, () => {
      this.ending.hide();
      this.hideBaseLower();
      this.sessionActive = false;
    });
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Join phase
  // ──────────────────────────────────────────────────────────────────────────

  onJoinOpen(payload: LotatJoinOpenPayload): void {
    console.log(`[LotATRenderer] join.open  window=${payload.windowSeconds}s`);
    this.joinPanel.show(payload.windowSeconds);
  }

  onJoinUpdate(payload: LotatJoinUpdatePayload): void {
    this.joinPanel.addPlayer(payload.username, payload.participantCount);
  }

  onJoinClose(payload: LotatJoinClosePayload): void {
    console.log(`[LotATRenderer] join.close  participants=${payload.participantCount}`);
    this.joinPanel.hide();

    if (payload.participantCount === 0) {
      // Zero-join — session.end will fire next and show the ending screen
      this.hideBaseLower();
    }
    // Otherwise node.enter fires next — components become visible then
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Story nodes
  // ──────────────────────────────────────────────────────────────────────────

  onNodeEnter(payload: LotatNodeEnterPayload): void {
    console.log(`[LotATRenderer] node.enter  node=${payload.nodeId}  type=${payload.nodeType}`);

    // Hide mechanic panels from previous node
    this.dice.hide();
    this.commander.hide();
    this.choices.hide();
    this.narration.hide();

    // Update persistent strip
    this.location.show(payload.shipSection);
    this.crewFocus.show(payload.crewFocus);

    // Show narration
    this.narration.show(payload.readAloud);

    // If this is an ending node, save the readAloud for the ending screen
    if (payload.nodeType === 'ending') {
      this.lastEndingReadAloud = payload.readAloud;
      // No choices shown for ending nodes
      return;
    }

    // Log sfx_hint if present (audio implementation deferred)
    if (payload.sfxHint) {
      console.log(`[LotATRenderer] sfx_hint="${payload.sfxHint}" (audio not yet implemented)`);
    }
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Commander window
  // ──────────────────────────────────────────────────────────────────────────

  onCommanderOpen(payload: LotatCommanderOpenPayload): void {
    console.log(`[LotATRenderer] commander.open  commander=${payload.commander}  window=${payload.windowSeconds}s`);
    this.narration.hide();
    this.commander.show(payload.commander, payload.prompt, payload.windowSeconds);
  }

  onCommanderClose(payload: LotatCommanderClosePayload): void {
    console.log(`[LotATRenderer] commander.close  outcome=${payload.outcome}`);
    this.commander.showResult(payload.outcome, payload.successText);

    // Brief pause so the result is visible, then hide
    this.scene.time.delayedCall(2500, () => {
      this.commander.hide();
    });
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Dice window
  // ──────────────────────────────────────────────────────────────────────────

  onDiceOpen(payload: LotatDiceOpenPayload): void {
    console.log(`[LotATRenderer] dice.open  purpose="${payload.purpose}"  threshold=${payload.successThreshold}  window=${payload.windowSeconds}s`);
    this.narration.hide();
    this.dice.show(payload.purpose, payload.successThreshold, payload.windowSeconds);
  }

  onDiceRoll(payload: LotatDiceRollPayload): void {
    this.dice.addRoll(payload.username, payload.rollValue, payload.isSuccess);
  }

  onDiceClose(payload: LotatDiceClosePayload): void {
    console.log(`[LotATRenderer] dice.close  outcome=${payload.outcome}`);
    this.dice.showOutcome(payload.outcome, payload.outcomeText);

    // Brief pause so the result is visible, then transition to narration
    this.scene.time.delayedCall(3000, () => {
      this.dice.hide();
      this.narration.show(payload.outcomeText,
        payload.outcome === 'success' ? '#00FF88' : '#FF4444');
    });
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Voting window
  // ──────────────────────────────────────────────────────────────────────────

  onVoteOpen(payload: LotatVoteOpenPayload): void {
    console.log(`[LotATRenderer] vote.open  node=${payload.nodeId}  choices=${payload.choices.length}  window=${payload.windowSeconds}s`);
    this.choices.show(payload.choices, payload.windowSeconds);
  }

  onVoteCast(payload: LotatVoteCastPayload): void {
    this.choices.updateTally(payload.voteTotals);
  }

  onVoteClose(payload: LotatVoteClosePayload): void {
    console.log(`[LotATRenderer] vote.close  winner=${payload.winningCommand ?? 'none'}`);
    this.choices.highlightWinner(payload.winningCommand);

    // Show result flavor text in the narration box
    if (payload.resultFlavor) {
      this.narration.show(payload.resultFlavor, '#FFD700');
    }
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Chaos meter
  // ──────────────────────────────────────────────────────────────────────────

  onChaosUpdate(payload: LotatChaosUpdatePayload): void {
    console.log(`[LotATRenderer] chaos.update  total=${payload.chaosTotal}  delta=${payload.delta}`);
    if (!this.chaos.isShown) {
      // First chaos update — initialise meter as visible
      this.chaos.show(payload.chaosTotal);
    } else {
      this.chaos.update(payload.chaosTotal);
    }
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Scene lifecycle
  // ──────────────────────────────────────────────────────────────────────────

  /** Call from OverlayScene cleanup / shutdown */
  destroy(): void {
    this.basePanelBg.destroy();
    this.location.destroy();
    this.crewFocus.destroy();
    this.chaos.destroy();
    this.narration.destroy();
    this.dice.destroy();
    this.commander.destroy();
    this.joinPanel.destroy();
    this.choices.destroy();
    this.ending.destroy();
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Helpers
  // ──────────────────────────────────────────────────────────────────────────

  private showBaseLower(): void {
    this.basePanelBg.setVisible(true);
  }

  private hideBaseLower(): void {
    this.basePanelBg.setVisible(false);
  }
}

/**
 * squad-renderer.ts — Squad visual layer orchestrator.
 *
 * Parallel to LotATRenderer.  Manages all squad game visuals within
 * OverlayScene's squadLayer.
 *
 * ## Responsibility
 * - Instantiates all per-game renderers (Duck, Pedro, Clone, Toothless)
 * - Receives typed payload objects from OverlayScene broker handlers
 * - Routes each payload to the correct game renderer's start/update/end call
 * - Ensures only one squad game is active at a time (mirrors the Streamer.bot
 *   mini-game lock contract — if a stale game is active when a new one starts,
 *   it is force-hidden first)
 *
 * ## Design contract
 * - This renderer is a dumb visual layer — no game logic here
 * - All payload types come from @stream-overlay/shared
 * - Game-specific state shapes are in protocol.ts (SquadDuck*, SquadClone*, etc.)
 */

import Phaser from 'phaser';
import type {
  SquadGameStartPayload,
  SquadGameUpdatePayload,
  SquadGameEndPayload,
  SquadDuckUpdateState,
  SquadDuckEndResult,
  SquadPedroUpdateState,
  SquadPedroEndResult,
  SquadCloneUpdateState,
  SquadCloneEndResult,
  SquadToothlessEndResult,
} from '@stream-overlay/shared';

import { DuckRenderer }      from '../components/squad/duck-renderer';
import { PedroRenderer }     from '../components/squad/pedro-renderer';
import { CloneRenderer }     from '../components/squad/clone-renderer';
import { ToothlessRenderer } from '../components/squad/toothless-renderer';

type GameName = 'duck' | 'pedro' | 'clone' | 'toothless';

export class SquadRenderer {
  private readonly duck:      DuckRenderer;
  private readonly pedro:     PedroRenderer;
  private readonly clone:     CloneRenderer;
  private readonly toothless: ToothlessRenderer;

  constructor(scene: Phaser.Scene) {
    this.duck      = new DuckRenderer(scene);
    this.pedro     = new PedroRenderer(scene);
    this.clone     = new CloneRenderer(scene);
    this.toothless = new ToothlessRenderer(scene);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Duck
  // ──────────────────────────────────────────────────────────────────────────

  onDuckStart(payload: SquadGameStartPayload): void {
    console.log(`[SquadRenderer] squad.duck.start  triggeredBy=${payload.triggeredBy}`);
    this.duck.onStart(payload.triggeredBy);
  }

  onDuckUpdate(payload: SquadGameUpdatePayload): void {
    this.duck.onUpdate(payload.state as unknown as SquadDuckUpdateState);
  }

  onDuckEnd(payload: SquadGameEndPayload): void {
    this.duck.onEnd(payload.result as unknown as SquadDuckEndResult);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Pedro
  // ──────────────────────────────────────────────────────────────────────────

  onPedroStart(payload: SquadGameStartPayload): void {
    console.log(`[SquadRenderer] squad.pedro.start  triggeredBy=${payload.triggeredBy}`);
    this.pedro.onStart(payload.triggeredBy);
  }

  onPedroUpdate(payload: SquadGameUpdatePayload): void {
    this.pedro.onUpdate(payload.state as unknown as SquadPedroUpdateState);
  }

  onPedroEnd(payload: SquadGameEndPayload): void {
    this.pedro.onEnd(payload.result as unknown as SquadPedroEndResult);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Clone
  // ──────────────────────────────────────────────────────────────────────────

  onCloneStart(payload: SquadGameStartPayload): void {
    console.log(`[SquadRenderer] squad.clone.start  triggeredBy=${payload.triggeredBy}`);
    this.clone.onStart(payload.triggeredBy);
  }

  onCloneUpdate(payload: SquadGameUpdatePayload): void {
    this.clone.onUpdate(payload.state as unknown as SquadCloneUpdateState);
  }

  onCloneEnd(payload: SquadGameEndPayload): void {
    this.clone.onEnd(payload.result as unknown as SquadCloneEndResult);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Toothless
  // ──────────────────────────────────────────────────────────────────────────

  onToothlessStart(payload: SquadGameStartPayload): void {
    console.log(`[SquadRenderer] squad.toothless.start  triggeredBy=${payload.triggeredBy}`);
    this.toothless.onStart(payload.triggeredBy);
  }

  onToothlessEnd(payload: SquadGameEndPayload): void {
    this.toothless.onEnd(payload.result as unknown as SquadToothlessEndResult);
  }

  // ──────────────────────────────────────────────────────────────────────────
  // Scene lifecycle
  // ──────────────────────────────────────────────────────────────────────────

  /** Call from OverlayScene cleanup / shutdown */
  destroy(): void {
    this.duck.destroy();
    this.pedro.destroy();
    this.clone.destroy();
    this.toothless.destroy();
  }
}

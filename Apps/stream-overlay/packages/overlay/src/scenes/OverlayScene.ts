import Phaser from 'phaser';
import {
  TOPICS,
  type BrokerMessage,
  type OverlaySpawnPayload,
  type OverlayMovePayload,
  type OverlayAnimatePayload,
  type OverlayRemovePayload,
  type OverlayClearPayload,
  type OverlayAudioPlayPayload,
  type OverlayAudioStopPayload,
  type LotatSessionStartPayload,
  type LotatSessionEndPayload,
  type LotatJoinOpenPayload,
  type LotatJoinUpdatePayload,
  type LotatJoinClosePayload,
  type LotatNodeEnterPayload,
  type LotatVoteOpenPayload,
  type LotatVoteCastPayload,
  type LotatVoteClosePayload,
  type LotatDiceOpenPayload,
  type LotatDiceRollPayload,
  type LotatDiceClosePayload,
  type LotatCommanderOpenPayload,
  type LotatCommanderClosePayload,
  type LotatChaosUpdatePayload,
  type SquadGameStartPayload,
  type SquadGameUpdatePayload,
  type SquadGameEndPayload,
} from '@stream-overlay/shared';
import { brokerClient } from '../broker-client';
import { AnimationSystem } from '../systems/animation-system';
import { AssetManager } from '../systems/asset-manager';
import { AudioManager } from '../systems/audio-manager';
import { LotATRenderer } from '../systems/lotat-renderer';
import { SquadRenderer } from '../systems/squad-renderer';

/**
 * OverlayScene — the always-running rendering surface.
 *
 * This is the single Phaser scene that lives for the entire overlay session.
 * It organises game objects into named layer groups, and delegates all asset
 * and audio work to dedicated manager classes:
 *
 *   AssetManager    — spawn, move, animate, remove, clear visual assets
 *   AnimationSystem — tween wrappers (used internally by AssetManager)
 *   AudioManager    — play and stop MP3 sounds
 *   LotATRenderer   — all lotat.* session UI (Prompt 06)
 *
 * Layer groups (higher depth = renders on top):
 *   alertLayer  — ephemeral stream alerts (subs, raids, follows, bits)
 *   lotatLayer  — LotAT session UI (join counter, vote tally, chaos meter)
 *   squadLayer  — squad mini-game visuals
 *   hudLayer    — persistent HUD elements (always on top)
 *
 * The overlay is a dumb renderer — it never decides WHAT to show. Streamer.bot
 * sends broker messages; this scene renders them.  No business logic lives here.
 */
export class OverlayScene extends Phaser.Scene {
  // Layer groups — all rendering goes through one of these.
  // Assign depth values at spawn time via the overlay.spawn `depth` payload field.
  private alertLayer!: Phaser.GameObjects.Group;
  private lotatLayer!: Phaser.GameObjects.Group;
  private squadLayer!: Phaser.GameObjects.Group;
  private hudLayer!: Phaser.GameObjects.Group;

  // Core systems — instantiated in create()
  private animationSystem!: AnimationSystem;
  private assetManager!: AssetManager;
  private audioManager!: AudioManager;
  private lotatRenderer!: LotATRenderer;
  private squadRenderer!: SquadRenderer;

  // Cleanup functions returned by brokerClient.on()
  private readonly unsubscribers: Array<() => void> = [];

  constructor() {
    super({ key: 'OverlayScene' });
  }

  create(): void {
    // Layer groups — currently used for logical grouping.
    // Assets within a group share no automatic depth; set depth per-asset via overlay.spawn.
    this.alertLayer = this.add.group();
    this.lotatLayer = this.add.group();
    this.squadLayer = this.add.group();
    this.hudLayer   = this.add.group();

    // Systems — AnimationSystem has no deps; AssetManager depends on AnimationSystem
    this.animationSystem = new AnimationSystem(this);
    this.assetManager    = new AssetManager(this, this.animationSystem);
    this.audioManager    = new AudioManager(this);
    this.lotatRenderer   = new LotATRenderer(this);
    this.squadRenderer   = new SquadRenderer(this);

    this.subscribeToBroker();

    this.events.on('shutdown', this.cleanup, this);

    console.log('[OverlayScene] Rendering surface ready');
  }

  /**
   * Called every game frame by Phaser.
   * Drives the GIF animation loop — copies the current frame of each active
   * animated GIF from its hidden <img> element to the corresponding CanvasTexture.
   * No-op when no GIFs are on screen.
   */
  override update(): void {
    this.assetManager.updateGifTextures();
  }

  // --------------------------------------------------------------------------
  // Broker subscriptions
  // --------------------------------------------------------------------------

  private subscribeToBroker(): void {

    // --- Generic overlay asset commands ---

    this.unsubscribers.push(
      brokerClient.on(TOPICS.OVERLAY_SPAWN, (msg: BrokerMessage) => {
        this.assetManager.spawn(msg.payload as OverlaySpawnPayload);
      }),

      brokerClient.on(TOPICS.OVERLAY_MOVE, (msg: BrokerMessage) => {
        this.assetManager.move(msg.payload as OverlayMovePayload);
      }),

      brokerClient.on(TOPICS.OVERLAY_ANIMATE, (msg: BrokerMessage) => {
        this.assetManager.animate(msg.payload as OverlayAnimatePayload);
      }),

      brokerClient.on(TOPICS.OVERLAY_REMOVE, (msg: BrokerMessage) => {
        this.assetManager.remove(msg.payload as OverlayRemovePayload);
      }),

      brokerClient.on(TOPICS.OVERLAY_CLEAR, (msg: BrokerMessage) => {
        this.assetManager.clear(msg.payload as OverlayClearPayload);
      }),
    );

    // --- Audio commands ---

    this.unsubscribers.push(
      brokerClient.on(TOPICS.OVERLAY_AUDIO_PLAY, (msg: BrokerMessage) => {
        this.audioManager.play(msg.payload as OverlayAudioPlayPayload);
      }),

      brokerClient.on(TOPICS.OVERLAY_AUDIO_STOP, (msg: BrokerMessage) => {
        this.audioManager.stop(msg.payload as OverlayAudioStopPayload);
      }),
    );

    // --- System events ---

    this.unsubscribers.push(
      brokerClient.on(TOPICS.SYSTEM_CLIENT_CONNECTED, (msg: BrokerMessage) => {
        console.log('[OverlayScene] system.client.connected', msg.payload);
      }),
      brokerClient.on(TOPICS.SYSTEM_CLIENT_DISCONNECTED, (msg: BrokerMessage) => {
        console.log('[OverlayScene] system.client.disconnected', msg.payload);
      }),
      brokerClient.on(TOPICS.SYSTEM_PONG, (msg: BrokerMessage) => {
        console.log('[OverlayScene] system.pong', msg.payload);
      }),
    );

    // --- Stream alert events (Prompt 05+: replace with alert rendering) ---

    this.unsubscribers.push(
      brokerClient.on(TOPICS.STREAM_SUB, (msg: BrokerMessage) => {
        console.log('[OverlayScene] stream.sub', msg.payload);
      }),
      brokerClient.on(TOPICS.STREAM_RAID, (msg: BrokerMessage) => {
        console.log('[OverlayScene] stream.raid', msg.payload);
      }),
      brokerClient.on(TOPICS.STREAM_FOLLOW, (msg: BrokerMessage) => {
        console.log('[OverlayScene] stream.follow', msg.payload);
      }),
      brokerClient.on(TOPICS.STREAM_BITS, (msg: BrokerMessage) => {
        console.log('[OverlayScene] stream.bits', msg.payload);
      }),
    );

    // --- LotAT session events — routed to LotATRenderer ---

    this.unsubscribers.push(
      brokerClient.on(TOPICS.LOTAT_SESSION_START, (msg: BrokerMessage) => {
        this.lotatRenderer.onSessionStart(msg.payload as LotatSessionStartPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_SESSION_END, (msg: BrokerMessage) => {
        this.lotatRenderer.onSessionEnd(msg.payload as LotatSessionEndPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_JOIN_OPEN, (msg: BrokerMessage) => {
        this.lotatRenderer.onJoinOpen(msg.payload as LotatJoinOpenPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_JOIN_UPDATE, (msg: BrokerMessage) => {
        this.lotatRenderer.onJoinUpdate(msg.payload as LotatJoinUpdatePayload);
      }),
      brokerClient.on(TOPICS.LOTAT_JOIN_CLOSE, (msg: BrokerMessage) => {
        this.lotatRenderer.onJoinClose(msg.payload as LotatJoinClosePayload);
      }),
      brokerClient.on(TOPICS.LOTAT_NODE_ENTER, (msg: BrokerMessage) => {
        this.lotatRenderer.onNodeEnter(msg.payload as LotatNodeEnterPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_VOTE_OPEN, (msg: BrokerMessage) => {
        this.lotatRenderer.onVoteOpen(msg.payload as LotatVoteOpenPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_VOTE_CAST, (msg: BrokerMessage) => {
        this.lotatRenderer.onVoteCast(msg.payload as LotatVoteCastPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_VOTE_CLOSE, (msg: BrokerMessage) => {
        this.lotatRenderer.onVoteClose(msg.payload as LotatVoteClosePayload);
      }),
      brokerClient.on(TOPICS.LOTAT_DICE_OPEN, (msg: BrokerMessage) => {
        this.lotatRenderer.onDiceOpen(msg.payload as LotatDiceOpenPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_DICE_ROLL, (msg: BrokerMessage) => {
        this.lotatRenderer.onDiceRoll(msg.payload as LotatDiceRollPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_DICE_CLOSE, (msg: BrokerMessage) => {
        this.lotatRenderer.onDiceClose(msg.payload as LotatDiceClosePayload);
      }),
      brokerClient.on(TOPICS.LOTAT_COMMANDER_OPEN, (msg: BrokerMessage) => {
        this.lotatRenderer.onCommanderOpen(msg.payload as LotatCommanderOpenPayload);
      }),
      brokerClient.on(TOPICS.LOTAT_COMMANDER_CLOSE, (msg: BrokerMessage) => {
        this.lotatRenderer.onCommanderClose(msg.payload as LotatCommanderClosePayload);
      }),
      brokerClient.on(TOPICS.LOTAT_CHAOS_UPDATE, (msg: BrokerMessage) => {
        this.lotatRenderer.onChaosUpdate(msg.payload as LotatChaosUpdatePayload);
      }),
    );

    // --- Squad game events — routed to SquadRenderer ---

    this.unsubscribers.push(
      // Duck
      brokerClient.on(TOPICS.SQUAD_DUCK_START, (msg: BrokerMessage) => {
        this.squadRenderer.onDuckStart(msg.payload as SquadGameStartPayload);
      }),
      brokerClient.on(TOPICS.SQUAD_DUCK_UPDATE, (msg: BrokerMessage) => {
        this.squadRenderer.onDuckUpdate(msg.payload as SquadGameUpdatePayload);
      }),
      brokerClient.on(TOPICS.SQUAD_DUCK_END, (msg: BrokerMessage) => {
        this.squadRenderer.onDuckEnd(msg.payload as SquadGameEndPayload);
      }),

      // Pedro
      brokerClient.on(TOPICS.SQUAD_PEDRO_START, (msg: BrokerMessage) => {
        this.squadRenderer.onPedroStart(msg.payload as SquadGameStartPayload);
      }),
      brokerClient.on(TOPICS.SQUAD_PEDRO_UPDATE, (msg: BrokerMessage) => {
        this.squadRenderer.onPedroUpdate(msg.payload as SquadGameUpdatePayload);
      }),
      brokerClient.on(TOPICS.SQUAD_PEDRO_END, (msg: BrokerMessage) => {
        this.squadRenderer.onPedroEnd(msg.payload as SquadGameEndPayload);
      }),

      // Clone
      brokerClient.on(TOPICS.SQUAD_CLONE_START, (msg: BrokerMessage) => {
        this.squadRenderer.onCloneStart(msg.payload as SquadGameStartPayload);
      }),
      brokerClient.on(TOPICS.SQUAD_CLONE_UPDATE, (msg: BrokerMessage) => {
        this.squadRenderer.onCloneUpdate(msg.payload as SquadGameUpdatePayload);
      }),
      brokerClient.on(TOPICS.SQUAD_CLONE_END, (msg: BrokerMessage) => {
        this.squadRenderer.onCloneEnd(msg.payload as SquadGameEndPayload);
      }),

      // Toothless
      brokerClient.on(TOPICS.SQUAD_TOOTHLESS_START, (msg: BrokerMessage) => {
        this.squadRenderer.onToothlessStart(msg.payload as SquadGameStartPayload);
      }),
      brokerClient.on(TOPICS.SQUAD_TOOTHLESS_UPDATE, (msg: BrokerMessage) => {
        // No update messages for Toothless (roll is instant) — log and ignore
        console.log('[OverlayScene] squad.toothless.update (ignored)', msg.payload);
      }),
      brokerClient.on(TOPICS.SQUAD_TOOTHLESS_END, (msg: BrokerMessage) => {
        this.squadRenderer.onToothlessEnd(msg.payload as SquadGameEndPayload);
      }),
    );
  }

  // --------------------------------------------------------------------------
  // Cleanup
  // --------------------------------------------------------------------------

  private cleanup(): void {
    this.unsubscribers.forEach((unsub) => unsub());
    this.unsubscribers.length = 0;

    this.assetManager.destroyAll();
    this.audioManager.stopAll();
    this.lotatRenderer.destroy();
    this.squadRenderer.destroy();
  }
}

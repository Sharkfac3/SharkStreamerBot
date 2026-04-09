/**
 * AssetManager — registry and lifecycle manager for all visual assets.
 *
 * Handles the full asset lifecycle in response to broker messages:
 *
 *   overlay.spawn   → create a Phaser Image (PNG/JPG) or GIFImage at position
 *   overlay.move    → tween asset to new position
 *   overlay.animate → apply AnimationPreset in-place
 *   overlay.remove  → play exit animation then destroy
 *   overlay.clear   → remove all (or prefix-filtered) assets
 *
 * Key behaviours:
 *   - Spawning with an existing assetId replaces that asset immediately.
 *   - Assets with a `lifetime` field auto-remove after that many ms, playing
 *     exitAnimation first if specified.
 *   - All move/animate/remove operations on an unknown assetId are silently
 *     logged — never throw.
 *   - GIF assets use phaser3-rex-plugins GIFImage, which extends RenderTexture,
 *     so Phaser tweens (move, fade, shake…) work identically on GIFs and images.
 *
 * Asset folder convention (for agents building on top of this):
 *   public/assets/alerts/   — stream event assets (subs, raids, follows, bits)
 *   public/assets/lotat/    — LotAT character art and scene images
 *   public/assets/squad/    — squad mini-game images
 *   public/assets/audio/    — all MP3 files (used by AudioManager, not here)
 *
 * Relative src paths resolve from packages/overlay/public/ in both dev and
 * OBS production (dist/) modes.
 */

import Phaser from 'phaser';
import type {
  OverlaySpawnPayload,
  OverlayMovePayload,
  OverlayAnimatePayload,
  OverlayRemovePayload,
  OverlayClearPayload,
  AnimationPreset,
} from '@stream-overlay/shared';
import { AnimationSystem, type TweenableGO } from './animation-system';

// --------------------------------------------------------------------------
// Internal registry record
// --------------------------------------------------------------------------

interface AssetRecord {
  assetId: string;
  /** The Phaser game object. Images are Phaser.GameObjects.Image;
   *  GIFs are GIFImage (extends Phaser.GameObjects.RenderTexture). */
  gameObject: TweenableGO;
  src: string;
  isGif: boolean;
  ttlTimer: ReturnType<typeof setTimeout> | null;
  /** Exit animation to play when TTL fires — stored so TTL handler can use it */
  ttlExitAnimation: AnimationPreset;
  ttlExitDuration: number;
}

// --------------------------------------------------------------------------
// Helpers
// --------------------------------------------------------------------------

/** Resize a game object to fit within the requested dimensions, preserving
 *  aspect ratio when only one dimension is given. */
function applySize(
  go: TweenableGO,
  nativeW: number,
  nativeH: number,
  width?: number,
  height?: number,
): void {
  if (!width && !height) return;
  if (nativeW <= 0 || nativeH <= 0) return;

  let targetW: number;
  let targetH: number;

  if (width && height) {
    targetW = width;
    targetH = height;
  } else if (width) {
    targetW = width;
    targetH = width * (nativeH / nativeW);
  } else {
    targetH = height!;
    targetW = height! * (nativeW / nativeH);
  }

  // Phaser.GameObjects.Image has setDisplaySize; GIFImage (RenderTexture) uses setScale
  const obj = go as TweenableGO & {
    setDisplaySize?: (w: number, h: number) => unknown;
    width: number;
    height: number;
  };

  if (typeof obj.setDisplaySize === 'function') {
    obj.setDisplaySize(targetW, targetH);
  } else {
    go.scaleX = targetW / nativeW;
    go.scaleY = targetH / nativeH;
  }
}

// --------------------------------------------------------------------------
// AssetManager
// --------------------------------------------------------------------------

/** Live state for one animated GIF — used to copy frames each tick */
interface GifEntry {
  img: HTMLImageElement;
  canvasTexture: Phaser.Textures.CanvasTexture;
}

export class AssetManager {
  private readonly scene: Phaser.Scene;
  private readonly animationSystem: AnimationSystem;
  private readonly registry = new Map<string, AssetRecord>();
  /** Tracks hidden <img> elements and canvas textures for animated GIFs */
  private readonly gifRegistry = new Map<string, GifEntry>();

  constructor(scene: Phaser.Scene, animationSystem: AnimationSystem) {
    this.scene = scene;
    this.animationSystem = animationSystem;
  }

  // --------------------------------------------------------------------------
  // Public API — one method per overlay.* topic
  // --------------------------------------------------------------------------

  spawn(payload: OverlaySpawnPayload): void {
    const {
      assetId,
      src,
      position,
      width,
      height,
      depth = 0,
      enterAnimation = 'none',
      enterDuration = 300,
      lifetime,
      exitAnimation = 'none',
      exitDuration = 300,
    } = payload;

    // Spawning with an existing ID replaces the current asset immediately
    if (this.registry.has(assetId)) {
      this.destroyRecord(assetId);
    }

    const isGif = src.toLowerCase().endsWith('.gif');

    // exactOptionalPropertyTypes: optional fields must be absent, not `undefined`.
    // Spread only the fields that are actually defined.
    const optionals = {
      ...(width    !== undefined ? { width }    : {}),
      ...(height   !== undefined ? { height }   : {}),
      ...(lifetime !== undefined ? { lifetime } : {}),
    };

    const spawnBase = {
      assetId, src, position, depth,
      enterAnimation, enterDuration,
      exitAnimation, exitDuration,
      ...optionals,
    };

    if (isGif) {
      this.spawnGif(spawnBase);
    } else {
      this.spawnImage(spawnBase);
    }
  }

  move(payload: OverlayMovePayload): void {
    const { assetId, position, duration = 300 } = payload;
    const record = this.registry.get(assetId);

    if (!record) {
      console.warn(`[AssetManager] overlay.move — unknown assetId "${assetId}"`);
      return;
    }

    this.animationSystem.moveTo(record.gameObject, position.x, position.y, duration);
  }

  animate(payload: OverlayAnimatePayload): void {
    const { assetId, animation, duration = 300 } = payload;
    const record = this.registry.get(assetId);

    if (!record) {
      console.warn(`[AssetManager] overlay.animate — unknown assetId "${assetId}"`);
      return;
    }

    this.animationSystem.apply(record.gameObject, animation, duration);
  }

  remove(payload: OverlayRemovePayload): void {
    const { assetId, exitAnimation = 'none', exitDuration = 300 } = payload;
    const record = this.registry.get(assetId);

    if (!record) {
      console.warn(`[AssetManager] overlay.remove — unknown assetId "${assetId}"`);
      return;
    }

    this.removeWithAnimation(assetId, exitAnimation, exitDuration);
  }

  clear(payload: OverlayClearPayload): void {
    const { assetIdPrefix, exitAnimation = 'none', exitDuration = 300 } = payload;

    const idsToRemove: string[] = [];
    for (const assetId of this.registry.keys()) {
      if (!assetIdPrefix || assetId.startsWith(assetIdPrefix)) {
        idsToRemove.push(assetId);
      }
    }

    for (const assetId of idsToRemove) {
      this.removeWithAnimation(assetId, exitAnimation, exitDuration);
    }

    console.log(
      `[AssetManager] overlay.clear — removing ${idsToRemove.length} asset(s)` +
      (assetIdPrefix ? ` (prefix: "${assetIdPrefix}")` : ' (all)'),
    );
  }

  /**
   * Returns the current registry record for a given assetId.
   * Used by other systems that need to query live asset state.
   */
  getAsset(assetId: string): Readonly<AssetRecord> | undefined {
    return this.registry.get(assetId);
  }

  /**
   * Called by OverlayScene.update() every game frame.
   * Copies the current GIF frame from each hidden <img> to its CanvasTexture,
   * refreshing the WebGL texture so the animation plays in Phaser.
   */
  updateGifTextures(): void {
    for (const [assetId, entry] of this.gifRegistry) {
      // If the asset was removed since the last tick, clean up and skip
      if (!this.registry.has(assetId)) {
        this.destroyGifEntry(entry);
        this.gifRegistry.delete(assetId);
        continue;
      }
      entry.canvasTexture.context.drawImage(entry.img, 0, 0);
      entry.canvasTexture.refresh();
    }
  }

  /**
   * Destroy all tracked assets without animations.
   * Called by OverlayScene on shutdown.
   */
  destroyAll(): void {
    for (const assetId of [...this.registry.keys()]) {
      this.destroyRecord(assetId);
    }
    // Defensive: clean up any GIF entries that survived (should be none)
    for (const entry of this.gifRegistry.values()) {
      entry.img.remove();
      // Do not call scene.textures.remove — scene may already be shutting down
    }
    this.gifRegistry.clear();
  }

  // --------------------------------------------------------------------------
  // Private — spawning
  // --------------------------------------------------------------------------

  private spawnImage(opts: SpawnOpts): void {
    const {
      assetId, src, position, width, height, depth,
      enterAnimation, enterDuration,
      lifetime, exitAnimation, exitDuration,
    } = opts;

    // Phaser texture key = src path (Phaser caches by key; same src = same cache hit)
    const textureKey = src;

    const doSpawn = () => {
      if (!this.scene.sys.isActive()) return;

      const go = this.scene.add.image(position.x, position.y, textureKey);
      go.setDepth(depth);
      applySize(go as unknown as TweenableGO, go.width, go.height, width, height);

      this.registerRecord(assetId, go as unknown as TweenableGO, src, false, lifetime, exitAnimation, exitDuration);
      this.animationSystem.applyEnter(go as unknown as TweenableGO, enterAnimation, enterDuration, position);

      console.log(`[AssetManager] Spawned image "${assetId}" at (${position.x}, ${position.y})`);
    };

    if (this.scene.textures.exists(textureKey)) {
      doSpawn();
    } else {
      this.scene.load.image(textureKey, src);
      this.scene.load.once(`filecomplete-image-${textureKey}`, doSpawn);
      this.scene.load.once('loaderror', (file: Phaser.Loader.File) => {
        if (file.key === textureKey) {
          console.error(`[AssetManager] Failed to load image: "${src}"`);
        }
      });
      this.scene.load.start();
    }
  }

  private spawnGif(opts: SpawnOpts): void {
    const {
      assetId, src, position, width, height, depth,
      enterAnimation, enterDuration,
      lifetime, exitAnimation, exitDuration,
    } = opts;

    const textureKey = `__gif_canvas_${assetId}`;

    // Create a hidden <img> element. The browser animates it automatically.
    // position:fixed + left:-9999px keeps it in the DOM (required for animation)
    // while keeping it invisible and out of the layout.
    const img = document.createElement('img');
    img.style.cssText = 'position:fixed;left:-9999px;top:-9999px;pointer-events:none;opacity:0;';
    document.body.appendChild(img);

    const doSpawn = () => {
      if (!this.scene.sys.isActive()) {
        img.remove();
        return;
      }

      const nativeW = img.naturalWidth > 0 ? img.naturalWidth : 100;
      const nativeH = img.naturalHeight > 0 ? img.naturalHeight : 100;

      // Create a CanvasTexture at the GIF's native resolution.
      // OverlayScene.update() will call updateGifTextures() every frame to
      // draw the current GIF frame from <img> to this canvas, keeping it animated.
      const canvasTexture = this.scene.textures.createCanvas(textureKey, nativeW, nativeH);
      if (canvasTexture === null) {
        console.error(`[AssetManager] Failed to create canvas texture for GIF "${assetId}"`);
        img.remove();
        return;
      }
      canvasTexture.context.drawImage(img, 0, 0);
      canvasTexture.refresh();

      const go = this.scene.add.image(position.x, position.y, textureKey);
      go.setDepth(depth);
      applySize(go as unknown as TweenableGO, nativeW, nativeH, width, height);

      this.gifRegistry.set(assetId, { img, canvasTexture });
      this.registerRecord(assetId, go as unknown as TweenableGO, src, true, lifetime, exitAnimation, exitDuration);
      this.animationSystem.applyEnter(go as unknown as TweenableGO, enterAnimation, enterDuration, position);

      console.log(`[AssetManager] Spawned animated GIF "${assetId}" at (${position.x}, ${position.y})`);
    };

    // If the browser already has the image cached, onload may not fire
    if (img.complete && img.naturalWidth > 0) {
      // Set src after checking complete to avoid a race on cached images
      img.src = src;
      doSpawn();
    } else {
      img.addEventListener('load', doSpawn, { once: true });
      img.addEventListener('error', () => {
        console.error(`[AssetManager] Failed to load GIF: "${src}"`);
        img.remove();
      }, { once: true });
      img.src = src;
    }
  }

  // --------------------------------------------------------------------------
  // Private — registry and lifecycle
  // --------------------------------------------------------------------------

  private registerRecord(
    assetId: string,
    gameObject: TweenableGO,
    src: string,
    isGif: boolean,
    lifetime: number | undefined,
    exitAnimation: AnimationPreset,
    exitDuration: number,
  ): void {
    let ttlTimer: ReturnType<typeof setTimeout> | null = null;

    if (lifetime !== undefined && lifetime > 0) {
      ttlTimer = setTimeout(() => {
        this.removeWithAnimation(assetId, exitAnimation, exitDuration);
      }, lifetime);
    }

    this.registry.set(assetId, {
      assetId,
      gameObject,
      src,
      isGif,
      ttlTimer,
      ttlExitAnimation: exitAnimation,
      ttlExitDuration: exitDuration,
    });
  }

  /**
   * Begin removing an asset: delete from registry immediately (so subsequent
   * messages targeting this ID are correctly treated as unknown), then play
   * the exit animation and destroy the game object when it finishes.
   *
   * Deleting from the registry first means:
   *   - If TTL fires AND overlay.remove arrives simultaneously, only one fires
   *   - A re-spawn with the same ID during the exit animation creates a fresh
   *     asset; the dying game object cleans itself up via the callback
   */
  private removeWithAnimation(
    assetId: string,
    exitAnimation: AnimationPreset,
    exitDuration: number,
  ): void {
    const record = this.registry.get(assetId);
    if (!record) return;

    // Pull out of registry immediately
    this.registry.delete(assetId);
    if (record.ttlTimer !== null) clearTimeout(record.ttlTimer);

    // Capture GIF entry now — stop the update loop from refreshing it, but
    // keep the canvas texture alive until the game object is actually destroyed
    // (so the exit animation still has a valid texture to render).
    const gifEntry = record.isGif ? this.gifRegistry.get(assetId) : undefined;
    if (gifEntry !== undefined) this.gifRegistry.delete(assetId);

    const { gameObject } = record;

    if (exitAnimation === 'none' || exitDuration === 0) {
      this.animationSystem.killTweens(gameObject);
      if (gameObject.active) gameObject.destroy();
      if (gifEntry !== undefined) this.destroyGifEntry(gifEntry);
      return;
    }

    this.animationSystem.apply(gameObject, exitAnimation, exitDuration, () => {
      if (gameObject.active) gameObject.destroy();
      if (gifEntry !== undefined) this.destroyGifEntry(gifEntry);
    });
  }

  /**
   * Immediate destroy — no animation. Kills active tweens and destroys the
   * game object. Used when replacing an asset on re-spawn.
   */
  private destroyRecord(assetId: string): void {
    const record = this.registry.get(assetId);
    if (!record) return;

    this.registry.delete(assetId);
    if (record.ttlTimer !== null) clearTimeout(record.ttlTimer);
    this.animationSystem.killTweens(record.gameObject);
    if (record.gameObject.active) record.gameObject.destroy();

    if (record.isGif) {
      const gifEntry = this.gifRegistry.get(assetId);
      if (gifEntry !== undefined) {
        this.gifRegistry.delete(assetId);
        this.destroyGifEntry(gifEntry);
      }
    }
  }

  /**
   * Cleans up a GIF entry: removes the hidden <img> from the DOM and
   * destroys the CanvasTexture from the Phaser texture cache.
   */
  private destroyGifEntry(entry: GifEntry): void {
    entry.img.remove();
    if (this.scene.textures.exists(entry.canvasTexture.key)) {
      this.scene.textures.remove(entry.canvasTexture);
    }
  }
}

// --------------------------------------------------------------------------
// Internal type — shared between spawnImage and spawnGif
// --------------------------------------------------------------------------

interface SpawnOpts {
  assetId: string;
  src: string;
  position: { x: number; y: number };
  width?: number;
  height?: number;
  depth: number;
  enterAnimation: AnimationPreset;
  enterDuration: number;
  lifetime?: number;
  exitAnimation: AnimationPreset;
  exitDuration: number;
}

/**
 * AnimationSystem — wraps Phaser tweens into a clean preset API.
 *
 * All AnimationPreset strings defined in @stream-overlay/shared are handled
 * here. Every preset resolves to one or more Phaser tweens. The caller never
 * constructs tween configs directly — it picks a named preset.
 *
 * Design rules:
 *   - Calling any method always kills existing tweens on the target first.
 *     This gives "mid-animation interrupt" for free: send a new overlay.move
 *     while a move is in progress and the tween smoothly redirects from the
 *     current position.
 *   - onComplete callbacks are safe to omit — all methods accept an optional
 *     callback and invoke it when the animation finishes.
 *   - slide-in-* presets are enter-only (they need an offscreen start position).
 *     They are handled by applyEnter(), not apply(). Calling apply() with a
 *     slide-in preset logs a warning and calls onComplete immediately.
 *
 * Used by AssetManager — not called directly from OverlayScene.
 */

import Phaser from 'phaser';
import type { AnimationPreset } from '@stream-overlay/shared';

// Canvas dimensions — used by slide-in presets to position the offscreen start
const CANVAS_WIDTH = 1920;
const CANVAS_HEIGHT = 1080;

// Offscreen buffer so assets fully clear the canvas edge before entering
const SLIDE_BUFFER = 200;

/**
 * Minimal interface for any game object that supports tween-able properties.
 * Phaser.GameObjects.Image and phaser3-rex-plugins GIFImage (RenderTexture)
 * both satisfy this shape.
 */
export interface TweenableGO extends Phaser.GameObjects.GameObject {
  x: number;
  y: number;
  alpha: number;
  angle: number;
  scaleX: number;
  scaleY: number;
}

export class AnimationSystem {
  private readonly scene: Phaser.Scene;

  constructor(scene: Phaser.Scene) {
    this.scene = scene;
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  /**
   * Kill all active tweens on a game object.
   * Called by AssetManager before destroy — prevents stale onComplete callbacks
   * firing after the game object no longer exists.
   */
  killTweens(go: Phaser.GameObjects.GameObject): void {
    this.scene.tweens.killTweensOf(go);
  }

  /**
   * Tween an asset to a new canvas position.
   * Interrupts any in-progress tween on the target — the tween picks up from
   * the current x/y so the transition is always smooth.
   *
   * duration = 0 → instant snap (no tween created).
   */
  moveTo(
    go: Phaser.GameObjects.GameObject,
    x: number,
    y: number,
    duration: number,
    onComplete?: () => void,
  ): void {
    this.scene.tweens.killTweensOf(go);

    if (duration === 0) {
      const obj = go as TweenableGO;
      obj.x = x;
      obj.y = y;
      onComplete?.();
      return;
    }

    this.scene.tweens.add({
      targets: go,
      x,
      y,
      duration,
      ease: 'Power2.easeOut',
      onComplete: () => onComplete?.(),
    });
  }

  /**
   * Apply an AnimationPreset to a game object.
   * Kills any existing tweens first. Calls onComplete when the animation ends.
   *
   * This is used for:
   *   - overlay.animate messages (in-place effects)
   *   - exit animations on overlay.remove / TTL expiry
   *
   * Do NOT use for enter animations that need a start position — use applyEnter().
   */
  apply(
    go: Phaser.GameObjects.GameObject,
    preset: AnimationPreset,
    duration: number,
    onComplete?: () => void,
  ): void {
    this.scene.tweens.killTweensOf(go);

    switch (preset) {
      case 'fade-in':
        this.fadeIn(go, duration, onComplete);
        break;

      case 'fade-out':
        this.fadeOut(go, duration, onComplete);
        break;

      case 'bounce':
        this.bounce(go, duration, onComplete);
        break;

      case 'shake':
        this.shake(go, duration, onComplete);
        break;

      case 'pulse':
        this.pulse(go, duration, onComplete);
        break;

      case 'spin':
        this.spin(go, duration, onComplete);
        break;

      case 'slide-in-left':
      case 'slide-in-right':
      case 'slide-in-top':
      case 'slide-in-bottom':
        // Slide-in presets require an offscreen start position — use applyEnter().
        // If sent via overlay.animate mid-scene, treat as no-op to avoid
        // teleporting the asset offscreen.
        console.warn(
          `[AnimationSystem] "${preset}" is an enter-only preset — ` +
          'use applyEnter() or send it as enterAnimation on overlay.spawn. ' +
          'Ignoring.',
        );
        onComplete?.();
        break;

      case 'none':
        onComplete?.();
        break;

      default: {
        // TypeScript exhaustiveness guard — should never reach here
        const _exhaustive: never = preset;
        console.warn(`[AnimationSystem] Unknown preset: ${String(_exhaustive)}`);
        onComplete?.();
      }
    }
  }

  /**
   * Apply an enter animation to a freshly-spawned asset.
   * Handles slide-in presets that need to reposition the object offscreen
   * before tweening it in.
   *
   * targetPosition is the final resting position the asset should arrive at.
   * For non-slide presets, the asset is already at targetPosition when this
   * is called.
   */
  applyEnter(
    go: Phaser.GameObjects.GameObject,
    preset: AnimationPreset,
    duration: number,
    targetPosition: { x: number; y: number },
  ): void {
    if (preset === 'none') return;

    const obj = go as TweenableGO;

    switch (preset) {
      case 'slide-in-left':
        obj.x = -(SLIDE_BUFFER);
        obj.y = targetPosition.y;
        this.scene.tweens.add({
          targets: go,
          x: targetPosition.x,
          duration,
          ease: 'Power2.easeOut',
        });
        break;

      case 'slide-in-right':
        obj.x = CANVAS_WIDTH + SLIDE_BUFFER;
        obj.y = targetPosition.y;
        this.scene.tweens.add({
          targets: go,
          x: targetPosition.x,
          duration,
          ease: 'Power2.easeOut',
        });
        break;

      case 'slide-in-top':
        obj.x = targetPosition.x;
        obj.y = -(SLIDE_BUFFER);
        this.scene.tweens.add({
          targets: go,
          y: targetPosition.y,
          duration,
          ease: 'Power2.easeOut',
        });
        break;

      case 'slide-in-bottom':
        obj.x = targetPosition.x;
        obj.y = CANVAS_HEIGHT + SLIDE_BUFFER;
        this.scene.tweens.add({
          targets: go,
          y: targetPosition.y,
          duration,
          ease: 'Power2.easeOut',
        });
        break;

      default:
        // All other presets don't need a start position — delegate to apply()
        this.apply(go, preset, duration);
    }
  }

  // --------------------------------------------------------------------------
  // Private — animation implementations
  // --------------------------------------------------------------------------

  private fadeIn(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    (go as TweenableGO).alpha = 0;
    this.scene.tweens.add({
      targets: go,
      alpha: 1,
      duration,
      ease: 'Linear',
      onComplete: () => onComplete?.(),
    });
  }

  private fadeOut(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    this.scene.tweens.add({
      targets: go,
      alpha: 0,
      duration,
      ease: 'Linear',
      onComplete: () => onComplete?.(),
    });
  }

  private bounce(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    const obj = go as TweenableGO;
    const originY = obj.y;
    // Pop up 30px then snap back — two quick yoyos for a satisfying bounce feel
    this.scene.tweens.add({
      targets: go,
      y: originY - 30,
      duration: duration * 0.35,
      ease: 'Power2.easeOut',
      yoyo: true,
      repeat: 1,
      onComplete: () => {
        obj.y = originY; // snap to origin in case of float drift
        onComplete?.();
      },
    });
  }

  private shake(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    const obj = go as TweenableGO;
    const originX = obj.x;
    const stepMs = 50; // each half-oscillation duration
    const repeatCount = Math.max(1, Math.floor(duration / (stepMs * 2)));

    this.scene.tweens.add({
      targets: go,
      x: { from: originX - 8, to: originX + 8 },
      duration: stepMs,
      ease: 'Linear',
      yoyo: true,
      repeat: repeatCount,
      onComplete: () => {
        obj.x = originX; // restore exact origin
        onComplete?.();
      },
    });
  }

  private pulse(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    // Scale up to 115% and back — single cycle
    this.scene.tweens.add({
      targets: go,
      scaleX: 1.15,
      scaleY: 1.15,
      duration: duration * 0.5,
      ease: 'Sine.easeInOut',
      yoyo: true,
      onComplete: () => onComplete?.(),
    });
  }

  private spin(
    go: Phaser.GameObjects.GameObject,
    duration: number,
    onComplete?: () => void,
  ): void {
    this.scene.tweens.add({
      targets: go,
      angle: '+=360',
      duration,
      ease: 'Linear',
      onComplete: () => onComplete?.(),
    });
  }
}

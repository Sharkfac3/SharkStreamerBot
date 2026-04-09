/**
 * AudioManager — handles MP3 playback via Phaser's WebAudio system.
 *
 * Responds to:
 *   overlay.audio.play  — load (if needed) and play an MP3 by soundId
 *   overlay.audio.stop  — stop one sound by soundId, or all sounds if omitted
 *
 * Sound IDs follow the same convention as asset IDs:
 *   Ephemeral sounds : UUID v4   (one-shot alerts)
 *   Looping ambient  : slug      ("ambient-engine", "lotat-theme")
 *
 * Spawning a sound with an existing soundId stops and replaces it.
 *
 * OBS note: browser source audio requires "Control audio via OBS" to be
 * checked in the browser source properties. Without this, Web Audio output
 * does not route to the stream capture. See overlay.md for setup instructions.
 *
 * Used by OverlayScene — not called directly from broker handlers.
 */

import Phaser from 'phaser';
import type { OverlayAudioPlayPayload, OverlayAudioStopPayload } from '@stream-overlay/shared';

interface SoundRecord {
  soundId: string;
  sound: Phaser.Sound.BaseSound;
  src: string;
}

export class AudioManager {
  private readonly scene: Phaser.Scene;
  private readonly registry = new Map<string, SoundRecord>();

  constructor(scene: Phaser.Scene) {
    this.scene = scene;
  }

  // --------------------------------------------------------------------------
  // Public API
  // --------------------------------------------------------------------------

  play(payload: OverlayAudioPlayPayload): void {
    const { soundId, src, volume = 1, loop = false } = payload;

    // Replace any existing sound under this ID
    if (this.registry.has(soundId)) {
      this.stopSound(soundId);
    }

    // Use src as the cache key — Phaser audio and texture caches are separate
    // so there is no collision with image keys that share the same path.
    const cacheKey = src;

    const doPlay = () => {
      if (!this.scene.sys.isActive()) return;

      const sound = this.scene.sound.add(cacheKey, { volume, loop });
      sound.play();

      // Non-looping sounds clean themselves up from the registry when done
      if (!loop) {
        sound.once('complete', () => {
          this.registry.delete(soundId);
        });
      }

      this.registry.set(soundId, { soundId, sound, src });
      console.log(`[AudioManager] Playing "${soundId}" (${src}) vol=${volume} loop=${loop}`);
    };

    if (this.scene.cache.audio.exists(cacheKey)) {
      doPlay();
    } else {
      this.scene.load.audio(cacheKey, src);
      // filecomplete-audio-{key} fires when this specific file finishes loading
      this.scene.load.once(`filecomplete-audio-${cacheKey}`, doPlay);
      this.scene.load.once(`loaderror`, (file: Phaser.Loader.File) => {
        if (file.key === cacheKey) {
          console.error(`[AudioManager] Failed to load audio: "${src}"`);
        }
      });
      this.scene.load.start();
    }
  }

  stop(payload: OverlayAudioStopPayload): void {
    const { soundId } = payload;

    if (soundId === undefined) {
      // Stop everything
      for (const id of [...this.registry.keys()]) {
        this.stopSound(id);
      }
      console.log('[AudioManager] Stopped all sounds');
    } else {
      if (!this.registry.has(soundId)) {
        console.warn(`[AudioManager] overlay.audio.stop — unknown soundId "${soundId}"`);
        return;
      }
      this.stopSound(soundId);
    }
  }

  /**
   * Stop all playing sounds without requiring a payload.
   * Called by OverlayScene on shutdown to prevent audio leaking after reload.
   */
  stopAll(): void {
    for (const id of [...this.registry.keys()]) {
      this.stopSound(id);
    }
  }

  // --------------------------------------------------------------------------
  // Private
  // --------------------------------------------------------------------------

  private stopSound(soundId: string): void {
    const record = this.registry.get(soundId);
    if (!record) return;

    record.sound.stop();
    record.sound.destroy();
    this.registry.delete(soundId);
    console.log(`[AudioManager] Stopped "${soundId}"`);
  }
}

/**
 * Minimal type declarations for phaser3-rex-plugins GIF support.
 *
 * STATUS (2026-04-08): phaser3-rex-plugins 1.80.x does NOT ship
 * plugins/gifimage.js — the file does not exist in the npm package.
 * asset-manager.ts no longer imports this module; spawnGif() falls back
 * to the static image loader (first frame only).
 *
 * This declaration is kept so the module shape is documented and the
 * import can be restored without research when a working GIF plugin is found.
 * If you restore the import, remove this comment block.
 */

declare module 'phaser3-rex-plugins/plugins/gifimage.js' {
  import Phaser from 'phaser';

  /**
   * GIFImage — a Phaser game object that plays an animated GIF.
   *
   * Extends RenderTexture, so all standard Phaser transform properties
   * (x, y, alpha, scaleX, scaleY, angle, depth) work, and Phaser tweens
   * can animate any of them.
   *
   * Loading is async and internal — the game object is created immediately
   * but the GIF frames are not available until the 'load' event fires.
   */
  class GIFImage extends Phaser.GameObjects.RenderTexture {
    constructor(
      scene: Phaser.Scene,
      x: number,
      y: number,
      config: {
        /** Path or URL to the .gif file */
        src: string;
        /** Start playing immediately after load. Defaults to true. */
        autoPlay?: boolean;
        /** Loop the animation indefinitely. Defaults to true. */
        loop?: boolean;
      },
    );

    /** Start or resume playback */
    play(): this;
    /** Pause playback */
    pause(): this;
    /** Stop playback and reset to first frame */
    stop(): this;
    /** Set auto-play on load */
    setAutoPlay(value: boolean): this;
  }

  export default GIFImage;
}

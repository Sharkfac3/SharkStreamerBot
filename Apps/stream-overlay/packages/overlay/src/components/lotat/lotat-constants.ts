/**
 * lotat-constants.ts
 *
 * Centralised layout, colour, and font constants for the LotAT visual layer.
 *
 * All LotAT components read from here.  Change a value once and every
 * component picks it up — no hunting through individual files.
 *
 * Design decisions (Prompt 06):
 *   Aesthetic  — retro terminal: dark panels, Courier New, green text.
 *                Zero image assets required for v1.
 *   Layout     — lower third (y ≥ ZONE_Y).  Stream content visible above.
 *   Depth      — LotAT elements live at depth 20-29 (see asset-system.md).
 */

// ---------------------------------------------------------------------------
// Layout — all pixel values assume a 1920 × 1080 canvas
// ---------------------------------------------------------------------------

export const LOTAT_LAYOUT = {
  canvasW: 1920,
  canvasH: 1080,

  /** Y coordinate where the LotAT lower-third zone begins */
  zoneY: 670,

  /** Height of the entire LotAT zone (670 → 1080) */
  zoneH: 410,

  /** Horizontal inset from canvas edges for padded elements */
  marginX: 60,

  // ── Info strip (location / crew / chaos / timer) ──────────────────────
  stripY: 678,
  stripH: 36,

  // ── Narration panel ──────────────────────────────────────────────────
  narrationY: 718,
  narrationH: 136,

  // ── Choice cards row ──────────────────────────────────────────────────
  choicesY: 862,
  choicesH: 110,
  choiceGap: 16,   // gap between adjacent cards

  // ── Join panel (centred inside lower zone) ────────────────────────────
  joinW: 900,
  joinH: 300,

  // ── Depth values ─────────────────────────────────────────────────────
  depthBase:    20,   // base dark panel that frames the lower third
  depthStrip:   21,   // location / crew / chaos strip
  depthContent: 22,   // narration text, choice cards
  depthOverlay: 24,   // join panel, dice, commander (sit above narration)
  depthEnding:  26,   // ending screen (topmost LotAT element)
} as const;

// ---------------------------------------------------------------------------
// Colours
// ---------------------------------------------------------------------------

/** Use hex numbers (0xRRGGBB) for Phaser Rectangle / Graphics fill colours. */
export const LOTAT_HEX = {
  panelDark:    0x000000,   // primary dark panel background
  panelAccent:  0x001800,   // subtle green-tinted panel (info strip)
  choiceBg:     0x001200,   // choice card background
  choiceHover:  0x003300,   // choice card winner highlight
  barBg:        0x002200,   // chaos bar track background
  barFill:      0x00aa44,   // chaos bar fill (low chaos)
  barMid:       0xaaaa00,   // chaos bar fill (mid chaos)
  barHigh:      0xaa2200,   // chaos bar fill (high chaos)
  success:      0x003300,   // ending panel bg for success
  partial:      0x332200,   // ending panel bg for partial
  failure:      0x330000,   // ending panel bg for failure
} as const;

/** Alpha values paired with LOTAT_HEX fills. */
export const LOTAT_ALPHA = {
  panel:   0.88,
  strip:   0.92,
  choice:  0.90,
  overlay: 0.93,
  ending:  0.95,
} as const;

/** CSS colour strings for Phaser.GameObjects.Text `color` style. */
export const LOTAT_COLOR = {
  primary:   '#00FF88',   // terminal green — main narration / labels
  dim:       '#557755',   // muted green — secondary labels
  accent:    '#FFD700',   // gold — crew name, timer, highlights
  cmd:       '#00CCFF',   // cyan — chat command text (!scan, !roll …)
  white:     '#FFFFFF',   // pure white — vote counts, roll values
  danger:    '#FF4444',   // red — failure, high chaos
  success:   '#00FF88',   // green — success ending
  partial:   '#FFD700',   // gold — partial ending
  failure:   '#FF5555',   // red — failure ending
  winner:    '#FFD700',   // gold — winning choice highlight
} as const;

// ---------------------------------------------------------------------------
// Fonts
// ---------------------------------------------------------------------------

export const LOTAT_FONT = {
  family: 'Courier New',

  sizeXS:  '14px',
  sizeSM:  '18px',
  sizeMD:  '22px',
  sizeLG:  '28px',
  sizeXL:  '40px',
  sizeXXL: '64px',
} as const;

// ---------------------------------------------------------------------------
// Chaos meter
// ---------------------------------------------------------------------------

/** Visual cap for the chaos bar — chaos beyond this fills the bar fully. */
export const CHAOS_MAX_VISUAL = 20;

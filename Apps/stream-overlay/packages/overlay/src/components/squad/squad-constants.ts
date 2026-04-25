/**
 * squad-constants.ts
 *
 * Centralised layout, colour, and font constants for all squad game visuals.
 * Mirrors the pattern from lotat-constants.ts.
 *
 * Design decisions (Prompt 07):
 *   Aesthetic  — retro terminal, same font/panel style as LotAT.
 *                Game-specific accent colours differentiate each game.
 *   Layout     — top-left panel (y < 250). LotAT owns the lower third (y ≥ 670).
 *                Toothless pop-up sits upper-right so it doesn't collide.
 *   Depth      — squad elements live at depth 30-39 (above LotAT 20-29).
 */

// ---------------------------------------------------------------------------
// Layout — all pixel values assume a 1920 × 1080 canvas
// ---------------------------------------------------------------------------

export const SQUAD_LAYOUT = {
  canvasW: 1920,
  canvasH: 1080,

  // ── Main squad panel (Duck / Pedro / Clone) — top-left ───────────────────
  panelX:  20,
  panelY:  20,
  panelW:  720,   // duck / pedro panel width
  panelH:  200,
  marginX: 20,    // inner horizontal padding
  marginY: 16,    // inner vertical padding

  // Clone panel is wider (5 position boxes)
  clonePanelW: 880,
  clonePanelH: 220,

  // Clone position boxes
  cloneBoxW:  152,  // per-box width
  cloneBoxH:   84,  // per-box height
  cloneBoxGap:  12,  // gap between adjacent boxes
  cloneBoxY:   100, // y offset inside panel for boxes

  // ── Toothless pop-up — upper-right ───────────────────────────────────────
  toothlessX:  1560,
  toothlessY:    20,
  toothlessW:   340,
  toothlessH:   170,

  // ── Shared strip layout (banner row inside panel) ──────────────────────
  bannerH: 36,
  bannerY: 20,     // y offset inside panel for banner row

  // ── Progress bar ──────────────────────────────────────────────────────────
  barH: 12,
  barY: 118,       // y offset inside panel

  // ── Depth values (squad elements: 30-39) ──────────────────────────────────
  depthBase:    30,   // dark background panel
  depthContent: 31,   // text elements, boxes
  depthBar:     32,   // progress bars
  depthPopup:   35,   // toothless pop-up (above everything)
} as const;

// ---------------------------------------------------------------------------
// Colours — hex numbers for Phaser fills, CSS strings for Text objects
// ---------------------------------------------------------------------------

export const SQUAD_HEX = {
  panelDark:   0x000000,   // primary dark panel
  panelAccent: 0x000818,   // very dark blue tint for content panels
  barBg:       0x001122,   // progress bar track
  boxBg:       0x001122,   // position box background
  boxElim:     0x110000,   // eliminated box background
  boxBorder:   0x224444,   // active box border colour
  boxElimBorder: 0x330000, // eliminated box border colour
} as const;

export const SQUAD_ALPHA = {
  panel:   0.88,
  content: 0.90,
  popup:   0.93,
} as const;

export const SQUAD_COLOR = {
  // Shared
  primary:  '#00FF88',   // terminal green — main text
  dim:      '#446655',   // muted secondary text
  white:    '#FFFFFF',   // counts, values
  danger:   '#FF4444',   // failure
  success:  '#00FF88',   // success (same as primary)

  // Per-game accent colours
  duck:     '#00CCFF',   // cyan
  pedro:    '#FF69B4',   // hot pink
  clone:    '#FFD700',   // gold
  toothless: '#CC88FF',  // purple

  // Toothless rarity colours
  rarityRegular: '#AAAAAA',
  raritySmol:    '#44AAFF',
  rarityLong:    '#44FF88',
  rarityFlight:  '#FFD700',
  rarityParty:   '#FF44FF',
} as const;

// ---------------------------------------------------------------------------
// Fonts — reuse the LotAT terminal font family
// ---------------------------------------------------------------------------

export const SQUAD_FONT = {
  family: 'Courier New',

  sizeXS:  '14px',
  sizeSM:  '18px',
  sizeMD:  '22px',
  sizeLG:  '28px',
  sizeXL:  '40px',
} as const;

// ── Clone Grid Game ────────────────────────────────────────────────────────
export const CLONE_GRID = {
  cols:      32,
  rows:      18,
  cellSize:  52,
  originX:   128,   // pixel x of top-left corner of cell (1,1)
  originY:   72,    // pixel y of top-left corner of cell (1,1)
  hudHeight: 64,
} as const;

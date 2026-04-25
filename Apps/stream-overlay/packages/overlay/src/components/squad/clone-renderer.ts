import Phaser from 'phaser';
import type {
  CloneGridCell,
  CloneGridPlayer,
  SquadCloneGridEndResult,
  SquadCloneGridStartPayload,
  SquadCloneGridUpdateState,
} from '@stream-overlay/shared';
import { CLONE_GRID, SQUAD_COLOR, SQUAD_FONT } from './squad-constants';

type GamePhase = 'idle' | 'join' | 'game' | 'ended';

type PlayerVisual = {
  rect: Phaser.GameObjects.Rectangle;
  label: Phaser.GameObjects.Text;
  indicator: Phaser.GameObjects.Text;
};

const DEPTH = {
  bg: 30,
  grid: 31,
  cells: 32,
  labels: 33,
  hud: 34,
  flash: 35,
  join: 36,
  result: 37,
} as const;

const CANVAS_W = 1920;
const CANVAS_H = 1080;
const GAME_DURATION_SECONDS = 5 * 60;
const EMPIRE_FILL = 0x8b0000;
const PLAYER_FILL = 0x003300;
const GRID_LINE = 0x1a1a2e;
const BG_NEAR_BLACK = 0x05070d;
const JOIN_PANEL_BG = 0x000000;
const RESULT_CARD_BG = 0x090d14;

export class CloneRenderer {
  private players: CloneGridPlayer[] = [];
  private empire: CloneGridCell[] = [];
  private elapsedSeconds = 0;
  private gamePhase: GamePhase = 'idle';

  private readonly empireRects = new Map<string, Phaser.GameObjects.Rectangle>();
  private readonly empireIndicators = new Map<string, Phaser.GameObjects.Text>();
  private readonly playerVisuals = new Map<string, PlayerVisual>();
  private readonly gridLines: Phaser.GameObjects.Rectangle[] = [];

  private readonly gameBackground: Phaser.GameObjects.Rectangle;
  private readonly hudBackground: Phaser.GameObjects.Rectangle;
  private readonly survivorText: Phaser.GameObjects.Text;
  private readonly timerText: Phaser.GameObjects.Text;
  private readonly empireText: Phaser.GameObjects.Text;

  private readonly joinOverlay: Phaser.GameObjects.Rectangle;
  private readonly joinTitle: Phaser.GameObjects.Text;
  private readonly joinSubtitle: Phaser.GameObjects.Text;
  private readonly joinPlayersHeader: Phaser.GameObjects.Text;
  private readonly joinPlayersText: Phaser.GameObjects.Text;
  private readonly joinCountdownText: Phaser.GameObjects.Text;

  private readonly resultOverlay: Phaser.GameObjects.Rectangle;
  private readonly resultCard: Phaser.GameObjects.Rectangle;
  private readonly resultTitle: Phaser.GameObjects.Text;
  private readonly resultList: Phaser.GameObjects.Text;

  private readonly flashText: Phaser.GameObjects.Text;

  private joinCountdownSeconds = 0;
  private joinScrollOffset = 0;
  private hideResultCall: Phaser.Time.TimerEvent | undefined;
  private joinCountdownEvent: Phaser.Time.TimerEvent | undefined;
  private gameTimerEvent: Phaser.Time.TimerEvent | undefined;

  constructor(private readonly scene: Phaser.Scene) {
    this.gameBackground = scene.add
      .rectangle(0, 0, CANVAS_W, CANVAS_H, BG_NEAR_BLACK, 0.92)
      .setOrigin(0, 0)
      .setDepth(DEPTH.bg)
      .setVisible(false);

    this.buildGridLines();

    this.hudBackground = scene.add
      .rectangle(0, 0, CANVAS_W, CLONE_GRID.hudHeight, 0x000000, 0.85)
      .setOrigin(0, 0)
      .setDepth(DEPTH.hud)
      .setVisible(false);

    this.survivorText = scene.add
      .text(24, 18, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.primary,
      })
      .setDepth(DEPTH.hud)
      .setVisible(false);

    this.timerText = scene.add
      .text(CANVAS_W / 2, 18, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.clone,
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.hud)
      .setVisible(false);

    this.empireText = scene.add
      .text(CANVAS_W - 24, 18, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.danger,
      })
      .setOrigin(1, 0)
      .setDepth(DEPTH.hud)
      .setVisible(false);

    this.joinOverlay = scene.add
      .rectangle(0, 0, CANVAS_W, CANVAS_H, JOIN_PANEL_BG, 0.7)
      .setOrigin(0, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.joinTitle = scene.add
      .text(CANVAS_W / 2, 150, '⚔️ EMPIRE APPROACHES', {
        fontFamily: SQUAD_FONT.family,
        fontSize: '52px',
        color: SQUAD_COLOR.clone,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.joinSubtitle = scene.add
      .text(CANVAS_W / 2, 230, 'Type !join to join the Rebel defense!', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.white,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.joinPlayersHeader = scene.add
      .text(CANVAS_W / 2, 320, 'Joined Rebels', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.primary,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.joinPlayersText = scene.add
      .text(CANVAS_W / 2, 370, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeSM,
        color: SQUAD_COLOR.white,
        align: 'center',
        lineSpacing: 8,
        wordWrap: { width: 1000, useAdvancedWrap: true },
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.joinCountdownText = scene.add
      .text(CANVAS_W / 2, CANVAS_H - 140, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeLG,
        color: SQUAD_COLOR.clone,
        align: 'center',
      })
      .setOrigin(0.5, 0)
      .setDepth(DEPTH.join)
      .setVisible(false);

    this.resultOverlay = scene.add
      .rectangle(0, 0, CANVAS_W, CANVAS_H, 0x000000, 0.82)
      .setOrigin(0, 0)
      .setDepth(DEPTH.result)
      .setVisible(false);

    this.resultCard = scene.add
      .rectangle(CANVAS_W / 2, CANVAS_H / 2, 1040, 420, RESULT_CARD_BG, 0.96)
      .setStrokeStyle(2, 0x223344, 0.9)
      .setDepth(DEPTH.result)
      .setVisible(false);

    this.resultTitle = scene.add
      .text(CANVAS_W / 2, 390, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: '48px',
        color: SQUAD_COLOR.clone,
        align: 'center',
        wordWrap: { width: 900, useAdvancedWrap: true },
      })
      .setOrigin(0.5, 0.5)
      .setDepth(DEPTH.result)
      .setVisible(false);

    this.resultList = scene.add
      .text(CANVAS_W / 2, 560, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: SQUAD_FONT.sizeMD,
        color: SQUAD_COLOR.white,
        align: 'center',
        lineSpacing: 8,
        wordWrap: { width: 820, useAdvancedWrap: true },
      })
      .setOrigin(0.5, 0.5)
      .setDepth(DEPTH.result)
      .setVisible(false);

    this.flashText = scene.add
      .text(CANVAS_W / 2, CANVAS_H / 2, '', {
        fontFamily: SQUAD_FONT.family,
        fontSize: '46px',
        color: SQUAD_COLOR.danger,
        align: 'center',
        stroke: '#000000',
        strokeThickness: 4,
      })
      .setOrigin(0.5, 0.5)
      .setDepth(DEPTH.flash)
      .setAlpha(0)
      .setVisible(false);
  }

  onStart(payload: SquadCloneGridStartPayload): void {
    this.clearResultTimer();
    this.hideResultCard();
    this.hideGameBoard();
    this.clearEmpireVisuals();
    this.clearPlayerVisuals();

    this.players = [...payload.players];
    this.empire = [];
    this.elapsedSeconds = 0;
    this.gamePhase = 'join';
    this.joinCountdownSeconds = payload.joinWindowSeconds;
    this.joinScrollOffset = 0;

    this.updateJoinPlayerList();
    this.updateJoinCountdownText();
    this.showJoinPanel();
    this.stopJoinCountdown();
    this.stopGameTimer();

    this.joinCountdownEvent = this.scene.time.addEvent({
      delay: 1000,
      loop: true,
      callback: () => {
        if (this.gamePhase !== 'join') return;
        if (this.joinCountdownSeconds > 0) {
          this.joinCountdownSeconds -= 1;
        }
        if (this.players.length > this.getJoinVisibleCount()) {
          this.joinScrollOffset = (this.joinScrollOffset + this.getJoinVisibleCount()) % this.players.length;
        }
        this.updateJoinCountdownText();
        this.updateJoinPlayerList();
      },
    });
  }

  onUpdate(state: SquadCloneGridUpdateState): void {
    this.players = [...state.players];
    this.empire = [...state.empire];
    this.elapsedSeconds = state.elapsedSeconds;

    if (state.event === 'game_start') {
      this.enterGamePhase();
    }

    if (this.gamePhase === 'join') {
      this.updateJoinPlayerList();
      return;
    }

    if (this.gamePhase !== 'game') {
      return;
    }

    this.syncEmpire(state.empire);
    this.syncPlayers(state.players);
    this.updateHud(state.survivorCount, state.empireCount);
    this.timerText.setText(`${this.formatDuration(this.elapsedSeconds)} / ${this.formatDuration(GAME_DURATION_SECONDS)}`);

    switch (state.event) {
      case 'player_died':
      case 'player_inactivity':
        this.showFlash(`💀 ${state.eventDetail ?? 'REBEL'} ELIMINATED`, SQUAD_COLOR.danger, 2000);
        break;
      case 'empire_spawned':
        this.showFlash('⚡ Empire expands!', SQUAD_COLOR.clone, 1500);
        break;
      case 'empire_killed':
        this.showFlash('✨ Empire cell destroyed!', SQUAD_COLOR.success, 1500);
        break;
      default:
        break;
    }
  }

  onEnd(result: SquadCloneGridEndResult): void {
    this.gamePhase = 'ended';
    this.players = [...result.survivors];
    this.stopJoinCountdown();
    this.stopGameTimer();
    this.hideJoinPanel();
    this.hideGameBoard();
    this.clearEmpireVisuals();
    this.clearPlayerVisuals();

    if (result.outcome === 'win') {
      this.resultTitle.setColor(SQUAD_COLOR.clone);
      this.resultTitle.setText('🏆 REBELS HOLD THE LINE!');
      this.resultList.setText(this.buildSurvivorText(result.survivors));
    } else {
      this.resultTitle.setColor(SQUAD_COLOR.danger);
      this.resultTitle.setText('☠️ THE EMPIRE PREVAILS');
      this.resultList.setText('No survivors remained in the grid.');
    }

    this.showResultCard();
    this.clearResultTimer();
    this.hideResultCall = this.scene.time.delayedCall(8000, () => {
      this.hideResultCard();
      this.gamePhase = 'idle';
    });
  }

  destroy(): void {
    this.clearResultTimer();
    this.stopJoinCountdown();
    this.stopGameTimer();

    this.clearEmpireVisuals();
    this.clearPlayerVisuals();
    this.gridLines.forEach((line) => line.destroy());

    this.gameBackground.destroy();
    this.hudBackground.destroy();
    this.survivorText.destroy();
    this.timerText.destroy();
    this.empireText.destroy();

    this.joinOverlay.destroy();
    this.joinTitle.destroy();
    this.joinSubtitle.destroy();
    this.joinPlayersHeader.destroy();
    this.joinPlayersText.destroy();
    this.joinCountdownText.destroy();

    this.resultOverlay.destroy();
    this.resultCard.destroy();
    this.resultTitle.destroy();
    this.resultList.destroy();
    this.flashText.destroy();
  }

  private buildGridLines(): void {
    const gridW = CLONE_GRID.cols * CLONE_GRID.cellSize;
    const gridH = CLONE_GRID.rows * CLONE_GRID.cellSize;

    for (let col = 0; col <= CLONE_GRID.cols; col += 1) {
      const x = CLONE_GRID.originX + col * CLONE_GRID.cellSize;
      const line = this.scene.add
        .rectangle(x, CLONE_GRID.originY, 1, gridH, GRID_LINE, 0.6)
        .setOrigin(0, 0)
        .setDepth(DEPTH.grid)
        .setVisible(false);
      this.gridLines.push(line);
    }

    for (let row = 0; row <= CLONE_GRID.rows; row += 1) {
      const y = CLONE_GRID.originY + row * CLONE_GRID.cellSize;
      const line = this.scene.add
        .rectangle(CLONE_GRID.originX, y, gridW, 1, GRID_LINE, 0.6)
        .setOrigin(0, 0)
        .setDepth(DEPTH.grid)
        .setVisible(false);
      this.gridLines.push(line);
    }
  }

  private enterGamePhase(): void {
    this.gamePhase = 'game';
    this.hideJoinPanel();
    this.showGameBoard();
    this.syncEmpire(this.empire);
    this.syncPlayers(this.players);
    this.updateHud(this.players.length, this.empire.length);
    this.timerText.setText(`${this.formatDuration(this.elapsedSeconds)} / ${this.formatDuration(GAME_DURATION_SECONDS)}`);

    this.stopGameTimer();
    this.gameTimerEvent = this.scene.time.addEvent({
      delay: 1000,
      loop: true,
      callback: () => {
        if (this.gamePhase !== 'game') return;
        this.elapsedSeconds += 1;
        this.timerText.setText(`${this.formatDuration(this.elapsedSeconds)} / ${this.formatDuration(GAME_DURATION_SECONDS)}`);
      },
    });
  }

  private syncEmpire(nextEmpire: CloneGridCell[]): void {
    const nextKeys = new Set(nextEmpire.map((cell) => this.cellKey(cell.col, cell.row)));

    for (const [key, rect] of this.empireRects.entries()) {
      if (nextKeys.has(key)) continue;
      rect.destroy();
      this.empireRects.delete(key);
      this.empireIndicators.get(key)?.destroy();
      this.empireIndicators.delete(key);
    }

    for (const cell of nextEmpire) {
      const key = this.cellKey(cell.col, cell.row);
      const center = this.getCellCenter(cell.col, cell.row);

      if (!this.empireRects.has(key)) {
        const rect = this.scene.add
          .rectangle(center.x, center.y, CLONE_GRID.cellSize - 2, CLONE_GRID.cellSize - 2, EMPIRE_FILL, 0.9)
          .setDepth(DEPTH.cells);
        const indicator = this.scene.add
          .text(center.x, center.y, '▲', {
            fontFamily: SQUAD_FONT.family,
            fontSize: SQUAD_FONT.sizeSM,
            color: SQUAD_COLOR.danger,
          })
          .setOrigin(0.5, 0.5)
          .setDepth(DEPTH.labels);

        this.empireRects.set(key, rect);
        this.empireIndicators.set(key, indicator);
        continue;
      }

      this.empireRects.get(key)?.setPosition(center.x, center.y).setVisible(true);
      this.empireIndicators.get(key)?.setPosition(center.x, center.y).setVisible(true);
    }
  }

  private syncPlayers(nextPlayers: CloneGridPlayer[]): void {
    const nextIds = new Set(nextPlayers.map((player) => player.userId));

    for (const [userId, visual] of this.playerVisuals.entries()) {
      if (nextIds.has(userId)) continue;
      visual.rect.destroy();
      visual.label.destroy();
      visual.indicator.destroy();
      this.playerVisuals.delete(userId);
    }

    for (const player of nextPlayers) {
      const center = this.getCellCenter(player.col, player.row);
      const labelText = this.truncateName(player.userName);
      const labelY = center.y + 9;
      const indicatorY = center.y - 10;

      const existing = this.playerVisuals.get(player.userId);
      if (existing) {
        existing.rect.setPosition(center.x, center.y).setVisible(true);
        existing.label.setPosition(center.x, labelY).setText(labelText).setVisible(true);
        existing.indicator.setPosition(center.x, indicatorY).setVisible(true);
        continue;
      }

      const rect = this.scene.add
        .rectangle(center.x, center.y, CLONE_GRID.cellSize - 4, CLONE_GRID.cellSize - 4, PLAYER_FILL, 0.85)
        .setDepth(DEPTH.cells);

      const indicator = this.scene.add
        .text(center.x, indicatorY, '◆', {
          fontFamily: SQUAD_FONT.family,
          fontSize: '12px',
          color: SQUAD_COLOR.primary,
        })
        .setOrigin(0.5, 0.5)
        .setDepth(DEPTH.labels);

      const label = this.scene.add
        .text(center.x, labelY, labelText, {
          fontFamily: SQUAD_FONT.family,
          fontSize: '10px',
          color: SQUAD_COLOR.primary,
          align: 'center',
        })
        .setOrigin(0.5, 0.5)
        .setDepth(DEPTH.labels);

      this.playerVisuals.set(player.userId, { rect, label, indicator });
    }
  }

  private updateHud(survivorCount: number, empireCount: number): void {
    this.survivorText.setText(`REBELS: ${survivorCount}`);
    this.empireText.setText(`EMPIRE: ${empireCount}`);
  }

  private updateJoinCountdownText(): void {
    this.joinCountdownText.setText(`Join window closes in ${this.joinCountdownSeconds}s`);
  }

  private updateJoinPlayerList(): void {
    if (this.players.length === 0) {
      this.joinPlayersText.setText('No rebels yet...');
      return;
    }

    const visibleCount = this.getJoinVisibleCount();
    const names = this.players.map((player) => player.userName);

    let visibleNames = names;
    if (names.length > visibleCount) {
      visibleNames = [];
      for (let i = 0; i < visibleCount; i += 1) {
        visibleNames.push(names[(this.joinScrollOffset + i) % names.length] ?? '');
      }
      visibleNames.push('');
      visibleNames.push(`(${names.length} total rebels — rotating roster)`);
    }

    this.joinPlayersText.setText(visibleNames.join('\n'));
  }

  private getJoinVisibleCount(): number {
    return 14;
  }

  private showJoinPanel(): void {
    this.joinOverlay.setVisible(true);
    this.joinTitle.setVisible(true);
    this.joinSubtitle.setVisible(true);
    this.joinPlayersHeader.setVisible(true);
    this.joinPlayersText.setVisible(true);
    this.joinCountdownText.setVisible(true);
  }

  private hideJoinPanel(): void {
    this.joinOverlay.setVisible(false);
    this.joinTitle.setVisible(false);
    this.joinSubtitle.setVisible(false);
    this.joinPlayersHeader.setVisible(false);
    this.joinPlayersText.setVisible(false);
    this.joinCountdownText.setVisible(false);
  }

  private showGameBoard(): void {
    this.gameBackground.setVisible(true);
    this.hudBackground.setVisible(true);
    this.survivorText.setVisible(true);
    this.timerText.setVisible(true);
    this.empireText.setVisible(true);
    this.gridLines.forEach((line) => line.setVisible(true));
  }

  private hideGameBoard(): void {
    this.gameBackground.setVisible(false);
    this.hudBackground.setVisible(false);
    this.survivorText.setVisible(false);
    this.timerText.setVisible(false);
    this.empireText.setVisible(false);
    this.gridLines.forEach((line) => line.setVisible(false));
  }

  private showResultCard(): void {
    this.resultOverlay.setVisible(true);
    this.resultCard.setVisible(true);
    this.resultTitle.setVisible(true);
    this.resultList.setVisible(true);
  }

  private hideResultCard(): void {
    this.resultOverlay.setVisible(false);
    this.resultCard.setVisible(false);
    this.resultTitle.setVisible(false);
    this.resultList.setVisible(false);
  }

  private showFlash(text: string, color: string, durationMs: number): void {
    this.flashText.setText(text);
    this.flashText.setColor(color);
    this.flashText.setAlpha(1);
    this.flashText.setVisible(true);

    this.scene.tweens.killTweensOf(this.flashText);
    this.scene.tweens.add({
      targets: this.flashText,
      alpha: 0,
      duration: durationMs,
      ease: 'Sine.easeOut',
      onComplete: () => {
        this.flashText.setVisible(false);
      },
    });
  }

  private buildSurvivorText(survivors: CloneGridPlayer[]): string {
    if (survivors.length === 0) {
      return 'No survivors recorded.';
    }

    return ['Survivors:', ...survivors.map((player) => `• ${player.userName}`)].join('\n');
  }

  private clearEmpireVisuals(): void {
    for (const rect of this.empireRects.values()) {
      rect.destroy();
    }
    for (const indicator of this.empireIndicators.values()) {
      indicator.destroy();
    }
    this.empireRects.clear();
    this.empireIndicators.clear();
  }

  private clearPlayerVisuals(): void {
    for (const visual of this.playerVisuals.values()) {
      visual.rect.destroy();
      visual.label.destroy();
      visual.indicator.destroy();
    }
    this.playerVisuals.clear();
  }

  private clearResultTimer(): void {
    this.hideResultCall?.remove(false);
    this.hideResultCall = undefined;
  }

  private stopJoinCountdown(): void {
    this.joinCountdownEvent?.remove(false);
    this.joinCountdownEvent = undefined;
  }

  private stopGameTimer(): void {
    this.gameTimerEvent?.remove(false);
    this.gameTimerEvent = undefined;
  }

  private cellKey(col: number, row: number): string {
    return `${col},${row}`;
  }

  private getCellCenter(col: number, row: number): { x: number; y: number } {
    return {
      x: CLONE_GRID.originX + (col - 1) * CLONE_GRID.cellSize + CLONE_GRID.cellSize / 2,
      y: CLONE_GRID.originY + (row - 1) * CLONE_GRID.cellSize + CLONE_GRID.cellSize / 2,
    };
  }

  private truncateName(userName: string): string {
    return userName.length > 9 ? `${userName.slice(0, 9)}…` : userName;
  }

  private formatDuration(totalSeconds: number): string {
    const safe = Math.max(0, totalSeconds);
    const minutes = Math.floor(safe / 60);
    const seconds = safe % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }
}

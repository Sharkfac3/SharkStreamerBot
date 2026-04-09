import { config, type LogLevel } from "./config.js";

const LEVELS: Record<LogLevel, number> = {
  minimal: 0,
  standard: 1,
  verbose: 2,
};

function shouldLog(required: LogLevel): boolean {
  return LEVELS[config.logLevel] >= LEVELS[required];
}

function ts(): string {
  return new Date().toISOString();
}

export const logger = {
  /** Always logged — broker errors are always visible */
  error(msg: string, ...args: unknown[]): void {
    console.error(`[${ts()}] ERROR ${msg}`, ...args);
  },

  /** Logged at standard and verbose — connections, disconnections, subscriptions */
  info(msg: string, ...args: unknown[]): void {
    if (shouldLog("standard")) {
      console.log(`[${ts()}] INFO  ${msg}`, ...args);
    }
  },

  /** Logged at verbose only — every routed message */
  debug(msg: string, ...args: unknown[]): void {
    if (shouldLog("verbose")) {
      console.log(`[${ts()}] DEBUG ${msg}`, ...args);
    }
  },
};

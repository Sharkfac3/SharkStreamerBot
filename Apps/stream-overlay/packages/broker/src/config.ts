import { BROKER_PORT } from "@stream-overlay/shared";

export type LogLevel = "minimal" | "standard" | "verbose";

function parseLogLevel(raw: string | undefined): LogLevel {
  if (raw === "verbose") return "verbose";
  if (raw === "minimal") return "minimal";
  return "standard";
}

export const config = {
  port: process.env.BROKER_PORT ? Number(process.env.BROKER_PORT) : BROKER_PORT,
  logLevel: parseLogLevel(process.env.LOG_LEVEL),
} as const;

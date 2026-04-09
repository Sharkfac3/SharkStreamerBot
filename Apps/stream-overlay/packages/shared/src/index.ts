// @stream-overlay/shared — barrel export for all shared types and protocol definitions.
//
// This package is the single source of truth for:
//   - WebSocket message protocol (topic names, payload shapes)
//   - Shared enums and constants
//   - Any type that crosses the broker ↔ overlay ↔ Streamer.bot boundary
//
// Populated in Prompt 01 (message protocol design).
// Import from this package everywhere — never duplicate type definitions across packages.

export * from "./protocol.js";
export * from "./topics.js";

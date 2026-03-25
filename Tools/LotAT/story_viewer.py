#!/usr/bin/env python3
"""Serve a local LotAT story viewer and handoff tool."""

from __future__ import annotations

import argparse
import json
from pathlib import Path
import shutil
import sys
from typing import Any, Sequence

APP_TITLE = "LotAT Story Viewer"
DEFAULT_HOST = "127.0.0.1"
DEFAULT_PORT = 8876
REPO_ROOT = Path(__file__).resolve().parents[2]
DRAFTS_DIR = REPO_ROOT / "Creative" / "WorldBuilding" / "Storylines" / "drafts"
READY_DIR = REPO_ROOT / "Creative" / "WorldBuilding" / "Storylines" / "ready"
HTML_PAGE = """<!doctype html>
<html lang=\"en\">
<head>
  <meta charset=\"utf-8\" />
  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\" />
  <title>LotAT Story Viewer</title>
  <style>
    :root {
      color-scheme: dark;
      --bg: #0b1020;
      --panel: #121a30;
      --panel-2: #182341;
      --panel-3: #0c1430;
      --text: #ecf2ff;
      --muted: #94a3c7;
      --accent: #7dd3fc;
      --accent-2: #c084fc;
      --good: #4ade80;
      --warn: #facc15;
      --bad: #fb7185;
      --border: #293557;
      --focus: #c084fc;
      --shadow: rgba(0, 0, 0, 0.28);
    }

    * { box-sizing: border-box; }

    body {
      margin: 0;
      min-height: 100vh;
      font-family: Arial, Helvetica, sans-serif;
      background: linear-gradient(180deg, #09101f, #11182e 35%, #0b1020);
      color: var(--text);
    }

    .app {
      display: grid;
      grid-template-columns: 280px minmax(0, 1fr);
      min-height: 100vh;
    }

    .sidebar {
      border-right: 1px solid var(--border);
      background: rgba(18, 26, 48, 0.9);
      padding: 18px 14px;
      position: sticky;
      top: 0;
      height: 100vh;
      overflow-y: auto;
    }

    .sidebar h1 {
      margin: 0;
      font-size: 1.15rem;
    }

    .sidebar p {
      margin: 6px 0 0;
      color: var(--muted);
      font-size: 0.92rem;
      line-height: 1.45;
    }

    .story-list {
      margin-top: 18px;
      display: flex;
      flex-direction: column;
      gap: 8px;
    }

    .story-item {
      width: 100%;
      text-align: left;
      border: 1px solid var(--border);
      background: var(--panel-2);
      color: var(--text);
      border-radius: 12px;
      padding: 12px;
      cursor: pointer;
      transition: border-color 120ms ease, transform 120ms ease, background 120ms ease, opacity 120ms ease;
    }

    .story-item:hover {
      border-color: rgba(125, 211, 252, 0.5);
      transform: translateY(-1px);
    }

    .story-item.active {
      background: rgba(125, 211, 252, 0.14);
      border-color: rgba(125, 211, 252, 0.42);
    }

    .story-item.handed-off {
      opacity: 0.48;
    }

    .story-name {
      font-weight: 700;
      word-break: break-word;
    }

    .story-subtitle {
      margin-top: 4px;
      color: var(--muted);
      font-size: 0.84rem;
    }

    .main {
      padding: 24px 24px 48px;
      overflow-x: hidden;
    }

    .empty, .error-box {
      background: rgba(18, 26, 48, 0.88);
      border: 1px solid var(--border);
      border-radius: 16px;
      padding: 28px;
      box-shadow: 0 18px 60px var(--shadow);
    }

    .error-box {
      border-color: rgba(251, 113, 133, 0.42);
    }

    .story-header {
      background: rgba(18, 26, 48, 0.88);
      border: 1px solid var(--border);
      border-radius: 18px;
      padding: 20px;
      margin-bottom: 18px;
      box-shadow: 0 18px 60px var(--shadow);
    }

    .story-header h2 {
      margin: 0 0 10px;
      font-size: 1.8rem;
    }

    .summary {
      margin: 0 0 14px;
      color: var(--text);
      line-height: 1.55;
    }

    .meta-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
      gap: 12px;
    }

    .meta-card {
      background: var(--panel-2);
      border: 1px solid var(--border);
      border-radius: 14px;
      padding: 12px 14px;
    }

    .meta-label {
      color: var(--muted);
      font-size: 0.78rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      margin-bottom: 6px;
    }

    .meta-value {
      line-height: 1.5;
      word-break: break-word;
    }

    .badge-row {
      display: flex;
      flex-wrap: wrap;
      gap: 8px;
      margin-bottom: 12px;
    }

    .badge {
      display: inline-flex;
      align-items: center;
      gap: 6px;
      border-radius: 999px;
      padding: 5px 10px;
      font-size: 0.82rem;
      line-height: 1;
      border: 1px solid var(--border);
      background: var(--panel-3);
      color: var(--text);
    }

    .badge.muted {
      color: var(--muted);
      font-family: Consolas, Monaco, monospace;
    }

    .badge.section {
      background: rgba(125, 211, 252, 0.14);
      border-color: rgba(125, 211, 252, 0.34);
      color: var(--accent);
    }

    .badge.command {
      font-family: Consolas, Monaco, monospace;
      color: var(--accent);
    }

    .badge.chaos { border-color: rgba(192, 132, 252, 0.38); color: var(--accent-2); }
    .badge.dice { border-color: rgba(250, 204, 21, 0.4); color: var(--warn); }
    .badge.commander { border-color: rgba(74, 222, 128, 0.35); color: var(--good); }
    .badge.end-success { border-color: rgba(74, 222, 128, 0.4); color: var(--good); }
    .badge.end-failure { border-color: rgba(251, 113, 133, 0.42); color: var(--bad); }
    .badge.end-partial { border-color: rgba(250, 204, 21, 0.42); color: var(--warn); }

    .tree-root {
      display: flex;
      flex-direction: column;
      gap: 18px;
    }

    .node-wrap {
      position: relative;
      padding-left: 0;
    }

    .choice-children {
      margin-left: 28px;
      padding-left: 18px;
      border-left: 2px solid rgba(125, 211, 252, 0.18);
      display: flex;
      flex-direction: column;
      gap: 18px;
      margin-top: 12px;
    }

    .node-card {
      background: rgba(18, 26, 48, 0.9);
      border: 1px solid var(--border);
      border-radius: 16px;
      padding: 16px;
      box-shadow: 0 14px 40px var(--shadow);
    }

    .node-card.stage {
      background: rgba(18, 26, 48, 0.94);
    }

    .node-card.ending-success {
      background: linear-gradient(180deg, rgba(12, 58, 35, 0.92), rgba(18, 26, 48, 0.94));
      border-color: rgba(74, 222, 128, 0.4);
    }

    .node-card.ending-failure {
      background: linear-gradient(180deg, rgba(71, 14, 29, 0.92), rgba(18, 26, 48, 0.94));
      border-color: rgba(251, 113, 133, 0.44);
    }

    .node-card.ending-partial {
      background: linear-gradient(180deg, rgba(77, 61, 9, 0.92), rgba(18, 26, 48, 0.94));
      border-color: rgba(250, 204, 21, 0.44);
    }

    .node-title {
      margin: 0 0 10px;
      font-size: 1.2rem;
    }

    .read-aloud {
      margin: 0;
      line-height: 1.6;
      white-space: pre-wrap;
    }

    .visited-note {
      margin-top: 10px;
      color: var(--muted);
      font-size: 0.88rem;
    }

    .choices {
      margin-top: 14px;
      display: grid;
      gap: 12px;
    }

    .choice {
      background: var(--panel-3);
      border: 1px solid var(--border);
      border-radius: 14px;
      padding: 12px;
    }

    .choice-head {
      display: flex;
      align-items: center;
      gap: 10px;
      flex-wrap: wrap;
      font-weight: 700;
    }

    .choice-arrow {
      color: var(--accent);
      font-size: 1rem;
      font-weight: 700;
    }

    .choice-destination {
      color: var(--muted);
      font-family: Consolas, Monaco, monospace;
      font-size: 0.88rem;
    }

    .handoff-panel {
      margin-top: 20px;
      background: rgba(18, 26, 48, 0.88);
      border: 1px solid var(--border);
      border-radius: 18px;
      padding: 18px;
      box-shadow: 0 18px 60px var(--shadow);
    }

    .handoff-button {
      border: 1px solid rgba(125, 211, 252, 0.42);
      border-radius: 12px;
      padding: 14px 18px;
      font: inherit;
      font-weight: 800;
      letter-spacing: 0.04em;
      cursor: pointer;
      color: var(--text);
      background: rgba(125, 211, 252, 0.18);
    }

    .handoff-button:hover:enabled {
      background: rgba(125, 211, 252, 0.24);
    }

    .handoff-button:disabled {
      cursor: default;
      opacity: 0.76;
    }

    .inline-status {
      min-height: 24px;
      margin-top: 12px;
      color: var(--accent);
    }

    .inline-status.error {
      color: var(--bad);
    }

    .loading {
      color: var(--muted);
    }

    code.inline-code {
      font-family: Consolas, Monaco, monospace;
      color: var(--accent);
    }

    @media (max-width: 900px) {
      .app {
        grid-template-columns: 1fr;
      }

      .sidebar {
        position: static;
        height: auto;
        border-right: 0;
        border-bottom: 1px solid var(--border);
      }

      .main {
        padding: 18px 14px 40px;
      }
    }
  </style>
</head>
<body>
  <div class=\"app\">
    <aside class=\"sidebar\">
      <h1>Draft Stories</h1>
      <p>Read the branching tree, then hand the selected draft off to the ready queue.</p>
      <div class=\"story-list\" id=\"storyList\">
        <div class=\"loading\">Loading draft stories…</div>
      </div>
    </aside>
    <main class=\"main\" id=\"mainPanel\">
      <div class=\"empty\">
        <h2 style=\"margin-top: 0;\">Loading viewer…</h2>
        <p>Select a draft story from the sidebar once the list loads.</p>
      </div>
    </main>
  </div>

  <script>
    const state = {
      stories: [],
      selectedId: null,
      handedOff: new Set(),
      storyData: null,
      loading: false,
    };

    function escapeHtml(value) {
      return String(value ?? "")
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/\"/g, "&quot;")
        .replace(/'/g, "&#39;");
    }

    async function fetchJson(url, options = {}) {
      const response = await fetch(url, {
        headers: { "Content-Type": "application/json" },
        ...options,
      });

      let payload = null;
      try {
        payload = await response.json();
      } catch (error) {
        payload = null;
      }

      if (!response.ok) {
        const message = payload && payload.detail ? payload.detail : `Request failed (${response.status})`;
        throw new Error(message);
      }

      return payload;
    }

    function renderSidebar() {
      const list = document.getElementById("storyList");
      if (!state.stories.length) {
        list.innerHTML = `
          <div class=\"empty\" style=\"padding: 18px;\">
            <strong>No draft stories found.</strong>
            <p style=\"margin-bottom: 0; color: #94a3c7;\">Put <code class=\"inline-code\">.json</code> files in <code class=\"inline-code\">Creative/WorldBuilding/Storylines/drafts/</code>.</p>
          </div>
        `;
        return;
      }

      list.innerHTML = state.stories.map((storyId) => {
        const activeClass = storyId === state.selectedId ? " active" : "";
        const handedOffClass = state.handedOff.has(storyId) ? " handed-off" : "";
        const subtitle = state.handedOff.has(storyId) ? "Handed off" : "Draft";
        return `
          <button class=\"story-item${activeClass}${handedOffClass}\" data-story-id=\"${escapeHtml(storyId)}\">
            <div class=\"story-name\">${escapeHtml(storyId)}</div>
            <div class=\"story-subtitle\">${escapeHtml(subtitle)}</div>
          </button>
        `;
      }).join("");

      list.querySelectorAll("[data-story-id]").forEach((button) => {
        button.addEventListener("click", () => {
          loadStory(button.getAttribute("data-story-id")).catch((error) => {
            renderError(error.message || String(error));
          });
        });
      });
    }

    function normalizeArray(value) {
      if (Array.isArray(value)) {
        return value.filter(Boolean).map((item) => String(item));
      }
      return [];
    }

    function castDisplay(cast) {
      if (Array.isArray(cast)) {
        return cast.join(", ") || "—";
      }
      if (cast && typeof cast === "object") {
        const commanders = normalizeArray(cast.commanders_used);
        const squad = normalizeArray(cast.squad_members_used);
        const parts = [];
        if (commanders.length) {
          parts.push(`Commanders: ${commanders.join(", ")}`);
        }
        if (squad.length) {
          parts.push(`Squad: ${squad.join(", ")}`);
        }
        return parts.join(" • ") || "—";
      }
      return "—";
    }

    function commandsDisplay(story) {
      return normalizeArray(story.commands_used).join(", ") || "—";
    }

    function badge(text, className = "") {
      return `<span class=\"badge ${className}\">${escapeHtml(text)}</span>`;
    }

    function endClass(endState) {
      const normalized = String(endState || "").trim().toLowerCase();
      if (normalized === "success") {
        return { card: "ending-success", badge: "end-success" };
      }
      if (normalized === "failure") {
        return { card: "ending-failure", badge: "end-failure" };
      }
      return { card: "ending-partial", badge: "end-partial" };
    }

    function renderNode(nodeId, nodeMap, branchTrail = []) {
      const node = nodeMap.get(nodeId);
      if (!node) {
        return `
          <div class=\"node-wrap\">
            <div class=\"node-card ending-failure\">
              <div class=\"badge-row\">
                ${badge(nodeId, "muted")}
                ${badge("MISSING NODE", "end-failure")}
              </div>
              <h3 class=\"node-title\">Broken branch reference</h3>
              <p class=\"read-aloud\">This choice points to <code class=\"inline-code\">${escapeHtml(nodeId)}</code>, but that node was not found in the story file.</p>
            </div>
          </div>
        `;
      }

      const duplicate = branchTrail.includes(nodeId);
      const chaos = node.chaos && typeof node.chaos === "object" ? node.chaos : {};
      const diceHook = node.dice_hook && typeof node.dice_hook === "object" ? node.dice_hook : {};
      const commanderMoment = node.commander_moment && typeof node.commander_moment === "object" ? node.commander_moment : {};
      const badges = [
        badge(node.node_id || nodeId, "muted"),
        badge(node.ship_section || "Unknown section", "section"),
      ];

      if (node.node_type === "ending") {
        const classes = endClass(node.end_state);
        badges.push(badge(String(node.end_state || "partial").toUpperCase(), classes.badge));
        if (Number(chaos.on_enter || 0) !== 0) badges.push(badge(`Chaos enter ${Number(chaos.on_enter || 0) > 0 ? "+" : ""}${Number(chaos.on_enter || 0)}`, "chaos"));
        if (Number(chaos.on_success || 0) !== 0) badges.push(badge(`Chaos success ${Number(chaos.on_success || 0) > 0 ? "+" : ""}${Number(chaos.on_success || 0)}`, "chaos"));
        if (Number(chaos.on_failure || 0) !== 0) badges.push(badge(`Chaos fail ${Number(chaos.on_failure || 0) > 0 ? "+" : ""}${Number(chaos.on_failure || 0)}`, "chaos"));
        if (diceHook.enabled === true) badges.push(badge("Dice Hook", "dice"));
        if (commanderMoment.enabled === true) badges.push(badge(`Commander: ${commanderMoment.commander || "Unknown"}`, "commander"));
        return `
          <div class=\"node-wrap\">
            <div class=\"node-card ${classes.card}\">
              <div class=\"badge-row\">${badges.join("")}</div>
              <h3 class=\"node-title\">${escapeHtml(node.title || nodeId)}</h3>
              <p class=\"read-aloud\">${escapeHtml(node.read_aloud || "")}</p>
              ${duplicate ? `<div class=\"visited-note\">(visited earlier — ${escapeHtml(nodeId)})</div>` : ""}
            </div>
          </div>
        `;
      }

      if (Number(chaos.on_enter || 0) !== 0) badges.push(badge(`Chaos enter ${Number(chaos.on_enter || 0) > 0 ? "+" : ""}${Number(chaos.on_enter || 0)}`, "chaos"));
      if (Number(chaos.on_success || 0) !== 0) badges.push(badge(`Chaos success ${Number(chaos.on_success || 0) > 0 ? "+" : ""}${Number(chaos.on_success || 0)}`, "chaos"));
      if (Number(chaos.on_failure || 0) !== 0) badges.push(badge(`Chaos fail ${Number(chaos.on_failure || 0) > 0 ? "+" : ""}${Number(chaos.on_failure || 0)}`, "chaos"));
      if (diceHook.enabled === true) badges.push(badge("Dice Hook", "dice"));
      if (commanderMoment.enabled === true) badges.push(badge(`Commander: ${commanderMoment.commander || "Unknown"}`, "commander"));

      const nextTrail = duplicate ? branchTrail : branchTrail.concat(nodeId);
      const choices = Array.isArray(node.choices) ? node.choices : [];
      const choiceHtml = choices.map((choice) => {
        const choiceLabel = choice && choice.label ? choice.label : "Unnamed choice";
        const command = choice && choice.command ? String(choice.command) : "";
        const nextNodeId = choice && choice.next_node_id ? String(choice.next_node_id) : "";
        const childHtml = nextNodeId
          ? renderNode(nextNodeId, nodeMap, nextTrail)
          : `
              <div class=\"node-wrap\">
                <div class=\"node-card ending-failure\">
                  <div class=\"badge-row\">${badge("MISSING next_node_id", "end-failure")}</div>
                  <h3 class=\"node-title\">Broken choice target</h3>
                  <p class=\"read-aloud\">This choice does not define a valid downstream node.</p>
                </div>
              </div>
            `;
        return `
          <div class=\"choice\">
            <div class=\"choice-head\">
              <span>${escapeHtml(choiceLabel)}</span>
              ${command ? badge(command, "command") : ""}
              <span class=\"choice-arrow\">→</span>
              <span class=\"choice-destination\">${escapeHtml(nextNodeId || "missing")}</span>
            </div>
            <div class=\"choice-children\">${childHtml}</div>
          </div>
        `;
      }).join("");

      return `
        <div class=\"node-wrap\">
          <div class=\"node-card stage\">
            <div class=\"badge-row\">${badges.join("")}</div>
            <h3 class=\"node-title\">${escapeHtml(node.title || nodeId)}</h3>
            <p class=\"read-aloud\">${escapeHtml(node.read_aloud || "")}</p>
            ${duplicate ? `<div class=\"visited-note\">(visited earlier — ${escapeHtml(nodeId)})</div>` : ""}
            ${duplicate ? "" : `<div class=\"choices\">${choiceHtml}</div>`}
          </div>
        </div>
      `;
    }

    function renderStory(storyId, story) {
      const nodes = Array.isArray(story.nodes) ? story.nodes : [];
      const nodeMap = new Map(nodes.map((node) => [String(node.node_id || ""), node]));
      const rootId = String(story.starting_node_id || "").trim();
      const treeHtml = rootId
        ? renderNode(rootId, nodeMap, [])
        : `
            <div class=\"node-card ending-failure\">
              <div class=\"badge-row\">${badge("Missing starting_node_id", "end-failure")}</div>
              <h3 class=\"node-title\">Story root missing</h3>
              <p class=\"read-aloud\">This story file does not define a valid <code class=\"inline-code\">starting_node_id</code>.</p>
            </div>
          `;

      const handedOff = state.handedOff.has(storyId);
      document.getElementById("mainPanel").innerHTML = `
        <section class=\"story-header\">
          <div class=\"badge-row\">
            ${badge(story.story_id || storyId, "muted")}
            ${story.starting_ship_section ? badge(story.starting_ship_section, "section") : ""}
            ${story.version ? badge(`v${story.version}`) : ""}
            ${handedOff ? badge("Ready", "commander") : badge("Draft")}
          </div>
          <h2>${escapeHtml(story.title || storyId)}</h2>
          <p class=\"summary\">${escapeHtml(story.summary || "No summary provided.")}</p>
          <div class=\"meta-grid\">
            <div class=\"meta-card\"><div class=\"meta-label\">Cast</div><div class=\"meta-value\">${escapeHtml(castDisplay(story.cast))}</div></div>
            <div class=\"meta-card\"><div class=\"meta-label\">Commands</div><div class=\"meta-value\">${escapeHtml(commandsDisplay(story))}</div></div>
            <div class=\"meta-card\"><div class=\"meta-label\">Ship Sections</div><div class=\"meta-value\">${escapeHtml(normalizeArray(story.ship_sections_used).join(", ") || "—")}</div></div>
            <div class=\"meta-card\"><div class=\"meta-label\">Root Node</div><div class=\"meta-value\"><code class=\"inline-code\">${escapeHtml(rootId || "—")}</code></div></div>
          </div>
        </section>

        <section class=\"tree-root\">${treeHtml}</section>

        <section class=\"handoff-panel\">
          <button id=\"handoffButton\" class=\"handoff-button\" ${handedOff ? "disabled" : ""}>${handedOff ? "Handed Off ✓" : "HAND OFF STORY"}</button>
          <div id=\"handoffStatus\" class=\"inline-status\"></div>
        </section>
      `;

      const handoffButton = document.getElementById("handoffButton");
      if (handoffButton && !handedOff) {
        handoffButton.addEventListener("click", async () => {
          await handoffStory(storyId);
        });
      }
    }

    function renderError(message) {
      document.getElementById("mainPanel").innerHTML = `
        <div class=\"error-box\">
          <h2 style=\"margin-top: 0;\">Viewer error</h2>
          <p>${escapeHtml(message)}</p>
        </div>
      `;
    }

    async function loadStories() {
      const payload = await fetchJson("/api/stories");
      state.stories = Array.isArray(payload.stories) ? payload.stories : [];
      if (!state.selectedId && state.stories.length) {
        state.selectedId = state.stories[0];
      }
      renderSidebar();
      if (state.selectedId) {
        await loadStory(state.selectedId);
      } else {
        document.getElementById("mainPanel").innerHTML = `
          <div class=\"empty\">
            <h2 style=\"margin-top: 0;\">No draft stories found</h2>
            <p>Put story <code class=\"inline-code\">.json</code> files in <code class=\"inline-code\">Creative/WorldBuilding/Storylines/drafts/</code>.</p>
          </div>
        `;
      }
    }

    async function loadStory(storyId) {
      state.selectedId = storyId;
      renderSidebar();
      document.getElementById("mainPanel").innerHTML = `
        <div class=\"empty\">
          <h2 style=\"margin-top: 0;\">Loading story…</h2>
          <p>${escapeHtml(storyId)}</p>
        </div>
      `;
      const story = await fetchJson(`/api/stories/${encodeURIComponent(storyId)}`);
      state.storyData = story;
      renderSidebar();
      renderStory(storyId, story);
    }

    async function handoffStory(storyId) {
      const button = document.getElementById("handoffButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      button.textContent = "Handing Off…";
      status.textContent = "Moving story to ready…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/${encodeURIComponent(storyId)}/handoff`, { method: "POST" });
        state.handedOff.add(storyId);
        button.textContent = "Handed Off ✓";
        button.disabled = true;
        status.textContent = "Story moved to ready.";
        renderSidebar();
      } catch (error) {
        button.disabled = false;
        button.textContent = "HAND OFF STORY";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    loadStories().catch((error) => {
      renderError(error.message || String(error));
      const list = document.getElementById("storyList");
      list.innerHTML = `<div class=\"error-box\" style=\"padding: 14px;\"><strong>Could not load draft list.</strong><p style=\"margin-bottom: 0;\">${escapeHtml(error.message || String(error))}</p></div>`;
    });
  </script>
</body>
</html>
"""


def build_parser() -> argparse.ArgumentParser:
    """Build the CLI parser."""
    parser = argparse.ArgumentParser(description="Serve the LotAT story viewer web UI")
    parser.add_argument("--host", default=DEFAULT_HOST, help=f"Bind host (default: {DEFAULT_HOST})")
    parser.add_argument("--port", type=int, default=DEFAULT_PORT, help=f"Bind port (default: {DEFAULT_PORT})")
    parser.add_argument("--reload", action="store_true", help="Enable uvicorn auto-reload for local development.")
    return parser


def parse_args() -> argparse.Namespace:
    """Parse command-line arguments."""
    return build_parser().parse_args()


def parse_args_from(argv: Sequence[str]) -> argparse.Namespace:
    """Parse arguments from an explicit argv list for tests."""
    return build_parser().parse_args(list(argv))


def ensure_story_dirs() -> None:
    """Ensure the draft and ready directories exist."""
    DRAFTS_DIR.mkdir(parents=True, exist_ok=True)
    READY_DIR.mkdir(parents=True, exist_ok=True)


def sanitize_story_id(story_id: str) -> str:
    """Restrict story IDs to safe draft JSON filenames."""
    cleaned = Path(str(story_id or "")).name
    if cleaned != story_id:
        raise ValueError("Invalid story id.")
    if not cleaned.endswith(".json"):
        raise ValueError("Story id must be a .json filename.")
    if cleaned in {".", ".."}:
        raise ValueError("Invalid story id.")
    return cleaned


def resolve_story_path(story_id: str, base_dir: Path = DRAFTS_DIR) -> Path:
    """Resolve a story file under the given base directory without traversal."""
    safe_name = sanitize_story_id(story_id)
    candidate = (base_dir / safe_name).resolve()
    base_resolved = base_dir.resolve()
    if candidate.parent != base_resolved:
        raise ValueError("Invalid story path.")
    return candidate


def list_draft_stories() -> list[str]:
    """Return the sorted list of draft story filenames."""
    ensure_story_dirs()
    return sorted(path.name for path in DRAFTS_DIR.glob("*.json") if path.is_file())


def load_story_json(story_id: str) -> dict[str, Any]:
    """Load a draft story JSON file."""
    path = resolve_story_path(story_id, DRAFTS_DIR)
    if not path.exists() or not path.is_file():
        raise FileNotFoundError(f"Draft story not found: {story_id}")

    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in {story_id}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Story file must contain a JSON object: {story_id}")
    return payload


def handoff_story(story_id: str) -> dict[str, Any]:
    """Move a story from drafts to ready, returning an idempotent result."""
    ensure_story_dirs()
    draft_path = resolve_story_path(story_id, DRAFTS_DIR)
    ready_path = resolve_story_path(story_id, READY_DIR)

    if ready_path.exists() and ready_path.is_file() and not draft_path.exists():
        return {
            "ok": True,
            "story_id": story_id,
            "status": "already_ready",
            "draft_path": str(draft_path),
            "ready_path": str(ready_path),
        }

    if not draft_path.exists() or not draft_path.is_file():
        raise FileNotFoundError(f"Draft story not found: {story_id}")

    if ready_path.exists():
        return {
            "ok": True,
            "story_id": story_id,
            "status": "already_ready",
            "draft_path": str(draft_path),
            "ready_path": str(ready_path),
        }

    shutil.move(str(draft_path), str(ready_path))
    return {
        "ok": True,
        "story_id": story_id,
        "status": "moved_to_ready",
        "draft_path": str(draft_path),
        "ready_path": str(ready_path),
    }


def create_app() -> Any:
    """Create the FastAPI app lazily so --help works before dependencies are installed."""
    try:
        from fastapi import FastAPI, HTTPException
        from fastapi.responses import HTMLResponse
    except ImportError as error:
        raise RuntimeError("FastAPI is not installed. Run 'pip install fastapi uvicorn'.") from error

    ensure_story_dirs()
    app = FastAPI(title=APP_TITLE)

    @app.get("/", response_class=HTMLResponse)
    def index() -> str:
        return HTML_PAGE

    @app.get("/api/stories")
    def get_stories() -> dict[str, Any]:
        return {
            "stories": list_draft_stories(),
            "drafts_dir": str(DRAFTS_DIR),
            "ready_dir": str(READY_DIR),
        }

    @app.get("/api/stories/{story_id}")
    def get_story(story_id: str) -> dict[str, Any]:
        try:
            return load_story_json(story_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/{story_id}/handoff")
    def post_handoff(story_id: str) -> dict[str, Any]:
        try:
            return handoff_story(story_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    return app


app = create_app()


def main(argv: Sequence[str] | None = None) -> int:
    """Run the local FastAPI server."""
    args = parse_args() if argv is None else parse_args_from(argv)

    if args.port <= 0 or args.port > 65535:
        print("[lotat-viewer] Configuration error: --port must be between 1 and 65535", file=sys.stderr)
        return 1

    try:
        import uvicorn
    except ImportError:
        print("[lotat-viewer] Missing dependency: uvicorn is not installed. Run 'pip install fastapi uvicorn'.", file=sys.stderr)
        return 1

    print(f"[lotat-viewer] Repo root: {REPO_ROOT}")
    print(f"[lotat-viewer] Draft stories: {DRAFTS_DIR}")
    print(f"[lotat-viewer] Ready stories: {READY_DIR}")
    print(f"[lotat-viewer] Serving http://{args.host}:{args.port}")

    uvicorn.run(app, host=args.host, port=args.port, reload=args.reload)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

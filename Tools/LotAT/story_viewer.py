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
LOADED_DIR = REPO_ROOT / "Creative" / "WorldBuilding" / "Storylines" / "loaded"
FINISHED_DIR = REPO_ROOT / "Creative" / "WorldBuilding" / "Storylines" / "finished"
LOADED_STORY_FILENAME = "current-story.json"
LOADED_META_FILENAME = "loaded-meta.json"
STORY_STAGE_DIRS = {
    "draft": DRAFTS_DIR,
    "ready": READY_DIR,
    "finished": FINISHED_DIR,
}
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

    .story-group {
      display: flex;
      flex-direction: column;
      gap: 8px;
      margin-top: 14px;
    }

    .story-group-title {
      color: var(--muted);
      font-size: 0.76rem;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      font-weight: 700;
      padding: 2px 2px 0;
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

    .story-item.ready {
      border-color: rgba(74, 222, 128, 0.24);
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
    .badge.story-stage-draft { border-color: rgba(125, 211, 252, 0.34); color: var(--accent); }
    .badge.story-stage-ready { border-color: rgba(74, 222, 128, 0.38); color: var(--good); }
    .badge.story-stage-finished { border-color: rgba(250, 204, 21, 0.42); color: var(--warn); }

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

    .node-card.toggleable {
      cursor: pointer;
      transition: border-color 120ms ease, transform 120ms ease, background 120ms ease;
    }

    .node-card.toggleable:hover {
      border-color: rgba(125, 211, 252, 0.45);
      transform: translateY(-1px);
    }

    .node-card.toggleable:focus-visible {
      outline: 2px solid var(--focus);
      outline-offset: 2px;
    }

    .node-card.collapsed {
      border-color: rgba(192, 132, 252, 0.38);
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

    .node-title-row {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 12px;
    }

    .node-toggle-hint {
      color: var(--accent);
      font-size: 0.82rem;
      font-weight: 700;
      letter-spacing: 0.04em;
      text-transform: uppercase;
      white-space: nowrap;
    }

    .children-collapsed {
      display: none !important;
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

    .handoff-button.ready-action {
      border-color: rgba(250, 204, 21, 0.42);
      background: rgba(250, 204, 21, 0.16);
    }

    .handoff-button:hover:enabled {
      background: rgba(125, 211, 252, 0.24);
    }

    .handoff-button.ready-action:hover:enabled {
      background: rgba(250, 204, 21, 0.22);
    }

    .handoff-button.reverse-action {
      border-color: rgba(192, 132, 252, 0.42);
      background: rgba(192, 132, 252, 0.16);
    }

    .handoff-button.reverse-action:hover:enabled {
      background: rgba(192, 132, 252, 0.24);
    }

    .handoff-actions {
      display: flex;
      flex-wrap: wrap;
      gap: 12px;
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
      <h1>Story Queue</h1>
      <p>Review draft, ready, and finished stories, then move them through the LotAT pipeline or load a ready story into the runtime slot.</p>
      <div class=\"story-list\" id=\"storyList\">
        <div class=\"loading\">Loading stories…</div>
      </div>
    </aside>
    <main class=\"main\" id=\"mainPanel\">
      <div class=\"empty\">
        <h2 style=\"margin-top: 0;\">Loading viewer…</h2>
        <p>Select a story from the sidebar once the list loads.</p>
      </div>
    </main>
  </div>

  <script>
    const state = {
      stories: { drafts: [], ready: [], finished: [] },
      loaded: null,
      selectedId: null,
      selectedStage: null,
      storyData: null,
      loading: false,
      collapsedNodeKeys: new Set(),
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

    function stageLabel(stage) {
      if (stage === "ready") return "Ready";
      if (stage === "finished") return "Finished";
      return "Draft";
    }

    function stageBadgeClass(stage) {
      if (stage === "ready") return "story-stage-ready";
      if (stage === "finished") return "story-stage-finished";
      return "story-stage-draft";
    }

    function isLoadedStory(storyStage, storyId) {
      if (storyStage !== "ready") {
        return false;
      }

      return !!(state.loaded && state.loaded.source_story_id === storyId);
    }

    function stageKeyToApiStage(stageKey) {
      return stageKey === "drafts" ? "draft" : stageKey;
    }

    function storyExists(stage, storyId) {
      if (stage === "draft") return state.stories.drafts.includes(storyId);
      if (stage === "ready") return state.stories.ready.includes(storyId);
      if (stage === "finished") return state.stories.finished.includes(storyId);
      return false;
    }

    function pickDefaultSelection() {
      if (state.stories.drafts.length) {
        state.selectedStage = "draft";
        state.selectedId = state.stories.drafts[0];
        return;
      }
      if (state.stories.ready.length) {
        state.selectedStage = "ready";
        state.selectedId = state.stories.ready[0];
        return;
      }
      if (state.stories.finished.length) {
        state.selectedStage = "finished";
        state.selectedId = state.stories.finished[0];
        return;
      }
      state.selectedStage = null;
      state.selectedId = null;
    }

    function renderStoryGroup(title, stageKey, stories) {
      if (!stories.length) {
        return "";
      }

      const stage = stageKeyToApiStage(stageKey);
      const itemsHtml = stories.map((storyId) => {
        const activeClass = storyId === state.selectedId && stage === state.selectedStage ? " active" : "";
        const stageClass = stage === "ready" ? " ready" : "";
        const subtitle = isLoadedStory(stage, storyId) ? `${stageLabel(stage)} • Loaded` : stageLabel(stage);
        return `
          <button class=\"story-item${activeClass}${stageClass}\" data-story-id=\"${escapeHtml(storyId)}\" data-story-stage=\"${escapeHtml(stage)}\">
            <div class=\"story-name\">${escapeHtml(storyId)}</div>
            <div class=\"story-subtitle\">${escapeHtml(subtitle)}</div>
          </button>
        `;
      }).join("");

      return `
        <div class=\"story-group\">
          <div class=\"story-group-title\">${escapeHtml(title)}</div>
          ${itemsHtml}
        </div>
      `;
    }

    function renderSidebar() {
      const list = document.getElementById("storyList");
      const hasDrafts = state.stories.drafts.length > 0;
      const hasReady = state.stories.ready.length > 0;
      const hasFinished = state.stories.finished.length > 0;

      if (!hasDrafts && !hasReady && !hasFinished) {
        list.innerHTML = `
          <div class=\"empty\" style=\"padding: 18px;\">
            <strong>No stories found.</strong>
            <p style=\"margin-bottom: 0; color: #94a3c7;\">Put <code class=\"inline-code\">.json</code> files in <code class=\"inline-code\">Creative/WorldBuilding/Storylines/drafts/</code>, <code class=\"inline-code\">Creative/WorldBuilding/Storylines/ready/</code>, or <code class=\"inline-code\">Creative/WorldBuilding/Storylines/finished/</code>. Loaded runtime copies are written to <code class=\"inline-code\">Creative/WorldBuilding/Storylines/loaded/</code>.</p>
          </div>
        `;
        return;
      }

      list.innerHTML = [
        renderStoryGroup("Draft Stories", "drafts", state.stories.drafts),
        renderStoryGroup("Ready Stories", "ready", state.stories.ready),
        renderStoryGroup("Finished Stories", "finished", state.stories.finished),
      ].join("");

      list.querySelectorAll("[data-story-id][data-story-stage]").forEach((button) => {
        button.addEventListener("click", () => {
          loadStory(button.getAttribute("data-story-stage"), button.getAttribute("data-story-id")).catch((error) => {
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

    function buildNodeKey(nodeId, branchTrail) {
      return branchTrail.concat(String(nodeId || "")).join(" > ");
    }

    function isNodeCollapsed(nodeKey) {
      return state.collapsedNodeKeys.has(nodeKey);
    }

    function bindTreeToggleHandlers() {
      document.querySelectorAll("[data-node-toggle='true']").forEach((card) => {
        const toggle = () => {
          const nodeKey = card.getAttribute("data-node-key") || "";
          const children = card.querySelector(":scope > .choices");
          const hint = card.querySelector("[data-toggle-hint]");
          if (!nodeKey || !children || !hint) {
            return;
          }

          const willCollapse = !children.classList.contains("children-collapsed");
          children.classList.toggle("children-collapsed", willCollapse);
          card.classList.toggle("collapsed", willCollapse);
          card.setAttribute("aria-expanded", willCollapse ? "false" : "true");
          hint.textContent = willCollapse ? "Expand" : "Collapse";

          if (willCollapse) {
            state.collapsedNodeKeys.add(nodeKey);
          } else {
            state.collapsedNodeKeys.delete(nodeKey);
          }
        };

        card.addEventListener("click", (event) => {
          event.stopPropagation();
          toggle();
        });
        card.addEventListener("keydown", (event) => {
          if (event.key !== "Enter" && event.key !== " ") {
            return;
          }

          event.preventDefault();
          event.stopPropagation();
          toggle();
        });
      });
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
      const nodeKey = buildNodeKey(nodeId, branchTrail);
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
      const isCollapsible = !duplicate && choices.length > 0;
      const isCollapsed = isCollapsible && isNodeCollapsed(nodeKey);
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
          <div
            class=\"node-card stage${isCollapsible ? " toggleable" : ""}${isCollapsed ? " collapsed" : ""}\"
            ${isCollapsible ? `data-node-toggle=\"true\" data-node-key=\"${escapeHtml(nodeKey)}\" tabindex=\"0\" role=\"button\" aria-expanded=\"${isCollapsed ? "false" : "true"}\"` : ""}
          >
            <div class=\"badge-row\">${badges.join("")}</div>
            <div class=\"node-title-row\">
              <h3 class=\"node-title\">${escapeHtml(node.title || nodeId)}</h3>
              ${isCollapsible ? `<span class=\"node-toggle-hint\" data-toggle-hint>${isCollapsed ? "Expand" : "Collapse"}</span>` : ""}
            </div>
            <p class=\"read-aloud\">${escapeHtml(node.read_aloud || "")}</p>
            ${duplicate ? `<div class=\"visited-note\">(visited earlier — ${escapeHtml(nodeId)})</div>` : ""}
            ${duplicate ? "" : `<div class=\"choices${isCollapsed ? " children-collapsed" : ""}\">${choiceHtml}</div>`}
          </div>
        </div>
      `;
    }

    function renderActionPanel(storyStage, storyId) {
      if (storyStage === "draft") {
        return `
          <section class=\"handoff-panel\">
            <div class=\"handoff-actions\">
              <button id=\"storyActionButton\" class=\"handoff-button\">HAND OFF STORY</button>
            </div>
            <div id=\"handoffStatus\" class=\"inline-status\"></div>
          </section>
        `;
      }

      if (storyStage === "ready") {
        const loadedNote = isLoadedStory(storyStage, storyId)
          ? `<div class=\"inline-status\">Loaded runtime copy: <code class=\"inline-code\">Creative/WorldBuilding/Storylines/loaded/${escapeHtml(state.loaded.loaded_file || "current-story.json")}</code></div>`
          : "";
        return `
          <section class=\"handoff-panel\">
            <div class=\"handoff-actions\">
              <button id=\"storyReverseButton\" class=\"handoff-button reverse-action\">MOVE TO DRAFTS</button>
              <button id=\"storyLoadButton\" class=\"handoff-button\">LOAD</button>
              <button id=\"storyActionButton\" class=\"handoff-button ready-action\">FINISHED</button>
            </div>
            ${loadedNote}
            <div id=\"handoffStatus\" class=\"inline-status\"></div>
          </section>
        `;
      }

      if (storyStage === "finished") {
        return `
          <section class=\"handoff-panel\">
            <div class=\"handoff-actions\">
              <button id=\"storyReverseButton\" class=\"handoff-button reverse-action\">MOVE TO READY</button>
            </div>
            <div id=\"handoffStatus\" class=\"inline-status\"></div>
          </section>
        `;
      }

      return `
        <section class=\"handoff-panel\">
          <div class=\"inline-status\">No further action is available for this story stage.</div>
        </section>
      `;
    }

    function renderStory(storyStage, storyId, story) {
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

      document.getElementById("mainPanel").innerHTML = `
        <section class=\"story-header\">
          <div class=\"badge-row\">
            ${badge(story.story_id || storyId, "muted")}
            ${story.starting_ship_section ? badge(story.starting_ship_section, "section") : ""}
            ${story.version ? badge(`v${story.version}`) : ""}
            ${badge(stageLabel(storyStage), stageBadgeClass(storyStage))}
            ${isLoadedStory(storyStage, storyId) ? badge("Loaded Runtime Copy", "commander") : ""}
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

        ${renderActionPanel(storyStage, storyId)}
      `;

      bindTreeToggleHandlers();

      const actionButton = document.getElementById("storyActionButton");
      const reverseButton = document.getElementById("storyReverseButton");
      const loadButton = document.getElementById("storyLoadButton");
      if (actionButton && storyStage === "draft") {
        actionButton.addEventListener("click", async () => {
          await handoffStory(storyId);
        });
      }
      if (actionButton && storyStage === "ready") {
        actionButton.addEventListener("click", async () => {
          await finishStory(storyId);
        });
      }
      if (reverseButton && storyStage === "ready") {
        reverseButton.addEventListener("click", async () => {
          await moveReadyStoryToDraft(storyId);
        });
      }
      if (loadButton && storyStage === "ready") {
        loadButton.addEventListener("click", async () => {
          await loadReadyStory(storyId);
        });
      }
      if (reverseButton && storyStage === "finished") {
        reverseButton.addEventListener("click", async () => {
          await moveFinishedStoryToReady(storyId);
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
      state.stories = {
        drafts: Array.isArray(payload.drafts) ? payload.drafts : [],
        ready: Array.isArray(payload.ready) ? payload.ready : [],
        finished: Array.isArray(payload.finished) ? payload.finished : [],
      };
      state.loaded = payload.loaded && typeof payload.loaded === "object" ? payload.loaded : null;

      if (!storyExists(state.selectedStage, state.selectedId)) {
        pickDefaultSelection();
      }

      renderSidebar();
      if (state.selectedId && state.selectedStage) {
        await loadStory(state.selectedStage, state.selectedId, false);
      } else {
        document.getElementById("mainPanel").innerHTML = `
          <div class=\"empty\">
            <h2 style=\"margin-top: 0;\">No stories found</h2>
            <p>Put story <code class=\"inline-code\">.json</code> files in <code class=\"inline-code\">Creative/WorldBuilding/Storylines/drafts/</code>, <code class=\"inline-code\">Creative/WorldBuilding/Storylines/ready/</code>, or <code class=\"inline-code\">Creative/WorldBuilding/Storylines/finished/</code>. Loaded runtime copies are written to <code class=\"inline-code\">Creative/WorldBuilding/Storylines/loaded/</code>.</p>
          </div>
        `;
      }
    }

    async function loadStory(storyStage, storyId, resetCollapsed = true) {
      state.selectedStage = storyStage;
      state.selectedId = storyId;
      if (resetCollapsed) {
        state.collapsedNodeKeys = new Set();
      }
      renderSidebar();
      document.getElementById("mainPanel").innerHTML = `
        <div class=\"empty\">
          <h2 style=\"margin-top: 0;\">Loading story…</h2>
          <p>${escapeHtml(storyId)}</p>
        </div>
      `;
      const story = await fetchJson(`/api/stories/${encodeURIComponent(storyStage)}/${encodeURIComponent(storyId)}`);
      state.storyData = story;
      renderSidebar();
      renderStory(storyStage, storyId, story);
    }

    async function handoffStory(storyId) {
      const button = document.getElementById("storyActionButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      button.textContent = "Handing Off…";
      status.textContent = "Moving story to ready…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/draft/${encodeURIComponent(storyId)}/handoff`, { method: "POST" });
        state.selectedStage = "ready";
        await loadStories();
      } catch (error) {
        button.disabled = false;
        button.textContent = "HAND OFF STORY";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    async function moveReadyStoryToDraft(storyId) {
      const button = document.getElementById("storyReverseButton");
      const forwardButton = document.getElementById("storyActionButton");
      const loadButton = document.getElementById("storyLoadButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      if (forwardButton) {
        forwardButton.disabled = true;
      }
      if (loadButton) {
        loadButton.disabled = true;
      }
      button.textContent = "Moving…";
      status.textContent = "Moving story back to drafts…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/ready/${encodeURIComponent(storyId)}/return-to-draft`, { method: "POST" });
        state.selectedStage = "draft";
        await loadStories();
      } catch (error) {
        button.disabled = false;
        if (forwardButton) {
          forwardButton.disabled = false;
        }
        if (loadButton) {
          loadButton.disabled = false;
        }
        button.textContent = "MOVE TO DRAFTS";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    async function loadReadyStory(storyId) {
      const button = document.getElementById("storyLoadButton");
      const reverseButton = document.getElementById("storyReverseButton");
      const forwardButton = document.getElementById("storyActionButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      if (reverseButton) {
        reverseButton.disabled = true;
      }
      if (forwardButton) {
        forwardButton.disabled = true;
      }
      button.textContent = "Loading…";
      status.textContent = "Copying story into the loaded runtime slot…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/ready/${encodeURIComponent(storyId)}/load`, { method: "POST" });
        status.textContent = `Loaded runtime copy updated from ${storyId}.`;
        await loadStories();
      } catch (error) {
        button.disabled = false;
        if (reverseButton) {
          reverseButton.disabled = false;
        }
        if (forwardButton) {
          forwardButton.disabled = false;
        }
        button.textContent = "LOAD";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    async function finishStory(storyId) {
      const button = document.getElementById("storyActionButton");
      const reverseButton = document.getElementById("storyReverseButton");
      const loadButton = document.getElementById("storyLoadButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      if (reverseButton) {
        reverseButton.disabled = true;
      }
      if (loadButton) {
        loadButton.disabled = true;
      }
      button.textContent = "Finishing…";
      status.textContent = "Moving story to finished…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/ready/${encodeURIComponent(storyId)}/finish`, { method: "POST" });
        state.selectedStage = "finished";
        await loadStories();
      } catch (error) {
        button.disabled = false;
        if (reverseButton) {
          reverseButton.disabled = false;
        }
        if (loadButton) {
          loadButton.disabled = false;
        }
        button.textContent = "FINISHED";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    async function moveFinishedStoryToReady(storyId) {
      const button = document.getElementById("storyReverseButton");
      const status = document.getElementById("handoffStatus");
      if (!button || !status) {
        return;
      }

      button.disabled = true;
      button.textContent = "Moving…";
      status.textContent = "Moving story back to ready…";
      status.classList.remove("error");

      try {
        await fetchJson(`/api/stories/finished/${encodeURIComponent(storyId)}/return-to-ready`, { method: "POST" });
        state.selectedStage = "ready";
        await loadStories();
      } catch (error) {
        button.disabled = false;
        button.textContent = "MOVE TO READY";
        status.textContent = error.message || String(error);
        status.classList.add("error");
      }
    }

    loadStories().catch((error) => {
      renderError(error.message || String(error));
      const list = document.getElementById("storyList");
      list.innerHTML = `<div class=\"error-box\" style=\"padding: 14px;\"><strong>Could not load story list.</strong><p style=\"margin-bottom: 0;\">${escapeHtml(error.message || String(error))}</p></div>`;
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
    """Ensure the draft, ready, loaded, and finished directories exist."""
    DRAFTS_DIR.mkdir(parents=True, exist_ok=True)
    READY_DIR.mkdir(parents=True, exist_ok=True)
    LOADED_DIR.mkdir(parents=True, exist_ok=True)
    FINISHED_DIR.mkdir(parents=True, exist_ok=True)


def sanitize_story_id(story_id: str) -> str:
    """Restrict story IDs to safe JSON filenames."""
    cleaned = Path(str(story_id or "")).name
    if cleaned != story_id:
        raise ValueError("Invalid story id.")
    if not cleaned.endswith(".json"):
        raise ValueError("Story id must be a .json filename.")
    if cleaned in {".", ".."}:
        raise ValueError("Invalid story id.")
    return cleaned


def get_stage_dir(stage: str) -> Path:
    """Resolve a supported story stage name to its directory."""
    normalized = str(stage or "").strip().lower()
    if normalized not in STORY_STAGE_DIRS:
        raise ValueError(f"Unsupported story stage: {stage}")
    return STORY_STAGE_DIRS[normalized]


def resolve_story_path(story_id: str, base_dir: Path = DRAFTS_DIR) -> Path:
    """Resolve a story file under the given base directory without traversal."""
    safe_name = sanitize_story_id(story_id)
    candidate = (base_dir / safe_name).resolve()
    base_resolved = base_dir.resolve()
    if candidate.parent != base_resolved:
        raise ValueError("Invalid story path.")
    return candidate


def list_story_files(base_dir: Path) -> list[str]:
    """Return the sorted list of JSON story filenames under a stage directory."""
    ensure_story_dirs()
    return sorted(path.name for path in base_dir.glob("*.json") if path.is_file())


def load_story_json(story_id: str, base_dir: Path) -> dict[str, Any]:
    """Load a story JSON file from the given stage directory."""
    path = resolve_story_path(story_id, base_dir)
    if not path.exists() or not path.is_file():
        raise FileNotFoundError(f"Story not found: {story_id}")

    try:
        payload = json.loads(path.read_text(encoding="utf-8"))
    except json.JSONDecodeError as error:
        raise ValueError(f"Invalid JSON in {story_id}: {error}") from error

    if not isinstance(payload, dict):
        raise ValueError(f"Story file must contain a JSON object: {story_id}")
    return payload


def get_loaded_story_path() -> Path:
    """Return the canonical runtime story path consumed by future engine code."""
    ensure_story_dirs()
    return (LOADED_DIR / LOADED_STORY_FILENAME).resolve()


def get_loaded_meta_path() -> Path:
    """Return the metadata path for the currently loaded runtime story copy."""
    ensure_story_dirs()
    return (LOADED_DIR / LOADED_META_FILENAME).resolve()


def get_loaded_story_info() -> dict[str, Any] | None:
    """Read metadata describing the currently loaded runtime story copy, if any."""
    meta_path = get_loaded_meta_path()
    story_path = get_loaded_story_path()
    if not meta_path.exists() or not story_path.exists():
        return None

    try:
        payload = json.loads(meta_path.read_text(encoding="utf-8"))
    except json.JSONDecodeError:
        return None

    if not isinstance(payload, dict):
        return None

    payload.setdefault("loaded_file", LOADED_STORY_FILENAME)
    payload.setdefault("loaded_path", str(story_path))
    return payload


def load_ready_story_into_runtime(story_id: str) -> dict[str, Any]:
    """Copy a ready story into the canonical loaded runtime slot for Streamer.bot consumers."""
    ensure_story_dirs()
    source_path = resolve_story_path(story_id, READY_DIR)
    if not source_path.exists() or not source_path.is_file():
        raise FileNotFoundError(f"Story not found in source stage: {story_id}")

    destination_path = get_loaded_story_path()
    shutil.copyfile(str(source_path), str(destination_path))

    payload = {
        "source_story_id": story_id,
        "source_path": str(source_path),
        "loaded_file": LOADED_STORY_FILENAME,
        "loaded_path": str(destination_path),
    }
    get_loaded_meta_path().write_text(json.dumps(payload, indent=2), encoding="utf-8")

    return {
        "ok": True,
        "story_id": story_id,
        "status": "loaded",
        **payload,
    }


def clear_loaded_story_if_matches(story_id: str) -> None:
    """Clear the runtime-loaded story when its source story leaves the ready queue."""
    loaded_info = get_loaded_story_info()
    if not loaded_info or loaded_info.get("source_story_id") != story_id:
        return

    loaded_path = get_loaded_story_path()
    meta_path = get_loaded_meta_path()
    if loaded_path.exists():
        loaded_path.unlink()
    if meta_path.exists():
        meta_path.unlink()


def move_story(story_id: str, source_dir: Path, destination_dir: Path, destination_status: str) -> dict[str, Any]:
    """Move a story between pipeline stage directories with idempotent success."""
    ensure_story_dirs()
    source_path = resolve_story_path(story_id, source_dir)
    destination_path = resolve_story_path(story_id, destination_dir)

    if destination_path.exists() and destination_path.is_file() and not source_path.exists():
        return {
            "ok": True,
            "story_id": story_id,
            "status": f"already_{destination_status}",
            "source_path": str(source_path),
            "destination_path": str(destination_path),
        }

    if not source_path.exists() or not source_path.is_file():
        raise FileNotFoundError(f"Story not found in source stage: {story_id}")

    if destination_path.exists():
        return {
            "ok": True,
            "story_id": story_id,
            "status": f"already_{destination_status}",
            "source_path": str(source_path),
            "destination_path": str(destination_path),
        }

    shutil.move(str(source_path), str(destination_path))
    if source_dir.resolve() == READY_DIR.resolve() and destination_dir.resolve() != READY_DIR.resolve():
        clear_loaded_story_if_matches(story_id)
    return {
        "ok": True,
        "story_id": story_id,
        "status": f"moved_to_{destination_status}",
        "source_path": str(source_path),
        "destination_path": str(destination_path),
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
            "drafts": list_story_files(DRAFTS_DIR),
            "ready": list_story_files(READY_DIR),
            "finished": list_story_files(FINISHED_DIR),
            "loaded": get_loaded_story_info(),
            "drafts_dir": str(DRAFTS_DIR),
            "ready_dir": str(READY_DIR),
            "loaded_dir": str(LOADED_DIR),
            "loaded_story_file": LOADED_STORY_FILENAME,
            "finished_dir": str(FINISHED_DIR),
        }

    @app.get("/api/stories/{story_stage}/{story_id}")
    def get_story(story_stage: str, story_id: str) -> dict[str, Any]:
        try:
            return load_story_json(story_id, get_stage_dir(story_stage))
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/draft/{story_id}/handoff")
    def post_handoff(story_id: str) -> dict[str, Any]:
        try:
            return move_story(story_id, DRAFTS_DIR, READY_DIR, "ready")
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/ready/{story_id}/return-to-draft")
    def post_return_to_draft(story_id: str) -> dict[str, Any]:
        try:
            return move_story(story_id, READY_DIR, DRAFTS_DIR, "draft")
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/ready/{story_id}/load")
    def post_load(story_id: str) -> dict[str, Any]:
        try:
            return load_ready_story_into_runtime(story_id)
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/ready/{story_id}/finish")
    def post_finish(story_id: str) -> dict[str, Any]:
        try:
            return move_story(story_id, READY_DIR, FINISHED_DIR, "finished")
        except FileNotFoundError as error:
            raise HTTPException(status_code=404, detail=str(error)) from error
        except ValueError as error:
            raise HTTPException(status_code=400, detail=str(error)) from error

    @app.post("/api/stories/finished/{story_id}/return-to-ready")
    def post_return_to_ready(story_id: str) -> dict[str, Any]:
        try:
            return move_story(story_id, FINISHED_DIR, READY_DIR, "ready")
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
    print(f"[lotat-viewer] Loaded runtime story: {get_loaded_story_path()}")
    print(f"[lotat-viewer] Finished stories: {FINISHED_DIR}")
    print(f"[lotat-viewer] Serving http://{args.host}:{args.port}")

    uvicorn.run(app, host=args.host, port=args.port, reload=args.reload)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

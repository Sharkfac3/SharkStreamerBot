#!/usr/bin/env python3
"""Sync routing tables in markdown docs from .agents/routing-manifest.json."""

import json
import os
import re
from pathlib import Path


TABLE_HEADING_RE_TEMPLATE = r"^##\s+{}\s*$"
GENERATED_BLOCK_RE_TEMPLATE = r"<!-- GENERATED:{}:start -->\n.*?<!-- GENERATED:{}:end -->"


def find_repo_root(start: str) -> Path:
    current = Path(start).resolve()
    for _ in range(10):
        if (current / "AGENTS.md").is_file():
            return current
        if current.parent == current:
            break
        current = current.parent
    raise RuntimeError("Could not find repo root (AGENTS.md not found).")


def format_alias_targets(targets: list[str]) -> str:
    return " or ".join(f"`{target}`" for target in targets)


def render_table(lines: list[str]) -> str:
    return "\n".join(lines) + "\n"


def replace_table_under_heading(raw_text: str, heading: str, new_table_lines: list[str]) -> str:
    heading_re = re.compile(TABLE_HEADING_RE_TEMPLATE.format(re.escape(heading)), re.M)
    match = heading_re.search(raw_text)
    if not match:
        raise RuntimeError(f"Could not find heading '## {heading}'")

    start = match.end()
    lines = raw_text[start:].splitlines(keepends=True)

    prefix_parts: list[str] = []
    index = 0
    while index < len(lines):
        stripped = lines[index].strip()
        if stripped.startswith("|"):
            break
        if lines[index].startswith("## "):
            raise RuntimeError(f"No markdown table found under heading '## {heading}'")
        prefix_parts.append(lines[index])
        index += 1

    if index >= len(lines) or not lines[index].strip().startswith("|"):
        raise RuntimeError(f"No markdown table found under heading '## {heading}'")

    while index < len(lines) and lines[index].strip().startswith("|"):
        index += 1

    replacement = "".join(prefix_parts) + render_table(new_table_lines)
    suffix = "".join(lines[index:])
    return raw_text[:start] + replacement + suffix



def replace_generated_block(raw_text: str, block_name: str, new_body: str) -> str:
    pattern = re.compile(
        GENERATED_BLOCK_RE_TEMPLATE.format(re.escape(block_name), re.escape(block_name)),
        re.S,
    )
    replacement = (
        f"<!-- GENERATED:{block_name}:start -->\n"
        f"{new_body}"
        f"<!-- GENERATED:{block_name}:end -->"
    )
    if not pattern.search(raw_text):
        raise RuntimeError(f"Could not find generated block '{block_name}'")
    return pattern.sub(replacement, raw_text, count=1)


def main() -> int:
    repo = find_repo_root(os.path.dirname(os.path.abspath(__file__)))
    manifest = json.loads((repo / ".agents" / "routing-manifest.json").read_text(encoding="utf-8"))

    roles = manifest["roles"]
    helpers = manifest["helpers"]
    quick_routing = manifest["quick_routing"]
    aliases = manifest["aliases"]

    role_table = [
        "| Role | Folder | When to Activate |",
        "|---|---|---|",
        *[
            f"| `{role['name']}` | `{role['folder']}` | {role['when']} |"
            for role in roles
        ],
    ]

    meta_table = [
        "| Wrapper | Purpose |",
        "|---|---|",
        *[
            f"| `{helper['name']}/SKILL.md` | {helper['purpose']} |"
            for helper in helpers
        ],
    ]

    alias_table = [
        "| Alias | Canonical wrapper |",
        "|---|---|",
        *[
            f"| `{alias['name']}` | {format_alias_targets(alias['canonical_targets'])} |"
            for alias in aliases
        ],
    ]

    quick_routing_table = [
        "| You're working on | Role | Agent Tree |",
        "|---|---|---|",
        *[
            f"| {row['work']} | `{row['role']}` | `{row['agent_tree']}` |"
            for row in quick_routing
        ],
    ]

    agents_path = repo / "AGENTS.md"
    agents_raw = agents_path.read_text(encoding="utf-8")
    agents_raw = replace_generated_block(agents_raw, "agents-quick-role-routing", render_table(quick_routing_table))
    agents_path.write_text(agents_raw, encoding="utf-8")

    entry_path = repo / ".agents" / "ENTRY.md"
    entry_raw = entry_path.read_text(encoding="utf-8")
    entry_raw = replace_table_under_heading(entry_raw, "Roles", role_table)
    entry_path.write_text(entry_raw, encoding="utf-8")

    readme_path = repo / ".pi" / "skills" / "README.md"
    readme_raw = readme_path.read_text(encoding="utf-8")
    readme_raw = replace_table_under_heading(readme_raw, "Roles", role_table)
    readme_raw = replace_table_under_heading(readme_raw, "Meta Wrappers", meta_table)
    readme_raw = replace_table_under_heading(readme_raw, "Compatibility Aliases", alias_table)
    readme_path.write_text(readme_raw, encoding="utf-8")

    print("Synced routing tables from .agents/routing-manifest.json:")
    print("- AGENTS.md")
    print("- .agents/ENTRY.md")
    print("- .pi/skills/README.md")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

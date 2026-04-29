#!/usr/bin/env python3
"""Validate Streamer.bot action scripts against AGENTS.md action contracts.

Local Actions/**/AGENTS.md files can contain a machine-readable contract block:

<!-- ACTION-CONTRACTS:START -->
```json
{
  "version": 1,
  "contracts": [
    {
      "script": "example.cs",
      "action": "Streamer.bot Action Name",
      "purpose": "What this action is supposed to do.",
      "triggers": ["Twitch -> Chat Message"],
      "globals": ["exampleGlobal"],
      "obsSources": ["Example Source"],
      "timers": [],
      "mixItUpCommandIds": [],
      "overlayTopics": [],
      "serviceUrls": [],
      "requiredLiterals": [],
      "runtimeBehavior": ["Exit safely when disabled."],
      "failureBehavior": ["Log and return true on missing optional integration."],
      "pasteTarget": "Matching Streamer.bot Execute C# Code action"
    }
  ]
}
```
<!-- ACTION-CONTRACTS:END -->

A script is considered aligned only when it has a current source/stamp header:

// ACTION-CONTRACT: Actions/Example/AGENTS.md#example.cs
// ACTION-CONTRACT-SHA256: <sha256 of canonical contract json>

Use --stamp to insert or refresh the stamp after updating the source contract.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Iterable

REPO_ROOT = Path(__file__).resolve().parents[3]
ACTIONS_DIR = REPO_ROOT / "Actions"
START_MARKER = "<!-- ACTION-CONTRACTS:START -->"
END_MARKER = "<!-- ACTION-CONTRACTS:END -->"
FENCE_RE = re.compile(r"```(?:json)?\s*(.*?)\s*```", re.DOTALL)
CONTRACT_LINE_RE = re.compile(r"^\s*//\s*ACTION-CONTRACT:\s*(.+?)\s*$", re.MULTILINE)
HASH_LINE_RE = re.compile(r"^\s*//\s*ACTION-CONTRACT-SHA256:\s*([a-fA-F0-9]{64})\s*$", re.MULTILINE)
STAMP_BLOCK_RE = re.compile(
    r"\A(?P<prefix>(?:\ufeff)?(?:\s*//.*\n|\s*/\*.*?\*/\s*)*)"
    r"(?://\s*ACTION-CONTRACT:\s*.+?\n//\s*ACTION-CONTRACT-SHA256:\s*[a-fA-F0-9]{64}\s*\n\n?)?",
    re.DOTALL,
)

LITERAL_FIELDS = (
    "globals",
    "timers",
    "obsSources",
    "obsScenes",
    "mixItUpCommandIds",
    "overlayTopics",
    "serviceUrls",
    "requiredLiterals",
)
REQUIRED_CONTRACT_FIELDS = ("script", "action", "purpose", "runtimeBehavior", "pasteTarget")


@dataclass(frozen=True)
class ContractRef:
    agents_path: Path
    script_path: Path
    contract: dict[str, Any]

    @property
    def source_label(self) -> str:
        return f"{self.agents_path.relative_to(REPO_ROOT).as_posix()}#{self.contract['script']}"

    @property
    def digest(self) -> str:
        canonical = json.dumps(self.contract, sort_keys=True, separators=(",", ":"), ensure_ascii=False)
        return hashlib.sha256(canonical.encode("utf-8")).hexdigest()


def repo_rel(path: Path) -> str:
    return path.relative_to(REPO_ROOT).as_posix()


def run_git_changed(base_ref: str | None) -> list[Path]:
    commands = []
    if base_ref:
        commands.append(["git", "diff", "--name-only", base_ref, "--", "Actions"])
    commands.extend([
        ["git", "diff", "--name-only", "--", "Actions"],
        ["git", "diff", "--name-only", "--cached", "--", "Actions"],
        ["git", "ls-files", "--others", "--exclude-standard", "--", "Actions"],
    ])
    names: set[str] = set()
    for command in commands:
        result = subprocess.run(command, cwd=REPO_ROOT, text=True, capture_output=True, check=False)
        if result.returncode not in (0, 1):
            continue
        for line in result.stdout.splitlines():
            if line.endswith(".cs") and line.startswith("Actions/"):
                names.add(line)
    return sorted((REPO_ROOT / name for name in names), key=lambda p: repo_rel(p))


def find_nearest_agents(script_path: Path) -> Path | None:
    current = script_path.parent
    while current == ACTIONS_DIR or ACTIONS_DIR in current.parents:
        candidate = current / "AGENTS.md"
        if candidate.exists():
            return candidate
        if current == ACTIONS_DIR:
            break
        current = current.parent
    return None


def extract_contract_doc(agents_path: Path) -> dict[str, Any]:
    text = agents_path.read_text(encoding="utf-8")
    if START_MARKER not in text or END_MARKER not in text:
        raise ValueError(f"{repo_rel(agents_path)} has no {START_MARKER} block")
    block = text.split(START_MARKER, 1)[1].split(END_MARKER, 1)[0]
    fence = FENCE_RE.search(block)
    raw = fence.group(1) if fence else block.strip()
    try:
        data = json.loads(raw)
    except json.JSONDecodeError as exc:
        raise ValueError(f"{repo_rel(agents_path)} action contract JSON is invalid: {exc}") from exc
    if data.get("version") != 1:
        raise ValueError(f"{repo_rel(agents_path)} action contract block must declare version 1")
    if not isinstance(data.get("contracts"), list):
        raise ValueError(f"{repo_rel(agents_path)} action contract block must contain contracts[]")
    return data


def get_contract_for_script(script_path: Path) -> ContractRef:
    agents_path = find_nearest_agents(script_path)
    if not agents_path:
        raise ValueError(f"{repo_rel(script_path)} has no nearest AGENTS.md")
    data = extract_contract_doc(agents_path)
    script_rel = script_path.relative_to(agents_path.parent).as_posix()
    matches = [item for item in data["contracts"] if isinstance(item, dict) and item.get("script") == script_rel]
    if not matches:
        raise ValueError(f"{repo_rel(script_path)} has no action contract in {repo_rel(agents_path)} for script '{script_rel}'")
    if len(matches) > 1:
        raise ValueError(f"{repo_rel(agents_path)} has duplicate action contracts for script '{script_rel}'")
    contract = matches[0]
    missing = [field for field in REQUIRED_CONTRACT_FIELDS if not contract.get(field)]
    if missing:
        raise ValueError(f"{repo_rel(agents_path)} contract for '{script_rel}' is missing required fields: {', '.join(missing)}")
    if not isinstance(contract.get("runtimeBehavior"), list) or not contract["runtimeBehavior"]:
        raise ValueError(f"{repo_rel(agents_path)} contract for '{script_rel}' must have non-empty runtimeBehavior[]")
    return ContractRef(agents_path=agents_path, script_path=script_path, contract=contract)


def iter_literals(contract: dict[str, Any]) -> Iterable[tuple[str, str]]:
    for field in LITERAL_FIELDS:
        values = contract.get(field, [])
        if values is None:
            continue
        if not isinstance(values, list):
            yield field, f"<INVALID non-list {field}>"
            continue
        for value in values:
            if isinstance(value, str) and value:
                yield field, value
            elif isinstance(value, dict):
                literal = value.get("value") or value.get("name") or value.get("id") or value.get("url")
                if isinstance(literal, str) and literal:
                    yield field, literal
                else:
                    yield field, f"<INVALID literal object in {field}>"
            else:
                yield field, f"<INVALID literal in {field}>"


def validate_ref(ref: ContractRef, require_stamp: bool, check_literals: bool) -> list[str]:
    errors: list[str] = []
    text = ref.script_path.read_text(encoding="utf-8-sig")
    contract_line = CONTRACT_LINE_RE.search(text)
    hash_line = HASH_LINE_RE.search(text)
    if require_stamp:
        if not contract_line:
            errors.append(f"{repo_rel(ref.script_path)} missing ACTION-CONTRACT source stamp")
        elif contract_line.group(1) != ref.source_label:
            errors.append(
                f"{repo_rel(ref.script_path)} contract source stamp is '{contract_line.group(1)}', expected '{ref.source_label}'"
            )
        if not hash_line:
            errors.append(f"{repo_rel(ref.script_path)} missing ACTION-CONTRACT-SHA256 stamp")
        elif hash_line.group(1).lower() != ref.digest:
            errors.append(
                f"{repo_rel(ref.script_path)} contract hash is stale; run this tool with --stamp after updating {repo_rel(ref.agents_path)}"
            )
    if check_literals:
        for field, literal in iter_literals(ref.contract):
            if literal.startswith("<INVALID"):
                errors.append(f"{repo_rel(ref.agents_path)} contract for {ref.contract['script']} has {literal}")
            elif literal not in text:
                errors.append(f"{repo_rel(ref.script_path)} does not contain documented {field} literal: {literal!r}")
    return errors


def stamp_ref(ref: ContractRef) -> bool:
    text = ref.script_path.read_text(encoding="utf-8-sig")
    stamp = f"// ACTION-CONTRACT: {ref.source_label}\n// ACTION-CONTRACT-SHA256: {ref.digest}\n\n"
    match = STAMP_BLOCK_RE.match(text)
    if match:
        prefix = match.group("prefix") or ""
        remainder = text[match.end():]
        new_text = prefix + stamp + remainder.lstrip("\n")
    else:
        new_text = stamp + text
    if new_text != text:
        ref.script_path.write_text(new_text, encoding="utf-8")
        return True
    return False


def collect_scripts(args: argparse.Namespace) -> list[Path]:
    scripts: list[Path] = []
    if args.scripts:
        scripts.extend((REPO_ROOT / item).resolve() for item in args.scripts)
    if args.changed:
        scripts.extend(run_git_changed(args.base_ref))
    if args.all:
        scripts.extend(ACTIONS_DIR.rglob("*.cs"))
    dedup: dict[str, Path] = {}
    for script in scripts:
        if script.exists() and script.suffix == ".cs" and (script == ACTIONS_DIR or ACTIONS_DIR in script.parents):
            dedup[repo_rel(script)] = script
    return [dedup[key] for key in sorted(dedup)]


def main() -> int:
    parser = argparse.ArgumentParser(description="Validate Actions .cs files against nearest AGENTS.md action contracts.")
    scope = parser.add_mutually_exclusive_group()
    scope.add_argument("--changed", action="store_true", help="validate changed Actions/**/*.cs files from git diff/untracked state (default)")
    scope.add_argument("--all", action="store_true", help="validate every Actions/**/*.cs script")
    parser.add_argument("--base-ref", help="optional git base ref for --changed, e.g. origin/main")
    parser.add_argument("--script", dest="scripts", action="append", help="specific script path to validate; may be repeated")
    parser.add_argument("--stamp", action="store_true", help="insert/update ACTION-CONTRACT stamps from AGENTS.md contracts before validating")
    parser.add_argument("--no-literal-check", action="store_true", help="skip documented literal presence checks")
    parser.add_argument("--no-stamp-required", action="store_true", help="do not require script stamp lines")
    args = parser.parse_args()
    if not args.changed and not args.all and not args.scripts:
        args.changed = True

    scripts = collect_scripts(args)
    if not scripts:
        print("No Actions/**/*.cs scripts selected.")
        return 0

    errors: list[str] = []
    stamped: list[str] = []
    for script in scripts:
        try:
            ref = get_contract_for_script(script)
            if args.stamp and stamp_ref(ref):
                stamped.append(repo_rel(script))
            errors.extend(validate_ref(ref, require_stamp=not args.no_stamp_required, check_literals=not args.no_literal_check))
        except ValueError as exc:
            errors.append(str(exc))

    if stamped:
        print("Stamped action contracts:")
        for item in stamped:
            print(f"  - {item}")
    if errors:
        print("Action contract validation failed:")
        for error in errors:
            print(f"  - {error}")
        return 1
    print(f"Action contract validation passed for {len(scripts)} script(s).")
    return 0


if __name__ == "__main__":
    sys.exit(main())

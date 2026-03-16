#!/usr/bin/env python3
"""
validate.py — SharkStreamerBot pre-commit validator

Checks:
  1. All files listed in Actions/SHARED-CONSTANTS.md "Used in:" blocks exist.
  2. Constant string values in SHARED-CONSTANTS.md match what is declared
     in each listed .cs file (detects silent rename drift).
  3. Story JSON files in Creative/WorldBuilding/Storylines/ conform to
     the shared story schema (required fields, node integrity, node graph).

Usage:
  python Tools/StreamerBot/Validation/validate.py     # run from repo root
  python validate.py                                   # run from Validation/

Exit codes:
  0  — all checks passed
  1  — one or more checks failed (blocks commit when used as a pre-commit hook)
"""

import json
import os
import re
import sys

# Ensure UTF-8 output on Windows consoles that default to cp1252
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# ---------------------------------------------------------------------------
# Repo root detection
# ---------------------------------------------------------------------------

def find_repo_root(start: str) -> str:
    """Walk up from start until we find AGENTS.md (repo root marker)."""
    current = os.path.abspath(start)
    for _ in range(10):
        if os.path.isfile(os.path.join(current, "AGENTS.md")):
            return current
        parent = os.path.dirname(current)
        if parent == current:
            break
        current = parent
    raise RuntimeError(
        "Could not find repo root (AGENTS.md not found). "
        "Run from inside the SharkStreamerBot repo."
    )


# ---------------------------------------------------------------------------
# Output helpers
# ---------------------------------------------------------------------------

PASS = "  \033[32m✓\033[0m"
FAIL = "  \033[31m✗\033[0m"
WARN = "  \033[33m⚠\033[0m"
SECTION = "\033[1m{}\033[0m"


def header(title: str) -> None:
    print(f"\n{SECTION.format(title)}")
    print("─" * (len(title) + 2))


def ok(msg: str) -> None:
    print(f"{PASS} {msg}")


def fail(msg: str) -> None:
    print(f"{FAIL} {msg}")


def warn(msg: str) -> None:
    print(f"{WARN} {msg}")


# ---------------------------------------------------------------------------
# Check 1 + 2: SHARED-CONSTANTS.md
# ---------------------------------------------------------------------------

# Matches:  - `CONST_NAME` = `value`   (with optional trailing note like *(persisted)*)
CONST_LINE_RE = re.compile(r"^- `([^`]+)` = `([^`]+)`")

# Matches:  - `Actions/path/file.cs`
FILE_LINE_RE = re.compile(r"^- `([^`]+\.cs)`")

# Matches C# const string declarations: [modifier] const string NAME = "value";
CS_CONST_RE = re.compile(r'const\s+string\s+(\w+)\s*=\s*"([^"]*)"')


def parse_shared_constants(md_path: str):
    """
    Parse SHARED-CONSTANTS.md into a list of sections.
    Each section: {"name": str, "constants": [(name, value), ...], "files": [path, ...]}
    """
    sections = []
    current: dict | None = None
    in_used_in = False

    with open(md_path, encoding="utf-8") as f:
        for raw_line in f:
            line = raw_line.rstrip()

            if line.startswith("## "):
                if current:
                    sections.append(current)
                current = {"name": line[3:].strip(), "constants": [], "files": []}
                in_used_in = False

            elif line.strip() == "Used in:":
                in_used_in = True

            elif line.strip() == "---":
                in_used_in = False

            elif not in_used_in and current:
                m = CONST_LINE_RE.match(line.strip())
                if m:
                    current["constants"].append((m.group(1), m.group(2)))

            elif in_used_in and current:
                m = FILE_LINE_RE.match(line.strip())
                if m:
                    current["files"].append(m.group(1))

    if current:
        sections.append(current)

    return sections


def load_cs_constants(cs_path: str) -> dict:
    """Extract all 'const string NAME = "value"' declarations from a .cs file."""
    constants = {}
    try:
        with open(cs_path, encoding="utf-8") as f:
            content = f.read()
        for m in CS_CONST_RE.finditer(content):
            constants[m.group(1)] = m.group(2)
    except OSError:
        pass
    return constants


def check_shared_constants(repo_root: str) -> bool:
    header("Check 1 & 2 — SHARED-CONSTANTS.md")

    md_path = os.path.join(repo_root, "Actions", "SHARED-CONSTANTS.md")
    if not os.path.isfile(md_path):
        fail("Actions/SHARED-CONSTANTS.md not found.")
        return False

    sections = parse_shared_constants(md_path)
    passed = True

    for section in sections:
        section_name = section["name"]
        constants = section["constants"]   # [(name, value), ...]
        files = section["files"]           # ["Actions/path/file.cs", ...]

        if not constants and not files:
            continue

        # --- Check 1: file existence ---
        for rel_path in files:
            abs_path = os.path.join(repo_root, rel_path.replace("/", os.sep))
            if not os.path.isfile(abs_path):
                fail(f"[{section_name}] Listed file not found: {rel_path}")
                passed = False
            else:
                ok(f"[{section_name}] File exists: {rel_path}")

        # --- Check 2: value drift ---
        # For each constant in the section, check each listed file that
        # actually declares that constant name uses the same value.
        for const_name, doc_value in constants:
            for rel_path in files:
                abs_path = os.path.join(repo_root, rel_path.replace("/", os.sep))
                if not os.path.isfile(abs_path):
                    continue  # already reported above

                cs_consts = load_cs_constants(abs_path)
                if const_name in cs_consts:
                    actual_value = cs_consts[const_name]
                    if actual_value != doc_value:
                        fail(
                            f"[{section_name}] VALUE DRIFT in {rel_path}: "
                            f"{const_name} is \"{actual_value}\" in code "
                            f"but \"{doc_value}\" in SHARED-CONSTANTS.md"
                        )
                        passed = False
                    else:
                        ok(
                            f"[{section_name}] {const_name} = \"{doc_value}\" "
                            f"matches in {os.path.basename(rel_path)}"
                        )

    return passed


# ---------------------------------------------------------------------------
# Check 3: Story JSON schema
# ---------------------------------------------------------------------------

STORY_TOP_REQUIRED = [
    "story_id", "title", "tone", "version", "summary",
    "starting_ship_section", "starting_node_id",
    "supported_mechanics", "cast", "ship_sections_used",
    "commands_used", "nodes",
]

SUPPORTED_MECHANICS_KEYS = [
    "chat_voting", "chaos_meter", "commander_moments", "dice_hooks", "landing_party",
]

NODE_REQUIRED = [
    "node_id", "node_type", "ship_section", "title", "read_aloud",
    "chaos", "dice_hook", "commander_moment", "choices", "tags", "end_state",
]

CHAOS_KEYS = ["on_enter", "on_success", "on_failure"]
DICE_HOOK_KEYS = ["enabled"]
COMMANDER_MOMENT_KEYS = ["enabled"]
VALID_NODE_TYPES = {"stage", "ending"}
VALID_END_STATES = {"success", "failure", "partial", "disaster", "escape", "unknown"}


def validate_story_file(json_path: str) -> list:
    """
    Validate a story JSON file.
    Returns a list of error strings (empty = valid).
    """
    errors = []
    filename = os.path.basename(json_path)

    try:
        with open(json_path, encoding="utf-8") as f:
            story = json.load(f)
    except json.JSONDecodeError as e:
        return [f"{filename}: Invalid JSON — {e}"]
    except OSError as e:
        return [f"{filename}: Could not read — {e}"]

    # Top-level required fields
    for field in STORY_TOP_REQUIRED:
        if field not in story:
            errors.append(f"{filename}: Missing top-level field '{field}'")

    # supported_mechanics keys
    mechanics = story.get("supported_mechanics", {})
    if isinstance(mechanics, dict):
        for key in SUPPORTED_MECHANICS_KEYS:
            if key not in mechanics:
                errors.append(f"{filename}: supported_mechanics missing key '{key}'")
    else:
        errors.append(f"{filename}: 'supported_mechanics' must be an object")

    # cast keys
    cast = story.get("cast", {})
    if isinstance(cast, dict):
        for key in ["commanders_used", "squad_members_used"]:
            if key not in cast:
                errors.append(f"{filename}: cast missing key '{key}'")
    else:
        errors.append(f"{filename}: 'cast' must be an object")

    # nodes
    nodes = story.get("nodes", [])
    if not isinstance(nodes, list):
        errors.append(f"{filename}: 'nodes' must be an array")
        return errors

    if len(nodes) == 0:
        errors.append(f"{filename}: 'nodes' array is empty")
        return errors

    # Build node_id set for graph validation
    node_ids = set()
    seen_ids = set()

    for i, node in enumerate(nodes):
        if not isinstance(node, dict):
            errors.append(f"{filename}: node[{i}] is not an object")
            continue

        node_id = node.get("node_id", f"<missing node_id at index {i}>")

        # Duplicate node_id
        if node_id in seen_ids:
            errors.append(f"{filename}: Duplicate node_id '{node_id}'")
        seen_ids.add(node_id)
        node_ids.add(node_id)

        # Required node fields
        for field in NODE_REQUIRED:
            if field not in node:
                errors.append(f"{filename}: node '{node_id}' missing field '{field}'")

        # node_type validation
        node_type = node.get("node_type")
        if node_type not in VALID_NODE_TYPES:
            errors.append(
                f"{filename}: node '{node_id}' has invalid node_type '{node_type}' "
                f"(must be one of: {', '.join(sorted(VALID_NODE_TYPES))})"
            )

        # chaos keys
        chaos = node.get("chaos", {})
        if isinstance(chaos, dict):
            for key in CHAOS_KEYS:
                if key not in chaos:
                    errors.append(f"{filename}: node '{node_id}' chaos missing key '{key}'")
        else:
            errors.append(f"{filename}: node '{node_id}' chaos must be an object")

        # dice_hook has 'enabled'
        dice_hook = node.get("dice_hook", {})
        if isinstance(dice_hook, dict):
            if "enabled" not in dice_hook:
                errors.append(f"{filename}: node '{node_id}' dice_hook missing key 'enabled'")
        else:
            errors.append(f"{filename}: node '{node_id}' dice_hook must be an object")

        # commander_moment has 'enabled'
        cm = node.get("commander_moment", {})
        if isinstance(cm, dict):
            if "enabled" not in cm:
                errors.append(f"{filename}: node '{node_id}' commander_moment missing key 'enabled'")
        else:
            errors.append(f"{filename}: node '{node_id}' commander_moment must be an object")

        # Ending node rules
        choices = node.get("choices", [])
        end_state = node.get("end_state")

        if node_type == "ending":
            if choices:
                errors.append(f"{filename}: ending node '{node_id}' should have empty choices array")
            if end_state is None:
                errors.append(f"{filename}: ending node '{node_id}' must have a non-null end_state")

        # Stage node rules
        if node_type == "stage":
            if end_state is not None:
                errors.append(
                    f"{filename}: stage node '{node_id}' should have null end_state "
                    f"(got '{end_state}')"
                )
            if not choices:
                errors.append(
                    f"{filename}: stage node '{node_id}' has no choices — "
                    "stage nodes should have at least one choice"
                )

    # Graph validation: starting_node_id exists
    starting_node_id = story.get("starting_node_id")
    if starting_node_id and starting_node_id not in node_ids:
        errors.append(
            f"{filename}: starting_node_id '{starting_node_id}' "
            "does not match any node_id in 'nodes'"
        )

    # Graph validation: all next_node_id references point to valid nodes
    for node in nodes:
        if not isinstance(node, dict):
            continue
        node_id = node.get("node_id", "<unknown>")
        for choice in node.get("choices", []):
            if not isinstance(choice, dict):
                continue
            next_id = choice.get("next_node_id")
            if next_id and next_id not in node_ids:
                errors.append(
                    f"{filename}: node '{node_id}' choice '{choice.get('choice_id', '?')}' "
                    f"references unknown next_node_id '{next_id}'"
                )

    return errors


def check_story_json(repo_root: str) -> bool:
    header("Check 3 — Story JSON Schema")

    storylines_dir = os.path.join(
        repo_root, "Creative", "WorldBuilding", "Storylines"
    )

    if not os.path.isdir(storylines_dir):
        warn("Creative/WorldBuilding/Storylines/ not found — skipping story check.")
        return True

    json_files = [
        os.path.join(storylines_dir, f)
        for f in os.listdir(storylines_dir)
        if f.endswith(".json")
    ]

    if not json_files:
        warn("No story JSON files found in Creative/WorldBuilding/Storylines/ — skipping.")
        return True

    passed = True
    for json_path in sorted(json_files):
        errors = validate_story_file(json_path)
        filename = os.path.basename(json_path)
        if errors:
            for err in errors:
                fail(err)
            passed = False
        else:
            ok(f"{filename} — schema valid")

    return passed


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------

def main() -> int:
    # Find repo root whether run from repo root, Validation/, or as a git hook
    script_dir = os.path.dirname(os.path.abspath(__file__))
    try:
        repo_root = find_repo_root(script_dir)
    except RuntimeError as e:
        print(f"\033[31mERROR: {e}\033[0m")
        return 1

    print(f"\n\033[1mSharkStreamerBot Validator\033[0m")
    print(f"Repo: {repo_root}")

    constants_ok = check_shared_constants(repo_root)
    story_ok = check_story_json(repo_root)

    all_passed = constants_ok and story_ok

    print()
    if all_passed:
        print("\033[32m✓ All checks passed.\033[0m\n")
        return 0
    else:
        print("\033[31m✗ One or more checks failed. Fix the issues above before committing.\033[0m\n")
        return 1


if __name__ == "__main__":
    sys.exit(main())

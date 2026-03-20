#!/usr/bin/env python3
"""
validate.py — SharkStreamerBot pre-commit validator

Checks:
  1. All files listed in Actions/SHARED-CONSTANTS.md "Used in:" blocks exist.
  2. Constant string values in SHARED-CONSTANTS.md match what is declared
     in each listed .cs file (detects silent rename drift).
  3. Story JSON files in Creative/WorldBuilding/Storylines/ conform to
     the shared story schema (required fields, node integrity, node graph).
  4. Pi skill wrappers follow the project's flat wrapper contract and point
     to valid Pi wrapper targets.
  5. Agent/Pi routing docs and manifests stay in sync.
  6. Routing manifest collisions, duplicate entries, and stale wrapper references are rejected.

Usage:
  python3 Tools/StreamerBot/Validation/validate.py     # run from repo root
  python3 validate.py                                  # run from Validation/

Exit codes:
  0  — all checks passed
  1  — one or more checks failed (blocks commit when used as a pre-commit hook)
"""

import json
import os
import re
import sys
from pathlib import Path

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

CONST_LINE_RE = re.compile(r"^- `([^`]+)` = `([^`]+)`")
FILE_LINE_RE = re.compile(r"^- `([^`]+\.cs)`")
CS_CONST_RE = re.compile(r'const\s+string\s+(\w+)\s*=\s*"([^"]*)"')


def parse_shared_constants(md_path: str):
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
        constants = section["constants"]
        files = section["files"]

        if not constants and not files:
            continue

        for rel_path in files:
            abs_path = os.path.join(repo_root, rel_path.replace("/", os.sep))
            if not os.path.isfile(abs_path):
                fail(f"[{section_name}] Listed file not found: {rel_path}")
                passed = False
            else:
                ok(f"[{section_name}] File exists: {rel_path}")

        for const_name, doc_value in constants:
            for rel_path in files:
                abs_path = os.path.join(repo_root, rel_path.replace("/", os.sep))
                if not os.path.isfile(abs_path):
                    continue

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
VALID_NODE_TYPES = {"stage", "ending"}


def validate_story_file(json_path: str) -> list[str]:
    errors = []
    filename = os.path.basename(json_path)

    try:
        with open(json_path, encoding="utf-8") as f:
            story = json.load(f)
    except json.JSONDecodeError as e:
        return [f"{filename}: Invalid JSON — {e}"]
    except OSError as e:
        return [f"{filename}: Could not read — {e}"]

    for field in STORY_TOP_REQUIRED:
        if field not in story:
            errors.append(f"{filename}: Missing top-level field '{field}'")

    mechanics = story.get("supported_mechanics", {})
    if isinstance(mechanics, dict):
        for key in SUPPORTED_MECHANICS_KEYS:
            if key not in mechanics:
                errors.append(f"{filename}: supported_mechanics missing key '{key}'")
    else:
        errors.append(f"{filename}: 'supported_mechanics' must be an object")

    cast = story.get("cast", {})
    if isinstance(cast, dict):
        for key in ["commanders_used", "squad_members_used"]:
            if key not in cast:
                errors.append(f"{filename}: cast missing key '{key}'")
    else:
        errors.append(f"{filename}: 'cast' must be an object")

    nodes = story.get("nodes", [])
    if not isinstance(nodes, list):
        errors.append(f"{filename}: 'nodes' must be an array")
        return errors
    if len(nodes) == 0:
        errors.append(f"{filename}: 'nodes' array is empty")
        return errors

    node_ids = set()
    seen_ids = set()

    for i, node in enumerate(nodes):
        if not isinstance(node, dict):
            errors.append(f"{filename}: node[{i}] is not an object")
            continue

        node_id = node.get("node_id", f"<missing node_id at index {i}>")
        if node_id in seen_ids:
            errors.append(f"{filename}: Duplicate node_id '{node_id}'")
        seen_ids.add(node_id)
        node_ids.add(node_id)

        for field in NODE_REQUIRED:
            if field not in node:
                errors.append(f"{filename}: node '{node_id}' missing field '{field}'")

        node_type = node.get("node_type")
        if node_type not in VALID_NODE_TYPES:
            errors.append(
                f"{filename}: node '{node_id}' has invalid node_type '{node_type}' "
                f"(must be one of: {', '.join(sorted(VALID_NODE_TYPES))})"
            )

        chaos = node.get("chaos", {})
        if isinstance(chaos, dict):
            for key in CHAOS_KEYS:
                if key not in chaos:
                    errors.append(f"{filename}: node '{node_id}' chaos missing key '{key}'")
        else:
            errors.append(f"{filename}: node '{node_id}' chaos must be an object")

        dice_hook = node.get("dice_hook", {})
        if isinstance(dice_hook, dict):
            if "enabled" not in dice_hook:
                errors.append(f"{filename}: node '{node_id}' dice_hook missing key 'enabled'")
        else:
            errors.append(f"{filename}: node '{node_id}' dice_hook must be an object")

        cm = node.get("commander_moment", {})
        if isinstance(cm, dict):
            if "enabled" not in cm:
                errors.append(f"{filename}: node '{node_id}' commander_moment missing key 'enabled'")
        else:
            errors.append(f"{filename}: node '{node_id}' commander_moment must be an object")

        choices = node.get("choices", [])
        end_state = node.get("end_state")

        if node_type == "ending":
            if choices:
                errors.append(f"{filename}: ending node '{node_id}' should have empty choices array")
            if end_state is None:
                errors.append(f"{filename}: ending node '{node_id}' must have a non-null end_state")

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

    starting_node_id = story.get("starting_node_id")
    if starting_node_id and starting_node_id not in node_ids:
        errors.append(
            f"{filename}: starting_node_id '{starting_node_id}' "
            "does not match any node_id in 'nodes'"
        )

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

    storylines_dir = os.path.join(repo_root, "Creative", "WorldBuilding", "Storylines")
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
# Check 4 + 5: Pi skill wrapper integrity and routing contract parity
# ---------------------------------------------------------------------------

MAX_NAME_LENGTH = 64
MAX_DESCRIPTION_LENGTH = 1024
PI_SKILL_REFERENCE_RE = re.compile(r"`\.pi/skills/([^`]+?)/SKILL\.md`")
TABLE_HEADING_RE_TEMPLATE = r"^##\s+{}\s*$"
GENERATED_BLOCK_RE_TEMPLATE = r"<!-- GENERATED:{}:start -->\n(.*?)<!-- GENERATED:{}:end -->"
ALIAS_WRAPPER_HEADING = "# Migrated Compatibility Alias"


def parse_frontmatter(raw_text: str) -> tuple[dict[str, str], str | None]:
    if not raw_text.startswith("---\n"):
        return {}, "missing frontmatter block"

    match = re.match(r"^---\n(.*?)\n---(?:\n|$)", raw_text, re.S)
    if not match:
        return {}, "unterminated frontmatter block"

    frontmatter_text = match.group(1)
    frontmatter: dict[str, str] = {}
    for line in frontmatter_text.splitlines():
        if ":" not in line:
            continue
        key, value = line.split(":", 1)
        frontmatter[key.strip()] = value.strip()
    return frontmatter, None



def validate_pi_skill_name(name: str, parent_dir_name: str) -> list[str]:
    errors = []
    if name != parent_dir_name:
        errors.append(f'name "{name}" does not match parent directory "{parent_dir_name}"')
    if len(name) > MAX_NAME_LENGTH:
        errors.append(f"name exceeds {MAX_NAME_LENGTH} characters ({len(name)})")
    if not re.fullmatch(r"[a-z0-9-]+", name):
        errors.append("name contains invalid characters (must be lowercase a-z, 0-9, hyphens only)")
    if name.startswith("-") or name.endswith("-"):
        errors.append("name must not start or end with a hyphen")
    if "--" in name:
        errors.append("name must not contain consecutive hyphens")
    return errors



def iter_pi_skill_files(pi_skills_dir: Path) -> list[Path]:
    files: list[Path] = []
    for path in sorted(pi_skills_dir.glob("*.md")):
        files.append(path)
    for path in sorted(pi_skills_dir.rglob("SKILL.md")):
        files.append(path)
    return files



def extract_markdown_table_lines(raw_text: str, heading: str) -> list[str]:
    heading_re = re.compile(TABLE_HEADING_RE_TEMPLATE.format(re.escape(heading)), re.M)
    heading_match = heading_re.search(raw_text)
    if not heading_match:
        return []

    lines = raw_text[heading_match.end():].splitlines()
    table_lines: list[str] = []
    in_table = False

    for line in lines:
        if not in_table and not line.strip():
            continue
        if line.startswith("## "):
            break
        if line.strip().startswith("|"):
            table_lines.append(line.rstrip())
            in_table = True
            continue
        if in_table:
            break

    return table_lines



def extract_generated_block_table_lines(raw_text: str, block_name: str) -> list[str]:
    pattern = re.compile(
        GENERATED_BLOCK_RE_TEMPLATE.format(re.escape(block_name), re.escape(block_name)),
        re.S,
    )
    match = pattern.search(raw_text)
    if not match:
        return []
    return [line.rstrip() for line in match.group(1).splitlines() if line.strip()]



def file_mentions_wrapper(raw_text: str, wrapper_name: str) -> bool:
    return (
        f"`{wrapper_name}/SKILL.md`" in raw_text
        or f"`.pi/skills/{wrapper_name}/SKILL.md`" in raw_text
        or f"{wrapper_name}/SKILL.md" in raw_text
    )



def format_alias_targets(targets: list[str]) -> str:
    return " or ".join(f"`{target}`" for target in targets)



def load_json_file(path: Path) -> tuple[dict | None, str | None]:
    try:
        return json.loads(path.read_text(encoding="utf-8")), None
    except OSError as e:
        return None, str(e)
    except json.JSONDecodeError as e:
        return None, str(e)



def expected_role_table_from_manifest(roles: list[dict]) -> list[str]:
    return [
        "| Role | Folder | When to Activate |",
        "|---|---|---|",
        *[
            f"| `{role['name']}` | `{role['folder']}` | {role['when']} |"
            for role in roles
        ],
    ]



def aliases_dict_from_manifest(aliases: list[dict]) -> dict[str, list[str]]:
    return {alias["name"]: alias["canonical_targets"] for alias in aliases}



def expected_meta_table_from_manifest(helpers: list[dict]) -> list[str]:
    return [
        "| Wrapper | Purpose |",
        "|---|---|",
        *[
            f"| `{helper['name']}/SKILL.md` | {helper['purpose']} |"
            for helper in helpers
        ],
    ]



def expected_agents_quick_routing_table_from_manifest(quick_routing: list[dict]) -> list[str]:
    return [
        "| You're working on | Role | Agent Tree |",
        "|---|---|---|",
        *[
            f"| {row['work']} | `{row['role']}` | `{row['agent_tree']}` |"
            for row in quick_routing
        ],
    ]



def canonical_surfaces_from_manifest(canonical_subskills: list[dict], helpers: list[dict]) -> dict[str, list[str]]:
    result: dict[str, list[str]] = {}
    for item in canonical_subskills:
        result[item["name"]] = item["required_surfaces"]
    for item in helpers:
        if item["name"] == "meta":
            continue
        result[item["name"]] = item["required_surfaces"]
    return result



def declared_wrapper_names(roles: list[dict], canonical_subskills: list[dict], helpers: list[dict], aliases: list[dict]) -> set[str]:
    return {
        *(role["pi_wrapper"] for role in roles),
        *(item["name"] for item in canonical_subskills),
        *(item["name"] for item in helpers),
        *(item["name"] for item in aliases),
    }



def collect_repo_markdown_files(repo: Path) -> list[Path]:
    return sorted(
        path for path in repo.rglob("*.md")
        if ".git" not in path.parts
    )



def actual_wrapper_dirs(pi_skills_dir: Path) -> set[str]:
    return {
        path.name
        for path in pi_skills_dir.iterdir()
        if path.is_dir() and (path / "SKILL.md").is_file()
    }



def check_pi_skill_wrappers(repo_root: str) -> bool:
    header("Check 4 — Pi skill wrapper integrity")

    repo = Path(repo_root)
    pi_skills_dir = repo / ".pi" / "skills"
    roles_dir = repo / ".agents" / "roles"
    routing_manifest_path = repo / ".agents" / "routing-manifest.json"
    entry_path = repo / ".agents" / "ENTRY.md"
    pi_readme_path = pi_skills_dir / "README.md"

    if not pi_skills_dir.is_dir():
        fail(".pi/skills/ not found.")
        return False
    if not roles_dir.is_dir():
        fail(".agents/roles/ not found.")
        return False

    passed = True
    skill_files = iter_pi_skill_files(pi_skills_dir)

    for path in skill_files:
        rel_path = path.relative_to(repo).as_posix()
        raw_text = path.read_text(encoding="utf-8")
        frontmatter, frontmatter_error = parse_frontmatter(raw_text)

        if frontmatter_error:
            fail(f"[{rel_path}] {frontmatter_error}")
            passed = False
            continue

        parent_name = path.parent.name
        name = frontmatter.get("name", "")
        description = frontmatter.get("description", "")

        if not description:
            fail(f"[{rel_path}] description is required")
            passed = False
        elif len(description) > MAX_DESCRIPTION_LENGTH:
            fail(f"[{rel_path}] description exceeds {MAX_DESCRIPTION_LENGTH} characters ({len(description)})")
            passed = False

        name_errors = validate_pi_skill_name(name, parent_name)
        if name_errors:
            for error in name_errors:
                fail(f"[{rel_path}] {error}")
            passed = False

    routing_manifest, routing_error = load_json_file(routing_manifest_path)
    if routing_error:
        fail(f"[routing manifest] Could not load {routing_manifest_path.relative_to(repo).as_posix()}: {routing_error}")
        return False

    roles = routing_manifest.get("roles")
    canonical_subskills = routing_manifest.get("canonical_subskills")
    helpers = routing_manifest.get("helpers")
    aliases = routing_manifest.get("aliases")
    quick_routing = routing_manifest.get("quick_routing")

    for key, value in [
        ("roles", roles),
        ("canonical_subskills", canonical_subskills),
        ("helpers", helpers),
        ("aliases", aliases),
        ("quick_routing", quick_routing),
    ]:
        if not isinstance(value, list):
            fail(f"[routing manifest] '{key}' must be an array")
            passed = False

    if not passed:
        return False

    role_names = set()
    role_wrappers = set()
    for role in roles:
        for key in ["name", "folder", "when", "pi_wrapper", "canonical_children"]:
            if key not in role:
                fail(f"[routing manifest] role entry missing '{key}'")
                passed = False
        name = role.get("name", "")
        role_names.add(name)
        role_wrappers.add(role.get("pi_wrapper", ""))

        for error in validate_pi_skill_name(role.get("pi_wrapper", ""), role.get("pi_wrapper", "")):
            fail(f"[routing manifest] role wrapper '{role.get('pi_wrapper', '')}' is invalid: {error}")
            passed = False

        if role.get("pi_wrapper") != name:
            fail(f"[routing manifest] role '{name}' must use matching role wrapper '{name}'")
            passed = False

        expected_folder = f"roles/{name}/"
        if role.get("folder") != expected_folder:
            fail(f"[routing manifest] role '{name}' folder must be '{expected_folder}'")
            passed = False

        if not isinstance(role.get("canonical_children"), list):
            fail(f"[routing manifest] role '{name}' canonical_children must be an array")
            passed = False

    canonical_names = set()
    canonical_surfaces_expected = canonical_surfaces_from_manifest(canonical_subskills, helpers)
    for item in canonical_subskills:
        for key in ["name", "owner_role", "required_surfaces"]:
            if key not in item:
                fail(f"[routing manifest] canonical_subskills entry missing '{key}'")
                passed = False
        name = item.get("name", "")
        canonical_names.add(name)
        if item.get("owner_role") not in role_names:
            fail(f"[routing manifest] canonical wrapper '{name}' references unknown owner_role '{item.get('owner_role')}'")
            passed = False
        if not isinstance(item.get("required_surfaces"), list) or not item.get("required_surfaces"):
            fail(f"[routing manifest] canonical wrapper '{name}' must list required_surfaces")
            passed = False

    helper_names = set()
    for item in helpers:
        for key in ["name", "purpose", "required_surfaces", "canonical_children"]:
            if key not in item:
                fail(f"[routing manifest] helper entry missing '{key}'")
                passed = False
        name = item.get("name", "")
        helper_names.add(name)
        if not item.get("purpose"):
            fail(f"[routing manifest] helper '{name}' must define a non-empty purpose")
            passed = False
        if not isinstance(item.get("required_surfaces"), list) or not item.get("required_surfaces"):
            fail(f"[routing manifest] helper '{name}' must list required_surfaces")
            passed = False
        if not isinstance(item.get("canonical_children"), list):
            fail(f"[routing manifest] helper '{name}' canonical_children must be an array")
            passed = False

    alias_names = set()
    aliases_expected = aliases_dict_from_manifest(aliases)
    for item in aliases:
        for key in ["name", "canonical_targets"]:
            if key not in item:
                fail(f"[routing manifest] alias entry missing '{key}'")
                passed = False
        name = item.get("name", "")
        alias_names.add(name)
        if not isinstance(item.get("canonical_targets"), list) or not item.get("canonical_targets"):
            fail(f"[routing manifest] alias '{name}' must list canonical_targets")
            passed = False
        elif len(item.get("canonical_targets", [])) != len(set(item.get("canonical_targets", []))):
            fail(f"[routing manifest] alias '{name}' has duplicate canonical_targets")
            passed = False

    category_names = {
        "roles": [role.get("name", "") for role in roles],
        "canonical_subskills": [item.get("name", "") for item in canonical_subskills],
        "helpers": [item.get("name", "") for item in helpers],
        "aliases": [item.get("name", "") for item in aliases],
    }
    for category, names in category_names.items():
        duplicates = sorted({name for name in names if name and names.count(name) > 1})
        for name in duplicates:
            fail(f"[routing manifest] duplicate {category} entry: '{name}'")
            passed = False

    wrapper_names_by_category = {
        "role": [role.get("pi_wrapper", "") for role in roles],
        "canonical": [item.get("name", "") for item in canonical_subskills],
        "helper": [item.get("name", "") for item in helpers],
        "alias": [item.get("name", "") for item in aliases],
    }
    wrapper_occurrences: dict[str, list[str]] = {}
    for category, names in wrapper_names_by_category.items():
        for name in names:
            if not name:
                continue
            wrapper_occurrences.setdefault(name, []).append(category)
    for name, categories in sorted(wrapper_occurrences.items()):
        if len(categories) > 1:
            fail(f"[routing manifest] wrapper name collision for '{name}': {', '.join(categories)}")
            passed = False

    quick_routing_work_values = [item.get("work", "") for item in quick_routing]
    duplicate_quick_work = sorted({work for work in quick_routing_work_values if work and quick_routing_work_values.count(work) > 1})
    for work in duplicate_quick_work:
        fail(f"[routing manifest] duplicate quick_routing work label: '{work}'")
        passed = False

    for role in roles:
        owner = role.get("name", "")
        children = role.get("canonical_children", [])
        if len(children) != len(set(children)):
            fail(f"[routing manifest] role '{owner}' has duplicate canonical_children entries")
            passed = False
        for child in children:
            if child not in canonical_names:
                fail(f"[routing manifest] role '{owner}' references undeclared canonical child '{child}'")
                passed = False

    owner_children_map = {role.get("name", ""): set(role.get("canonical_children", [])) for role in roles}
    for item in canonical_subskills:
        owner = item.get("owner_role", "")
        name = item.get("name", "")
        if name not in owner_children_map.get(owner, set()):
            fail(f"[routing manifest] canonical wrapper '{name}' is not listed under role '{owner}' canonical_children")
            passed = False

    for helper in helpers:
        owner = helper.get("name", "")
        children = helper.get("canonical_children", [])
        if len(children) != len(set(children)):
            fail(f"[routing manifest] helper '{owner}' has duplicate canonical_children entries")
            passed = False
        for child in children:
            if child not in canonical_surfaces_expected:
                fail(f"[routing manifest] helper '{owner}' references undeclared canonical child '{child}'")
                passed = False

    quick_routing_role_values = [item.get("role", "") for item in quick_routing]
    duplicate_quick_roles = sorted({role for role in quick_routing_role_values if role and quick_routing_role_values.count(role) > 1})
    for role in duplicate_quick_roles:
        fail(f"[routing manifest] duplicate quick_routing role entry: '{role}'")
        passed = False

    for item in quick_routing:
        for key in ["work", "role", "agent_tree"]:
            if key not in item:
                fail(f"[routing manifest] quick_routing entry missing '{key}'")
                passed = False
        if item.get("role") not in role_names:
            fail(f"[routing manifest] quick_routing references unknown role '{item.get('role')}'")
            passed = False
        expected_tree = f".agents/roles/{item.get('role')}/"
        if item.get("agent_tree") != expected_tree:
            fail(
                f"[routing manifest] quick_routing role '{item.get('role')}' must use agent_tree '{expected_tree}'"
            )
            passed = False

    if not passed:
        return False

    for role_dir in sorted(roles_dir.iterdir()):
        if not role_dir.is_dir() or role_dir.name.startswith(".") or role_dir.name == "_template":
            continue
        if not (role_dir / "role.md").is_file():
            continue

        role_wrapper = pi_skills_dir / role_dir.name / "SKILL.md"
        if not role_wrapper.is_file():
            fail(
                f"[role parity] Missing Pi role wrapper for .agents role '{role_dir.name}': "
                f"expected {role_wrapper.relative_to(repo).as_posix()}"
            )
            passed = False
        else:
            ok(f"[role parity] Pi role wrapper exists for '{role_dir.name}'")

        if role_dir.name not in role_names:
            fail(f"[routing manifest] .agents role '{role_dir.name}' is missing from .agents/routing-manifest.json")
            passed = False

    for role_name in sorted(role_names):
        if not (roles_dir / role_name / "role.md").is_file():
            fail(f"[routing manifest] role '{role_name}' does not have .agents/roles/{role_name}/role.md")
            passed = False

    entry_roles_table = extract_markdown_table_lines(entry_path.read_text(encoding="utf-8"), "Roles")
    agents_md_path = repo / "AGENTS.md"
    agents_md_raw = agents_md_path.read_text(encoding="utf-8")
    agents_quick_routing_table = extract_generated_block_table_lines(agents_md_raw, "agents-quick-role-routing")
    pi_readme_raw = pi_readme_path.read_text(encoding="utf-8")
    pi_roles_table = extract_markdown_table_lines(pi_readme_raw, "Roles")
    pi_meta_table = extract_markdown_table_lines(pi_readme_raw, "Meta Wrappers")
    expected_roles_table = expected_role_table_from_manifest(roles)
    expected_agents_quick_routing_table = expected_agents_quick_routing_table_from_manifest(quick_routing)
    expected_meta_table = expected_meta_table_from_manifest(helpers)

    if not entry_roles_table:
        fail("[role table] Could not find '## Roles' table in .agents/ENTRY.md")
        passed = False
    elif entry_roles_table != expected_roles_table:
        fail("[role table] .agents/ENTRY.md Roles table differs from .agents/routing-manifest.json")
        for line in expected_roles_table:
            fail(f"  MANIFEST: {line}")
        for line in entry_roles_table:
            fail(f"  ENTRY:    {line}")
        passed = False
    else:
        ok("[role table] .agents/ENTRY.md matches .agents/routing-manifest.json")

    if not pi_roles_table:
        fail("[role table] Could not find '## Roles' table in .pi/skills/README.md")
        passed = False
    elif pi_roles_table != expected_roles_table:
        fail("[role table] .pi/skills/README.md Roles table differs from .agents/routing-manifest.json")
        for line in expected_roles_table:
            fail(f"  MANIFEST: {line}")
        for line in pi_roles_table:
            fail(f"  README:   {line}")
        passed = False
    else:
        ok("[role table] .pi/skills/README.md matches .agents/routing-manifest.json")

    if not agents_quick_routing_table:
        fail("[quick routing] Could not find generated quick-routing block in AGENTS.md")
        passed = False
    elif agents_quick_routing_table != expected_agents_quick_routing_table:
        fail("[quick routing] AGENTS.md quick-routing block differs from .agents/routing-manifest.json")
        for line in expected_agents_quick_routing_table:
            fail(f"  MANIFEST: {line}")
        for line in agents_quick_routing_table:
            fail(f"  AGENTS:   {line}")
        passed = False
    else:
        ok("[quick routing] AGENTS.md matches .agents/routing-manifest.json")

    if not pi_meta_table:
        fail("[meta table] Could not find '## Meta Wrappers' table in .pi/skills/README.md")
        passed = False
    elif pi_meta_table != expected_meta_table:
        fail("[meta table] .pi/skills/README.md Meta Wrappers table differs from .agents/routing-manifest.json")
        for line in expected_meta_table:
            fail(f"  MANIFEST: {line}")
        for line in pi_meta_table:
            fail(f"  README:   {line}")
        passed = False
    else:
        ok("[meta table] .pi/skills/README.md matches .agents/routing-manifest.json")

    ok("[routing manifest] alias inventory and canonical wrapper surface map loaded from .agents/routing-manifest.json")

    alias_table = extract_markdown_table_lines(pi_readme_raw, "Compatibility Aliases")
    expected_alias_table = [
        "| Alias | Canonical wrapper |",
        "|---|---|",
        *[
            f"| `{alias['name']}` | {format_alias_targets(alias['canonical_targets'])} |"
            for alias in aliases
        ],
    ]
    if not alias_table:
        fail("[alias inventory] Could not find '## Compatibility Aliases' table in .pi/skills/README.md")
        passed = False
    elif alias_table != expected_alias_table:
        fail("[alias inventory] .pi/skills/README.md Compatibility Aliases table differs from .agents/routing-manifest.json")
        passed = False
    else:
        ok("[alias inventory] README alias table matches .agents/routing-manifest.json")

    actual_wrappers = actual_wrapper_dirs(pi_skills_dir)
    declared_wrappers = declared_wrapper_names(roles, canonical_subskills, helpers, aliases)
    undeclared = sorted(actual_wrappers - declared_wrappers)
    missing_declared = sorted(declared_wrappers - actual_wrappers)
    if undeclared:
        for wrapper in undeclared:
            fail(f"[routing manifest] Pi wrapper exists but is not declared: .pi/skills/{wrapper}/SKILL.md")
        passed = False
    if missing_declared:
        for wrapper in missing_declared:
            fail(f"[routing manifest] Declared wrapper is missing: .pi/skills/{wrapper}/SKILL.md")
        passed = False
    if not undeclared and not missing_declared:
        ok("[routing manifest] all Pi wrappers are declared with no orphan wrapper directories")

    role_children_map = {role["name"]: set(role["canonical_children"]) for role in roles}
    helper_children_map = {helper["name"]: set(helper["canonical_children"]) for helper in helpers}

    for role_name, children in role_children_map.items():
        role_raw = (pi_skills_dir / role_name / "SKILL.md").read_text(encoding="utf-8")
        for child in sorted(children):
            if child not in canonical_names:
                fail(f"[routing manifest] role '{role_name}' references undeclared canonical child '{child}'")
                passed = False
                continue
            if not file_mentions_wrapper(role_raw, child):
                fail(f"[routing manifest] role wrapper '{role_name}' does not mention canonical child '{child}'")
                passed = False

    for helper_name, children in helper_children_map.items():
        helper_raw = (pi_skills_dir / helper_name / "SKILL.md").read_text(encoding="utf-8")
        for child in sorted(children):
            if child not in canonical_surfaces_expected:
                fail(f"[routing manifest] helper '{helper_name}' references undeclared canonical child '{child}'")
                passed = False
                continue
            if not file_mentions_wrapper(helper_raw, child):
                fail(f"[routing manifest] helper wrapper '{helper_name}' does not mention canonical child '{child}'")
                passed = False

    for alias_name, targets in aliases_expected.items():
        alias_raw = (pi_skills_dir / alias_name / "SKILL.md").read_text(encoding="utf-8")
        for error in validate_pi_skill_name(alias_name, alias_name):
            fail(f"[alias inventory] alias '{alias_name}' is invalid: {error}")
            passed = False
        if ALIAS_WRAPPER_HEADING not in alias_raw:
            fail(f"[alias inventory] alias wrapper '{alias_name}' must include '{ALIAS_WRAPPER_HEADING}'")
            passed = False
        for target in targets:
            if target in alias_names:
                fail(f"[alias inventory] alias '{alias_name}' must not point to another alias '{target}'")
                passed = False
            if target not in declared_wrappers:
                fail(f"[alias inventory] alias '{alias_name}' points to undeclared canonical target '{target}'")
                passed = False
            if not file_mentions_wrapper(alias_raw, target):
                fail(f"[alias inventory] alias wrapper '{alias_name}' does not mention canonical wrapper '{target}'")
                passed = False

    for wrapper_name, surface_files in canonical_surfaces_expected.items():
        for error in validate_pi_skill_name(wrapper_name, wrapper_name):
            fail(f"[canonical routing] wrapper '{wrapper_name}' is invalid: {error}")
            passed = False
        for surface_rel in surface_files:
            surface_path = repo / surface_rel
            if not surface_path.is_file():
                fail(f"[canonical routing] Surface file not found for '{wrapper_name}': {surface_rel}")
                passed = False
                continue
            surface_raw = surface_path.read_text(encoding="utf-8")
            if not file_mentions_wrapper(surface_raw, wrapper_name):
                fail(f"[canonical routing] Surface '{surface_rel}' does not mention canonical wrapper '{wrapper_name}'")
                passed = False

    for path in skill_files:
        rel_path = path.relative_to(repo).as_posix()
        raw_text = path.read_text(encoding="utf-8")
        for match in PI_SKILL_REFERENCE_RE.finditer(raw_text):
            target = match.group(1)
            if "<" in target or ">" in target or "*" in target:
                continue
            if "/" in target:
                fail(f"[{rel_path}] nested Pi wrapper reference is not allowed: .pi/skills/{target}/SKILL.md")
                passed = False
                continue
            target_path = pi_skills_dir / target / "SKILL.md"
            if not target_path.is_file():
                fail(f"[{rel_path}] referenced Pi wrapper not found: {target_path.relative_to(repo).as_posix()}")
                passed = False

    repo_markdown_files = collect_repo_markdown_files(repo)
    for path in repo_markdown_files:
        rel_path = path.relative_to(repo).as_posix()
        raw_text = path.read_text(encoding="utf-8")
        for match in PI_SKILL_REFERENCE_RE.finditer(raw_text):
            target = match.group(1)
            if "<" in target or ">" in target or "*" in target:
                continue
            if "/" in target:
                fail(f"[stale reference] {rel_path} uses nested Pi wrapper reference: .pi/skills/{target}/SKILL.md")
                passed = False
                continue
            if target not in declared_wrappers:
                fail(f"[stale reference] {rel_path} references undeclared Pi wrapper: .pi/skills/{target}/SKILL.md")
                passed = False

    if passed:
        ok("Pi wrapper names, routing manifest parity, and wrapper references are valid")

    return passed


# ---------------------------------------------------------------------------
# Entry point
# ---------------------------------------------------------------------------


def main() -> int:
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
    pi_wrappers_ok = check_pi_skill_wrappers(repo_root)

    all_passed = constants_ok and story_ok and pi_wrappers_ok

    print()
    if all_passed:
        print("\033[32m✓ All checks passed.\033[0m\n")
        return 0
    print("\033[31m✗ One or more checks failed. Fix the issues above before committing.\033[0m\n")
    return 1


if __name__ == "__main__":
    sys.exit(main())

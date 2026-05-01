#!/usr/bin/env python3
"""Validate the agent routing tree against .agents/manifest.json.

This is the Phase E acceptance gate for the agent-tree reflow. It is intentionally
strict: planned target paths are reported as failures until migrations create the
files or mark coverage explicitly.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Iterable
from urllib.parse import unquote, urlparse

try:
    import jsonschema  # type: ignore
except Exception:  # pragma: no cover - environment dependent
    jsonschema = None

ID_RE = re.compile(r"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$")
DOMAIN_ID_RE = re.compile(r"^(actions|apps|tools|creative|docs|root)-[a-z0-9]+(?:-[a-z0-9]+)*$")
MD_LINK_RE = re.compile(r"(?<!!)(?:\[[^\]]*\]|\[[^\]]*\]\[[^\]]*\])\(([^)\s]+)(?:\s+\"[^\"]*\")?\)")
REF_LINK_RE = re.compile(r"^\s*\[[^\]]+\]:\s+(\S+)", re.M)
BACKTICK_RE = re.compile(r"`([^`\n]+)`")
GENERATED_BLOCK_RE = re.compile(r"<!-- GENERATED:([^:]+):start -->\n(.*?)<!-- GENERATED:\1:end -->", re.S)
FRONTMATTER_RE = re.compile(r"\A---\n(.*?)\n---\n", re.S)
PATH_ROOTS = (
    ".agents/", "Actions/", "Apps/", "Tools/", "Creative/",
    "Projects/", "AGENTS.md", "CLAUDE.md", "README.md",
)
DOMAIN_ROOTS = ("Actions", "Apps", "Tools", "Creative")
DERIVED_BLOCKS = {
    "AGENTS.md": ["agents-quick-role-routing"],
}


@dataclass
class Failure:
    check: str
    message: str
    path: str | None = None
    line: int | None = None

    def format(self) -> str:
        loc = ""
        if self.path:
            loc = self.path
            if self.line:
                loc += f":{self.line}"
            loc += ": "
        return f"[{self.check}] {loc}{self.message}"


@dataclass
class CheckResult:
    name: str
    checked: int = 0
    failures: int = 0


def find_repo_root(start: Path) -> Path:
    current = start.resolve()
    for _ in range(12):
        if (current / "AGENTS.md").is_file() and (current / ".agents").is_dir():
            return current
        if current.parent == current:
            break
        current = current.parent
    raise RuntimeError("Could not find repo root containing AGENTS.md and .agents/.")


def rel(repo: Path, path: Path) -> str:
    try:
        return path.resolve().relative_to(repo.resolve()).as_posix()
    except ValueError:
        return path.as_posix()


def line_for_offset(text: str, offset: int) -> int:
    return text.count("\n", 0, offset) + 1


def load_json(path: Path) -> Any:
    return json.loads(path.read_text(encoding="utf-8"))


def add(failures: list[Failure], check: str, message: str, path: str | None = None, line: int | None = None) -> None:
    failures.append(Failure(check, message, path, line))


def path_exists_case_sensitive(repo: Path, candidate: str) -> bool:
    path = repo / candidate
    if not path.exists():
        return False
    current = repo
    for part in Path(candidate).parts:
        names = {child.name for child in current.iterdir()} if current.is_dir() else set()
        if part not in names:
            return False
        current = current / part
    return True


def normalize_segment(segment: str) -> str:
    # Project-approved historical names are normalized as whole words; do not
    # split acronym/brand compounds like LotAT or WorldBuilding unless the
    # naming findings explicitly call for it.
    preserved = {
        "LotAT": "lotat",
        "WorldBuilding": "worldbuilding",
    }
    if segment in preserved:
        return preserved[segment]
    spaced = re.sub(r"(?<=[a-z0-9])(?=[A-Z])", "-", segment)
    spaced = re.sub(r"[^A-Za-z0-9]+", "-", spaced)
    return spaced.strip("-").lower()


def expected_domain_id(domain_path: str) -> str | None:
    parts = [p for p in Path(domain_path).parts if p not in ("", ".")]
    if len(parts) < 2:
        return None
    root = parts[0].lower()
    if root not in {"actions", "apps", "tools", "creative", "docs"}:
        return None
    return root + "-" + "-".join(normalize_segment(p) for p in parts[1:])


def is_internal_target(target: str) -> bool:
    if target.startswith("#"):
        return False
    parsed = urlparse(target)
    if parsed.scheme in {"http", "https", "mailto", "tel", "ws", "wss"}:
        return False
    return True


def split_target(target: str) -> str:
    return unquote(target.split("#", 1)[0]).strip()


def looks_like_path(text: str) -> bool:
    if not text or "<" in text or ">" in text:
        return False
    if " " in text and not any(text.startswith(root) for root in ("Actions/", "Apps/", "Tools/", "Creative/", "Projects/")):
        return False
    if re.fullmatch(r"\.[A-Za-z0-9]+", text):
        return False
    if text.startswith(("http://", "https://", "mailto:")):
        return False
    if text.startswith(PATH_ROOTS):
        return True
    if text.startswith(("./", "../")):
        return True
    if "/" in text and not re.match(r"^[a-z][a-z0-9-]*$", text):
        return True
    return bool(re.search(r"\.(md|json|py|cs|ts|tsx|js|yml|yaml|txt)$", text))


def resolve_doc_target(repo: Path, source: Path, target: str) -> Path | None:
    target = split_target(target)
    if not target:
        return None
    if target.startswith("/") or re.match(r"^[A-Za-z]:", target):
        return None
    if target.startswith(PATH_ROOTS):
        return repo / target
    return (source.parent / target).resolve()


def render_table(lines: list[str]) -> str:
    return "\n".join(lines) + "\n"


def render_quick_routing(manifest: dict[str, Any]) -> str:
    skill_loc = {s["id"]: s["location"] for s in manifest.get("skills", [])}
    lines = ["| You're working on | Role | Agent Tree |", "|---|---|---|"]
    for row in manifest.get("quick_routing", []):
        route = row.get("route", row.get("role"))
        lines.append(f"| {row['work']} | `{row['role']}` | `{skill_loc.get(route, route)}` |")
    return render_table(lines)


def render_roles_table(manifest: dict[str, Any]) -> str:
    lines = ["| Role | Folder | When to Activate |", "|---|---|---|"]
    for skill in manifest.get("skills", []):
        loc = skill.get("location", "")
        if loc.startswith(".agents/roles/") and loc.endswith("/role.md"):
            lines.append(f"| `{skill['id']}` | `{str(Path(loc).parent).replace(os.sep, '/')}/` | {skill['description']} |")
    return render_table(lines)


def table_after_heading(text: str, heading: str) -> str | None:
    m = re.search(rf"^##\s+{re.escape(heading)}\s*$", text, re.M)
    if not m:
        return None
    rest = text[m.end():]
    lines = rest.splitlines()
    start = None
    for i, line in enumerate(lines):
        if line.strip().startswith("|"):
            start = i
            break
        if line.startswith("## "):
            return None
    if start is None:
        return None
    out = []
    for line in lines[start:]:
        if not line.strip().startswith("|"):
            break
        out.append(line)
    return "\n".join(out) + "\n"


def parse_frontmatter(text: str) -> dict[str, str]:
    m = FRONTMATTER_RE.match(text)
    if not m:
        return {}
    values: dict[str, str] = {}
    for line in m.group(1).splitlines():
        if ":" not in line or line.startswith(" "):
            continue
        key, value = line.split(":", 1)
        values[key.strip()] = value.strip().strip('"\'')
    return values


def collect_doc_refs(repo: Path, md_files: Iterable[Path], failures: list[Failure]) -> set[str]:
    referenced: set[str] = set()
    for path in md_files:
        text = path.read_text(encoding="utf-8", errors="replace")
        source_rel = rel(repo, path)
        for regex in (MD_LINK_RE, REF_LINK_RE):
            for m in regex.finditer(text):
                target = m.group(1)
                if not is_internal_target(target):
                    continue
                resolved = resolve_doc_target(repo, path, target)
                line = line_for_offset(text, m.start(1))
                if resolved is None:
                    add(failures, "link-integrity", f"absolute or invalid markdown link target `{target}`", source_rel, line)
                    continue
                if not resolved.exists():
                    add(failures, "link-integrity", f"broken markdown link `{target}` -> `{rel(repo, resolved)}`", source_rel, line)
                else:
                    referenced.add(rel(repo, resolved if resolved.is_file() else resolved))
        for m in BACKTICK_RE.finditer(text):
            target = m.group(1).strip()
            if not looks_like_path(target) or not is_internal_target(target):
                continue
            # Trim common punctuation from inline prose while preserving folder slash.
            target = target.rstrip(".,;:")
            resolved = resolve_doc_target(repo, path, target)
            line = line_for_offset(text, m.start(1))
            if resolved is None:
                add(failures, "link-integrity", f"absolute or invalid backtick path `{target}`", source_rel, line)
                continue
            if not resolved.exists():
                add(failures, "link-integrity", f"broken backtick path `{target}` -> `{rel(repo, resolved)}`", source_rel, line)
            else:
                referenced.add(rel(repo, resolved if resolved.is_file() else resolved))
    return referenced


def run(repo: Path, report_path: Path | None = None) -> tuple[list[CheckResult], list[Failure]]:
    failures: list[Failure] = []
    results: dict[str, CheckResult] = {name: CheckResult(name) for name in [
        "schema", "folder-coverage", "link-integrity", "drift", "stub-presence", "orphan", "naming"
    ]}

    manifest_path = repo / ".agents" / "manifest.json"
    schema_path = repo / ".agents" / "manifest.schema.json"
    manifest = load_json(manifest_path)
    schema = load_json(schema_path)

    # Schema check.
    results["schema"].checked += 1
    if jsonschema is None:
        add(failures, "schema", "python package `jsonschema` is not available; install it or use the project environment", rel(repo, manifest_path))
    else:
        try:
            jsonschema.Draft202012Validator.check_schema(schema)
            errors = sorted(jsonschema.Draft202012Validator(schema).iter_errors(manifest), key=lambda e: list(e.path))
            for err in errors:
                loc = "/".join(str(p) for p in err.path) or "<root>"
                add(failures, "schema", f"schema violation at {loc}: {err.message}", rel(repo, manifest_path))
        except Exception as exc:
            add(failures, "schema", f"schema validation crashed: {exc}", rel(repo, manifest_path))

    skills = manifest.get("skills", [])
    domains = manifest.get("domains", [])
    co_locations = manifest.get("co_locations", [])
    workflows = manifest.get("workflows", [])
    skill_ids = {s.get("id") for s in skills}
    domain_ids = {d.get("id") for d in domains}
    workflow_ids = {w.get("id") for w in workflows}
    declared_paths = {p for p in [
        *(s.get("location") for s in skills),
        *(c.get("path") for c in co_locations),
        *(w.get("path") for w in workflows),
        *(d.get("agentDoc") for d in domains),
    ] if p}

    # Folder coverage.
    for root_name in DOMAIN_ROOTS:
        root = repo / root_name
        if not root.exists():
            continue
        for child in sorted(p for p in root.iterdir() if p.is_dir()):
            results["folder-coverage"].checked += 1
            child_rel = rel(repo, child) + "/"
            match = [d for d in domains if d.get("path", "").rstrip("/") == child_rel.rstrip("/")]
            if not match:
                add(failures, "folder-coverage", "first-level domain folder has no manifest domain route", child_rel)
            else:
                d = match[0]
                if not d.get("agentDoc") and not d.get("coveredBy"):
                    add(failures, "folder-coverage", "domain route has neither agentDoc nor coveredBy", ".agents/manifest.json")
    for skill in skills:
        results["folder-coverage"].checked += 1
        loc = skill.get("location")
        if loc and not (repo / loc).exists():
            add(failures, "folder-coverage", f"skill `{skill.get('id')}` location does not exist: `{loc}`", loc)
    for coloc in co_locations:
        results["folder-coverage"].checked += 1
        path = coloc.get("path")
        if path and not (repo / path).exists():
            add(failures, "folder-coverage", f"declared co-location `{path}` does not exist", path)
    for domain in domains:
        results["folder-coverage"].checked += 1
        path = domain.get("path")
        if path and not (repo / path).exists():
            add(failures, "folder-coverage", f"domain path does not exist: `{path}`", path)

    # Link integrity.
    md_files = [p for p in (repo / ".agents").rglob("*.md")]
    for extra in (repo / "AGENTS.md", repo / "CLAUDE.md"):
        if extra.exists():
            md_files.append(extra)
    # Include declared co-located docs that already exist.
    for p in declared_paths:
        if str(p).endswith(".md") and (repo / p).exists():
            path = repo / p
            if path not in md_files:
                md_files.append(path)
    results["link-integrity"].checked = len(md_files)
    referenced = collect_doc_refs(repo, md_files, failures)

    # Drift detection.
    agents_path = repo / "AGENTS.md"
    if agents_path.exists():
        text = agents_path.read_text(encoding="utf-8")
        results["drift"].checked += 1
        blocks = {m.group(1): m.group(2) for m in GENERATED_BLOCK_RE.finditer(text)}
        expected = render_quick_routing(manifest)
        body = blocks.get("agents-quick-role-routing")
        if body is None:
            add(failures, "drift", "missing GENERATED block `agents-quick-role-routing`", "AGENTS.md")
        elif body != expected:
            add(failures, "drift", "GENERATED block `agents-quick-role-routing` is stale relative to .agents/manifest.json", "AGENTS.md")
    for doc, heading, expected in [
        (repo / ".agents" / "ENTRY.md", "Roles", render_roles_table(manifest)),
    ]:
        results["drift"].checked += 1
        if not doc.exists():
            add(failures, "drift", "derived routing doc is missing", rel(repo, doc))
            continue
        actual = table_after_heading(doc.read_text(encoding="utf-8"), heading)
        if actual is None:
            add(failures, "drift", f"could not find markdown table under `## {heading}`", rel(repo, doc))
        elif actual != expected:
            add(failures, "drift", f"`## {heading}` table is stale relative to .agents/manifest.json", rel(repo, doc))

    # Stub presence / frontmatter.
    required_fm = {"id", "type", "description", "status"}
    for skill in skills:
        results["stub-presence"].checked += 1
        loc = skill.get("location")
        sid = skill.get("id")
        if not loc:
            add(failures, "stub-presence", f"skill `{sid}` has no location", ".agents/manifest.json")
            continue
        path = repo / loc
        if not path.exists():
            add(failures, "stub-presence", f"entry file for skill `{sid}` is missing", loc)
            continue
        if path.suffix.lower() != ".md":
            continue
        fm = parse_frontmatter(path.read_text(encoding="utf-8", errors="replace"))
        missing = sorted(required_fm - set(fm))
        if missing:
            add(failures, "stub-presence", f"entry file for skill `{sid}` missing required frontmatter: {', '.join(missing)}", loc, 1)
        if fm.get("id") and fm.get("id") != sid:
            add(failures, "stub-presence", f"frontmatter id `{fm.get('id')}` does not match manifest skill id `{sid}`", loc, 1)

    # Orphan check.
    declared = {str(p) for p in declared_paths}
    referenced.update(declared)
    for path in sorted((repo / ".agents").rglob("*")):
        if not path.is_file():
            continue
        r = rel(repo, path)
        results["orphan"].checked += 1
        if r in {".agents/manifest.json", ".agents/manifest.schema.json"}:
            continue
        if r not in referenced:
            add(failures, "orphan", "file under .agents/ is not declared in manifest and has no inbound doc/path reference", r)

    # Naming check.
    for skill in skills:
        results["naming"].checked += 1
        sid = skill.get("id", "")
        loc = skill.get("location", "")
        if not ID_RE.fullmatch(sid):
            add(failures, "naming", f"skill id `{sid}` is not kebab-case", ".agents/manifest.json")
        if loc and not path_exists_case_sensitive(repo, loc) and (repo / loc).exists():
            add(failures, "naming", f"path casing does not match filesystem exactly: `{loc}`", loc)
        if loc.startswith(".agents/roles/"):
            parts = Path(loc).parts
            if len(parts) >= 4 and parts[2] != sid:
                add(failures, "naming", f"role skill `{sid}` location folder `{parts[2]}` does not match id", loc)
    for workflow in workflows:
        results["naming"].checked += 1
        wid = workflow.get("id", "")
        wpath = workflow.get("path", "")
        if not ID_RE.fullmatch(wid):
            add(failures, "naming", f"workflow id `{wid}` is not kebab-case", ".agents/manifest.json")
        if wpath != f".agents/workflows/{wid}.md":
            add(failures, "naming", f"workflow `{wid}` path should be `.agents/workflows/{wid}.md`, got `{wpath}`", ".agents/manifest.json")
    for domain in domains:
        results["naming"].checked += 1
        did = domain.get("id", "")
        dpath = domain.get("path", "")
        agent_doc = domain.get("agentDoc", "")
        if not DOMAIN_ID_RE.fullmatch(did):
            add(failures, "naming", f"domain route id `{did}` does not match domain route pattern", ".agents/manifest.json")
        expected = expected_domain_id(dpath)
        if expected and did != expected:
            add(failures, "naming", f"domain id `{did}` should normalize from path `{dpath}` as `{expected}`", ".agents/manifest.json")
        if agent_doc and Path(agent_doc).name != "AGENTS.md":
            add(failures, "naming", f"domain agent doc should be named AGENTS.md, got `{agent_doc}`", agent_doc)
    for alias in manifest.get("aliases", []):
        results["naming"].checked += 1
        aid = alias.get("id", "")
        target = alias.get("target")
        if not ID_RE.fullmatch(aid):
            add(failures, "naming", f"alias id `{aid}` is not kebab-case", ".agents/manifest.json")
        if target not in skill_ids and target not in domain_ids and target not in workflow_ids:
            add(failures, "naming", f"alias `{aid}` target `{target}` is not a known skill/domain/workflow id", ".agents/manifest.json")

    # Fill failure counts.
    for failure in failures:
        if failure.check in results:
            results[failure.check].failures += 1

    ordered = [results[name] for name in ["schema", "folder-coverage", "link-integrity", "drift", "stub-presence", "orphan", "naming"]]
    if report_path:
        report_path.parent.mkdir(parents=True, exist_ok=True)
        report_path.write_text("\n".join(f.format() for f in failures) + ("\n" if failures else ""), encoding="utf-8")
    return ordered, failures


def print_summary(results: list[CheckResult], failures: list[Failure], report_path: Path | None) -> None:
    print("Agent tree validation summary")
    print("| Check | Checked | Failures | Status |")
    print("|---|---:|---:|---|")
    for result in results:
        status = "PASS" if result.failures == 0 else "FAIL"
        print(f"| {result.name} | {result.checked} | {result.failures} | {status} |")
    print(f"\nTotal failures: {len(failures)}")
    if report_path:
        print(f"Failure report: {report_path.as_posix()}")


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Validate .agents/manifest.json and agent-tree routing docs.")
    parser.add_argument("--repo", type=Path, default=None, help="Repository root (auto-detected by default).")
    parser.add_argument("--report", type=Path, default=None, help="Optional path to write the full failure list.")
    args = parser.parse_args(argv)

    repo = args.repo.resolve() if args.repo else find_repo_root(Path.cwd())
    report = args.report
    if report and not report.is_absolute():
        report = repo / report

    results, failures = run(repo, report)
    print_summary(results, failures, report)
    for failure in failures:
        print(failure.format(), file=sys.stderr)
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(main())

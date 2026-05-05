#!/usr/bin/env python3
"""Validate the agent routing tree using AGENTS.md frontmatter."""

from __future__ import annotations

import argparse
import re
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Any, Iterable
from urllib.parse import unquote, urlparse

ID_RE = re.compile(r"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$")
DOMAIN_ID_RE = re.compile(r"^(actions|apps|tools|creative|docs|root)-[a-z0-9]+(?:-[a-z0-9]+)*$")
MD_LINK_RE = re.compile(r"(?<!!)(?:\[[^\]]*\]|\[[^\]]*\]\[[^\]]*\])\(([^)\s]+)(?:\s+\"[^\"]*\")?\)")
REF_LINK_RE = re.compile(r"^\s*\[[^\]]+\]:\s+(\S+)", re.M)
BACKTICK_RE = re.compile(r"`([^`\n]+)`")
FRONTMATTER_RE = re.compile(r"\A---\n(.*?)\n---\n", re.S)
PATH_ROOTS = (
    ".agents/", "Actions/", "Apps/", "Tools/", "Creative/",
    "Projects/", "AGENTS.md", "CLAUDE.md", "README.md",
)
DOMAIN_ROOTS = ("Actions", "Apps", "Tools", "Creative")


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


def parse_frontmatter(text: str) -> dict[str, Any]:
    m = FRONTMATTER_RE.match(text)
    if not m:
        return {}
    values: dict[str, Any] = {}
    current_key: str | None = None
    for line in m.group(1).splitlines():
        stripped = line.strip()
        if not stripped or stripped.startswith("#"):
            continue
        if line.startswith((" ", "\t")):
            if stripped.startswith("- ") and current_key:
                if not isinstance(values.get(current_key), list):
                    values[current_key] = []
                values[current_key].append(stripped[2:].strip().strip('"\''))
            continue
        if ":" not in line:
            current_key = None
            continue
        key, value = line.split(":", 1)
        current_key = key.strip()
        value = value.strip()
        if value == "[]":
            values[current_key] = []
        elif value:
            values[current_key] = value.strip('"\'')
        else:
            values[current_key] = []
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


def discover_agent_docs(repo: Path) -> tuple[list[Path], list[Path]]:
    agents_docs = sorted(p for p in repo.rglob("AGENTS.md") if ".git" not in p.parts)
    dot_agents_docs = sorted((repo / ".agents").rglob("*.md")) if (repo / ".agents").is_dir() else []
    return agents_docs, dot_agents_docs


def has_covering_agents_doc(repo: Path, domain_root: Path, folder: Path) -> bool:
    current = folder
    while True:
        if (current / "AGENTS.md").is_file():
            return True
        if current == domain_root or current == repo:
            break
        current = current.parent
    return False


def run(repo: Path, report_path: Path | None = None) -> tuple[list[CheckResult], list[Failure]]:
    failures: list[Failure] = []
    order = ["frontmatter", "folder-coverage", "link-integrity", "naming", "id-uniqueness"]
    results: dict[str, CheckResult] = {name: CheckResult(name) for name in order}

    agents_docs, dot_agents_docs = discover_agent_docs(repo)
    all_agent_docs = sorted(set(agents_docs + dot_agents_docs))
    frontmatter_by_path: dict[Path, dict[str, Any]] = {}

    # Frontmatter.
    required_fm = {"id", "type", "description", "status"}
    frontmatter_targets = sorted(set(agents_docs + [p for p in dot_agents_docs if p.match("**/roles/*/role.md")]))
    for path in frontmatter_targets:
        results["frontmatter"].checked += 1
        source_rel = rel(repo, path)
        fm = parse_frontmatter(path.read_text(encoding="utf-8", errors="replace"))
        frontmatter_by_path[path] = fm
        if not fm:
            add(failures, "frontmatter", "missing YAML frontmatter", source_rel, 1)
            continue
        missing = sorted(required_fm - set(fm))
        if path.match("**/.agents/roles/*/role.md") and "owner" not in fm:
            missing.append("owner")
        if missing:
            add(failures, "frontmatter", f"missing required frontmatter: {', '.join(missing)}", source_rel, 1)

    # Parse frontmatter for every discovered doc so naming can inspect ids without
    # broadening the frontmatter-required check beyond AGENTS.md and role overviews.
    for path in all_agent_docs:
        if path not in frontmatter_by_path:
            frontmatter_by_path[path] = parse_frontmatter(path.read_text(encoding="utf-8", errors="replace"))

    # Folder coverage.
    for root_name in DOMAIN_ROOTS:
        root = repo / root_name
        if not root.exists():
            continue
        for child in sorted(p for p in root.iterdir() if p.is_dir()):
            results["folder-coverage"].checked += 1
            child_rel = rel(repo, child) + "/"
            if not has_covering_agents_doc(repo, root, child):
                add(failures, "folder-coverage", "first-level domain folder has no AGENTS.md coverage", child_rel)

    # Link integrity.
    md_files = list(all_agent_docs)
    claude = repo / "CLAUDE.md"
    if claude.exists():
        md_files.append(claude)
    md_files = sorted(set(md_files))
    results["link-integrity"].checked = len(md_files)
    collect_doc_refs(repo, md_files, failures)

    # Naming.
    for path, fm in sorted(frontmatter_by_path.items(), key=lambda item: rel(repo, item[0])):
        fid = fm.get("id")
        if not fid:
            continue
        results["naming"].checked += 1
        source_rel = rel(repo, path)
        if not isinstance(fid, str) or not ID_RE.fullmatch(fid):
            add(failures, "naming", f"frontmatter id `{fid}` is not kebab-case", source_rel, 1)
        if path.name == "AGENTS.md" and any(root in path.parts for root in DOMAIN_ROOTS):
            domain_rel = rel(repo, path.parent) + "/"
            expected = expected_domain_id(domain_rel)
            if expected:
                if not DOMAIN_ID_RE.fullmatch(str(fid)):
                    add(failures, "naming", f"domain route id `{fid}` does not match domain route pattern", source_rel, 1)
                if fid != expected:
                    add(failures, "naming", f"domain id `{fid}` should normalize from path `{domain_rel}` as `{expected}`", source_rel, 1)
        if path.name == "AGENTS.md" and not path_exists_case_sensitive(repo, source_rel):
            add(failures, "naming", f"path casing does not match filesystem exactly: `{source_rel}`", source_rel)

    # ID uniqueness among AGENTS.md files.
    ids: dict[str, list[Path]] = {}
    for path in agents_docs:
        results["id-uniqueness"].checked += 1
        fid = frontmatter_by_path.get(path, {}).get("id")
        if isinstance(fid, str) and fid:
            ids.setdefault(fid, []).append(path)
    for fid, paths in sorted(ids.items()):
        if len(paths) < 2:
            continue
        locations = ", ".join(rel(repo, path) for path in paths)
        for path in paths:
            add(failures, "id-uniqueness", f"frontmatter id `{fid}` is duplicated by: {locations}", rel(repo, path), 1)

    # Fill failure counts.
    for failure in failures:
        if failure.check in results:
            results[failure.check].failures += 1

    ordered = [results[name] for name in order]
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
    parser = argparse.ArgumentParser(description="Validate the agent routing tree using AGENTS.md frontmatter.")
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

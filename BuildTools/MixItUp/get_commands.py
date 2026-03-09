#!/usr/bin/env python3
"""
Mix It Up command fetch utility.

Purpose:
- Run outside Streamer.bot as a local build/integration utility.
- Fetch the full command list from the Mix It Up Desktop API (with pagination).
- Save results to a text file for downstream tooling.

Usage:
  python3 BuildTools/MixItUp/get_commands.py
  python3 BuildTools/MixItUp/get_commands.py --url http://localhost:8911/api/v2/commands --out ./mixitup-commands.txt
  python3 BuildTools/MixItUp/get_commands.py --page-size 100
"""

from __future__ import annotations

import argparse
import json
import sys
import urllib.error
import urllib.parse
import urllib.request
from typing import Any

DEFAULT_URL = "http://localhost:8911/api/v2/commands"
DEFAULT_OUTPUT = "mixitup-commands.txt"
DEFAULT_PAGE_SIZE = 100


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Fetch Mix It Up commands via Desktop API")
    parser.add_argument("-u", "--url", default=DEFAULT_URL, help=f"Commands endpoint (default: {DEFAULT_URL})")
    parser.add_argument(
        "-o",
        "--out",
        default=DEFAULT_OUTPUT,
        help=f"Output text file path (default: {DEFAULT_OUTPUT})",
    )
    parser.add_argument(
        "--page-size",
        type=int,
        default=DEFAULT_PAGE_SIZE,
        help=f"Requested commands per page for pagination (default: {DEFAULT_PAGE_SIZE})",
    )
    return parser.parse_args()


def fetch_payload(url: str) -> Any:
    request = urllib.request.Request(url, headers={"Accept": "application/json"}, method="GET")
    with urllib.request.urlopen(request, timeout=15) as response:
        status = getattr(response, "status", 200)
        if status < 200 or status >= 300:
            raise RuntimeError(f"API error: HTTP {status}")

        content = response.read().decode("utf-8")
        return json.loads(content)


def with_query_params(url: str, params: dict[str, Any]) -> str:
    parsed = urllib.parse.urlparse(url)
    existing_query = dict(urllib.parse.parse_qsl(parsed.query, keep_blank_values=True))

    for key, value in params.items():
        existing_query[key] = str(value)

    return urllib.parse.urlunparse(
        parsed._replace(query=urllib.parse.urlencode(existing_query, doseq=True))
    )


def extract_commands(payload: Any) -> tuple[list[dict[str, Any]], str]:
    if isinstance(payload, list):
        return [item for item in payload if isinstance(item, dict)], "root(list)"

    if isinstance(payload, dict):
        candidate_keys = ["commands", "Commands", "data", "Data", "items", "Items", "results", "Results"]

        for key in candidate_keys:
            value = payload.get(key)
            if isinstance(value, list):
                return [item for item in value if isinstance(item, dict)], f"root['{key}']"

        for outer_key, outer_value in payload.items():
            if not isinstance(outer_value, dict):
                continue

            for inner_key in candidate_keys:
                value = outer_value.get(inner_key)
                if isinstance(value, list):
                    return [item for item in value if isinstance(item, dict)], f"root['{outer_key}']['{inner_key}']"

        if "id" in payload and "name" in payload:
            return [payload], "root(single-command-object)"

    return [], "not-found"


def extract_total_count(payload: Any) -> int | None:
    if isinstance(payload, dict):
        for key in ("TotalCount", "totalCount", "total", "Total", "count", "Count"):
            value = payload.get(key)
            if isinstance(value, int):
                return value
    return None


def fetch_all_commands(base_url: str, page_size: int) -> tuple[list[dict[str, Any]], dict[str, Any]]:
    if page_size <= 0:
        raise ValueError("page_size must be > 0")

    all_commands: list[dict[str, Any]] = []
    skip = 0
    pages_fetched = 0
    first_page_source = "unknown"
    first_page_keys: list[str] = []
    reported_total_count: int | None = None

    while True:
        page_url = with_query_params(base_url, {"skip": skip, "pageSize": page_size})
        payload = fetch_payload(page_url)

        commands, source = extract_commands(payload)
        if pages_fetched == 0:
            first_page_source = source
            if isinstance(payload, dict):
                first_page_keys = list(payload.keys())

        total_count = extract_total_count(payload)
        if total_count is not None:
            reported_total_count = total_count

        fetched_this_page = len(commands)
        all_commands.extend(commands)
        pages_fetched += 1

        if fetched_this_page == 0:
            break

        skip += fetched_this_page

        if reported_total_count is not None and len(all_commands) >= reported_total_count:
            break

        if fetched_this_page < page_size and reported_total_count is None:
            break

        if pages_fetched >= 1000:
            raise RuntimeError("Pagination safety limit reached (1000 pages)")

    metadata = {
        "pagesFetched": pages_fetched,
        "requestedPageSize": page_size,
        "reportedTotalCount": reported_total_count,
        "firstPageCommandSource": first_page_source,
        "firstPageTopLevelKeys": first_page_keys,
    }
    return all_commands, metadata


def build_output(commands: list[dict[str, Any]], metadata: dict[str, Any]) -> str:
    lines: list[str] = []

    lines.append("Mix It Up Commands Export")
    lines.append(f"Command count: {len(commands)}")
    lines.append(f"Pages fetched: {metadata.get('pagesFetched')}")
    lines.append(f"Requested page size: {metadata.get('requestedPageSize')}")
    lines.append(f"Reported total count: {metadata.get('reportedTotalCount')}")
    lines.append(f"First page command source: {metadata.get('firstPageCommandSource')}")

    top_level_keys = metadata.get("firstPageTopLevelKeys") or []
    if top_level_keys:
        lines.append(f"First page top-level keys: {', '.join(top_level_keys)}")

    lines.append("")
    lines.append("Command Summary:")

    if commands:
        for index, command in enumerate(commands, start=1):
            command_id = command.get("id")
            name = command.get("name")
            command_type = command.get("commandType", command.get("type"))
            enabled = command.get("enabled")
            lines.append(f"{index}. id={command_id} | name={name} | type={command_type} | enabled={enabled}")
    else:
        lines.append("(No command objects detected in payload.)")

    lines.append("")
    lines.append("Full JSON Payload (All Commands):")
    lines.append(json.dumps(commands, indent=2))
    lines.append("")

    return "\n".join(lines)


def main() -> int:
    args = parse_args()

    try:
        commands, metadata = fetch_all_commands(args.url, args.page_size)
    except urllib.error.URLError as error:
        print(f"[mixitup] Request failed: {error}", file=sys.stderr)
        print("[mixitup] Make sure Mix It Up is running and Developer API is enabled.", file=sys.stderr)
        return 1
    except json.JSONDecodeError as error:
        print(f"[mixitup] Invalid JSON response: {error}", file=sys.stderr)
        return 1
    except Exception as error:
        print(f"[mixitup] Unexpected failure: {error}", file=sys.stderr)
        return 1

    output_text = build_output(commands, metadata)

    with open(args.out, "w", encoding="utf-8") as f:
        f.write(output_text)

    print(f"[mixitup] Retrieved {len(commands)} command(s) from {args.url}")
    print(
        f"[mixitup] Pagination: pages={metadata.get('pagesFetched')} "
        f"requestedPageSize={metadata.get('requestedPageSize')} "
        f"reportedTotalCount={metadata.get('reportedTotalCount')}"
    )
    print(f"[mixitup] Saved results to {args.out}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())

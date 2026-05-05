Read every AGENTS.md file under Actions/ (including subfolders). Do not change anything.

Produce a report with two parts:

PART 1 — Line counts
List every AGENTS.md path and its line count, sorted descending. Include a total.

PART 2 — Contract coverage
For each AGENTS.md, note:
- Does it contain the text `ACTION-CONTRACTS:START`? (yes/no)
- How many .cs files are in the same folder (not subfolders)?

Then run:
  python3 Tools/StreamerBot/Validation/action_contracts.py --all

Report the full output. If it fails, list each failure. These are pre-existing issues — do not fix them in this step.

Output the report to the terminal. No files created or modified.

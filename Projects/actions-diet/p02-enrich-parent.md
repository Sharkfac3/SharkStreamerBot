Edit `Actions/AGENTS.md`. Add the following content. Do not remove or rewrite anything already there — only add.

This step is required before any other AGENTS.md file is slimmed. Later prompts will drop boilerplate from child files on the assumption that it lives here.

---

ADD a "Shared Ownership Rules" section (insert after any existing ownership section, or before "Folder Routing" if none exists):

```
## Shared Ownership Rules

`streamerbot-dev` owns all C# runtime behavior under `Actions/` by default.

`brand-steward` review is required before any change to: public chat output, TTS/spoken text,
overlay copy, command names visible to chat, or event announcement wording — across all folders.
Local AGENTS.md files only list secondary owners when the folder adds exceptions to this rule.

`ops` handles validation, paste/sync workflow, and agent-tree maintenance across all folders.
```

---

ADD a "Universal Script Rules" section (insert into or append to the existing "Domain Rules" section — do not duplicate rules already stated there):

```
## Universal Script Rules

These apply to every .cs script under Actions/ and are not restated in local guides:

- Scripts must remain pasteable into Streamer.bot `Execute C# Code` actions.
- Scripts are self-contained: do not assume shared runtime files can be imported at runtime.
- Read runtime state defensively via `CPH.TryGetArg` or Streamer.bot globals; protect against missing or malformed inputs.
- Be explicit about persisted vs. non-persisted globals when using `CPH.SetGlobalVar`.
```

---

ADD a "Shared Required Reading" section (insert into or append to the existing "Start Here" section):

```
## Shared Required Reading

Every session working under Actions/ must read:
- `Actions/SHARED-CONSTANTS.md` — canonical global var, timer, OBS source, and broker topic names
- `Actions/Helpers/AGENTS.md` — reusable Streamer.bot C# patterns

Local AGENTS.md files list additional required reading specific to that folder only.
```

---

ADD a "Validation and Handoff" section (append near the end of the file):

```
## Validation and Handoff

After editing any Actions/**/*.cs file:
1. Run `python Tools/StreamerBot/Validation/action_contracts.py --changed` to check contract alignment.
2. Include Streamer.bot paste targets in your handoff.
3. Note smoke-test steps for the changed action.

After editing an ACTION-CONTRACTS block in any AGENTS.md:
1. Run `python Tools/StreamerBot/Validation/action_contracts.py --stamp` to refresh SHA256 stamps in .cs files.
2. Run `--all` to confirm clean state.
```

---

After editing, confirm the file still reads coherently as the Actions/ domain entrypoint.

# Ollama Setup on Windows 11 for WSL-Based Workflows

This guide explains how to set up **Ollama on native Windows 11** while still doing repo and tool work from **WSL**.

The goal is:
- Ollama runs on Windows, not inside WSL
- WSL-based tools can still call the Ollama HTTP API
- the machine is ready for local model pulls and inference

---

## What this setup is for

This repo uses WSL for development, but in this setup we want Ollama to run as a normal Windows application.

That means:
- install Ollama in Windows
- start and manage Ollama from Windows
- access it from WSL over `http://localhost:11434`

For this project, that usually means WSL tools should point at:

```text
http://localhost:11434/api/generate
```

---

## What you need before you start

You should have:
- Windows 11
- WSL already installed
- internet access for downloading Ollama and pulling models
- enough disk space for local models
- a terminal available in both Windows and WSL

Optional but recommended:
- a supported NVIDIA GPU if you want better local inference performance
- up-to-date GPU drivers on Windows

---

## 1. Install Ollama on Windows 11

### Option A: install from the Ollama website

Open Windows in your normal browser and download the Windows installer from:

```text
https://ollama.com/download/windows
```

Run the installer and complete the setup.

### Option B: install with `winget`

Open **Windows PowerShell** or **Windows Terminal** on the Windows side and run:

```powershell
winget install Ollama.Ollama
```

If `winget` is not available, use the website installer instead.

---

## 2. Start Ollama on Windows

After install, launch Ollama from the Windows Start menu.

You can also start it from Windows PowerShell:

```powershell
ollama serve
```

Important:
- run this from **Windows**, not WSL
- leave it running while WSL tools need Ollama

In many Windows installs, Ollama also adds a tray icon or background app behavior. If it is already running, you may not need to start `ollama serve` manually every time.

---

## 3. Verify Ollama works on the Windows side

In **Windows PowerShell**, run:

```powershell
ollama --version
```

Then check that the local API responds:

```powershell
Invoke-RestMethod http://localhost:11434/api/tags
```

If Ollama is running correctly, you should get a JSON response.

---

## 4. Pull at least one model on Windows

From **Windows PowerShell**, pull the model you want to use.

Example for this repo's current Content Pipeline default:

```powershell
ollama pull llama3.1:8b
```

You can list installed models with:

```powershell
ollama list
```

---

## 5. Verify WSL can reach the Windows Ollama server

Now switch to your **WSL terminal** and test the Windows-hosted API.

Run:

```bash
curl http://localhost:11434/api/tags
```

If that works, WSL can talk to Ollama running on Windows.

This is the preferred setup for this machine.

---

## 6. Configure repo tools to use the Windows-hosted Ollama instance

For this repo, WSL-based tools should generally use:

```text
http://localhost:11434/api/generate
```

For the Content Pipeline, that means `Tools/ContentPipeline/.env` should use:

```text
CONTENT_PIPELINE_OLLAMA_URL=http://localhost:11434/api/generate
CONTENT_PIPELINE_OLLAMA_MODEL=llama3.1:8b
```

If those values are already present, you do not need to change anything.

---

## 7. Quick end-to-end test from WSL

From WSL, run:

```bash
curl http://localhost:11434/api/tags
```

You should see a response that includes the model you pulled.

If you want to confirm the exact model is available, you can also check from Windows:

```powershell
ollama list
```

---

## Recommended day-to-day workflow

### Start of day

1. Boot Windows
2. Start Ollama on Windows if it is not already running
3. Open WSL
4. Verify WSL can reach Ollama:

```bash
curl http://localhost:11434/api/tags
```

5. Run your WSL-based repo tools

### If a tool says Ollama is unavailable

First check on Windows:

```powershell
ollama list
```

Then check from WSL:

```bash
curl http://localhost:11434/api/tags
```

---

## Troubleshooting

### Problem: `ollama` command is not found in Windows

Possible causes:
- Ollama was not installed successfully
- the terminal was open before the install finished
- Windows PATH has not refreshed yet

Try:
- close and reopen Windows Terminal or PowerShell
- run the installer again
- confirm the app exists in the Start menu

---

### Problem: WSL cannot reach `http://localhost:11434`

First confirm Ollama is actually running on Windows.

In Windows PowerShell:

```powershell
Invoke-RestMethod http://localhost:11434/api/tags
```

If Windows can reach it but WSL cannot:
- restart WSL
- restart Ollama on Windows
- try again from WSL

You can restart WSL from Windows PowerShell with:

```powershell
wsl --shutdown
```

Then reopen your WSL terminal and test again:

```bash
curl http://localhost:11434/api/tags
```

---

### Problem: the model is missing

Pull it from Windows:

```powershell
ollama pull llama3.1:8b
```

Then verify:

```powershell
ollama list
```

---

### Problem: Ollama is running, but inference is slow

Possible causes:
- the model is too large for the machine
- GPU acceleration is not available
- other tools are already using system memory or VRAM

Try:
- using a smaller model
- closing other heavy applications
- confirming Windows GPU drivers are current

---

### Problem: a repo tool still points at a WSL-hosted Ollama install

Search your config for old values such as:
- a non-localhost Ollama URL
- WSL-only launch assumptions
- instructions that say to run `ollama serve` inside WSL

For this Windows-native setup, the intended target is:

```text
http://localhost:11434/api/generate
```

with Ollama itself running on Windows.

---

## Quick start summary

### On Windows

```powershell
winget install Ollama.Ollama
ollama serve
ollama pull llama3.1:8b
ollama list
```

### In WSL

```bash
curl http://localhost:11434/api/tags
```

### Repo config target

```text
CONTENT_PIPELINE_OLLAMA_URL=http://localhost:11434/api/generate
CONTENT_PIPELINE_OLLAMA_MODEL=llama3.1:8b
```

---

## Expected result

If everything is working, you should be able to:
- run Ollama natively on Windows 11
- pull models from Windows
- access the Ollama API from WSL with `localhost:11434`
- run WSL-based repo tools against the Windows-hosted Ollama instance

That means the machine is set up correctly for a Windows-native Ollama + WSL development workflow.

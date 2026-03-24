---
name: content-repurposer-pipeline
description: Content-pipeline tooling for `Tools/ContentPipeline/` — transcription, highlight detection, clip extraction, formatting, review queue, and feedback tooling.
---

# Pipeline Tooling — content-repurposer

Full context: `.agents/roles/content-repurposer/skills/pipeline/_index.md`

## Chain

```
content-repurposer-pipeline → content-repurposer → ops-change-summary
```

Load `.agents/roles/content-repurposer/skills/core.md` first when the task also needs clip criteria, caption rules, or brand/output context.

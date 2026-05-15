# DrugstoreSystem — Documentation Index

This folder is the **executable design** of DrugstoreSystem. Every document here answers a specific question. Read the relevant doc before reading or writing code.

The top-level charter is at **[../CLAUDE.md](../CLAUDE.md)**.
Skill files (implementation handbooks) are at **[../.claude/skills/](../.claude/skills/)**.

---

## When to read what

| If you want to… | Read |
|---|---|
| Understand the whole project, its constraints, and rules | [../CLAUDE.md](../CLAUDE.md) |
| Know the system architecture and component boundaries | [architecture.md](architecture.md) |
| Know the exact database schema, indexes, and relationships | [database-schema.md](database-schema.md) |
| Know what service interfaces and DTOs exist | [api-contracts.md](api-contracts.md) |
| Know what Blazor pages exist and how they behave | [ui-pages.md](ui-pages.md) |
| Understand the multi-stage medicine search algorithm | [search-algorithm.md](search-algorithm.md) |
| Understand the Haversine pharmacy-ranking algorithm | [optimization-algorithm.md](optimization-algorithm.md) |
| Plan work, find the current sprint, or mark a sprint done | [sprints.md](sprints.md) |
| Write code in this project's style | [coding-standards.md](coding-standards.md) |
| Configure the app or manage secrets | [security-and-config.md](security-and-config.md) |
| Know what seed data looks like and how to extend it | [demo-data.md](demo-data.md) |

---

## Skill files

Skill files encode deep implementation knowledge and are consulted sprint-by-sprint.

| Skill | When to open |
|---|---|
| [dotnet-clean-architecture](../.claude/skills/dotnet-clean-architecture/SKILL.md) | Before adding any new class — deciding which project it belongs in |
| [blazor-mudblazor-patterns](../.claude/skills/blazor-mudblazor-patterns/SKILL.md) | Before writing any `.razor` file |
| [pharmacy-domain](../.claude/skills/pharmacy-domain/SKILL.md) | Before touching entities, medicines, or pharmacy data |
| [search-algorithm](../.claude/skills/search-algorithm/SKILL.md) | Before implementing or modifying the search service |
| [optimization-algorithm](../.claude/skills/optimization-algorithm/SKILL.md) | Before implementing or modifying Haversine ranking |
| [git-github-workflow](../.claude/skills/git-github-workflow/SKILL.md) | Before any git operation |
| [thesis-writing](../.claude/skills/thesis-writing/SKILL.md) | Before writing any Uzbek thesis chapter |
| [thesis-structure](../.claude/skills/thesis-structure/SKILL.md) | Before drafting any thesis chapter — get the outline first |
| [thesis-screenshots](../.claude/skills/thesis-screenshots/SKILL.md) | When figuring out which screenshots to capture |

---

## Document states

- **Stable** — appended to only; historical statements stay. Examples: `architecture.md`, `search-algorithm.md`, `optimization-algorithm.md`, `coding-standards.md`
- **Living** — actively edited as work progresses. Examples: `sprints.md` (status updated after every sprint)

All doc changes are committed with the `docs(<scope>):` conventional-commit prefix.

---

## Rule: docs and code stay in sync

From [CLAUDE.md §10](../CLAUDE.md):

> **No code without docs.** If it is not in `docs/`, do not build it.

This means:
1. **Before writing code** — the relevant doc must exist and be correct.
2. **After writing code** — if implementation diverged from the doc, update the doc in the same commit.

---

## Reading order for a fresh session

1. `../CLAUDE.md` — scope, rules, algorithms overview
2. `sprints.md` — where things stand, what is next
3. `architecture.md` — how the code is structured
4. `database-schema.md` — what the data looks like
5. `search-algorithm.md` + `optimization-algorithm.md` — the two core algorithms
6. `api-contracts.md` + `ui-pages.md` — the surface area
7. `coding-standards.md`, `security-and-config.md`, `demo-data.md` — reference

---

## Thesis output (later)

Uzbek thesis chapters will be written under `docs/thesis/` as Markdown files during DOC sprints, then assembled into `.docx` via Pandoc. Screenshots and diagrams go under `docs/thesis/images/`. This is intentionally deferred until DEV-10 is complete.

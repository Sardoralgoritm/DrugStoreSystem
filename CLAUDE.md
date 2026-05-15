# CLAUDE.md — DrugstoreSystem Project Charter

> **Purpose of this file:** Single source of truth for every Claude Code session. Read this first, every time, no exceptions.

---

## 1. Project Identity

| Field | Value |
|---|---|
| **Project name** | DrugstoreSystem |
| **Full title (UZ)** | Dorixonalardan dori buyurtma berishning eng yaqin yechimini topib beruvchi dasturiy vositasini ishlab chiqish |
| **Full title (RU)** | Разработка программного инструмента, который находит наиболее оптимальное решение для заказа лекарств в аптеках |
| **Full title (EN)** | Developing a software tool that finds the closest solution for ordering medicines from pharmacies |
| **Type** | University graduation thesis (Bitiruv Malakaviy Ishi — BMI) |
| **Institution** | Alfraganus universiteti |
| **Department** | Raqamli texnologiyalari kafedrasi |
| **Supervisor** | Hamroev Alisher Shodmonqulovich, dotsent |
| **Student** | Sharipova Moxichexra Oltinovna |
| **Group** | M313-22 Dlo' |
| **Code + docs language** | English |
| **UI language** | Uzbek |
| **Thesis language** | Uzbek |

---

## 2. Scope

A multi-pharmacy web platform. Registered pharmacies manage their own medicine inventory. Guest users (no login required) search for any medicine and receive a ranked list of pharmacies that currently have it in stock — sorted by proximity or price.

**The two algorithms are the academic core of this thesis:**
1. **Multi-stage fuzzy search** — finds the correct medicine even with typos, Russian names, synonyms, or partial input
2. **Haversine optimization** — ranks pharmacies by geodesic distance or ascending price

### 2.1 Core features

1. **Admin panel** — register and control pharmacies on the platform; no involvement in medicine data
2. **Pharmacist panel** — manage own pharmacy profile and medicine inventory via shared-catalog autocomplete
3. **Shared medicine catalog** — crowdsourced: pharmacists collectively build it via autocomplete-then-create flow (if medicine exists → select it; if not → create it as a new shared entry)
4. **Public medicine search** — guest users search by name, synonym, or Russian name; results show only available pharmacies (`quantity > 0`)
5. **Proximity / price ranking** — Haversine-based distance ranking (default) or ascending price sort; user switches mode freely
6. **Overridable location** — defaults to browser geolocation; user can type any city/address to search on behalf of someone else
7. **Medicine & pharmacy detail pages** — full info; pharmacy page includes a Google Maps redirect link (not embedded map)

### 2.2 Out of scope (hard NO for v1)

- Online ordering or purchasing of any kind
- Payment processing
- SMS / email notifications
- Prescription management or controlled-substance tracking
- Delivery tracking
- User reviews or ratings
- Mobile app (web only)
- Embedded interactive map (Google Maps redirect link is sufficient)
- AI / ML features (deterministic algorithms only)
- Multi-language UI (Uzbek only)
- Multi-tenant admin (single admin account)

### 2.3 Why this scope

Thesis evaluation criteria: (a) reliable demo during defense, (b) meets department formatting standards, (c) student can explain every line. The search + Haversine algorithms are the academic contribution — everything else is scaffolding that supports them.

---

## 3. Tech Stack (final — do not change without discussion)

| Layer | Technology |
|---|---|
| Runtime | .NET 10.0 |
| UI framework | Blazor Server |
| Component library | MudBlazor |
| Backend framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 |
| Database | PostgreSQL 16+ |
| Search extension | `pg_trgm` (trigram similarity) |
| Authentication | ASP.NET Core Identity (cookie-based) |
| Validation | FluentValidation |
| Logging | Serilog (console + rolling file under `logs/`) |
| Secrets | `dotnet user-secrets` (dev); environment variables (demo) |
| IDE | Visual Studio 2022 / Rider |

### Architecture

**Clean Architecture, simplified monolith** — four projects in one solution:

```
DrugstoreSystem.sln
├── src/
│   ├── DrugstoreSystem.Domain          — entities, enums, domain exceptions (zero dependencies)
│   ├── DrugstoreSystem.Application     — service interfaces, DTOs, validators, pure logic (Haversine lives here)
│   ├── DrugstoreSystem.Infrastructure  — EF Core, repositories, Identity, pg_trgm raw queries
│   └── DrugstoreSystem.Web             — Blazor Server pages, DI composition root
└── tests/
    └── DrugstoreSystem.UnitTests       — Haversine formula + search ranking tests only
```

Dependency direction: `Web → Infrastructure → Application → Domain`. Domain has zero outward dependencies.

See [docs/architecture.md](docs/architecture.md) for full details.

---

## 4. Key Algorithms

### 4.1 Multi-Stage Search Algorithm

**Spec:** [docs/search-algorithm.md](docs/search-algorithm.md)
**Skill:** [.claude/skills/search-algorithm/SKILL.md](.claude/skills/search-algorithm/SKILL.md)

Five ranked stages applied in a single SQL query:
1. Exact match on `name` / `generic_name` / `name_ru` → score 1.0
2. ILIKE contains on all name fields → score 0.7
3. ILIKE contains on `MedicineSynonym.synonym` → score 0.65
4. `pg_trgm similarity() > 0.3` on `name` → score = similarity value
5. Deduplicate, sort by score DESC, then filter to pharmacies with `quantity > 0`

### 4.2 Haversine Optimization Algorithm

**Spec:** [docs/optimization-algorithm.md](docs/optimization-algorithm.md)
**Skill:** [.claude/skills/optimization-algorithm/SKILL.md](.claude/skills/optimization-algorithm/SKILL.md)

After the search returns matching medicines and their stocked pharmacies:
- **Distance mode** (default): rank by `Haversine(userLat, userLng, pharmacyLat, pharmacyLng)` ASC
- **Price mode**: rank by `price` ASC
- Availability (`quantity > 0` AND `pharmacy.is_active = true`) always pre-filtered
- Google Maps URL: `https://www.google.com/maps/search/?api=1&query={lat},{lng}`

---

## 5. Roles and Access

| Role | Login | Capabilities |
|---|---|---|
| **Admin** | Yes (single seeded account) | Pharmacy CRUD; activate / deactivate; dashboard stats |
| **Pharmacist** | Yes (one account per pharmacy) | Edit own pharmacy profile; manage medicine inventory |
| **Guest** | No — fully public | Search medicines; view medicine detail; view pharmacy detail |

---

## 6. Working Agreement

### 6.1 Plan-first discipline
No code is written until the relevant design doc in `docs/` exists and is current. If a feature is ambiguous, propose options with trade-offs — do not guess.

### 6.2 Sprint execution
- Work is organized by sprint: [docs/sprints.md](docs/sprints.md)
- On session start: read `CLAUDE.md` → check `docs/sprints.md` → check `git log` → ask the user which sprint to continue. Never assume.
- Complete sprints in order. Skipping is only allowed if the user explicitly requests it and the dependency analysis is clear.
- A sprint is DONE only when every DoD item is checked off.

### 6.3 Commits
- **Conventional Commits** format: `feat(search): ...`, `feat(haversine): ...`, `feat(admin): ...`, `test(haversine): ...`
- One logical change per commit — no bulk commits
- Tag each completed sprint: `git tag DEV-XX`
- Never commit secrets, never force-push, never skip hooks

### 6.4 No premature abstraction
One interface, one implementation. No factory/registry/strategy patterns unless the sprint explicitly requires it. No MediatR, CQRS, or event sourcing.

### 6.5 UI verification
After every UI sprint: user runs `dotnet run` and confirms behavior visually before the sprint is marked DONE. Never claim a UI feature works based on compilation alone.

### 6.6 Screenshot discipline
During UI sprints, Claude reminds which screenshots to capture for the thesis before moving to the next sprint. Screenshot inventory: [.claude/skills/thesis-screenshots/SKILL.md](.claude/skills/thesis-screenshots/SKILL.md).

### 6.7 Language rules
- **Code identifiers** (class, method, variable, property): English
- **All `docs/` and `.claude/skills/` files**: English
- **UI strings visible to users**: Uzbek, in `Resources/Strings.uz.resx`
- **Database column names**: English snake_case (`UseSnakeCaseNamingConvention()`)
- **Thesis write-up** (`docs/thesis/*.md`): Uzbek

### 6.8 Secrets handling
```bash
dotnet user-secrets init --project src/DrugstoreSystem.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<conn-string>"
dotnet user-secrets set "Seed:AdminPassword" "<password>"
```
Never in `appsettings.json`. Never in git. See [docs/security-and-config.md](docs/security-and-config.md).

---

## 7. Repository Layout

```
DrugstoreSystem/
├── .claude/
│   └── skills/
│       ├── dotnet-clean-architecture/   — .NET layering rules
│       ├── blazor-mudblazor-patterns/   — Blazor Server + MudBlazor UI patterns
│       ├── pharmacy-domain/             — domain knowledge: medicines, pharmacies, pg_trgm
│       ├── search-algorithm/            — search implementation guide
│       ├── optimization-algorithm/      — Haversine + sorting implementation guide
│       ├── git-github-workflow/         — VCS conventions
│       ├── thesis-writing/              — Uzbek BMI writing style rules
│       ├── thesis-structure/            — BMI chapter plan for this project
│       └── thesis-screenshots/          — screenshot inventory + capture instructions
├── docs/
│   ├── README.md                        — documentation index
│   ├── architecture.md
│   ├── database-schema.md
│   ├── api-contracts.md
│   ├── ui-pages.md
│   ├── search-algorithm.md              — multi-stage search specification
│   ├── optimization-algorithm.md        — Haversine + ranking specification
│   ├── sprints.md                       — execution plan (living document)
│   ├── coding-standards.md
│   ├── security-and-config.md
│   ├── demo-data.md
│   └── thesis/
│       └── images/                      — screenshots and diagrams for thesis
├── src/
│   ├── DrugstoreSystem.Domain/
│   ├── DrugstoreSystem.Application/
│   ├── DrugstoreSystem.Infrastructure/
│   └── DrugstoreSystem.Web/
├── tests/
│   └── DrugstoreSystem.UnitTests/
├── .gitignore
├── .editorconfig
├── DrugstoreSystem.sln
├── README.md
└── CLAUDE.md                            — this file
```

---

## 8. Definition of Ready / Done

### Definition of Ready (before starting a sprint)
- Sprint is fully described in `docs/sprints.md` with goal, tasks, and DoD
- All docs referenced by the sprint exist and are current
- Blocking questions from prior sprints are resolved

### Definition of Done (for any sprint)
- All listed tasks are checked off
- `dotnet build` succeeds with **zero warnings**
- `dotnet test` passes (when tests apply to the sprint)
- Manual UI verification by the user (for UI sprints)
- Sprint-specific screenshots captured and filed
- At least one commit per sprint with a descriptive message
- Sprint entry in `docs/sprints.md` updated with final commit SHA and any deviations

---

## 9. Session Protocol

On every new session, Claude must:
1. Read `CLAUDE.md` (this file)
2. Read `docs/sprints.md` — identify the last DONE sprint
3. Run `git log --oneline -5` to see what was done last
4. Ask the user: "Last completed sprint was `DEV-XX`. Shall we continue with `DEV-YY`?" — **do not assume**
5. Load the relevant skills for the upcoming sprint

---

## 10. Non-Negotiable Rules

1. **No code without docs.** If it is not in `docs/`, do not build it.
2. **No secrets in git.** Ever.
3. **No scope creep.** Features listed in §2.2 stay out until defense day.
4. **No skipped sprints.** Dependencies are real.
5. **No silent failures.** Report blockers immediately.
6. **No quick fixes that hide problems.** Fix the root cause or document the issue as a follow-up.

---

## 11. Glossary

| Term | Meaning |
|---|---|
| BMI | Bitiruv Malakaviy Ishi — graduation thesis |
| Pharmacy | A registered drugstore on the platform |
| Medicine | An entry in the shared medicine catalog |
| PharmacyMedicine | Inventory record linking a pharmacy to a medicine with price + quantity |
| MedicineSynonym | An alias or trade name for a Medicine (e.g., "Panadol" → Paracetamol) |
| Availability | `quantity > 0` AND `pharmacy.is_active = true` — always pre-filtered |
| Search Algorithm | 5-stage fuzzy medicine search using `pg_trgm` |
| Haversine | Formula for computing geodesic distance between two lat/lng coordinates |
| Optimization Algorithm | Haversine-based pharmacy ranking by distance or price |
| Guest | An unauthenticated user browsing the public-facing search interface |
| Shared Catalog | The single `medicines` table populated collectively by all pharmacists |
| SortMode | Enum: `Distance` (default) or `Price` |

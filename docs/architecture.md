# Architecture — DrugstoreSystem

This document describes the architectural style, component boundaries, request flows, and cross-cutting concerns. It is intentionally tight — aligned with the constraint that this is a university thesis, not a production SaaS.

---

## 1. Architectural Style

**Simplified Clean Architecture on a monolithic Blazor Server application.**

- **Monolith**: one process, one deployment, one database
- **Clean**: dependencies point inward — `Web → Infrastructure → Application → Domain`
- **Simplified**: no CQRS, no MediatR, no microservices, no domain events in v1

### Rationale

| Alternative | Why rejected |
|---|---|
| Microservices | Overkill for a single-user thesis demo |
| Minimal API + Blazor WASM frontend | Doubles work with no thesis benefit |
| Razor Pages / MVC | Less impressive at defense; Blazor Server demo is more interactive |
| Blazor WebAssembly | More complex deployment; no need for client-side execution |
| MediatR / CQRS | Ceremony without benefit at this scale |

---

## 2. Component Diagram (Logical)

```
 ┌──────────────────────────────────────────────────────────────────────┐
 │                        DrugstoreSystem.Web                            │
 │  ┌──────────┐  ┌──────────────┐  ┌──────────────┐  ┌─────────────┐  │
 │  │  Auth    │  │  Admin       │  │  Pharmacist  │  │  Public     │  │
 │  │  pages   │  │  pages       │  │  pages       │  │  pages      │  │
 │  └──────────┘  └──────────────┘  └──────────────┘  └─────────────┘  │
 │                       ▲ uses (via DI)                                │
 └───────────────────────┼──────────────────────────────────────────────┘
                         │
 ┌───────────────────────┼──────────────────────────────────────────────┐
 │                       ▼                                               │
 │               DrugstoreSystem.Application                             │
 │  ┌──────────────────┐  ┌──────────────────┐  ┌───────────────────┐  │
 │  │  IPharmacyService│  │  ISearchService  │  │  IInventoryService│  │
 │  └──────────────────┘  └──────────────────┘  └───────────────────┘  │
 │  ┌──────────────────┐  ┌──────────────────────────────────────────┐  │
 │  │  IMedicineService│  │  HaversineCalculator (pure static)       │  │
 │  └──────────────────┘  └──────────────────────────────────────────┘  │
 │               ▲                                                       │
 └───────────────┼───────────────────────────────────────────────────────┘
                 │
 ┌───────────────┼───────────────────────────────────────────────────────┐
 │               ▼                                                        │
 │          DrugstoreSystem.Infrastructure                                │
 │  ┌─────────────────┐  ┌───────────────────┐  ┌──────────────────┐    │
 │  │  DrugstoreDb    │  │  SearchRepository │  │  Repository      │    │
 │  │  Context (EF)   │  │  (pg_trgm queries)│  │  implementations │    │
 │  └─────────────────┘  └───────────────────┘  └──────────────────┘    │
 │  ┌─────────────────┐                                                  │
 │  │  Identity stores│                                                  │
 │  │  (ASP.NET Core) │                                                  │
 │  └─────────────────┘                                                  │
 └────────────────────────────────────────────────────────────────────────┘
                 │
                 ▼
         ┌──────────────┐
         │  PostgreSQL  │
         │  + pg_trgm   │
         └──────────────┘
```

---

## 3. Layer Responsibilities

### DrugstoreSystem.Domain
Pure business model: entities (`Pharmacy`, `Medicine`, `MedicineSynonym`, `Category`, `PharmacyMedicine`), enums (`SortMode`, `DosageForm`), and domain exceptions. Zero external dependencies. No ORM attributes.

### DrugstoreSystem.Application
Use-case layer. Defines service interfaces (`IPharmacyService`, `ISearchService`, `IInventoryService`, `IMedicineService`), DTOs, FluentValidation validators, and repository interfaces. Contains `HaversineCalculator` — a pure static class (no I/O, easily unit-tested). Does NOT talk to the database directly.

### DrugstoreSystem.Infrastructure
Fulfils Application interfaces. Hosts `DrugstoreDbContext`, all repository implementations, `SearchRepository` (raw SQL with `pg_trgm`), Identity entities (`AppUser`), migrations, and seed data. Nothing here is referenced directly by Web — Web uses Application interfaces; Infrastructure is wired via DI.

### DrugstoreSystem.Web
Blazor Server entry point. Contains pages grouped by role (`Pages/Public/`, `Pages/Admin/`, `Pages/Pharmacist/`, `Pages/Auth/`), layout, navigation, MudBlazor setup, Serilog setup, and the DI composition root (`Program.cs`). No business logic in pages.

### DrugstoreSystem.UnitTests
xUnit + FluentAssertions. Tests cover `HaversineCalculator` (all edge cases) and search result ranking (score ordering). Intentionally minimal — correctness of the two algorithms is what matters.

---

## 4. Request Flow: "Search for a medicine"

The primary user journey — the one that exercises both algorithms:

1. **User action** (on `/`): Guest types "paratsetamol" and the system reads their geolocation (or manual override).
2. **Web → Application**: `SearchPage.razor` calls `ISearchService.SearchAsync(SearchRequest)`.
3. **Application → Infrastructure (search)**: `SearchService` calls `ISearchRepository.FindMedicinesAsync(query)` — runs the 5-stage pg_trgm SQL query, returns `List<MedicineCandidateDto>` ranked by match score.
4. **Application → Infrastructure (inventory)**: For each matched medicine, `IInventoryRepository.GetAvailablePharmaciesAsync(medicineId)` returns pharmacies where `quantity > 0` AND `is_active = true`.
5. **Application (Haversine)**: `HaversineCalculator.Distance(userLat, userLng, p.Latitude, p.Longitude)` computed for each pharmacy. Results sorted by distance ASC or price ASC per `SortMode`.
6. **Web**: Results rendered in `MudTable`. Each pharmacy row shows name, address, price, distance, and a "Google Maps" icon link.

### Sequence (compressed)

```
Guest ──► SearchPage.razor ──► ISearchService.SearchAsync
                                │
                                ├─► ISearchRepository.FindMedicinesAsync ──► PostgreSQL (pg_trgm)
                                ├─► IInventoryRepository.GetAvailablePharmacies (per medicine)
                                └─► HaversineCalculator.Distance (per pharmacy)
                                     └─► Sort by distance or price
                                          └─► SearchResultDto[]
```

---

## 5. Request Flow: "Pharmacist adds a medicine to inventory"

1. Pharmacist opens `/pharmacist/inventory/add`.
2. Types medicine name → `IMedicineService.AutocompleteAsync(query)` returns matching shared-catalog entries → MudAutocomplete shows them.
3. **If medicine found**: pharmacist selects it, enters price + quantity, clicks Save → `IInventoryService.UpsertAsync(pharmacyId, request)` creates/updates `PharmacyMedicine` row.
4. **If medicine not found**: pharmacist fills a create-medicine form (name, generic name, Russian name, category, dosage form, synonyms) → `IMedicineService.CreateAsync(request)` creates a new `Medicine` entry (visible to all pharmacists) → then upserts `PharmacyMedicine`.

---

## 6. Configuration Flow

```
dotnet user-secrets (dev)           appsettings.Production.json (demo)
        │                                        │
        └──────────────────┬──────────────────────┘
                           ▼
              IConfiguration (ASP.NET Core)
              ├── ConnectionStrings:DefaultConnection  → EF Core
              └── Seed:AdminPassword                   → DatabaseSeeder
```

---

## 7. Cross-Cutting Concerns

### Search (pg_trgm)
The `pg_trgm` extension must be enabled on the PostgreSQL database: `CREATE EXTENSION IF NOT EXISTS pg_trgm`. GIN indexes on `medicine.name`, `medicine.generic_name`, `medicine.name_ru`, and `medicine_synonym.synonym` are created in the initial migration. Raw SQL is used in `SearchRepository` because EF Core cannot express trigram similarity queries via LINQ.

### Logging (Serilog)
Console sink (dev) + rolling file under `logs/` (both dev and demo). Minimum level: `Information` for app code, `Warning` for Microsoft/EF. Never log connection strings or user passwords.

### Authentication & Authorization
Cookie-based (ASP.NET Identity). Two roles: `Admin` and `Pharmacist`. Public pages (`/`, `/medicine/{id}`, `/pharmacy/{id}`) have no `[Authorize]` attribute. All admin and pharmacist pages have `[Authorize(Roles = "Admin")]` or `[Authorize(Roles = "Pharmacist")]`.

### Geolocation
Browser geolocation is requested via a small JS interop call on the search page (`navigator.geolocation.getCurrentPosition`). The result (lat/lng) is stored in Blazor component state. If denied or unavailable, the user is prompted to enter a location manually. No server-side geolocation.

### Background Work
None in v1. The search + Haversine run synchronously in-request. Typical search latency is expected to be under 200ms for the demo dataset.

---

## 8. Security Boundaries

| Boundary | Control |
|---|---|
| Guest → Web | No auth required; rate limiting on search endpoint if needed |
| Pharmacist → own data | `[Authorize(Roles = "Pharmacist")]` + pharmacyId claim check in service |
| Admin → all pharmacies | `[Authorize(Roles = "Admin")]` |
| Web → Database | EF Core parameterized queries; raw SQL in SearchRepository uses `@parameter` placeholders |
| Secrets at rest | `dotnet user-secrets` in dev; env vars in demo |

---

## 9. Deployment Model

For the thesis demo:
- **Local**: `dotnet run` on the student's laptop
- PostgreSQL runs locally (installed natively or via Docker Desktop)
- `pg_trgm` extension enabled once: `CREATE EXTENSION IF NOT EXISTS pg_trgm`
- Migrations applied at startup via `dbContext.Database.MigrateAsync()`
- Seed data runs on first startup when `pharmacies` table is empty

Not in v1: Docker image, cloud deployment, CI/CD, reverse proxy.

---

## 10. Known Trade-offs

| Choice | Trade-off |
|---|---|
| Monolith | Can't scale independently; not needed for thesis |
| Synchronous search | User waits inline; acceptable with MudBlazor progress indicator |
| Raw SQL in SearchRepository | Bypasses EF LINQ safety; mitigated by parameterized queries |
| Browser geolocation | Requires HTTPS in production; fine on localhost for demo |
| Single admin | Simpler auth; no multi-admin scenarios needed |
| Google Maps redirect (not embedded) | Less impressive visually; eliminates Leaflet.js JS interop complexity and potential demo-day failures |

---

## 11. Diagrams to produce for the thesis

1. **Architecture diagram** (this doc §2, cleaner version) → `docs/thesis/images/2.1.2-architecture.png`
2. **Solution Explorer screenshot** → `docs/thesis/images/2.1.1-solution-explorer.png`
3. **ER diagram** (from `database-schema.md`) → `docs/thesis/images/2.2.1-er-diagram.png`
4. **Search algorithm flowchart** → `docs/thesis/images/2.1.4-search-flowchart.png`
5. **Haversine formula diagram** → `docs/thesis/images/3.2.1-haversine-diagram.png`

---

## 12. Changelog

- **2026-05-14** — initial draft.

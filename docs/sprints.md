# Sprints — DrugstoreSystem

Execution plan. Each sprint is a self-contained, independently-completable unit of work. Sprints are completed **in order**. Time estimates are not given — pace depends on availability. What matters is that each sprint has an unambiguous Definition of Done.

Prefixes: `DEV-XX` for development, `DOC-XX` for thesis write-up.

---

## Legend

- **Status:** `TODO` / `IN_PROGRESS` / `DONE`
- **Depends on:** prior sprints whose DoD must be met first
- **DoD:** concrete checks that must all pass before marking DONE
- **Screenshots:** thesis images capturable during this sprint

---

## DEV-00 — Project Bootstrap

**Status:** TODO
**Depends on:** — (first sprint)

### Goal
Establish the empty-but-runnable repository. No business code — just scaffolding so everything after this compiles, runs, and is committed cleanly.

### Tasks
1. Create `.gitignore` (see [docs/security-and-config.md](security-and-config.md))
2. Create `.editorconfig` (see [docs/coding-standards.md](coding-standards.md) §7)
3. Create `DrugstoreSystem.sln`
4. Add four class library projects under `src/`:
   - `DrugstoreSystem.Domain` (net10.0)
   - `DrugstoreSystem.Application` (net10.0)
   - `DrugstoreSystem.Infrastructure` (net10.0)
5. Add `DrugstoreSystem.Web` as ASP.NET Core Blazor Server (net10.0)
6. Add `DrugstoreSystem.UnitTests` as xUnit test project under `tests/`
7. Wire project references: Web → Infrastructure → Application → Domain
8. Add NuGet packages per [docs/coding-standards.md](coding-standards.md) §8
9. Configure MudBlazor in `Program.cs`: `AddMudServices()`, providers in `MainLayout.razor`
10. Verify `dotnet build` is clean (zero warnings)
11. Verify `dotnet run --project src/DrugstoreSystem.Web` shows default Blazor welcome page
12. Add minimal `README.md` at repo root
13. Initial commit

### Packages to add at this sprint
- `DrugstoreSystem.Application`: `FluentValidation`, `Microsoft.Extensions.Logging.Abstractions`
- `DrugstoreSystem.Infrastructure`: (none yet — added in DEV-02)
- `DrugstoreSystem.Web`: `MudBlazor`, `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`, `FluentValidation.AspNetCore`
- `DrugstoreSystem.UnitTests`: `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`, `Microsoft.NET.Test.Sdk`

### Definition of Done
- [ ] Five `.csproj` files exist with `net10.0`
- [ ] Project references match architecture.md §2
- [ ] `dotnet restore` + `dotnet build` + `dotnet test` all pass
- [ ] `dotnet run` shows Blazor welcome page
- [ ] MudBlazor providers present in `MainLayout.razor`
- [ ] `.gitignore` excludes `bin/`, `obj/`, `logs/`, user-secrets paths
- [ ] Commit: `chore: bootstrap DrugstoreSystem solution (DEV-00)`

### Commit cadence
1–2 commits.

### Screenshots
- `2.1.1-solution-explorer.png` — Solution Explorer showing all projects

---

## DEV-01 — Domain Layer

**Status:** TODO
**Depends on:** DEV-00

### Goal
Author every domain entity and enum. No persistence, no EF attributes. Domain project compiles standalone.

### Tasks
1. Create enums under `Domain/Enums/`:
   - `SortMode` (Distance, Price)
   - `DosageForm` (Tablet, Capsule, Syrup, Injection, Cream, Drops, Powder, Suppository, Patch, Solution)
2. Create entities under `Domain/Entities/`:
   - `Category` (Id, Name)
   - `Medicine` (Id, Name, GenericName, NameRu, DosageForm, CategoryId, Manufacturer, Description, CreatedAt, CreatedByPharmacyId)
   - `MedicineSynonym` (Id, MedicineId, Synonym)
   - `Pharmacy` (Id, Name, Address, Latitude, Longitude, Phone, WorkingHours, IsActive, IsVerified, CreatedAt)
   - `PharmacyMedicine` (Id, PharmacyId, MedicineId, Price, Quantity, UpdatedAt)
3. Create `Domain/Exceptions/DomainException.cs`
4. `dotnet build` on Domain project — zero warnings

### Definition of Done
- [ ] `DrugstoreSystem.Domain` builds with zero warnings
- [ ] No `using Microsoft.*` beyond `System.*`
- [ ] No EF attributes (`[Key]`, `[Column]`, etc.)
- [ ] All entity setters are `private`
- [ ] All collection navigations initialized in constructors
- [ ] Private parameterless constructor on every entity
- [ ] Commit: `feat(domain): add core entities and enums (DEV-01)`

### Commit cadence
1–2 commits.

### Screenshots
- None mandatory. Folder screenshot optional for thesis §2.1.

---

## DEV-02 — Database and Migrations

**Status:** TODO
**Depends on:** DEV-01

### Goal
Wire EF Core to PostgreSQL with `pg_trgm`, produce the initial migration, verify it applies.

### Tasks
1. Install PostgreSQL locally (or `docker run -d --name drugstore-pg -e POSTGRES_PASSWORD=postgres -p 5432:5432 postgres:16`)
2. Install packages on `DrugstoreSystem.Infrastructure`:
   - `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Design`
   - `Npgsql.EntityFrameworkCore.PostgreSQL`, `EFCore.NamingConventions`
   - `Microsoft.AspNetCore.Identity.EntityFrameworkCore`
3. Create `Infrastructure/Identity/AppUser.cs` (extends `IdentityUser<int>`, adds `PharmacyId`)
4. Create `Infrastructure/Persistence/DrugstoreDbContext.cs`:
   - Extends `IdentityDbContext<AppUser, IdentityRole<int>, int>`
   - `DbSet<T>` for all domain entities
   - `UseSnakeCaseNamingConvention()`
5. Create `IEntityTypeConfiguration<T>` classes under `Persistence/Configurations/` per [database-schema.md](database-schema.md)
6. In the initial migration, add `migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pg_trgm;")` and all GIN indexes (see database-schema.md §2)
7. Configure DI in `Infrastructure/DependencyInjection.cs`
8. In `Program.cs`: call `builder.Services.AddInfrastructure(builder.Configuration)`
9. Set connection string via user-secrets (see [security-and-config.md](security-and-config.md) §1)
10. `dotnet ef migrations add 01_InitialSchema --project src/DrugstoreSystem.Infrastructure --startup-project src/DrugstoreSystem.Web`
11. `dotnet ef database update` — verify tables in pgAdmin

### Definition of Done
- [ ] All expected tables in `drugstore` database with correct types
- [ ] Snake_case applied: `pharmacy_medicines`, `medicine_synonyms`
- [ ] `pg_trgm` extension present: `SELECT * FROM pg_extension WHERE extname = 'pg_trgm'`
- [ ] GIN indexes present on medicine name columns
- [ ] `dotnet ef database update` is idempotent
- [ ] App starts without errors
- [ ] Commits: `feat(db): add DbContext and initial migration with pg_trgm (DEV-02)`

### Commit cadence
2–3 commits.

### Screenshots
- `2.2.2-connection-string.png` — appsettings.json snippet (no real password)
- `2.2.3-dbcontext.png` — DbContext class in IDE
- `2.2.4-entity-model.png` — Medicine entity + configuration class
- `2.2.6-migration-output.png` — terminal output of migration add + update

---

## DEV-03 — Authentication

**Status:** TODO
**Depends on:** DEV-02

### Goal
Admin and Pharmacist can log in. All admin/pharmacist pages require auth. Public pages remain open.

### Tasks
1. Register ASP.NET Identity with `Admin` and `Pharmacist` roles
2. Configure cookie auth: `LoginPath = "/auth/login"`, `AccessDeniedPath = "/auth/login"`
3. Implement `Pages/Auth/Login.razor` per [ui-pages.md](ui-pages.md) §3.1
4. Implement logout endpoint: POST `/auth/logout` → sign out → redirect to `/`
5. Implement `ICurrentUser` + `CurrentUser` in Infrastructure (reads from `IHttpContextAccessor`)
6. Implement `DatabaseSeeder.SeedAdminAsync`: seed Admin user + roles on first startup
7. `MainLayout.razor`: show logged-in user's email in AppBar + Logout button (only when authenticated)
8. Public pages (`/`, `/medicine/{id}`, `/pharmacy/{id}`) — verify they are accessible without login

### Definition of Done
- [ ] Unauthenticated visit to `/admin/dashboard` redirects to `/auth/login`
- [ ] Correct admin credentials → redirected to `/admin/dashboard`
- [ ] Wrong credentials → "Email yoki parol noto'g'ri"
- [ ] Logout → redirected to `/auth/login`
- [ ] `/` accessible without login
- [ ] DB has Admin user in `asp_net_users` after first startup
- [ ] Commit: `feat(auth): add login/logout, roles, and admin seed (DEV-03)`

### Commit cadence
2–3 commits.

### Screenshots
- `3.1.1-login.png` — Login page in browser

---

## DEV-04 — Admin Panel: Pharmacy Management

**Status:** TODO
**Depends on:** DEV-03

### Goal
Admin can register pharmacies, view them, edit, activate/deactivate, and delete. Each new pharmacy also gets a pharmacist login account.

### Tasks
1. Implement `IPharmacyRepository` + `PharmacyRepository`
2. Implement `IPharmacyService` + `PharmacyService`
3. Implement `IAdminService.CreatePharmacistAccountAsync` (creates Pharmacy + AppUser atomically)
4. Validators: `CreatePharmacyRequestValidator`, `UpdatePharmacyRequestValidator`
5. Pages:
   - `Pages/Admin/PharmacyList.razor`
   - `Pages/Admin/PharmacyCreate.razor` (MudStepper: pharmacy info → pharmacist account)
   - `Pages/Admin/PharmacyEdit.razor`
   - `Pages/Admin/Dashboard.razor` (basic stat cards — data from `IAdminService.GetDashboardAsync`)
6. NavMenu for Admin role

### Definition of Done
- [ ] Admin can create a pharmacy + pharmacist account in one flow
- [ ] Pharmacy list shows all pharmacies with active status
- [ ] Toggle switch activates/deactivates a pharmacy
- [ ] Edit updates name, address, coordinates, phone, working hours
- [ ] Delete with confirm dialog — works
- [ ] Pharmacist account created and able to log in (verify manually)
- [ ] Commits: `feat(admin): pharmacy CRUD and pharmacist account creation (DEV-04)`

### Commit cadence
3–5 commits.

### Screenshots
- `3.1.2-admin-dashboard.png`
- `3.1.3-pharmacy-list.png`
- `3.1.4-pharmacy-create.png`

---

## DEV-05 — Pharmacist Panel: Profile and Inventory

**Status:** TODO
**Depends on:** DEV-04

### Goal
Pharmacist can edit their own pharmacy profile and manage their medicine inventory (view, update prices/quantities, remove items). The medicine autocomplete is a stub in this sprint — real shared catalog comes in DEV-06.

### Tasks
1. Implement `IInventoryRepository` + `InventoryRepository`
2. Implement `IInventoryService` + `InventoryService`
3. Pages:
   - `Pages/Pharmacist/Profile.razor` — edit own pharmacy (name, address, lat, lng, phone, hours)
   - `Pages/Pharmacist/Inventory.razor` — list with inline edit (price, quantity) and remove
   - `Pages/Pharmacist/Dashboard.razor` — stat cards (total medicines, low stock count)
4. NavMenu for Pharmacist role
5. `ICurrentUser.PharmacyId` claim enforcement in `InventoryService` (pharmacist can only see/edit own pharmacy)

### Definition of Done
- [ ] Pharmacist can edit their pharmacy profile; changes persist
- [ ] Inventory list shows all their medicines
- [ ] Inline quantity/price edit works and updates `updated_at`
- [ ] Remove item with confirm dialog works
- [ ] Pharmacist cannot access another pharmacy's inventory (verify by URL manipulation)
- [ ] Commits: `feat(pharmacist): profile edit and inventory management (DEV-05)`

### Commit cadence
3–4 commits.

### Screenshots
- `3.1.5-pharmacist-profile.png`
- `3.1.6-inventory-list.png`

---

## DEV-06 — Shared Medicine Catalog (Autocomplete + Create)

**Status:** TODO
**Depends on:** DEV-05

### Goal
Implement the crowdsourced shared catalog. Pharmacist can add a medicine to their inventory by selecting from the shared catalog (autocomplete) or creating a new medicine entry if it doesn't exist.

### Tasks
1. Implement `IMedicineRepository` + `MedicineRepository`
2. Implement `IMedicineService` + `MedicineService` (autocomplete + create)
3. Implement `ISearchRepository.AutocompleteAsync` (partial match + trigram on `name`)
4. Build `Pages/Pharmacist/InventoryAdd.razor`:
   - `MudAutocomplete<MedicineAutocompleteDto>` — debounced 300ms, calls `AutocompleteAsync`
   - Selected medicine info card (read-only)
   - "Yangi dori yaratish" expansion panel when nothing found
   - Synonyms chip input
   - Price + quantity fields
   - Save → `IInventoryService.UpsertAsync`
5. "Create new" flow: `IMedicineService.CreateAsync` → then upsert inventory

### Definition of Done
- [ ] Typing 3+ chars shows matching medicines from shared catalog
- [ ] Selecting an existing medicine and saving → appears in inventory list
- [ ] Creating a new medicine → visible to other pharmacists in autocomplete
- [ ] Synonyms are saved and appear in medicine detail
- [ ] Commits: `feat(catalog): shared medicine catalog autocomplete and creation (DEV-06)`

### Commit cadence
3–5 commits.

### Screenshots
- `3.1.7-inventory-add-autocomplete.png` — autocomplete dropdown visible
- `3.1.8-create-new-medicine.png` — create form expanded

---

## DEV-07 — Public Search UI

**Status:** TODO
**Depends on:** DEV-06

### Goal
Guest users can search for medicines. Results show available pharmacies. This sprint wires the 5-stage search SQL but NOT the Haversine ranking yet — pharmacies shown in database order. Location and sort mode UI is present but non-functional (placeholder for DEV-08).

### Tasks
1. Implement `ISearchRepository.FindMedicinesAsync` — full pg_trgm SQL (see [search-algorithm.md](search-algorithm.md) §4)
2. Implement `IInventoryRepository.GetAvailablePharmaciesAsync`
3. Implement `ISearchService.SearchAsync` (without Haversine — return unsorted pharmacies)
4. Build `Pages/Public/SearchPage.razor` per [ui-pages.md](ui-pages.md) §2.1
5. Build `Pages/Public/MedicineDetail.razor`
6. Build `Pages/Public/PharmacyDetail.razor`
7. Build `Shared/PharmacyResultRow.razor` + `Shared/EmptyState.razor`
8. Add `wwwroot/js/geolocation.js` (JS interop — location read, no ranking yet)
9. NavMenu for Guest (logo + search link only)

### Definition of Done
- [ ] Searching "para" returns Paracetamol (requires seed data from DEV-09, use manual test data)
- [ ] Searching "paratsetamol" returns Paracetamol (trigram match)
- [ ] Searching "Парацетамол" returns Paracetamol (Russian name match)
- [ ] Pharmacies with `quantity = 0` do NOT appear in results
- [ ] Inactive pharmacies do NOT appear in results
- [ ] Medicine detail page shows synonyms
- [ ] Pharmacy detail page shows Google Maps button (opens in new tab)
- [ ] All empty states display correctly
- [ ] Commits: `feat(search): 5-stage fuzzy search and public pages (DEV-07)`

### Commit cadence
4–6 commits.

### Screenshots
- `3.1.9-search-results.png` — search results with pharmacy table
- `3.1.10-medicine-detail.png` — medicine detail page
- `3.1.11-pharmacy-detail.png` — pharmacy detail with maps button

---

## DEV-08 — Optimization Algorithm + Location

**Status:** TODO
**Depends on:** DEV-07

### Goal
Implement `HaversineCalculator` and `PharmacyRanker`. Wire them into `SearchService`. Connect geolocation JS interop and manual location input. Sort toggle becomes functional.

### Tasks
1. Implement `HaversineCalculator` (pure static class) per [optimization-algorithm.md](optimization-algorithm.md) §3
2. Implement `PharmacyRanker` per [optimization-algorithm.md](optimization-algorithm.md) §4
3. Write unit tests: `HaversineCalculatorTests.cs` — all 5 test cases from spec §7
4. Wire `PharmacyRanker.Rank()` into `SearchService.SearchAsync`
5. In `SearchPage.razor`:
   - JS interop `getLocation()` on page load → populate lat/lng state
   - Manual location input: `MudTextField` + Nominatim geocoding call (free, no API key) → update lat/lng
   - Sort toggle (`MudToggleGroup`) → update `SortMode` state → re-sort in-memory (no new DB call)
6. In `MedicineDetail.razor`: same sort toggle + location logic
7. Display `DistanceKm` formatted per spec ("0.4 km", "Masofa noma'lum")
8. `dotnet test` — all Haversine tests pass

### Definition of Done
- [ ] All `HaversineCalculatorTests` pass
- [ ] Search results are sorted by distance by default (closest pharmacy first)
- [ ] Toggle to "Narx bo'yicha" → results reorder by price ascending (no page reload)
- [ ] Geolocation prompt fires on search page load
- [ ] Manual location input overrides geolocation
- [ ] Distance shown as "X.X km" next to each pharmacy
- [ ] Tashkent → Samarkand distance ~281 km (demo verifiable with pharmacy #7)
- [ ] Commits: `feat(haversine): distance calculation and sort mode (DEV-08)` + `test(haversine): unit tests for all cases`

### Commit cadence
3–5 commits.

### Screenshots
- `3.2.1-haversine-code.png` — `HaversineCalculator.cs` in IDE
- `3.2.2-sort-distance.png` — results sorted by distance
- `3.2.3-sort-price.png` — results sorted by price
- `3.2.4-unit-tests.png` — test runner showing all Haversine tests passed

---

## DEV-09 — Seed Data

**Status:** TODO
**Depends on:** DEV-08

### Goal
Populate the database with demo data so the defense has meaningful content to show.

### Tasks
1. Implement `DatabaseSeeder` in `Infrastructure/Persistence/Seed/DatabaseSeeder.cs`
2. Create `pharmacies.json` — 8 pharmacies from [demo-data.md](demo-data.md) §2
3. Create `medicines.json` — 30 medicines with synonyms from [demo-data.md](demo-data.md) §3
4. Seed pharmacy inventories — each pharmacy stocked with ~15 medicines at different prices
5. Guard: `if (await context.Pharmacies.AnyAsync()) return;` — idempotent
6. Call seeder from `Program.cs` in dev mode after `MigrateAsync()`
7. Verify demo script from [demo-data.md](demo-data.md) §7 works end-to-end

### Definition of Done
- [ ] Fresh database + app start = 8 pharmacies, 30 medicines, ~120 inventory rows
- [ ] "para" search returns Paracetamol with 5+ pharmacies having it
- [ ] Sort by distance: pharmacies in correct order from Tashkent (closest first)
- [ ] Sort by price: cheapest pharmacy for Paracetamol is first
- [ ] "paratsetamol" typo still finds Paracetamol
- [ ] "Парацетамол" still finds Paracetamol
- [ ] Commit: `feat(seed): add demo pharmacies and medicines (DEV-09)`

### Commit cadence
2–3 commits.

### Screenshots
- `3.1.12-demo-data-search.png` — search with real seeded data showing distances

---

## DEV-10 — Final QA, Polish, Demo Prep

**Status:** TODO
**Depends on:** DEV-09

### Goal
Stabilize. Fix all bugs found in a complete end-to-end walkthrough. Capture all remaining thesis screenshots. Tag v1.0.

### Tasks
1. Run full demo script (demo-data.md §7) — fix every bug encountered
2. Check all empty states, error states, and edge cases
3. Capture all remaining screenshots per [.claude/skills/thesis-screenshots/SKILL.md](../.claude/skills/thesis-screenshots/SKILL.md)
4. Serilog request logging: verify `logs/drugstore-YYYYMMDD.log` is written
5. Ensure `dotnet build` — zero warnings
6. Ensure `dotnet test` — all pass
7. Tag: `git tag v1.0-defense`

### Definition of Done
- [ ] Full demo script runs without errors
- [ ] All thesis screenshots captured and organized under `docs/thesis/images/`
- [ ] `dotnet build` — zero warnings
- [ ] `dotnet test` — all pass
- [ ] Log file written to `logs/`
- [ ] Tag `v1.0-defense` pushed
- [ ] Commits: `fix: ...` (per bug), `chore: final QA and demo prep (DEV-10)`

### Commit cadence
3–5 commits.

---

## Sprint Dependency Graph

```
DEV-00 Bootstrap
  └─ DEV-01 Domain
       └─ DEV-02 Database + pg_trgm
            └─ DEV-03 Auth
                 └─ DEV-04 Admin Panel (Pharmacy CRUD)
                      └─ DEV-05 Pharmacist Panel (Profile + Inventory)
                           └─ DEV-06 Shared Catalog (Autocomplete + Create)
                                └─ DEV-07 Public Search UI
                                     └─ DEV-08 Optimization Algorithm + Location
                                          └─ DEV-09 Seed Data
                                               └─ DEV-10 QA + Demo Prep
```

---

## Post-dev: Thesis Write-up Sprints (detail after DEV-10)

- **DOC-01** — Capture final screenshots, organize `docs/thesis/images/`
- **DOC-02** — Write KIRISH (Introduction)
- **DOC-03** — Write I BOB (Theoretical chapter)
- **DOC-04** — Write II BOB (Design chapter)
- **DOC-05** — Write III BOB (Implementation chapter)
- **DOC-06** — Write IV BOB (Hayot faoliyati xavfsizligi — standard template)
- **DOC-07** — Write XULOSA + ADABIYOTLAR + DASTUR ILOVASI
- **DOC-08** — Pandoc → Word export, final formatting to university standard
- **DOC-09** — Antiplagiat check + fixes
- **DOC-10** — Defense presentation (PPT) + handout prep

---

## Current Status

| Sprint | Status | Branch | Commit | Notes |
|---|---|---|---|---|
| DEV-00 | TODO | — | — | — |
| DEV-01 | TODO | — | — | — |
| DEV-02 | TODO | — | — | — |
| DEV-03 | TODO | — | — | — |
| DEV-04 | TODO | — | — | — |
| DEV-05 | TODO | — | — | — |
| DEV-06 | TODO | — | — | — |
| DEV-07 | TODO | — | — | — |
| DEV-08 | TODO | — | — | — |
| DEV-09 | TODO | — | — | — |
| DEV-10 | TODO | — | — | — |

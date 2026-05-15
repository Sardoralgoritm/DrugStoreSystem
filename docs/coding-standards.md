# Coding Standards — DrugstoreSystem

---

## 1. Language & Runtime

- Target framework: `net10.0`
- C# 13 features enabled
- `<Nullable>enable</Nullable>` — all projects
- `<ImplicitUsings>enable</ImplicitUsings>` — all projects
- `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` — Domain and Application only

---

## 2. Naming Conventions

| Artifact | Convention | Example |
|---|---|---|
| Class, interface, record | PascalCase | `PharmacyService`, `ISearchRepository` |
| Method | PascalCase | `SearchAsync`, `GetByIdAsync` |
| Property | PascalCase | `PharmacyId`, `IsActive` |
| Local variable | camelCase | `userLat`, `matchedMedicines` |
| Private field | `_camelCase` | `_pharmacyRepo`, `_logger` |
| Constant | PascalCase | `EarthRadiusKm` |
| Enum | PascalCase values | `SortMode.Distance` |
| DB column (via EF) | snake_case auto | `pharmacy_id`, `created_at` |
| Blazor component | PascalCase file | `SearchPage.razor`, `NavMenu.razor` |
| DTO / record | PascalCase + suffix | `PharmacyDto`, `SearchRequest`, `SearchResultDto` |

---

## 3. Project Conventions

### Domain project rules
- No `using` of any external NuGet package
- All entity setters are `private` (EF Core uses private setters)
- Every entity has a private parameterless constructor for EF Core proxy creation
- Collection navigations initialized in constructors: `Skills = new List<Skill>()`
- No `[Key]`, `[Column]`, or data annotation attributes — EF config is in `IEntityTypeConfiguration<T>` classes

### Application project rules
- Interfaces for all services and repositories
- DTOs are `record` types (immutable)
- Service methods return DTOs, never domain entities
- All service methods are `async Task<T>` — no sync wrappers
- `HaversineCalculator` and `PharmacyRanker` are `static` classes — pure functions, no DI

### Infrastructure project rules
- One `IEntityTypeConfiguration<T>` class per entity, under `Persistence/Configurations/`
- Raw SQL only in `SearchRepository` — parameterized via `FormattableString` or `DbParameter`
- Never interpolate user input directly into SQL strings
- Migrations under `Migrations/` (EF default output)

### Web project rules
- Pages never contain business logic — they call services and render results
- `@inject` services at top of `.razor` files
- Navigation via `NavigationManager.NavigateTo()` — no `href` for SPA navigation
- All protected pages have `@attribute [Authorize(Roles = "...")]`
- Public pages have no `[Authorize]` attribute

---

## 4. File Organization

```
DrugstoreSystem.Domain/
├── Entities/
│   ├── Pharmacy.cs
│   ├── Medicine.cs
│   ├── MedicineSynonym.cs
│   ├── Category.cs
│   └── PharmacyMedicine.cs
├── Enums/
│   ├── SortMode.cs
│   └── DosageForm.cs
└── Exceptions/
    └── DomainException.cs

DrugstoreSystem.Application/
├── Interfaces/
│   ├── ISearchService.cs
│   ├── IPharmacyService.cs
│   ├── IMedicineService.cs
│   ├── IInventoryService.cs
│   ├── IAdminService.cs
│   ├── ISearchRepository.cs
│   ├── IPharmacyRepository.cs
│   ├── IInventoryRepository.cs
│   └── IMedicineRepository.cs
├── Services/
│   ├── SearchService.cs
│   ├── PharmacyService.cs
│   ├── MedicineService.cs
│   ├── InventoryService.cs
│   └── AdminService.cs
├── Algorithms/
│   ├── HaversineCalculator.cs
│   └── PharmacyRanker.cs
├── DTOs/
│   └── (all DTO record types)
└── Validators/
    └── (all FluentValidation validators)

DrugstoreSystem.Infrastructure/
├── Persistence/
│   ├── DrugstoreDbContext.cs
│   ├── Configurations/
│   │   └── (IEntityTypeConfiguration classes)
│   ├── Repositories/
│   │   └── (repository implementations)
│   ├── Search/
│   │   └── SearchRepository.cs
│   └── Seed/
│       ├── DatabaseSeeder.cs
│       ├── pharmacies.json
│       └── medicines.json
├── Identity/
│   └── AppUser.cs
├── Migrations/
└── DependencyInjection.cs

DrugstoreSystem.Web/
├── Pages/
│   ├── Public/
│   │   ├── SearchPage.razor
│   │   ├── MedicineDetail.razor
│   │   └── PharmacyDetail.razor
│   ├── Admin/
│   │   ├── Dashboard.razor
│   │   ├── PharmacyList.razor
│   │   ├── PharmacyCreate.razor
│   │   └── PharmacyEdit.razor
│   ├── Pharmacist/
│   │   ├── Dashboard.razor
│   │   ├── Profile.razor
│   │   ├── Inventory.razor
│   │   └── InventoryAdd.razor
│   └── Auth/
│       └── Login.razor
├── Shared/
│   ├── NavMenu.razor
│   ├── MainLayout.razor
│   ├── PharmacyResultRow.razor
│   └── EmptyState.razor
├── Resources/
│   └── Strings.uz.resx
├── wwwroot/
│   └── js/
│       └── geolocation.js
└── Program.cs
```

---

## 5. Commit Messages (Conventional Commits)

Format: `<type>(<scope>): <description>`

| Type | When |
|---|---|
| `feat` | New feature |
| `fix` | Bug fix |
| `test` | Adding or updating tests |
| `refactor` | Code change that is neither feature nor fix |
| `docs` | Documentation changes |
| `chore` | Build, config, dependency changes |
| `style` | Formatting only |

**Scopes:** `domain`, `db`, `auth`, `admin`, `pharmacist`, `search`, `haversine`, `inventory`, `public`, `seed`, `web`

**Examples:**
```
feat(domain): add Pharmacy and Medicine entities (DEV-01)
feat(db): add initial migration and pg_trgm indexes (DEV-02)
feat(auth): login/logout for Admin and Pharmacist roles (DEV-03)
feat(search): implement 5-stage fuzzy medicine search (DEV-07)
feat(haversine): implement distance ranking and SortMode toggle (DEV-08)
test(haversine): add distance calculation unit tests
docs(search): update search-algorithm.md with LIMIT rationale
```

---

## 6. Code Comments Policy

Comments are written only when the **WHY** is non-obvious:
- A pg_trgm quirk
- A PostgreSQL limitation or workaround
- A scoring weight rationale
- Non-intuitive business rule

**Never comment:**
- What the code does (the code says it)
- Caller references ("used by SearchPage")
- TODOs left in committed code

One-line `///` summary on public Application-layer service methods. Nothing on internal helpers.

---

## 7. `.editorconfig` Settings

```ini
[*.cs]
indent_style = space
indent_size = 4
end_of_line = crlf
charset = utf-8-bom
trim_trailing_whitespace = true
insert_final_newline = true
dotnet_sort_system_directives_first = true
```

---

## 8. NuGet Packages

| Project | Package |
|---|---|
| Application | `FluentValidation`, `Microsoft.Extensions.Logging.Abstractions` |
| Infrastructure | `Microsoft.EntityFrameworkCore`, `Npgsql.EntityFrameworkCore.PostgreSQL`, `EFCore.NamingConventions`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore` |
| Web | `MudBlazor`, `Serilog.AspNetCore`, `Serilog.Sinks.Console`, `Serilog.Sinks.File`, `FluentValidation.AspNetCore` |
| UnitTests | `xunit`, `xunit.runner.visualstudio`, `FluentAssertions`, `Microsoft.NET.Test.Sdk` |

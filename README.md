# DrugstoreSystem

A multi-pharmacy web platform that helps users find medicines at nearby pharmacies.

**Thesis:** Dorixonalardan dori buyurtma berishning eng yaqin yechimini topib beruvchi dasturiy vositasini ishlab chiqish  
**Student:** Sharipova Moxichexra Oltinovna, Alfraganus universiteti  
**Stack:** .NET 10 · Blazor Server · MudBlazor · EF Core · PostgreSQL · pg_trgm

## Quick Start

```bash
dotnet user-secrets init --project src/DrugstoreSystem.Web
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=drugstore;Username=postgres;Password=YOUR_PASSWORD"
dotnet user-secrets set "Seed:AdminPassword" "Admin@123!"

dotnet run --project src/DrugstoreSystem.Web
```

## Project Structure

```
src/
  DrugstoreSystem.Domain          — entities, enums
  DrugstoreSystem.Application     — services, DTOs, Haversine algorithm
  DrugstoreSystem.Infrastructure  — EF Core, repositories, pg_trgm search
  DrugstoreSystem.Web             — Blazor Server UI
tests/
  DrugstoreSystem.UnitTests       — Haversine + search ranking tests
```

## Documentation

See [docs/README.md](docs/README.md) for the full documentation index.

---
name: dotnet-clean-architecture
description: .NET 10 Clean Architecture layering rules for DrugstoreSystem. Open before adding any new class to decide which project it belongs in, and before wiring DI.
---

# .NET Clean Architecture — DrugstoreSystem

---

## 1. The Dependency Rule (non-negotiable)

```
DrugstoreSystem.Web
    ↓ references
DrugstoreSystem.Infrastructure
    ↓ references
DrugstoreSystem.Application
    ↓ references
DrugstoreSystem.Domain
```

**Domain has zero outward dependencies.** If you find yourself adding a NuGet package to Domain, stop — you are in the wrong layer.

---

## 2. What Goes Where

### Domain (`DrugstoreSystem.Domain`)
✅ Entities: `Pharmacy`, `Medicine`, `MedicineSynonym`, `Category`, `PharmacyMedicine`
✅ Enums: `SortMode`, `DosageForm`
✅ Domain exceptions: `DomainException`
❌ No EF Core, no FluentValidation, no ASP.NET types
❌ No `using Microsoft.EntityFrameworkCore`

### Application (`DrugstoreSystem.Application`)
✅ Service interfaces: `ISearchService`, `IPharmacyService`, `IMedicineService`, `IInventoryService`, `IAdminService`
✅ Repository interfaces: `ISearchRepository`, `IPharmacyRepository`, `IInventoryRepository`, `IMedicineRepository`
✅ DTOs (record types): `PharmacyDto`, `SearchResultDto`, `RankedPharmacyDto`, etc.
✅ FluentValidation validators
✅ Service implementations (business logic only): `SearchService`, `PharmacyService`, etc.
✅ Pure algorithm classes: `HaversineCalculator`, `PharmacyRanker`
❌ No EF Core DbContext
❌ No HTTP clients
❌ No Blazor or ASP.NET types

### Infrastructure (`DrugstoreSystem.Infrastructure`)
✅ `DrugstoreDbContext` (EF Core)
✅ Repository implementations
✅ `SearchRepository` (raw SQL with pg_trgm)
✅ Identity: `AppUser`, role seeding
✅ Migrations
✅ `DatabaseSeeder`
✅ `DependencyInjection.cs` — wires everything
❌ Never referenced directly by Web (except `DependencyInjection.cs` extension method)

### Web (`DrugstoreSystem.Web`)
✅ Blazor pages (`.razor` files)
✅ Shared components
✅ `Program.cs` (DI composition root)
✅ Serilog setup
✅ Resource files (`Strings.uz.resx`)
✅ `wwwroot/` (JS, CSS)
❌ No business logic in pages
❌ No direct EF Core usage

---

## 3. DI Wiring Pattern

All DI registrations for Infrastructure go in one extension method:

```csharp
// DrugstoreSystem.Infrastructure/DependencyInjection.cs
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DrugstoreDbContext>(options =>
            options
                .UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

        services.AddIdentity<AppUser, IdentityRole<int>>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<DrugstoreDbContext>()
        .AddDefaultTokenProviders();

        // Repositories
        services.AddScoped<IPharmacyRepository, PharmacyRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IMedicineRepository, MedicineRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();

        // Services (Application layer — registered here so Web doesn't reference Infrastructure directly)
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IPharmacyService, PharmacyService>();
        services.AddScoped<IMedicineService, MedicineService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IAdminService, AdminService>();

        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddHttpContextAccessor();

        return services;
    }
}
```

**Why services are registered in Infrastructure:** Application services depend on repository interfaces. Infrastructure owns both the implementations AND the registration. Web just calls `AddInfrastructure()`.

---

## 4. Entity Rules (Domain layer)

```csharp
// Example: Medicine.cs
public class Medicine
{
    // EF Core requires private parameterless ctor
    private Medicine() { }

    public Medicine(string name, string? genericName, ...)
    {
        Name = name;
        GenericName = genericName;
        Synonyms = new List<MedicineSynonym>();
        CreatedAt = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string? GenericName { get; private set; }
    // ... other properties with private setters

    public ICollection<MedicineSynonym> Synonyms { get; private set; }

    // Domain behavior methods (if needed)
    public void Update(string name, ...) { Name = name; ... }
}
```

---

## 5. EF Configuration (Infrastructure layer)

```csharp
// Infrastructure/Persistence/Configurations/MedicineConfiguration.cs
public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.GenericName).HasMaxLength(200);
        builder.Property(m => m.NameRu).HasMaxLength(200);
        builder.Property(m => m.DosageForm).HasMaxLength(100)
               .HasConversion<string>(); // store enum as string

        builder.HasOne<Category>().WithMany()
               .HasForeignKey(m => m.CategoryId).IsRequired(false);

        builder.HasMany(m => m.Synonyms).WithOne()
               .HasForeignKey(s => s.MedicineId).OnDelete(DeleteBehavior.Cascade);

        // GIN indexes are added via migrationBuilder.Sql() — NOT here
    }
}
```

---

## 6. Startup Pattern

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthentication(...);

var app = builder.Build();

// Migrate and seed on startup
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<DrugstoreDbContext>();
    await ctx.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider);
}

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

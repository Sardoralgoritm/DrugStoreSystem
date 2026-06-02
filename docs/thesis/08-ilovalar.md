# DASTUR ILOVASI

## Ilova 1. HaversineCalculator.cs — Geodezik masofa hisoblash algoritmi

```csharp
namespace DrugstoreSystem.Application.Algorithms;

public static class HaversineCalculator
{
    private const double EarthRadiusKm = 6371.0;

    public static double Distance(double lat1, double lng1, double lat2, double lng2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLng = ToRadians(lng2 - lng1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
              * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

        var c = 2.0 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0 - a));
        return EarthRadiusKm * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
```

## Ilova 2. PharmacyRanker.cs — Dorixonalarni saralash moduli

```csharp
using DrugstoreSystem.Application.DTOs;
using DrugstoreSystem.Domain.Enums;

namespace DrugstoreSystem.Application.Algorithms;

public static class PharmacyRanker
{
    public static IReadOnlyList<RankedPharmacyDto> Rank(
        IEnumerable<PharmacyResultDto> pharmacies,
        double? userLat,
        double? userLng,
        SortMode sortMode)
    {
        var ranked = pharmacies.Select(p => new RankedPharmacyDto(
            PharmacyId:   p.PharmacyId,
            PharmacyName: p.PharmacyName,
            Address:      p.Address,
            Phone:        p.Phone,
            WorkingHours: p.WorkingHours,
            Price:        p.Price,
            Quantity:     p.Quantity,
            DistanceKm:   (userLat.HasValue && userLng.HasValue)
                              ? Math.Round(HaversineCalculator.Distance(
                                    userLat.Value, userLng.Value,
                                    p.Latitude, p.Longitude), 1)
                              : (double?)null,
            MapsUrl: FormattableString.Invariant(
                $"https://www.google.com/maps/search/?api=1&query={p.Latitude},{p.Longitude}")
        ));

        return sortMode switch
        {
            SortMode.Price    => ranked.OrderBy(p => p.Price).ToList(),
            SortMode.Distance => ranked.OrderBy(p => p.DistanceKm ?? double.MaxValue).ToList(),
            _                 => throw new ArgumentOutOfRangeException(nameof(sortMode))
        };
    }
}
```

## Ilova 3. SearchRepository.cs — pg_trgm 5 bosqichli qidiruv

```csharp
public async Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(
    string query, CancellationToken ct = default)
{
    var q = query.Trim().ToLower();
    if (q.Length < 2) return [];

    var results = await _db.Database
        .SqlQuery<MedicineCandidateRaw>($"""
            SELECT
                m.id, m.name, m.generic_name, m.name_ru,
                m.dosage_form, m.manufacturer, m.description,
                c.name AS category_name,
                CASE
                    WHEN LOWER(m.name) = {q}
                      OR LOWER(m.generic_name) = {q}
                      OR LOWER(m.name_ru) = {q}
                    THEN 1.0
                    WHEN LOWER(m.name) LIKE '%' || {q} || '%'
                      OR LOWER(m.generic_name) LIKE '%' || {q} || '%'
                      OR LOWER(m.name_ru) LIKE '%' || {q} || '%'
                    THEN 0.7
                    WHEN EXISTS (
                        SELECT 1 FROM medicine_synonyms s
                        WHERE s.medicine_id = m.id
                          AND LOWER(s.synonym) LIKE '%' || {q} || '%'
                    )
                    THEN 0.65
                    ELSE GREATEST(
                        similarity(m.name, {q}),
                        COALESCE(similarity(m.generic_name, {q}), 0.0),
                        COALESCE(similarity(m.name_ru, {q}), 0.0)
                    )
                END AS score
            FROM medicines m
            LEFT JOIN categories c ON c.id = m.category_id
            WHERE
                LOWER(m.name) LIKE '%' || {q} || '%'
                OR LOWER(m.generic_name) LIKE '%' || {q} || '%'
                OR LOWER(m.name_ru) LIKE '%' || {q} || '%'
                OR EXISTS (
                    SELECT 1 FROM medicine_synonyms s
                    WHERE s.medicine_id = m.id
                      AND LOWER(s.synonym) LIKE '%' || {q} || '%'
                )
                OR similarity(m.name, {q}) > 0.3
                OR similarity(m.generic_name, {q}) > 0.3
                OR similarity(m.name_ru, {q}) > 0.3
            ORDER BY score DESC
            LIMIT 20
        """)
        .ToListAsync(ct);

    return results
        .Select(r => new MedicineCandidateDto(
            r.Id, r.Name, r.GenericName, r.NameRu,
            r.DosageForm, r.Manufacturer, r.Description,
            r.CategoryName, r.Score))
        .ToList();
}
```

## Ilova 4. DrugstoreDbContext.cs — Ma'lumotlar bazasi konteksti

```csharp
using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Persistence;

public class DrugstoreDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public DrugstoreDbContext(DbContextOptions<DrugstoreDbContext> options)
        : base(options) { }

    public DbSet<Category>         Categories         => Set<Category>();
    public DbSet<Medicine>         Medicines          => Set<Medicine>();
    public DbSet<MedicineSynonym>  MedicineSynonyms   => Set<MedicineSynonym>();
    public DbSet<Pharmacy>         Pharmacies         => Set<Pharmacy>();
    public DbSet<PharmacyMedicine> PharmacyMedicines  => Set<PharmacyMedicine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DrugstoreDbContext).Assembly);
    }
}
```

## Ilova 5. Program.cs — Tizim ishga tushirish konfiguratsiyasi

```csharp
using DrugstoreSystem.Infrastructure;
using DrugstoreSystem.Infrastructure.Persistence;
using MudBlazor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/drugstore-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddMudServices();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DrugstoreDbContext>();
    await db.Database.MigrateAsync();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
```

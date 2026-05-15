---
name: search-algorithm
description: Implementation guide for the 5-stage fuzzy medicine search using pg_trgm. Open before implementing or modifying SearchRepository, ISearchService, or any search-related code.
---

# Search Algorithm — Implementation Guide

Full specification: [docs/search-algorithm.md](../../docs/search-algorithm.md)

---

## 1. Architecture Location

```
DrugstoreSystem.Application
  └── Interfaces/ISearchRepository.cs     — interface (FindMedicinesAsync, AutocompleteAsync)
  └── Services/SearchService.cs           — orchestrates search + ranking

DrugstoreSystem.Infrastructure
  └── Persistence/Search/SearchRepository.cs  — raw SQL implementation
```

Search logic lives in Infrastructure (raw SQL) but the orchestration (search → rank) lives in Application. This keeps Application testable.

---

## 2. Raw SQL Execution in EF Core (Correct Pattern)

Use `Database.SqlQuery<T>()` with interpolated strings (EF Core 7+ parameterizes these automatically):

```csharp
// SearchRepository.cs
public async Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(string query, CancellationToken ct)
{
    var q = query.Trim().ToLower();
    if (q.Length < 2) return [];

    var results = await _context.Database
        .SqlQuery<MedicineCandidateRaw>($"""
            SELECT
                m.id,
                m.name,
                m.generic_name,
                m.name_ru,
                m.dosage_form,
                m.manufacturer,
                m.description,
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

    return results.Select(r => new MedicineCandidateDto(...)).ToList();
}
```

**Important:** `Database.SqlQuery<T>()` with `$""` interpolated strings in EF Core **automatically uses parameterized queries** — no SQL injection risk. Do NOT use string concatenation.

---

## 3. The Raw Result Type

EF Core's `SqlQuery<T>` maps by column name. Create a private record:

```csharp
// In SearchRepository.cs (private, not in Application)
private record MedicineCandidateRaw(
    int Id,
    string Name,
    string? GenericName,
    string? NameRu,
    string? DosageForm,
    string? Manufacturer,
    string? Description,
    string? CategoryName,
    double Score
);
```

Column names in SQL must match property names exactly (case-insensitive with Npgsql).

---

## 4. Autocomplete (for Pharmacist Inventory Add)

Simpler query — partial match + light trigram, top 10 results:

```csharp
public async Task<IReadOnlyList<MedicineAutocompleteDto>> AutocompleteAsync(string query, CancellationToken ct)
{
    var q = query.Trim().ToLower();
    if (q.Length < 2) return [];

    return await _context.Medicines
        .Where(m =>
            EF.Functions.ILike(m.Name, $"%{q}%") ||
            EF.Functions.ILike(m.GenericName ?? "", $"%{q}%") ||
            EF.Functions.TrigramsSimilarity(m.Name, q) > 0.2)  // needs Npgsql.EntityFrameworkCore.PostgreSQL
        .OrderByDescending(m => EF.Functions.TrigramsSimilarity(m.Name, q))
        .Take(10)
        .Select(m => new MedicineAutocompleteDto(m.Id, m.Name, m.GenericName, m.NameRu, m.DosageForm))
        .ToListAsync(ct);
}
```

If `EF.Functions.TrigramsSimilarity` is not available in the installed Npgsql version, fall back to raw SQL for autocomplete too.

---

## 5. pg_trgm Similarity Threshold Rationale

| Threshold | Catches | Misses |
|---|---|---|
| > 0.1 | Too many false positives | — |
| > 0.2 | Good for autocomplete (3+ char input) | Very short inputs |
| > 0.3 | Right for main search (2+ char input) | Very short inputs |
| > 0.5 | Only close matches | "paratsetamol" → misses Paracetamol |

The search uses **0.3** as the trigram threshold. Exact and LIKE stages already catch most correct matches; trigram is the safety net for typos.

---

## 6. Debouncing in Blazor (Autocomplete)

```razor
@* InventoryAdd.razor *@
<MudAutocomplete T="MedicineAutocompleteDto"
                 Label="Dori nomini qidiring..."
                 SearchFunc="@SearchMedicines"
                 ToStringFunc="@(m => m?.Name ?? "")"
                 MinCharacters="2"
                 DebounceInterval="300"
                 ResetValueOnEmptyText="true" />

@code {
    private async Task<IEnumerable<MedicineAutocompleteDto>> SearchMedicines(string value, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length < 2)
            return [];
        return await MedicineService.AutocompleteAsync(value, ct);
    }
}
```

`DebounceInterval="300"` prevents a DB call on every keystroke.

---

## 7. Common Mistakes to Avoid

| Mistake | Correct approach |
|---|---|
| String interpolation in raw SQL (`$"...{userInput}..."`) | Use EF Core's `SqlQuery<T>($"...{param}...")` — parameterized |
| Forgetting `COALESCE` on nullable columns in `GREATEST()` | `COALESCE(similarity(m.generic_name, {q}), 0.0)` |
| Not normalizing input before query | Always `.Trim().ToLower()` before `q` is used |
| Returning medicines with score = 0 | The `WHERE` clause guarantees at least one stage matched |
| Not limiting results | Always `LIMIT 20` on the main search query |

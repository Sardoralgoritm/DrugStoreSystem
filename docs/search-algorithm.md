# Search Algorithm — DrugstoreSystem

This document fully specifies the multi-stage medicine search algorithm. It is the **first academic contribution** of this thesis. Implementation must match this spec exactly.

**Implementation class:** `DrugstoreSystem.Infrastructure/Search/SearchRepository.cs`
**Interface:** `DrugstoreSystem.Application/Interfaces/ISearchRepository.cs`
**Skill file:** [.claude/skills/search-algorithm/SKILL.md](../.claude/skills/search-algorithm/SKILL.md)

---

## 1. Purpose

Users will search for medicines in many ways:
- Correct spelling: "Paracetamol"
- Typo: "Paratsetamol", "Paracetomol"
- Russian name: "Парацетамол"
- Trade/brand name: "Panadol", "Para-500"
- Partial: "para", "amox"
- Generic name: "Acetaminophen"

The system must return the correct medicine in all these cases. A simple `LIKE` query handles only exact partial matches. The multi-stage approach combines four matching strategies, ranks results by relevance, and returns a unified scored list.

---

## 2. Prerequisites

`pg_trgm` extension must be enabled **once** on the database:
```sql
CREATE EXTENSION IF NOT EXISTS pg_trgm;
```

GIN indexes (created in migration DEV-02):
```sql
CREATE INDEX ix_medicines_name_trgm         ON medicines USING GIN (name gin_trgm_ops);
CREATE INDEX ix_medicines_generic_name_trgm  ON medicines USING GIN (generic_name gin_trgm_ops);
CREATE INDEX ix_medicines_name_ru_trgm      ON medicines USING GIN (name_ru gin_trgm_ops);
CREATE INDEX ix_synonyms_trgm               ON medicine_synonyms USING GIN (synonym gin_trgm_ops);
```

---

## 3. Algorithm Stages

### Stage 0: Input Normalization
```
query = input.Trim().ToLower()
if (query.Length < 2) → return empty result immediately
```

### Stage 1–4: Single SQL Query

All four match strategies are combined into one SQL query with a `CASE` expression that assigns a score:

| Stage | Strategy | Score |
|---|---|---|
| 1 | Exact match on `name`, `generic_name`, or `name_ru` | **1.0** |
| 2 | ILIKE contains (`%query%`) on `name`, `generic_name`, or `name_ru` | **0.7** |
| 3 | ILIKE contains on any `medicine_synonyms.synonym` | **0.65** |
| 4 | Trigram similarity `> 0.3` on `name`, `generic_name`, or `name_ru` | **similarity value** (0.3–1.0) |

A medicine passes if ANY stage matches (score > 0). Results are deduplicated (one row per medicine) and ordered by score DESC.

### Stage 5: Availability Filter
After medicines are found, only pharmacies where `quantity > 0` AND `is_active = true` are included. A medicine with no available pharmacy is still returned — but its pharmacy list will be empty (show "Bu dori hech bir dorixonada mavjud emas" message).

---

## 4. Full SQL Specification

```sql
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
        WHEN LOWER(m.name) = :q
          OR LOWER(m.generic_name) = :q
          OR LOWER(m.name_ru) = :q
        THEN 1.0

        WHEN LOWER(m.name) LIKE '%' || :q || '%'
          OR LOWER(m.generic_name) LIKE '%' || :q || '%'
          OR LOWER(m.name_ru) LIKE '%' || :q || '%'
        THEN 0.7

        WHEN EXISTS (
            SELECT 1 FROM medicine_synonyms s
            WHERE s.medicine_id = m.id
              AND LOWER(s.synonym) LIKE '%' || :q || '%'
        )
        THEN 0.65

        ELSE GREATEST(
            similarity(m.name, :q),
            COALESCE(similarity(m.generic_name, :q), 0.0),
            COALESCE(similarity(m.name_ru, :q), 0.0)
        )
    END AS score
FROM medicines m
LEFT JOIN categories c ON c.id = m.category_id
WHERE
    LOWER(m.name) LIKE '%' || :q || '%'
    OR LOWER(m.generic_name) LIKE '%' || :q || '%'
    OR LOWER(m.name_ru) LIKE '%' || :q || '%'
    OR EXISTS (
        SELECT 1 FROM medicine_synonyms s
        WHERE s.medicine_id = m.id
          AND LOWER(s.synonym) LIKE '%' || :q || '%'
    )
    OR similarity(m.name, :q) > 0.3
    OR similarity(m.generic_name, :q) > 0.3
    OR similarity(m.name_ru, :q) > 0.3
ORDER BY score DESC
LIMIT 20;
```

**Parameter:** `:q` = normalized query string (lowercase, trimmed). Use parameterized query — never string interpolation.

---

## 5. C# Interface

```csharp
// DrugstoreSystem.Application/Interfaces/ISearchRepository.cs
public interface ISearchRepository
{
    Task<IReadOnlyList<MedicineCandidateDto>> FindMedicinesAsync(string query, CancellationToken ct = default);
}

// DrugstoreSystem.Application/DTOs/MedicineCandidateDto.cs
public record MedicineCandidateDto(
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

---

## 6. Full Search Service Flow

```csharp
// DrugstoreSystem.Application/Services/SearchService.cs
public async Task<IReadOnlyList<SearchResultDto>> SearchAsync(SearchRequest request)
{
    var query = request.Query.Trim().ToLower();
    if (query.Length < 2) return [];

    // Stage 1–4: find matching medicines (ranked)
    var medicines = await _searchRepo.FindMedicinesAsync(query);

    var results = new List<SearchResultDto>();
    foreach (var medicine in medicines)
    {
        // Stage 5: get available pharmacies
        var pharmacies = await _inventoryRepo.GetAvailablePharmaciesAsync(medicine.Id);

        // Apply optimization algorithm (Haversine or price sort)
        var ranked = _optimizer.Rank(pharmacies, request.UserLat, request.UserLng, request.SortMode);

        results.Add(new SearchResultDto(medicine, ranked));
    }

    return results;
}
```

---

## 7. Search Request / Result DTOs

```csharp
public record SearchRequest(
    string Query,
    double? UserLatitude,
    double? UserLongitude,
    SortMode SortMode = SortMode.Distance
);

public record SearchResultDto(
    MedicineCandidateDto Medicine,
    IReadOnlyList<RankedPharmacyDto> Pharmacies
);

public record RankedPharmacyDto(
    int PharmacyId,
    string PharmacyName,
    string Address,
    string? Phone,
    string? WorkingHours,
    decimal Price,
    int Quantity,
    double? DistanceKm,   // null if user location unknown
    string MapsUrl        // Google Maps redirect
);
```

---

## 8. Score Interpretation (for UI display)

| Score range | What it means | UI hint |
|---|---|---|
| 1.0 | Exact match | Show medicine name as-is |
| 0.7 | Name contains query | — |
| 0.65 | Synonym match | Optionally show "shuningdek: [synonym]" |
| 0.3–0.69 | Fuzzy match | Optionally show "Siz qidirdingizmi: [name]?" |

---

## 9. Edge Cases

| Case | Behavior |
|---|---|
| Query length < 2 | Return empty immediately (no DB hit) |
| No medicine found | Return empty list; UI shows "Dori topilmadi" |
| Medicine found, no available pharmacy | Return medicine with empty pharmacy list; show "Bu dori hozirda mavjud emas" |
| User location not available | `DistanceKm = null`; if `SortMode = Distance`, fall back to `SortMode.Price` silently |
| All pharmacies out of stock | Same as "no available pharmacy" case |

---

## 10. Performance Notes

- GIN trigram indexes make `similarity()` and `LIKE` queries fast for the demo dataset (~40 medicines, ~10 pharmacies)
- The `LIMIT 20` cap prevents excessive results
- For the demo, total search latency (DB query + Haversine for all pharmacies) should be < 200ms
- In production, consider caching `GetAvailablePharmaciesAsync` results for popular medicines

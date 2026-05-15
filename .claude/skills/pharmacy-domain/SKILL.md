---
name: pharmacy-domain
description: Domain knowledge for DrugstoreSystem — medicine naming conventions, pharmacy data, pg_trgm setup, shared catalog patterns. Open before touching entities, medicines, or pharmacy data.
---

# Pharmacy Domain — DrugstoreSystem

---

## 1. Medicine Naming Reality (O'zbekiston context)

Medicines in Uzbekistan are commonly referred to by **multiple names simultaneously**:

| Name type | Example | Stored in |
|---|---|---|
| Official/generic | Paracetamol | `medicines.name` |
| INN (International) | Acetaminophen | `medicines.generic_name` |
| Russian name | Парацетамол | `medicines.name_ru` |
| Trade/brand name | Panadol, Para-500, Tylenol | `medicine_synonyms` |

**Why `name_ru` matters:** Pharmacists and patients in Uzbekistan overwhelmingly use Russian names in conversation (Soviet medical legacy). "Analgin" (Russian trade name) is more recognized than "Metamizol" (INN). The search system MUST handle Russian names.

**Why synonyms matter:** "Panadol" and "Paracetamol" are the same medicine but a patient may only know the brand name. Synonyms bridge this gap.

---

## 2. Medicine Dosage Forms

Standard values for the `DosageForm` enum:

| Value | Uzbek label | Notes |
|---|---|---|
| `Tablet` | Tabletka | Most common |
| `Capsule` | Kapsul | |
| `Syrup` | Sirop | Liquid, especially for children |
| `Injection` | Ukol / Inyeksiya | Requires pharmacy storage protocol |
| `Cream` | Krem | Topical |
| `Drops` | Tomchi | Eye, ear, or nasal |
| `Powder` | Kukuncha | Often sachets (Smecta, etc.) |
| `Suppository` | Sham | Rectal/vaginal |
| `Patch` | Plaster | Transdermal |
| `Solution` | Eritma | Antiseptic liquids, etc. |

---

## 3. Pharmacy Data Rules

**Coordinates:** Always `double precision`. Tashkent bounding box:
- Latitude: 41.15 – 41.45
- Longitude: 69.10 – 69.45

When helping pharmacist enter coordinates, remind them: "Google Mapsda dorixona ustiga o'ng tugma bosib, koordinatalarni nusxalang."

**Working hours:** Free-form string (`varchar(200)`). Acceptable formats:
- `"09:00–22:00 (Har kuni)"`
- `"Ish kunlari: 08:00–20:00, Dam olish: 09:00–18:00"`

No structured time parsing — this is display-only.

**`is_active` vs `is_verified`:**
- `is_active` — Admin can toggle this; inactive pharmacies are hidden from search results
- `is_verified` — Admin marks pharmacy as verified (informational badge); does NOT affect search

---

## 4. Shared Catalog Pattern

The `medicines` table is a **shared, crowdsourced catalog**. Key rules:

1. **First-adder creates**: When a pharmacist adds a medicine not yet in the catalog, a new `Medicine` row is created. `created_by_pharmacy_id` is set to that pharmacist's pharmacy.
2. **Others inherit**: All subsequent pharmacists see this medicine in their autocomplete and can add it to their inventory with their own price/quantity.
3. **No admin gate**: Admin does not approve or create medicines. The catalog grows organically.
4. **Deduplication responsibility**: `IMedicineService.AutocompleteAsync` uses trigram similarity to surface near-duplicates before a pharmacist creates a new entry — reducing "Paracetamol" vs "Paratsetamol" duplicate entries.

---

## 5. PharmacyMedicine (Inventory) Rules

- `UNIQUE (pharmacy_id, medicine_id)` — a pharmacy can have at most one price/quantity per medicine. Use `UpsertAsync` (INSERT ... ON CONFLICT DO UPDATE).
- `quantity >= 0` — enforced by check constraint. Never negative.
- `updated_at` — must be refreshed on every price or quantity change.
- **Availability = `quantity > 0`** — this is the only availability check. There is no separate `is_available` flag.

---

## 6. pg_trgm Setup Checklist

Before DEV-07 search works, this must be in place:

```sql
-- 1. Extension enabled (in migration)
CREATE EXTENSION IF NOT EXISTS pg_trgm;

-- 2. GIN indexes (in migration)
CREATE INDEX ix_medicines_name_trgm ON medicines USING GIN (name gin_trgm_ops);
CREATE INDEX ix_medicines_generic_name_trgm ON medicines USING GIN (generic_name gin_trgm_ops);
CREATE INDEX ix_medicines_name_ru_trgm ON medicines USING GIN (name_ru gin_trgm_ops);
CREATE INDEX ix_synonyms_trgm ON medicine_synonyms USING GIN (synonym gin_trgm_ops);
```

Verify after migration:
```sql
SELECT * FROM pg_extension WHERE extname = 'pg_trgm';
SELECT indexname FROM pg_indexes WHERE tablename = 'medicines';
```

If GIN indexes are missing, `similarity()` queries work but are slow (full table scan).

---

## 7. EF Core Notes for this Domain

- `UseSnakeCaseNamingConvention()` is applied globally — column `PharmacyId` → `pharmacy_id`
- `owned` types are NOT used — every entity has its own table
- The GIN indexes CANNOT be created via EF Fluent API — use `migrationBuilder.Sql(...)` in the migration
- `PharmacyMedicine.UpdatedAt` should be set in `InventoryRepository.UpsertAsync`, not via EF interceptor (keep it simple)

---

## 8. Google Maps URL

The redirect URL for a pharmacy's location:
```
https://www.google.com/maps/search/?api=1&query={latitude},{longitude}
```

This format:
- Works without an API key
- Opens Google Maps in a new tab
- Shows a red pin at the exact coordinates
- Works on mobile (opens the Maps app)

Build in `PharmacyRanker.BuildMapsUrl(double lat, double lng)`.

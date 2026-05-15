# Database Schema — DrugstoreSystem

All tables use **snake_case** column names (EF Core `UseSnakeCaseNamingConvention()`). PostgreSQL 16+. The `pg_trgm` extension must be enabled before running the initial migration.

```sql
CREATE EXTENSION IF NOT EXISTS pg_trgm;
```

---

## 1. Entity Overview

```
categories          ──< medicines >── medicine_synonyms
                           │
                    pharmacy_medicines >── pharmacies
                                               │
                                         asp_net_users (pharmacist)
```

---

## 2. Tables

### 2.1 `categories`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `name` | `varchar(100)` | NOT NULL, UNIQUE |

Seed values: Analgetik, Antibiotik, Antivirusli, Kardio, Enterosorbent, Vitamini, Spazmolitik, Ko'z tomchilari, Tashqi (Krem/Malham), Boshqa.

---

### 2.2 `medicines` (shared catalog)

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `name` | `varchar(200)` | NOT NULL |
| `generic_name` | `varchar(200)` | nullable |
| `name_ru` | `varchar(200)` | nullable — Russian trade/generic name |
| `dosage_form` | `varchar(100)` | nullable — see enum below |
| `category_id` | `integer` | FK → `categories.id`, nullable |
| `manufacturer` | `varchar(200)` | nullable |
| `description` | `text` | nullable |
| `created_at` | `timestamptz` | NOT NULL, DEFAULT NOW() |
| `created_by_pharmacy_id` | `integer` | FK → `pharmacies.id`, nullable — who first added this medicine |

**DosageForm enum values:** Tablet, Capsule, Syrup, Injection, Cream, Drops, Powder, Suppository, Patch, Solution.

**Indexes:**
```sql
CREATE INDEX ix_medicines_name_trgm        ON medicines USING GIN (name gin_trgm_ops);
CREATE INDEX ix_medicines_generic_name_trgm ON medicines USING GIN (generic_name gin_trgm_ops);
CREATE INDEX ix_medicines_name_ru_trgm     ON medicines USING GIN (name_ru gin_trgm_ops);
CREATE INDEX ix_medicines_category         ON medicines (category_id);
```

---

### 2.3 `medicine_synonyms`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `medicine_id` | `integer` | FK → `medicines.id` ON DELETE CASCADE |
| `synonym` | `varchar(200)` | NOT NULL |

**Indexes:**
```sql
CREATE INDEX ix_medicine_synonyms_trgm      ON medicine_synonyms USING GIN (synonym gin_trgm_ops);
CREATE INDEX ix_medicine_synonyms_medicine  ON medicine_synonyms (medicine_id);
```

Examples:
- Paracetamol → ["Panadol", "Tylenol", "Para-500", "Парацетамол"]
- Ibuprofen → ["Nurofen", "Advil", "Ибупрофен"]
- Metamizol → ["Analgin", "Анальгин"]
- Drotaverine → ["No-Spa", "Но-шпа"]

---

### 2.4 `pharmacies`

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `name` | `varchar(200)` | NOT NULL |
| `address` | `varchar(500)` | NOT NULL |
| `latitude` | `double precision` | NOT NULL |
| `longitude` | `double precision` | NOT NULL |
| `phone` | `varchar(50)` | nullable |
| `working_hours` | `varchar(200)` | nullable — e.g., "09:00–22:00 (Har kuni)" |
| `is_active` | `boolean` | NOT NULL, DEFAULT true |
| `is_verified` | `boolean` | NOT NULL, DEFAULT false |
| `created_at` | `timestamptz` | NOT NULL, DEFAULT NOW() |

**Indexes:**
```sql
CREATE INDEX ix_pharmacies_active ON pharmacies (is_active) WHERE is_active = true;
```

---

### 2.5 `pharmacy_medicines` (inventory)

| Column | Type | Constraints |
|---|---|---|
| `id` | `integer` | PK, auto-increment |
| `pharmacy_id` | `integer` | FK → `pharmacies.id` ON DELETE CASCADE |
| `medicine_id` | `integer` | FK → `medicines.id` ON DELETE CASCADE |
| `price` | `numeric(10,2)` | NOT NULL — price in UZS (sum) |
| `quantity` | `integer` | NOT NULL, DEFAULT 0, CHECK >= 0 |
| `updated_at` | `timestamptz` | NOT NULL, DEFAULT NOW() |

**Constraints & Indexes:**
```sql
ALTER TABLE pharmacy_medicines ADD CONSTRAINT ux_pharmacy_medicine UNIQUE (pharmacy_id, medicine_id);

-- For search queries: availability filter + price sort
CREATE INDEX ix_pm_medicine_available ON pharmacy_medicines (medicine_id, price)
  WHERE quantity > 0;
```

---

### 2.6 `asp_net_users` (ASP.NET Core Identity + extension)

Standard Identity columns plus:

| Column | Type | Constraints |
|---|---|---|
| `pharmacy_id` | `integer` | FK → `pharmacies.id`, nullable — NULL for Admin, set for Pharmacist |

**Roles table:** Standard `asp_net_roles`. Seeded values: `Admin`, `Pharmacist`.

---

## 3. Relationships Summary

| Relationship | Type |
|---|---|
| Category → Medicine | One-to-Many (optional) |
| Medicine → MedicineSynonym | One-to-Many (cascade delete) |
| Pharmacy → PharmacyMedicine | One-to-Many (cascade delete) |
| Medicine → PharmacyMedicine | One-to-Many (cascade delete) |
| Pharmacy → AppUser | One-to-One (pharmacist) |
| Medicine → Pharmacy (creator) | Many-to-One (optional, informational) |

---

## 4. Key Query Patterns

### 4.1 Search query (simplified)
```sql
SELECT m.id, m.name, m.generic_name, m.name_ru,
       CASE
         WHEN LOWER(m.name) = :q OR LOWER(m.generic_name) = :q OR LOWER(m.name_ru) = :q THEN 1.0
         WHEN LOWER(m.name) LIKE '%'||:q||'%'
           OR LOWER(m.generic_name) LIKE '%'||:q||'%'
           OR LOWER(m.name_ru) LIKE '%'||:q||'%' THEN 0.7
         WHEN EXISTS (
           SELECT 1 FROM medicine_synonyms s
           WHERE s.medicine_id = m.id AND LOWER(s.synonym) LIKE '%'||:q||'%'
         ) THEN 0.65
         ELSE GREATEST(
           similarity(m.name, :q),
           COALESCE(similarity(m.generic_name, :q), 0),
           COALESCE(similarity(m.name_ru, :q), 0)
         )
       END AS score
FROM medicines m
WHERE
     LOWER(m.name) LIKE '%'||:q||'%'
  OR LOWER(m.generic_name) LIKE '%'||:q||'%'
  OR LOWER(m.name_ru) LIKE '%'||:q||'%'
  OR EXISTS (SELECT 1 FROM medicine_synonyms s WHERE s.medicine_id = m.id AND LOWER(s.synonym) LIKE '%'||:q||'%')
  OR similarity(m.name, :q) > 0.3
  OR similarity(m.generic_name, :q) > 0.3
HAVING score > 0.0
ORDER BY score DESC
LIMIT 20;
```

### 4.2 Available pharmacies for a medicine
```sql
SELECT pm.price, pm.quantity, p.*
FROM pharmacy_medicines pm
JOIN pharmacies p ON p.id = pm.pharmacy_id
WHERE pm.medicine_id = :medicineId
  AND pm.quantity > 0
  AND p.is_active = true;
```

### 4.3 Medicine autocomplete (pharmacist panel)
```sql
SELECT id, name, generic_name, name_ru, dosage_form
FROM medicines
WHERE LOWER(name) LIKE '%'||:q||'%'
   OR LOWER(generic_name) LIKE '%'||:q||'%'
   OR similarity(name, :q) > 0.2
ORDER BY similarity(name, :q) DESC
LIMIT 10;
```

---

## 5. Migration Strategy

1. Initial migration: all tables + indexes + `CREATE EXTENSION IF NOT EXISTS pg_trgm`
2. GIN indexes created in the migration (not via EF fluent API — use `migrationBuilder.Sql(...)`)
3. `database.MigrateAsync()` runs at startup in dev mode
4. Seed runs after migration when `pharmacies` table is empty

---

## 6. Naming Conventions

- Tables: plural, snake_case — `pharmacies`, `medicines`, `pharmacy_medicines`
- Columns: snake_case — `created_at`, `medicine_id`, `is_active`
- Foreign keys: `{table_singular}_id`
- Indexes: `ix_{table}_{column}` or `ix_{table}_{column}_trgm` for GIN trigram indexes
- Unique constraints: `ux_{table}_{columns}`

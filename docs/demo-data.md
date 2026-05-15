# Demo Data — DrugstoreSystem

Seed data for the thesis defense demo. The `DatabaseSeeder` runs on startup when the `pharmacies` table is empty.

---

## 1. Categories (10)

```json
["Analgetik", "Antibiotik", "Antivirusli", "Kardio", "Enterosorbent",
 "Vitamini", "Spazmolitik", "Ko'z tomchilari", "Tashqi (Krem/Malham)", "Boshqa"]
```

---

## 2. Pharmacies (8)

Real Tashkent + regional coordinates for meaningful Haversine demo.

| # | Name | Address | Lat | Lng |
|---|---|---|---|---|
| 1 | Shifo Dorixona | Toshkent, Chilonzor tumani, Bunyodkor ko'ch. 12 | 41.2995 | 69.2401 |
| 2 | Dori-Darmon Plus | Toshkent, Yunusobod tumani, Amir Temur shoh ko'ch. 108 | 41.3375 | 69.3019 |
| 3 | Salomatlik Apteka | Toshkent, Mirzo Ulug'bek tumani, Bog'ishamol ko'ch. 56 | 41.3191 | 69.3018 |
| 4 | Al-Farabi Dorixona | Toshkent, Shayxontohur tumani, Navoi ko'ch. 30 | 41.3169 | 69.2619 |
| 5 | Hayot Apteka | Toshkent, Sergeli tumani, Yangi Sergeli ko'ch. 5 | 41.2398 | 69.2100 |
| 6 | Baraka Dorixona | Toshkent, Uchtepa tumani, Mustaqillik ko'ch. 44 | 41.3011 | 69.2169 |
| 7 | Sog'lom Hayot | Samarqand sh., Registon ko'ch. 2 | 39.6542 | 66.9597 |
| 8 | Farg'ona Apteka | Farg'ona sh., Al-Farg'oniy ko'ch. 15 | 40.3764 | 71.7971 |

All pharmacies: `is_active = true`, `is_verified = true`.
Working hours: "09:00–22:00 (Har kuni)" for all (simplifies demo).

---

## 3. Medicines (30 — shared catalog)

Core medicines with Uzbek-relevant Russian names and synonyms for search algorithm demo.

| # | Name | GenericName | NameRu | Category | Dosage Form | Synonyms |
|---|---|---|---|---|---|---|
| 1 | Paracetamol | Acetaminophen | Парацетамол | Analgetik | Tablet | Panadol, Tylenol, Para-500 |
| 2 | Ibuprofen | Ibuprofen | Ибупрофен | Analgetik | Tablet | Nurofen, Advil, Ibufen |
| 3 | Analgin | Metamizol | Анальгин | Analgetik | Tablet | Baralgin |
| 4 | Aspirin | Acetylsalicylic acid | Аспирин | Analgetik | Tablet | Cardiomagnil |
| 5 | Citramon | Paracetamol+Caffeine | Цитрамон | Analgetik | Tablet | — |
| 6 | No-Spa | Drotaverine | Но-шпа | Spazmolitik | Tablet | Spazmol |
| 7 | Amoxicillin | Amoxicillin | Амоксициллин | Antibiotik | Capsule | Flemoxin, Ospamox |
| 8 | Azithromycin | Azithromycin | Азитромицин | Antibiotik | Tablet | Sumamed, Azitro |
| 9 | Cefazolin | Cefazolin | Цефазолин | Antibiotik | Injection | — |
| 10 | Metronidazole | Metronidazole | Метронидазол | Antibiotik | Tablet | Flagyl, Metrogyl |
| 11 | Acyclovir | Acyclovir | Ацикловир | Antivirusli | Tablet | Zovirax |
| 12 | Arbidol | Umifenovir | Арбидол | Antivirusli | Capsule | — |
| 13 | Validol | Menthol | Валидол | Kardio | Tablet | — |
| 14 | Corvalol | Phenobarbital+Ethyl | Корвалол | Kardio | Drops | — |
| 15 | Amlodipine | Amlodipine | Амлодипин | Kardio | Tablet | Norvasc |
| 16 | Activated Carbon | Activated Charcoal | Активированный уголь | Enterosorbent | Tablet | Ko'mir tabletka |
| 17 | Smecta | Diosmectite | Смекта | Enterosorbent | Powder | — |
| 18 | Enterosgel | Polymethylsiloxane | Энтеросгель | Enterosorbent | Gel | — |
| 19 | Vitamin C | Ascorbic acid | Витамин С | Vitamini | Tablet | Askorutin |
| 20 | Vitamin D3 | Cholecalciferol | Витамин Д3 | Vitamini | Drops | Aquadetrim |
| 21 | Suprastin | Chloropyramine | Супрастин | Boshqa | Tablet | — |
| 22 | Loratadine | Loratadine | Лоратадин | Boshqa | Tablet | Claritin, Claritine |
| 23 | Omeprazole | Omeprazole | Омепразол | Boshqa | Capsule | Losek |
| 24 | Mezim | Pancreatin | Мезим | Boshqa | Tablet | — |
| 25 | Bisacodyl | Bisacodyl | Бисакодил | Boshqa | Tablet | Dulcolax |
| 26 | Diclofenac | Diclofenac | Диклофенак | Analgetik | Tablet | Voltaren |
| 27 | Chlorhexidine | Chlorhexidine | Хлоргексидин | Tashqi | Solution | — |
| 28 | Panthenol | Dexpanthenol | Пантенол | Tashqi | Cream | D-Panthenol |
| 29 | Tobramycin eye drops | Tobramycin | Тобрамицин | Ko'z | Drops | Tobrex |
| 30 | Artificial Tears | Hydroxypropyl | Искусственные слезы | Ko'z | Drops | Visine |

---

## 4. Pharmacy Inventories

Each pharmacy stocks ~15–20 medicines at varying prices and quantities. Prices are in UZS (Uzbek sum).

**Design principle for demo:** Prices differ between pharmacies for the same medicine — this makes the price-sort feature meaningful. Quantities vary so some pharmacies are "out of stock" for certain medicines.

**Sample inventory (Shifo Dorixona — Pharmacy #1):**

| Medicine | Price (UZS) | Qty |
|---|---|---|
| Paracetamol | 3,500 | 120 |
| Ibuprofen | 8,200 | 45 |
| Analgin | 4,100 | 80 |
| No-Spa | 12,000 | 30 |
| Amoxicillin | 22,500 | 25 |
| Vitamin C | 6,800 | 200 |
| Activated Carbon | 2,100 | 150 |
| Smecta | 18,500 | 20 |
| Suprastin | 7,300 | 60 |
| Omeprazole | 14,200 | 35 |
| Diclofenac | 9,600 | 40 |
| Aspirin | 3,200 | 90 |
| Loratadine | 11,000 | 28 |
| Validol | 5,500 | 50 |
| Chlorhexidine | 4,800 | 70 |

**Sample inventory (Dori-Darmon Plus — Pharmacy #2):**

Same medicines, different prices (±10–30% variance) to demonstrate price sorting. Some medicines out of stock (qty=0) to demo availability filtering.

---

## 5. Seed Admin Account

| Field | Value |
|---|---|
| Email | `admin@drugstore.local` |
| Password | from `Seed:AdminPassword` user-secret (default: `Admin@123!`) |
| Role | Admin |

---

## 6. Seeder Guard

```csharp
// DatabaseSeeder.cs
public static async Task SeedAsync(IServiceProvider services)
{
    var context = services.GetRequiredService<DrugstoreDbContext>();

    // Only seed if empty
    if (await context.Pharmacies.AnyAsync()) return;

    // 1. Seed categories
    // 2. Seed medicines + synonyms (from medicines.json)
    // 3. Seed pharmacies (from pharmacies.json)
    // 4. Seed pharmacy_medicines
    // 5. Seed admin account via UserManager
}
```

Running `dotnet ef database update` + app start = fully populated database. Re-running is idempotent.

---

## 7. Demo Script (for defense)

1. Open `https://localhost:5XXX` (or http://localhost:5XXX)
2. Allow geolocation (Tashkent coordinates auto-detected)
3. Search "para" → Paracetamol appears (partial match demo)
4. Search "paratsetamol" → Paracetamol appears (fuzzy/typo demo)
5. Search "Парацетамол" → Paracetamol appears (Russian name demo)
6. Toggle sort "Narx bo'yicha" → observe reordering
7. Click pharmacy name → Pharmacy detail with "Xaritada ko'rish" button
8. Click medicine name → Medicine detail with synonym list
9. Login as admin → show pharmacy management
10. Login as pharmacist → show inventory + autocomplete add medicine

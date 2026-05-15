---
name: thesis-structure
description: Full BMI chapter structure for "Dorixonalardan dori buyurtma berishning eng yaqin yechimini topib beruvchi dasturiy vositasini ishlab chiqish". Read before drafting any thesis chapter.
---

# BMI Structure — DrugstoreSystem

**Student:** Sharipova Moxichexra Oltinovna
**Group:** M313-22 Dlo'
**Institution:** Alfraganus universiteti, Raqamli texnologiyalari kafedrasi
**Supervisor:** Hamroev Alisher Shodmonqulovich, dotsent
**Topic (UZ):** Dorixonalardan dori buyurtma berishning eng yaqin yechimini topib beruvchi dasturiy vositasini ishlab chiqish

---

## Overall Structure

```
1. TITUL VARAQ                           (1 bet)
2. ANNOTATSIYA (uzb/rus/eng)             (3 bet)
3. MUNDARIJA                             (1–2 bet)
4. KIRISH                                (5–6 bet)
5. I BOB — NAZARIY ASOSLAR              (13–16 bet)
   1.1. Zamonaviy dorixona tizimlari     (4–5 bet)
   1.2. Qidiruv va optimallashtirish     (4–5 bet)
   1.3. Mavjud platformalar tahlili      (5–6 bet)
6. II BOB — LOYIHALASH                   (12–15 bet)
   2.1. Arxitektura va texnologiyalar    (6–7 bet)
   2.2. Ma'lumotlar bazasi               (6–8 bet)
7. III BOB — AMALIY TATBIQ              (9–11 bet)
   3.1. Tizim imkoniyatlari              (5–6 bet)
   3.2. Algoritmlar samaradorligi        (4–5 bet)
8. IV BOB — HAYOT XAVFSIZLIGI           (8–10 bet)
   4.1. Ergonomika                       (4–5 bet)
   4.2. Yong'in xavfsizligi             (4–5 bet)
9. XULOSA                                (2–3 bet)
10. FOYDALANILGAN ADABIYOTLAR           (2–3 bet, 14–20 manba)
11. DASTUR ILOVASI                       (3–7 bet, kod)

JAMI: ~55–70 bet
```

---

## ANNOTATSIYA (3 tilda, har biri 200–250 so'z)

**Mazmun (har tilda):**
- Mavzu va dolzarblik (2 jumla)
- Maqsad: dorixonalardan dori qidirish va optimal dorixona topish tizimi
- Texnologiyalar: .NET 10, Blazor Server, PostgreSQL, pg_trgm, Haversine
- Natija: qidiruv algoritmi va geodezik masofa algoritmi ishlab chiqildi
- Kalit so'zlar (5–7): dorixona tizimi, fuzzy qidiruv, pg_trgm, Haversine formulasi, Blazor Server, PostgreSQL, optimallashtirish

---

## KIRISH (5–6 bet)

### Kirish paragraflari (2–3 paragraf, ~300 so'z)
- Farmatsevtika sohasida raqamlashtirish zarurati
- Dori qidirish muammosi: dorixonalar orasida optimal tanlash
- Axborot texnologiyalarining yechim sifatidagi o'rni

### "Bitiruv malakaviy ishining dolzarbligi:"
2 paragraf (~400 so'z):
- "Raqamli O'zbekiston – 2030" strategiyasi va sog'liqni saqlash sohasini raqamlashtirish
- Dori qidirishda vaqt va masofa muammosi (bemorlar bir nechta dorixonani aylanib chiqishi)
- Mavjud mahalliy platformalarning cheklovlari
- AI va algoritmlarga asoslangan yechimlarning iqtisodiy samarasi

### "Bitiruv malakaviy ishining maqsadi:"
1 umumiy maqsad + 5 ta jihat (bullet):
> "Ushbu bitiruv malakaviy ishining asosiy maqsadi — dorixonalardan dori buyurtma berishda eng yaqin va qulay yechimni avtomatik topib beruvchi dasturiy vositasini ishlab chiqishdan iborat."

Jihatlar:
- Dorixona va dori ma'lumotlari bazasini boshqarish tizimini yaratish
- Ko'p bosqichli fuzzy qidiruv algoritmini (pg_trgm) ishlab chiqish
- Haversine formulasi asosida geodezik masofa hisoblash modulini yaratish
- Foydalanuvchi joylashuvi asosida optimal dorixonani aniqlash imkonini berish
- Farmatsevtlar uchun umumiy dori katalogini boshqarish interfeysini ishlab chiqish

### "Bitiruv malakaviy ishining vazifalari:"
7–8 bullet:
- Dorixona axborot tizimlarining nazariy asoslarini o'rganish
- Fuzzy qidiruv va geodezik masofani hisoblash algoritmlarini tahlil qilish
- Mavjud dorixona platformalarini qiyosiy tahlil qilish
- Tizim arxitekturasini loyihalash (Clean Architecture)
- PostgreSQL va pg_trgm yordamida qidiruv modulini ishlab chiqish
- Haversine formulasi asosida optimallashtirish modulini yaratish
- MudBlazor asosida foydalanuvchi interfeysini ishlab chiqish
- Tizimni sinovdan o'tkazish va natijalarni tahlil qilish

### "Bitiruv malakaviy ishining tuzilishi:"
Har bob uchun 1 jumla.

---

## I BOB — NAZARIY ASOSLAR

**Bob nomi:** "DORIXONA AXBOROT TIZIMLARINI RAQAMLASHTIRISH VA OPTIMALLASHTIRISH ALGORITMLARINING NAZARIY ASOSLARI"

### 1.1. "Zamonaviy dorixona boshqaruv tizimlari va farmatsevtika sohasini raqamlashtirish" (4–5 bet)

Paragraph bloklari:
1. Kirish — farmatsevtika sohasida AT ning ahamiyati
2. Dorixona tizimlarining evolyutsiyasi — qo'lda, kompyuterlashtirish, onlayn platformalar
3. Multi-pharmacy platformalar — bir nechta dorixonani birlashtiruvchi tizimlar
4. Foydalanuvchi tomonlari — mijoz (public search) va farmatsevt (inventory)
5. Shared catalog konsepsiyasi — umumiy dori bazasi qanday ishlaydi
6. O'zbekiston konteksti (MAJBURIY) — apteka.uz, mahalliy dorixona zanjirlari, "Raqamli O'zbekiston 2030"
7. Muammolar — cheklovlar (internet yo'q, qidiruv aniq emas, koordinatlar noto'g'ri)
8. Xulosa paragrafi

**Rasm:** 1.1.1-rasm — dorixona tizimi arxitekturasi diagrammasi (uchburchak: Dorixona → Platforma → Mijoz)

---

### 1.2. "Qidiruv algoritmlari va geodezik masofani hisoblash nazariy asoslari" (4–5 bet)

Paragraph bloklari:
1. Kirish — nima uchun oddiy LIKE qidiruv yetarli emas
2. Trigram qidiruv (pg_trgm) — ta'rif, qanday ishlaydi, o'xshashlik koeffitsienti
3. Fuzzy search yo'nalishlari — Levenshtein, BK-tree, trigram (solishtiruv)
4. Ko'p bosqichli qidiruv — exact → partial → synonym → trigram → ranking
5. Geodezik masofa va Haversine formulasi — matematik ta'rif, Yer sferasi, formula
6. Haversine vs Euclidean — nima farqi bor, nima uchun Haversine to'g'riroq
7. Optimallashtirish mezonlari — masofa yoki narx bo'yicha saralash
8. O'zbekiston konteksti — O'zbekistonda dori nomlari ko'p tillilik muammosi (o'zbek/rus/ingliz)
9. Xulosa paragrafi

**Rasmlar:**
- 1.2.1-rasm — trigram qidiruv jarayoni diagrammasi (string → trigrams → similarity)
- 1.2.2-rasm — Haversine formulasi vizualizatsiyasi (Yer sferasi + ikkita nuqta)

---

### 1.3. "Mavjud dorixona platformalari va qidiruv tizimlarining qiyosiy tahlili" (5–6 bet)

Paragraph bloklari:
1. Kirish — qiyosiy tahlil zarurati
2. Xalqaro platformalar (har biri 1 paragraf):
   - GoodRx (AQSh) — narx solishtiruvi
   - DocMorris (Evropa) — onlayn dorixona
   - Tabletki.ua (Ukraina) — dorixona qidiruv
   - Apteka.ru / iApteka (Rossiya) — geolokatsiya asosida qidiruv
3. Mahalliy platformalar:
   - apteka.uz (O'zbekiston)
   - OLX.uz Sog'liqni saqlash bo'limi
4. Qiyosiy jadval (1.3.1-jadval) — 5 parametr:
   - Platforma | Fuzzy qidiruv | Geolokatsiya | Ko'p dorixona | Ochiq katalog
5. Bizning tizimning afzalligi — bo'shliqni to'ldiradi
6. Xulosa paragrafi

**Jadval:** 1.3.1-jadval — Mavjud platformalarning qiyosiy tahlili
**Rasm:** 1.3.1-rasm — tabletki.ua interfeysi (internetdan)

---

## II BOB — LOYIHALASH VA ISHLAB CHIQISH

**Bob nomi:** "DORIXONA QIDIRUV TIZIMINI LOYIHALASH VA ISHLAB CHIQISH"

### 2.1. "Tizim arxitekturasi va zaruriy texnologiyalarni tanlash" (6–7 bet)

Paragraph bloklari:
1. Kirish — arxitektura tanlashning ahamiyati
2. Clean Architecture — 4 qatlam, dependency rule
3. Har texnologiya uchun "nega tanladim" paragrafi:
   - .NET 10 + C# — 1 paragraf
   - Blazor Server — 1 paragraf (nega WASM emas)
   - MudBlazor — 1 paragraf
   - ASP.NET Core Identity — 1 paragraf
   - Entity Framework Core — 1 paragraf
   - PostgreSQL — 1 paragraf (nega pg_trgm uchun PostgreSQL tanladim)
   - pg_trgm extension — 1 paragraf (bu loyihaning asosiy texnik tanlovlaridan biri)
4. Xulosa paragrafi

**Rasmlar (5 ta):**
- 2.1.1-rasm — Solution Explorer [USER SCREENSHOT]
- 2.1.2-rasm — Arxitektura diagrammasi [draw.io]
- 2.1.3-rasm — Blazor komponentlar ko'rinishi [USER SCREENSHOT]
- 2.1.4-rasm — Qidiruv algoritmi oqimi diagrammasi [draw.io]
- 2.1.5-rasm — Haversine formulasi kodi [USER SCREENSHOT]

---

### 2.2. "Ma'lumotlar bazasini loyihalash va integratsiyalash" (6–8 bet)

Paragraph bloklari:
1. Kirish — DB loyihalash ahamiyati
2. Code-First yondashuvi — 1 paragraf
3. PostgreSQL + pg_trgm tanlash sababi — trigram indekslar
4. Asosiy jadvallar tavsifi (har biri 1–2 paragraf):
   - `medicines` — umumiy dori katalogi
   - `medicine_synonyms` — sinonimlar jadvali
   - `categories` — dori kategoriyalari
   - `pharmacies` — dorixonalar
   - `pharmacy_medicines` — inventar (narx, miqdor)
   - `asp_net_users` — foydalanuvchilar (Admin, Farmatsevt)
5. GIN indekslar va pg_trgm setup — 1 paragraf
6. EF Core migrations — 1 paragraf
7. Shared catalog pattern — 1 paragraf (crowdsourced catalog)
8. Ma'lumotlar xavfsizligi — 1 paragraf
9. Xulosa paragrafi

**Rasmlar (6 ta):**
- 2.2.1-rasm — ER diagramma [dbdiagram.io]
- 2.2.2-rasm — Ulanish satri konfiguratsiyasi [USER SCREENSHOT]
- 2.2.3-rasm — DbContext sinfi [USER SCREENSHOT]
- 2.2.4-rasm — Medicine entity modeli [USER SCREENSHOT]
- 2.2.5-rasm — GIN indeks migration kodi [USER SCREENSHOT]
- 2.2.6-rasm — Migration terminal natijasi [USER SCREENSHOT]

---

## III BOB — AMALIY TATBIQ

**Bob nomi:** "LOYIHANING AMALIY TATBIQI VA ALGORITMLARINING SAMARADORLIGI"

### 3.1. "Tizim imkoniyatlari bilan tanishish" (5–6 bet)

Paragraph bloklari (har biri 1 ta screenshot bilan):
1. Kirish — tizimning umumiy imkoniyatlari
2. Login sahifasi — Admin va Farmatsevt autentifikatsiyasi
3. Admin paneli — Dashboard va dorixonalar boshqaruvi
4. Dorixona qo'shish — 2 bosqichli forma (MudStepper)
5. Farmatsevt paneli — profil tahrirlash
6. Inventar boshqaruvi — dori qo'shish (autocomplete flow)
7. Yangi dori yaratish — umumiy katalog boyishi
8. Jamoat qidiruvi — bosh sahifa (search bar)
9. Qidiruv natijalari — dorixonalar ro'yxati narx va masofa bilan
10. Dori detail sahifasi — to'liq ma'lumot
11. Dorixona detail sahifasi — "Xaritada ko'rish" tugmasi
12. Xulosa paragrafi

**Rasmlar (10+ ta):** Hammasi USER SCREENSHOT — ishlayotgan UI ning isboti.

---

### 3.2. "Qidiruv va optimallashtirish algoritmlarining samaradorligi" (4–5 bet)

Paragraph bloklari:
1. Kirish — bu bo'limning maqsadi
2. Ko'p bosqichli qidiruv algoritmi — kodni tushuntirish
3. pg_trgm o'xshashlik koeffitsienti — qanday ishlaydi, test natijalari
4. Haversine algoritmi — formulani ko'rsatish, C# implementatsiya
5. Test natijalari jadvali (3.2.1-jadval):
   - 8–10 ta test kase: qidiruv so'rovi | kutilgan natija | olingan natija | masofa (km)
6. Typo toleransining demonstratsiyasi — "paratsetamol" → Paracetamol topildi
7. Masofa hisoblashning aniqligi — Toshkent → Samarqand ~281 km
8. Xulosa paragrafi

**Rasmlar (4–5 ta):**
- 3.2.1-rasm — Qidiruv algoritmi SQL kodi [USER SCREENSHOT]
- 3.2.2-rasm — pg_trgm o'xshashlik natijasi [USER SCREENSHOT]
- 3.2.3-rasm — HaversineCalculator kodi [USER SCREENSHOT]
- 3.2.4-rasm — Unit test natijalari [USER SCREENSHOT]
- 3.2.5-rasm — Narx va masofa bo'yicha saralash natijasi [USER SCREENSHOT]

---

## IV BOB — HAYOT FAOLIYATI XAVFSIZLIGI (STANDART BOB)

Bu bob loyihaga bog'liq emas — har BMI da bor. Shablondan olinadi.

### 4.1. Ergonomika va sog'liqni saqlash (4–5 bet)
- Ergonomika ta'rifi
- "Inson — kompyuter" tizimi
- Ish joyini to'g'ri tashkil etish
- Zararli omillar (ko'z charchashi, karpal tunnel)
- Profilaktika (20-20-20 qoidasi)

**Rasm:** 4.1.1-rasm — to'g'ri o'tirish pozasi (internetdan)

### 4.2. Yong'in xavfsizligi (4–5 bet)
- O'zbekiston "Yong'in xavfsizligi" qonunidan matn
- Taqiqlangan harakatlar
- Yong'in chiqsa nima qilish kerak

**Rasm:** 4.2.1-rasm — yong'in o'chirish vositalari (internetdan)

---

## XULOSA (2–3 bet)

1. Kirish paragrafi — ish nimaga bag'ishlanganligi
2. I bob xulosa — 1 paragraf
3. II bob xulosa — 1 paragraf
4. III bob xulosa — 1 paragraf
5. IV bob xulosa — 1 paragraf
6. "Ish natijasida quyidagi asosiy xulosalar chiqarildi:" — 5 bullet:
   - Ko'p bosqichli fuzzy qidiruv algoritmi (pg_trgm) dori nomlarini to'g'ri va noto'g'ri yozilgan holda ham topib berishini isbotladi
   - Haversine formulasi asosidagi optimallashtirish algoritmi foydalanuvchiga eng yaqin dorixonani aniq ko'rsatdi
   - Umumiy dori katalogi (crowdsourced) farmatsevtlar tomonidan mustaqil boshqarilishi samarali ekanligini ko'rsatdi
   - Blazor Server + PostgreSQL kombinatsiyasi O'zbekiston shart-sharoitlari uchun optimal yechim ekanligini amaliyot ko'rsatdi
   - Kelajakda: mobil versiya, onlayn buyurtma, retsept boshqaruvi qo'shish mumkin

---

## ADABIYOTLAR (14–20 manba)

**O'zbekiston hujjatlari (4–6):**
1. O'zbekiston Respublikasining "Dori vositalari va farmatsevtika faoliyati to'g'risida" Qonuni
2. O'zbekiston Respublikasi Prezidentining "Raqamli O'zbekiston — 2030" strategiyasi. PF-6079, 2020.
3. O'zbekiston Respublikasining "Yong'in xavfsizligi to'g'risida"gi Qonuni
4. O'zbekiston Respublikasining "Axborot texnologiyalari to'g'risida"gi Qonuni
5. Vazirlar Mahkamasining sog'liqni saqlashni raqamlashtirish bo'yicha qarorlari

**Xalqaro standartlar (2–3):**
6. ISO/IEC 27001:2022. Information security management systems
7. IEEE 830-1998. Recommended Practice for Software Requirements Specifications

**Ilmiy manbalar (3–5):**
8. Christen, P. (2012). Data Matching: Concepts and Techniques for Record Linkage, Entity Resolution, and Duplicate Detection. Springer.
9. Sinnott, R.W. (1984). Virtues of the Haversine. Sky and Telescope. Vol. 68, No. 2, p. 159.
10. Bernstein, P.A., Newcomer, E. (2009). Principles of Transaction Processing. Morgan Kaufmann.

**Texnik dokumentatsiya (4–5):**
11. Microsoft Corporation. (2024). ASP.NET Core Documentation. https://docs.microsoft.com/aspnet/core
12. Microsoft Corporation. (2024). Blazor Server Documentation
13. PostgreSQL Global Development Group. (2024). pg_trgm — trigram similarity. https://www.postgresql.org/docs/current/pgtrgm.html
14. PostgreSQL Global Development Group. (2024). PostgreSQL Documentation
15. MudBlazor. (2024). Component Library Documentation. https://mudblazor.com/docs

---

## DASTUR ILOVASI (3–7 bet, faqat kod)

Qo'yiladigan kodlar:
1. `Program.cs` — startup
2. `DrugstoreDbContext.cs` — DbContext
3. `HaversineCalculator.cs` — geodezik masofa algoritmi
4. `SearchRepository.cs` — pg_trgm SQL qidiruv
5. `PharmacyRanker.cs` — optimallashtirish
6. `SearchPage.razor` — bosh qidiruv sahifasi

---

## Fayl saqlash joyi

```
docs/thesis/
├── 00-titul-annotatsiya.md
├── 01-kirish.md
├── 02-bob1-01-dorixona-tizimlari.md
├── 02-bob1-02-qidiruv-haversine.md
├── 02-bob1-03-mavjud-platformalar.md
├── 03-bob2-01-arxitektura.md
├── 03-bob2-02-database.md
├── 04-bob3-01-imkoniyatlar.md
├── 04-bob3-02-algoritmlar.md
├── 05-bob4-01-ergonomika.md
├── 05-bob4-02-yongin.md
├── 06-xulosa.md
├── 07-adabiyotlar.md
└── 08-ilovalar.md
```

# KIRISH

Zamonaviy axborot texnologiyalarining jadal rivojlanishi barcha sohalarda, jumladan farmatsevtika va sog'liqni saqlash sohasida ham tubdan o'zgarishlarga olib kelmoqda. Bugungi kunda fuqarolar zarur dori vositalarini topish va sotib olish uchun bir nechta dorixonaga shaxsan borishga majbur bo'lmoqda, bu esa bemorlar uchun qimmatli vaqt va kuch sarfiga olib keladi. Xususan, kerakli dori mavjudligini oldindan bilmasdan dorixonadan dorixonaga yurish — ayniqsa katta shaharlarda va favqulodda holatlarda — jiddiy muammo sifatida namoyon bo'ladi.

O'zbekistonda dorixona tarmog'i keng rivojlangan bo'lishiga qaramay, foydalanuvchilarga kerakli dorini qaysi dorixonada va qanday narxda topish mumkinligini ko'rsatuvchi raqamli platforma hali yetarli darajada shakllanmagan. Mavjud yechimlar bir necha asosiy cheklovga ega: birinchidan, dori nomini xato yoki boshqa tilda kiritilganda qidiruv natija bermaydi; ikkinchidan, dorixona ro'yxati foydalanuvchi joylashuvidan masofaga ko'ra saralangan holda ko'rsatilmaydi; uchinchidan, bir nechta dorixonaning narxlarini bir joyda solishtirish imkoniyati mavjud emas. Ushbu muammolarni hal etishda zamonaviy qidiruv algoritmlari va geodezik masofa hisoblash texnologiyalari muhim rol o'ynashi mumkin.

Mazkur bitiruv malakaviy ishi dorixonalardan dori buyurtma berishning eng yaqin va qulay yechimini avtomatik topib beruvchi dasturiy vositasini ishlab chiqishga bag'ishlangan bo'lib, unda ko'p bosqichli fuzzy qidiruv algoritmi va Haversine geodezik masofa formulasidan samarali foydalanilgan. Ishlab chiqilgan tizim foydalanuvchilarga kerakli dorini qaysi dorixonada qancha narxda va qancha masofada topish mumkinligini bir zumda aniqlab berish imkonini yaratadi.

***

**Bitiruv malakaviy ishining dolzarbligi:**

O'zbekiston Respublikasi Prezidentining 2020-yil 5-oktabrdagi PF-6079-son "Raqamli O'zbekiston — 2030" strategiyasini amalga oshirish chora-tadbirlari to'g'risidagi farmoni mamlakatimizda raqamli iqtisodiyotni rivojlantirish, axborot texnologiyalarini barcha sohalarga joriy etish va sog'liqni saqlash tizimini raqamlashtirish bo'yicha aniq maqsad va vazifalarni belgilab bergan. Ushbu strategik hujjat doirasida farmatsevtika sohasini raqamlashtirish ham ustuvor yo'nalishlardan biri sifatida belgilangan bo'lib, fuqarolarga dori vositalaridan qulay foydalanish imkoniyatini yaratish davlat siyosatining muhim qismi hisoblanadi. Sog'liqni saqlash sohasida raqamli yechimlarni joriy etish nafaqat vaqtni tejash, balki muolaja sifatini oshirish va dori vositalarining mavjudligini oshirish nuqtai nazaridan ham katta ahamiyat kasb etadi.

O'zbekiston farmatsevtika bozorida bugungi kunda 10 000 dan ortiq dorixona faoliyat yuritmoqda, biroq ularning inventar ma'lumotlari yagona raqamli tizimda jamlangan emas. Mavjud apteka.uz kabi platformalar dori mavjudligini real vaqtda tekshirish va geolokatsiya asosida saralash imkoniyatlarini taqdim eta olmaydi. Bundan tashqari, dori nomlarining o'zbek, rus va ingliz tillarida turlicha yozilishi oddiy qidiruv tizimlarini samarasiz qilib qo'ymoqda. Yuqorida keltirilgan muammolar va raqamli transformatsiya zarurati ushbu tadqiqotning dolzarbligini to'la asoslab beradi.

***

**Bitiruv malakaviy ishining maqsadi:**

Ushbu bitiruv malakaviy ishining asosiy maqsadi — dorixonalardan dori buyurtma berishda eng yaqin va qulay yechimni avtomatik topib beruvchi dasturiy vositasini ishlab chiqishdan iborat. Ushbu maqsadga erishish uchun quyidagi jihatlar ko'zda tutilgan:

- Dorixona va dori ma'lumotlari bazasini boshqarish tizimini yaratish;
- Ko'p bosqichli fuzzy qidiruv algoritmini (pg_trgm) ishlab chiqish;
- Haversine formulasi asosida geodezik masofa hisoblash modulini yaratish;
- Foydalanuvchi joylashuvi asosida optimal dorixonani aniqlash imkonini berish;
- Farmatsevtlar uchun umumiy dori katalogini boshqarish interfeysini ishlab chiqish.

***

**Bitiruv malakaviy ishining vazifalari:**

Belgilangan maqsadga erishish uchun quyidagi vazifalar hal etilishi ko'zda tutilgan:

- Dorixona axborot tizimlarining nazariy asoslarini o'rganish;
- Fuzzy qidiruv va geodezik masofani hisoblash algoritmlarini tahlil qilish;
- Mavjud dorixona platformalarini qiyosiy tahlil qilish;
- Tizim arxitekturasini loyihalash (Clean Architecture);
- PostgreSQL va pg_trgm yordamida ko'p bosqichli qidiruv modulini ishlab chiqish;
- Haversine formulasi asosida optimallashtirish modulini yaratish;
- MudBlazor asosida foydalanuvchi interfeysini ishlab chiqish;
- Tizimni sinovdan o'tkazish va natijalarni tahlil qilish.

***

**Bitiruv malakaviy ishining tuzilishi:**

Ushbu bitiruv malakaviy ishi kirish, to'rt bob, xulosa, foydalanilgan adabiyotlar ro'yxati va dastur ilovasidan iborat.

Kirish qismida tadqiqotning dolzarbligi asoslab berilgan, maqsad va vazifalar aniq shakllantirilgan hamda ishning umumiy tuzilishi bayon etilgan.

Birinchi bob "Dorixona axborot tizimlarini raqamlashtirish va optimallashtirish algoritmlarining nazariy asoslari" deb nomlanib, zamonaviy dorixona boshqaruv tizimlari, qidiruv algoritmlari va geodezik masofa hisoblashning nazariy asoslari, shuningdek mavjud dorixona platformalarining qiyosiy tahlili yoritilgan.

Ikkinchi bob "Dorixona qidiruv tizimini loyihalash va ishlab chiqish" deb nomlanib, tizim arxitekturasi va zaruriy texnologiyalarni tanlash asoslari hamda ma'lumotlar bazasini loyihalash va integratsiyalash masalalari batafsil ko'rib chiqilgan.

Uchinchi bob "Loyihaning amaliy tatbiqi va algoritmlarining samaradorligi" deb nomlanib, tizim imkoniyatlari bilan tanishish va qidiruv hamda optimallashtirish algoritmlarining samaradorligi tahlil qilingan.

To'rtinchi bob "Hayot faoliyati xavfsizligi" deb nomlanib, kompyuter bilan ishlashda ergonomika va sog'liqni saqlash, shuningdek yong'in xavfsizligi masalalari ko'rib chiqilgan.

Xulosa qismida olib borilgan tadqiqot natijalari umumlashtirilgan va asosiy xulosalar shakllantirilgan. Foydalanilgan adabiyotlar ro'yxatida tadqiqot davomida murojaat etilgan barcha manbalar keltirilgan. Dastur ilovasida tizimning asosiy kod qismlari joylashtirilgan.

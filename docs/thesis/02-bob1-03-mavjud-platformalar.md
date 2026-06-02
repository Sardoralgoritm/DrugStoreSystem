## 1.3. Mavjud dorixona platformalari va qidiruv tizimlarining qiyosiy tahlili

Zamonaviy farmatsevtika texnologiyalari bozorida dori qidiruv va dorixona boshqaruv tizimlari tobora keng tarqalmoqda. Ushbu bo'limda eng ko'p qo'llaniladigan xalqaro va mahalliy dorixona platformalari tahlil qilinib, ularning afzalliklari, kamchiliklari va DrugstoreSystem loyihasidan farqli tomonlari ko'rsatib o'tiladi. Bunday qiyosiy tahlil yangi tizim ishlab chiqishda qanday bo'shliqlarni to'ldirish zarurligini aniq belgilash va o'z yechimimizning afzalligini asoslash imkonini beradi.

### Xalqaro platformalar

**GoodRx** — bu AQShda keng tarqalgan dori narxlarini solishtiruvchi platforma bo'lib, foydalanuvchilarga yaqin atrofdagi dorixonalardagi narxlarni ko'rish va chegirmali kuponlar olish imkoniyatini beradi. GoodRx ning asosiy kuchi — keng geografik qamrovi va narxlar shaffofligida: millionlab foydalanuvchi ushbu platforma yordamida dori xaridlarida o'rtacha 80% gacha tejashga erishadilar. Biroq, platforma faqat AQSh bozori uchun mo'ljallangan, geolokatsiya asosida saralash amalga oshirilsa-da, fuzzy qidiruv mexanizmi cheklangan va O'zbekiston kabi ko'p tilli bozorlar uchun mutlaqo moslashtirilmagan.

**DocMorris** — bu Germaniyada ishga tushirilgan Evropaning yetakchi onlayn dorixona platformasi bo'lib, foydalanuvchilarga dori vositalarini onlayn buyurtma qilish va uyiga yetkazish imkoniyatini taqdim etadi. Platforma keng assortiment va retseptsiz dorilarning qulay tanlovini ta'minlaydi. Biroq, DocMorris asosan Germaniya va Niderlandiya bozorlariga yo'naltirilgan, geolokatsiya asosida saralash funksiyasi mavjud emas — platforma yetkazib berish xizmatiga, fizik dorixonalarni ko'rsatishga emas, tayanadi.

**Tabletki.ua** — bu Ukrainaning eng yirik dorixona qidiruv platformasi bo'lib, foydalanuvchilarga kerakli dorini qaysi dorixonada topish va narxini oldindan ko'rish imkoniyatini beradi. Platforma real vaqtda inventar yangilanishini, dorixona lokatsiyasini xaritada ko'rsatishni va narx bo'yicha filtrlashni qo'llab-quvvatlaydi. Fuzzy qidiruv cheklangan darajada amalga oshirilgan — rus tilidagi nomlar qo'llab-quvvatlansa-da, xato yozilgan so'zlar uchun alternativ taklif qilish mexanizmi zaifligi seziladi.

**iApteka.ru (Rossiya)** — bu Rossiyaning yetakchi onlayn dorixona aggregatoridir. Platforma geolokatsiya asosida eng yaqin dorixonalarni ko'rsatish, narxlarni solishtirish va dorixona zaxirasini real vaqtda tekshirish imkoniyatlarini taqdim etadi. Qidiruv algoritmi rus tilidagi nomlarni yaxshi qo'llab-quvvatlaydi, sinonimlar bilan ishlash mexanizmi ham mavjud. Biroq, platforma faqat rus tiliga yo'naltirilgan — o'zbek tili va lotincha yozilish variantlari qo'llab-quvvatlanmaydi.

### Mahalliy platformalar

**Apteka.uz** — O'zbekistondagi asosiy dorixona ma'lumotlari portali bo'lib, dorixonalar ro'yxati, ularning manzili va telefon raqamlarini saqlaydi. Platforma asosiy katalog funksiyasini bajaradi, lekin real vaqtda inventar tekshirish, narxlarni solishtirish va geolokatsiya asosida saralash imkoniyatlari mavjud emas. Qidiruv faqat aniq mos (exact match) asosida ishlaydi — xato yozilgan nomlar yoki sinonimlar uchun hech qanday natija bermaydi.

**OLX.uz Sog'liqni saqlash bo'limi** — bu umumiy e'lonlar platformasining tibbiyot bo'limi bo'lib, asosan b/u tibbiy asbob-uskunalar va ba'zan dori e'lonlarini o'z ichiga oladi. Platforma professional dorixona qidiruv tizimi vazifasini bajara olmaydi — ma'lumotlar tartibsiz, dori mavjudligi tasdiqlanmagan va qidiruv funksiyasi juda cheklangan.

### Qiyosiy tahlil jadvali

**1.3.1-jadval. Mavjud dorixona platformalarining qiyosiy tahlili**

| Platforma | Fuzzy qidiruv | Geolokatsiya | Ko'p tillilik | Real inventar | O'zbekiston |
|---|---|---|---|---|---|
| GoodRx | Qisman | Ha | Yo'q | Ha | Yo'q |
| DocMorris | Yo'q | Yo'q | Qisman | Ha | Yo'q |
| Tabletki.ua | Qisman | Ha | Qisman | Ha | Yo'q |
| iApteka.ru | Qisman | Ha | Faqat rus | Ha | Yo'q |
| Apteka.uz | Yo'q | Yo'q | Yo'q | Yo'q | Ha |
| OLX.uz | Yo'q | Yo'q | Yo'q | Yo'q | Ha |
| **DrugstoreSystem** | **To'liq (5 bosqich)** | **Ha (Haversine)** | **O'zb+Rus+Ing** | **Ha** | **Ha** |

Jadvaldan ko'rinib turibdiki, xalqaro platformalar inventar boshqaruvi va qidiruv imkoniyatlari jihatidan kuchli, ammo mahalliy bozorga va ko'p tillilikka moslashmagan. Mahalliy platformalar esa mahalliy kontekstni bilsa-da, texnik imkoniyatlar jihatidan juda orqada. Aynan shu bo'shliqni to'ldirish ushbu loyihaning asosiy maqsadi hisoblanadi.

### DrugstoreSystem yechimining farqlovchi xususiyatlari

Men ishlab chiqqan DrugstoreSystem platformasi yuqoridagi tahlil asosida aniqlangan asosiy bo'shliqni to'ldiradi. Tizimning birinchi farqlovchi xususiyati — pg_trgm asosidagi 5 bosqichli fuzzy qidiruv algoritmi bo'lib, u xato yozilgan, sinonim va rus tilidagi dori nomlarini ham aniq topish imkonini beradi. Ikkinchi xususiyat — Haversine geodezik masofa formulasi asosida qurilgan optimallashtirish moduli bo'lib, u foydalanuvchiga eng yaqin dorixonani kilometr aniqligida ko'rsatadi. Uchinchi xususiyat — shared catalog arxitekturasi bo'lib, farmatsevtlar birgalikda umumiy dori katalogini boyitib boradilar, bu esa vaqt o'tishi bilan tizimning sifati avtomatik oshib borishini ta'minlaydi. To'rtinchi xususiyat — mehmon foydalanuvchi modeli bo'lib, login talab qilinmaydi va har kim darhol qidirishni boshlashi mumkin. Ushbu xususiyatlar kombinatsiyasi DrugstoreSystem ni mavjud analoglardan sifat jihatidan ajratib turadi.

### Xulosa

Shunday qilib, mavjud dorixona platformalari va qidiruv tizimlarini qiyosiy tahlil qilish shuni ko'rsatadiki, O'zbekiston bozori uchun moslashtirilgan, fuzzy qidiruv va geolokatsiya imkoniyatlarini to'liq qo'llaydigan hamda ko'p tillilikni qo'llab-quvvatlaydigan mahalliy platformaga bo'lgan talab hali ham qondirilmagan. Xalqaro platformalar texnik jihatdan kuchli, ammo mahalliy kontekstni hisobga olmaydi; mahalliy platformalar esa texnik imkoniyatlardan mahrum. Keyingi bobda DrugstoreSystem platformasining arxitekturaviy yechimlari va ishlatilgan texnologiyalar batafsil ko'rib chiqiladi.

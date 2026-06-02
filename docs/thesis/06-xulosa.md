# XULOSA

Ushbu bitiruv malakaviy ishi dorixonalardan dori buyurtma berishda eng yaqin va qulay yechimni avtomatik topib beruvchi dasturiy vositasini ishlab chiqishga bag'ishlangan bo'lib, ko'p bosqichli fuzzy qidiruv algoritmi va Haversine geodezik masofa formulasini o'zida mujassam etgan to'liq funksional multi-pharmacy veb-platforma sifatida amalga oshirildi.

Birinchi bobda dorixona axborot tizimlarini raqamlashtirish va optimallashtirish algoritmlarining nazariy asoslari chuqur o'rganildi. Zamonaviy multi-pharmacy platformalarning rivojlanish bosqichlari, shared catalog konsepsiyasi va O'zbekiston farmatsevtika bozorining o'ziga xos xususiyatlari tahlil qilindi. Fuzzy qidiruv texnologiyalari — xususan PostgreSQL ning `pg_trgm` kengaytmasi — ning nazariy asoslari va Haversine formulasining matematik poydevori batafsil ko'rib chiqildi. Mavjud xalqaro (GoodRx, DocMorris, Tabletki.ua, iApteka.ru) va mahalliy (apteka.uz) platformalarning qiyosiy tahlili O'zbekiston bozorida to'liq echimga bo'lgan talabni aniq ko'rsatdi.

Ikkinchi bobda DrugstoreSystem platformasining arxitekturaviy yechimlari va ma'lumotlar bazasi modeli batafsil ko'rib chiqildi. Clean Architecture tamoyiliga asoslangan to'rt qatlamli tuzilma (Domain, Application, Infrastructure, Web), texnologiyalar to'plami (.NET 10, Blazor Server, MudBlazor, EF Core, PostgreSQL 16) va ularni tanlash sabablari asoslab berildi. PostgreSQL dagi `pg_trgm` kengaytmasi va GIN indekslar, shared catalog arxitekturasi hamda Code-First yondashuvi loyihaning texnik poydevorini tashkil etdi.

Uchinchi bobda tizimning amaliy imkoniyatlari va algoritmlarning samaradorligi real sinovlar orqali tasdiqlandi. Admin paneli, farmatsevt paneli va mehmon foydalanuvchi uchun qidiruv sahifalari to'liq funksional holda namoyish etildi. 5 bosqichli fuzzy qidiruv algoritmi xato yozilgan, sinonim va rus tilidagi dori nomlarini ham to'g'ri aniqlaganligini, Haversine formulasi esa Toshkent–Samarqand masofasini ~267.5 km aniqlikda hisoblashini amaliy sinovlar tasdiqladi.

To'rtinchi bobda kompyuter bilan ishlashda ergonomika va sog'liqni saqlash, shuningdek yong'in xavfsizligi masalalari O'zbekiston Respublikasining amaldagi qonunlari doirasida ko'rib chiqildi.

Ish natijasida quyidagi asosiy xulosalar chiqarildi:

- Ko'p bosqichli fuzzy qidiruv algoritmi (pg_trgm) dori nomlarini to'g'ri va noto'g'ri yozilgan, sinonim va rus tilidagi holatlarda ham ishonchli topib berishini amaliyot isbotladi;
- Haversine formulasi asosidagi optimallashtirish algoritmi foydalanuvchiga eng yaqin dorixonani geodezik masofa bo'yicha aniq ko'rsatdi va barcha unit testlardan muvaffaqiyatli o'tdi;
- Shared catalog arxitekturasi farmatsevtlar tomonidan birgalikda boyitiladigan umumiy dori bazasini shakllantirish uchun samarali yondashuv ekanligi tasdiqlandi;
- Blazor Server, MudBlazor va PostgreSQL kombinatsiyasi O'zbekiston shart-sharoitlari uchun — bitta C# ekotizimida server va klient tomonini birlashtirgan — optimal texnik yechim ekanligini ko'rsatdi;
- Mehmon foydalanuvchi modeli (login talab qilinmaydigan ochiq qidiruv) platformaning foydalanuvchi auditoriyasini sezilarli kengaytiradi va amaliy qiymatini oshiradi.

Kelajakda platformani quyidagi yo'nalishlarda rivojlantirish mumkin: mobil versiya yaratish (MAUI yoki PWA), dorixonalar uchun onlayn buyurtma moduli qo'shish, retsept boshqaruvi va tibbiy muassasalar bilan integratsiya, shuningdek dori narxlari dinamikasini tahlil qiluvchi statistik modul ishlab chiqish.

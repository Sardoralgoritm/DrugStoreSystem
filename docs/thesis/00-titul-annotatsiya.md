# ANNOTATSIYA

## O'zbek tilida

Ushbu bitiruv malakaviy ishi O'zbekistonda dorixonalardan dori qidirishda foydalanuvchiga eng yaqin va qulay yechimni avtomatik topib beruvchi dasturiy vositasini ishlab chiqishga bag'ishlangan. Zamonaviy farmatsevtika sohasida dori vositalarini bir nechta dorixona bo'ylab qo'lda qidirish jarayoni vaqt talab qiluvchi va noqulay bo'lib, mavjud mahalliy platformalar foydalanuvchiga geolokatsiya asosida saralangan natijalarni taqdim eta olmaydi — aynan shu bo'shliqni to'ldirish ushbu ishning asosiy motivatsiyasi hisoblanadi.

Ishning asosiy maqsadi — ko'p bosqichli fuzzy qidiruv algoritmi va Haversine geodezik masofa formulasiga asoslangan multi-pharmacy veb-platforma yaratishdan iborat. Tizimda ikkita muhim algoritm markaziy o'rin tutadi: birinchisi, pg_trgm kengaytmasi yordamida xato yozilgan, sinonim yoki rus tilidagi dori nomlarini ham aniq topib beruvchi 5 bosqichli fuzzy qidiruv algoritmi; ikkinchisi, foydalanuvchi joylashuvidan har bir dorixonagacha bo'lgan geodezik masofani hisoblash orqali eng yaqin va qulay dorixonani aniqlash imkonini beruvchi Haversine optimallashtirish algoritmi.

Dasturiy vosita .NET 10, Blazor Server va MudBlazor texnologiyalari asosida ishlab chiqilgan bo'lib, ma'lumotlar bazasi sifatida PostgreSQL 16 ishlatilgan. Tizim uch xil foydalanuvchi rolini qo'llab-quvvatlaydi: admin (dorixonalarni boshqaradi), farmatsevt (o'z inventarini boshqaradi) va mehmon foydalanuvchi (login talab qilinmaydi, faqat qidiradi). Umumiy dori katalogi farmatsevtlar tomonidan birgalikda to'ldiriladi va autocomplete orqali yangi dorilar qo'shiladi.

Tizimni sinovdan o'tkazish natijasida 5 bosqichli qidiruv algoritmi xato yozilgan va rus tilidagi dori nomlarini ham to'g'ri aniqlaganligini, Haversine formulasi esa Toshkent–Samarqand masofasini ~267,5 km aniqlikda hisoblashini tasdiqladi.

**Kalit so'zlar:** dorixona tizimi, fuzzy qidiruv, pg_trgm, Haversine formulasi, Blazor Server, PostgreSQL, geodezik masofa, optimallashtirish algoritmi

---

## Rus tilida (На русском языке)

Данная выпускная квалификационная работа посвящена разработке программного инструмента, который автоматически находит наиболее оптимальное решение для заказа лекарств в аптеках Узбекистана на основе геолокации пользователя. В условиях недостаточного развития цифровых фармацевтических платформ на местном рынке разработанная система призвана заполнить существующий пробел, предоставляя пользователям ранжированные результаты поиска с учётом расстояния до аптек и цены на препараты.

Основная цель работы — создание многофункциональной веб-платформы на основе многоэтапного алгоритма нечёткого поиска и формулы Хаверсина для вычисления геодезического расстояния. В системе реализованы два ключевых алгоритма: пятиэтапный алгоритм нечёткого поиска с использованием расширения pg_trgm, позволяющий находить лекарства даже при опечатках, синонимах или русскоязычном написании; а также алгоритм оптимизации на основе формулы Хаверсина, определяющий ближайшую аптеку с учётом геодезического расстояния от местоположения пользователя.

Система разработана на платформе .NET 10 с использованием Blazor Server и MudBlazor, в качестве базы данных применяется PostgreSQL 16 с расширением pg_trgm. Поддерживаются три роли пользователей: администратор (управление аптеками), фармацевт (управление инвентарём) и гость (поиск без регистрации). Общий каталог лекарств формируется совместно фармацевтами через функцию автодополнения.

Тестирование системы подтвердило корректную работу алгоритма поиска при опечатках и русскоязычных запросах, а формула Хаверсина обеспечила точность вычисления расстояния Ташкент–Самарканд на уровне ~267,5 км.

**Ключевые слова:** система аптек, нечёткий поиск, pg_trgm, формула Хаверсина, Blazor Server, PostgreSQL, геодезическое расстояние, алгоритм оптимизации

---

## Ingliz tilida (In English)

This graduation thesis is dedicated to the development of a software tool that automatically finds the most optimal solution for ordering medicines from pharmacies in Uzbekistan based on the user's geolocation. In the context of insufficiently developed digital pharmaceutical platforms in the local market, the developed system aims to bridge the existing gap by providing users with ranked pharmacy search results sorted by proximity and price.

The main objective of the work is to create a multi-pharmacy web platform based on a multi-stage fuzzy search algorithm and the Haversine geodesic distance formula. The system incorporates two core algorithms: a five-stage fuzzy search algorithm using the pg_trgm extension, which correctly identifies medicines even with typos, synonyms, or Russian-language input; and a Haversine-based optimization algorithm that determines the nearest pharmacy by calculating the geodesic distance from the user's location.

The software tool is developed using .NET 10, Blazor Server, and MudBlazor, with PostgreSQL 16 as the database management system. The system supports three user roles: administrator (pharmacy management), pharmacist (inventory management), and guest users (search without registration). A shared medicine catalog is collectively maintained by pharmacists through an autocomplete-based workflow.

System testing confirmed that the five-stage search algorithm correctly identifies medicines with typos and Russian-language queries, while the Haversine formula calculated the Tashkent–Samarkand distance with an accuracy of approximately 267.5 km.

**Keywords:** pharmacy system, fuzzy search, pg_trgm, Haversine formula, Blazor Server, PostgreSQL, geodesic distance, optimization algorithm

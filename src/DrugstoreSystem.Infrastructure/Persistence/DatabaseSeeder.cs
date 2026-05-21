using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Domain.Enums;
using DrugstoreSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DrugstoreSystem.Infrastructure.Persistence;

public class DatabaseSeeder
{
    private readonly DrugstoreDbContext _db;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        DrugstoreDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedAdminAsync();
        await SeedCategoriesAsync();
        await SeedPharmaciesAsync();
        await SeedMedicinesAsync();
        await SeedInventoryAsync();
    }

    private async Task SeedRolesAsync()
    {
        foreach (var role in new[] { "Admin", "Pharmacist" })
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
                _logger.LogInformation("Created role {Role}", role);
            }
        }
    }

    private async Task SeedCategoriesAsync()
    {
        if (await _db.Categories.AnyAsync()) return;

        var categories = new[]
        {
            "Analgetik", "Antibiotik", "Antivirusli", "Kardio",
            "Enterosorbent", "Vitamini", "Spazmolitik",
            "Ko'z tomchilari", "Tashqi", "Boshqa"
        };

        _db.Categories.AddRange(categories.Select(n => new Category(n)));
        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} categories", categories.Length);
    }

    private async Task SeedAdminAsync()
    {
        var adminEmail = _configuration["Seed:AdminEmail"] ?? "admin@drugstore.local";
        var adminPassword = _configuration["Seed:AdminPassword"] ?? "Admin@1234";

        if (await _userManager.FindByEmailAsync(adminEmail) is not null) return;

        var admin = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, "Admin");
            _logger.LogInformation("Seeded admin user {Email}", adminEmail);
        }
    }

    private async Task SeedPharmaciesAsync()
    {
        var existingEmails = await _db.Users.Select(u => u.Email).ToHashSetAsync();

        var pharmacyData = new[]
        {
            ("Shifo Dorixona",      "Toshkent, Chilonzor tumani, Bunyodkor ko'ch. 12",        41.2995, 69.2401, "shifo@pharm.uz",     "Pharmacy@123!"),
            ("Dori-Darmon Plus",    "Toshkent, Yunusobod tumani, Amir Temur shoh ko'ch. 108", 41.3375, 69.3019, "doridarm@pharm.uz",  "Pharmacy@123!"),
            ("Salomatlik Apteka",   "Toshkent, Mirzo Ulug'bek tumani, Bog'ishamol ko'ch. 56", 41.3191, 69.3018, "salomatlik@pharm.uz","Pharmacy@123!"),
            ("Al-Farabi Dorixona",  "Toshkent, Shayxontohur tumani, Navoi ko'ch. 30",         41.3169, 69.2619, "alfarabi@pharm.uz",  "Pharmacy@123!"),
            ("Hayot Apteka",        "Toshkent, Sergeli tumani, Yangi Sergeli ko'ch. 5",        41.2398, 69.2100, "hayot@pharm.uz",     "Pharmacy@123!"),
            ("Baraka Dorixona",     "Toshkent, Uchtepa tumani, Mustaqillik ko'ch. 44",         41.3011, 69.2169, "baraka@pharm.uz",    "Pharmacy@123!"),
            ("Sog'lom Hayot",       "Samarqand sh., Registon ko'ch. 2",                        39.6542, 66.9597, "soglom@pharm.uz",    "Pharmacy@123!"),
            ("Farg'ona Apteka",     "Farg'ona sh., Al-Farg'oniy ko'ch. 15",                    40.3764, 71.7971, "fargona@pharm.uz",   "Pharmacy@123!"),
        };

        int seeded = 0;
        foreach (var (name, address, lat, lng, email, password) in pharmacyData)
        {
            if (existingEmails.Contains(email)) continue;

            var pharmacy = new Pharmacy(name, address, lat, lng, "+998901234567", "09:00–22:00 (Har kuni)");
            pharmacy.Activate();
            pharmacy.Verify();
            _db.Pharmacies.Add(pharmacy);
            await _db.SaveChangesAsync();

            var user = new AppUser { UserName = email, Email = email, EmailConfirmed = true, PharmacyId = pharmacy.Id };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
                await _userManager.AddToRoleAsync(user, "Pharmacist");
            seeded++;
        }

        if (seeded > 0) _logger.LogInformation("Seeded {Count} pharmacies", seeded);
    }

    private async Task SeedMedicinesAsync()
    {
        if (await _db.Medicines.AnyAsync(m => m.CreatedByPharmacyId == null)) return;

        // Ensure all required categories exist (idempotent)
        var requiredCategories = new[]
        {
            "Analgetik", "Antibiotik", "Antivirusli", "Kardio",
            "Enterosorbent", "Vitamini", "Spazmolitik",
            "Ko'z tomchilari", "Tashqi", "Boshqa"
        };
        var existingCategories = await _db.Categories.ToListAsync();
        var existingNames = existingCategories.Select(c => c.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var catName in requiredCategories.Where(n => !existingNames.Contains(n)))
        {
            _db.Categories.Add(new Category(catName));
        }
        await _db.SaveChangesAsync();

        var categories = await _db.Categories.ToListAsync();
        int CatId(string name) => categories.First(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)).Id;

        var medicines = new[]
        {
            // (Name, GenericName, NameRu, DosageForm, Category, Manufacturer, Description, synonyms[])
            ("Paracetamol",   "Acetaminophen",          "Парацетамол",          DosageForm.Tablet,   "Analgetik",     "Pharmstandard",   "Isitma va og'riqni kamaytiruvchi dori.",                       new[]{"Panadol","Para-500","Tylenol"}),
            ("Ibuprofen",     "Ibuprofen",               "Ибупрофен",            DosageForm.Tablet,   "Analgetik",     "Reckitt Benckiser","Yallig'lanishga qarshi og'riq qoldiruvchi.",                   new[]{"Nurofen","Advil","Ibufen"}),
            ("Analgin",       "Metamizol",               "Анальгин",             DosageForm.Tablet,   "Analgetik",     "Pharmstandard",   "Kuchli og'riq qoldiruvchi.",                                   new[]{"Baralgin"}),
            ("Aspirin",       "Acetylsalicylic acid",    "Аспирин",              DosageForm.Tablet,   "Analgetik",     "Bayer",           "Og'riq va isitmaga qarshi, qon suyultiruvchi.",                new[]{"Cardiomagnil"}),
            ("Citramon",      "Paracetamol+Caffeine",    "Цитрамон",             DosageForm.Tablet,   "Analgetik",     "Pharmstandard",   "Bosh og'riqqa qarshi kombinatsiyalangan dori.",                new string[]{}),
            ("Diclofenac",    "Diclofenac",              "Диклофенак",           DosageForm.Tablet,   "Analgetik",     "Novartis",        "Yallig'lanish va bo'g'im og'riqlarida qo'llaniladi.",          new[]{"Voltaren"}),
            ("No-Spa",        "Drotaverine",             "Но-шпа",               DosageForm.Tablet,   "Spazmolitik",   "Sanofi",          "Silliq mushaklarning spazmini bartaraf etadi.",                new[]{"Spazmol","Drotaverin"}),
            ("Amoxicillin",   "Amoxicillin",             "Амоксициллин",         DosageForm.Capsule,  "Antibiotik",    "Sandoz",          "Keng spektrli penitsillin guruhiga mansub antibiotik.",        new[]{"Flemoxin","Ospamox"}),
            ("Azithromycin",  "Azithromycin",            "Азитромицин",          DosageForm.Tablet,   "Antibiotik",    "Pfizer",          "Nafas yo'llari infeksiyalarida qo'llanuvchi antibiotik.",      new[]{"Sumamed","Azitro"}),
            ("Cefazolin",     "Cefazolin",               "Цефазолин",            DosageForm.Injection,"Antibiotik",    "Biochemie",       "Keng spektrli sefalosporin antibiotigi.",                      new string[]{}),
            ("Metronidazole", "Metronidazole",           "Метронидазол",         DosageForm.Tablet,   "Antibiotik",    "Nidapharma",      "Anaerob bakteriyalar va protozoyalarga qarshi.",               new[]{"Flagyl","Metrogyl"}),
            ("Acyclovir",     "Acyclovir",               "Ацикловир",            DosageForm.Tablet,   "Antivirusli",   "GlaxoSmithKline", "Gerpes virusiga qarshi antivirus dori.",                       new[]{"Zovirax"}),
            ("Arbidol",       "Umifenovir",              "Арбидол",              DosageForm.Capsule,  "Antivirusli",   "Pharmstandard",   "Gripp va ORVI profilaktikasi va davolanishi uchun.",           new string[]{}),
            ("Validol",       "Menthol",                 "Валидол",              DosageForm.Tablet,   "Kardio",        "Pharmstandard",   "Yurak og'rig'i va asab qo'zg'alishida qo'llaniladi.",          new string[]{}),
            ("Corvalol",      "Phenobarbital+Ethanol",   "Корвалол",             DosageForm.Drops,    "Kardio",        "Krewel Meuselbach","Yurak urishi va asab buzilishlarida.",                        new string[]{}),
            ("Amlodipine",    "Amlodipine",              "Амлодипин",            DosageForm.Tablet,   "Kardio",        "Pfizer",          "Arterial gipertenziya va stenokardiyada qo'llaniladi.",        new[]{"Norvasc"}),
            ("Activated Carbon","Activated Charcoal",    "Активированный уголь", DosageForm.Tablet,   "Enterosorbent", "Pharmstandard",   "Zaharlanish va ich ketishda enterosorbent sifatida.",          new[]{"Ko'mir tabletka"}),
            ("Smecta",        "Diosmectite",             "Смекта",               DosageForm.Powder,   "Enterosorbent", "Ipsen",           "Ich ketish va oshqozon yallig'lanishida qo'llaniladi.",        new string[]{}),
            ("Enterosgel",    "Polymethylsiloxane",      "Энтеросгель",          DosageForm.Solution, "Enterosorbent", "Bioline",         "Organizmdagi toksinlarni chiqaruvchi enterosorbent.",          new string[]{}),
            ("Vitamin C",     "Ascorbic acid",           "Витамин С",            DosageForm.Tablet,   "Vitamini",      "Pharmstandard",   "Immunitetni mustahkamlovchi, antioksidant vitamin.",           new[]{"Askorutin","Ascorbinka"}),
            ("Vitamin D3",    "Cholecalciferol",         "Витамин Д3",           DosageForm.Drops,    "Vitamini",      "Medana Pharma",   "Suyak va immunitet uchun zarur vitamin.",                      new[]{"Aquadetrim"}),
            ("Suprastin",     "Chloropyramine",          "Супрастин",            DosageForm.Tablet,   "Boshqa",        "Egis",            "Allergik reaksiyalarda qo'llanuvchi antihistamin.",            new string[]{}),
            ("Loratadine",    "Loratadine",              "Лоратадин",            DosageForm.Tablet,   "Boshqa",        "Schering-Plough", "Allergiya alomatlari uchun antihistamin dori.",                new[]{"Claritin","Claritine"}),
            ("Omeprazole",    "Omeprazole",              "Омепразол",            DosageForm.Capsule,  "Boshqa",        "AstraZeneca",     "Oshqozon kislotasini kamaytiruvchi proton nasos inhibitori.",  new[]{"Losek"}),
            ("Mezim",         "Pancreatin",              "Мезим",                DosageForm.Tablet,   "Boshqa",        "Berlin-Chemie",   "Hazm qilishni yaxshilovchi ferment preparati.",               new string[]{}),
            ("Bisacodyl",     "Bisacodyl",               "Бисакодил",            DosageForm.Tablet,   "Boshqa",        "Boehringer",      "Ich qotishda qo'llanuvchi laksativ dori.",                     new[]{"Dulcolax"}),
            ("Chlorhexidine", "Chlorhexidine",           "Хлоргексидин",         DosageForm.Solution, "Tashqi",        "ICN Pharmaceuticals","Yaralarni dezinfeksiya qilish uchun antiseptik.",           new string[]{}),
            ("Panthenol",     "Dexpanthenol",            "Пантенол",             DosageForm.Cream,    "Tashqi",        "Bayer",           "Teri shikastlanishlari, kuyish va yaralar uchun.",            new[]{"D-Panthenol"}),
            ("Tobramycin",    "Tobramycin",              "Тобрамицин",           DosageForm.Drops,    "Ko'z tomchilari","Alcon",          "Ko'z infeksiyalarida qo'llanuvchi antibiotik tomchilar.",      new[]{"Tobrex"}),
            ("Artificial Tears","Hydroxypropyl",         "Искусственные слезы",  DosageForm.Drops,    "Ko'z tomchilari","Allergan",       "Ko'z quruqligida namlantiruvchi tomchilar.",                   new[]{"Visine"}),
        };

        var pharmacy1 = await _db.Pharmacies.FirstAsync();

        foreach (var (name, generic, nameRu, form, catName, manufacturer, desc, synonyms) in medicines)
        {
            var medicine = new Medicine(name, generic, nameRu, form, CatId(catName), manufacturer, desc, pharmacy1.Id);
            await _db.Medicines.AddAsync(medicine);
            await _db.SaveChangesAsync();

            foreach (var s in synonyms.Where(s => !string.IsNullOrWhiteSpace(s)))
            {
                _db.MedicineSynonyms.Add(new MedicineSynonym(medicine.Id, s));
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded 30 medicines");
    }

    private async Task SeedInventoryAsync()
    {
        // Only seed inventory for pharmacies that have none yet
        var pharmaciesWithStock = await _db.PharmacyMedicines
            .Select(pm => pm.PharmacyId)
            .Distinct()
            .ToListAsync();

        var pharmacies = await _db.Pharmacies
            .Where(p => !pharmaciesWithStock.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ToListAsync();

        if (pharmacies.Count == 0) return;
        var medicines = await _db.Medicines.OrderBy(m => m.Id).ToListAsync();

        // Price base per medicine (UZS), each pharmacy gets ±15% variance
        var basePrices = new[]
        {
            3500m, 8200m, 4100m, 3200m, 5800m, 9600m, 12000m, 22500m,
            18000m, 35000m, 16000m, 14000m, 12500m, 5500m, 9000m, 21000m,
            2100m, 18500m, 22000m, 6800m, 15000m, 7300m, 11000m, 14200m,
            8500m, 4800m, 9500m, 45000m, 28000m, 12000m,
        };

        var rng = new Random(42);

        foreach (var pharmacy in pharmacies)
        {
            // Each pharmacy stocks 18–25 medicines (skip some to vary availability)
            var indicesToStock = Enumerable.Range(0, medicines.Count)
                .Where(i => rng.NextDouble() > 0.25)
                .ToList();

            foreach (var i in indicesToStock)
            {
                var medicine = medicines[i];
                var basePrice = basePrices[i % basePrices.Length];
                var variance = 0.85 + rng.NextDouble() * 0.30; // 85%–115%
                var price = Math.Round(basePrice * (decimal)variance / 100) * 100;
                var quantity = rng.Next(5, 200);

                _db.PharmacyMedicines.Add(new PharmacyMedicine(pharmacy.Id, medicine.Id, price, quantity));
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded inventory for {Count} pharmacies", pharmacies.Count);
    }
}

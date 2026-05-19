using DrugstoreSystem.Domain.Entities;
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
            "Og'riq qoldiruvchi",
            "Antibiotik",
            "Vitamin va minerallar",
            "Yurak-tomir",
            "Oshqozon-ichak",
            "Asab tizimi",
            "Allergiya",
            "Nafas yo'llari",
            "Dermatologik",
            "Ko'z dorisi",
            "Quloq va burun",
            "Gormonal",
            "Diabet",
            "Boshqa",
        };

        _db.Categories.AddRange(categories.Select(n => new Category(n)));
        await _db.SaveChangesAsync();
        _logger.LogInformation("Seeded {Count} categories", categories.Length);
    }

    private async Task SeedAdminAsync()
    {
        var adminEmail = _configuration["Seed:AdminEmail"] ?? "admin@drugstore.local";
        var adminPassword = _configuration["Seed:AdminPassword"] ?? "Admin@1234";

        if (await _userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, "Admin");
            _logger.LogInformation("Seeded admin user {Email}", adminEmail);
        }
        else
        {
            _logger.LogError("Failed to seed admin: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}

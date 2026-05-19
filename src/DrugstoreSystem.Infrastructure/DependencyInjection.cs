using DrugstoreSystem.Application.Interfaces;
using DrugstoreSystem.Infrastructure.Identity;
using DrugstoreSystem.Infrastructure.Persistence;
using DrugstoreSystem.Infrastructure.Persistence.Search;
using DrugstoreSystem.Infrastructure.Repositories;
using DrugstoreSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrugstoreSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<DrugstoreDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .UseSnakeCaseNamingConvention());

        services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<DrugstoreDbContext>()
            .AddDefaultTokenProviders()
            .AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/auth/login";
            options.LogoutPath = "/auth/logout";
            options.AccessDeniedPath = "/auth/login";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddScoped<DatabaseSeeder>();

        // Repositories
        services.AddScoped<IPharmacyRepository, PharmacyRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IMedicineRepository, MedicineRepository>();
        services.AddScoped<ISearchRepository, SearchRepository>();

        // Services
        services.AddScoped<IPharmacyService, PharmacyService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IMedicineService, MedicineService>();

        return services;
    }
}

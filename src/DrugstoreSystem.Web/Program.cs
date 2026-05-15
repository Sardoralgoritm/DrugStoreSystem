using DrugstoreSystem.Infrastructure;
using DrugstoreSystem.Infrastructure.Identity;
using DrugstoreSystem.Infrastructure.Persistence;
using DrugstoreSystem.Web.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services)
              .Enrich.FromLogContext());

    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents();

    builder.Services.AddMudServices();

    var app = builder.Build();

    // Apply migrations and seed on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<DrugstoreDbContext>();
        await db.Database.MigrateAsync();

        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    // Login endpoint: validates credentials and sets auth cookie
    app.MapPost("/auth/login-handler", async (
        HttpContext ctx,
        SignInManager<AppUser> signInManager,
        UserManager<AppUser> userManager) =>
    {
        var form = await ctx.Request.ReadFormAsync();
        var email = form["email"].ToString();
        var password = form["password"].ToString();

        var result = await signInManager.PasswordSignInAsync(email, password, isPersistent: false, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Results.Redirect("/auth/login?error=1");

        var user = await userManager.FindByEmailAsync(email);
        var roles = await userManager.GetRolesAsync(user!);
        var redirectUrl = roles.Contains("Admin") ? "/admin/dashboard" : "/pharmacist/dashboard";
        return Results.Redirect(redirectUrl);
    });

    // Logout endpoint
    app.MapPost("/auth/logout-handler", async (
        HttpContext ctx,
        SignInManager<AppUser> signInManager) =>
    {
        await signInManager.SignOutAsync();
        return Results.Redirect("/auth/login");
    });

    app.MapStaticAssets();
    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

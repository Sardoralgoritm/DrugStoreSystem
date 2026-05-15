using DrugstoreSystem.Domain.Entities;
using DrugstoreSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DrugstoreSystem.Infrastructure.Persistence;

public class DrugstoreDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public DrugstoreDbContext(DbContextOptions<DrugstoreDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Medicine> Medicines => Set<Medicine>();
    public DbSet<MedicineSynonym> MedicineSynonyms => Set<MedicineSynonym>();
    public DbSet<Pharmacy> Pharmacies => Set<Pharmacy>();
    public DbSet<PharmacyMedicine> PharmacyMedicines => Set<PharmacyMedicine>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DrugstoreDbContext).Assembly);
    }
}

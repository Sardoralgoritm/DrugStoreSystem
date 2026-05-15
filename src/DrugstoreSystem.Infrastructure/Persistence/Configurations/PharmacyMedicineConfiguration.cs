using DrugstoreSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrugstoreSystem.Infrastructure.Persistence.Configurations;

public class PharmacyMedicineConfiguration : IEntityTypeConfiguration<PharmacyMedicine>
{
    public void Configure(EntityTypeBuilder<PharmacyMedicine> builder)
    {
        builder.HasKey(pm => pm.Id);
        builder.Property(pm => pm.Price).HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(pm => pm.Quantity).IsRequired();
        builder.Property(pm => pm.UpdatedAt).IsRequired();

        builder.HasIndex(pm => new { pm.PharmacyId, pm.MedicineId }).IsUnique()
            .HasDatabaseName("ux_pharmacy_medicine");

        builder.HasOne(pm => pm.Pharmacy)
            .WithMany(p => p.PharmacyMedicines)
            .HasForeignKey(pm => pm.PharmacyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pm => pm.Medicine)
            .WithMany(m => m.PharmacyMedicines)
            .HasForeignKey(pm => pm.MedicineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

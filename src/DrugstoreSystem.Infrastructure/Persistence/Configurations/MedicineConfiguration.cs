using DrugstoreSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrugstoreSystem.Infrastructure.Persistence.Configurations;

public class MedicineConfiguration : IEntityTypeConfiguration<Medicine>
{
    public void Configure(EntityTypeBuilder<Medicine> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).HasMaxLength(200).IsRequired();
        builder.Property(m => m.GenericName).HasMaxLength(200);
        builder.Property(m => m.NameRu).HasMaxLength(200);
        builder.Property(m => m.DosageForm).HasConversion<string>().HasMaxLength(100);
        builder.Property(m => m.Manufacturer).HasMaxLength(200);
        builder.Property(m => m.Description).HasColumnType("text");
        builder.Property(m => m.CreatedAt).IsRequired();

        builder.HasOne(m => m.Category)
            .WithMany(c => c.Medicines)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasMany(m => m.Synonyms)
            .WithOne(s => s.Medicine)
            .HasForeignKey(s => s.MedicineId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

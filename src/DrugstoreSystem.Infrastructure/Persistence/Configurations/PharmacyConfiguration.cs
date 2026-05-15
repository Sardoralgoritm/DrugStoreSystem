using DrugstoreSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrugstoreSystem.Infrastructure.Persistence.Configurations;

public class PharmacyConfiguration : IEntityTypeConfiguration<Pharmacy>
{
    public void Configure(EntityTypeBuilder<Pharmacy> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Address).HasMaxLength(500).IsRequired();
        builder.Property(p => p.Phone).HasMaxLength(50);
        builder.Property(p => p.WorkingHours).HasMaxLength(200);
        builder.Property(p => p.IsActive).IsRequired();
        builder.Property(p => p.IsVerified).IsRequired();
        builder.Property(p => p.CreatedAt).IsRequired();
    }
}

using DrugstoreSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DrugstoreSystem.Infrastructure.Persistence.Configurations;

public class MedicineSynonymConfiguration : IEntityTypeConfiguration<MedicineSynonym>
{
    public void Configure(EntityTypeBuilder<MedicineSynonym> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Synonym).HasMaxLength(200).IsRequired();
    }
}

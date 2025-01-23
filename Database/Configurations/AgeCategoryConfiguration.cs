using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class AgeCategoryConfiguration
    : IEntityTypeConfiguration<AgeCategory>
{
    public void Configure(EntityTypeBuilder<AgeCategory> builder)
    {
        builder.ToTable("tblAgeCategorys").HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasMany<Film>()
            .WithOne()
            .HasForeignKey(x => x.AgeCategoryId);
        builder.HasMany<Serie>()
            .WithOne()
            .HasForeignKey(x => x.AgeCategoryId);
    }
}
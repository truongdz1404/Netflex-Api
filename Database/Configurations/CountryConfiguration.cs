using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class CountryConfiguration
    : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("tblCountrys").HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasMany<FilmCountry>()
            .WithOne()
            .HasForeignKey(fc => fc.CountryId)
            .IsRequired();
        builder.HasMany<SerieCountry>()
            .WithOne()
            .HasForeignKey(sc => sc.CountryId)
            .IsRequired();
    }
}
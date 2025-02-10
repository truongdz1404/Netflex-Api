using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FilmCountryConfiguration
    : IEntityTypeConfiguration<FilmCountry>
{
    public void Configure(EntityTypeBuilder<FilmCountry> builder)
    {
        builder.ToTable("tblFilmCountries").HasKey(x => new { x.FilmId, x.CountryId });
    }
}
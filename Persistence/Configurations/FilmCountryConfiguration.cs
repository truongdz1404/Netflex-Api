using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class FilmCountryConfiguration
    : IEntityTypeConfiguration<FilmCountry>
{
    public void Configure(EntityTypeBuilder<FilmCountry> builder)
    {
        builder.ToTable("tblFilmCountrys").HasKey(x => new { x.FilmId, x.CountryId });
    }
}
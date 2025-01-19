using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class SerieCountryConfiguration
    : IEntityTypeConfiguration<SerieCountry>
{
    public void Configure(EntityTypeBuilder<SerieCountry> builder)
    {
        builder.ToTable("tblSerieCountrys").HasKey(x => new { x.SerieId, x.CountryId });
    }
}
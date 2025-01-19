using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class SerieGenreConfiguration
    : IEntityTypeConfiguration<SerieGenre>
{
    public void Configure(EntityTypeBuilder<SerieGenre> builder)
    {
        builder.ToTable("tblSerieGenres").HasKey(x => new { x.SerieId, x.GenreId });
    }
}
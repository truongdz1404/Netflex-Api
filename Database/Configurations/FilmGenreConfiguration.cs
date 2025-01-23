using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FilmGenreConfiguration
    : IEntityTypeConfiguration<FilmGenre>
{
    public void Configure(EntityTypeBuilder<FilmGenre> builder)
    {
        builder.ToTable("tblFilmGenres").HasKey(x => new { x.FilmId, x.GenreId });
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class SerieGenreConfiguration
    : IEntityTypeConfiguration<SerieGenre>
{
    public void Configure(EntityTypeBuilder<SerieGenre> builder)
    {
        builder.ToTable("tblSerieGenres").HasKey(x => new { x.SerieId, x.GenreId });
        builder.HasOne<Serie>()
            .WithMany(s => s.SerieGenres)
            .HasForeignKey(sc => sc.SerieId)
            .IsRequired();
    }
}
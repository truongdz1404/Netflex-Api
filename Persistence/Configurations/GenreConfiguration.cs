using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class GenreConfiguration
    : IEntityTypeConfiguration<Genre>
{
    public void Configure(EntityTypeBuilder<Genre> builder)
    {
        builder.ToTable("tblGenres").HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        builder.HasMany<FilmGenre>()
            .WithOne()
            .HasForeignKey(x => x.GenreId)
            .IsRequired();
        builder.HasMany<SerieGenre>()
            .WithOne()
            .HasForeignKey(x => x.GenreId)
            .IsRequired();
    }
}
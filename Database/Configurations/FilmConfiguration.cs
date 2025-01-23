using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FilmConfiguration
    : IEntityTypeConfiguration<Film>
{
    public void Configure(EntityTypeBuilder<Film> builder)
    {
        builder.ToTable("tblFilms").HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Path).IsRequired();
        builder.Property(x => x.About).HasMaxLength(1000);
        builder.Property(x => x.Poster).HasMaxLength(200);
        builder.Property(x => x.ProductionYear);
        builder.Property(x => x.HowLong);
        builder.Property(x => x.CreatedAt);

        builder.HasMany<Follow>()
            .WithOne()
            .HasForeignKey(f => f.FilmId);

        builder.HasMany<FilmCountry>()
            .WithOne()
            .HasForeignKey(fc => fc.FilmId)
            .IsRequired();
        builder.HasMany<FilmActor>()
            .WithOne()
            .HasForeignKey(fa => fa.FilmId)
            .IsRequired();
        builder.HasMany<FilmGenre>()
            .WithOne()
            .HasForeignKey(fg => fg.FilmId)
            .IsRequired();
        builder.HasMany<Review>()
            .WithOne()
            .HasForeignKey(r => r.FilmId);
    }
}
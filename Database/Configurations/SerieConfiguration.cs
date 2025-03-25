using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class SerieConfiguration
    : IEntityTypeConfiguration<Serie>
{
    public void Configure(EntityTypeBuilder<Serie> builder)
    {
        builder.ToTable("tblSeries").HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
        builder.Property(x => x.ProductionYear);
        builder.Property(x => x.About).HasMaxLength(1000);
        builder.Property(x => x.Poster).HasMaxLength(200);

        builder.HasMany<Follow>()
            .WithOne()
            .HasForeignKey(f => f.SerieId);

        builder.HasMany<Episode>()
            .WithOne()
            .HasForeignKey(e => e.SerieId)
            .IsRequired();
        builder.HasMany(s => s.SerieActors)
            .WithOne()
            .HasForeignKey(sa => sa.SerieId)
            .IsRequired();
        builder.HasMany(sg => sg.SerieGenres)
            .WithOne()
            .HasForeignKey(sg => sg.SerieId)
            .IsRequired();
        builder.HasMany(sc => sc.SerieCountries)
            .WithOne()
            .HasForeignKey(sc => sc.SerieId)
            .IsRequired();

        builder.HasMany<Review>()
            .WithOne()
            .HasForeignKey(r => r.SerieId);
    }
}
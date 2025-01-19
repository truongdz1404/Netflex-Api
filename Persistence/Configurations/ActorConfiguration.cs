using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class ActorConfiguration
    : IEntityTypeConfiguration<Actor>
{
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
        builder.ToTable("tblActors").HasKey(x => x.Id);
        builder.Property(x => x.About).HasMaxLength(1000);
        builder.Property(x => x.Photo).HasMaxLength(200);

        builder.HasMany<FilmActor>()
            .WithOne()
            .HasForeignKey(fa => fa.ActorId)
            .IsRequired();

        builder.HasMany<SerieActor>()
            .WithOne()
            .HasForeignKey(sa => sa.ActorId)
            .IsRequired();
    }
}
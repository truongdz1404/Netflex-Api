using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FilmActorConfiguration
    : IEntityTypeConfiguration<FilmActor>
{
    public void Configure(EntityTypeBuilder<FilmActor> builder)
    {
        builder.ToTable("tblFilmActors").HasKey(x => new { x.FilmId, x.ActorId });
    }
}
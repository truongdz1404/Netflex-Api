using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class SerieActorConfiguration
    : IEntityTypeConfiguration<SerieActor>
{
    public void Configure(EntityTypeBuilder<SerieActor> builder)
    {
        builder.ToTable("tblSerieActors").HasKey(x => new { x.SerieId, x.ActorId });
    }
}
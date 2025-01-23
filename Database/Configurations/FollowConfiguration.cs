using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FollowConfiguration
    : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("tblFollows").HasKey(x => new { x.FollowerId, x.FilmId, x.SerieId });
        builder.Property(x => x.FollowedAt).IsRequired();
    }
}
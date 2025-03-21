using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class FollowConfiguration
    : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ToTable("tblFollows").HasKey(x => x.Id);
        builder.Property(x => x.FollowerId).IsRequired();
        builder.Property(x => x.FollowedAt).IsRequired();
        builder.Property(x => x.FilmId);
        builder.Property(x => x.SerieId);
    }
}
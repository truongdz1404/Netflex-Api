using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;


public class EpisodeConfiguration
    : IEntityTypeConfiguration<Episode>
{
    public void Configure(EntityTypeBuilder<Episode> builder)
    {
        builder.ToTable("tblEpisodes").HasKey(x => x.Id);
        builder.Property(x => x.Title).HasMaxLength(100).IsRequired();
        builder.Property(x => x.About).HasMaxLength(1000);
        builder.Property(x => x.Path).HasMaxLength(2000);
        builder.Property(x => x.HowLong);
    }
}
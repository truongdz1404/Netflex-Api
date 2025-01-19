using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class SerieFollowConfiguration
    : IEntityTypeConfiguration<SerieFollow>
{
    public void Configure(EntityTypeBuilder<SerieFollow> builder)
    {
        builder.ToTable("tblSerieFollows").HasKey(x => new { x.SerieId, x.UserId });
    }
}
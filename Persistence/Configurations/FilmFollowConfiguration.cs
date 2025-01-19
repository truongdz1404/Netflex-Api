using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class FilmFollowConfiguration
    : IEntityTypeConfiguration<FilmFollow>
{
    public void Configure(EntityTypeBuilder<FilmFollow> builder)
    {
        builder.ToTable("tblFilmFollows").HasKey(x => new { x.FilmId, x.UserId });
    }
}
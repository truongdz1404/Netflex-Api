using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class ReviewConfiguration
    : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("tblReviews").HasKey(x => new { x.FilmId, x.CreaterId, x.SerieId });
        builder.Property(x => x.Rating).IsRequired();
    }
}
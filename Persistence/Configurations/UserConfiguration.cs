using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Persistence.Configurations;

public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("tblUsers");
        builder.HasMany<FilmFollow>()
            .WithOne()
            .HasForeignKey(f => f.UserId)
            .IsRequired();
        builder.HasMany<SerieFollow>()
            .WithOne()
            .HasForeignKey(s => s.UserId)
            .IsRequired();
        builder.HasMany<Review>()
            .WithOne()
            .HasForeignKey(r => r.CreaterId)
            .IsRequired();

        builder.HasMany<Blog>()
            .WithOne()
            .HasForeignKey(b => b.CreaterId)
            .IsRequired();

        builder.HasMany<UserNotification>()
            .WithOne()
            .HasForeignKey(n => n.UserId)
            .IsRequired();
    }
}
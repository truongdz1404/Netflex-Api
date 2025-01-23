using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Netflex.Database.Configurations;

public class NotificationConfiguration
    : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("tblNotifications").HasKey(x => x.Id);
        builder.Property(x => x.Content).HasMaxLength(1000).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(100).IsRequired();
        builder.Property(x => x.CreatedAt);
        builder.HasMany<UserNotification>()
            .WithOne()
            .HasForeignKey(un => un.NotificationId)
            .IsRequired();
    }
}
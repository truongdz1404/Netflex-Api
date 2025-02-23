using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Netflex.Database;
using Netflex.Models;

namespace Netflex.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new NetflexContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<NetflexContext>>()))
            {
                // Kiểm tra và thêm dữ liệu cho bảng Users trước
                if (!context.TblUsers.Any())
                {
                    context.TblUsers.AddRange(
                        new TblUser
                        {
                            Id = "user1",
                            UserName = "john_doe",
                            NormalizedUserName = "JOHN_DOE",
                            Email = "john@example.com",
                            NormalizedEmail = "JOHN@EXAMPLE.COM",
                            EmailConfirmed = true,
                            PasswordHash = null, // Có thể hash mật khẩu nếu cần
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "0123456789",
                            PhoneNumberConfirmed = true,
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                            AccessFailedCount = 0
                        },
                        new TblUser
                        {
                            Id = "user2",
                            UserName = "jane_smith",
                            NormalizedUserName = "JANE_SMITH",
                            Email = "jane@example.com",
                            NormalizedEmail = "JANE@EXAMPLE.COM",
                            EmailConfirmed = true,
                            PasswordHash = null,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "0987654321",
                            PhoneNumberConfirmed = true,
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                            AccessFailedCount = 0
                        },
                        new TblUser
                        {
                            Id = "user3",
                            UserName = "alex_brown",
                            NormalizedUserName = "ALEX_BROWN",
                            Email = "alex@example.com",
                            NormalizedEmail = "ALEX@EXAMPLE.COM",
                            EmailConfirmed = true,
                            PasswordHash = null,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "0933221100",
                            PhoneNumberConfirmed = true,
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                            AccessFailedCount = 0
                        },
                        new TblUser
                        {
                            Id = "user4",
                            UserName = "chris_lee",
                            NormalizedUserName = "CHRIS_LEE",
                            Email = "chris@example.com",
                            NormalizedEmail = "CHRIS@EXAMPLE.COM",
                            EmailConfirmed = true,
                            PasswordHash = null,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            PhoneNumber = "0944332211",
                            PhoneNumberConfirmed = true,
                            LockoutEnabled = false,
                            TwoFactorEnabled = false,
                            AccessFailedCount = 0
                        }
                    );
                    context.SaveChanges();
                }

                // Kiểm tra và thêm dữ liệu cho bảng Blogs
                if (!context.TblBlogs.Any())
                {
                    context.TblBlogs.AddRange(
                        new TblBlog
                        {
                            Id = Guid.NewGuid(),
                            Title = "Khám phá thế giới điện ảnh",
                            Content = "Bài viết này sẽ đưa bạn vào hành trình khám phá những bộ phim kinh điển nhất.",
                            Thumbnail = "images/movies.jpg",
                            CreatedAt = DateTime.UtcNow,
                            CreaterId = "user1"
                        },
                        new TblBlog
                        {
                            Id = Guid.NewGuid(),
                            Title = "Bí quyết tạo nội dung hấp dẫn",
                            Content = "Những mẹo nhỏ giúp bạn viết bài blog thu hút độc giả.",
                            Thumbnail = "images/writing_tips.jpg",
                            CreatedAt = DateTime.UtcNow,
                            CreaterId = "user2"
                        },
                        new TblBlog
                        {
                            Id = Guid.NewGuid(),
                            Title = "Top 10 bộ phim đáng xem nhất năm",
                            Content = "Danh sách những bộ phim không thể bỏ lỡ trong năm nay.",
                            Thumbnail = "images/top_movies.jpg",
                            CreatedAt = DateTime.UtcNow,
                            CreaterId = "user3"
                        },
                        new TblBlog
                        {
                            Id = Guid.NewGuid(),
                            Title = "Hành trình làm phim của đạo diễn nổi tiếng",
                            Content = "Câu chuyện đầy cảm hứng về những nhà làm phim hàng đầu thế giới.",
                            Thumbnail = "images/director_story.jpg",
                            CreatedAt = DateTime.UtcNow,
                            CreaterId = "user4"
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}

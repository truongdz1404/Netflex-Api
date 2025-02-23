using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Netflex.Models;

public partial class NetflexContext : DbContext
{
    public NetflexContext()
    {
    }

    public NetflexContext(DbContextOptions<NetflexContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblActor> TblActors { get; set; }

    public virtual DbSet<TblAgeCategory> TblAgeCategorys { get; set; }

    public virtual DbSet<TblBlog> TblBlogs { get; set; }

    public virtual DbSet<TblCountry> TblCountrys { get; set; }

    public virtual DbSet<TblEpisode> TblEpisodes { get; set; }

    public virtual DbSet<TblFilm> TblFilms { get; set; }

    public virtual DbSet<TblFollow> TblFollows { get; set; }

    public virtual DbSet<TblGenre> TblGenres { get; set; }

    public virtual DbSet<TblNotification> TblNotifications { get; set; }

    public virtual DbSet<TblReview> TblReviews { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRoleClaim> TblRoleClaims { get; set; }

    public virtual DbSet<TblSeries> TblSeries { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserClaim> TblUserClaims { get; set; }

    public virtual DbSet<TblUserLogin> TblUserLogins { get; set; }

    public virtual DbSet<TblUserToken> TblUserTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblActor>(entity =>
        {
            entity.ToTable("tblActors");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.About).HasMaxLength(1000);
            entity.Property(e => e.Photo).HasMaxLength(200);
        });

        modelBuilder.Entity<TblAgeCategory>(entity =>
        {
            entity.ToTable("tblAgeCategorys");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TblBlog>(entity =>
        {
            entity.ToTable("tblBlogs");

            entity.HasIndex(e => e.CreaterId, "IX_tblBlogs_CreaterId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).HasMaxLength(3000);
            entity.Property(e => e.Thumbnail).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Creater).WithMany(p => p.TblBlogs).HasForeignKey(d => d.CreaterId);
        });

        modelBuilder.Entity<TblCountry>(entity =>
        {
            entity.ToTable("tblCountrys");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TblEpisode>(entity =>
        {
            entity.ToTable("tblEpisodes");

            entity.HasIndex(e => e.SerieId, "IX_tblEpisodes_SerieId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.About).HasMaxLength(1000);
            entity.Property(e => e.Path).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Serie).WithMany(p => p.TblEpisodes).HasForeignKey(d => d.SerieId);
        });

        modelBuilder.Entity<TblFilm>(entity =>
        {
            entity.ToTable("tblFilms");

            entity.HasIndex(e => e.AgeCategoryId, "IX_tblFilms_AgeCategoryId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.About).HasMaxLength(1000);
            entity.Property(e => e.Poster).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.AgeCategory).WithMany(p => p.TblFilms).HasForeignKey(d => d.AgeCategoryId);

            entity.HasMany(d => d.Actors).WithMany(p => p.Films)
                .UsingEntity<Dictionary<string, object>>(
                    "TblFilmActor",
                    r => r.HasOne<TblActor>().WithMany().HasForeignKey("ActorId"),
                    l => l.HasOne<TblFilm>().WithMany().HasForeignKey("FilmId"),
                    j =>
                    {
                        j.HasKey("FilmId", "ActorId");
                        j.ToTable("tblFilmActors");
                        j.HasIndex(new[] { "ActorId" }, "IX_tblFilmActors_ActorId");
                    });

            entity.HasMany(d => d.Countries).WithMany(p => p.Films)
                .UsingEntity<Dictionary<string, object>>(
                    "TblFilmCountry",
                    r => r.HasOne<TblCountry>().WithMany().HasForeignKey("CountryId"),
                    l => l.HasOne<TblFilm>().WithMany().HasForeignKey("FilmId"),
                    j =>
                    {
                        j.HasKey("FilmId", "CountryId");
                        j.ToTable("tblFilmCountries");
                        j.HasIndex(new[] { "CountryId" }, "IX_tblFilmCountries_CountryId");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Films)
                .UsingEntity<Dictionary<string, object>>(
                    "TblFilmGenre",
                    r => r.HasOne<TblGenre>().WithMany().HasForeignKey("GenreId"),
                    l => l.HasOne<TblFilm>().WithMany().HasForeignKey("FilmId"),
                    j =>
                    {
                        j.HasKey("FilmId", "GenreId");
                        j.ToTable("tblFilmGenres");
                        j.HasIndex(new[] { "GenreId" }, "IX_tblFilmGenres_GenreId");
                    });
        });

        modelBuilder.Entity<TblFollow>(entity =>
        {
            entity.HasKey(e => new { e.FollowerId, e.FilmId, e.SerieId });

            entity.ToTable("tblFollows");

            entity.HasIndex(e => e.FilmId, "IX_tblFollows_FilmId");

            entity.HasIndex(e => e.SerieId, "IX_tblFollows_SerieId");

            entity.HasOne(d => d.Film).WithMany(p => p.TblFollows).HasForeignKey(d => d.FilmId);

            entity.HasOne(d => d.Follower).WithMany(p => p.TblFollows).HasForeignKey(d => d.FollowerId);

            entity.HasOne(d => d.Serie).WithMany(p => p.TblFollows).HasForeignKey(d => d.SerieId);
        });

        modelBuilder.Entity<TblGenre>(entity =>
        {
            entity.ToTable("tblGenres");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TblNotification>(entity =>
        {
            entity.ToTable("tblNotifications");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(100);
        });

        modelBuilder.Entity<TblReview>(entity =>
        {
            entity.HasKey(e => new { e.FilmId, e.CreaterId, e.SerieId });

            entity.ToTable("tblReviews");

            entity.HasIndex(e => e.CreaterId, "IX_tblReviews_CreaterId");

            entity.HasIndex(e => e.SerieId, "IX_tblReviews_SerieId");

            entity.HasOne(d => d.Creater).WithMany(p => p.TblReviews).HasForeignKey(d => d.CreaterId);

            entity.HasOne(d => d.Film).WithMany(p => p.TblReviews).HasForeignKey(d => d.FilmId);

            entity.HasOne(d => d.Serie).WithMany(p => p.TblReviews).HasForeignKey(d => d.SerieId);
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("tblRoles");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<TblRoleClaim>(entity =>
        {
            entity.ToTable("tblRoleClaims");

            entity.HasIndex(e => e.RoleId, "IX_tblRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRoleClaims).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<TblSeries>(entity =>
        {
            entity.ToTable("tblSeries");

            entity.HasIndex(e => e.AgeCategoryId, "IX_tblSeries_AgeCategoryId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.About).HasMaxLength(1000);
            entity.Property(e => e.Poster).HasMaxLength(200);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.AgeCategory).WithMany(p => p.TblSeries).HasForeignKey(d => d.AgeCategoryId);

            entity.HasMany(d => d.Actors).WithMany(p => p.Series)
                .UsingEntity<Dictionary<string, object>>(
                    "TblSerieActor",
                    r => r.HasOne<TblActor>().WithMany().HasForeignKey("ActorId"),
                    l => l.HasOne<TblSeries>().WithMany().HasForeignKey("SerieId"),
                    j =>
                    {
                        j.HasKey("SerieId", "ActorId");
                        j.ToTable("tblSerieActors");
                        j.HasIndex(new[] { "ActorId" }, "IX_tblSerieActors_ActorId");
                    });

            entity.HasMany(d => d.Countries).WithMany(p => p.Series)
                .UsingEntity<Dictionary<string, object>>(
                    "TblSerieCountry",
                    r => r.HasOne<TblCountry>().WithMany().HasForeignKey("CountryId"),
                    l => l.HasOne<TblSeries>().WithMany().HasForeignKey("SerieId"),
                    j =>
                    {
                        j.HasKey("SerieId", "CountryId");
                        j.ToTable("tblSerieCountries");
                        j.HasIndex(new[] { "CountryId" }, "IX_tblSerieCountries_CountryId");
                    });

            entity.HasMany(d => d.Genres).WithMany(p => p.Series)
                .UsingEntity<Dictionary<string, object>>(
                    "TblSerieGenre",
                    r => r.HasOne<TblGenre>().WithMany().HasForeignKey("GenreId"),
                    l => l.HasOne<TblSeries>().WithMany().HasForeignKey("SerieId"),
                    j =>
                    {
                        j.HasKey("SerieId", "GenreId");
                        j.ToTable("tblSerieGenres");
                        j.HasIndex(new[] { "GenreId" }, "IX_tblSerieGenres_GenreId");
                    });
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.ToTable("tblUsers");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Notifications).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "TblUserNotification",
                    r => r.HasOne<TblNotification>().WithMany().HasForeignKey("NotificationId"),
                    l => l.HasOne<TblUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "NotificationId");
                        j.ToTable("tblUserNotifications");
                        j.HasIndex(new[] { "NotificationId" }, "IX_tblUserNotifications_NotificationId");
                    });

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "TblUserRole",
                    r => r.HasOne<TblRole>().WithMany().HasForeignKey("RoleId"),
                    l => l.HasOne<TblUser>().WithMany().HasForeignKey("UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.ToTable("tblUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "IX_tblUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<TblUserClaim>(entity =>
        {
            entity.ToTable("tblUserClaims");

            entity.HasIndex(e => e.UserId, "IX_tblUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserClaims).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<TblUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            entity.ToTable("tblUserLogins");

            entity.HasIndex(e => e.UserId, "IX_tblUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserLogins).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<TblUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

            entity.ToTable("tblUserTokens");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserTokens).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

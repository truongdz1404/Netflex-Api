//using System;
//using System.Collections.Generic;
//using Microsoft.EntityFrameworkCore;

//namespace Netflex.Models;

//public partial class NetflexContext : DbContext
//{
//    public NetflexContext()
//    {
//    }

//    public NetflexContext(DbContextOptions<NetflexContext> options)
//        : base(options)
//    {
//    }

    

//    public virtual DbSet<BlogViewModel> TblBlogs { get; set; }

    
//    public virtual DbSet<UserViewModel> TblUsers { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:Database");

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        modelBuilder.Entity<BlogViewModel>(entity =>
//        {
//            entity.ToTable("tblBlogs");

//            entity.HasIndex(e => e.CreaterId, "IX_tblBlogs_CreaterId");

//            entity.Property(e => e.Id).ValueGeneratedNever();
//            entity.Property(e => e.Content).HasMaxLength(3000);
//            entity.Property(e => e.Thumbnail).HasMaxLength(200);
//            entity.Property(e => e.Title).HasMaxLength(200);

//            entity.HasOne(d => d.Creater).WithMany(p => p.TblBlogs).HasForeignKey(d => d.CreaterId);
//        });

//        modelBuilder.Entity<UserViewModel>(entity =>
//        {
//            entity.ToTable("tblUsers");

//            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

//            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
//                .IsUnique()
//                .HasFilter("([NormalizedUserName] IS NOT NULL)");

//            entity.Property(e => e.Email).HasMaxLength(256);
//            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
//            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
//            entity.Property(e => e.UserName).HasMaxLength(256);
//        });
//        OnModelCreatingPartial(modelBuilder);
//    }

//    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
//}

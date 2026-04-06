using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BANK_TEST.Database.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblBlog> TblBlogs { get; set; }

    public virtual DbSet<TransferAmt> TransferAmts { get; set; }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-BPF6HTF\\SQL2022;Initial Catalog=DotNetTrainingBatch5;User Id=sa;Password=p@ssw0rd;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblBlog>(entity =>
        {
            entity.HasKey(e => e.BlogId);

            entity.ToTable("TBL_BLOG");

            entity.Property(e => e.BlogAuthor).HasMaxLength(50);
            entity.Property(e => e.BlogTitle).HasMaxLength(50);
        });

        modelBuilder.Entity<TransferAmt>(entity =>
        {
            entity.ToTable("TRANSFER_AMT");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("CREATED_DATE");
            entity.Property(e => e.FrMobileNo)
                .HasMaxLength(10)
                .HasColumnName("FR_MOBILE_NO");
            entity.Property(e => e.ToMobileNo)
                .HasMaxLength(10)
                .HasColumnName("TO_MOBILE_NO");
        });

        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("USER_PROFILE");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("BALANCE");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("datetime")
                .HasColumnName("CREATED_DATE");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");
            entity.Property(e => e.MobileNo)
                .HasMaxLength(10)
                .HasColumnName("MOBILE_NO");
            entity.Property(e => e.Pin)
                .HasMaxLength(10)
                .HasColumnName("PIN");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

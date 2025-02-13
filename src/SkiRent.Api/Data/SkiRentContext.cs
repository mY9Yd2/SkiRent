using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data;

public partial class SkiRentContext : DbContext
{
    public SkiRentContext(DbContextOptions<SkiRentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.UserRole).HasDefaultValueSql("'customer'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

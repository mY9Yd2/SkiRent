using Microsoft.EntityFrameworkCore;

using SkiRent.Api.Data.Models;

namespace SkiRent.Api.Data;

public partial class SkiRentContext : DbContext
{
    public SkiRentContext(DbContextOptions<SkiRentContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<BookingItem> Bookingitems { get; set; }

    public virtual DbSet<Equipment> Equipments { get; set; }

    public virtual DbSet<EquipmentCategory> Equipmentcategories { get; set; }

    public virtual DbSet<EquipmentImage> Equipmentimages { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.Status).HasDefaultValueSql("'Pending'");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_ibfk_1");
        });

        modelBuilder.Entity<BookingItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingitems).HasConstraintName("bookingitems_ibfk_1");

            entity.HasOne(d => d.Equipment).WithMany(p => p.Bookingitems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingitems_ibfk_2");
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.Category).WithMany(p => p.Equipment)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("equipments_ibfk_1");

            entity.HasOne(d => d.MainImage).WithMany(p => p.Equipment)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("equipments_ibfk_2");
        });

        modelBuilder.Entity<EquipmentCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
        });

        modelBuilder.Entity<EquipmentImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasOne(d => d.EquipmentNavigation).WithMany(p => p.Equipmentimages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("equipmentimages_ibfk_1");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("current_timestamp()");

            entity.HasOne(d => d.Booking).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("invoices_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Invoices)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("invoices_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.UserRole).HasDefaultValueSql("'Customer'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

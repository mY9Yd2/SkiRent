﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("bookings")]
[Index("PaymentId", Name = "PaymentId", IsUnique = true)]
[Index("UserId", Name = "UserId")]
public partial class Booking
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int UserId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    public Guid PaymentId { get; set; }

    [Column(TypeName = "enum('Pending','Paid','InDelivery','Received','Cancelled','Returned')")]
    public string Status { get; set; } = null!;

    [Column(TypeName = "timestamp")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTimeOffset UpdatedAt { get; set; }

    [StringLength(70)]
    public string FullName { get; set; } = null!;

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(20)]
    public string? MobilePhoneNumber { get; set; }

    [StringLength(50)]
    public string AddressCountry { get; set; } = null!;

    [StringLength(10)]
    public string AddressPostalCode { get; set; } = null!;

    [StringLength(50)]
    public string AddressCity { get; set; } = null!;

    [StringLength(60)]
    public string AddressStreet { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();

    [InverseProperty("Booking")]
    public virtual Invoice? Invoice { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User User { get; set; } = null!;
}

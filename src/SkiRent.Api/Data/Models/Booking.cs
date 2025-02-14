using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("bookings")]
[Index("EquipmentId", Name = "EquipmentId")]
[Index("UserId", Name = "UserId")]
public partial class Booking
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int UserId { get; set; }

    [Column(TypeName = "int(11)")]
    public int EquipmentId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [Precision(10, 2)]
    public decimal TotalPrice { get; set; }

    [Column(TypeName = "enum('pending','confirmed','cancelled','returned')")]
    public string Status { get; set; } = null!;

    [ForeignKey("EquipmentId")]
    [InverseProperty("Bookings")]
    public virtual Equipment Equipment { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User User { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("bookingitems")]
[Index("BookingId", "EquipmentId", Name = "BookingId", IsUnique = true)]
public partial class BookingItem
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int BookingId { get; set; }

    [Column(TypeName = "int(11)")]
    public int EquipmentId { get; set; }

    [StringLength(100)]
    public string NameAtBooking { get; set; } = null!;

    [Precision(10, 2)]
    public decimal PriceAtBooking { get; set; }

    [Column(TypeName = "int(11)")]
    public int Quantity { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingItems")]
    public virtual Booking Booking { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("bookingitems")]
[Index("BookingId", Name = "BookingId")]
[Index("EquipmentId", Name = "EquipmentId")]
public partial class BookingItem
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int BookingId { get; set; }

    [Column(TypeName = "int(11)")]
    public int EquipmentId { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Bookingitems")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("EquipmentId")]
    [InverseProperty("Bookingitems")]
    public virtual Equipment Equipment { get; set; } = null!;
}

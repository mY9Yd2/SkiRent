using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("bookingitems")]
[Index("BookingId", "EquipmentId", Name = "BookingId", IsUnique = true)]
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

    [Column(TypeName = "int(11)")]
    public int Quantity { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("BookingItems")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("EquipmentId")]
    [InverseProperty("BookingItems")]
    public virtual Equipment Equipment { get; set; } = null!;
}

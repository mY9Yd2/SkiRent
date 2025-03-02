using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("invoices")]
[Index("BookingId", Name = "BookingId", IsUnique = true)]
[Index("UserId", Name = "UserId")]
public partial class Invoice
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int? UserId { get; set; }

    [Column(TypeName = "int(11)")]
    public int? BookingId { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTimeOffset CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Invoice")]
    public virtual Booking? Booking { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Invoices")]
    public virtual User? User { get; set; }
}

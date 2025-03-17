using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkiRent.Api.Data.Models;

[Table("equipmentimages")]
public partial class EquipmentImage
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(255)]
    public string? DisplayName { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTimeOffset CreatedAt { get; set; }

    [Column(TypeName = "timestamp")]
    public DateTimeOffset UpdatedAt { get; set; }

    [InverseProperty("MainImage")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}

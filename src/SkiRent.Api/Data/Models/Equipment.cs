using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("equipments")]
[Index("CategoryId", Name = "CategoryId")]
[Index("MainImageId", Name = "MainImageId")]
public partial class Equipment
{
    [Key]
    [Column(TypeName = "int(11)")]
    public int Id { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "text")]
    public string? Description { get; set; }

    [Column(TypeName = "int(11)")]
    public int CategoryId { get; set; }

    public Guid? MainImageId { get; set; }

    [Precision(10, 2)]
    public decimal PricePerDay { get; set; }

    [Column(TypeName = "int(11)")]
    public int AvailableQuantity { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Equipment")]
    public virtual EquipmentCategory Category { get; set; } = null!;

    [ForeignKey("MainImageId")]
    [InverseProperty("Equipment")]
    public virtual EquipmentImage? MainImage { get; set; }
}

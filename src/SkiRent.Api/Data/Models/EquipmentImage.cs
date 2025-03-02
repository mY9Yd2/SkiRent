using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace SkiRent.Api.Data.Models;

[Table("equipmentimages")]
[Index("EquipmentId", "DisplayName", Name = "EquipmentId", IsUnique = true)]
public partial class EquipmentImage
{
    [Key]
    public Guid Id { get; set; }

    [Column(TypeName = "int(11)")]
    public int EquipmentId { get; set; }

    public string DisplayName { get; set; } = null!;

    [InverseProperty("MainImage")]
    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    [ForeignKey("EquipmentId")]
    [InverseProperty("EquipmentImages")]
    public virtual Equipment EquipmentNavigation { get; set; } = null!;
}
